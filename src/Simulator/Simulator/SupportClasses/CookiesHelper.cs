using DotNetBrowser.WPF;
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
    }
}
