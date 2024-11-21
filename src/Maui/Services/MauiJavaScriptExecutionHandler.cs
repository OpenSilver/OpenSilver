namespace OpenSilver.Maui;

public partial class WebViewManagerService
{
    public class MauiJavaScriptExecutionHandler : DotNetForHtml5.IJavaScriptExecutionHandler
    {
        private readonly Func<Action, bool> _dispatcherAction;
        private readonly Func<Func<Task>, Task> _dispatcherAsyncAction;
        private readonly Func<bool> _dispatcherCheckAccessAction;
        private readonly Func<string, Task<string>> _executeScriptAsyncAction;

        public MauiJavaScriptExecutionHandler(
            Func<string, Task<string>> executeScriptAsyncAction,
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

        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            return ExecuteJavascriptAsync(javaScriptToExecute).GetAwaiter().GetResult();
        }

        private Task<string> ExecuteJavascriptAsync(string javaScriptToExecute)
        {
            var tcs = new TaskCompletionSource<string>();

            _dispatcherAction(async () =>
            {
                try
                {
                    var rawResult = await _executeScriptAsyncAction(javaScriptToExecute);

                    if (rawResult != null) rawResult = rawResult.Trim('"');

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