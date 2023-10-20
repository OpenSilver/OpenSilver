
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CSHTML5.Internal;
using CSHTML5.Types;
using OpenSilver.Internal;

namespace OpenSilver
{
    /// <summary>
    /// Provides static methods for executing JavaScript code from within C#.
    /// </summary>
    public static class Interop
    {
        // for debugging/testing only
        // if > 0, we're dumping All JS objects every X millis
        public static int DumpAllJavascriptObjectsEveryMs
        {
            get => _dumpAllJavascriptObjectsEveryMs;
            set
            {
                if (_dumpAllJavascriptObjectsEveryMs == value)
                    return;
                _dumpAllJavascriptObjectsEveryMs = value;
                if (_dumpAllJavascriptObjectsEveryMs > 0)
                    JSObjectReferenceHolder.Instance.StartTracking(_dumpAllJavascriptObjectsEveryMs);
            }
        }

        // for debugging/testing only
        // if true, we dump stack trace when dumping the JS Ref objects
        public static bool DumpAllJavascriptObjectsVerbose { get; set; } = true;
        // how many functions (from the stack trace) to dump? (since the stack trace can end up being insanely huge)
        public static int DumpAllJavascriptObjectsStackTraceCount { get; set; } = 15;

        // filter when dumping added Javascript Objects -- perhaps you already know some objects that are persistent throughout most of the app
        // in this case, filter those out, so you can focus on the leaks
        //
        // function-name, javascript-code
        public static Func<string, string, bool> DumpAllJavascriptObjectsFilter { get; set; } = (a, b) => true;

        // the idea: in release AOT, Console.WriteLine doesn't work in OpenSilver, but it works on client code
        public static Action<string> DumpAllJavascriptObjectsLogger { get; set; } = Console.WriteLine;

        internal static bool IsTrackingAllJavascriptObjects => DumpAllJavascriptObjectsEveryMs > 0;

        private static T ConvertJavascriptResult<T>(object value)
        {
            Type t = typeof(T);
            if (t == typeof(string))
            {
                string s = ConvertJSResultToString(value);
                return Unsafe.As<string, T>(ref s);
            }
            else if (t == typeof(double))
            {
                double d = ConvertJSResultToDouble(value);
                return Unsafe.As<double, T>(ref d);
            }
            else if (t == typeof(int))
            {
                int i = ConvertJSResultToInt32(value);
                return Unsafe.As<int, T>(ref i);
            }
            else if (t == typeof(bool))
            {
                bool b = ConvertJSResultToBoolean(value);
                return Unsafe.As<bool, T>(ref b);
            }
            else if (t == typeof(char))
            {
                char c = ConvertJSResultToChar(value);
                return Unsafe.As<char, T>(ref c);
            }
            else if (t == typeof(float))
            {
                float f = ConvertJSResultToSingle(value);
                return Unsafe.As<float, T>(ref f);
            }
            else if (t == typeof(byte))
            {
                byte b = ConvertJSResultToByte(value);
                return Unsafe.As<byte, T>(ref b);
            }
            else if (t == typeof(uint))
            {
                uint i = ConvertJSResultToUInt32(value);
                return Unsafe.As<uint, T>(ref i);
            }
            else if (t == typeof(long))
            {
                long l = ConvertJSResultToInt64(value);
                return Unsafe.As<long, T>(ref l);
            }
            else if (t == typeof(ulong))
            {
                ulong l = ConvertJSResultToUInt64(value);
                return Unsafe.As<ulong, T>(ref l);
            }
            else if (t == typeof(short))
            {
                short s = ConvertJSResultToInt16(value);
                return Unsafe.As<short, T>(ref s);
            }
            else if (t == typeof(decimal))
            {
                decimal d = ConvertJSResultToDecimal(value);
                return Unsafe.As<decimal, T>(ref d);
            }
            else if (t == typeof(DateTime))
            {
                DateTime d = ConvertJSResultToDateTime(value);
                return Unsafe.As<DateTime, T>(ref d);
            }
            else
            {
                throw new ArgumentException($"Type '{t.FullName}' is not supported.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ConvertJSResultToString(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToString(value) : Convert.ToString(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ConvertJSResultToDouble(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToDouble(value) : Convert.ToDouble(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ConvertJSResultToInt32(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToInt32(value) : Convert.ToInt32(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ConvertJSResultToBoolean(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToBoolean(value) : Convert.ToBoolean(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char ConvertJSResultToChar(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToChar(value) : Convert.ToChar(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ConvertJSResultToSingle(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToSingle(value) : Convert.ToSingle(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ConvertJSResultToByte(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToByte(value) : Convert.ToByte(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ConvertJSResultToUInt32(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToUInt32(value) : Convert.ToUInt32(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ConvertJSResultToInt64(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToInt64(value) : Convert.ToInt64(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ConvertJSResultToUInt64(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToUInt64(value) : Convert.ToUInt64(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short ConvertJSResultToInt16(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToInt16(value) : Convert.ToInt16(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static decimal ConvertJSResultToDecimal(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToDecimal(value) : Convert.ToDecimal(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime ConvertJSResultToDateTime(object value) =>
            IsRunningInTheSimulator ? INTERNAL_JSObjectReference.ToDateTime(value) : Convert.ToDateTime(value);

        public static T1 ExecuteJavaScriptGetResult<T1>(string javascript)
        {
            var result = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: -1, wantsResult: true, flush: true);
            T1 t1 = ConvertJavascriptResult<T1>(result);
            return t1;
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static void ExecuteJavaScriptVoid(string javascript, bool flushQueue)
        {
            INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: -1, wantsResult: false, flush: flushQueue);
        }

        internal static void ExecuteJavaScriptVoid(string javascript, bool flushQueue, params object[] variables)
        {
            CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_Implementation(javascript,
                runAsynchronously: false,
                wantsResult: false,
                wantsReferenceId: false,
                hasImpactOnPendingJSCode: flushQueue,
                variables: variables);
        }

        public static void ExecuteJavaScriptVoid(string javascript, params object[] variables)
        {
            ExecuteJavaScriptVoid(javascript, false, variables);
        }

        public static void ExecuteJavaScriptVoid(string javascript)
        {
            INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: -1, wantsResult: false, flush: true);
        }

        public static void ExecuteJavaScriptVoidAsync(string javascript, params object[] variables)
        {
            javascript = CSHTML5.INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, variables);
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javascript);
        }

