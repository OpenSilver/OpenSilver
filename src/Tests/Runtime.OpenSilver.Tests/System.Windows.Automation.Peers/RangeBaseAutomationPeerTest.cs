
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

            Assert.IsNotNull(provider);
            Assert.Throws<ElementNotEnabledException>(() => provider.SetValue(0.5d));
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

            Assert.IsNotNull(provider);
            Assert.Throws<ArgumentOutOfRangeException>(() => provider.SetValue(-1d));
            Assert.Throws<ArgumentOutOfRangeException>(() => provider.SetValue(2d));
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

            Assert.IsNotNull(provider);

            provider.SetValue(0.42d);

            Assert.AreEqual(0.42d, range.Value);
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

            Assert.IsNotNull(provider);
            Assert.AreEqual(0.69d, provider.Value);
        }

        [TestMethod]
        public void IRangeValueProvider_IsReadOnly()
        {
            var range = new MyRangeBase();
            var peer = new RangeBaseAutomationPeer(range);
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;

            Assert.IsNotNull(provider);

            range.IsEnabled = true;
            Assert.IsFalse(provider.IsReadOnly);

            range.IsEnabled = false;
            Assert.IsTrue(provider.IsReadOnly);
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

            Assert.IsNotNull(provider);
            Assert.AreEqual(50d, provider.Maximum);
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

            Assert.IsNotNull(provider);
            Assert.AreEqual(-50d, provider.Minimum);
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

            Assert.IsNotNull(provider);
            Assert.AreEqual(3d, provider.SmallChange);
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

            Assert.IsNotNull(provider);
            Assert.AreEqual(15d, provider.LargeChange);
        }

        private class MyRangeBase : RangeBase { }
    }
}
