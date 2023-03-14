
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

using CSHTML5.Internal;
using CSHTML5.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver
{
    /*
        // AOT results
        //
        //result=9999999, run=1000000 took 1111
        //result=0, run=1000000 took 844
        //result=abcggggggggggggggggggggggggggggggggggggggggggggggg... [50003], run=50000 took 5710
        //result=, run=50000 took 39
        private void RunSyncJS<T>(string init, string increment, int count, bool getResult, T defaultValue = default) {

            Stopwatch watch = Stopwatch.StartNew();
            T result = defaultValue;
            Interop.ExecuteJavaScriptVoid(init);
            for (int i = 0; i < count; ++i) {
                if (getResult)
                    Interop.ExecuteJavaScriptGetResult(increment, ref result);
                else 
                    Interop.ExecuteJavaScriptVoid(increment);
            }

            var str = result.ToString();
            if (str.Length > 50) {
                var len = str.Length;
                str = $"{str.Substring(0, 50) }... [{len}]" ;
            }

            Console.WriteLine($"result={str}, run={count} took {watch.ElapsedMilliseconds}");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            Console.WriteLine("test started");
            RunSyncJS<int>("var i = 0;", "i += 10;", 1000000, true);
            RunSyncJS<int>("var i = 0;", "i += 10;", 1000000, false);

            RunSyncJS<string>("document.sss = new String('abc');", "document.sss += 'g';", 50000, true, "");
            RunSyncJS<string>("document.sss = new String('abc');", "document.sss += 'g';", 50000, false, "");
            Console.WriteLine("test ended");
        }
     */

    /// <summary>
    /// Provides static methods for executing JavaScript code from within C#.
    /// </summary>
    public static class Interop
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // START OF ExecuteJavascript* functions
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // not public -- if we return an object, when in the Simulator, we'd need to convert it,
        // so the user wouldn't really know what to do with it
        internal static object ExecuteJavaScriptGetResult(string javascript) {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: 0, wantsResult:true, flush:true);
            return value;
        }

        private static IReadOnlyList<object> ExecuteJavaScriptsGetResultsImpl(IReadOnlyList<string> javascript) {
            List<object> results = new List<object>();
            var needsFlush = true;
            foreach (var js in javascript) {
                var value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(js, referenceId: 0, wantsResult:true, flush: needsFlush);
                needsFlush = false;
                results.Add(value);
            }

            return results;
        }

        private static void ConvertJavascriptResult<T>(ref T t, object value) {
            object converted = null;
            if (t is double) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToDouble(null);
                else
                    converted = Convert.ToDouble(value);
            } else if (t is float) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToSingle(null);
                else
                    converted = Convert.ToSingle(value);
            } else if (t is bool) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToBoolean(null);
                else
                    converted = Convert.ToBoolean(value);
            } else if (t is byte) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToByte(null);
                else
                    converted = Convert.ToByte(value);
            } else if (t is char) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToChar(null);
                else
                    converted = Convert.ToChar(value);
            } else if (t is string) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToString(null);
                else
                    converted = Convert.ToString(value);
            } else if (t is int) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToInt32(null);
                else
                    converted = Convert.ToInt32(value);
            } else if (t is uint) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToUInt32(null);
                else
                    converted = Convert.ToUInt32(value);
            } else if (t is long) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToInt64(null);
                else
                    converted = Convert.ToInt64(value);
            } else if (t is ulong) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToUInt64(null);
                else
                    converted = Convert.ToUInt64(value);
            } else if (t is short) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToInt16(null);
                else
                    converted = Convert.ToInt16(value);
            } else if (t is decimal) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToDecimal(null);
                else
                    converted = Convert.ToDecimal(value);
            } else if (t is DateTime) {
                if (IsRunningInTheSimulator)
                    converted = new INTERNAL_JSObjectReference(value).ToDateTime(null);
                else
                    converted = Convert.ToDateTime(value);
            } else
                throw new Exception($"type {t.GetType()} not supported");

            t = (T)(object)converted;
        }

        // the idea for executing several Javascripts at the same time: 
        // optimization: only flush pending JS once (before executing the first one)
        public static void ExecuteJavaScriptGetResult<T1>(string javascript, ref T1 t1) {
            var result = ExecuteJavaScriptGetResult(javascript);
            ConvertJavascriptResult(ref t1, result);
        }
        public static void ExecuteJavaScriptGetResult<T1>(IReadOnlyList<string> javascripts, ref T1 t1) {
            var list = ExecuteJavaScriptsGetResultsImpl(javascripts);
            ConvertJavascriptResult(ref t1, list[0]);
        }
        public static void ExecuteJavaScriptGetResult<T1, T2>(IReadOnlyList<string> javascripts, ref T1 t1, ref T2 t2) {
            var list = ExecuteJavaScriptsGetResultsImpl(javascripts);
            ConvertJavascriptResult(ref t1, list[0]);
            ConvertJavascriptResult(ref t2, list[1]);
        }
        public static void ExecuteJavaScriptGetResult<T1, T2, T3>(IReadOnlyList<string> javascripts, ref T1 t1, ref T2 t2, ref T3 t3) {
            var list = ExecuteJavaScriptsGetResultsImpl(javascripts);
            ConvertJavascriptResult(ref t1, list[0]);
            ConvertJavascriptResult(ref t2, list[1]);
            ConvertJavascriptResult(ref t3, list[2]);
        }
        public static void ExecuteJavaScriptGetResult<T1, T2, T3, T4>(IReadOnlyList<string> javascripts, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4) {
            var list = ExecuteJavaScriptsGetResultsImpl(javascripts);
            ConvertJavascriptResult(ref t1, list[0]);
            ConvertJavascriptResult(ref t2, list[1]);
            ConvertJavascriptResult(ref t3, list[2]);
            ConvertJavascriptResult(ref t4, list[3]);
        }
        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static void ExecuteJavaScriptVoid(string javascript, bool flushQueue )
        {
            INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: 0, wantsResult:false, flush:flushQueue);
        }

        public static void ExecuteJavaScriptVoid(string javascript) {
            INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: 0, wantsResult:false, flush:true);
        }

        public static void ExecuteJavaScriptVoid(IReadOnlyList<string> javascript) {
            var needsFlush = true;
            foreach (var js in javascript) {
                INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(js, referenceId: 0, wantsResult:false, flush: needsFlush);
                needsFlush = false;
            }
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <returns>The result, if any, of the JavaScript call.</returns>
        public static object ExecuteJavaScript(string javascript)
        {
            return CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_GetJSObject(javascript, runAsynchronously: false);
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary> 
        internal static double ExecuteJavaScriptDouble(string javascript, bool flushQueue = true)
        {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: 0, wantsResult:true, flush:flushQueue);
            if (IsRunningInTheSimulator)
            {
                return new INTERNAL_JSObjectReference(value).ToDouble(null);
            }
            else
            {
                return Convert.ToDouble(value);
            }
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static int ExecuteJavaScriptInt32(string javascript, bool flushQueue = true)
        {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: 0, wantsResult:true, flush:flushQueue);
            if (IsRunningInTheSimulator)
            {
                return new INTERNAL_JSObjectReference(value).ToInt32(null);
            }
            else
            {
                return Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static string ExecuteJavaScriptString(string javascript, bool flushQueue = true)
        {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: 0, wantsResult:true, flush:flushQueue);
            if (IsRunningInTheSimulator)
            {
                return new INTERNAL_JSObjectReference(value).ToString(null);
            }
            else
            {
                return Convert.ToString(value);
            }
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static bool ExecuteJavaScriptBoolean(string javascript, bool flushQueue = true)
        {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: 0, wantsResult:true, flush:flushQueue);
            if (IsRunningInTheSimulator)
            {
                return new INTERNAL_JSObjectReference(value).ToBoolean(null);
            }
            else
            {
                return Convert.ToBoolean(value);
            }
        }


        /// <summary>
        /// Allows calling JavaScript code from within C#.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <param name="variables">The objects to use inside the JavaScript call.</param>
        /// <returns>The result, if any, of the JavaScript call.</returns>
        public static object ExecuteJavaScript(string javascript, params object[] variables)
        {
            return CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_GetJSObject(javascript, runAsynchronously: false, variables: variables);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        public static object ExecuteJavaScriptAsync(string javascript)
        {
            return CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_GetJSObject(javascript, runAsynchronously: true);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <param name="variables">The objects to use inside the JavaScript call.</param>
        public static object ExecuteJavaScriptAsync(string javascript, params object[] variables)
        {
            return CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_GetJSObject(javascript, runAsynchronously: true, variables: variables);
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static void ExecuteJavaScriptFastAsync(string javascript)
        {
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javascript);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // END OF ExecuteJavascript* functions
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Unboxes the value passed as a parameter. It is particularly useful for the variables of the ExecuteJavaScript Methods calls aimed at using third party libraries.
        /// </summary>
        /// <param name="value">The value to unbox.</param>
        /// <returns>the unboxed value if the value was boxed, the value itself otherwise.</returns>
#if BRIDGE
        [Bridge.Template("({value} == undefined ? {value} : ({value}.v != undefined ? {value}.v : {value}))")]
#endif
        public static object Unbox(object value)
        {
            return value;
        }

        /// <summary>
        /// Adds a 'script' tag to the HTML page and waits for the script to finish loading.
        /// </summary>
        /// <param name="url">The URL of the JavaScript file, with the syntax ms-appx:///AssemblyName/Folder/FileName.js or /AssemblyName;component/Folder/FileName.js</param>
        /// <returns>Nothing.</returns>
        public static Task<object> LoadJavaScriptFile(string url)
        {
            // Get the assembly name of the calling method: 
            //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
            //that is executed immediately after the one where the URI is defined! Be careful
            //when moving the following line of code.
#if NETSTANDARD
	        string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
#elif BRIDGE
            string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif

            var t = new TaskCompletionSource<object>();
            CSHTML5.INTERNAL_InteropImplementation.LoadJavaScriptFile(
                url, 
                callerAssemblyName, 
                () => t.SetResult(null), () => t.SetException(new Exception("Could not load file: \"" + url + "\"."))
            );
            return t.Task;
        }

        private static HashSet<string> _jsScriptFileKeys = new HashSet<string>(); //todo: This is probably redundant with the _pendingJSFile and _loadedFiles in INTERNAL_InteropImplementation so remove this?
        
        public static Task<object> LoadJavaScriptFile(ResourceFile resourceFile)
        {
            if (!_jsScriptFileKeys.Contains(resourceFile.Key))
            {
                // Get the assembly name of the calling method: 
                // IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
                // that is executed immediately after the one where the URI is defined! Be careful
                // when moving the following line of code.
#if NETSTANDARD
	            string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
#elif BRIDGE
                string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif

                var t = new TaskCompletionSource<object>();
                CSHTML5.INTERNAL_InteropImplementation.LoadJavaScriptFile(resourceFile.Url, callerAssemblyName,
                    () =>
                    {
                        t.SetResult(null);
                        _jsScriptFileKeys.Add(resourceFile.Key); //we only set this when it successfully loaded so that the next time we try to load a file with the same key, finding the key in the HashSet means that it was already SUCCESSFULLY loaded.
                    },
                    () => t.SetException(new Exception("Could not load file: \"" + resourceFile.Url + "\".")));

                return t.Task;
            }

            var task = new TaskCompletionSource<object>();
            task.SetResult(null);
            return task.Task;
        }

        /// <summary>
        /// Loads a list of JavaScript files from either an online location (http/https) or the local project. Note: This method will stop at the first script it cannot load, meaning that all subsequent scripts will not be loaded.
        /// </summary>
        /// <param name="urls">The URLs of the JavaScript files, with the syntax ms-appx:///AssemblyName/Folder/FileName.js or /AssemblyName;component/Folder/FileName.js or https://someAddress/FileName.js
        /// </param>
        /// <param name="callback">The method that is called when all the files have successfully finished loading.</param>
        /// <param name="callbackOnError">The method that is called when one of the files could not be loaded.</param>
        public static void LoadJavaScriptFilesAsync(IEnumerable<string> urls, Action callback, Action callbackOnError = null)
        {
            // Get the assembly name of the calling method: 
            // IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
            // that is executed immediately after the one where the URI is defined! Be careful
            // when moving the following line of code.
#if NETSTANDARD
	        string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
#else
            string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif
            List<string> urlsAsList = (urls is List<string> ? (List<string>)urls : new List<string>(urls));
            CSHTML5.INTERNAL_InteropImplementation.LoadJavaScriptFiles(urlsAsList, callerAssemblyName, callback, callbackOnError);
        }

        public static void LoadJavaScriptFilesAsync(IEnumerable<ResourceFile> resourceFiles, Action callback, Action callbackOnError = null)
        {
            // Get the assembly name of the calling method: 
            // IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
            // that is executed immediately after the one where the URI is defined! Be careful
            // when moving the following line of code.
#if NETSTANDARD
	        string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
#elif BRIDGE
            string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif
            List<string> urlsAsList = new List<string>();
            foreach (var resourceFile in resourceFiles)
            {
                //add the key to the dictionary and add the url to the list:
                if(!_jsScriptFileKeys.Contains(resourceFile.Key))
                {
                    _jsScriptFileKeys.Add(resourceFile.Key);
                    urlsAsList.Add(resourceFile.Url);
                }
            }
            CSHTML5.INTERNAL_InteropImplementation.LoadJavaScriptFiles(urlsAsList, callerAssemblyName, callback, callbackOnError);
        }

        /// <summary>
        /// Adds a 'link' tag to the HTML page and waits for the CSS file to finish loading.
        /// </summary>
        /// <param name="url">The URL of the CSS file, with the syntax ms-appx:///AssemblyName/Folder/FileName.css or /AssemblyName;component/Folder/FileName.css</param>
        /// <returns>Nothing</returns>
        public static Task<object> LoadCssFile(string url)
        {
            var t = new TaskCompletionSource<object>();
            CSHTML5.INTERNAL_InteropImplementation.LoadCssFile(url, () => t.SetResult(null));
            return t.Task;
        }

        static HashSet<string> _cssFileKeys = new HashSet<string>();
        public static Task<object> LoadCssFile(ResourceFile resourceFile)
        {
            if (!_cssFileKeys.Contains(resourceFile.Key))
            {
                var t = new TaskCompletionSource<object>();
                CSHTML5.INTERNAL_InteropImplementation.LoadCssFile(resourceFile.Url, () => t.SetResult(null));
                _cssFileKeys.Add(resourceFile.Key);
                return t.Task;
            }

            var task = new TaskCompletionSource<object>();
            task.SetResult(null);
            return task.Task;
        }


        /// <summary>
        /// Adds 'link' tags to the HTML page and waits for the CSS files to finish loading.
        /// </summary>
        /// <param name="urls">The URL of the CSS files, with the syntax ms-appx:///AssemblyName/Folder/FileName.css or /AssemblyName;component/Folder/FileName.css</param>
        /// <param name="callback">The method that is called when the CSS files have finished loading.</param>
        public static void LoadCssFilesAsync(IEnumerable<string> urls, Action callback)
        {
            List<string> urlsAsList = (urls is List<string> ? (List<string>)urls : new List<string>(urls));
            CSHTML5.INTERNAL_InteropImplementation.LoadCssFiles(urlsAsList, callback);
        }

        public static void LoadCssFilesAsync(IEnumerable<ResourceFile> resourceFiles, Action callback)
        {
            List<string> urlsAsList = new List<string>();
            foreach (var resourceFile in resourceFiles)
            {
                //add the key to the dictionary and add the url to the list:
                if (!_cssFileKeys.Contains(resourceFile.Key))
                {
                    _cssFileKeys.Add(resourceFile.Key);
                    urlsAsList.Add(resourceFile.Url);
                }
            }
            CSHTML5.INTERNAL_InteropImplementation.LoadCssFiles(urlsAsList, callback);
        }

        /// <summary>
        /// Returns the HTML Div that is associated to the specified FrameworkElement.
        /// Note: the FrameworkElement must be in the visual tree. Consider calling this
        /// method from the 'Loaded' event to ensure that the element is in the visual tree.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static object GetDiv(UIElement element)
        {
            return element.INTERNAL_OuterDomElement;
        }

#if CSHTML5BLAZOR
        /// <summary>
        /// Returns True is the app is running in C#, and False otherwise. 
        /// To know if you're in the simulator use IsRunningInTheSimulator_WorkAround.
        /// </summary>
#else
        /// <summary>
        /// Returns True is the app is running in C# inside the Simulator, and False otherwise.
        /// </summary>
#endif
        public static bool IsRunningInTheSimulator
        {
            get
            {
#if OPENSILVER
	            return CSHTML5.INTERNAL_InteropImplementation.IsRunningInTheSimulator_WorkAround();
#elif BRIDGE
                return CSHTML5.INTERNAL_InteropImplementation.IsRunningInTheSimulator();
#endif
            }
        }

#if CSHTML5BLAZOR
        // For backwards compatibility
        
        /// <summary>
        /// Returns True is the app is running inside the Simulator, and False otherwise.
        /// </summary>
        public static bool IsRunningInTheSimulator_WorkAround
        {
            get
            {
                return IsRunningInTheSimulator;
            }
        }
#endif


        /// <summary>
        /// Check if the given jsnode is undefined
        /// </summary>
#if BRIDGE
        [Bridge.Template("(typeof({jsObject}) === 'undefined')")]
#endif
        public static bool IsUndefined(object jsObject)
        {
            return ((CSHTML5.Types.INTERNAL_JSObjectReference)jsObject).IsUndefined();
        }

        /// <summary>
        /// Check if the given jsnode is undefined
        /// </summary>
#if BRIDGE
        [Bridge.Template("({jsObject} === null)")]
#endif
        public static bool IsNull(object jsObject)
        {
            return ((CSHTML5.Types.INTERNAL_JSObjectReference)jsObject).IsNull();
        }

        public class ResourceFile
        {
            public ResourceFile(string key, string url)
            {
                Key = key;
                Url = url;
            }

            /// <summary>
            /// The Key associated with the file. This is used to avoid loading a file multiple times or loading multiple files with the same purpose (for example JQuery).
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// The path to the file.
            /// </summary>
            public string Url { get; set; }
        }
    }
}
