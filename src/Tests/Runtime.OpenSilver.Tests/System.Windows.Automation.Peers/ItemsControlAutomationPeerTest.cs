
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
using System.Linq;

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
    public class ItemsControlAutomationPeerTest
    {
        [TestMethod]
        public void GetChildren_1()
        {
            var ic = new ItemsControlTest();
            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(ic).As<ItemsControlTestAutomationPeer>();
            var children = peer.GetChildren();
            children.Should().BeNull();
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
            children.Should().NotBeNull();
            children.Count.Should().Be(0);
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
            children.Should().NotBeNull();
            children.Count.Should().Be(3);
            
            for (int i = 0; i < children.Count; i++)
            {
                var item = children[i].As<ItemTestAutomationPeer>();
                item.Should().NotBeNull();
                item.ItemsControlAutomationPeer.Should().BeSameAs(peer);
                item.Item.Should().BeSameAs(ic.Items[i]);
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
            
            children.Should().NotBeNull();
            children.Count.Should().Be(1);
            
            var child = children[0].As<ItemTestAutomationPeer>();
            child.Should().NotBeNull();
            child.ItemsControlAutomationPeer.Should().BeSameAs(peer);
            child.Item.Should().BeSameAs(ic.Items[1]);
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
