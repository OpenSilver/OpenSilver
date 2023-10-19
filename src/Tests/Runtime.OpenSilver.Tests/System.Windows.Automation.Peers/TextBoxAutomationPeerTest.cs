
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
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class TextBoxAutomationPeerTest
    {
        [TestMethod]
        public void GetName()
        {
            var textbox = new TextBox { Text = "Some text" };
            var peer = new TextBoxAutomationPeer(textbox);

            peer.GetName().Should().Be("Some text");

            AutomationProperties.SetName(textbox, "Some name");
            
            peer.GetName().Should().Be("Some name");
        }

        [TestMethod] 
        public void IValueProvider_SetValue_Should_Throw_ElementNotEnabledException_1()
        {
            var textbox = new TextBox { IsEnabled = false };
            var peer = new TextBoxAutomationPeer(textbox);
            var provider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.SetValue("Some text"));
        }

        [TestMethod]
        public void IValueProvider_SetValue_Should_Throw_ElementNotEnabledException_2()
        {
            var textbox = new TextBox { IsReadOnly = true };
            var peer = new TextBoxAutomationPeer(textbox);
            var provider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.SetValue("Some text"));
        }

        [TestMethod]
        public void IValueProvider_SetValue_Should_Throw_ArgumentNullException()
        {
            var textbox = new TextBox();
            var peer = new TextBoxAutomationPeer(textbox);
            var provider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ArgumentNullException>(() => provider.SetValue(null));
        }

        [TestMethod]
        public void IValueProvider_SetValue()
        {
            var textbox = new TextBox();
            var peer = new TextBoxAutomationPeer(textbox);
            var provider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            provider.Should().NotBeNull();
            provider.SetValue("Some text");
            textbox.Text.Should().Be("Some text");
        }

        [TestMethod]
        public void IValueProvider_Value()
        {
            var textbox = new TextBox { Text = "Some text" };
            var peer = new TextBoxAutomationPeer(textbox);
            var provider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            provider.Should().NotBeNull();
            provider.Value.Should().Be("Some text");
        }

        [TestMethod]
        public void IValueProvider_IsReadOnly()
        {
            var textbox = new TextBox();
            var peer = new TextBoxAutomationPeer(textbox);
            var provider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            provider.Should().NotBeNull();

            textbox.IsReadOnly = false;
            provider.IsReadOnly.Should().BeFalse();

            textbox.IsReadOnly = true;
            provider.IsReadOnly.Should().BeTrue();
        }
    }
}
