
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

using System;
using System.Linq;
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
    public class ItemAutomationPeerTest
    {
        [TestMethod]
        public void Ctor_1_Should_Throw_InvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new ItemTestAutomationPeer(null));
        }

        [TestMethod]
        public void Ctor_2_Should_Throw_InvalidOperationException()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new ItemTestAutomationPeer(null, null));
            Assert.ThrowsException<InvalidOperationException>(() => new ItemTestAutomationPeer("Test", null));
            Assert.ThrowsException<InvalidOperationException>(() => new ItemTestAutomationPeer(new DependencyObject(), null));
        }

        [TestMethod]
        public void GetAcceleratorKey_1()
        {
            var textblock = new TextBlock();
            AutomationProperties.SetAcceleratorKey(textblock, "key");
            new ItemTestAutomationPeer(textblock).GetAcceleratorKey().Should().Be("key");
        }

        [TestMethod]
        public void GetAcceleratorKey_2()
        {
            using (var wrapper = new FocusableControlWrapper<ItemsControl>(CreateItemsControl()))
            {
                var peer = FrameworkElementAutomationPeer.CreatePeerForElement(wrapper.Control);
                var itemPeer = peer.GetChildren().FirstOrDefault().As<ItemTestAutomationPeer>();
                itemPeer.Should().NotBeNull();
                AutomationProperties.SetAcceleratorKey(wrapper.Control.ItemContainerGenerator.ContainerFromItem("Item"), "key");
                itemPeer.GetAcceleratorKey().Should().Be("key");
            }
        }

        [TestMethod]
        public void GetAccessKey_1()
        {
            var textblock = new TextBlock();
            AutomationProperties.SetAccessKey(textblock, "key");
            new ItemTestAutomationPeer(textblock).GetAccessKey().Should().Be("key");
        }

        [TestMethod]
        public void GetAccessKey_2()
        {
            using (var wrapper = new FocusableControlWrapper<ItemsControl>(CreateItemsControl()))
            {
                var peer = FrameworkElementAutomationPeer.CreatePeerForElement(wrapper.Control);
                var itemPeer = peer.GetChildren().FirstOrDefault().As<ItemTestAutomationPeer>();
                itemPeer.Should().NotBeNull();
                AutomationProperties.SetAccessKey(wrapper.Control.ItemContainerGenerator.ContainerFromItem("Item"), "key");
                itemPeer.GetAccessKey().Should().Be("key");
            }
        }

        [TestMethod]
        public void GetParent()
        {
            var ic = new ItemsControlTest();
            var icPeer = FrameworkElementAutomationPeer.CreatePeerForElement(ic).As<ItemsControlTestAutomationPeer>();
            var peer = new ItemTestAutomationPeer("Item", icPeer);

            peer.GetParent().Should().BeSameAs(icPeer);
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
