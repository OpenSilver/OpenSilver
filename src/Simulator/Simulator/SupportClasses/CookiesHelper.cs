

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



using DotNetBrowser.Browser;
using DotNetBrowser.Cookies;
using OpenSilver;
using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal static class CookiesHelper
    {
        const string MICROSOFT_COOKIES_URL = "https://login.microsoftonline.com";

        public static void LoadMicrosoftCookies(IBrowser browser, string registryName)
        {
            LoadCookies(browser, MICROSOFT_COOKIES_URL, registryName);
        }

        public static void LoadCookies(IBrowser browser, string url, string registryName)
        {
            // we search for cookies with a specific url into registries
            string cookiesAsString = RegistryHelpers.GetSetting(registryName + "_" + url, null);
            if (cookiesAsString != null)
            {
                List<CookieData> cookiesList = Serializer.Load<List<CookieData>>(cookiesAsString);
                foreach (var cookie in cookiesList)
                {
                    var builder = new Cookie.Builder(cookie.domain)
                    {
                        Name = cookie.name,
                        Value = cookie.value,
                        Path = cookie.path,
                        Secure = cookie.secure,
                        HttpOnly = cookie.httpOnly
                    };

                    if (!cookie.session)
                    {
                        builder.ExpirationTime = cookie.expirationTime;
                    }

                    browser.Engine.Profiles.Default.CookieStore.SetCookie(builder.Build()).Wait();
                }
            }
        }

        public static void SaveMicrosoftCookies(IBrowser browser, string registryName)
        {
            SaveCookies(browser, MICROSOFT_COOKIES_URL, registryName);
        }

        public static void SaveCookies(IBrowser browser, string url, string registryName)
        {
            // we register cookies with a specific url into registries
            if (url != null && browser != null && browser.Engine.Profiles.Default.CookieStore != null)
            {
                IEnumerable<Cookie> cookiesList = browser.Engine.Profiles.Default.CookieStore.GetAllCookies(url).Result;
                List<CookieData> cookiesDataList = new List<CookieData>();
                foreach (var cookie in cookiesList)
                    cookiesDataList.Add(new CookieData(cookie, url));
                string cookiesAsString = Serializer.Save<List<CookieData>>(cookiesDataList);
                RegistryHelpers.SaveSetting(registryName + "_" + url, cookiesAsString);
            }
        }

        public static void ClearCookies(IBrowser browser, string registryName = null)
        {
            if (!string.IsNullOrEmpty(registryName))
            {
                RegistryHelpers.DeleteSetting(registryName + "_" + MICROSOFT_COOKIES_URL);
            }
            if (browser != null && browser.Engine.Profiles.Default.CookieStore != null)
            {
                int numberOfDeletedCookies = browser.Engine.Profiles.Default.CookieStore.DeleteAllCookies().Result;
                browser.Engine.Profiles.Default.CookieStore.Save();
                MessageBox.Show(numberOfDeletedCookies.ToString() + " cookies have been deleted.");
            }
        }

        public static void SetCustomCookies(IBrowser browser, IList<CookieData> cookies)
        {
            if (cookies == null)
                return;

            foreach (CookieData data in cookies)
            {
                var builder = new Cookie.Builder(data.domain)
                {
                    Name = data.name,
                    Value = data.value,
                    Path = data.path,
                    Secure = data.secure,
                    HttpOnly = data.httpOnly
                };

                if (!data.session)
                {
                    builder.ExpirationTime = data.expirationTime;
                }

                browser.Engine.Profiles.Default.CookieStore.SetCookie(builder.Build()).Wait();
            }
        }

    }
}
