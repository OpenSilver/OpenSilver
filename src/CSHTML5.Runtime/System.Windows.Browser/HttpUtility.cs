
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



using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides methods for encoding and decoding HTML and URL strings.
    /// </summary>
    //[SecuritySafeCritical]
    public static class HttpUtility
    {
        ///// <summary>
        ///// Converts a string that has been HTML-encoded (for HTTP transmission) into
        ///// a decoded string.
        ///// </summary>
        ///// <param name="html">An HTML-encoded string to decode.</param>
        ///// <returns>A decoded string.</returns>
        //[JSIL.Meta.JSReplacement("decodeURIComponent($html)")]
        //public static string HtmlDecode(string html)
        //{
        //    //Note: this is not correct, it should apparently replace the elements with &amp; and such instead of %26 and whatnot.
        //    return Interop.ExecuteJavaScript("decodeURIComponent($0)", html).ToString();
        //}

        ///// <summary>
        ///// Converts a text string into an HTML-encoded string.
        ///// </summary>
        ///// <param name="html">The text to HTML-encode.</param>
        ///// <returns>An HTML-encoded string.</returns>
        //[JSIL.Meta.JSReplacement("encodeURIComponent($html)")]
        //public static string HtmlEncode(string html)
        //{
        //    return Interop.ExecuteJavaScript("encodeURIComponent($0)", html).ToString();
        //}

        /// <summary>
        /// Converts a string that has been encoded for transmission in a URL into a
        /// decoded string.
        /// </summary>
        /// <param name="url">A URL-encoded string to decode.</param>
        /// <returns>A decoded string.</returns>
#if !BRIDGE
        [JSIL.Meta.JSReplacement("decodeURIComponent($url)")]
#else
        [Bridge.Template("decodeURIComponent({url})")]
#endif
        public static string UrlDecode(string url)
        {
            return Interop.ExecuteJavaScript("decodeURIComponent($0)", url).ToString();
        }

        /// <summary>
        /// Converts a text string into a URL-encoded string.
        /// </summary>
        /// <param name="url">The text to URL-encode.</param>
        /// <returns>A URL-encoded string.</returns>
#if !BRIDGE
        [JSIL.Meta.JSReplacement("encodeURIComponent($url)")]
#else
        [Bridge.Template("encodeURIComponent({url})")]
#endif
        public static string UrlEncode(string url)
        {
            return Interop.ExecuteJavaScript("encodeURIComponent($0)", url).ToString();
        }
    }
}
