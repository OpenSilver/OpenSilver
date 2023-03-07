

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


using CSHTML5.Internal;
using DotNetForHtml5;
using DotNetForHtml5.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Runtime.OpenSilver.Tests
{
    [TestClass]
    public class TestSetup
    {
        public static event EventHandler<ExecuteJavascriptEventArgs> ExecuteJavascript;

        private static void OnExecuteJavascript(ExecuteJavascriptEventArgs e)
        {
            ExecuteJavascript?.Invoke(null, e);
        }

        private static object ExecuteJsMock(string param)
        {
            var e = new ExecuteJavascriptEventArgs
            {
                Javascript = param
            };

            OnExecuteJavascript(e);

            if (e.Handled)
            {
                return e.Result;
            }

            // Mocks INTERNAL_GridHelpers isCSSGridSupported() and isMSGrid()
            if (param == @"document.isGridSupported" || param == @"document.isMSGrid")
            {
                return false;
            }
            // Mocks Simulator portion of UIElement.TransformToVisual
            // JS code example is:
            // document.callScriptSafe("154","(document.getElementByIdSafe(\"id31\").getBoundingClientRect().left -
            // document.getElementByIdSafe(\"id1\").getBoundingClientRect().left) + '|' +
            // (document.getElementByIdSafe(\"id31\").getBoundingClientRect().top -
            // document.getElementByIdSafe(\"id1\").getBoundingClientRect().top)",108)
            if (Regex.Matches(param, @"\(.+getBoundingClientRect\(\).left - .+.getBoundingClientRect\(\).left\) \+ '\|' \+ \(.+getBoundingClientRect\(\).top - .+.getBoundingClientRect\(\).top\)").Count == 1)
            {
                return JsonDocument.Parse(@"""0|0""").RootElement;
            }
            return new JsonElement();
        }

        /// <summary>
        /// This method will be executed whenever the assembly is loaded,
        /// so before any number of tests being run.
        /// </summary>
        /// <param name="testContext"></param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            var javaScriptExecutionHandlerMock = new Mock<IWebAssemblyExecutionHandler>();
            javaScriptExecutionHandlerMock
                .Setup(x => x.ExecuteJavaScriptWithResult(It.IsAny<string>()))
                .Returns<string>(ExecuteJsMock);
            javaScriptExecutionHandlerMock
                .Setup(x => x.InvokeUnmarshalled<byte[], int, object>(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns<string, byte[], int>((name, bytes, length) => ExecuteJsMock(Encoding.Unicode.GetString(bytes, 0, length)));

            var javaScriptExecutionHandler2 = javaScriptExecutionHandlerMock.Object;
            INTERNAL_Simulator.JavaScriptExecutionHandler = javaScriptExecutionHandler2;

            // Instantiating Application because it sets itself as Application.Current
            _ = new Application
            {
                RootVisual = new Grid(),
            };
        }

        public static void AttachVisualChild(UIElement element)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(element,
                Window.Current);
        }

        public static void SleepWhile(Func<bool> condition, string description = null, int timeoutInMs = 2000)
        {
            const int interval = 100;
            int total = 0;

            while (condition())
            {
                Thread.Sleep(interval);
                total += interval;
                if (total >= timeoutInMs)
                {
                    throw new Exception($"Timed out on waiting while {$"'{description}'" ?? "condition"}." +
                        " Consider increasing timeout or evaluating other conditions" +
                        " that do not take this long to flip.");
                }
            }
        }
    }
}
