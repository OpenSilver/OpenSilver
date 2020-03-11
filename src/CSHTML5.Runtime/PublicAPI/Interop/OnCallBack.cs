

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

#if !BUILDINGDOCUMENTATION
using DotNetBrowser;
#endif

#if CSHTML5NETSTANDARD
using System.Reflection;
using System.Linq;
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
    // used to debug Browser Version
    // todo: remove when fixed
    public static class MethodInfoExtension
    {
        public static string MethodSignature(this MethodInfo mi)
        {
            String[] param = mi.GetParameters()
                          .Select(p => 
                          {
                              string paramType;
                              try
                              {
                                  paramType = p.ParameterType.Name;
                              }
                              catch
                              {
                                  paramType = "UnknownType";
                              }
                              string name;
                              try
                              {
                                  name = p.Name;
                              }
                              catch
                              {
                                  name = "unknown";
                              }
                              return String.Format("{0} {1}", p.ParameterType.Name, name);
                          })
                          .ToArray();

            string returnType;
            try
            {
                returnType = mi.ReturnType.Name;
            }
            catch
            {
                returnType = "UnknownType";
            }

            string methodName;
            try
            {
                methodName = mi.Name;
            }
            catch
            {
                methodName = "Unknown";
            }

            string signature = String.Format("{0} {1}({2})", returnType, methodName, String.Join(",", param));

            return signature;
        }
    }
#endif


    // In Previous Net Standard Version OnCallback was a static class but because OpenSilver has too match both simulator and browser API
    // The Class may be instantiated and cannot be static anymore
    public class OnCallBack
    {
        static private Dictionary<int, Delegate> _dictionary;

        public static void SetCallbacksDictionary(Dictionary<int, Delegate> dictionary)
        {
            _dictionary = dictionary;
        }

        //constructor
        public OnCallBack(Dictionary<int, Delegate> dictionary)
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

        // In The OpenSilver Version, there are 2 options for callback:
        // - Browser   Version: the callback is called from js but using the Microsoft Interop. It uses an object[] 
        // - Simulator Version: the callback is called from js but using the DotNetBrowser Interop. It uses a  JSArray  

        static public void OnCallbackFromJavaScript(
            string callbackIdString,
            string idWhereCallbackArgsAreStored,
            object[] callbackArgsString
            )
        {
            //Console.WriteLine("ID string: " + callbackIdString);
            int callbackId = int.Parse(callbackIdString);
            object[] callbackArgsObject = new object[0];
            Action action = () =>
            {
                //----------------------------------
                // Get the C# callback from its ID:
                //----------------------------------
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
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.Error.WriteLine("DEBUG: OnCallBack: OnCallBackFromJavascript: " + ex);
#endif
                    throw;
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
                // Run the action and then flush any pending async JavaScript code that was executed during that action:
                INTERNAL_SimulatorExecuteJavaScript.RunActionThenExecutePendingAsyncJSCodeExecutedDuringThatAction(
                    action
                    );
            }
            else
            {
                action();
            }
        }

        static private object[] MakeArgumentsForCallback(
            int count,
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object[]
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

        static public void OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            JSArray callbackArgsObject
            )
        {
            Action action = () =>
            {
                //----------------------------------
                // Get the C# callback from its ID:
                //----------------------------------
                Delegate callback = _dictionary[callbackId];
                //#if CSHTML5NETSTANDARD
                //                try
                //                {
                //                    Console.WriteLine(
                //                        "CallbackID: " + callbackId.ToString() + "\n" +
                //                        "Callback Method Name: " + callback.Method.Name + "\n" +
                //                        "Callback Method Body: " + callback.Method.GetMethodBody().ToString() + "\n" +
                //                        "Callback Target: " + callback.Target.ToString() + "\n" +
                //                        "idWhereCallbackArgsAreStored: " + idWhereCallbackArgsAreStored.ToString() + "\n" +
                //                        "Dictionary.Count: " + _dictionary.Count.ToString());
                //                }
                //                catch (Exception e)
                //                {

                //                }
                //#endif

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
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.Error.WriteLine("DEBUG: OnCallBack: OnCallBackFromJavascript: " + ex);
#endif
                    throw;
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

            if (Interop.IsRunningInTheSimulator_WorkAround)
            {
                // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        static private object[] MakeArgumentsForCallback(
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

        static private object DelegateDynamicInvoke(Delegate d, params object[] args)
        {
            object obj = null;
            string methodSingature = "Unknown Method";
            try
            {
#if DEBUG
                try
                {
                    methodSingature = d.Method.MethodSignature();
                    Console.WriteLine("DEBUG: OnCallBack: DelegateDynamicnvoke: " + methodSingature);
                }
                catch (Exception e)
                {
                    Console.WriteLine("DEBUG: OnCallBack: DelegateDynamicnvoke: ?????????");
                    // we failed to display signature of the method
                }
#endif
                if (args != null)
                    obj = d.DynamicInvoke(args);
            }
            catch (Exception e)
            {
#if DEBUG
                Console.Error.WriteLine("DEBUG: OnCallBack: DelegateDynamicnvoke: failed to invoke: " + methodSingature + ": " + e);
#endif
                //throw;
            }
            return obj;
        }
#else

        public void OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            JSArray callbackArgsObject
            )
        {
            Action action = () =>
            {
                //----------------------------------
                // Get the C# callback from its ID:
                //----------------------------------
                Delegate callback = _dictionary[callbackId];
//#if CSHTML5NETSTANDARD
//                try
//                {
//                    Console.WriteLine(
//                        "CallbackID: " + callbackId.ToString() + "\n" +
//                        "Callback Method Name: " + callback.Method.Name + "\n" +
//                        "Callback Method Body: " + callback.Method.GetMethodBody().ToString() + "\n" +
//                        "Callback Target: " + callback.Target.ToString() + "\n" +
//                        "idWhereCallbackArgsAreStored: " + idWhereCallbackArgsAreStored.ToString() + "\n" +
//                        "Dictionary.Count: " + _dictionary.Count.ToString());
//                }
//                catch (Exception e)
//                {

//                }
//#endif

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
                // Go back to the UI thread because DotNetBrowser calls the callback from the socket background thread:
                INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        private object DelegateDynamicInvoke(Delegate d, params object[] args)
        {
#if BRIDGE
            return INTERNAL_Simulator.SimulatorProxy.DelegateDynamicInvoke(d, args);
#else
            return d.DynamicInvoke(args);
#endif
            }

        //[JSReplacement("null")]
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
#endif
#endregion
    }
}
#endif
