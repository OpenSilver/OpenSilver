using DotNetForHtml5;
using DotNetForHtml5.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Text.Json;
using System.Text.RegularExpressions;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Runtime.OpenSilver.Tests.Setup
{
    [TestClass]
    public class TestSetup
    {
        /// <summary>
        /// This method will be executed whenever the assembly is loaded,
        /// so before any number of tests being run.
        /// </summary>
        /// <param name="testContext"></param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            Mock<IJavaScriptExecutionHandler> javaScriptExecutionHandlerMock = new Mock<IJavaScriptExecutionHandler>();
            javaScriptExecutionHandlerMock
                .Setup(x => x.ExecuteJavaScriptWithResult(It.IsAny<string>()))
                .Returns<string>(param =>
                {
                    // JS code example is: document.callScriptSafe("14","document.isGridSupported",9)
                    if (Regex.Matches(param, @"document.callScriptSafe\(""\d+"",""document.isGridSupported"",\d+\)").Count == 1)
                    {
                        return JsonDocument.Parse("false").RootElement;
                    }
                    return new JsonElement();
                });

            IJavaScriptExecutionHandler javaScriptExecutionHandler = javaScriptExecutionHandlerMock.Object;
            INTERNAL_Simulator.JavaScriptExecutionHandler = javaScriptExecutionHandler;

            // Instantiating Application because it sets itself as Application.Current
            new Application();
        }
    }
}
