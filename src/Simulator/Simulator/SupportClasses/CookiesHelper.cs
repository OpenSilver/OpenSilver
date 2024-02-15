

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
using OpenSilver;
using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal static class CookiesHelper
    {
        const string MICROSOFT_COOKIES_URL = "https://login.microsoftonline.com";

        public static void LoadMicrosoftCookies(WebView2 wpfWebView, string registryName)
        {
            LoadCookies(wpfWebView, MICROSOFT_COOKIES_URL, registryName);
        }

        public static void LoadCookies(WebView2 wpfWebView, string url, string registryName)
        {
            // we search for cookies with a specific url into registries
            string cookiesAsString = RegistryHelpers.GetSetting(registryName + "_" + url, null);
            if (cookiesAsString != null)
            {
                var cookieManager = wpfWebView.CoreWebView2.CookieManager;
                if (cookieManager != null)
                {
                    List<CookieData> cookiesList = Serializer.Load<List<CookieData>>(cookiesAsString);
                    foreach (var cookie in cookiesList)
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

        public static void SaveMicrosoftCookies(WebView2 wpfWebView, string registryName)
        {
            SaveCookies(wpfWebView, MICROSOFT_COOKIES_URL, registryName);
        }

        public static void SaveCookies(WebView2 wpfWebView, string url, string registryName)
        {
            // we register cookies with a specific url into registries
            if (url != null && wpfWebView.CoreWebView2?.CookieManager != null)
            {
                List<CoreWebView2Cookie> cookiesList = wpfWebView.CoreWebView2?.CookieManager.GetCookiesAsync(url).GetAwaiter().GetResult();
                List<CookieData> cookiesDataList = new List<CookieData>();
                foreach (var cookie in cookiesList)
                    cookiesDataList.Add(new CookieData(cookie, url));
                string cookiesAsString = Serializer.Save<List<CookieData>>(cookiesDataList);
                RegistryHelpers.SaveSetting(registryName + "_" + url, cookiesAsString);
            }
        }

        public static void ClearCookies(WebView2 wpfWebView, string registryName = null)
        {
            if (!string.IsNullOrEmpty(registryName))
            {
                RegistryHelpers.DeleteSetting(registryName + "_" + MICROSOFT_COOKIES_URL);
            }
            if (wpfWebView.CoreWebView2?.CookieManager != null)
            {
                wpfWebView.CoreWebView2.CookieManager.DeleteAllCookies();
                MessageBox.Show("All cookies have been deleted.");
            }
        }

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
