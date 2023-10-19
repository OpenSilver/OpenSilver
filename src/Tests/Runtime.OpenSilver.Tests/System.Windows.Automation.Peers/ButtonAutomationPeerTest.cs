
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
    public class ButtonAutomationPeerTest
    {
        [TestMethod]
        public void Should_Click_On_Button()
        {
            var buttonClicked = false;
            var button = new Button();
            button.Click += (sender, e) =>
            {
                buttonClicked = true;
            };

            //Some projects use the following approach to click on button
            var buttonAutoPeer = new ButtonAutomationPeer(button);
            var invokeProvider = buttonAutoPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            
            invokeProvider.Should().NotBeNull();
            
            invokeProvider.Invoke();

            Assert.IsTrue(buttonClicked);
        }

        [TestMethod]
        public void Should_Throw_ElementNotEnabledException_For_Disabled_Button()
        {
            var button = new Button
            {
                IsEnabled = false
            };

            var buttonAutoPeer = new ButtonAutomationPeer(button);
            var invokeProvider = buttonAutoPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

            Assert.ThrowsException<ElementNotEnabledException>(() => invokeProvider?.Invoke());
        }
    }
}
