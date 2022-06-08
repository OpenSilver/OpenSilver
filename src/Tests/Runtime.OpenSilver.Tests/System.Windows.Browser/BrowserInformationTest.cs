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


using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.Json;
using System.Windows.Browser;

namespace Runtime.OpenSilver.Tests.System.Windows.Browser
{
    [TestClass]
    public class BrowserInformationTest
    {
        private const string ChromeUserAgent =
            "Mozilla/5.27 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36";

        private const string SafariUserAgent =
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.5 Safari/605.1.15";

        private const string OperaUserAgent =
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 OPR/86.0.4363.59";

        private const string FirefoxUserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:101.0) Gecko/20100101 Firefox/101.0";

        private const string EdgeUserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36 Edg/102.0.1245.33";

        private const string Platform = "Win32";

        private static string _userAgent = ChromeUserAgent;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            TestSetup.ExecuteJavascript += HandleJavascriptCall;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestSetup.ExecuteJavascript -= HandleJavascriptCall;
        }

        private static void HandleJavascriptCall(object sender, ExecuteJavascriptEventArgs args)
        {
            var js = args.Javascript;
            var lastLine = js.Split(
                new [] { Environment.NewLine },
                StringSplitOptions.None
            ).Last();
            string result = null;
            if (lastLine.Contains("navigator.userAgent"))
            {
                result = _userAgent;
            } else if (lastLine.Contains("navigator.cookieEnabled"))
            {
                result = "true";
            } else if (lastLine.Contains("navigator.platform"))
            {
                result = Platform;
            }

            if (result == null)
            {
                return;
            }

            args.Handled = true;
            args.Result = JsonDocument.Parse($"\"{result}\"").RootElement;
        }

        [TestMethod]
        public void CookiesEnabled_Should_Return_True()
        {
            HtmlPage.BrowserInformation.CookiesEnabled.Should().BeTrue();
        }

        [TestMethod]
        public void ProductName_Should_Return_Value()
        {
            HtmlPage.BrowserInformation.ProductName.Should().Be("Mozilla");
        }

        [TestMethod]
        public void ProductVersion_Should_Return_Empty_String()
        {
            //Silverlight always returns "" according to source code
            HtmlPage.BrowserInformation.ProductVersion.Should().Be("");
        }

        [TestMethod]
        public void BrowserVersion_Should_Return_Value()
        {
            HtmlPage.BrowserInformation.BrowserVersion.Should().Be(new Version(5, 27));
        }

        [TestMethod]
        public void Name_Should_Return_Value()
        {
            HtmlPage.BrowserInformation.Name.Should().Be("Chrome");
        }

        [TestMethod]
        public void Name_Should_Return_Safari()
        {
            var initUserAgent = _userAgent;
            _userAgent = SafariUserAgent;
            try
            {
                new BrowserInformation().Name.Should().Be("Safari");
            }
            finally
            {
                _userAgent = initUserAgent;
            }
        }

        [TestMethod]
        public void Name_Should_Return_Opera()
        {
            var initUserAgent = _userAgent;
            _userAgent = OperaUserAgent;
            try
            {
                new BrowserInformation().Name.Should().Be("Opera");
            }
            finally
            {
                _userAgent = initUserAgent;
            }
        }

        [TestMethod]
        public void Name_Should_Return_Firefox()
        {
            var initUserAgent = _userAgent;
            _userAgent = FirefoxUserAgent;
            try
            {
                new BrowserInformation().Name.Should().Be("Firefox");
            }
            finally
            {
                _userAgent = initUserAgent;
            }
        }

        [TestMethod]
        public void Name_Should_Return_Edge()
        {
            var initUserAgent = _userAgent;
            _userAgent = EdgeUserAgent;
            try
            {
                new BrowserInformation().Name.Should().Be("Edge");
            }
            finally
            {
                _userAgent = initUserAgent;
            }
        }

        [TestMethod]
        public void Name_Should_Return_Netscape()
        {
            var initUserAgent = _userAgent;
            _userAgent = "Mozilla/5.0 Unknown";
            try
            {
                new BrowserInformation().Name.Should().Be("Netscape");
            }
            finally
            {
                _userAgent = initUserAgent;
            }
        }

        [TestMethod]
        public void Platform_Should_Return_Value()
        {
            HtmlPage.BrowserInformation.Platform.Should().Be(Platform);
        }

        [TestMethod]
        public void UserAgent_Should_Return_Value()
        {
            HtmlPage.BrowserInformation.UserAgent.Should().Be(_userAgent);
        }
    }
}
