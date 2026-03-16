
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
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class FrameworkElementAutomationPeerTest
    {
        [TestMethod]
        public void CreatePeerForElement_Should_Return_Peer()
        {
            Assert.IsInstanceOfType<FE1AutomationPeer>(FrameworkElementAutomationPeer.CreatePeerForElement(new FE1()));
        }

        [TestMethod]
        public void FromElement_Should_Return_Null()
        {
            Assert.IsNull(FrameworkElementAutomationPeer.FromElement(new FE1()));
        }

        [TestMethod]
        public void FromElement_Should_Return_Peer()
        {
            var fe = new FE1();
            _ = FrameworkElementAutomationPeer.CreatePeerForElement(fe);

            Assert.IsInstanceOfType<FE1AutomationPeer>(FrameworkElementAutomationPeer.FromElement(fe));
        }

        [TestMethod]
        public void Peer_Should_Be_Cached()
        {
            var fe = new FE1();

            var peer1 = FrameworkElementAutomationPeer.CreatePeerForElement(fe);
            var peer2 = FrameworkElementAutomationPeer.CreatePeerForElement(fe);
            var peer3 = FrameworkElementAutomationPeer.FromElement(fe);

            Assert.AreSame(peer1, peer2);
            Assert.AreSame(peer2, peer3);
        }

        [TestMethod]
        public void FromElement_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => FrameworkElementAutomationPeer.FromElement(null));
        }

        [TestMethod]
        public void CreatePeerForElement_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => FrameworkElementAutomationPeer.CreatePeerForElement(null));
        }

        [TestMethod]
        public void Owner_Should_Not_Be_Null()
        {
            var fe = new FE1();

            Assert.AreSame(FrameworkElementAutomationPeer.CreatePeerForElement(fe).As<FrameworkElementAutomationPeer>().Owner, fe);
        }

        [TestMethod]
        public void GetAcceleratorKey_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "AcceleratorKey";
            AutomationProperties.SetAcceleratorKey(fe, value);

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).GetAcceleratorKey(), value);
        }

        [TestMethod]
        public void GetAccessKey_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "AccessKey";
            AutomationProperties.SetAccessKey(fe, value);

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).GetAccessKey(), value);
        }

        [TestMethod]
        public void GetAutomationId_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "AutomationId";
            AutomationProperties.SetAutomationId(fe, value);

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).GetAutomationId(), value);
        }

        [TestMethod]
        public void GetHelpText_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "HelpText";
            AutomationProperties.SetHelpText(fe, value);

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).GetHelpText(), value);
        }

        [TestMethod]
        public void GetItemStatus_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "ItemStatus";
            AutomationProperties.SetItemStatus(fe, value);

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).GetItemStatus(), value);
        }

        [TestMethod]
        public void GetItemType_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "ItemType";
            AutomationProperties.SetItemType(fe, value);

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).GetItemType(), value);
        }

        [TestMethod]
        public void GetLabeledBy_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            var c = new C1();
            AutomationProperties.SetLabeledBy(fe, c);

            Assert.AreSame(FrameworkElementAutomationPeer.CreatePeerForElement(fe).GetLabeledBy(), FrameworkElementAutomationPeer.FromElement(c));
        }

        [TestMethod]
        public void GetName_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            string value = "Name";
            AutomationProperties.SetName(fe, value);

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).GetName(), value);
        }

        [TestMethod]
        public void IsRequiredForForm_Should_Use_AutomationProperties()
        {
            var fe = new FE1();
            bool value = true;
            AutomationProperties.SetIsRequiredForForm(fe, value);

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).IsRequiredForForm(), value);
        }

        [TestMethod]
        public void GetAutomationControlType_Should_Return_Custom()
        {
            Assert.AreEqual(AutomationControlType.Custom, FrameworkElementAutomationPeer.CreatePeerForElement(new FE1()).GetAutomationControlType());
        }

        [TestMethod]
        public void GetClassName_Should_Return_String_Empty()
        {
            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(new FE1()).GetClassName(), string.Empty);
        }

        [TestMethod]
        public void IsContentElement_Should_Return_True()
        {
            Assert.IsTrue(FrameworkElementAutomationPeer.CreatePeerForElement(new FE1()).IsContentElement());
        }

        [TestMethod]
        public void IsControlElement_Should_Return_True()
        {
            Assert.IsTrue(FrameworkElementAutomationPeer.CreatePeerForElement(new FE1()).IsControlElement());
        }

        [TestMethod]
        public void IsPassword_Should_Return_True()
        {
            Assert.IsFalse(FrameworkElementAutomationPeer.CreatePeerForElement(new FE1()).IsPassword());
        }

        [TestMethod]
        public void IsEnabled_Should_Return_FrameworkElement_IsEnabled()
        {
            var fe = new FE1();
            fe.IsEnabled = true;

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).IsEnabled(), fe.IsEnabled);

            fe.IsEnabled = false;

            Assert.AreEqual(FrameworkElementAutomationPeer.CreatePeerForElement(fe).IsEnabled(), fe.IsEnabled);
        }

        [TestMethod]
        public void GetLocalizedControl_Should_Return_Lowered_AutomationControlType()
        {
            var peer = new FE1AutomationPeer(new FE1());

            foreach (AutomationControlType controlType in Enum.GetValues(typeof(AutomationControlType)))
            {
                peer.AutomationControlType = controlType;

                Assert.AreEqual(peer.GetLocalizedControlType(), peer.GetAutomationControlType().ToString().ToLower());
            }
        }

        [TestMethod]
        public void GetLocalizedControl_Should_Return_String_Empty()
        {
            Assert.AreEqual(new FE1AutomationPeer(new FE1()) { AutomationControlType = (AutomationControlType)50 }.GetLocalizedControlType(), string.Empty);
        }

        [TestMethod]
        public void GetOrientation_Should_Return_None()
        {
            Assert.AreEqual(AutomationOrientation.None, new FE1AutomationPeer(new FE1()).GetOrientation());
        }

        [TestMethod]
        public void GetParent_Should_Return_Null()
        {
            Assert.IsNull(new FE1AutomationPeer(new FE1()).GetParent());
        }

        [TestMethod]
        public void GetParent_Should_Use_Visual_Parent_1()
        {
            var fe = new FE1();
            var c = new C1();
            fe.Content = c;

            Assert.AreSame(FrameworkElementAutomationPeer.CreatePeerForElement(c).GetParent(), FrameworkElementAutomationPeer.FromElement(fe));
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

            Assert.AreSame(FrameworkElementAutomationPeer.CreatePeerForElement(c).GetParent(), FrameworkElementAutomationPeer.FromElement(fe));
        }

        [TestMethod]
        public void IsOffScreen_Should_Return_Not_IsVisible()
        {
            var fe = new FE1();
            var peer = new FE1AutomationPeer(fe);

            Assert.IsFalse(fe.IsVisible);
            Assert.AreEqual(peer.IsOffscreen(), !fe.IsVisible);

            using (var wrapper = new FocusableControlWrapper<FE1>(fe))
            {
                Assert.IsTrue(fe.IsVisible);
                Assert.AreEqual(peer.IsOffscreen(), !fe.IsVisible);
            }
        }

        [TestMethod]
        public void SetFocus_Should_Set_Focus_For_Control()
        {
            using (var wrapper = new FocusableControlWrapper<C1>(new C1()))
            {
                var peer = new C1AutomationPeer(wrapper.Control);

                peer.SetFocus();

                Assert.AreSame(FocusManager.GetFocusedElement(), wrapper.Control);

                FocusManager.SetFocusedElement(Window.Current, null);

                wrapper.Control.IsEnabled = false;

                peer.SetFocus();

                Assert.IsNull(FocusManager.GetFocusedElement());
            }
        }

        [TestMethod]
        public void SetFocus_Should_Not_Set_Focus_For_FrameworkElement()
        {
            using (var wrapper = new FocusableControlWrapper<FE1>(new FE1()))
            {
                var peer = new FE1AutomationPeer(wrapper.Control);
                peer.SetFocus();

                Assert.IsNull(FocusManager.GetFocusedElement());

                wrapper.Control.IsEnabled = false;
                peer.SetFocus();

                Assert.IsNull(FocusManager.GetFocusedElement());
            }
        }

        [TestMethod]
        public void GetChildren_Should_Return_Null()
        {
            Assert.IsNull(new FE1AutomationPeer(new FE1()).GetChildren());
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

            Assert.HasCount(2, children);
            Assert.IsInstanceOfType<C1AutomationPeer>(children[0]);
            Assert.AreSame(children[0].As<C1AutomationPeer>().Owner, c1);
            Assert.IsInstanceOfType<C1AutomationPeer>(children[1]);
            Assert.AreSame(children[1].As<C1AutomationPeer>().Owner, c2);
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

            Assert.HasCount(2, children);
            Assert.IsInstanceOfType<C1AutomationPeer>(children[0]);
            Assert.AreSame(children[0].As<C1AutomationPeer>().Owner, c1);
            Assert.IsInstanceOfType<FE1AutomationPeer>(children[1]);
            Assert.AreSame(children[1].As<FE1AutomationPeer>().Owner, fe2);
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

            protected override int VisualChildrenCount => Content == null ? 0 : 1;

            protected override UIElement GetVisualChild(int index)
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
