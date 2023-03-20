
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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSHTML5;
using CSHTML5.Internal;

namespace OpenSilver
{
    public class BulkExecuteJavascriptAsync
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly StringBuilder _javascript = new StringBuilder();

        public BulkExecuteJavascriptAsync() { }

        public IDisposable AddJavascriptAsync(string javascript, params object[] variables)
        {
            javascript = INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, variables);
            return AddJavascriptAsync(javascript);
        }

        public IDisposable AddJavascriptAsync(string javascript)
        {
            return AddDisposable(Interop.ExecuteJavaScriptAsync(javascript));
        }

        private IDisposable AddDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
            return disposable;
        }

        public void AddJavascript(string javascript, params object[] variables)
        {
            javascript = INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, variables);
            AddJavascript(javascript);
        }

        public void AddJavascript(string javascript)
        {
            javascript = javascript.Trim();
            _javascript.Append(javascript);
            if (!javascript.EndsWith(";"))
                _javascript.Append(";");
        }

        public async Task ExecuteAndDisposeAsync()
        {
            if (_javascript.Length > 0)
            {
                try
                {
                    // very important: the first functions need to be executed before executing the remaining javascript,
                    // since the remaining javascript can rely on the results from here
                    INTERNAL_ExecuteJavaScript.JavaScriptRuntime.Flush();

                    await INTERNAL_ExecuteJavaScript.ExecuteJavaScriptAsync(_javascript.ToString(), 0, false);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"error executing Javascript: {e}");
                }
            }

            // Console.WriteLine($"disposed of JS Obj Refs: {string.Join(",", _disposables.OfType<INTERNAL_JSObjectReference>().Select(js => js.ReferenceId))}");
            foreach (var d in _disposables)
            {
                d.Dispose();
            }
        }

        public async Task ExecuteAndDispose()
        {
            await ExecuteAndDisposeAsync();
        }
    }
}
