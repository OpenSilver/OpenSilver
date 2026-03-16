
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
using OpenSilver;
using OpenSilver.Internal.Helpers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class ListBoxAutomationPeerTest
    {
        [TestMethod]
        public void ISelectionProvider_CanSelectMultiple()
        {
            var listbox = new ListBox();
            var peer = new ListBoxAutomationPeer(listbox);
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;

            Assert.IsNotNull(provider);

            listbox.SelectionMode = SelectionMode.Single;
            Assert.IsFalse(provider.CanSelectMultiple);

            listbox.SelectionMode = SelectionMode.Multiple;
            Assert.IsTrue(provider.CanSelectMultiple);

            listbox.SelectionMode = SelectionMode.Extended;
            Assert.IsTrue(provider.CanSelectMultiple);
        }

        [TestMethod]
        public void ISelectionProvider_IsSelectionRequired()
        {
            var peer = new ListBoxAutomationPeer(new ListBox());
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;

            Assert.IsNotNull(provider);
            Assert.IsFalse(provider.IsSelectionRequired);
        }

        [TestMethod]
        public void ISelectionProvider_GetSelection()
        {
            var listbox = new ListBox();
            listbox.Items.Add("Item 1");
            listbox.Items.Add("Item 2");
            listbox.Items.Add("Item 3");
            listbox.Items.Add("Item 4");
            var peer = new ListBoxAutomationPeer(listbox);
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;

            Assert.IsNotNull(provider);

            using (var wrapper = new FocusableControlWrapper<ListBox>(listbox))
            {
                listbox.SelectedItem = "Item 2";
                var selection = provider.GetSelection();

                Assert.HasCount(1, selection);
                Assert.IsInstanceOfType<ListBoxItemAutomationPeer>(selection[0].Peer);
                Assert.AreSame(selection[0].Peer.As<ListBoxItemAutomationPeer>().ItemsControlAutomationPeer, peer);

                listbox.SelectionMode = SelectionMode.Multiple;
                listbox.SelectedItems.Add("Item 4");
                selection = provider.GetSelection();

                Assert.HasCount(2, selection);
                Assert.IsInstanceOfType<ListBoxItemAutomationPeer>(selection[0].Peer);
                Assert.AreSame(selection[0].Peer.As<ListBoxItemAutomationPeer>().ItemsControlAutomationPeer, peer);
                Assert.IsInstanceOfType<ListBoxItemAutomationPeer>(selection[1].Peer);
                Assert.AreSame(selection[1].Peer.As<ListBoxItemAutomationPeer>().ItemsControlAutomationPeer, peer);
            }
        }
    }
}
