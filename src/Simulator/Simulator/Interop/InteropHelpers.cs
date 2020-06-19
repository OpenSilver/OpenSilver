using DotNetBrowser;
using DotNetBrowser.DOM;
using System;
using System.Reflection;
using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    static class InteropHelpers
    {
#if OPENSILVER
        internal static void InjectIsRunningInTheSimulator_WorkAround(Assembly coreAssembly)
        {
            InjectPropertyValue("IsRunningInTheSimulator_WorkAround", true, coreAssembly);
        }
#endif

        internal static void InjectDOMDocument(DOMDocument document, Assembly coreAssembly)
        {
            InjectPropertyValue("DOMDocument", document, coreAssembly);
        }

        internal static void InjectHtmlDocument(JSValue htmlDocument, Assembly coreAssembly)
        {
            InjectPropertyValue("HtmlDocument", htmlDocument, coreAssembly);
        }

        internal static void InjectWebControlDispatcherBeginInvoke(dynamic webControl, Assembly coreAssembly)
        {
            InjectPropertyValue("WebControlDispatcherBeginInvoke", new Action<Action>((method) => webControl.Dispatcher.BeginInvoke(method)), coreAssembly);
        }

        internal static void InjectJavaScriptExecutionHandler(dynamic javaScriptExecutionHandler, Assembly coreAssembly)
        {
#if OPENSILVER
            InjectPropertyValue("DynamicJavaScriptExecutionHandler", javaScriptExecutionHandler, coreAssembly);
#else
            InjectPropertyValue("JavaScriptExecutionHandler", javaScriptExecutionHandler, coreAssembly);
#endif
        }

        internal static void InjectWpfMediaElementFactory(Assembly coreAssembly)
        {
            InjectPropertyValue("WpfMediaElementFactory", new WpfMediaElementFactory(), coreAssembly);
        }

        internal static void InjectWebClientFactory(Assembly coreAssembly)
        {
            InjectPropertyValue("WebClientFactory", new WebClientFactory(), coreAssembly);
        }

        internal static void InjectClipboardHandler(Assembly coreAssembly)
        {
            InjectPropertyValue("ClipboardHandler", new ClipboardHandler(), coreAssembly);
        }

        internal static void InjectSimulatorProxy(SimulatorProxy simulatorProxy, Assembly coreAssembly)
        {
            InjectPropertyValue("SimulatorProxy", simulatorProxy, coreAssembly);
        }

        internal static dynamic GetPropertyValue(string propertyName, Assembly coreAssembly)
        {
            var typeInCoreAssembly = coreAssembly.GetType("DotNetForHtml5.Core.INTERNAL_Simulator");
            if (typeInCoreAssembly != null)
            {
                PropertyInfo staticProperty = typeInCoreAssembly.GetProperty(propertyName);
                if (staticProperty != null)
                {
                    return staticProperty.GetValue(null);
                }
                else
                {
                    MessageBox.Show("ERROR: Could not find the public static property \"" + propertyName + "\" in the type \"INTERNAL_Simulator\" in the core assembly.");
                    return null;
                }
            }
            else
            {
                MessageBox.Show("ERROR: Could not find the type \"INTERNAL_Simulator\" in the core assembly.");
                return null;
            }
        }

        static void InjectPropertyValue(string propertyName, object propertyValue, Assembly coreAssembly)
        {
            Type typeInCoreAssembly = coreAssembly.GetType("DotNetForHtml5.Core.INTERNAL_Simulator");
            if (typeInCoreAssembly == null)
            {
                MessageBox.Show("ERROR: Could not find the type \"INTERNAL_Simulator\" in the core assembly.");
                return;
            }

            PropertyInfo staticProperty = typeInCoreAssembly.GetProperty(propertyName);
            if (staticProperty == null)
            {
                MessageBox.Show("ERROR: Could not find the public static property \"" + propertyName + "\" in the type \"INTERNAL_Simulator\" in the core assembly.");
                return;
            }

            staticProperty.SetValue(null, propertyValue);
        }

        internal static void InjectCodeToDisplayTheMessageBox(
            Func<string, string, bool, bool> codeToShowTheMessageBoxWithTitleAndButtons,
            Assembly coreAssembly)
        {
            Type type = coreAssembly.GetType("Windows.UI.Xaml.MessageBox");
            if (type == null)
            {
                type = coreAssembly.GetType("System.Windows.MessageBox"); // For "SL Migration" projects.
            }
            if (type != null)
            {
                PropertyInfo staticProperty = type.GetProperty("INTERNAL_CodeToShowTheMessageBoxWithTitleAndButtons");
                if (staticProperty != null)
                {
                    staticProperty.SetValue(null, codeToShowTheMessageBoxWithTitleAndButtons);
                }
                else
                {
                    MessageBox.Show("ERROR: Could not find the public static property \"INTERNAL_CodeToShowTheMessageBoxWithTitleAndButtons\" in the type \"MessageBox\" in the core assembly.");
                }
            }
            else
            {
                MessageBox.Show("ERROR: Could not find the type \"MessageBox\" in the core assembly.");
            }
        }

        internal static void RaiseReloadedEvent(Assembly coreAssembly)
        {
            var type = coreAssembly.GetType("Windows.UI.Xaml.Application");
            if (type == null)
                type = coreAssembly.GetType("System.Windows.Application");
            var method = type.GetMethod("INTERNAL_RaiseReloadedEvent", BindingFlags.Static | BindingFlags.Public);
            method.Invoke(null, null);
        }
    }
}
