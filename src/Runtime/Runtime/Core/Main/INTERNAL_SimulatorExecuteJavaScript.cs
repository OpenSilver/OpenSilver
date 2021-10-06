

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetForHtml5.Core;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_SimulatorExecuteJavaScript
    {
        internal static bool EnableInteropLogging;
        static List<string> _pendingAsyncJavaScriptToExecute = new List<string>();
        static bool _disableAsyncJavaScriptExecution = false;
        static bool _isDispatcherPending = false;
#if CSHTML5NETSTANDARD
        static bool _isInsideMethodToRunAnActionAndThenExecuteItsPendingJS = false; //todo: make sure this variable is thread-safe.
#else
#endif
        /// <summary>
        /// Executes JavaScript code immediately. This also forces all the pending async JS code to be executed (flush).
        /// </summary>
        /// <param name="javaScriptToExecute">The JS code to execute.</param>
        /// <param name="commentForDebugging">Some optional comments to write to the log of JS calls.</param>
        /// <returns></returns>
#if !BRIDGE
        [JSIgnore]
#endif
        internal static object ExecuteJavaScriptSync(string javaScriptToExecute, string commentForDebugging = null, bool noImpactOnPendingJSCode = false)
        {
            if (!noImpactOnPendingJSCode)
            {
                if (EnableInteropLogging)
                    AddCommentsForDebuggingIfAny(ref javaScriptToExecute, commentForDebugging);

                string aggregatedPendingJavaScriptCode = ReadAndClearAggregatedPendingJavaScriptCode();

                if (!string.IsNullOrWhiteSpace(aggregatedPendingJavaScriptCode))
                {
                    javaScriptToExecute = string.Join(Environment.NewLine, new List<string>
                    {
                        "// [START OF PENDING JAVASCRIPT]",
                        aggregatedPendingJavaScriptCode,
                        "// [END OF PENDING JAVASCRIPT]" + Environment.NewLine,
                        javaScriptToExecute
                    });
                }
            }

            return PerformActualInteropCall(javaScriptToExecute, "SYNC");
        }

        /// <summary>
        /// Postpone the executes of the JavaScript code to until a JavaScript execution "with return value" is made, or when the process is idle. This enables to significantly improve performance in the Simulator.
        /// </summary>
        /// <param name="javaScriptToExecute">The JS code to execute.</param>
        /// <param name="commentForDebugging">Some optional comments to write to the log of JS calls.</param>
#if !BRIDGE
        [JSIgnore]
#endif
        internal static void ExecuteJavaScriptAsync(string javaScriptToExecute, string commentForDebugging = null)
        {
            if (EnableInteropLogging)
                AddCommentsForDebuggingIfAny(ref javaScriptToExecute, commentForDebugging);

            if (!_disableAsyncJavaScriptExecution)
            {
                //--------------------------------------------------------
                // Note: since we moved from the "Awesomium" control to the "DotNetBrowser" control
                // for rending the HTML in the WPF-based Simulator, we have stopped using the
                // "ExecuteJavaScriptAsync" method provided by the browser control, because it
                // causes DotNetBrowser to freeze (cf. the email conversation with DotNetBrowser
                // support in December 2017).
                // Instead, what we do is that we postpone the execution of the JS code that does
                // not return any value (we do that by calling the "Dispatcher.BeginInvoke" method),
                // and aggregate them in order to reduce the number of interop JS calls from C#.
                // This significantly improves performance.
                //--------------------------------------------------------

                lock (_pendingAsyncJavaScriptToExecute)
                {
                    _pendingAsyncJavaScriptToExecute.Add(javaScriptToExecute);
                }

#if !CSHTML5NETSTANDARD
                if (!_isDispatcherPending)
                {
                    _isDispatcherPending = true;

                    INTERNAL_Simulator.WebControlDispatcherBeginInvoke((Action)(() =>
                        {
                            if (_isDispatcherPending) // We check again, because in the meantime the dispatcher can be cancelled in case of a forced execution of the pending JS code, for example when making a JavaScript execution that "returns a value".
                            {
                                ExecutePendingJavaScriptCode("BEGININVOKE COMPLETED");
                            }
                        }));
                }
#else
                if (_isInsideMethodToRunAnActionAndThenExecuteItsPendingJS)
                {
#if OPTIMIZATION_LOG
                    Console.WriteLine("[OPTIMIZATION] Add to pending JS code (" + _pendingAsyncJavaScriptToExecute.Count.ToString() + ")");
#endif
                    //--------------------------------------------------------
                    // Nothing else to do here: the pending JS code will get executed when the thread is free, thanks
                    // to the fact that we always call "EnsurePendingAsyncJavaScriptGetFlushedAfterExecutingThisAction"
                    // before execute any piece of C# code (ie. at all the entry points of C#, such as when the app
                    // starts, and in the callbacks from JavaScript).
                    //--------------------------------------------------------
                }
                else
                {
                    //--------------------------------------------------------
                    // We may arrive here is there is an "async" method that is not awaited and for example that contains
                    // "await Task.Delay(...)", which means that the code will resume without being "wrapped" inside a
                    // call to "RunActionThenExecutePendingAsyncJSCodeExecutedDuringThatAction". In this case we use
                    // the setTimeout method to ensure that the Flush is always executed.
                    //--------------------------------------------------------

#if OPTIMIZATION_LOG
                    Console.WriteLine("[OPTIMIZATION] Direct execution of code. _isDispatcherPending: " + _isDispatcherPending.ToString());
#endif
                    if (!_isDispatcherPending)
                    {
                        _isDispatcherPending = true;

#if OPTIMIZATION_LOG
                        Console.WriteLine("[OPTIMIZATION] Calling setTimeout. _isDispatcherPending: " + _isDispatcherPending.ToString());
#endif
                        string action = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(
                            (Action)(() =>
                            {
#if OPTIMIZATION_LOG
                                    Console.WriteLine("[OPTIMIZATION] Executing setTimeout. _isDispatcherPending: " + _isDispatcherPending.ToString());
#endif
                                if (_isDispatcherPending) // We check again, because in the meantime the dispatcher can be cancelled in case of a forced execution of the pending JS code, for example when making a JavaScript execution that "returns a value".
                                {
                                    ExecutePendingJavaScriptCode("SETTIMEOUT COMPLETED");
                                }
                            })
                        );
                        CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_SimulatorImplementation(
                            javascript: $"setTimeout({action}, 1)",
                            runAsynchronously: false,
                            noImpactOnPendingJSCode: true
                        );
                    }
                }
#endif
                                }
            else
            {
#if OPTIMIZATION_LOG
                Console.WriteLine("[OPTIMIZATION] Direct call");
#endif
                PerformActualInteropCall(javaScriptToExecute, "ASYNC DISABLED");
            }
        }

#if !BRIDGE
        [JSIgnore]
#endif
        static void ExecutePendingJavaScriptCode(string reasonForPerformingTheCallNow)
        {
            string aggregatedPendingJavaScriptCode = ReadAndClearAggregatedPendingJavaScriptCode();

            if (!string.IsNullOrWhiteSpace(aggregatedPendingJavaScriptCode))
            {
                PerformActualInteropCall(aggregatedPendingJavaScriptCode, reasonForPerformingTheCallNow);
            }
        }

#if !BRIDGE
        [JSIgnore]
#endif
        static string ReadAndClearAggregatedPendingJavaScriptCode()
        {
#if OPTIMIZATION_LOG
            Console.WriteLine("[OPTIMIZATION] About to reset _isDispatcherPending: " + _isDispatcherPending.ToString());
#endif
            _isDispatcherPending = false;

#if OPTIMIZATION_LOG
            Console.WriteLine("[OPTIMIZATION] Done resetting _isDispatcherPending: " + _isDispatcherPending.ToString());
#endif
            lock (_pendingAsyncJavaScriptToExecute)
            {
                if (_pendingAsyncJavaScriptToExecute.Count == 0)
                    return null;

                string aggregatedPendingJavaScriptCode = string.Join("\r\n", _pendingAsyncJavaScriptToExecute.ToList());
                _pendingAsyncJavaScriptToExecute.Clear();
                return aggregatedPendingJavaScriptCode;
            }
        }

#if !BRIDGE
        [JSIgnore]
#endif
        static void AddCommentsForDebuggingIfAny(ref string javaScriptToExecute, string commentForDebugging)
        {
            if (commentForDebugging != null)
                javaScriptToExecute = "//" + commentForDebugging + Environment.NewLine + javaScriptToExecute;
        }

#if !BRIDGE
        [JSIgnore]
#endif
        static object PerformActualInteropCall(string javaScriptToExecute, string reasonForPerformingTheCallNow)
        {
            if (EnableInteropLogging)
            {
                javaScriptToExecute = "//---- START INTEROP (" + reasonForPerformingTheCallNow + ") ----"
                    + Environment.NewLine
                    + javaScriptToExecute
                    + Environment.NewLine
                    + "//---- END INTEROP (" + reasonForPerformingTheCallNow + ") ----";
            }

            try
            {
#if CSHTML5BLAZOR
                if (EnableInteropLogging)
                {
                    global::System.Diagnostics.Debug.WriteLine(javaScriptToExecute);
                }

                // OpenSilver Version has two distincts JavaScriptExecutionHandler:
                // - DynamicJavaScriptExecutionHandler is a dynamic typed JavaScriptExecutionHandler setted by the Emulator  
                // - JavaScriptExecutionHandler        is a static typed JavaScriptExecutionHandler used in the browser version
                if (Interop.IsRunningInTheSimulator_WorkAround) // this is the JavaScriptHandler injected by the Emulator
                    return INTERNAL_Simulator.DynamicJavaScriptExecutionHandler.ExecuteJavaScriptWithResult(javaScriptToExecute);
                else
                    return INTERNAL_Simulator.JavaScriptExecutionHandler.ExecuteJavaScriptWithResult(javaScriptToExecute);

#else
                return ((dynamic)INTERNAL_Simulator.JavaScriptExecutionHandler).ExecuteJavaScriptWithResult(javaScriptToExecute);
#endif
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Unable to execute the following JavaScript code: " + Environment.NewLine + javaScriptToExecute, ex);
            }
        }

        public static bool DisableAsyncJavaScriptExecution
        {
            get { return _disableAsyncJavaScriptExecution; }
            set { _disableAsyncJavaScriptExecution = value; }
        }

#if CSHTML5NETSTANDARD
        /// <summary>
        /// Makes sure that, after the provided action has been executed, all the
        /// pending aysnc JavaScript calls that were made during that action will
        /// get flushed (ie. they get executed).
        /// The goal is to greatly improve performance by reducing the number of
        /// C# &lt;-&gt; JS interop calls, by aggregating the async JS calls and
        /// executing them all at once at the end of the current thread execution.
        /// </summary>
        /// <param name="action">The action to execute before flushing the pending JS code.</param>
        public static void RunActionThenExecutePendingAsyncJSCodeExecutedDuringThatAction(Action action)
        {
            try
            { 
                if (_isInsideMethodToRunAnActionAndThenExecuteItsPendingJS)
                {
                    //-----------------------------
                    // This means that we have already planned an auto-flush, so we do
                    // not need to plan it again: we just execute the provided "action".
                    // This is usually the case when we are in a nested call, which
                    // means that this method is being execute from within itself.
                    //-----------------------------

                    action();
                }
                else
                {
                    _isInsideMethodToRunAnActionAndThenExecuteItsPendingJS = true;

                    action();

                    _isInsideMethodToRunAnActionAndThenExecuteItsPendingJS = false;

#if OPTIMIZATION_LOG
                Console.WriteLine("[OPTIMIZATION] Auto-flush");
#endif
                    // After the action has finished execution, let's flush all the pending JavaScript calls if any:
                    ExecutePendingJavaScriptCode("AUTO-FLUSH");
                }
            }
            catch (Exception e)
            {
                Application.Current.OnUnhandledException(e, false);
            } 
        }
#endif
    }
}
