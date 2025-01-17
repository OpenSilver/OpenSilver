
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

namespace OpenSilver.MauiHybrid.JavaScript
{
    internal class MauiJavaScriptExecutionHandler : DotNetForHtml5.IJavaScriptExecutionHandler
    {
        private readonly Func<Action, bool> _dispatcherAction;
        private readonly Func<Func<Task>, Task> _dispatcherAsyncAction;
        private readonly Func<bool> _dispatcherCheckAccessAction;
        private readonly Func<string, Task<object?>> _executeScriptAsyncAction;

        public MauiJavaScriptExecutionHandler(
            Func<string, Task<object?>> executeScriptAsyncAction,
            Func<Action, bool> dispatcherAction,
            Func<Func<Task>, Task> dispatcherAsyncAction,
            Func<bool> dispatcherCheckAccessAction)
        {
            _executeScriptAsyncAction = executeScriptAsyncAction;
            _dispatcherAction = dispatcherAction;
            _dispatcherAsyncAction = dispatcherAsyncAction;
            _dispatcherCheckAccessAction = dispatcherCheckAccessAction;
        }

        public async void ExecuteJavaScript(string javaScriptToExecute)
        {
            if (_dispatcherCheckAccessAction())
            {
                await _executeScriptAsyncAction(javaScriptToExecute);
            }
            else
            {
                await _dispatcherAsyncAction(() => _executeScriptAsyncAction(javaScriptToExecute));
            }
        }

        public object? ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            return ExecuteJavascriptAsync(javaScriptToExecute).GetAwaiter().GetResult();
        }

        private Task<object?> ExecuteJavascriptAsync(string javaScriptToExecute)
        {
            var tcs = new TaskCompletionSource<object?>();

            _dispatcherAction(async () =>
            {
                try
                {
                    var rawResult = await _executeScriptAsyncAction(javaScriptToExecute);

                    tcs.SetResult(rawResult);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    }
}
