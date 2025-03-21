
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

namespace OpenSilver.Photino.JavaScript
{
    internal sealed class PhotinoExecutionHandler : DotNetForHtml5.IJavaScriptExecutionHandler
    {
        private readonly int _mainThreadId;

        private readonly Action<Action> _dispatcherAction;
        private readonly Func<string, Task<object?>> _executeScriptAsyncAction;

        public PhotinoExecutionHandler(
            Func<string, Task<object?>> executeScriptAsyncAction,
            Action<Action> dispatcherAction)
        {
            _mainThreadId = Environment.CurrentManagedThreadId;

            _executeScriptAsyncAction = executeScriptAsyncAction;
            _dispatcherAction = dispatcherAction;
        }

        private bool DispatcherCheckAccess()
        {
            return Environment.CurrentManagedThreadId == _mainThreadId;
        }

        public void ExecuteJavaScript(string javaScriptToExecute)
        {
            if (DispatcherCheckAccess())
            {
                _executeScriptAsyncAction(javaScriptToExecute);
            }
            else
            {
                _dispatcherAction(() => _executeScriptAsyncAction(javaScriptToExecute));
            }
        }

        public object? ExecuteJavaScriptWithResult(string javaScriptToExecute)
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
            return tcs.Task.GetAwaiter().GetResult();
        }
    }
}