        public static void ExecuteJavaScriptVoidAsync(string javascript)
        {
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javascript);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <returns>The result, if any, of the JavaScript call.</returns>
        ///
        /// FIXME: further improvement - if I only need the result, then don't create a referenceId
        public static IDisposable ExecuteJavaScript(string javascript)
        {
            // returns INTERNAL_JSObjectReference
            return CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_GetJSObject(javascript, runAsynchronously: false);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <param name="variables">The objects to use inside the JavaScript call.</param>
        /// <returns>The result, if any, of the JavaScript call.</returns>
        public static IDisposable ExecuteJavaScript(string javascript, params object[] variables)
        {
            // returns INTERNAL_JSObjectReference
            return CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_GetJSObject(javascript, runAsynchronously: false, variables: variables);
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary> 
        internal static double ExecuteJavaScriptDouble(string javascript, bool flushQueue = true)
        {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: -1, wantsResult: true, flush: flushQueue);
            return ConvertJSResultToDouble(value);
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static int ExecuteJavaScriptInt32(string javascript, bool flushQueue = true)
        {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: -1, wantsResult: true, flush: flushQueue);
            return ConvertJSResultToInt32(value);
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static string ExecuteJavaScriptString(string javascript, bool flushQueue = true)
        {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: -1, wantsResult: true, flush: flushQueue);
            return ConvertJSResultToString(value);
        }

        /// <summary>
        /// Execute JavaScript code without document.callScriptSafe
        /// </summary>
        internal static bool ExecuteJavaScriptBoolean(string javascript, bool flushQueue = true)
        {
            object value = INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(javascript, referenceId: -1, wantsResult: true, flush: flushQueue);
            return ConvertJSResultToBoolean(value);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        public static IDisposable ExecuteJavaScriptAsync(string javascript)
        {
            return CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_GetJSObject(javascript, runAsynchronously: true);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <param name="variables">The objects to use inside the JavaScript call.</param>
        public static IDisposable ExecuteJavaScriptAsync(string javascript, params object[] variables)
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

        /// <summary>
        /// Register a .NET method to allow it to be invoked from JavaScript code.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> object that must be disposed after it has been used.
        /// If the object is not disposed, it will be leaked.
        /// </returns>
        public static IDisposable CreateJavascriptCallback(Delegate d) => JavaScriptCallback.Create(d, true);

        /// <summary>
        /// Unboxes the value passed as a parameter. It is particularly useful for the variables of the ExecuteJavaScript Methods calls aimed at using third party libraries.
        /// </summary>
        /// <param name="value">The value to unbox.</param>
        /// <returns>the unboxed value if the value was boxed, the value itself otherwise.</returns>
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
            string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;

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
                string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;

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
            string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
            List<string> urlsAsList = (urls is List<string> ? (List<string>)urls : new List<string>(urls));
            CSHTML5.INTERNAL_InteropImplementation.LoadJavaScriptFiles(urlsAsList, callerAssemblyName, callback, callbackOnError);
        }

        public static void LoadJavaScriptFilesAsync(IEnumerable<ResourceFile> resourceFiles, Action callback, Action callbackOnError = null)
        {
            // Get the assembly name of the calling method: 
            // IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
            // that is executed immediately after the one where the URI is defined! Be careful
            // when moving the following line of code.
            string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
            List<string> urlsAsList = new List<string>();
            foreach (var resourceFile in resourceFiles)
            {
                //add the key to the dictionary and add the url to the list:
                if (!_jsScriptFileKeys.Contains(resourceFile.Key))
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
        private static int _dumpAllJavascriptObjectsEveryMs = 0;

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

        /// <summary>
        /// Returns True is the app is running in C#, and False otherwise. 
        /// To know if you're in the simulator use IsRunningInTheSimulator_WorkAround.
        /// </summary>
        public static bool IsRunningInTheSimulator =>
            CSHTML5.INTERNAL_InteropImplementation.IsRunningInTheSimulator_WorkAround();

        /// <summary>
        /// Returns True is the app is running inside the Simulator, and False otherwise.
        /// </summary>
        public static bool IsRunningInTheSimulator_WorkAround => IsRunningInTheSimulator;

        /// <summary>
        /// Check if the given jsnode is undefined
        /// </summary>
        public static bool IsUndefined(object jsObject)
        {
            return ((INTERNAL_JSObjectReference)jsObject).IsUndefined();
        }

        /// <summary>
        /// Check if the given jsnode is undefined
        /// </summary>
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
