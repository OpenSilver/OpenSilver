

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





using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using DotNetForHtml5;

namespace OpenSilver.Simulator
{
    internal static class CookiesHelper
    {
        const string MICROSOFT_COOKIES_URL = "https://login.microsoftonline.com";

        public static void LoadMicrosoftCookies(WebView2 WebView2, string registryName)
        {
            LoadCookies(WebView2, MICROSOFT_COOKIES_URL, registryName);
        }

        public static void LoadCookies(WebView2 browser, string url, string registryName)
        {
            // we search for cookies with a specific url into registries
            string cookiesAsString = RegistryHelpers.GetSetting(registryName + "_" + url, null);
            if (cookiesAsString != null)
            {
                List<CoreWebView2Cookie> cookiesList = Serializer.Load<List<CoreWebView2Cookie>>(cookiesAsString);
                foreach (var cookie in cookiesList)
                {
                    browser.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);
                    //var wvCookie = browser.CoreWebView2.CookieManager.CreateCookie(cookie.name, cookie.value, cookie.domain, cookie.url);
                    //wvCookie.Expires = cookie.session ? System.DateTime.MinValue : ;

                    //if (cookie.session)
                    //    WebView2.Browser.CookieStorage.SetSessionCookie(cookie.url, cookie.name, cookie.value, cookie.domain, cookie.path, cookie.secure, cookie.httpOnly);
                    //else
                    //    WebView2.Browser.CookieStorage.SetCookie(cookie.url, cookie.name, cookie.value, cookie.domain, cookie.path, cookie.expirationTime, cookie.secure, cookie.httpOnly);
                }
            }
        }

        public static void SaveMicrosoftCookies(WebView2 WebView2, string registryName)
        {
            SaveCookies(WebView2, MICROSOFT_COOKIES_URL, registryName);
        }

        public static async void SaveCookies(WebView2 browser, string url, string registryName)
        {
            // we register cookies with a specific url into registries
            if (url != null && browser != null)
            {
                var cookiesList = await browser.CoreWebView2.CookieManager.GetCookiesAsync(url);
                string cookiesAsString = Serializer.Save<List<CoreWebView2Cookie>>(cookiesList);
                RegistryHelpers.SaveSetting(registryName + "_" + url, cookiesAsString);
            }
        }

        public static void ClearCookies(WebView2 browser, string registryName = null)
        {
            if (!string.IsNullOrEmpty(registryName))
            {
                RegistryHelpers.DeleteSetting(registryName + "_" + MICROSOFT_COOKIES_URL);
            }

            if (browser != null)
            {
                browser.CoreWebView2.CookieManager.DeleteAllCookies();
                MessageBox.Show("All cookies have been deleted.");
            }
        }

        public static void SetCustomCookies(WebView2 browser, IList<CookieData> cookies)
        {
            if (cookies == null)
                return;

            foreach (var cookie in cookies)
            {
                var browserCookie = browser.CoreWebView2.CookieManager.CreateCookie(cookie.name, cookie.value, cookie.domain, cookie.path);
                browser.CoreWebView2.CookieManager.AddOrUpdateCookie(browserCookie);
                //ams> the default of browserCookie.IsSession=true // how to make a cookie not a session cookie!
            }
        }
    }
}
