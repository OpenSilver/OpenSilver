
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if BRIDGE
using Bridge;
#else
using JSIL.Meta;
#endif

#if !BUILDINGDOCUMENTATION && !CSHTML5NETSTANDARD
using DotNetBrowser;
#endif

using CSHTML5.Types;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5
{
    internal static class INTERNAL_InteropImplementation
    {
        static bool IsJavaScriptCSharpInteropSetUp;
        static Dictionary<int, Delegate> CallbacksDictionary = new Dictionary<int, Delegate>();
        static Random RandomGenerator = new Random();

        static INTERNAL_InteropImplementation()
        {
            Application.INTERNAL_Reloaded += (sender, e) =>
            {
                IsJavaScriptCSharpInteropSetUp = false;
            };
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("null")]
#else
        [Bridge.Template("null")]
#endif
        internal static object ExecuteJavaScript_SimulatorImplementation(string javascript, bool runAsynchronously, bool noImpactOnPendingJSCode = false, params object[] variables)
        {
#if !BUILDINGDOCUMENTATION
            //---------------
            // Due to the fact that it is not possible to pass JavaScript objects between the simulator JavaScript context
            // and the C# context, we store the JavaScript objects in a global dictionary inside the JavaScript context.
            // This dictionary is named "jsSimulatorObjectReferences". It associates a unique integer ID to each JavaScript
            // object. In C# we only manipulate those IDs by manipulating instances of the "JSObjectReference" class.
            // When we need to re-use those JavaScript objects, the C# code passes to the JavaScript context the ID
            // of the object, so that the JavaScript code can retrieve the JavaScript object instance by using the 
            // aforementioned dictionary.
            //---------------

            // Verify the arguments:
            if (noImpactOnPendingJSCode && runAsynchronously)
                throw new ArgumentException("You cannot set both 'noImpactOnPendingJSCode' and 'runAsynchronously' to True. The 'noImpactOnPendingJSCode' only has meaning when running synchronously.");

            // Make sure the JS to C# interop is set up:
            if (!IsJavaScriptCSharpInteropSetUp)
            {
#if !CSHTML5NETSTANDARD
                // Adding a property to the JavaScript "window" object:
                JSObject jsWindow = (JSObject)INTERNAL_HtmlDomManager.ExecuteJavaScriptWithResult("window");
                jsWindow.SetProperty("onCallBack", new OnCallBack(CallbacksDictionary));
#else
                OnCallBack.SetCallbacksDictionary(CallbacksDictionary);
#endif
                IsJavaScriptCSharpInteropSetUp = true;
            }

            // If the javascript code has references to previously obtained JavaScript objects, we replace those references with calls to the "document.jsSimulatorObjectReferences" dictionary.
            for (int i = variables.Length - 1; i >= 0; i--) // Note: we iterate in reverse order because, when we replace ""$" + i.ToString()", we need to replace "$10" before replacing "$1", otherwise it thinks that "$10" is "$1" followed by the number "0". To reproduce the issue, call "ExecuteJavaScript" passing 10 arguments and using "$10".
            {
                var variable = variables[i];
                if (variable is INTERNAL_JSObjectReference)
                {
                    //----------------------
                    // JS Object References
                    //----------------------

                    var jsObjectReference = (INTERNAL_JSObjectReference)variable;
                    string jsCodeForAccessingTheObject;

                    if (jsObjectReference.IsArray)
                        jsCodeForAccessingTheObject = string.Format(@"document.jsSimulatorObjectReferences[""{0}""][{1}]", jsObjectReference.ReferenceId, jsObjectReference.ArrayIndex);
                    else
                        jsCodeForAccessingTheObject = string.Format(@"document.jsSimulatorObjectReferences[""{0}""]", jsObjectReference.ReferenceId);

                    javascript = javascript.Replace("$" + i.ToString(), jsCodeForAccessingTheObject);
                }
                else if (variable is INTERNAL_HtmlDomElementReference)
                {
                    //------------------------
                    // DOM Element References
                    //------------------------

                    string id = ((INTERNAL_HtmlDomElementReference)variable).UniqueIdentifier;
                    javascript = javascript.Replace("$" + i.ToString(), string.Format(@"document.getElementById(""{0}"")", id));
                }
                else if (variable is INTERNAL_SimulatorJSExpression)
                {
                    //------------------------
                    // JS Expression (simulator only)
                    //------------------------

                    string expression = ((INTERNAL_SimulatorJSExpression)variable).Expression;
                    javascript = javascript.Replace("$" + i.ToString(), expression);
                }
                else if (variable is Delegate)
                {
                    //-----------
                    // Delegates
                    //-----------

                    Delegate callback = (Delegate)variable;

                    // Add the callback to the document:
                    int callbackId = ReferenceIDGenerator.GenerateId();
                    CallbacksDictionary.Add(callbackId, callback);

#if CSHTML5NETSTANDARD
                    //Console.WriteLine("Added ID: " + callbackId.ToString());
#endif

                    // Change the JS code to point to that callback:
                    javascript = javascript.Replace("$" + i.ToString(), string.Format(
                    @"(function() {{
                        var argsArray = Array.prototype.slice.call(arguments);
                        var idWhereCallbackArgsAreStored = ""callback_args_"" + Math.floor(Math.random() * 1000000);
                        document.jsSimulatorObjectReferences[idWhereCallbackArgsAreStored] = argsArray;
                        setTimeout(
                            function() 
                            {{
                               window.onCallBack.OnCallbackFromJavaScript({0}, idWhereCallbackArgsAreStored, argsArray);
                            }}
                            , 1);
                      }})", callbackId));

                    // Note: generating the random number in JS rather than C# is important in order to be able to put this code inside a JavaScript "for" statement (cf. deserialization code of the JsonConvert extension, and also ZenDesk ticket #974) so that the "closure" system of JavaScript ensures that the number is the same before and inside the "setTimeout" call, but different for each iteration of the "for" statement in which this piece of code is put.
                    // Note: we store the arguments in the jsSimulatorObjectReferences that is inside the JS context, so that the user can access them from the callback.
                    // Note: "Array.prototype.slice.call" will convert the arguments keyword into an array (cf. http://stackoverflow.com/questions/960866/how-can-i-convert-the-arguments-object-to-an-array-in-javascript )
                    // Note: in the command above, we use "setTimeout" to avoid thread/locks problems.
                }
                else if (variable == null)
                {
                    //--------------------
                    // Null
                    //--------------------

                    javascript = javascript.Replace("$" + i.ToString(), "null");
                }
                else
                {
                    //--------------------
                    // Simple value types or other objects (note: this includes objects that override the "ToString" method, such as the class "Uri")
                    //--------------------

                    javascript = javascript.Replace("$" + i.ToString(), INTERNAL_HtmlDomManager.ConvertToStringToUseInJavaScriptCode(variable));
                }
            }

            // Surround the javascript code with some code that will store the result into the "document.jsSimulatorObjectReferences" for later use in subsequent calls to this method:
            int referenceId = ReferenceIDGenerator.GenerateId();
            javascript = string.Format(
@"var result = eval(""{0}"");
document.jsSimulatorObjectReferences[""{1}""] = result;
result;
", INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(javascript), referenceId);

            // Execute the javascript code:
            object value = null;
            if (!runAsynchronously)
            {
                value = CastFromJsValue(INTERNAL_HtmlDomManager.ExecuteJavaScriptWithResult(javascript, noImpactOnPendingJSCode: noImpactOnPendingJSCode));
            }
            else
            {
                INTERNAL_HtmlDomManager.ExecuteJavaScript(javascript);
            }

            var objectReference = new INTERNAL_JSObjectReference()
            {
                Value = value,
                ReferenceId = referenceId.ToString()
            };

            return objectReference;
#else
                    return null;
#endif
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("null")]
#else
        [Bridge.Template("null")]
#endif
        internal static object CastFromJsValue(object obj)
        {
#if CSHTML5NETSTANDARD
            return obj;
#else
#if !BUILDINGDOCUMENTATION
            var res = (JSValue)obj;
            int resInt;

            if (res.IsString())
                return res.AsString().Value;
            else if (res.IsBool())
                return res.AsBoolean().Value;
            else if (res.IsNumber())
                return res.AsNumber().Value;
            else if (int.TryParse(res.ToString(), out resInt))
                return resInt;
            else if (res.IsNull())
                return null;
            else
                return res;

            //JSValue jsValue = (JSValue)obj;
            //if (jsValu)
            //{
            //    return (string)jsValue;
            //}
            //else if (jsValue.IsBoolean)
            //{
            //    return (bool)jsValue;
            //}
            //else if (jsValue.IsDouble || jsValue.IsNumber)
            //{
            //    return (double)jsValue;
            //}
            //else if (jsValue.IsInteger)
            //{
            //    return (int)jsValue;
            //}
            //else if (jsValue.IsNaN || jsValue.IsNull || jsValue.IsUndefined)
            //{
            //    return null;
            //}
            //else
            //    return jsValue; // Objects cannot be passed between C# and JavaScript, so we return the JSValue just for the sake of returning something, but we also keep a reference to the JS object via the "document.jsSimulatorObjectReferences" dictionary for later use (see comments above).
#else
            return obj;
#endif

#endif
        }


        /// <summary>
        /// This class has a method that generates IDs in sequence (0, 1, 2, 3...)
        /// </summary>
#if !BRIDGE
        [JSIL.Meta.JSIgnore]
#else
        [External]
#endif
        internal static class ReferenceIDGenerator
        {
            static int NextFreeId = 0;
            internal static int GenerateId()
            {
                int freeId = NextFreeId;
                NextFreeId++;
                return freeId;
            }
        }

        internal static void LoadJavaScriptFile(string url, string callerAssemblyName, Action callbackOnSuccess, Action callbackOnFailure = null)
        {
            string html5Path = INTERNAL_UriHelper.ConvertToHtml5Path(url);

            CSHTML5.Interop.ExecuteJavaScript(
@"// Add the script tag to the head
var head = document.getElementsByTagName('head')[0];
var script = document.createElement('script');
script.type = 'text/javascript';
script.src = $0;

// Then bind the event to the callback function
// There are several events for cross browser compatibility.
if(script.onreadystatechange != undefined) {
script.onreadystatechange = $1;
} else {
script.onload = $1;
script.onerror = $2;
}

// Fire the loading
head.appendChild(script);", html5Path, callbackOnSuccess, callbackOnFailure);
        }

        internal static void LoadJavaScriptFiles(List<string> urls, string callerAssemblyName, Action onCompleted, Action onError = null)
        {
            if (urls.Count > 0)
            {
                LoadJavaScriptFile(urls[0], callerAssemblyName, () =>
                {
                    urls.Remove(urls[0]);
                    LoadJavaScriptFiles(urls, callerAssemblyName, onCompleted, onError);
                },
                () =>
                {
                    if(onError != null)
                        onError();
                });
            }
            else
            {
                if (onCompleted != null)
                    onCompleted();
            }
        }

        internal static void LoadCssFile(string url, Action callback)
        {
            // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
#if !BRIDGE
            string callerAssemblyName = Interop.IsRunningInTheSimulator ? Assembly.GetCallingAssembly().GetName().Name : INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#else
            string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif

            string html5Path = INTERNAL_UriHelper.ConvertToHtml5Path(url);

            CSHTML5.Interop.ExecuteJavaScript(
@"// Add the link tag to the head
var head = document.getElementsByTagName('head')[0];
var link = document.createElement('link');
link.rel  = 'stylesheet';
link.type = 'text/css';
link.href = $0;
link.media = 'all';

// Fire the loading
head.appendChild(link);

// Some browsers do not support the 'onload' event of the 'link' element,
// therefore we use the 'onerror' event of the 'img' tag instead, which is always triggered:
var img = document.createElement('img');
img.onerror = $1;
img.src = $0;", html5Path, callback);
        }

        internal static void LoadCssFiles(List<string> urls, Action onCompleted)
        {
            if (urls.Count > 0)
            {
                LoadCssFile(urls[0], () =>
                {
                    urls.Remove(urls[0]);
                    LoadCssFiles(urls, onCompleted);
                });
            }
            else
            {
                onCompleted();
            }
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("false")]
#else
        [Bridge.Template("false")]
#endif
        internal static bool IsRunningInTheSimulator()
        {
            return true;
        }
    }
}
