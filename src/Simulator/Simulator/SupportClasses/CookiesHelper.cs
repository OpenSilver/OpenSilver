

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



using DotNetBrowser;
using DotNetBrowser.WPF;
using OpenSilver;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal static class CookiesHelper
    {
        const string MICROSOFT_COOKIES_URL = "https://login.microsoftonline.com";

        public static void LoadMicrosoftCookies(WPFBrowserView wpfBrowserView, string registryName)
        {
            LoadCookies(wpfBrowserView, MICROSOFT_COOKIES_URL, registryName);
        }

        public static void LoadCookies(WPFBrowserView wpfBrowserView, string url, string registryName)
        {
            // we search for cookies with a specific url into registries
            string cookiesAsString = RegistryHelpers.GetSetting(registryName + "_" + url, null);
            if (cookiesAsString != null)
            {
                List<CookieData> cookiesList = Serializer.Load<List<CookieData>>(cookiesAsString);
                foreach (var cookie in cookiesList)
                {
                    if (cookie.session)
                        wpfBrowserView.Browser.CookieStorage.SetSessionCookie(cookie.url, cookie.name, cookie.value, cookie.domain, cookie.path, cookie.secure, cookie.httpOnly);
                    else
                        wpfBrowserView.Browser.CookieStorage.SetCookie(cookie.url, cookie.name, cookie.value, cookie.domain, cookie.path, cookie.expirationTime, cookie.secure, cookie.httpOnly);
                }
            }
        }

        public static void SaveMicrosoftCookies(WPFBrowserView wpfBrowserView, string registryName)
        {
            SaveCookies(wpfBrowserView, MICROSOFT_COOKIES_URL, registryName);
        }

        public static void SaveCookies(WPFBrowserView wpfBrowserView, string url, string registryName)
        {
            // we register cookies with a specific url into registries
            if (url != null && wpfBrowserView.Browser != null && wpfBrowserView.Browser.CookieStorage != null)
            {
                List<DotNetBrowser.Cookie> cookiesList = wpfBrowserView.Browser.CookieStorage.GetAllCookies(url);
                List<CookieData> cookiesDataList = new List<CookieData>();
                foreach (var cookie in cookiesList)
                    cookiesDataList.Add(new CookieData(cookie, url));
                string cookiesAsString = Serializer.Save<List<CookieData>>(cookiesDataList);
                RegistryHelpers.SaveSetting(registryName + "_" + url, cookiesAsString);
            }
        }

        public static void ClearCookies(WPFBrowserView wpfBrowserView, string registryName = null)
        {
            if (!string.IsNullOrEmpty(registryName))
            {
                RegistryHelpers.DeleteSetting(registryName + "_" + MICROSOFT_COOKIES_URL);
            }
            if (wpfBrowserView.Browser != null && wpfBrowserView.Browser.CookieStorage != null)
            {
                int numberOfDeletedCookies = wpfBrowserView.Browser.CookieStorage.DeleteAll();
                wpfBrowserView.Browser.CookieStorage.Save();
                MessageBox.Show(numberOfDeletedCookies.ToString() + " cookies have been deleted.");
            }
        }

        public static void SetCustomCookies(WPFBrowserView wpfBrowserView, IList<CookieData> cookies)
        {
            if (cookies == null)
                return;

            foreach (CookieData data in cookies)
            {
                if (data.session)
                {
                    wpfBrowserView.Browser.CookieStorage.SetSessionCookie(data.url, data.name, data.value, data.domain, data.path, data.secure, data.httpOnly);
                }
                else
                {
                    wpfBrowserView.Browser.CookieStorage.SetCookie(data.url, data.name, data.value, data.domain, data.path, data.expirationTime, data.secure, data.httpOnly);
                }
            }
        }

    }
}
