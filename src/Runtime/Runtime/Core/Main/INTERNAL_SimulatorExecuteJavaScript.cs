
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

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_SimulatorExecuteJavaScript
    {
        internal static IPendingJavascript JavaScriptRuntime { get; set; }

        internal static bool EnableInteropLogging;
        static bool _isDispatcherPending = false;
        static bool _isInsideMethodToRunAnActionAndThenExecuteItsPendingJS = false; //todo: make sure this variable is thread-safe.

        /// <summary>
        /// Executes JavaScript code immediately. This also forces all the pending async JS code to be executed (flush).
        /// </summary>
        /// <param name="javaScriptToExecute">The JS code to execute.</param>
        /// <param name="commentForDebugging">Some optional comments to write to the log of JS calls.</param>
        /// <param name="noImpactOnPendingJSCode">true to ignore pending javascript code from asynchronous interops</param>
        /// <returns></returns>
        internal static object ExecuteJavaScriptSync(string javaScriptToExecute, string commentForDebugging = null, bool noImpactOnPendingJSCode = false)
        {
            if (!noImpactOnPendingJSCode && EnableInteropLogging)
            {
                AddCommentsForDebuggingIfAny(ref javaScriptToExecute, commentForDebugging);
            }

            return JavaScriptRuntime.ExecuteJavaScript(javaScriptToExecute, !noImpactOnPendingJSCode);
        }

        /// <summary>
        /// Postpone the executes of the JavaScript code to until a JavaScript execution "with return value" is made, or when the process is idle. This enables to significantly improve performance in the Simulator.
        /// </summary>
        /// <param name="javaScriptToExecute">The JS code to execute.</param>
        /// <param name="commentForDebugging">Some optional comments to write to the log of JS calls.</param>
        internal static void ExecuteJavaScriptAsync(string javaScriptToExecute, string commentForDebugging = null)
        {
            if (EnableInteropLogging)
                AddCommentsForDebuggingIfAny(ref javaScriptToExecute, commentForDebugging);

            if (!DisableAsyncJavaScriptExecution)
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

                JavaScriptRuntime.AddJavaScript(javaScriptToExecute);

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

                        string action = INTERNAL_InteropImplementation.GetVariableStringForJS(
                            JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback(() =>
                            {
#if OPTIMIZATION_LOG
                                Console.WriteLine("[OPTIMIZATION] Executing setTimeout. _isDispatcherPending: " + _isDispatcherPending.ToString());
#endif
                                if (_isDispatcherPending)
                                {
                                    ExecutePendingJavaScriptCode();
                                }
                            }));

                        ExecuteJavaScriptSync($"setTimeout({action}, 1)", null, true);
                    }
                }
            }
            else
            {
                JavaScriptRuntime.ExecuteJavaScript(javaScriptToExecute, false);
#if OPTIMIZATION_LOG
                Console.WriteLine("[OPTIMIZATION] Direct call");
#endif
            }
        }

        static void ExecutePendingJavaScriptCode()
        {
            _isDispatcherPending = false;
            JavaScriptRuntime.ExecuteJavaScript(null, true);
        }

        static void AddCommentsForDebuggingIfAny(ref string javaScriptToExecute, string commentForDebugging)
        {
            if (commentForDebugging != null)
                javaScriptToExecute = string.Concat("//", commentForDebugging, Environment.NewLine, javaScriptToExecute);
        }

        public static bool DisableAsyncJavaScriptExecution { get; set; }

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
                    ExecutePendingJavaScriptCode();
                }
            }
            catch (Exception e)
            {
                Application.Current.OnUnhandledException(e, false);
            }
        }
    }
}