
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

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers.Tests
#else
namespace Windows.UI.Xaml.Automation.Peers.Tests
#endif
{
    [TestClass]
    public class HyperlinkButtonAutomationPeerTest
    {
        [TestMethod]
        public void IInvokeProvider_Invoke_Should_Throw_ElementNotEnabledException()
        {
            var hyperlink = new HyperlinkButton
            {
                IsEnabled = false,
            };
            var peer = new HyperlinkButtonAutomationPeer(hyperlink);
            var provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

            provider.Should().NotBeNull();

            Assert.ThrowsException<ElementNotEnabledException>(() => provider.Invoke());
        }

        [TestMethod]
        public void IInvokeProvider_Invoke_Should_Raise_Click()
        {
            var buttonClicked = false;
            var hyperlink = new HyperlinkButton();
            hyperlink.Click += (o, e) =>
            {
                buttonClicked = true;
            };
            var peer = new HyperlinkButtonAutomationPeer(hyperlink);
            var provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            provider.Should().NotBeNull();
            provider.Invoke();
            buttonClicked.Should().BeTrue();
        }
    }
}
