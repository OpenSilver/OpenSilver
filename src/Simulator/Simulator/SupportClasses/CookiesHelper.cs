

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
using Microsoft.Web.WebView2.Wpf;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal static class CookiesHelper
    {
        public static void SetCustomCookies(WebView2 wpfWebView, IList<CookieData> cookies)
        {
            if (cookies == null)
            {
                return;
            }

            var cookieManager = wpfWebView.CoreWebView2?.CookieManager;
            if (cookieManager == null)
            {
                return;
            }

            foreach (CookieData cookie in cookies)
            {
                CoreWebView2Cookie coreCookie = cookieManager.CreateCookie(cookie.name, cookie.value, cookie.domain, cookie.path);
                coreCookie.IsSecure = cookie.secure;
                coreCookie.IsHttpOnly = cookie.httpOnly;
                coreCookie.SameSite = cookie.sameSite;

                if (!cookie.session)
                {
                    coreCookie.Expires = cookie.expirationTime;
                }

                cookieManager.AddOrUpdateCookie(coreCookie);
            }
        }
    }
}
