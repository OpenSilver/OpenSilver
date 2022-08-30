
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

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers.Tests
#else
namespace Windows.UI.Xaml.Automation.Peers.Tests
#endif
{
    [TestClass]
    public class ScrollBarAutomationPeerTest
    {
        [TestMethod]
        public void GetClickablePoint()
        {
            var peer = new ScrollBarAutomationPeer(new ScrollBar());
            var point = peer.GetClickablePoint();
            double.IsNaN(point.X).Should().BeTrue();
            double.IsNaN(point.Y).Should().BeTrue();
        }

        [TestMethod]
        public void GetOrientation()
        {
            var scrollbar = new ScrollBar();
            var peer = new ScrollBarAutomationPeer(scrollbar);

            scrollbar.Orientation = Orientation.Horizontal;
            peer.GetOrientation().Should().Be(AutomationOrientation.Horizontal);
            scrollbar.Orientation = Orientation.Vertical;
            peer.GetOrientation().Should().Be(AutomationOrientation.Vertical);
        }
    }
}
