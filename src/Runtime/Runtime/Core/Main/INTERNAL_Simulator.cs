

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Core
{
#if CSHTML5NETSTANDARD //todo: remove this directive and use the "InternalsVisibleTo" attribute instead.
    public
#else
    internal
#endif
        static class INTERNAL_Simulator
    {
        // Note: all the properties here are populated by the Simulator, which "injects" stuff here when the application is launched in the Simulator.

        static dynamic htmlDocument;
        public static dynamic HtmlDocument
        {
            set // Intended to be called by the "Emulator" project to inject the HTML document.
            {
                htmlDocument = value;
            }
            internal get
            {
                return htmlDocument;
            }
        }

        // Here we get the Document from DotNetBrowser
        static dynamic domDocument;
        public static dynamic DOMDocument
        {
            set // Intended to be called by the "Emulator" project to inject the Document.
            {
                domDocument = value;
            }
            internal get
            {
                return domDocument;
            }
        }

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

#if CSHTML5NETSTANDARD
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
                        INTERNAL_SimulatorExecuteJavaScript.JavaScriptRuntime =
                            new PendingJavascript(Cshtml5Initializer.PendingJsBufferSize, wasmHandler);
                    }
                    else
                    {
                        jsRuntime = new JSRuntimeWrapper(value);
                        INTERNAL_SimulatorExecuteJavaScript.JavaScriptRuntime = new PendingJavascriptSimulator(value);
                    }
                }
                
                WebAssemblyExecutionHandler = jsRuntime;
            }
        } // Intended to be injected when the app is initialized.

        internal static IWebAssemblyExecutionHandler WebAssemblyExecutionHandler
        {
            get;
            set;
        }
#endif

        static dynamic dynamicJavaScriptExecutionHandler;

        public static dynamic DynamicJavaScriptExecutionHandler
        {
            internal get => dynamicJavaScriptExecutionHandler;
            set // Intended to be called by the "Emulator" project to inject the JavaScriptExecutionHandler.
            {
                dynamicJavaScriptExecutionHandler = value;
                if (dynamicJavaScriptExecutionHandler is not null)
                {
                    WebAssemblyExecutionHandler = new SimulatorDynamicJSRuntime(value);
                    INTERNAL_SimulatorExecuteJavaScript.JavaScriptRuntime = new PendingJavascriptSimulator(WebAssemblyExecutionHandler);
                }
                else
                {
                    WebAssemblyExecutionHandler = null;
                }
            }
        }

        static dynamic wpfMediaElementFactory;
        public static dynamic WpfMediaElementFactory
        {
            set // Intended to be called by the "Emulator" project to inject the WpfMediaElementFactory.
            {
                wpfMediaElementFactory = value;
            }
            internal get
            {
                return wpfMediaElementFactory;
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

#if CSHTML5BLAZOR
        // In OpenSilver Version, we use this work-around to know if we're in the simulator
        static bool isRunningInTheSimulator_WorkAround = false;

        public static bool IsRunningInTheSimulator_WorkAround
        {
            set // Intended to be setted by the "Emulator" project.
            {
                isRunningInTheSimulator_WorkAround = value; 
            }
            get 
            { 
                return isRunningInTheSimulator_WorkAround; 
            }
        }
#endif

        public static Func<object, object> ConvertBrowserResult { get; set; }
    }
}
