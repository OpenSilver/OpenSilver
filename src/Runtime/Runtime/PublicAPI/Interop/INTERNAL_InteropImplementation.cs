

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using CSHTML5.Types;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading;

#if OPENSILVER
using System.Text.Json;
#endif

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5
{
    internal static class INTERNAL_InteropImplementation
    {
        private static bool _isInitialized;
        private static readonly SynchronyzedStore<Delegate> _callbacksStore = new SynchronyzedStore<Delegate>();
        private static readonly SynchronyzedStore<string> _javascriptCallsStore = new SynchronyzedStore<string>();
        private static readonly ReferenceIDGenerator _refIdGenerator = new ReferenceIDGenerator();

        static INTERNAL_InteropImplementation()
        {
            Application.INTERNAL_Reloaded += (sender, e) =>
            {
                _isInitialized = false;
            };
        }

#if BRIDGE
        [Bridge.Template("null")]
#endif
        internal static INTERNAL_JSObjectReference ExecuteJavaScript_SimulatorImplementation(string javascript, bool runAsynchronously, bool noImpactOnPendingJSCode = false, params object[] variables)
        {
            //---------------
            // Due to the fact that it is not possible to pass JavaScript objects between the simulator JavaScript context
            // and the C# context, we store the JavaScript objects in a global dictionary inside the JavaScript context.
            // This dictionary is named "jsObjRef". It associates a unique integer ID to each JavaScript
            // object. In C# we only manipulate those IDs by manipulating instances of the "JSObjectReference" class.
            // When we need to re-use those JavaScript objects, the C# code passes to the JavaScript context the ID
            // of the object, so that the JavaScript code can retrieve the JavaScript object instance by using the 
            // aforementioned dictionary.
            //---------------

            // Verify the arguments:
            if (noImpactOnPendingJSCode && runAsynchronously)
                throw new ArgumentException("You cannot set both 'noImpactOnPendingJSCode' and 'runAsynchronously' to True. The 'noImpactOnPendingJSCode' only has meaning when running synchronously.");

            // Make sure the JS to C# interop is set up:
            if (!_isInitialized)
            {
#if OPENSILVER
                if (Interop.IsRunningInTheSimulator_WorkAround)
                {
#endif
                    // Adding a property to the JavaScript "window" object:
                    dynamic jsWindow = INTERNAL_HtmlDomManager.ExecuteJavaScriptWithResult("window");
                    jsWindow.SetProperty("onCallBack", new OnCallBack(_callbacksStore));
#if OPENSILVER
                }
                else
                {
                    OnCallBack.SetCallbacksDictionary(_callbacksStore);
                }
#endif
                _isInitialized = true;
            }

            string unmodifiedJavascript = javascript;

            // If the javascript code has references to previously obtained JavaScript objects,
            // we replace those references with calls to the "document.jsObjRef"
            // dictionary.
            // Note: we iterate in reverse order because, when we replace ""$" + i.ToString()", we
            // need to replace "$10" before replacing "$1", otherwise it thinks that "$10" is "$1"
            // followed by the number "0". To reproduce the issue, call "ExecuteJavaScript" passing
            // 10 arguments and using "$10".
            for (int i = variables.Length - 1; i >= 0; i--)
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
                    {
                        jsCodeForAccessingTheObject = $@"document.jsObjRef[""{jsObjectReference.ReferenceId}""][{jsObjectReference.ArrayIndex}]";
                    }
                    else
                    {
                        jsCodeForAccessingTheObject = $@"document.jsObjRef[""{jsObjectReference.ReferenceId}""]";
                    }

                    javascript = javascript.Replace("$" + i.ToString(), jsCodeForAccessingTheObject);
                }
                else if (variable is INTERNAL_HtmlDomElementReference)
                {
                    //------------------------
                    // DOM Element References
                    //------------------------

                    string id = ((INTERNAL_HtmlDomElementReference)variable).UniqueIdentifier;
                    javascript = javascript.Replace("$" + i.ToString(), $@"document.getElementByIdSafe(""{id}"")");
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
                    int callbackId = _callbacksStore.Add(callback);

                    var isVoid = callback.Method.ReturnType == typeof(void);

                    // Change the JS code to point to that callback:
                    javascript = javascript.Replace("$" + i.ToString(), string.Format(
                                       @"(function() {{ return document.eventCallback({0}, {1}, {2});}})", callbackId,
#if OPENSILVER
                                       Interop.IsRunningInTheSimulator_WorkAround ? "arguments" : "Array.prototype.slice.call(arguments)",
#elif BRIDGE
                                       "Array.prototype.slice.call(arguments)",
#endif
                                       (!isVoid).ToString().ToLower()
                                       ));

                    // Note: generating the random number in JS rather than C# is important in order
                    // to be able to put this code inside a JavaScript "for" statement (cf.
                    // deserialization code of the JsonConvert extension, and also ZenDesk ticket #974)
                    // so that the "closure" system of JavaScript ensures that the number is the same
                    // before and inside the "setTimeout" call, but different for each iteration of the
                    // "for" statement in which this piece of code is put.
                    // Note: we store the arguments in the jsObjRef that is inside
                    // the JS context, so that the user can access them from the callback.
                    // Note: "Array.prototype.slice.call" will convert the arguments keyword into an array
                    // (cf. http://stackoverflow.com/questions/960866/how-can-i-convert-the-arguments-object-to-an-array-in-javascript)
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
                    // Simple value types or other objects
                    // (note: this includes objects that
                    // override the "ToString" method, such
                    // as the class "Uri")
                    //--------------------

                    javascript = javascript.Replace("$" + i.ToString(), INTERNAL_HtmlDomManager.ConvertToStringToUseInJavaScriptCode(variable));
                }
            }

            // Change the JS code to call ShowErrorMessage in case of error:
            string errorCallBackId = _javascriptCallsStore.Add(unmodifiedJavascript).ToString();

            // Surround the javascript code with some code that will store the
            // result into the "document.jsObjRef" for later
            // use in subsequent calls to this method
            string referenceId = _refIdGenerator.NewId().ToString();
            javascript = $"document.callScriptSafe(\"{referenceId}\",\"{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(javascript)}\",{errorCallBackId})";

            // Execute the javascript code:
            object value = null;
            if (!runAsynchronously)
            {
                value = INTERNAL_HtmlDomManager.ExecuteJavaScriptWithResult(javascript, noImpactOnPendingJSCode: noImpactOnPendingJSCode);
            }
            else
            {
                INTERNAL_HtmlDomManager.ExecuteJavaScript(javascript);
            }

            return new INTERNAL_JSObjectReference(value, referenceId);
        }

        internal static void ResetLoadedFilesDictionaries()
        {
            _pendingJSFile.Clear();
            _loadedFiles.Clear();
        }

        internal static void ShowErrorMessage(string errorMessage, int indexOfCallInList)
        {
            string str = _javascriptCallsStore.Get(indexOfCallInList);

#if OPENSILVER
            if (IsRunningInTheSimulator_WorkAround())
#else
            if (IsRunningInTheSimulator())
#endif
            {
                DotNetForHtml5.Core.INTERNAL_Simulator.SimulatorProxy.ReportJavaScriptError(errorMessage, str);
            }
            else
            {
                string message = string.Format(@"Error in the following javascript code:

{0}

----- Error: -----

{1}
", str, errorMessage);
                Console.WriteLine(message);
            }
        }

        //This Dictionary is here to:
        // - know when we are already attempting to load the file so we do not try to load it a second time
        // - call the correct callback (success or failure) for everything that tried to load said file (so
        // access to the callback) once we receive the event saying that the loading was successful/failed.
        private static Dictionary<string, List<Tuple<Action, Action>>> _pendingJSFile = new Dictionary<string, List<Tuple<Action, Action>>>();

        // To know which files have already been successfully loaded so can we simply call the OnSuccess callback.
        private static HashSet<string> _loadedFiles = new HashSet<string>();

        internal static void LoadJavaScriptFile(string url, string callerAssemblyName, Action callbackOnSuccess, Action callbackOnFailure = null)
        {
            string html5Path = INTERNAL_UriHelper.ConvertToHtml5Path(url);
            if (_loadedFiles.Contains(html5Path)) //Note: using html5Path so we consider different ways of writing the same path as one and only path.
            {
                callbackOnSuccess();
            }
            else if (_pendingJSFile.ContainsKey(html5Path))
            {
                _pendingJSFile[html5Path].Add(new Tuple<Action, Action>(callbackOnSuccess, callbackOnFailure));
            }
            else
            {
                _pendingJSFile.Add(html5Path, new List<Tuple<Action, Action>> { new Tuple<Action, Action>(callbackOnSuccess, callbackOnFailure) });
                CSHTML5.Interop.ExecuteJavaScript(
    @"// Add the script tag to the head
var filePath = $0;
var head = document.getElementsByTagName('head')[0];
var script = document.createElement('script');
script.type = 'text/javascript';
script.src = filePath;
// Then bind the event to the callback function
// There are several events for cross browser compatibility.
if(script.onreadystatechange != undefined) {
script.onreadystatechange = $1;
} else {
script.onload = function () { $1(filePath) };
script.onerror = function () { $2(filePath) };
}

// Fire the loading
head.appendChild(script);", html5Path, (Action<object>)LoadJavaScriptFileSuccess, (Action<object>)LoadJavaScriptFileFailure);
            }
        }

        private static void LoadJavaScriptFileSuccess(object jsArgument)
        {
            // using an Interop call instead of jsArgument.ToString because it causes errors in OpenSilver.
            string loadedFileName = Convert.ToString(Interop.ExecuteJavaScript(@"$0", jsArgument));
            foreach (Tuple<Action, Action> actions in _pendingJSFile[loadedFileName])
            {
                actions.Item1();
            }
            _loadedFiles.Add(loadedFileName);
            _pendingJSFile.Remove(loadedFileName);
        }

        private static void LoadJavaScriptFileFailure(object jsArgument)
        {
            // using an Interop call instead of jsArgument.ToString because it causes errors in OpenSilver.
            string loadedFileName = Convert.ToString(Interop.ExecuteJavaScript(@"$0", jsArgument));
            foreach (Tuple<Action, Action> actions in _pendingJSFile[loadedFileName])
            {
                actions.Item2();
            }
            _pendingJSFile.Remove(loadedFileName);
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
                    if (onError != null)
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
            // Get the assembly name of the calling method: 
            // IMPORTANT: the call to the "GetCallingAssembly" method must be done in
            // the method that is executed immediately after the one where the URI is
            // defined! Be careful when moving the following line of code.
#if NETSTANDARD
            string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
#elif BRIDGE
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

#if BRIDGE
        [Bridge.Template("false")]
#endif
        internal static bool IsRunningInTheSimulator()
        {
            return true;
        }

#if OPENSILVER
        // In the OpenSilver Version
        // This is does not reprensent if we are in the simulator but if we're
        // For This purpose use DotNetForHtml5.Core.IsRunningInTheSimulator_WorkAround
        internal static bool IsRunningInTheSimulator_WorkAround()
        {
            return DotNetForHtml5.Core.INTERNAL_Simulator.IsRunningInTheSimulator_WorkAround;
        }
#endif
    }

    /// <summary>
    /// This class has a method that generates IDs in sequence (0, 1, 2, 3...)
    /// </summary>
#if BRIDGE
    [Bridge.External]
#endif
    internal class ReferenceIDGenerator
    {
        private int _id = 0;

        internal int NewId() => Interlocked.Increment(ref _id);
    }

    internal class SynchronyzedStore<T>
    {
        private readonly object _lock = new object();
        private readonly List<T> _items;

        public SynchronyzedStore()
            : this(8192)
        {
        }

        public SynchronyzedStore(int initialCapacity)
        {
            _items = new List<T>(initialCapacity);
        }

        public int Add(T item)
        {
            lock (_lock)
            {
                _items.Add(item);
                return _items.Count - 1;
            }
        }

        public T Get(int index) => _items[index];
    }
}
