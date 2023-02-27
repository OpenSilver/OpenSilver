
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
using Runtime.OpenSilver.PublicAPI.Interop;

namespace Runtime.OpenSilver.Tests.PublicApi.Interop
{
    [TestClass]
    public class PendingJavascriptTest
    {
        [TestMethod]
        public void Should_Aggregate_Javascript()
        {
            var pj = new PendingJavascript(1024);

            pj.AddJavascript("console.log(1)");
            pj.AddJavascript("console.log(2)");

            pj.TakeJsOut().Should().Be("console.log(1);\nconsole.log(2);\n");
        }

        [TestMethod]
        public void Should_Handle_Small_Buffer_Size()
        {
            var pj = new PendingJavascript(4);

            pj.AddJavascript("console.log(1)");

            pj.TakeJsOut().Should().Be("console.log(1);\n");
        }

        [TestMethod]
        public void Should_Handle_Circular_Call()
        {
            /*
             * The real possible case: we call JS, it calls C#, which tries to execute pending js again
             */
            var pj = new PendingJavascript(1024);

            pj.AddJavascript("console.log(1)");

            var executionHandlerMock = new Mock<IWebAssemblyExecutionHandler>();
            executionHandlerMock
                .Setup(x => x.InvokeUnmarshalled<byte[], int, object>(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns<string, byte[], int>((name, bytes, length) => pj.ExecutePending(executionHandlerMock.Object));

            var res = pj.ExecutePending(executionHandlerMock.Object);
            res.Should().BeNull();
        }
    }
}
