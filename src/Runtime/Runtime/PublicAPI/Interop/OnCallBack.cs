

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
using System.Collections.Generic;

#if BRIDGE
using Bridge;
using DotNetBrowser;
#endif

namespace CSHTML5.Internal
{
#if BRIDGE
    [External] //we exclude this class
#endif
    // In Previous Net Standard Version OnCallback was a static class but because OpenSilver has too match both simulator and browser API
    // The Class may be instantiated and cannot be static anymore
    public class OnCallBack
    {
        private static SynchronyzedStore<Delegate> _store;

        internal static void SetCallbacksDictionary(SynchronyzedStore<Delegate> store)
        {
            _store = store;
        }

        //constructor
        internal OnCallBack(SynchronyzedStore<Delegate> store)
        {
            _store = store;
        }

        #region CallBack methods

#if BRIDGE
        public void UpdateDictionary(Dictionary<int, Delegate> newDictionary)
        {
            _dictionary = newDictionary;
        }
#endif

#if OPENSILVER
        [Microsoft.JSInterop.JSInvokable]
#endif
        public static void OnCallbackFromJavaScriptError(string idWhereCallbackArgsAreStored)
        {
            Action action = () =>
            {
                string errorMessage = Convert.ToString(Interop.ExecuteJavaScript("document.jsObjRef[$0][0]", idWhereCallbackArgsAreStored));
                int indexOfNextUnmodifiedJSCallInList = Convert.ToInt32(Interop.ExecuteJavaScript("document.jsObjRef[$0][1]", idWhereCallbackArgsAreStored));
                INTERNAL_InteropImplementation.ShowErrorMessage(errorMessage, indexOfNextUnmodifiedJSCallInList);
            };

#if OPENSILVER
            if (Interop.IsRunningInTheSimulator_WorkAround)
#else
            if (Interop.IsRunningInTheSimulator)
#endif
            {
                // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                INTERNAL_Simulator.WebControlDispatcherBeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        private static object DelegateDynamicInvoke(Delegate d, params object[] args)
        {
#if OPENSILVER
            return d.DynamicInvoke(args);
#elif BRIDGE
            return INTERNAL_Simulator.SimulatorProxy.DelegateDynamicInvoke(d, args);
#endif
        }

        private static object CallMethod<T>(int callbackId, string idWhereCallbackArgsAreStored, Func<int, int, string, T, Type[], object[]> makeArguments,
            T callbackArgs)
        {
            //----------------------------------
            // Get the C# callback from its ID:
            //----------------------------------
            Delegate callback = _store.Get(callbackId);

            Type callbackType = callback.GetType();
            Type[] callbackGenericArgs = null;
            if (callbackType.IsGenericType)
            {
                callbackGenericArgs = callbackType.GetGenericArguments();
                callbackType = callbackType.GetGenericTypeDefinition();
            }

            if (callbackType == typeof(Action) || callbackType == typeof(Func<>))
            {
                return DelegateDynamicInvoke(callback);
            }
            if (callbackType == typeof(Action<>) || callbackType == typeof(Func<,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(1, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,>) || callbackType == typeof(Func<,,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(2, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,>) || callbackType == typeof(Func<,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(3, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,>) || callbackType == typeof(Func<,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(4, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,>) || callbackType == typeof(Func<,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(5, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,,>) || callbackType == typeof(Func<,,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(6, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,,,>) || callbackType == typeof(Func<,,,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(7, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,,,,>) || callbackType == typeof(Func<,,,,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(8, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }
            if (callbackType == typeof(Action<,,,,,,,,>) || callbackType == typeof(Func<,,,,,,,,,>))
            {
                return DelegateDynamicInvoke(callback,
                    makeArguments(9, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
            }

            throw new Exception(string.Format(
                "Callback type not supported: {0}  Please report this issue to support@cshtml5.com",
                callbackType.ToString()));
        }

        private static void GenerateDeadlockException(TimeSpan timeout)
        {
            throw new ApplicationException(
                $"The callback method has not finished execution in {timeout} seconds.\n" +
                "This method was called in a sync way, and very likely, the process is deadlocked. It happens when the code from the UI thread calls JS code, which calls C# back synchronously.\n" +
                "The Example:\n" +
                "OpenSilver.Interop.ExecuteJavaScript(\"$0();\", (Func<string>)(() => \"Message from C#\"));\n" +
                "The solution:\n" +
                "If a callback returns a value(for example, a Func), verify that this callback is not invoked from the C# code in the UI thread.");
        }

        //---------------------------------------------------------------------------------------
        // This code follows the architecture drawn by DotNetBrowser
        // (cf https://dotnetbrowser.support.teamdev.com/support/solutions/articles/9000109868-calling-javascript-from-net)
        // For an example of implementation, go to INTERNAL_InteropImplementation.cs and
        // ExecuteJavaScript_SimulatorImplementation method, in the first "if".
        //---------------------------------------------------------------------------------------

#if OPENSILVER

        private static object OnCallbackFromJavaScript<T>(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            T callbackArgsObject,
            Func<int, int, string, T, Type[], object[]> makeArguments,
            bool isInSimulator,
            bool returnValue)
        {
            object result = null;
            var actionExecuted = false;
            Action action = () =>
            {
                INTERNAL_SimulatorExecuteJavaScript.RunActionThenExecutePendingAsyncJSCodeExecutedDuringThatAction(
                    () =>
                    {
                        //--------------------
                        // Call the callback:
                        //--------------------
                        try
                        {
                            result = CallMethod(callbackId, idWhereCallbackArgsAreStored, makeArguments, callbackArgsObject);
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
            };
            if (isInSimulator)
            {
                // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                if (returnValue)
                {
                    var timeout = TimeSpan.FromSeconds(30);
                    INTERNAL_Simulator.WebControlDispatcherInvoke(action, timeout);
                    if (!actionExecuted)
                    {
                        GenerateDeadlockException(timeout);
                    }
                }
                else
                {
                    INTERNAL_Simulator.WebControlDispatcherBeginInvoke(action);
                }
            }
            else
            {
                action();
            }

            return returnValue ? result : null;
        }

        // In The OpenSilver Version, there are 2 options for callback:
        // - Browser   Version: the callback is called from js but using the Microsoft Interop. It uses an object[]
        // - Simulator Version: the callback is called from js but using the DotNetBrowser Interop. It uses a  JSArray

        #region Simulator

        // This method can be removed later. Now it is used for easier migration from old cshtml5.js to new one
        public static object OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object callbackArgsObject)
        {
            return OnCallbackFromJavaScript(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject,
                false);
        }

        public static object OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object callbackArgsObject,
            bool returnValue)
        {
            return OnCallbackFromJavaScript(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject,
                MakeArgumentsForCallbackSimulator, true, returnValue);
        }

        private static object[] MakeArgumentsForCallbackSimulator(
            int count,
            int callbackId,
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

        #endregion Simulator

        #region Blazor

        // This method can be removed later. Now it is used for easier migration from old opensilver.js to new one
        [Microsoft.JSInterop.JSInvokable]
        public static object OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object[] callbackArgsObject)
        {
            return OnCallbackFromJavaScriptBrowser(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject,
                false);
        }

        [Microsoft.JSInterop.JSInvokable]
        public static object OnCallbackFromJavaScriptBrowser(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object[] callbackArgsObject,
            bool returnValue)
        {
            return OnCallbackFromJavaScript(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject,
                MakeArgumentsForCallbackBrowser, false, returnValue);
        }

        private static object[] MakeArgumentsForCallbackBrowser(
            int count,
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object[] callbackArgs,
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

        #endregion Blazor

#elif BRIDGE

        public object OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            JSArray callbackArgsObject,
            bool returnValue)
        {
            object result = null;
            var actionExecuted = false;
            Action action = () =>
            {
                //--------------------
                // Call the callback:
                //--------------------
                try
                {
                    result = CallMethod(callbackId, idWhereCallbackArgsAreStored, MakeArgumentsForCallback, callbackArgsObject);
                    actionExecuted = true;
                }
                catch (global::System.Reflection.TargetInvocationException ex)
                {
                    if (global::System.Diagnostics.Debugger.IsAttached)
                    {
                        if (ex.InnerException != null)
                        {
                            // We rethrow the inner exception if any, that is, the exception that happens in the C# code being called by "DynamicInvoke". cf. https://stackoverflow.com/questions/57383/in-c-how-can-i-rethrow-innerexception-without-losing-stack-trace
                            INTERNAL_Simulator.SimulatorProxy.ThrowExceptionWithoutLosingStackTrace(ex.InnerException);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        INTERNAL_Simulator.SimulatorProxy.ShowException(ex.InnerException ?? ex);
                    }
                }
            };

            if (Interop.IsRunningInTheSimulator)
            {
                // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                if (returnValue)
                {
                    var timeout = TimeSpan.FromSeconds(30);
                    INTERNAL_Simulator.WebControlDispatcherInvoke(action, timeout);
                    if (!actionExecuted)
                    {
                        GenerateDeadlockException(timeout);
                    }
                }
                else
                {
                    INTERNAL_Simulator.WebControlDispatcherBeginInvoke(action);
                }
            }
            else
            {
                action();
            }

            return result;
        }

        private object[] MakeArgumentsForCallback(
            int count,
            int callbackId,
            string idWhereCallbackArgsAreStored,
            JSArray callbackArgs,
            Type[] callbackGenericArgs
            )
        {
            var result = new object[count];

            for (int i = 0; i < count; i++)
            {
                var arg = new INTERNAL_JSObjectReference(callbackArgs, idWhereCallbackArgsAreStored, i);
                if (callbackGenericArgs != null
                    && i < callbackGenericArgs.Length
                    && callbackGenericArgs[i] != typeof(object)
                    && ((bool)INTERNAL_Simulator.SimulatorProxy.TypeIsPrimitive(callbackGenericArgs[i])
                        || callbackGenericArgs[i] == typeof(string)))
                {
                    // Attempt to cast from JS object to the desired primitive or string type.
                    // This is useful for example when passing an Action<string> to an
                    // Interop.ExecuteJavaScript so as to not get an exception that says that
                    // it cannot cast the JS object into string (when running in the Simulator
                    // only)
                    result[i] = INTERNAL_Simulator.SimulatorProxy.ConvertChangeType(arg, callbackGenericArgs[i]);
                }
                else
                {
                    result[i] = arg;
                }
            }
            return result;
        }
#endif
        #endregion CallBack methods
    }
}
