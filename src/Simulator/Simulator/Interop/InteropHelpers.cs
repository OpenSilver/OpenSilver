

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using Microsoft.Web.WebView2.Wpf;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    static class InteropHelpers
    {
        internal static void InjectIsRunningInTheSimulator_WorkAround(Assembly coreAssembly)
        {
            InjectPropertyValue("IsRunningInTheSimulator_WorkAround", true, coreAssembly);
        }

        internal static void InjectWebControlDispatcher(WebView2 webControl, Assembly coreAssembly)
        {
            InjectPropertyValue("WebControlDispatcherBeginInvoke",
                new Action<Action>((method) => webControl.Dispatcher.BeginInvoke(method)), coreAssembly);

            InjectPropertyValue("WebControlDispatcherInvoke",
                new Action<Action, TimeSpan>((method, timeout) => webControl.Dispatcher.Invoke(method, timeout)), coreAssembly);

            InjectPropertyValue("WebControlDispatcherCheckAccess",
                new Func<bool>(() => webControl.Dispatcher.CheckAccess()), coreAssembly);
        }

        internal static void InjectJavaScriptExecutionHandler(dynamic javaScriptExecutionHandler, Assembly coreAssembly)
        {
            InjectPropertyValue("DynamicJavaScriptExecutionHandler", javaScriptExecutionHandler, coreAssembly);
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

        internal static void InjectSimulatorCallbackSetup(Action<object> callbackSetup, Assembly coreAssembly)
        {
            InjectPropertyValue("SimulatorCallbackSetup", callbackSetup, coreAssembly);
        }

        internal static void InjectOpenSilverRuntimeDispatcher(Dispatcher dispatcher, Assembly coreAssembly)
        {
            InjectPropertyValue("OpenSilverDispatcherBeginInvoke",
                new Action<Action>((method) => dispatcher.BeginInvoke(method)), coreAssembly);

            InjectPropertyValue("OpenSilverDispatcherInvoke",
                new Action<Action, TimeSpan>((method, timeout) => dispatcher.Invoke(method, timeout)), coreAssembly);

            InjectPropertyValue("OpenSilverDispatcherCheckAccess",
                new Func<bool>(() => dispatcher.CheckAccess()), coreAssembly);
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
