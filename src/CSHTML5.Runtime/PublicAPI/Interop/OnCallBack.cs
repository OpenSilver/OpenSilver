
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using CSHTML5;
using DotNetForHtml5.Core;
using CSHTML5.Types;
using System;
using System.Collections.Generic;


#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if BRIDGE
using Bridge;
#else
using JSIL.Meta;
#endif

#if !BUILDINGDOCUMENTATION && !CSHTML5NETSTANDARD
using DotNetBrowser;
#endif

#if !BUILDINGDOCUMENTATION
namespace CSHTML5.Internal
{
#if BRIDGE
    [External] //we exclude this class
#else
    [JSIgnore]
#endif
#if CSHTML5NETSTANDARD
    static
#endif
    public class OnCallBack
    {
#if CSHTML5NETSTANDARD
        static
#endif
        private Dictionary<int, Delegate> _dictionary;

#if CSHTML5NETSTANDARD
        public static void SetCallbacksDictionary(Dictionary<int, Delegate> dictionary)
#else
        //constructor
        public OnCallBack(Dictionary<int, Delegate> dictionary)
#endif
        {
            _dictionary = dictionary;
        }

#region CallBack methods

#if !CSHTML5NETSTANDARD
        public void UpdateDictionary(Dictionary<int, Delegate> newDictionary)
        {
            _dictionary = newDictionary;
        }
#endif

