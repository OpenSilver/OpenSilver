

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using Microsoft.Web.WebView2.Core;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public struct CookieData
    {
        public string url;
        public string domain;
        public string name;
        public string path;
        public bool secure;
        public bool httpOnly;
        public bool session;
        public string value;
        public DateTime expirationTime;

        public CookieData(CoreWebView2Cookie cookie, string url)
        {
            this.url = url;
            this.name = cookie.Name;
            this.path = cookie.Path;
            this.secure = cookie.IsSecure;
            this.httpOnly = cookie.IsHttpOnly;
            this.value = cookie.Value;
            this.domain = cookie.Domain;
            this.expirationTime = cookie.Expires;
            this.session = cookie.IsSession;
        }
    }
}
