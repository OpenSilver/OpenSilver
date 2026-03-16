
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
    public class ComboBoxAutomationPeerTest
    {
        [TestMethod]
        public void IExpandCollapseProvider_Expand_Should_Throw_ElementNotEnabledException()
        {
            var comboBox = new ComboBox();
            var peer = new ComboBoxAutomationPeer(comboBox);
            comboBox.IsEnabled = false;

            Assert.IsFalse(peer.IsEnabled());

            var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;

            Assert.IsNotNull(provider);
            Assert.Throws<ElementNotEnabledException>(() => provider.Expand());
        }

        [Ignore]
        [TestMethod]
        public void IExpandCollapseProvider_Expand_Should_Set_IsDropDownOpen_True()
        {
            using (var wrapper = new FocusableControlWrapper<ComboBox>(new ComboBox()))
            {
                ComboBox comboBox = wrapper.Control;
                var peer = new ComboBoxAutomationPeer(comboBox);
                var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;

                Assert.IsNotNull(provider);
                Assert.IsFalse(comboBox.IsDropDownOpen);

                provider.Expand();

                Assert.IsTrue(comboBox.IsDropDownOpen);
            }
        }

        [TestMethod]
        public void IExpandCollapseProvider_Collapse_Should_Throw_ElementNotEnabledException()
        {
            var comboBox = new ComboBox();
            var peer = new ComboBoxAutomationPeer(comboBox);
            comboBox.IsEnabled = false;

            Assert.IsFalse(peer.IsEnabled());

            var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;

            Assert.IsNotNull(provider);
            Assert.Throws<ElementNotEnabledException>(() => provider.Collapse());
        }

        [Ignore]
        [TestMethod]
        public void IExpandCollapseProvider_Collapse_Should_Set_IsDropDownOpen_False()
        {
            using (var wrapper = new FocusableControlWrapper<ComboBox>(new ComboBox()))
            {
                ComboBox comboBox = wrapper.Control;
                comboBox.IsDropDownOpen = true;
                var peer = new ComboBoxAutomationPeer(comboBox);
                var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;

                Assert.IsNotNull(provider);
                Assert.IsTrue(comboBox.IsDropDownOpen);

                provider.Collapse();

                Assert.IsFalse(comboBox.IsDropDownOpen);
            };

        }

        [Ignore]
        [TestMethod]
        public void IExpandCollapseProvider_ExpandCollapseState_Should_Map_To_IsDropDownOpen()
        {
            using (var wrapper = new FocusableControlWrapper<ComboBox>(new ComboBox()))
            {
                ComboBox comboBox = wrapper.Control;
                var peer = new ComboBoxAutomationPeer(comboBox);
                var provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;

                Assert.IsNotNull(provider);
                Assert.AreEqual(ExpandCollapseState.Collapsed, provider.ExpandCollapseState);

                comboBox.IsDropDownOpen = true;

                Assert.AreEqual(ExpandCollapseState.Expanded, provider.ExpandCollapseState);
            }
        }

        [TestMethod]
        public void ISelectionProvider_CanSelectMultiple_Should_Return_False()
        {
            var peer = new ComboBoxAutomationPeer(new ComboBox());
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;

            Assert.IsNotNull(provider);
            Assert.IsFalse(provider.CanSelectMultiple);
        }

        [TestMethod]
        public void ISelectionProvider_IsSelectionRequired_Should_Return_False()
        {
            var peer = new ComboBoxAutomationPeer(new ComboBox());
            var provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider;

            Assert.IsNotNull(provider);
            Assert.IsFalse(provider.IsSelectionRequired);
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

            Assert.IsNotNull(provider);

            using (var wrapper = new FocusableControlWrapper<ComboBox>(comboBox))
            {
                IRawElementProviderSimple[] selection = provider.GetSelection();

                Assert.HasCount(1, selection);
                Assert.IsInstanceOfType<ListBoxItemAutomationPeer>(selection[0].Peer);
                Assert.AreSame(selection[0].Peer.As<ListBoxItemAutomationPeer>().ItemsControlAutomationPeer, peer);
            }
        }
    }
}
