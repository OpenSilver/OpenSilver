
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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers.Tests
#else
namespace Windows.UI.Xaml.Automation.Peers.Tests
#endif
{
    [TestClass]
    public class FrameworkElementAutomationPeerTest
    {
        [TestMethod]
        public void CreatePeerForElement_Should_Return_Peer()
        {
            FrameworkElementAutomationPeer.CreatePeerForElement(new FE1())
                .Should()
                .BeOfType<FE1AutomationPeer>();
        }

        [TestMethod]
        public void FromElement_Should_Return_Null()
        {
            FrameworkElementAutomationPeer.FromElement(new FE1())
                .Should().BeNull();
        }

        [TestMethod]
        public void FromElement_Should_Return_Peer()
        {
            var fe = new FE1();
            _ = FrameworkElementAutomationPeer.CreatePeerForElement(fe);

            FrameworkElementAutomationPeer.FromElement(fe)
                .Should()
                .BeOfType<FE1AutomationPeer>();
        }

        [TestMethod]
        public void Peer_Should_Be_Cached()
        {
            var fe = new FE1();

            var peer1 = FrameworkElementAutomationPeer.CreatePeerForElement(fe);
            var peer2 = FrameworkElementAutomationPeer.CreatePeerForElement(fe);
            var peer3 = FrameworkElementAutomationPeer.FromElement(fe);

            peer1.Should().BeSameAs(peer2);
            peer2.Should().BeSameAs(peer3);
        }

        [TestMethod]
        public void FromElement_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => FrameworkElementAutomationPeer.FromElement(null));
        }

        [TestMethod]
        public void CreatePeerForElement_Should_Throw_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => FrameworkElementAutomationPeer.CreatePeerForElement(null));
        }

        [TestMethod]
        public void Owner_Should_Not_Be_Null()
        {
            var fe = new FE1();
            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .As<FrameworkElementAutomationPeer>()
                .Owner
                .Should()
                .BeSameAs(fe);
        }

        [TestMethod]
        public void GetAcceleratorKey_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "AcceleratorKey";
            AutomationProperties.SetAcceleratorKey(fe, value);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .GetAcceleratorKey()
                .Should()
                .Be(value);
        }

        [TestMethod]
        public void GetAccessKey_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "AccessKey";
            AutomationProperties.SetAccessKey(fe, value);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .GetAccessKey()
                .Should()
                .Be(value);
        }

        [TestMethod]
        public void GetAutomationId_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "AutomationId";
            AutomationProperties.SetAutomationId(fe, value);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .GetAutomationId()
                .Should()
                .Be(value);
        }

        [TestMethod]
        public void GetHelpText_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "HelpText";
            AutomationProperties.SetHelpText(fe, value);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .GetHelpText()
                .Should()
                .Be(value);
        }

        [TestMethod]
        public void GetItemStatus_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "ItemStatus";
            AutomationProperties.SetItemStatus(fe, value);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .GetItemStatus()
                .Should()
                .Be(value);
        }

        [TestMethod]
        public void GetItemType_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "ItemType";
            AutomationProperties.SetItemType(fe, value);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .GetItemType()
                .Should()
                .Be(value);
        }

        [TestMethod]
        public void GetLabeledBy_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            var c = new C1();
            AutomationProperties.SetLabeledBy(fe, c);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .GetLabeledBy()
                .Should()
                .BeSameAs(FrameworkElementAutomationPeer.FromElement(c));
        }

        [TestMethod]
        public void GetName_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "Name";
            AutomationProperties.SetName(fe, value);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .GetName()
                .Should()
                .Be(value);
        }

        [TestMethod]
        public void IsRequiredForForm_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            bool value = true;
            AutomationProperties.SetIsRequiredForForm(fe, value);

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .IsRequiredForForm()
                .Should()
                .Be(value);
        }

        [TestMethod]
        public void GetAutomationControlType_Should_Return_Custom()
        {
            FrameworkElementAutomationPeer.CreatePeerForElement(new FE1())
                .GetAutomationControlType()
                .Should()
                .Be(AutomationControlType.Custom);
        }

        [TestMethod]
        public void GetClassName_Should_Return_String_Empty()
        {
            FrameworkElementAutomationPeer.CreatePeerForElement(new FE1())
                .GetClassName()
                .Should()
                .Be(string.Empty);
        }

        [TestMethod]
        public void IsContentElement_Should_Return_True()
        {
            FrameworkElementAutomationPeer.CreatePeerForElement(new FE1())
                .IsContentElement()
                .Should()
                .Be(true);
        }

        [TestMethod]
        public void IsControlElement_Should_Return_True()
        {
            FrameworkElementAutomationPeer.CreatePeerForElement(new FE1())
                .IsControlElement()
                .Should()
                .Be(true);
        }

        [TestMethod]
        public void IsPassword_Should_Return_True()
        {
            FrameworkElementAutomationPeer.CreatePeerForElement(new FE1())
                .IsPassword()
                .Should()
                .Be(false);
        }

        [TestMethod]
        public void IsEnabled_Should_Return_FrameworkElement_IsEnabled()
        {
            var fe = new FE1();
            fe.IsEnabled = true;

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .IsEnabled()
                .Should()
                .Be(fe.IsEnabled);

            fe.IsEnabled = false;

            FrameworkElementAutomationPeer.CreatePeerForElement(fe)
                .IsEnabled()
                .Should()
                .Be(fe.IsEnabled);
        }

        [TestMethod]
        public void GetLocalizedControl_Should_Return_Lowered_AutomationControlType()
        {
            var peer = new FE1AutomationPeer(new FE1());

            foreach (AutomationControlType controlType in Enum.GetValues(typeof(AutomationControlType)))
            {
                peer.AutomationControlType = controlType;
                peer.GetLocalizedControlType()
                    .Should()
                    .Be(peer.GetAutomationControlType().ToString().ToLower());
            }
        }

        [TestMethod]
        public void GetLocalizedControl_Should_Return_String_Empty()
        {
            new FE1AutomationPeer(new FE1()) { AutomationControlType = (AutomationControlType)50 }
                .GetLocalizedControlType()
                .Should()
                .Be(string.Empty);
        }

        [TestMethod]
        public void GetOrientation_Should_Return_None()
        {
            new FE1AutomationPeer(new FE1()).GetOrientation()
                .Should()
                .Be(AutomationOrientation.None);
        }

        [TestMethod]
        public void GetParent_Should_Return_Null()
        {
            new FE1AutomationPeer(new FE1()).GetParent()
                .Should()
                .BeNull();
        }

        [TestMethod]
        public void GetParent_Should_Use_Visual_Parent_1()
        {
            var fe = new FE1();
            var c = new C1();
            fe.Content = c;

            FrameworkElementAutomationPeer.CreatePeerForElement(c)
                .GetParent()
                .Should()
                .BeSameAs(FrameworkElementAutomationPeer.FromElement(fe));
        }

        [TestMethod]
        public void GetParent_Should_Use_Visual_Parent_2()
        {
            /*
            <FE1> <!-- Has an AutomationPeer -->
              <StackPanel> <!-- No AutomationPeer -->
                <C1 /> <!-- Has an AutomationPeer -->
              </StackPanel>
            </FE1>
            */
            var fe = new FE1();
            var panel = new StackPanel();
            var c = new C1();
            panel.Children.Add(c);
            fe.Content = panel;

            FrameworkElementAutomationPeer.CreatePeerForElement(c)
                .GetParent()
                .Should()
                .BeSameAs(FrameworkElementAutomationPeer.FromElement(fe));
        }

        [TestMethod]
        public void IsOffScreen_Should_Return_Not_IsVisible()
        {
            var fe = new FE1();
            var peer = new FE1AutomationPeer(fe);

            fe.IsVisible.Should().Be(false);
            peer.IsOffscreen().Should().Be(!fe.IsVisible);

            using (var wrapper = new FocusableControlWrapper<FE1>(fe))
            {
                fe.IsVisible.Should().Be(true);
                peer.IsOffscreen().Should().Be(!fe.IsVisible);
            }
        }

        [TestMethod]
        public void SetFocus_Should_Set_Focus_For_Control()
        {
            using (var wrapper = new FocusableControlWrapper<C1>(new C1()))
            {
                var peer = new C1AutomationPeer(wrapper.Control);

                peer.SetFocus();

                FocusManager.GetFocusedElement().Should().BeSameAs(wrapper.Control);

                FocusManager.SetFocusedElement(Window.Current, null);

                wrapper.Control.IsEnabled = false;

                peer.SetFocus();

                FocusManager.GetFocusedElement().Should().BeNull();
            }
        }

        [TestMethod]
        public void SetFocus_Should_Not_Set_Focus_For_FrameworkElement()
        {
            using (var wrapper = new FocusableControlWrapper<FE1>(new FE1()))
            {
                var peer = new FE1AutomationPeer(wrapper.Control);
                peer.SetFocus();

                FocusManager.GetFocusedElement().Should().BeNull();

                wrapper.Control.IsEnabled = false;
                peer.SetFocus();

                FocusManager.GetFocusedElement().Should().BeNull();
            }
        }

        [TestMethod]
        public void GetChildren_Should_Return_Null()
        {
            new FE1AutomationPeer(new FE1()).GetChildren().Should().BeNull();
        }

        [TestMethod]
        public void GetChildren_Should_Use_VisualTree()
        {
            /*
            <FE1>
              <StackPanel>
                <C1 />
                <C1 />
              </StackPanel>
            </FE1>
            */
            var fe = new FE1();
            var panel = new StackPanel();
            var c1 = new C1();
            panel.Children.Add(c1);
            var c2 = new C1();
            panel.Children.Add(c2);
            fe.Content = panel;

            var peer = new FE1AutomationPeer(fe);
            var children = peer.GetChildren();

            children.Count.Should().Be(2);
            children[0].Should().BeOfType<C1AutomationPeer>();
            children[0].As<C1AutomationPeer>().Owner.Should().BeSameAs(c1);
            children[1].Should().BeOfType<C1AutomationPeer>();
            children[1].As<C1AutomationPeer>().Owner.Should().BeSameAs(c2);
        }

        [TestMethod]
        public void GetChildren_Should_Stop_After_First_Peer()
        {
            /*
            <FE1>
              <StackPanel>
                <C1 />
                <FE1>
                  <C1 />
                </FE1>
              </StackPanel>
            </FE1>
            */
            var fe1 = new FE1();
            var panel = new StackPanel();
            var c1 = new C1();
            panel.Children.Add(c1);
            var fe2 = new FE1();
            var c2 = new C1();
            fe2.Content = c2;
            panel.Children.Add(fe2);
            fe1.Content = panel;

            var peer = new FE1AutomationPeer(fe1);
            var children = peer.GetChildren();

            children.Count.Should().Be(2);
            children[0].Should().BeOfType<C1AutomationPeer>();
            children[0].As<C1AutomationPeer>().Owner.Should().BeSameAs(c1);
            children[1].Should().BeOfType<FE1AutomationPeer>();
            children[1].As<FE1AutomationPeer>().Owner.Should().BeSameAs(fe2);
        }

        private class FE1 : FrameworkElement
        {
            private UIElement _content;

            public UIElement Content
            {
                get => _content;
                set
                {
                    RemoveVisualChild(_content);
                    _content = value;
                    AddVisualChild(_content);
                }
            }

            protected override AutomationPeer OnCreateAutomationPeer()
                => new FE1AutomationPeer(this);

            internal override int VisualChildrenCount => Content == null ? 0 : 1;

            internal override UIElement GetVisualChild(int index)
            {
                if (Content is null || index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return Content;
            }
        }

        private class FE1AutomationPeer : FrameworkElementAutomationPeer
        {
            public FE1AutomationPeer(FrameworkElement owner)
                : base(owner)
            {
            }

            public AutomationControlType? AutomationControlType { get; set; }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                if (AutomationControlType.HasValue)
                {
                    return AutomationControlType.Value;
                }

                return base.GetAutomationControlTypeCore();
            }
        }

        private class C1 : Control
        {
            protected override AutomationPeer OnCreateAutomationPeer()
                => new C1AutomationPeer(this);
        }

        private class C1AutomationPeer : FrameworkElementAutomationPeer
        {
            public C1AutomationPeer(C1 owner)
                : base(owner)
            {
            }
        }
    }
}
