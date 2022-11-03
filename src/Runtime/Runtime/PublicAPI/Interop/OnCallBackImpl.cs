
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

using DotNetForHtml5.Core;
using CSHTML5.Types;
using System;

#if BRIDGE
using Bridge;
using DotNetBrowser;
#endif

#if OPENSILVER
#endif

namespace CSHTML5.Internal
{
#if BRIDGE
    [External] //we exclude this class
#endif
    internal class OnCallBackImpl
    {
        private OnCallBackImpl()
        {
        }

        public static OnCallBackImpl Instance { get; } = new OnCallBackImpl();

        public void OnCallbackFromJavaScriptError(string idWhereCallbackArgsAreStored)
        {
            Action action = () =>
            {
                string errorMessage = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("document.jsObjRef[$0][0]", idWhereCallbackArgsAreStored));
                int indexOfNextUnmodifiedJSCallInList = Convert.ToInt32(OpenSilver.Interop.ExecuteJavaScript("document.jsObjRef[$0][1]", idWhereCallbackArgsAreStored));
                INTERNAL_InteropImplementation.ShowErrorMessage(errorMessage, indexOfNextUnmodifiedJSCallInList);
            };

            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                INTERNAL_Simulator.WebControlDispatcherBeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        //---------------------------------------------------------------------------------------
        // This code follows the architecture drawn by DotNetBrowser
        // (cf https://dotnetbrowser.support.teamdev.com/support/solutions/articles/9000109868-calling-javascript-from-net)
        // For an example of implementation, go to INTERNAL_InteropImplementation.cs and
        // ExecuteJavaScript_SimulatorImplementation method, in the first "if".
        //---------------------------------------------------------------------------------------

        public object OnCallbackFromJavaScript<T>(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            T callbackArgsObject,
            bool isInSimulator,
            bool returnValue)
        {
            object result = null;
            var actionExecuted = false;

            void InvokeCallback()
            {
                INTERNAL_SimulatorExecuteJavaScript.RunActionThenExecutePendingAsyncJSCodeExecutedDuringThatAction(
                () =>
                {
                    //--------------------
                    // Call the callback:
                    //--------------------
                    try
                    {
                        result = CallMethod(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject);
                        actionExecuted = true;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("DEBUG: OnCallBack: OnCallBackFromJavascript: " + ex);
                        //#if DEBUG
                        //                            Console.Error.WriteLine("DEBUG: OnCallBack: OnCallBackFromJavascript: " + ex);
                        //#endif
                        //                            throw;
                    }
                });
            }

            if (isInSimulator)
            {
                // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                if (returnValue)
                {
                    var timeout = TimeSpan.FromSeconds(30);
                    INTERNAL_Simulator.WebControlDispatcherInvoke(InvokeCallback, timeout);
                    if (!actionExecuted)
                    {
                        throw GenerateDeadlockException(timeout);
                    }
                }
                else
                {
                    INTERNAL_Simulator.WebControlDispatcherBeginInvoke(InvokeCallback);
                }
            }
            else
            {
                InvokeCallback();
            }

            return returnValue ? result : null;
        }

        private static object DelegateDynamicInvoke(Delegate d, params object[] args)
        {
            return d.DynamicInvoke(args);
        }

        private bool TryOptimizationForCommonTypes(Delegate callback, out object result)
        {
            switch (callback)
            {
                case Action action:
                    action();
                    result = null;
                    return true;

                default:
                    result = null;
                    return false;
            }
        }

        private object CallMethod<T>(int callbackId, string idWhereCallbackArgsAreStored, T callbackArgs)
        {
            //----------------------------------
            // Get the C# callback from its ID:
            //----------------------------------
            var callback = JavascriptCallback.Get(callbackId)?.GetCallback();

            if (callback == null)
            {
                return null;
            }

            if (TryOptimizationForCommonTypes(callback, out object result))
            {
                return result;
            }

            Type callbackType = callback.GetType();
            Type[] callbackGenericArgs = null;
            if (callbackType.IsGenericType)
            {
                callbackGenericArgs = callbackType.GetGenericArguments();
                callbackType = callbackType.GetGenericTypeDefinition();
            }

            if (callbackType == typeof(Func<>))
            {
                return DelegateDynamicInvoke(callback);
            }
            if (callbackType == typeof(Action<>) || callbackType == typeof(Func<,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(1, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,>) || callbackType == typeof(Func<,,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(2, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,>) || callbackType == typeof(Func<,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(3, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,>) || callbackType == typeof(Func<,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(4, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,>) || callbackType == typeof(Func<,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(5, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,,>) || callbackType == typeof(Func<,,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(6, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,,,>) || callbackType == typeof(Func<,,,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(7, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,,,,>) || callbackType == typeof(Func<,,,,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(8, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,,,,,>) || callbackType == typeof(Func<,,,,,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    MakeArgumentsForCallback(9, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }

            throw new Exception(string.Format(
                "Callback type not supported: {0}  Please report this issue to support@cshtml5.com",
                callbackType.ToString()));
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

        private ref struct Test
        {
            private string _temp;
        }

        private static object[] MakeArgumentsForCallback(
            int count,
            string idWhereCallbackArgsAreStored,
            object callbackArgs,
            Type[] callbackGenericArgs)
        {
            var result = new object[count];

            for (int i = 0; i < count; i++)
            {
                var arg = new INTERNAL_JSObjectReference(callbackArgs, idWhereCallbackArgsAreStored, i);
                if (callbackGenericArgs != null
                    && i < callbackGenericArgs.Length
                    && callbackGenericArgs[i] != typeof(object)
                    && (
                    callbackGenericArgs[i].IsPrimitive
                    || callbackGenericArgs[i] == typeof(string)))
                {
                    // Attempt to cast from JS object to the desired primitive or string type. This is useful for example when passing an Action<string> to an Interop.ExecuteJavaScript so as to not get an exception that says that it cannot cast the JS object into string (when running in the Simulator only):
                    result[i] = Convert.ChangeType(arg, callbackGenericArgs[i]);
                }
                else
                {
                    result[i] = arg;
                }
            }
            return result;
        }

        //private static object MakeArgumentForCallbackAtIndex(
        //    string idWhereCallbackArgsAreStored,
        //    object callbackArgs,
        //    Type argType,
        //    int index)
        //{

        //}
    }
}
