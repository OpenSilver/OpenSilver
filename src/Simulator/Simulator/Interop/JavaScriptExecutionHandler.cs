
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

extern alias opensilver;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Windows.Threading;
using IJavaScriptExecutionHandler = opensilver::DotNetForHtml5.IJavaScriptExecutionHandler;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public class JavaScriptExecutionHandler : IJavaScriptExecutionHandler
    {
        private bool _webControlDisposed = false;
        private readonly WebView2 _webControl;
        private readonly List<string> _fullLogOfExecutedJavaScriptCode = new();

        public JavaScriptExecutionHandler(WebView2 webControl)
        {
            _webControl = webControl;
        }

        public void MarkWebControlAsDisposed()
        {
            _webControlDisposed = true;
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public async void ExecuteJavaScript(string javaScriptToExecute)
        {
            // This prevents interop calls from throwing an exception if they are called after the simulator started closing
            if (_webControlDisposed || javaScriptToExecute == null)
                return;
            if (OpenSilver.Simulator.SimulatorLauncher.Parameters.LogExecutedJavaScriptCode)
                _fullLogOfExecutedJavaScriptCode.Add(javaScriptToExecute);

            if (_webControl.Dispatcher.CheckAccess())
            {
                await _webControl.CoreWebView2.ExecuteScriptAsync(javaScriptToExecute);
            }
            else
            {
                await _webControl.Dispatcher.InvokeAsync(async () =>
                {
                    await _webControl.CoreWebView2.ExecuteScriptAsync(javaScriptToExecute);
                });
            }
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            // This prevents interop calls from throwing an exception if they are called after the simulator started closing
            if (_webControlDisposed || javaScriptToExecute == null)
                return null;

            if (OpenSilver.Simulator.SimulatorLauncher.Parameters.LogExecutedJavaScriptCode)
                _fullLogOfExecutedJavaScriptCode.Add(javaScriptToExecute);

            string execScriptTaskResultAsString = null;
            string execScriptTaskResultAsJson = null;
            int result = 0;

            if (_webControl.Dispatcher.CheckAccess())
            {
                Task<CoreWebView2ExecuteScriptResult> execScriptTask = _webControl.CoreWebView2.ExecuteScriptWithResultAsync(javaScriptToExecute);
                WaitTaskCompleted(execScriptTask);
                var execScriptTaskResult = execScriptTask.Result;

                try
                {
                    execScriptTaskResult.TryGetResultAsString(out execScriptTaskResultAsString, out result);
                }
                catch (Exception) { }

                execScriptTaskResultAsJson = execScriptTaskResult.ResultAsJson;
            }
            else
            {
                Task execScriptTask = _webControl.Dispatcher.InvokeAsync(async () =>
                {
                    var execScriptTaskResult = await _webControl.CoreWebView2.ExecuteScriptWithResultAsync(javaScriptToExecute);

                    try
                    {
                        execScriptTaskResult.TryGetResultAsString(out execScriptTaskResultAsString, out result);
                    }
                    catch (Exception) { }

                    execScriptTaskResultAsJson = execScriptTaskResult.ResultAsJson;

                }).Result;

                execScriptTask.GetAwaiter().GetResult();
            }

            if (result != 0)
            {
                return execScriptTaskResultAsString;
            }

            return execScriptTaskResultAsJson;
        }

        private static void WaitTaskCompleted(Task execScriptTask)
        {
            SpinWait.SpinUntil(() =>
            {
                bool taskCompleted = execScriptTask.IsCompleted;
                if (!taskCompleted)
                {
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, static () => { });
                }
                return taskCompleted;
            });
        }

        public string FullLogOfExecutedJavaScriptCode
        {
            get
            {
                return
@"window.onCallBack = {
    OnCallbackFromJavaScriptWithResult: function(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject)
    {
        // dummy function
    },
    OnCallbackFromJavaScript: function(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject)
    {
        // dummy function
    },
    OnCallbackFromJavaScriptError: function(idWhereCallbackArgsAreStored)
    {
        // dummy function
    }
};
"
                + string.Join("\n\n", _fullLogOfExecutedJavaScriptCode);
            }
        }
    }
}
