
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
using OpenSilver.Internal;
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
    public class TextBlockAutomationPeerTest
    {
        [TestMethod]
        public void GetName()
        {
            var textblock = new TextBlock { Text = "Some text" };
            var peer = new TextBlockAutomationPeer(textblock);

            peer.GetName().Should().Be("Some text");

            AutomationProperties.SetName(textblock, "Some name");

            peer.GetName().Should().Be("Some name");
        }

        [TestMethod]
        public void IsControlElement()
        {
            var textblock = new TextBlock();
            var peer = new TextBlockAutomationPeer(textblock);

            peer.IsControlElement().Should().BeTrue();

            textblock.TemplatedParent = new Button();

            peer.IsControlElement().Should().BeFalse();
        }
    }
}
