using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSHTML5;
using CSHTML5.Internal;

namespace Runtime.OpenSilver.PublicAPI.Interop
{
    public class BulkExecuteJavascriptAsync
    {
        private List<IDisposable> _disposables = new List<IDisposable>();
        private StringBuilder _javascript = new StringBuilder();

        public BulkExecuteJavascriptAsync() {

        }

        public IDisposable AddJavascriptAsync(string javascript, params object[] variables) {
            javascript = INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, variables);
            return AddJavascriptAsync(javascript);
        }

        public IDisposable AddJavascriptAsync(string javascript) {
            return AddDisposable(global::OpenSilver.Interop.ExecuteJavaScriptAsync(javascript));
        }

        private IDisposable AddDisposable(IDisposable disposable) {
            _disposables.Add(disposable);
            return disposable;
        }

        public void AddJavascript(string javascript, params object[] variables) {
            javascript = INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, variables);
            AddJavascript(javascript);
        }

        public void AddJavascript(string javascript) {
            javascript = javascript.Trim();
            _javascript.Append(javascript);
            if (!javascript.EndsWith(";"))
                _javascript.Append(";");
        }

        public async Task ExecuteAndDisposeAsync() {
            if (_javascript.Length > 0)
                await INTERNAL_ExecuteJavaScript.ExecuteJavaScriptAsync(_javascript.ToString(), 0, false);
            foreach (var d in _disposables)
                d.Dispose();
        }

        public async void ExecuteAndDispose() {
            await ExecuteAndDisposeAsync();
        }
    }
}
