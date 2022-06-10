
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

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides general information about the browser, such as name, version, and operating
    /// system.
    /// </summary>
    public sealed class BrowserInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserInformation"/> class.
        /// </summary>
        internal BrowserInformation(string userAgent, string platform)
        {
            UserAgent = userAgent;
            Platform = platform;

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent#syntax
            BrowserVersion = GetVersion(userAgent);
            Name = GetName(userAgent);
            ProductName = GetProductName(userAgent);
            ProductVersion = string.Empty;
        }

        /// <summary>
        /// Gets the version of the browser technology that the current browser is based on.
        /// </summary>
        public Version BrowserVersion { get; }

        /// <summary>
        /// Gets the name of the browser technology that the current browser is based on.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the browser operating system.
        /// </summary>
        public string Platform { get; }

        /// <summary>
        /// Gets the product name of the browser.
        /// </summary>
        public string ProductName { get; }

        /// <summary>
        /// Gets the product version number of the browser.
        /// </summary>
        public string ProductVersion { get; }

        /// <summary>
        /// Gets the user agent string of the browser.
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        /// Gets a value that indicates whether the browser supports cookies.
        /// </summary>
        public bool CookiesEnabled =>
            Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("navigator.cookieEnabled"));

        private static Version GetVersion(string userAgent)
        {
            var filter = new Regex(@"(\d+(?:\.\d+)+)");
            var match = filter.Match(userAgent);

            return match.Success ? Version.Parse(match.Value) : new Version();
        }

        private static string GetName(string userAgent)
        {
            var patterns = new Dictionary<string, string>
            {
                // Order is important
                { "opr/|opera", "Opera" },
                { "edg", "Edge" },
                { "chrome|chromium|crios", "Chrome" },
                { "firefox|fxios", "Firefox" },
                { "safari", "Safari" }
            };

            foreach (var kvp in patterns)
            {
                if (Regex.IsMatch(userAgent, kvp.Key, RegexOptions.IgnoreCase))
                {
                    return kvp.Value;
                }
            }

            return "Netscape";
        }

        private static string GetProductName(string userAgent)
        {
            return userAgent.Split('/')[0];
        }
    }
}
