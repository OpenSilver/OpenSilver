
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


using DotNetForHtml5;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CSHTML5.Internal
{
    [TestClass]
    public class PendingJavascriptTest
    {
        [TestMethod]
        public void Should_Aggregate_Javascript()
        {
            var pj = new PendingJavascriptSimulator(new Mock<IJavaScriptExecutionHandler>().Object);

            pj.AddJavaScript("console.log(1)");
            pj.AddJavaScript("console.log(2)");

            pj.ReadAndClearAggregatedPendingJavaScriptCode().Should().Be("console.log(1);\nconsole.log(2);\n");
        }

        [TestMethod]
        public void Should_Handle_Small_Buffer_Size()
        {
            var pj = new PendingJavascriptSimulator(new Mock<IJavaScriptExecutionHandler>().Object);

            pj.AddJavaScript("console.log(1)");

            pj.ReadAndClearAggregatedPendingJavaScriptCode().Should().Be("console.log(1);\n");
        }

        [TestMethod]
        public void Should_Handle_Circular_Call()
        {
            /*
             * The real possible case: we call JS, it calls C#, which tries to execute pending js again
             */
            var executionHandlerMock = new Mock<IWebAssemblyExecutionHandler>();
            var pj = new PendingJavascript(1024, executionHandlerMock.Object);

            pj.AddJavaScript("console.log(1)");

            executionHandlerMock
                .Setup(x => x.InvokeUnmarshalled<byte[], int, object>(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns<string, byte[], int>((name, bytes, length) => pj.ExecuteJavaScript(null, true));

            var res = pj.ExecuteJavaScript("", true);
            res.Should().BeNull();
        }
    }
}
