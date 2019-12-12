
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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

namespace CSHTML5.Internal
{
    internal static class INTERNAL_SimulatorExecuteJavaScript
    {
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
                AddCommentsForDebuggingIfAny(ref javaScriptToExecute, commentForDebugging);

                string aggregatedPendingJavaScriptCode = ReadAndClearAggregatedPendingJavaScriptCode();

                if (!string.IsNullOrWhiteSpace(aggregatedPendingJavaScriptCode))
                {
                    javaScriptToExecute = "// [START OF PENDING JAVASCRIPT]"
                        + Environment.NewLine
                        + aggregatedPendingJavaScriptCode
                        + Environment.NewLine
                        + "// [END OF PENDING JAVASCRIPT]"
                        + Environment.NewLine
                        + Environment.NewLine
                        + javaScriptToExecute;
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

                _pendingAsyncJavaScriptToExecute.Add(javaScriptToExecute);

#if !CSHTML5NETSTANDARD
                if (!_isDispatcherPending)
                {
                    _isDispatcherPending = true;

                    INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke((Action)(() =>
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
                        CSHTML5.INTERNAL_InteropImplementation.ExecuteJavaScript_SimulatorImplementation(
                            javascript: "setTimeout($0, 1)",
                            runAsynchronously: false,
                            noImpactOnPendingJSCode: true,
                            variables: new object[]
                            {
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
                            }
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
            string aggregatedPendingJavaScriptCode = string.Join("\r\n", _pendingAsyncJavaScriptToExecute);

            _pendingAsyncJavaScriptToExecute.Clear();

            return aggregatedPendingJavaScriptCode;
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
            javaScriptToExecute = "//---- START INTEROP (" + reasonForPerformingTheCallNow + ") ----"
                + Environment.NewLine
                + javaScriptToExecute
                + Environment.NewLine
                + "//---- END INTEROP (" + reasonForPerformingTheCallNow + ") ----";
            try
            {
#if CSHTML5BLAZOR
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
        /// C# <-> JS interop calls, by aggregating the async JS calls and
        /// executing them all at once at the end of the current thread execution.
        /// </summary>
        /// <param name="action">The action to execute before flushing the pending JS code.</param>
        public static void RunActionThenExecutePendingAsyncJSCodeExecutedDuringThatAction(Action action)
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
#endif
            }
        }
