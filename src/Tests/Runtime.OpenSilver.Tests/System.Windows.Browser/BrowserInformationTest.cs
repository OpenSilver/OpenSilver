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
            object result = null;
            if (lastLine.Contains("navigator.userAgent"))
            {
                result = JsonDocument.Parse($"\"{ChromeUserAgent}\"").RootElement;
            }
            else if (lastLine.Contains("navigator.cookieEnabled"))
            {
                result = true;
            }
            else if (lastLine.Contains("navigator.platform"))
            {
                result = JsonDocument.Parse($"\"{Platform}\"").RootElement;
            }

            if (result == null)
            {
                return;
            }

            args.Handled = true;
            args.Result = result;
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
            new BrowserInformation(SafariUserAgent, Platform).Name.Should().Be("Safari");
        }

        [TestMethod]
        public void Name_Should_Return_Opera()
        {
            new BrowserInformation(OperaUserAgent, Platform).Name.Should().Be("Opera");
        }

        [TestMethod]
        public void Name_Should_Return_Firefox()
        {
            new BrowserInformation(FirefoxUserAgent, Platform).Name.Should().Be("Firefox");
        }

        [TestMethod]
        public void Name_Should_Return_Edge()
        {
            new BrowserInformation(EdgeUserAgent, Platform).Name.Should().Be("Edge");
        }

        [TestMethod]
        public void Name_Should_Return_Netscape()
        {
            new BrowserInformation("Mozilla/5.0 Unknown", Platform).Name.Should().Be("Netscape");
        }

        [TestMethod]
        public void Platform_Should_Return_Value()
        {
            HtmlPage.BrowserInformation.Platform.Should().Be(Platform);
        }

        [TestMethod]
        public void UserAgent_Should_Return_Value()
        {
            HtmlPage.BrowserInformation.UserAgent.Should().Be(ChromeUserAgent);
        }
    }
}
