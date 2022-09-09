
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
using OpenSilver;

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
    public class ListBoxAutomationPeerTest
    {
        [TestMethod]
        public void ISelectionProvider_CanSelectMultiple()
        {
            var listbox = new ListBox();
            var peer = new ListBoxAutomationPeer(listbox);
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
            provider.Should().NotBeNull();

            listbox.SelectionMode = SelectionMode.Single;
            provider.CanSelectMultiple.Should().BeFalse();

            listbox.SelectionMode = SelectionMode.Multiple;
            provider.CanSelectMultiple.Should().BeTrue();

            listbox.SelectionMode = SelectionMode.Extended;
            provider.CanSelectMultiple.Should().BeTrue();
        }

        [TestMethod]
        public void ISelectionProvider_IsSelectionRequired()
        {
            var peer = new ListBoxAutomationPeer(new ListBox());
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
            provider.Should().NotBeNull();
            provider.IsSelectionRequired.Should().BeFalse();
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
            provider.Should().NotBeNull();

            using (var wrapper = new FocusableControlWrapper<ListBox>(listbox))
            {
                listbox.SelectedItem = "Item 2";
                var selection = provider.GetSelection();
                selection.Length.Should().Be(1);
                selection[0].Peer.Should().BeOfType<ListBoxItemAutomationPeer>();
                selection[0].Peer.As<ListBoxItemAutomationPeer>().ItemsControlAutomationPeer.Should().BeSameAs(peer);

                listbox.SelectionMode = SelectionMode.Multiple;
                listbox.SelectedItems.Add("Item 4");
                selection = provider.GetSelection();
                selection.Length.Should().Be(2);
                selection[0].Peer.Should().BeOfType<ListBoxItemAutomationPeer>();
                selection[0].Peer.As<ListBoxItemAutomationPeer>().ItemsControlAutomationPeer.Should().BeSameAs(peer);
                selection[1].Peer.Should().BeOfType<ListBoxItemAutomationPeer>();
                selection[1].Peer.As<ListBoxItemAutomationPeer>().ItemsControlAutomationPeer.Should().BeSameAs(peer);
            }
        }
    }
}
