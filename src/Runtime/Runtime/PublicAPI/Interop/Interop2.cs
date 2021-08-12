

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
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if OPENSILVER
namespace OpenSilver
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
        public static object ExecuteJavaScript(string javascript)
        {
            return CSHTML5.Interop.ExecuteJavaScript(javascript);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <param name="variables">The objects to use inside the JavaScript call.</param>
        /// <returns>The result, if any, of the JavaScript call.</returns>
        public static object ExecuteJavaScript(string javascript, params object[] variables)
        {
            return CSHTML5.Interop.ExecuteJavaScript(javascript, variables);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        public static object ExecuteJavaScriptAsync(string javascript)
        {
            return CSHTML5.Interop.ExecuteJavaScriptAsync(javascript);
        }

        /// <summary>
        /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
        /// </summary>
        /// <param name="javascript">The JavaScript code to execute.</param>
        /// <param name="variables">The objects to use inside the JavaScript call.</param>
        public static object ExecuteJavaScriptAsync(string javascript, params object[] variables)
        {
            return CSHTML5.Interop.ExecuteJavaScriptAsync(javascript, variables);
        }

        /// <summary>
        /// Unboxes the value passed as a parameter. It is particularly useful for the variables of the ExecuteJavaScript Methods calls aimed at using third party libraries.
        /// </summary>
        /// <param name="value">The value to unbox.</param>
        /// <returns>the unboxed value if the value was boxed, the value itself otherwise.</returns>
        public static object Unbox(object value)
        {
            return CSHTML5.Interop.Unbox(value);
        }

        /// <summary>
        /// Adds a 'script' tag to the HTML page and waits for the script to finish loading.
        /// </summary>
        /// <param name="url">The URL of the JavaScript file, with the syntax ms-appx:///AssemblyName/Folder/FileName.js or /AssemblyName;component/Folder/FileName.js</param>
        /// <returns>Nothing.</returns>
        public static Task<object> LoadJavaScriptFile(string url)
        {
            return CSHTML5.Interop.LoadJavaScriptFile(url);
        }

        public static Task<object> LoadJavaScriptFile(CSHTML5.Interop.ResourceFile resourceFile)
        {
            return CSHTML5.Interop.LoadJavaScriptFile(resourceFile);
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
            CSHTML5.Interop.LoadJavaScriptFilesAsync(urls, callback, callbackOnError);
        }

        /// <summary>
        /// Adds a 'link' tag to the HTML page and waits for the CSS file to finish loading.
        /// </summary>
        /// <param name="url">The URL of the CSS file, with the syntax ms-appx:///AssemblyName/Folder/FileName.css or /AssemblyName;component/Folder/FileName.css</param>
        /// <returns>Nothing</returns>
        public static Task<object> LoadCssFile(string url)
        {
            return CSHTML5.Interop.LoadCssFile(url);
        }

        public static Task<object> LoadCssFile(CSHTML5.Interop.ResourceFile resourceFile)
        {
            return CSHTML5.Interop.LoadCssFile(resourceFile);
        }


        /// <summary>
        /// Adds 'link' tags to the HTML page and waits for the CSS files to finish loading.
        /// </summary>
        /// <param name="urls">The URL of the CSS files, with the syntax ms-appx:///AssemblyName/Folder/FileName.css or /AssemblyName;component/Folder/FileName.css</param>
        /// <param name="callback">The method that is called when the CSS files have finished loading.</param>
        public static void LoadCssFilesAsync(IEnumerable<string> urls, Action callback)
        {
            CSHTML5.Interop.LoadCssFilesAsync(urls, callback);
        }

        public static void LoadCssFilesAsync(IEnumerable<CSHTML5.Interop.ResourceFile> resourceFiles, Action callback)
        {
            CSHTML5.Interop.LoadCssFilesAsync(resourceFiles, callback);
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
            return CSHTML5.Interop.GetDiv(frameworkElement);
        }

        /// <summary>
        /// Returns True is the app is running in C#, and False otherwise. 
        /// </summary>
        public static bool IsRunningInTheSimulator
        {
            get
            {
                return CSHTML5.Interop.IsRunningInTheSimulator;
            }
        }

        // For backwards compatibility
        /// <summary>
        /// Returns True is the app is running inside the Simulator, and False otherwise.
        /// </summary>
        public static bool IsRunningInTheSimulator_WorkAround
        {
            get
            {
                return CSHTML5.Interop.IsRunningInTheSimulator_WorkAround;
            }
        }


        /// <summary>
        /// Check if the given jsnode is undefined
        /// </summary>
        public static bool IsUndefined(object jsObject)
        {
            return CSHTML5.Interop.IsUndefined(jsObject);
        }

        /// <summary>
        /// Check if the given jsnode is undefined
        /// </summary>
        public static bool IsNull(object jsObject)
        {
            return CSHTML5.Interop.IsNull(jsObject);
        }
    }
}
#endif
