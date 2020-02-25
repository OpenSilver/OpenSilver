﻿
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

#if CSHTML5NETSTANDARD
        public static IJavaScriptExecutionHandler JavaScriptExecutionHandler 
        { 
            get;
            set; 
        } // Intended to be injected when the app is initialized.
#endif

#if CSHTML5NETSTANDARD
        static dynamic dynamicJavaScriptExecutionHandler;
#else
        static dynamic javaScriptExecutionHandler;
#endif

#if CSHTML5NETSTANDARD
        public static dynamic DynamicJavaScriptExecutionHandler
#else
        public static dynamic JavaScriptExecutionHandler
#endif
        {
            set // Intended to be called by the "Emulator" project to inject the JavaScriptExecutionHandler.
            {
#if CSHTML5NETSTANDARD
                dynamicJavaScriptExecutionHandler = value;
#else
                javaScriptExecutionHandler = value;
#endif
            }
            internal get
            {
#if CSHTML5NETSTANDARD
                return dynamicJavaScriptExecutionHandler;
#else
                return javaScriptExecutionHandler;
#endif
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
    }
}
