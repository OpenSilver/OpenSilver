
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
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers.Tests
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

            Assert.IsNotNull(provider);

            radio.IsChecked = true;
            Assert.IsTrue(provider.IsSelected);

            radio.IsChecked = false;
            Assert.IsFalse(provider.IsSelected);

            radio.IsChecked = null;
            Assert.IsFalse(provider.IsSelected);
        }

        [TestMethod]
        public void ISelectionItemProvider_SelectionContainer()
        {
            var peer = new RadioButtonAutomationPeer(new RadioButton());
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;

            Assert.IsNotNull(provider);
            Assert.IsNull(provider.SelectionContainer);
        }

        [TestMethod]
        public void ISelectionItemProvider_RemoveFromSelection_Should_Throw_InvalidOperationException()
        {
            var peer = new RadioButtonAutomationPeer(new RadioButton { IsChecked = true });
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;

            Assert.IsNotNull(provider);
            Assert.Throws<InvalidOperationException>(() => provider.RemoveFromSelection());
        }

        [TestMethod]
        public void ISelectionItemProvider_AddToSelection_Should_Throw_InvalidOperationException()
        {
            var peer = new RadioButtonAutomationPeer(new RadioButton { IsChecked = false });
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;

            Assert.IsNotNull(provider);
            Assert.Throws<InvalidOperationException>(() => provider.AddToSelection());
        }

        [TestMethod]
        public void ISelectionItemProvider_Select_Should_Throw_ElementNotEnabledException()
        {
            var peer = new RadioButtonAutomationPeer(new RadioButton { IsEnabled = false });
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;

            Assert.IsNotNull(provider);
            Assert.Throws<ElementNotEnabledException>(() => provider.Select());
        }

        [TestMethod]
        public void ISelectionItemProvider_Select()
        {
            var radio = new RadioButton { IsChecked = false };
            var peer = new RadioButtonAutomationPeer(radio);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;

            Assert.IsNotNull(provider)
                ;
            provider.Select();

            Assert.IsTrue(radio.IsChecked);
        }
    }
}
