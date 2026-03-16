
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
using System.Windows.Controls;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class ItemsControlAutomationPeerTest
    {
        [TestMethod]
        public void GetChildren_1()
        {
            var ic = new ItemsControlTest();
            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(ic).As<ItemsControlTestAutomationPeer>();
            var children = peer.GetChildren();

            Assert.IsNull(children);
        }

        [TestMethod]
        public void GetChildren_2()
        {
            var ic = new ItemsControlTest();
            ic.Items.Add(new TextBlock());
            ic.Items.Add(new TextBlock());
            ic.Items.Add(new TextBlock());
            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(ic).As<ItemsControlTestAutomationPeer>();
            var children = peer.GetChildren();

            Assert.IsNotNull(children);
            Assert.IsEmpty(children);
        }

        [TestMethod]
        public void GetChildren_3()
        {
            var ic = new ItemsControlTest();
            ic.Items.Add(new ItemTest());
            ic.Items.Add(new ItemTest());
            ic.Items.Add(new ItemTest());
            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(ic).As<ItemsControlTestAutomationPeer>();
            var children = peer.GetChildren();

            Assert.IsNotNull(children);
            Assert.HasCount(3, children);
            
            for (int i = 0; i < children.Count; i++)
            {
                var item = children[i].As<ItemTestAutomationPeer>();

                Assert.IsNotNull(item);
                Assert.AreSame(item.ItemsControlAutomationPeer, peer);
                Assert.AreSame(item.Item, ic.Items[i]);
            }
        }

        [TestMethod]
        public void GetChildren_4()
        {
            var ic = new ItemsControlTest();
            ic.Items.Add(new TextBlock());
            ic.Items.Add(new ItemTest());
            ic.Items.Add(new TextBlock());
            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(ic).As<ItemsControlTestAutomationPeer>();
            var children = peer.GetChildren();
            
            Assert.IsNotNull(children);
            Assert.HasCount(1, children);
            
            var child = children[0].As<ItemTestAutomationPeer>();

            Assert.IsNotNull(child);
            Assert.AreSame(child.ItemsControlAutomationPeer, peer);
            Assert.AreSame(child.Item, ic.Items[1]);
        }

        private class ItemsControlTest : ItemsControl
        {
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
