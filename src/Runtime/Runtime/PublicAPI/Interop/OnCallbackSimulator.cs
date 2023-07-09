
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

using System;
using System.Runtime.InteropServices;
using DotNetForHtml5.Core;

namespace CSHTML5.Internal
{
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public sealed class OnCallbackSimulator
    {
        public OnCallbackSimulator()
        {
            CheckIsRunningInTheSimulator();
        }

        public void OnCallbackFromJavaScriptError(string idWhereCallbackArgsAreStored)
        {
            OnCallBackImpl.Instance.OnCallbackFromJavaScriptError(idWhereCallbackArgsAreStored);
        }

        public object OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object callbackArgsObject,
            bool returnValue)
        {
            object result = null;
            var actionExecuted = false;

            void InvokeCallback()
            {
                try
                {
                    result = OnCallBackImpl.Instance.OnCallbackFromJavaScript(
                        callbackId,
                        idWhereCallbackArgsAreStored,
                        callbackArgsObject);
                    
                    actionExecuted = true;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("DEBUG: OnCallBack: OnCallBackFromJavascript: " + ex);
                    throw;
                }

                INTERNAL_ExecuteJavaScript.ExecutePendingJavaScriptCode();
            }

            // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
            if (returnValue)
            {
                var timeout = TimeSpan.FromSeconds(30);
                INTERNAL_Simulator.OpenSilverDispatcherInvoke(InvokeCallback, timeout);
                if (!actionExecuted)
                {
                    throw GenerateDeadlockException(timeout);
                }
            }
            else
            {
                INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke(InvokeCallback);
            }

            return returnValue ? result : null;
        }

        private static void CheckIsRunningInTheSimulator()
        {
            if (!OpenSilver.Interop.IsRunningInTheSimulator)
            {
                throw new InvalidOperationException($"'{nameof(OnCallbackSimulator)}' is not supported in the browser.");
            }
        }

        private static ApplicationException GenerateDeadlockException(TimeSpan timeout)
        {
            return new ApplicationException(
                $"The callback method has not finished execution in {timeout} seconds.\n" +
                "This method was called in a sync way, and very likely, the process is deadlocked. It happens when the code from the UI thread calls JS code, which calls C# back synchronously.\n" +
                "The Example:\n" +
                "OpenSilver.Interop.ExecuteJavaScript(\"$0();\", (Func<string>)(() => \"Message from C#\"));\n" +
                "The solution:\n" +
                "If a callback returns a value(for example, a Func), verify that this callback is not invoked from the C# code in the UI thread.");
        }
    }
}
