
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
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class ListBoxItemAutomationPeerTest
    {
        [TestMethod]
        public void ISelectionItemProvider_Select_Should_Throw_ElementNotEnabledException()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            listboxItem.Should().NotBeNull();
            listboxItem.IsEnabled = false;
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.Select());
        }

        [TestMethod]
        public void ISelectionItemProvider_Select()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            listboxItem.Should().NotBeNull();
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            provider.Select();
            listboxItem.IsSelected.Should().BeTrue();
        }

        [TestMethod]
        public void ISelectionItemProvider_AddToSelection_Should_Throws_ElementNotEnabledException()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            listboxItem.Should().NotBeNull();
            listboxItem.IsEnabled = false;
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.AddToSelection());
        }

        [TestMethod]
        public void ISelectionItemProvider_AddToSelection()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            listboxItem.Should().NotBeNull();
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            listboxItem.IsSelected.Should().BeFalse();
            provider.AddToSelection();
            listboxItem.IsSelected.Should().BeTrue();
        }

        [TestMethod]
        public void ISelectionItemProvider_RemoveFromSelection_Should_Throw_ElementNotEnabledException()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            listboxItem.Should().NotBeNull();
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            listboxItem.IsEnabled = false;
            listboxItem.IsSelected = true;
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.RemoveFromSelection());
        }

        [TestMethod]
        public void ISelectionItemProvider_RemoveFromSelection()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            listboxItem.Should().NotBeNull();
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            ListBox listbox = peer.ItemsControlAutomationPeer.Owner.As<ListBox>();
            listbox.Should().NotBeNull();
            listbox.SelectedItem = listboxItem;
            provider.RemoveFromSelection();
            listbox.SelectedItem.Should().BeNull();
        }

        [TestMethod]
        public void ISelectionItemProvider_IsSelected()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            listboxItem.Should().NotBeNull();
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();
            ListBox listbox = peer.ItemsControlAutomationPeer.Owner.As<ListBox>();
            listbox.Should().NotBeNull();
            listbox.SelectedItem = null;
            provider.IsSelected.Should().BeFalse();
            listbox.SelectedItem = listboxItem;
            provider.IsSelected.Should().BeTrue();
        }

        [TestMethod]
        public void ISelectionItemProvider_SelectionContainer()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            listboxItem.Should().NotBeNull();
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            provider.Should().NotBeNull();

            IRawElementProviderSimple container = provider.SelectionContainer;
            container.Should().NotBeNull();
            container.Peer.Should().BeSameAs(peer.ItemsControlAutomationPeer);
        }

        private static ListBoxItemAutomationPeer CreatePeer()
        {
            var listbox = new ListBox();
            var listboxItem = new ListBoxItem();
            listbox.Items.Add(listboxItem);

            var peer = new ListBoxItemAutomationPeer(
                listboxItem,
                (ListBoxAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(listbox));

            return peer;
        }
    }
}
