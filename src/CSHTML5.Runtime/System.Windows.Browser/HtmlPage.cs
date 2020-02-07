
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

        public static bool IsEnabled { get; private set; }

        public static BrowserInformation BrowserInformation { get; private set; }
#endif

    }
}
