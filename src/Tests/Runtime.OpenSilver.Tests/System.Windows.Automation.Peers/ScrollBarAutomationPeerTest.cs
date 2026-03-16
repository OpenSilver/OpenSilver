
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class ScrollBarAutomationPeerTest
    {
        [TestMethod]
        public void GetClickablePoint()
        {
            var peer = new ScrollBarAutomationPeer(new ScrollBar());
            var point = peer.GetClickablePoint();

            Assert.IsTrue(double.IsNaN(point.X));
            Assert.IsTrue(double.IsNaN(point.Y));
        }

        [TestMethod]
        public void GetOrientation()
        {
            var scrollbar = new ScrollBar();
            var peer = new ScrollBarAutomationPeer(scrollbar);

            scrollbar.Orientation = Orientation.Horizontal;
            Assert.AreEqual(AutomationOrientation.Horizontal, peer.GetOrientation());

            scrollbar.Orientation = Orientation.Vertical;
            Assert.AreEqual(AutomationOrientation.Vertical, peer.GetOrientation());
        }
    }
}