        //---------------------------------------------------------------------------------------
        // This code follows the architecture drawn by DotNetBrowser (cf https://dotnetbrowser.support.teamdev.com/support/solutions/articles/9000109868-calling-javascript-from-net)
        // For an example of implementation, go to INTERNAL_InteropImplementation.cs & ExecuteJavaScript_SimulatorImplementation method, in the first "if".
        //---------------------------------------------------------------------------------------
#if CSHTML5NETSTANDARD
        static
#endif
        public void OnCallbackFromJavaScript(
#if CSHTML5NETSTANDARD
            string callbackIdString,
#else
            int callbackId,
#endif
            string idWhereCallbackArgsAreStored,
#if CSHTML5NETSTANDARD
            object[] callbackArgsString
#else
            JSArray callbackArgsObject
#endif
            )
        {
#if CSHTML5NETSTANDARD
            //Console.WriteLine("ID string: " + callbackIdString);
            int callbackId = int.Parse(callbackIdString);
            object[] callbackArgsObject = new object[0];
#endif
            Action action = () =>
            {
                //----------------------------------
                // Get the C# callback from its ID:
                //----------------------------------
//#if CSHTML5NETSTANDARD
//                Console.WriteLine("CallbackID: " + callbackId.ToString());
//                Console.WriteLine("idWhereCallbackArgsAreStored: " + idWhereCallbackArgsAreStored.ToString());
//                Console.WriteLine("Dictionary.Count: " + _dictionary.Count.ToString());
//#endif
                Delegate callback = _dictionary[callbackId];
                Type callbackType = callback.GetType();
                Type[] callbackGenericArgs = null;
                if (callbackType.IsGenericType)
                {
                    callbackGenericArgs = callbackType.GetGenericArguments();
                    callbackType = callbackType.GetGenericTypeDefinition();
                }
                var callbackArgs = callbackArgsObject;

                //--------------------
                // Call the callback:
                //--------------------
#if CATCH_INTEROP_EXCEPTIONS // Note: previously, when using the "Awesomium" browser control in the Simulator, we handled the exceptions because otherwise they got captured by the WebBrowser control, due to the fact that the callback is executed in the context of the WebBrowser control. This is no longer true with the "DotNetBrowser" control.
                try
                {
#endif
                try
                {
                    if (callbackType == typeof(Action))
                    {
                        DelegateDynamicInvoke(callback);
                    }
                    else if (callbackType == typeof(Action<>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(1, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else if (callbackType == typeof(Action<,>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(2, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else if (callbackType == typeof(Action<,,>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(3, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else if (callbackType == typeof(Action<,,,>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(4, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else if (callbackType == typeof(Action<,,,,>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(5, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else if (callbackType == typeof(Action<,,,,,>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(6, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else if (callbackType == typeof(Action<,,,,,,>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(7, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else if (callbackType == typeof(Action<,,,,,,,>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(8, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else if (callbackType == typeof(Action<,,,,,,,,>))
                    {
                        DelegateDynamicInvoke(callback, MakeArgumentsForCallback(9, callbackId, idWhereCallbackArgsAreStored, callbackArgs, callbackGenericArgs));
                    }
                    else
                        throw new Exception(string.Format("Callback type not supported: {0}  Please report this issue to support@cshtml5.com", callbackType.ToString()));

                    //if (callbackType.IsSubclassOf(typeof(MulticastDelegate)) &&
                    //    callbackType.GetMethod("Invoke").ReturnType == typeof(void))
                    //{
                    //    DelegateDynamicInvoke(callback);
                    //}
                }
                catch(global::System.Reflection.TargetInvocationException ex)
                {
#if CSHTML5NETSTANDARD
                    throw;
#else
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
#endif
                }
#if CATCH_INTEROP_EXCEPTIONS // Note: previously, when using the "Awesomium" browser control in the Simulator, we handled the exceptions because otherwise they got captured by the WebBrowser control, due to the fact that the callback is executed in the context of the WebBrowser control. This is no longer true with the "DotNetBrowser" control.
                }
                catch (Exception ex) // We catch and handle the Exception because otherwise it gets captured by the WebBrowser control, due to the fact that the callback is executed in the context of the WebBrowser control.
                {
                    string message = ex.Message;

                    if (Interop.IsRunningInTheSimulator && ex.InnerException != null) // Note: "InnerException" is only supported in the Simulator as of July 27, 2017.
                    {
                        message += Environment.NewLine + Environment.NewLine + ex.InnerException.Message
                            + Environment.NewLine + Environment.NewLine + ex.InnerException.StackTrace;
                    }
                    else
                        message += Environment.NewLine + Environment.NewLine + ex.StackTrace;


                    if (Interop.IsRunningInTheSimulator)
                    {
                        // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                        INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                MessageBox.Show(message);
                            }));
                    }
                    else
                    {
                        MessageBox.Show(message);
                    }
                }
#endif
            };

            if (Interop.IsRunningInTheSimulator)
            {
#if !CSHTML5NETSTANDARD
                // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke(action);
#else
                // Run the action and then flush any pending async JavaScript code that was executed during that action:
                INTERNAL_SimulatorExecuteJavaScript.RunActionThenExecutePendingAsyncJSCodeExecutedDuringThatAction(
                    action
                    );
#endif
            }
            else
            {
                action();
            }
        }

#if CSHTML5NETSTANDARD
        static
#endif
        private object DelegateDynamicInvoke(Delegate d, params object[] args)
        {
#if BRIDGE
            return INTERNAL_Simulator.SimulatorProxy.DelegateDynamicInvoke(d, args);
#else
            return d.DynamicInvoke(args);
#endif
        }

        //[JSReplacement("null")]
#if CSHTML5NETSTANDARD
    static
#endif
        private object[] MakeArgumentsForCallback(
            int count,
            int callbackId,
            string idWhereCallbackArgsAreStored,
#if CSHTML5NETSTANDARD
            object[]
#else
            JSArray
#endif
            callbackArgs,
            Type[] callbackGenericArgs
            )
        {
            var result = new object[count];

            for (int i = 0; i < count; i++)
            {
                var arg = new INTERNAL_JSObjectReference()
                {
                    ReferenceId = idWhereCallbackArgsAreStored,
                    IsArray = true,
                    ArrayIndex = i,
                    Value = callbackArgs
                };
                if (callbackGenericArgs != null
                    && i < callbackGenericArgs.Length
                    && callbackGenericArgs[i] != typeof(object)
                    && (
#if BRIDGE
                    (bool)INTERNAL_Simulator.SimulatorProxy.TypeIsPrimitive(callbackGenericArgs[i])
#else
                    callbackGenericArgs[i].IsPrimitive
#endif
                    || callbackGenericArgs[i] == typeof(string)))
                {
                    // Attempt to cast from JS object to the desired primitive or string type. This is useful for example when passing an Action<string> to an Interop.ExecuteJavaScript so as to not get an exception that says that it cannot cast the JS object into string (when running in the Simulator only):
#if BRIDGE
                    result[i] = INTERNAL_Simulator.SimulatorProxy.ConvertChangeType(arg, callbackGenericArgs[i]);
#else
                    result[i] = Convert.ChangeType(arg, callbackGenericArgs[i]);
#endif
                }
                else
                {
                    result[i] = arg;
                }
            }
            return result;
        }
#endregion
    }
}
#endif
