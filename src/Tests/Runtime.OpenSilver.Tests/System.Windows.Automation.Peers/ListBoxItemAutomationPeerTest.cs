
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
using OpenSilver.Internal.Helpers;
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
            Assert.IsNotNull(listboxItem);
            listboxItem.IsEnabled = false;
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            Assert.IsNotNull(provider);
            Assert.Throws<ElementNotEnabledException>(() => provider.Select());
        }

        [TestMethod]
        public void ISelectionItemProvider_Select()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            Assert.IsNotNull(listboxItem);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            Assert.IsNotNull(provider);
            provider.Select();
            Assert.IsTrue(listboxItem.IsSelected);
        }

        [TestMethod]
        public void ISelectionItemProvider_AddToSelection_Should_Throws_ElementNotEnabledException()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            Assert.IsNotNull(listboxItem);
            listboxItem.IsEnabled = false;
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            Assert.IsNotNull(provider);
            Assert.Throws<ElementNotEnabledException>(() => provider.AddToSelection());
        }

        [TestMethod]
        public void ISelectionItemProvider_AddToSelection()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            Assert.IsNotNull(listboxItem);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            Assert.IsNotNull(provider);
            Assert.IsFalse(listboxItem.IsSelected);
            provider.AddToSelection();
            Assert.IsTrue(listboxItem.IsSelected);
        }

        [TestMethod]
        public void ISelectionItemProvider_RemoveFromSelection_Should_Throw_ElementNotEnabledException()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            Assert.IsNotNull(listboxItem);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            Assert.IsNotNull(provider);
            listboxItem.IsEnabled = false;
            listboxItem.IsSelected = true;
            Assert.Throws<ElementNotEnabledException>(() => provider.RemoveFromSelection());
        }

        [TestMethod]
        public void ISelectionItemProvider_RemoveFromSelection()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            Assert.IsNotNull(listboxItem);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            Assert.IsNotNull(provider);
            ListBox listbox = peer.ItemsControlAutomationPeer.Owner.As<ListBox>();
            Assert.IsNotNull(listbox);
            listbox.SelectedItem = listboxItem;
            provider.RemoveFromSelection();
            Assert.IsNull(listbox.SelectedItem);
        }

        [TestMethod]
        public void ISelectionItemProvider_IsSelected()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            Assert.IsNotNull(listboxItem);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            Assert.IsNotNull(provider);
            ListBox listbox = peer.ItemsControlAutomationPeer.Owner.As<ListBox>();
            Assert.IsNotNull(listbox);
            listbox.SelectedItem = null;
            Assert.IsFalse(provider.IsSelected);
            listbox.SelectedItem = listboxItem;
            Assert.IsTrue(provider.IsSelected);
        }

        [TestMethod]
        public void ISelectionItemProvider_SelectionContainer()
        {
            ListBoxItemAutomationPeer peer = CreatePeer();
            ListBoxItem listboxItem = peer.Owner.As<ListBoxItem>();
            Assert.IsNotNull(listboxItem);
            var provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            Assert.IsNotNull(provider);

            IRawElementProviderSimple container = provider.SelectionContainer;
            Assert.IsNotNull(container);
            Assert.AreSame(container.Peer, peer.ItemsControlAutomationPeer);
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
