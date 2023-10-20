

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // BeginInvoke of the WebControl's Dispatcher
        public static Action<Action> WebControlDispatcherBeginInvoke
        {
            set;
            internal get;
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
                IWebAssemblyExecutionHandler jsRuntime = null;
                if (value is not null)
                {
                    if (value is IWebAssemblyExecutionHandler wasmHandler)
                    {
                        jsRuntime = wasmHandler;
                        INTERNAL_ExecuteJavaScript.JavaScriptRuntime =
                            new PendingJavascript(Cshtml5Initializer.PendingJsBufferSize, wasmHandler);
                    }
                    else
                    {
                        jsRuntime = new JSRuntimeWrapper(value);
                        INTERNAL_ExecuteJavaScript.JavaScriptRuntime = new PendingJavascriptSimulator(value);
                    }
                }
                
                WebAssemblyExecutionHandler = jsRuntime;
            }
        }

        internal static IWebAssemblyExecutionHandler WebAssemblyExecutionHandler
        {
            get;
            set;
        }

        private static dynamic dynamicJavaScriptExecutionHandler;

        public static dynamic DynamicJavaScriptExecutionHandler
        {
            internal get => dynamicJavaScriptExecutionHandler;
            set // Intended to be called by the "Emulator" project to inject the JavaScriptExecutionHandler.
            {
                dynamicJavaScriptExecutionHandler = value;
                if (dynamicJavaScriptExecutionHandler is not null)
                {
                    WebAssemblyExecutionHandler = new SimulatorDynamicJSRuntime(value);
                    INTERNAL_ExecuteJavaScript.JavaScriptRuntime = new PendingJavascriptSimulator(WebAssemblyExecutionHandler);
                }
                else
                {
                    WebAssemblyExecutionHandler = null;
                }
            }
        }

        static private dynamic webClientFactory;
        public static dynamic WebClientFactory
        {
            get { return webClientFactory; }
            set { webClientFactory = value; }
        }

        static dynamic clipboardHandler;
        public static dynamic ClipboardHandler
        {
            set // Intended to be called by the "Emulator" project to inject the ClipboardHandler.
            {
                clipboardHandler = value;
            }
            internal get
            {
                return clipboardHandler;
            }
        }

        static dynamic simulatorProxy;
        public static dynamic SimulatorProxy
        {
            set // Intended to be called by the "Emulator" project to inject the SimulatorProxy.
            {
                simulatorProxy = value;
            }
            internal get
            {
                return simulatorProxy;
            }
        }

        public static bool IsRunningInTheSimulator_WorkAround
        {
            get;
            set;
        }

        public static Action<object> SimulatorCallbackSetup
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
