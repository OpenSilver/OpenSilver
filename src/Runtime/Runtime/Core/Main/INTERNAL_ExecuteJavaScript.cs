
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
using System.Threading.Tasks;
using System.Windows.Threading;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_ExecuteJavaScript
    {
        internal static IPendingJavascript JavaScriptRuntime { get; set; }

        internal static bool EnableInteropLogging;
        private static bool _isDispatcherPending = false;

        private static readonly SynchronyzedStore<string> _javascriptCallsStore = new SynchronyzedStore<string>();

        public static object ExecuteJavaScriptWithResult(string javascript, bool flush = true)
            => ExecuteJavaScriptSync(javascript, referenceId: 0, wantsResult: true, flush: flush);

        /// <summary>
        /// Executes JavaScript code immediately. This also forces all the pending async JS code to be executed (flush).
        /// </summary>
        /// <param name="javaScriptToExecute">The JS code to execute.</param>
        /// <param name="referenceId"></param>
        /// <param name="wantsResult"></param>
        /// <param name="commentForDebugging">Some optional comments to write to the log of JS calls.</param>
        /// <param name="flush"></param>
        /// <returns></returns>
        public static object ExecuteJavaScriptSync(string javaScriptToExecute, int referenceId, bool wantsResult, string commentForDebugging = null, bool flush = true)
        {
            if (flush && EnableInteropLogging)
                AddCommentsForDebuggingIfAny(ref javaScriptToExecute, commentForDebugging);

            if (flush)
                JavaScriptRuntime.Flush();

            var result = JavaScriptRuntime.ExecuteJavaScript(javaScriptToExecute, referenceId, wantsResult);
            return result;
        }

        public static Task<object> ExecuteJavaScriptAsync(string javaScriptToExecute, int referenceId, bool wantsResult, string commentForDebugging = null, bool hasImpactOnPendingCode = false)
        {
            return Task.Run(() => ExecuteJavaScriptSync(javaScriptToExecute, referenceId, wantsResult, commentForDebugging, hasImpactOnPendingCode));
        }

        public static string WrapReferenceIdInJavascriptCall(string javascript, int referenceId)
        {
            // Change the JS code to call ShowErrorMessage in case of error:
            string errorCallBackId = _javascriptCallsStore.Add(javascript).ToString();

            javascript = $"document.callScriptSafe(\"{referenceId}\",\"{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(javascript)}\",{errorCallBackId})";
            return javascript;
        }

        public static void QueueExecuteJavaScript(string javascript, int referenceId)
        {
            javascript = WrapReferenceIdInJavascriptCall(javascript, referenceId);
            QueueExecuteJavaScript(javascript);
        }

        /// <summary>
        /// Postpone the executes of the JavaScript code to until a JavaScript execution "with return value" is made, or when the process is idle. This enables to significantly improve performance in the Simulator.
        /// </summary>
        /// <param name="javaScriptToExecute">The JS code to execute.</param>
        /// <param name="commentForDebugging">Some optional comments to write to the log of JS calls.</param>
        internal static void QueueExecuteJavaScript(string javaScriptToExecute, string commentForDebugging = null)
        {
            if (EnableInteropLogging)
                AddCommentsForDebuggingIfAny(ref javaScriptToExecute, commentForDebugging);

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

#if OPTIMIZATION_LOG
            Console.WriteLine("[OPTIMIZATION] Direct execution of code. _isDispatcherPending: " + _isDispatcherPending.ToString());
#endif
            if (!_isDispatcherPending)
            {
                _isDispatcherPending = true;

#if OPTIMIZATION_LOG
                Console.WriteLine("[OPTIMIZATION] Calling setTimeout. _isDispatcherPending: " + _isDispatcherPending.ToString());
#endif
                INTERNAL_DispatcherHelpers.QueueAction(ExecutePendingJavaScriptCode);
            }
        }

        public static void ExecutePendingJavaScriptCode()
        {
            _isDispatcherPending = false;
            JavaScriptRuntime.Flush();
        }

        private static void AddCommentsForDebuggingIfAny(ref string javaScriptToExecute, string commentForDebugging)
        {
            if (commentForDebugging != null)
                javaScriptToExecute = string.Concat("//", commentForDebugging, Environment.NewLine, javaScriptToExecute);
        }

        // FIXME update after PR724 is approved
        internal static void ShowErrorMessage(string errorMessage, int indexOfCallInList)
        {
            string str = _javascriptCallsStore.Get(indexOfCallInList);

#if OPENSILVER
            if (INTERNAL_InteropImplementation.IsRunningInTheSimulator_WorkAround())
#else
            if (INTERNAL_InteropImplementation.IsRunningInTheSimulator())
#endif
            {
                DotNetForHtml5.Core.INTERNAL_Simulator.SimulatorProxy.ReportJavaScriptError(errorMessage, str);
            }
            else
            {
                string message = string.Format(@"Error in the following javascript code:

{0}

----- Error: -----

{1}
", str, errorMessage);
                Console.WriteLine(message);
            }
        }

    }
}