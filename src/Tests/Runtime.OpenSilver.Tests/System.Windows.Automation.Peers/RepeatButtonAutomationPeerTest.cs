﻿
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers.Tests
#else
namespace Windows.UI.Xaml.Automation.Peers.Tests
#endif
{
    [TestClass]
    public class RepeatButtonAutomationPeerTest
    {
        [TestMethod]
        public void IInvokeProvider_Invoke_Should_Throw_ElementNotEnabledException()
        {
            var peer = new RepeatButtonAutomationPeer(new RepeatButton { IsEnabled = false });
            var provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.Invoke());
        }

        [TestMethod]
        public void IInvokeProvider_Invoke()
        {
            var button = new RepeatButton();
            var peer = new RepeatButtonAutomationPeer(button);
            var provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

            bool isButtonClicked = false;
            button.Click += (o, e) => isButtonClicked = true;

            provider.Invoke();

            isButtonClicked.Should().BeTrue();
        }
    }
}
