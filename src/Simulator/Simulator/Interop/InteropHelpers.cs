
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

extern alias opensilver;

using Microsoft.Web.WebView2.Wpf;
using System.Windows.Threading;
using INTERNAL_Simulator = opensilver::DotNetForHtml5.Core.INTERNAL_Simulator;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal static class InteropHelpers
    {
        internal static void InjectIsRunningInTheSimulator_WorkAround()
        {
            INTERNAL_Simulator.IsRunningInTheSimulator_WorkAround = true;
        }

        internal static void InjectWebControlDispatcher(WebView2 webControl)
        {
            INTERNAL_Simulator.WebControlDispatcherBeginInvoke = (method) => webControl.Dispatcher.BeginInvoke(method);
            INTERNAL_Simulator.WebControlDispatcherInvoke = (method, timeout) => webControl.Dispatcher.Invoke(method, timeout);
            INTERNAL_Simulator.WebControlDispatcherCheckAccess = () => webControl.Dispatcher.CheckAccess();
        }

        internal static void InjectJavaScriptExecutionHandler(opensilver::DotNetForHtml5.IJavaScriptExecutionHandler javaScriptExecutionHandler)
        {
            INTERNAL_Simulator.JavaScriptExecutionHandler = javaScriptExecutionHandler;
        }

        internal static void InjectWebClientFactory()
        {
            INTERNAL_Simulator.WebClientFactory = new WebClientFactory();
        }

        internal static void InjectClipboardHandler()
        {
            INTERNAL_Simulator.AsyncClipboard = new ClipboardHandler();
        }

        internal static void InjectSimulatorProxy(SimulatorProxy simulatorProxy)
        {
            INTERNAL_Simulator.SimulatorProxy = simulatorProxy;
        }

        internal static void InjectOpenSilverRuntimeDispatcher(Dispatcher dispatcher)
        {
            INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke = (method) => dispatcher.BeginInvoke(method);
            INTERNAL_Simulator.OpenSilverDispatcherInvoke = (method, timeout) => dispatcher.Invoke(method, timeout);
            INTERNAL_Simulator.OpenSilverDispatcherCheckAccess = () => dispatcher.CheckAccess();
        }

        internal static void InjectCodeToDisplayTheMessageBox(Func<string, string, bool, bool> codeToShowTheMessageBoxWithTitleAndButtons)
        {
            opensilver::System.Windows.MessageBox.INTERNAL_CodeToShowTheMessageBoxWithTitleAndButtons = codeToShowTheMessageBoxWithTitleAndButtons;
        }
    }
}
