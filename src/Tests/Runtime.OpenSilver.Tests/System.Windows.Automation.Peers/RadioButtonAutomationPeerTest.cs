
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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

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
    public class RadioButtonAutomationPeerTest
    {
        [TestMethod]
        public void ISelectionItemProvider_IsSelected()
        {
            var radio = new RadioButton { IsThreeState = true };
            var peer = new RadioButtonAutomationPeer(radio);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();

            radio.IsChecked = true;
            provider.IsSelected.Should().BeTrue();
            radio.IsChecked = false;
            provider.IsSelected.Should().BeFalse();
            radio.IsChecked = null;
            provider.IsSelected.Should().BeFalse();
        }

        [TestMethod]
        public void ISelectionItemProvider_SelectionContainer()
        {
            var peer = new RadioButtonAutomationPeer(new RadioButton());
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            provider.SelectionContainer.Should().BeNull();
        }

        [TestMethod]
        public void ISelectionItemProvider_RemoveFromSelection_Should_Throw_InvalidOperationException()
        {
            var peer = new RadioButtonAutomationPeer(new RadioButton { IsChecked = true });
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<InvalidOperationException>(() => provider.RemoveFromSelection());
        }

        [TestMethod]
        public void ISelectionItemProvider_AddToSelection_Should_Throw_InvalidOperationException()
        {
            var peer = new RadioButtonAutomationPeer(new RadioButton { IsChecked = false });
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<InvalidOperationException>(() => provider.AddToSelection());
        }

        [TestMethod]
        public void ISelectionItemProvider_Select_Should_Throw_ElementNotEnabledException()
        {
            var peer = new RadioButtonAutomationPeer(new RadioButton { IsEnabled = false });
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.Select());
        }

        [TestMethod]
        public void ISelectionItemProvider_Select()
        {
            var radio = new RadioButton { IsChecked = false };
            var peer = new RadioButtonAutomationPeer(radio);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            provider.Select();
            radio.IsChecked.Should().BeTrue();
        }
    }
}
