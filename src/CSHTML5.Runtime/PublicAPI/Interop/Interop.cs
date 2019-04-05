
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



#if !BRIDGE
using CSHTML5.Types;
using JSIL.Meta;
#else
using Bridge;
#endif
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5
{
    /// <summary>
    /// Provides static methods for executing JavaScript code from within C#.
    /// </summary>
    public static class Interop
    {
        /// <summary>
        /// Allows calling JavaScript code from within C#.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <returns>The result, if any, of the JavaScript call.</returns>
#if BRIDGE
        [Template]
#endif
        public static object ExecuteJavaScript(string javascript)
        {
            return INTERNAL_InteropImplementation.ExecuteJavaScript_SimulatorImplementation(javascript, runAsynchronously: false);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <param name="variables">The objects to use inside the JavaScript call.</param>
        /// <returns>The result, if any, of the JavaScript call.</returns>
#if BRIDGE
        [Template]
#endif
        public static object ExecuteJavaScript(string javascript, params object[] variables)
        {
            
            return INTERNAL_InteropImplementation.ExecuteJavaScript_SimulatorImplementation(javascript, runAsynchronously: false, variables: variables);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
#if BRIDGE
        [Template]
#endif
        public static object ExecuteJavaScriptAsync(string javascript)
        {
            return INTERNAL_InteropImplementation.ExecuteJavaScript_SimulatorImplementation(javascript, runAsynchronously: true);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <param name="variables">The objects to use inside the JavaScript call.</param>
#if BRIDGE
        [Template]
#endif
        public static object ExecuteJavaScriptAsync(string javascript, params object[] variables)
        {
            return INTERNAL_InteropImplementation.ExecuteJavaScript_SimulatorImplementation(javascript, runAsynchronously: true, variables: variables);
        }

        /// <summary>
        /// Adds a 'script' tag to the HTML page and waits for the script to finish loading.
        /// </summary>
        /// <param name="url">The URL of the JavaScript file, with the syntax ms-appx:///AssemblyName/Folder/FileName.js or /AssemblyName;component/Folder/FileName.js</param>
        /// <returns>Nothing.</returns>
        public static Task<object> LoadJavaScriptFile(string url)
        {
            // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
#if !BRIDGE
            string callerAssemblyName = Interop.IsRunningInTheSimulator ? Assembly.GetCallingAssembly().GetName().Name : INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#else
            string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif

            var t = new TaskCompletionSource<object>();
            INTERNAL_InteropImplementation.LoadJavaScriptFile(url, callerAssemblyName, () => t.SetResult(null), () => t.SetException(new Exception("Could not load file: \"" + url + "\".")));
            return t.Task;
        }

        static HashSet2<string> _jsScriptFileKeys = new HashSet2<string>();
        public static Task<object> LoadJavaScriptFile(ResourceFile resourceFile)
        {
            if (!_jsScriptFileKeys.Contains(resourceFile.Key))
            {
                // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
#if !BRIDGE
                string callerAssemblyName = Interop.IsRunningInTheSimulator ? Assembly.GetCallingAssembly().GetName().Name : INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#else
                string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif

                var t = new TaskCompletionSource<object>();
                INTERNAL_InteropImplementation.LoadJavaScriptFile(resourceFile.Url, callerAssemblyName,
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
            // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
#if !BRIDGE
            string callerAssemblyName = Interop.IsRunningInTheSimulator ? Assembly.GetCallingAssembly().GetName().Name : INTERNAL_UriHelper.GetJavaScriptCallingAssembly();

#else
            string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#endif
            List<string> urlsAsList = (urls is List<string> ? (List<string>)urls : new List<string>(urls));
            INTERNAL_InteropImplementation.LoadJavaScriptFiles(urlsAsList, callerAssemblyName, callback, callbackOnError);
        }

        public static void LoadJavaScriptFilesAsync(IEnumerable<ResourceFile> resourceFiles, Action callback, Action callbackOnError = null)
        {
            // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
#if !BRIDGE
            string callerAssemblyName = Interop.IsRunningInTheSimulator ? Assembly.GetCallingAssembly().GetName().Name : INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#else
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
            INTERNAL_InteropImplementation.LoadJavaScriptFiles(urlsAsList, callerAssemblyName, callback, callbackOnError);
        }

        /// <summary>
        /// Adds a 'link' tag to the HTML page and waits for the CSS file to finish loading.
        /// </summary>
        /// <param name="url">The URL of the CSS file, with the syntax ms-appx:///AssemblyName/Folder/FileName.css or /AssemblyName;component/Folder/FileName.css</param>
        /// <returns>Nothing</returns>
        public static Task<object> LoadCssFile(string url)
        {
            var t = new TaskCompletionSource<object>();
            INTERNAL_InteropImplementation.LoadCssFile(url, () => t.SetResult(null));
            return t.Task;
        }

        static HashSet2<string> _cssFileKeys = new HashSet2<string>();
        public static Task<object> LoadCssFile(ResourceFile resourceFile)
        {
            if (!_cssFileKeys.Contains(resourceFile.Key))
            {
                var t = new TaskCompletionSource<object>();
                INTERNAL_InteropImplementation.LoadCssFile(resourceFile.Url, () => t.SetResult(null));
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
            INTERNAL_InteropImplementation.LoadCssFiles(urlsAsList, callback);
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
            INTERNAL_InteropImplementation.LoadCssFiles(urlsAsList, callback);
        }

        /// <summary>
        /// Returns the HTML Div that is associated to the specified FrameworkElement.
        /// Note: the FrameworkElement must be in the visual tree. Consider calling this
        /// method from the 'Loaded' event to ensure that the element is in the visual tree.
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <returns></returns>
        public static object GetDiv(FrameworkElement frameworkElement)
        {
            if (frameworkElement.INTERNAL_OuterDomElement == null)
                throw new InvalidOperationException("Cannot get the Div because the control is not in the visual tree. Consider waiting until the 'FrameworkElement.Loaded' event is fired before calling this piece of code.");

            if (IsRunningInTheSimulator)
                return (INTERNAL_HtmlDomElementReference)frameworkElement.INTERNAL_OuterDomElement;
            else
                return frameworkElement.INTERNAL_OuterDomElement;
        }

        /// <summary>
        /// Returns True is the app is running in C# inside the Simulator, and False otherwise.
        /// </summary>
        public static bool IsRunningInTheSimulator
        {
            get
            {
                return INTERNAL_InteropImplementation.IsRunningInTheSimulator();
            }
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
