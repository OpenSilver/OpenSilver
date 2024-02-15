
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
using OpenSilver.Internal;
using System;
using System.ComponentModel;

namespace DotNetForHtml5.Core
{
    public static class INTERNAL_Simulator
    {
        // Note: all the properties here are populated by the Simulator, which "injects" stuff here when the application is launched in the Simulator.

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static dynamic HtmlDocument { internal get; set; }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static dynamic DOMDocument { internal get; set; }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static dynamic WpfMediaElementFactory { internal get; set; }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<object, object> ConvertBrowserResult { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use System.Windows.Clipboard instead.")]
        public static dynamic ClipboardHandler { internal get; set; }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static dynamic DynamicJavaScriptExecutionHandler { internal get; set; }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Action<object> SimulatorCallbackSetup { get; set; }

        // BeginInvoke of the WebControl's Dispatcher
        public static Action<Action> WebControlDispatcherBeginInvoke
        {
            internal get;
            set;
        }
        // internal static dynamic WebControlDispatcherBeginInvoke => webControl;

        // Invoke of the WebControl's Dispatcher
        public static Action<Action, TimeSpan> WebControlDispatcherInvoke
        {
            set;
            internal get;
        }

        /// <summary>
        /// CheckAccess() of WebControl's Dispatcher.
        /// </summary>
        public static Func<bool> WebControlDispatcherCheckAccess { get; internal set; }

        public static IJavaScriptExecutionHandler JavaScriptExecutionHandler
        {
            get => WebAssemblyExecutionHandler;
            set
            {
                if (value is not null)
                {
                    OpenSilver.Interop.SetRuntime(value);
                }
                
                WebAssemblyExecutionHandler = value switch
                {
                    IWebAssemblyExecutionHandler wasmHandler => wasmHandler,
                    IJavaScriptExecutionHandler jsHandler => new JSRuntimeWrapper(jsHandler),
                    null => null,
                };
            }
        }

        internal static IWebAssemblyExecutionHandler WebAssemblyExecutionHandler { get; private set; }

        public static dynamic WebClientFactory { get; set; }

        internal static IAsyncClipboard AsyncClipboard { get; set; }

        public static dynamic SimulatorProxy { internal get; set; }

        public static bool IsRunningInTheSimulator_WorkAround
        {
            get;
            set;
        }

        public static Action<Action> OpenSilverDispatcherBeginInvoke
        {
            set;
            internal get;
        }

        public static Action<Action, TimeSpan> OpenSilverDispatcherInvoke
        {
            set;
            internal get;
        }

        public static Func<bool> OpenSilverDispatcherCheckAccess
        {
            get;
            internal set;
        }

    }
}
