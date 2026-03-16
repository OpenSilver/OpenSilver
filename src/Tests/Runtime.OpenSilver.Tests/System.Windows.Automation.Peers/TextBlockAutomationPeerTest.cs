
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

namespace System.Windows.Automation.Peers.Tests
{
    [TestClass]
    public class TextBlockAutomationPeerTest
    {
        [TestMethod]
        public void GetName()
        {
            var textblock = new TextBlock { Text = "Some text" };
            var peer = new TextBlockAutomationPeer(textblock);

            Assert.AreEqual("Some text", peer.GetName());

            AutomationProperties.SetName(textblock, "Some name");

            Assert.AreEqual("Some name", peer.GetName());
        }

        [TestMethod]
        public void IsControlElement()
        {
            var textblock = new TextBlock();
            var peer = new TextBlockAutomationPeer(textblock);

            Assert.IsTrue(peer.IsControlElement());

            textblock.SetTemplatedParent(new(new Button()));

            Assert.IsFalse(peer.IsControlElement());
        }
    }
}
