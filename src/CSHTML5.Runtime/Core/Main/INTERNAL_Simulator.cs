
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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

#if !CSHTML5NETSTANDARD
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
#endif

#if !CSHTML5NETSTANDARD
        static dynamic webControl;
        public static dynamic WebControl
        {
            set // Intended to be called by the "Emulator" project to inject the WebControl.
            {
                webControl = value;
            }
            internal get
            {
                return webControl;
            }
        }
#endif

#if CSHTML5NETSTANDARD
        public static IJavaScriptExecutionHandler JavaScriptExecutionHandler { get; set; } // Intended to be injected when the app is initialized.
#else
        static dynamic javaScriptExecutionHandler;
        public static dynamic JavaScriptExecutionHandler
        {
            set // Intended to be called by the "Emulator" project to inject the JavaScriptExecutionHandler.
            {
                javaScriptExecutionHandler = value;
            }
            internal get
            {
                return javaScriptExecutionHandler;
            }
        }
#endif

#if !CSHTML5NETSTANDARD
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
#endif

#if !CSHTML5NETSTANDARD
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
#endif

#if !CSHTML5NETSTANDARD
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
#endif
    }
}
