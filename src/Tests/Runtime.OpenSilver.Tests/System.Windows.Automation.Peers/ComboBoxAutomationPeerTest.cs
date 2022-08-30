
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
    public class ComboBoxAutomationPeerTest
    {
        [TestMethod]
        public void IExpandCollapseProvider_Expand_Should_Throw_ElementNotEnabledException()
        {
            var comboBox = new ComboBox();
            var peer = new ComboBoxAutomationPeer(comboBox);
            comboBox.IsEnabled = false;
            peer.IsEnabled().Should().BeFalse();
            var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.Expand());
        }

        [TestMethod]
        public void IExpandCollapseProvider_Expand_Should_Set_IsDropDownOpen_True()
        {
            var comboBox = new ComboBox();
            var peer = new ComboBoxAutomationPeer(comboBox);
            var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;

            provider.Should().NotBeNull();
            comboBox.IsDropDownOpen.Should().BeFalse();
            provider.Expand();
            comboBox.IsDropDownOpen.Should().BeTrue();
        }

        [TestMethod]
        public void IExpandCollapseProvider_Collapse_Should_Throw_ElementNotEnabledException()
        {
            var comboBox = new ComboBox();
            var peer = new ComboBoxAutomationPeer(comboBox);
            comboBox.IsEnabled = false;
            peer.IsEnabled().Should().BeFalse();
            var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.Collapse());
        }

        [TestMethod]
        public void IExpandCollapseProvider_Collapse_Should_Set_IsDropDownOpen_False()
        {
            var comboBox = new ComboBox
            {
                IsDropDownOpen = true,
            };
            var peer = new ComboBoxAutomationPeer(comboBox);
            var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
            provider.Should().NotBeNull();
            comboBox.IsDropDownOpen.Should().BeTrue();
            provider.Collapse();
            comboBox.IsDropDownOpen.Should().BeFalse();
        }

        [TestMethod]
        public void IExpandCollapseProvider_ExpandCollapseState_Should_Map_To_IsDropDownOpen()
        {
            var comboBox = new ComboBox();
            var peer = new ComboBoxAutomationPeer(comboBox);
            var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
            provider.Should().NotBeNull();
            provider.ExpandCollapseState.Should().Be(ExpandCollapseState.Collapsed);
            comboBox.IsDropDownOpen = true;
            provider.ExpandCollapseState.Should().Be(ExpandCollapseState.Expanded);
        }

        [TestMethod]
        public void ISelectionProvider_CanSelectMultiple_Should_Return_False()
        {
            var peer = new ComboBoxAutomationPeer(new ComboBox());
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
            provider.Should().NotBeNull();
            provider.CanSelectMultiple.Should().BeFalse();
        }

        [TestMethod]
        public void ISelectionProvider_IsSelectionRequired_Should_Return_False()
        {
            var peer = new ComboBoxAutomationPeer(new ComboBox());
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
            provider.Should().NotBeNull();
            provider.IsSelectionRequired.Should().BeFalse();
        }

        [TestMethod]
        public void ISelectionProvider_GetSelection()
        {
            var comboBox = new ComboBox();
            comboBox.Items.Add("Item 1");
            comboBox.Items.Add("Item 2");
            comboBox.Items.Add("Item 3");
            comboBox.SelectedIndex = 2;

            var peer = new ComboBoxAutomationPeer(comboBox);
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;
            provider.Should().NotBeNull();

            using (var wrapper = new FocusableControlWrapper<ComboBox>(comboBox))
            {
                IRawElementProviderSimple[] selection = provider.GetSelection();
                selection.Length.Should().Be(1);
                selection[0].Peer.Should().BeOfType<ListBoxItemAutomationPeer>();
                selection[0].Peer.As<ListBoxItemAutomationPeer>().ItemsControlAutomationPeer.Should().BeSameAs(peer);
            }
        }
    }
}
