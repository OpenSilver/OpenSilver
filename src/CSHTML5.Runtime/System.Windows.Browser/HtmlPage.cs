
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
using System.Windows.Browser;

namespace System.Windows.Browser
{
    public static class HtmlPage
    {
        static HtmlWindow _initWindow;
        static HtmlDocument _initDocument;
#if WORKINPROGRESS
        static HtmlElement _initPlugin;
#endif
        /// <summary>
        /// Gets the browser's window object.
        /// </summary>
        public static HtmlWindow Window
        {
            get
            {
                if (_initWindow == null)
                {
                    _initWindow = new HtmlWindow();
                }
                return _initWindow;
            }
        }

        /// <summary>
        /// Gets the browser's document object.
        /// </summary>
        public static HtmlDocument Document
        {
            get
            {
                if (_initDocument == null)
                {
                    _initDocument = new HtmlDocument();
                }
                return _initDocument;
            }
        }
        #if WORKINPROGRESS
        //
        // Summary:
        //     Gets a reference to the Silverlight plug-in that is defined within an <object>
        //     or <embed> tag on the host HTML page.
        //
        // Returns:
        //     The Silverlight plug-in in the Document Object Model (DOM).
        public static HtmlElement Plugin
        {
            get
            {
                if (_initPlugin == null)
                {
                    _initPlugin = new HtmlElement();
                }
                return _initPlugin;
            }
        }
        #endif

    }
}
