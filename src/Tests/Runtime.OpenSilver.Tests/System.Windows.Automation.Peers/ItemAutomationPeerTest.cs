
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver;
using System.Windows.Controls;
using OpenSilver.Internal.Helpers;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class ItemAutomationPeerTest
    {
        [TestMethod]
        public void Ctor_1_Should_Throw_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new ItemTestAutomationPeer(null));
        }

        [TestMethod]
        public void Ctor_2_Should_Throw_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new ItemTestAutomationPeer(null, null));
            Assert.Throws<InvalidOperationException>(() => new ItemTestAutomationPeer("Test", null));
            Assert.Throws<InvalidOperationException>(() => new ItemTestAutomationPeer(new DependencyObject(), null));
        }

        [TestMethod]
        public void GetAcceleratorKey_1()
        {
            var textblock = new TextBlock();
            AutomationProperties.SetAcceleratorKey(textblock, "key");

            Assert.AreEqual("key", new ItemTestAutomationPeer(textblock).GetAcceleratorKey());
        }

        [TestMethod]
        public void GetAcceleratorKey_2()
        {
            using (var wrapper = new FocusableControlWrapper<ItemsControl>(CreateItemsControl()))
            {
                var peer = FrameworkElementAutomationPeer.CreatePeerForElement(wrapper.Control);
                var itemPeer = peer.GetChildren().FirstOrDefault().As<ItemTestAutomationPeer>();

                Assert.IsNotNull(itemPeer);

                AutomationProperties.SetAcceleratorKey(wrapper.Control.ItemContainerGenerator.ContainerFromItem("Item"), "key");

                Assert.AreEqual("key", itemPeer.GetAcceleratorKey());
            }
        }

        [TestMethod]
        public void GetAccessKey_1()
        {
            var textblock = new TextBlock();
            AutomationProperties.SetAccessKey(textblock, "key");

            Assert.AreEqual("key", new ItemTestAutomationPeer(textblock).GetAccessKey());
        }

        [TestMethod]
        public void GetAccessKey_2()
        {
            using (var wrapper = new FocusableControlWrapper<ItemsControl>(CreateItemsControl()))
            {
                var peer = FrameworkElementAutomationPeer.CreatePeerForElement(wrapper.Control);
                var itemPeer = peer.GetChildren().FirstOrDefault().As<ItemTestAutomationPeer>();

                Assert.IsNotNull(itemPeer);

                AutomationProperties.SetAccessKey(wrapper.Control.ItemContainerGenerator.ContainerFromItem("Item"), "key");

                Assert.AreEqual("key", itemPeer.GetAccessKey());
            }
        }

        [TestMethod]
        public void GetParent()
        {
            var ic = new ItemsControlTest();
            var icPeer = FrameworkElementAutomationPeer.CreatePeerForElement(ic).As<ItemsControlTestAutomationPeer>();
            var peer = new ItemTestAutomationPeer("Item", icPeer);

            Assert.AreSame(peer.GetParent(), icPeer);
        }

        private static ItemsControl CreateItemsControl()
        {
            var ic = new ItemsControlTest();
            ic.Items.Add("Item");
            return ic;
        }

        private class ItemsControlTest : ItemsControl
        {
            protected override DependencyObject GetContainerForItemOverride()
                => new ItemTest();

            protected override bool IsItemItsOwnContainerOverride(object item)
                => item is ItemTest;

            protected override AutomationPeer OnCreateAutomationPeer()
                => new ItemsControlTestAutomationPeer(this);
        }

        private class ItemsControlTestAutomationPeer : ItemsControlAutomationPeer
        {
            public ItemsControlTestAutomationPeer(ItemsControl owner)
                : base(owner)
            {
            }
        }

        private class ItemTest : FrameworkElement
        {
            protected override AutomationPeer OnCreateAutomationPeer()
                => new ItemTestAutomationPeer(this);
        }

        private class ItemTestAutomationPeer : ItemAutomationPeer
        {
            public ItemTestAutomationPeer(UIElement item)
                : base(item)
            {
            }

            public ItemTestAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer)
                : base(item, itemsControlAutomationPeer)
            {
            }

            public new object Item => base.Item;
        }
    }
}
