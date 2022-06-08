

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
    public sealed class BrowserInformation
    {
        private string _name;

        private string _platform;

        private string _productName;

        private Version _browserVersion;

        private string _userAgent;

        //
        // Summary:
        //     Gets the version of the browser technology that the current browser is based
        //     on.
        //
        // Returns:
        //     The version of the underlying browser technology.
        public Version BrowserVersion
        {
            get
            {
                if (_browserVersion != null)
                {
                    return _browserVersion;
                }

                //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent#syntax
                var filter = new Regex(@"(\d+(?:\.\d+)+)");
                var match = filter.Match(UserAgent);
                _browserVersion = match.Success ? Version.Parse(match.Value) : new Version();

                return _browserVersion;
            }
        }

        //
        // Summary:
        //     Gets a value that indicates whether the browser supports cookies.
        //
        // Returns:
        //     true if the browser supports cookies; otherwise, false.
        public bool CookiesEnabled =>
            Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("navigator.cookieEnabled"));

        //
        // Summary:
        //     Gets the name of the browser technology that the current browser is based on.
        //
        // Returns:
        //     The name of the underlying browser technology.
        public string Name {
            get
            {
                if (_name != null)
                {
                    return _name;
                }

                var patterns = new Dictionary<string, string>
                {
                    //Order is important
                    { "opr/|opera", "Opera" },
                    { "edg", "Edge" },
                    { "chrome|chromium|crios", "Chrome" },
                    { "firefox|fxios", "Firefox" },
                    { "safari", "Safari" }
                };

                foreach (var kvp in patterns)
                {
                    if (!Regex.IsMatch(UserAgent, kvp.Key, RegexOptions.IgnoreCase))
                    {
                        continue;
                    }

                    _name = kvp.Value;
                    break;
                }

                return _name ?? (_name = "Netscape");
            }
        }

        //
        // Summary:
        //     Gets the name of the browser operating system.
        //
        // Returns:
        //     The name of the operating system that the browser is running on.
        public string Platform => _platform ??
                                  (_platform =
                                      Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("navigator.platform")));

        //
        // Summary:
        //     Gets the product name of the browser.
        //
        // Returns:
        //     The product name of the browser.
        public string ProductName
        {
            get
            {
                if (_productName != null)
                {
                    return _productName;
                }

                //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent#syntax
                _productName = UserAgent.Split('/')[0];

                return _productName;
            }
        }

        //
        // Summary:
        //     Gets the product version number of the browser.
        //
        // Returns:
        //     The product version number of the browser.
        public string ProductVersion => "";

        //
        // Summary:
        //     Gets the user agent string of the browser.
        //
        // Returns:
        //     The user agent string that identifies the browser.
        public string UserAgent => _userAgent ?? (_userAgent = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("navigator.userAgent")));
    }
}
