
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
using System.Collections.Generic;
using System.Linq;
using CSHTML5.Types;
using DotNetForHtml5.Core;

namespace CSHTML5.Internal
{
    internal sealed class OnCallBackImpl
    {
        private OnCallBackImpl()
        {
        }

        public static OnCallBackImpl Instance { get; } = new OnCallBackImpl();

        public void OnCallbackFromJavaScriptError(string idWhereCallbackArgsAreStored)
        {
            Action action = () =>
            {
                string errorMessage = OpenSilver.Interop.ExecuteJavaScriptString($"document.jsObjRef['{idWhereCallbackArgsAreStored}'][0]");
                int indexOfNextUnmodifiedJSCallInList = OpenSilver.Interop.ExecuteJavaScriptInt32($"document.jsObjRef['{idWhereCallbackArgsAreStored}'][1]");
                ShowErrorMessage(errorMessage, indexOfNextUnmodifiedJSCallInList);
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

        private static void ShowErrorMessage(string errorMessage, int indexOfCallInList)
        {
            string javascript = OpenSilver.Interop.GetJavaScript(indexOfCallInList);

            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                INTERNAL_Simulator.SimulatorProxy.ReportJavaScriptError(errorMessage, javascript);
            }
            else
            {
                string message = string.Format(@"Error in the following javascript code:

{0}

----- Error: -----

{1}
", javascript, errorMessage);
                Console.WriteLine(message);
            }
        }

        //---------------------------------------------------------------------------------------
        // This code follows the architecture drawn by DotNetBrowser
        // (cf https://dotnetbrowser.support.teamdev.com/support/solutions/articles/9000109868-calling-javascript-from-net)
        // For an example of implementation, go to INTERNAL_InteropImplementation.cs and
        // ExecuteJavaScript_Implementation method, in the first "if".
        //---------------------------------------------------------------------------------------

        public object OnCallbackFromJavaScript<T>(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            T callbackArgsObject)
        {
            return CallMethod(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject);
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

                case Func<object> func:
                    result = func();
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
            JavaScriptCallback jsCallback = JavaScriptCallback.Get(callbackId);
            if (jsCallback == null)
            {
                return null;
            }

            Delegate callback = jsCallback.GetCallback();

            if (TryOptimizationForCommonTypes(callback, out object simpleResult))
            {
                return simpleResult;
            }

            Type callbackType = callback.GetType();
            Type[] callbackGenericArgs = null;
            if (callbackType.IsGenericType)
            {
                callbackGenericArgs = callbackType.GetGenericArguments();
                callbackType = callbackType.GetGenericTypeDefinition();
            }

            object[] arguments = null;
            IReadOnlyList<IDisposable> extraJsObjects = null;
            try
            {
                int argumentCount = 0;
                if (callbackType == typeof(Action<>) || callbackType == typeof(Func<,>))
                    argumentCount = 1;
                else if (callbackType == typeof(Action<,>) || callbackType == typeof(Func<,,>))
                    argumentCount = 2;
                else if (callbackType == typeof(Action<,,>) || callbackType == typeof(Func<,,,>))
                    argumentCount = 3;
                else if (callbackType == typeof(Action<,,,>) || callbackType == typeof(Func<,,,,>))
                    argumentCount = 4;
                else if (callbackType == typeof(Action<,,,,>) || callbackType == typeof(Func<,,,,,>))
                    argumentCount = 5;
                else if (callbackType == typeof(Action<,,,,,>) || callbackType == typeof(Func<,,,,,,>))
                    argumentCount = 6;
                else if (callbackType == typeof(Action<,,,,,,>) || callbackType == typeof(Func<,,,,,,,>))
                    argumentCount = 7;
                else if (callbackType == typeof(Action<,,,,,,,>) || callbackType == typeof(Func<,,,,,,,,>))
                    argumentCount = 8;
                else if (callbackType == typeof(Action<,,,,,,,,>) || callbackType == typeof(Func<,,,,,,,,,>))
                    argumentCount = 9;

                (arguments, extraJsObjects) = MakeArgumentsForCallback(argumentCount, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs);
                return DelegateDynamicInvoke(callback, arguments);
            }
            finally
            {
                if (arguments != null)
                {
                    foreach (var arg in arguments.OfType<IDisposable>())
                    {
                        arg.Dispose();
                    }
                }
                if (extraJsObjects != null)
                {
                    foreach (IDisposable arg in extraJsObjects)
                    {
                        arg.Dispose();
                    }
                }
            }

            throw new Exception($"Callback type not supported: '{callbackType.FullName}'");
        }

        private static (object[], IReadOnlyList<IDisposable>) MakeArgumentsForCallback(
            int count,
            string idWhereCallbackArgsAreStored,
            object callbackArgs,
            Type[] callbackGenericArgs)
        {
            var result = new object[count];
            List<IDisposable> extraJsObjects = null;

            for (int i = 0; i < count; i++)
            {
                var arg = new JSObjectRef(callbackArgs, idWhereCallbackArgsAreStored, i);

                if (callbackGenericArgs != null
                    && i < callbackGenericArgs.Length
                    && callbackGenericArgs[i] != typeof(object)
                    && (callbackGenericArgs[i].IsPrimitive || callbackGenericArgs[i] == typeof(string)))
                {
                    // Attempt to cast from JS object to the desired primitive or string type. This is useful for example
                    // when passing an Action<string> to an Interop.ExecuteJavaScript so as to not get an exception that says
                    // that it cannot cast the JS object into string (when running in the Simulator only):
                    result[i] = Convert.ChangeType(arg, callbackGenericArgs[i]);
                    extraJsObjects ??= new();
                    extraJsObjects.Add(arg);
                }
                else
                {
                    result[i] = arg;
                }
            }
            return (result, extraJsObjects);
        }
    }
}
