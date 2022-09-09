
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
using OpenSilver.Internal;
using OpenSilver;

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
    public class ToggleButtonAutomationPeerTest
    {
        [TestMethod]
        public void IToggleProvider_Toggle_Should_Throw_ElementNotEnabledException()
        {
            var toggle = new ToggleButton { IsEnabled = false };
            var peer = new ToggleButtonAutomationPeer(toggle);
            var provider = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.Toggle());
        }

        [TestMethod]
        public void IToggleProvider_Toggle()
        {
            var toggle = new ToggleButton { IsThreeState = true, IsChecked = true };
            var peer = new ToggleButtonAutomationPeer(toggle);
            var provider = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
            provider.Should().NotBeNull();
            provider.Toggle();
            toggle.IsChecked.Should().BeNull();
            provider.Toggle();
            toggle.IsChecked.Should().BeFalse();
            provider.Toggle();
            toggle.IsChecked.Should().BeTrue();
        }

        [TestMethod]
        public void IToggleProvider_ToggleState()
        {
            var toggle = new ToggleButton { IsThreeState = true };
            var peer = new ToggleButtonAutomationPeer(toggle);
            var provider = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
            provider.Should().NotBeNull();

            toggle.IsChecked = true;
            provider.ToggleState.Should().Be(ToggleState.On);

            toggle.IsChecked = false;
            provider.ToggleState.Should().Be(ToggleState.Off);

            toggle.IsChecked = null;
            provider.ToggleState.Should().Be(ToggleState.Indeterminate);
        }
    }
}
