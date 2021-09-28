
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

using HttpUtilityStd = System.Web.HttpUtility;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides methods for encoding and decoding HTML and URL strings.
    /// </summary>
    public static class HttpUtility
    {
        /// <summary>
        /// Converts a string that has been HTML-encoded (for HTTP transmission) into a decoded
        /// string.
        /// </summary>
        /// <param name="html">An HTML-encoded string to decode.</param>
        /// <returns>A decoded string.</returns>
        public static string HtmlDecode(string html)
        {
            return HttpUtilityStd.HtmlDecode(html);
        }

        /// <summary>
        /// Converts a text string into an HTML-encoded string.
        /// </summary>
        /// <param name="html">The text to HTML-encode.</param>
        /// <returns>An HTML-encoded string.</returns>
        public static string HtmlEncode(string html)
        {
            return HttpUtilityStd.HtmlEncode(html);
        }

        /// <summary>
        /// Converts a string that has been encoded for transmission in a URL into a decoded
        /// string.
        /// </summary>
        /// <param name="url">A URL-encoded string to decode.</param>
        /// <returns>A decoded string.</returns>
        public static string UrlDecode(string url)
        {
            return HttpUtilityStd.UrlDecode(url);
        }

        /// <summary>
        /// Converts a text string into a URL-encoded string.
        /// </summary>
        /// <param name="url">The text to URL-encode.</param>
        /// <returns>A URL-encoded string.</returns>
        public static string UrlEncode(string url)
        {
            return HttpUtilityStd.UrlEncode(url);
        }
    }
}
