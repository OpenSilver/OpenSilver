
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
using System.Windows.Controls.Primitives;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class RangeBaseAutomationPeerTest
    {
        [TestMethod]
        public void IRangeValueProvider_SetValue_Should_Throw_ElementNotEnabledException()
        {
            var peer = new RangeBaseAutomationPeer(new MyRangeBase { IsEnabled = false });
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ElementNotEnabledException>(() => provider.SetValue(0.5d));
        }

        [TestMethod]
        public void IRangeValueProvider_SetValue_Should_Throw_ArgumentOutOfRangeException()
        {
            var peer = new RangeBaseAutomationPeer(new MyRangeBase
            {
                Minimum = 0d,
                Maximum = 1d,
            });
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => provider.SetValue(-1d));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => provider.SetValue(2d));
        }

        [TestMethod]
        public void IRangeValueProvider_SetValue()
        {
            var range = new MyRangeBase
            {
                Minimum = 0d,
                Maximum = 1d,
            };
            var peer = new RangeBaseAutomationPeer(range);
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.SetValue(0.42d);
            range.Value.Should().Be(0.42d);
        }

        [TestMethod]
        public void IRangeValueProvider_Value()
        {
            var range = new MyRangeBase
            {
                Minimum = 0d,
                Maximum = 1d,
                Value = 0.69d,
            };
            var peer = new RangeBaseAutomationPeer(range);
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.Value.Should().Be(0.69d);
        }

        [TestMethod]
        public void IRangeValueProvider_IsReadOnly()
        {
            var range = new MyRangeBase();
            var peer = new RangeBaseAutomationPeer(range);
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();

            range.IsEnabled = true;
            provider.IsReadOnly.Should().BeFalse();

            range.IsEnabled = false;
            provider.IsReadOnly.Should().BeTrue();
        }

        [TestMethod]
        public void IRangeValueProvider_Maximum()
        {
            var peer = new RangeBaseAutomationPeer(new MyRangeBase
            {
                Minimum = -50d,
                Maximum = 50d,
            });
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.Maximum.Should().Be(50d);
        }

        [TestMethod]
        public void IRangeValueProvider_Minimum()
        {
            var peer = new RangeBaseAutomationPeer(new MyRangeBase
            {
                Minimum = -50d,
                Maximum = 50d,
            });
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.Minimum.Should().Be(-50d);
        }

        [TestMethod]
        public void IRangeValueProvider_SmallChange()
        {
            var peer = new RangeBaseAutomationPeer(new MyRangeBase
            {
                SmallChange = 3d,
                LargeChange = 15d,
            });
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.SmallChange.Should().Be(3d);
        }

        [TestMethod]
        public void IRangeValueProvider_LargeChange()
        {
            var peer = new RangeBaseAutomationPeer(new MyRangeBase
            {
                SmallChange = 3d,
                LargeChange = 15d,
            });
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.LargeChange.Should().Be(15d);
        }

        private class MyRangeBase : RangeBase { }
    }
}
