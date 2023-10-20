

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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Windows;
using CSHTML5.Types;
using CSHTML5.Internal;
using OpenSilver.Internal;
using DotNetForHtml5.Core;

namespace CSHTML5
{
    internal static class INTERNAL_InteropImplementation
    {
        private static bool _isInitialized;
        private static readonly ReferenceIDGenerator _refIdGenerator = new ReferenceIDGenerator();

        static INTERNAL_InteropImplementation()
        {
            Application.INTERNAL_Reloaded += (sender, e) =>
            {
                _isInitialized = false;
            };
        }

        private static void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                INTERNAL_Simulator.SimulatorCallbackSetup(new OnCallbackSimulator());
            }

            _isInitialized = true;
        }

        internal static string GetVariableStringForJS(object variable)
        {
            static JavaScriptCallback CreateJSCallback(Delegate callback)
            {
                if (callback is null)
                {
                    throw new ArgumentNullException(nameof(callback));
                }

                bool sync = callback.Method.ReturnType != typeof(void);

                return JavaScriptCallback.Create(callback, sync);
            }

            variable = variable is Delegate d ? CreateJSCallback(d) : variable;

            if (variable is IJavaScriptConvertible jsConvertible)
            {
                return jsConvertible.ToJavaScriptString();
            }
            else if (variable == null)
            {
                //--------------------
                // Null
                //--------------------

                return "null";
            }
            else
            {
                //--------------------
                // Simple value types or other objects
                // (note: this includes objects that
                // override the "ToString" method, such
                // as the class "Uri")
                //--------------------

                return INTERNAL_HtmlDomManager.ConvertToStringToUseInJavaScriptCode(variable);
            }
        }

        internal static string ReplaceJSArgs(string javascript, params object[] variables)
        {
            // Make sure the JS to C# interop is set up:
            EnsureInitialized();

            // If the javascript code has references to previously obtained JavaScript objects,
            // we replace those references with calls to the "document.jsObjRef"
            // dictionary.
            // Note: we iterate in reverse order because, when we replace ""$" + i.ToString()", we
            // need to replace "$10" before replacing "$1", otherwise it thinks that "$10" is "$1"
            // followed by the number "0". To reproduce the issue, call "ExecuteJavaScript" passing
            // 10 arguments and using "$10".
            for (int i = variables.Length - 1; i >= 0; i--)
            {
                javascript = javascript.Replace($"${i}", GetVariableStringForJS(variables[i]));
            }

            return javascript;
        }

        internal static object ExecuteJavaScript_Implementation(
            string javascript,
            bool runAsynchronously,
            bool wantsResult = true,
            bool wantsReferenceId = true,
            bool hasImpactOnPendingJSCode = true,
            params object[] variables)
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

            javascript = ReplaceJSArgs(javascript, variables);

            object result = null;

            // Surround the javascript code with some code that will store the
            // result into the "document.jsObjRef" for later
            // use in subsequent calls to this method
            int referenceId = wantsReferenceId ? _refIdGenerator.NewId() : -1;
            if (runAsynchronously)
            {
                if (wantsReferenceId)
                    INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javascript, referenceId);
                else
                    INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javascript);
            }
            else
            {
                // run sync
                result = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId, wantsResult, flush: hasImpactOnPendingJSCode);
            }

            if (wantsResult)
            {
                if (wantsReferenceId)
                    result = new INTERNAL_JSObjectReference(result, referenceId.ToString(), javascript);
                else if (runAsynchronously)
                    throw new Exception("runAsync + wantsResult + !wantsReferenceId -> use INTERNAL_ExecuteJavaScript.ExecuteJavaScriptAsync");
            }
            else
                result = null;

            return result;
        }

        internal static INTERNAL_JSObjectReference ExecuteJavaScript_GetJSObject(
            string javascript,
            bool runAsynchronously,
            bool hasImpactOnPendingJSCode = true,
            params object[] variables)
        {
            var result = ExecuteJavaScript_Implementation(javascript, runAsynchronously,
                                                          wantsResult: true,
                                                          wantsReferenceId: true,
                                                          hasImpactOnPendingJSCode, variables);
            return (INTERNAL_JSObjectReference)result;
        }

        internal static void ResetLoadedFilesDictionaries()
        {
            _pendingJSFile.Clear();
            _loadedFiles.Clear();
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
                string sSuccessAction = GetVariableStringForJS((Action<object>)LoadJavaScriptFileSuccess);
                string sFailureAction = GetVariableStringForJS((Action<object>)LoadJavaScriptFileFailure);
                OpenSilver.Interop.ExecuteJavaScriptVoid(
    $@"// Add the script tag to the head
var filePath = {GetVariableStringForJS(html5Path)};
var head = document.getElementsByTagName('head')[0];
var script = document.createElement('script');
script.type = 'text/javascript';
script.src = filePath;
// Then bind the event to the callback function
// There are several events for cross browser compatibility.
if(script.onreadystatechange != undefined) {{
script.onreadystatechange = {sSuccessAction};
}} else {{
script.onload = function () {{ {sSuccessAction}(filePath) }};
script.onerror = function () {{ {sFailureAction}(filePath) }};
}}

// Fire the loading
head.appendChild(script);");
            }
        }

        private static void LoadJavaScriptFileSuccess(object jsArgument)
        {
            // using an Interop call instead of jsArgument.ToString because it causes errors in OpenSilver.
            string loadedFileName = OpenSilver.Interop.ExecuteJavaScriptString(GetVariableStringForJS(jsArgument));
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
            string loadedFileName = OpenSilver.Interop.ExecuteJavaScriptString(GetVariableStringForJS(jsArgument));
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
            string html5Path = INTERNAL_UriHelper.ConvertToHtml5Path(url);

            string sHtml5Path = GetVariableStringForJS(html5Path);
            string sCallback = GetVariableStringForJS(callback);
            OpenSilver.Interop.ExecuteJavaScriptVoid(
$@"// Add the link tag to the head
var head = document.getElementsByTagName('head')[0];
var link = document.createElement('link');
link.rel  = 'stylesheet';
link.type = 'text/css';
link.href = {sHtml5Path};
link.media = 'all';

// Fire the loading
head.appendChild(link);

// Some browsers do not support the 'onload' event of the 'link' element,
// therefore we use the 'onerror' event of the 'img' tag instead, which is always triggered:
var img = document.createElement('img');
img.onerror = {sCallback};
img.src = {sHtml5Path};");
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

        // In the OpenSilver Version
        // This is does not reprensent if we are in the simulator but if we're
        // For This purpose use DotNetForHtml5.Core.IsRunningInTheSimulator_WorkAround
        internal static bool IsRunningInTheSimulator_WorkAround()
        {
            return INTERNAL_Simulator.IsRunningInTheSimulator_WorkAround;
        }
    }

    internal sealed class SynchronyzedStore<T>
    {
        private readonly object _lock = new();
        private readonly Dictionary<int, T> _items;
        private int _slot;

        public SynchronyzedStore()
            : this(8192)
        {
        }

        public SynchronyzedStore(int initialCapacity)
        {
            _items = new Dictionary<int, T>(initialCapacity);
        }

        public int Add(T item)
        {
            lock (_lock)
            {
                int slot = _slot++;
                _items.Add(slot, item);
                return slot;
            }
        }

        public void Clean(int index)
        {
            lock (_lock)
            {
                _items.Remove(index);
            }
        }

        public T Get(int index) => _items.TryGetValue(index, out T value) ? value : default;
    }
}
