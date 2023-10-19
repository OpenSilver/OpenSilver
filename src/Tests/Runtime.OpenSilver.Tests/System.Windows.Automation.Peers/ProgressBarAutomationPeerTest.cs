
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
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class ProgressBarAutomationPeerTest
    {
        [TestMethod]
        public void IRangeValueProvider_SetValue_Should_Throw_InvalidOperationException()
        {
            var peer = new ProgressBarAutomationPeer(new ProgressBar());
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            Assert.ThrowsException<InvalidOperationException>(() => provider.SetValue(42));
        }

        [TestMethod]
        public void IRangeValueProvider_IsReadOnly_Should_Return_True()
        {
            var peer = new ProgressBarAutomationPeer(new ProgressBar());
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.IsReadOnly.Should().BeTrue();
        }

        [TestMethod]
        public void IRangeValueProvider_LargeChange_Should_Return_NaN()
        {
            var peer = new ProgressBarAutomationPeer(new ProgressBar());
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.LargeChange.Should().Be(double.NaN);
        }

        [TestMethod]
        public void IRangeValueProvider_SmallChange_Should_Return_NaN()
        {
            var peer = new ProgressBarAutomationPeer(new ProgressBar());
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.SmallChange.Should().Be(double.NaN);
        }

        [TestMethod]
        public void IRangeValueProvider_Minimum()
        {
            var progressBar = new ProgressBar { Minimum = 50d, Maximum = 100d };
            var peer = new ProgressBarAutomationPeer(progressBar);
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.Minimum.Should().Be(50d);
        }

        [TestMethod]
        public void IRangeValueProvider_Maximum()
        {
            var progressBar = new ProgressBar { Minimum = 50d, Maximum = 100d };
            var peer = new ProgressBarAutomationPeer(progressBar);
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.Maximum.Should().Be(100d);
        }

        [TestMethod]
        public void IRangeValueProvider_Value()
        {
            var progressBar = new ProgressBar { Minimum = 50d, Maximum = 100d, Value = 61d, };
            var peer = new ProgressBarAutomationPeer(progressBar);
            var provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            provider.Should().NotBeNull();
            provider.Value.Should().Be(61d);
        }
    }
}
