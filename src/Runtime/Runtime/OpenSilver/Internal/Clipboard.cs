
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
using DotNetForHtml5.Core;

namespace OpenSilver.Internal
{
    internal interface IAsyncClipboard : IClipboard
    {
        Task SetTextAsync(string text);

        Task<string> GetTextAsync();

        Task<bool> ContainsTextAsync();
    }

    internal interface IClipboard
    {
        void SetText(string text);

        string GetText();

        bool ContainsText();
    }

    internal static class ClipboardProvider
    {
        public static IAsyncClipboard GetClipboard()
        {
            if (IsWPFClipboardAvailable())
            {
                return new WPFClipboard();
            }
            else if (IsNavigatorClipboardAvailable())
            {
                return new NavigatorClipboard();
            }
            else
            {
                return new ExecCommandClipboard();
            }
        }

        private static bool IsWPFClipboardAvailable()
            => Interop.IsRunningInTheSimulator && INTERNAL_Simulator.ClipboardHandler != null;

        private static bool IsNavigatorClipboardAvailable()
            => Convert.ToBoolean(Interop.ExecuteJavaScript("!!navigator.clipboard"));

        private class WPFClipboard : IAsyncClipboard
        {
            public void SetText(string text) => INTERNAL_Simulator.ClipboardHandler.SetText(text);

            public Task SetTextAsync(string text)
            {
                SetText(text);
                return Task.CompletedTask;
            }

            public string GetText() => INTERNAL_Simulator.ClipboardHandler.GetText();

            public Task<string> GetTextAsync() => Task.FromResult(GetText());

            public bool ContainsText() => INTERNAL_Simulator.ClipboardHandler.ContainsText();

            public Task<bool> ContainsTextAsync() => Task.FromResult(ContainsText());
        }

        private class NavigatorClipboard : IAsyncClipboard
        {
            [Obsolete]
            public void SetText(string text)
            {
                _ = SetTextAsync(text);
            }

            public Task SetTextAsync(string text)
            {
                var tcs = new TaskCompletionSource<object>();

                Interop.ExecuteJavaScript("navigator.clipboard.writeText($0).then($1());",
                    text,
                    new Action(() =>
                    {
                        try
                        {
                            tcs.SetResult(null);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }));

                return tcs.Task;
            }

            [Obsolete]
            public string GetText() => string.Empty;

            public Task<string> GetTextAsync()
            {
                var tcs = new TaskCompletionSource<string>();

                Interop.ExecuteJavaScript("navigator.clipboard.readText().then(text => $0(text));",
                    new Action<string>(content =>
                    {
                        try
                        {
                            tcs.SetResult(content);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }));

                return tcs.Task;
            }

            [Obsolete]
            public bool ContainsText() => false;

            public Task<bool> ContainsTextAsync()
            {
                var tcs = new TaskCompletionSource<bool>();

                Interop.ExecuteJavaScript("navigator.clipboard.readText().then(text => $0(!!text))",
                    new Action<bool>(b =>
                    {
                        try
                        {
                            tcs.SetResult(b);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }));

                return tcs.Task;
            }
        }

        private class ExecCommandClipboard : IAsyncClipboard
        {
            static ExecCommandClipboard()
            {
                Interop.ExecuteJavaScript(@"_opensilver.clipboard = {
    writeText : function (data) {
        const input = document.createElement('input');
        document.body.appendChild(input);
        input.value = data;
        input.select();
        document.execCommand('copy');
    },

    readText : function () {
        const input = document.createElement('input');
        document.body.appendChild(input);
        input.focus();
        document.execCommand('paste');
        const text = input.value;
        input.remove();
        return text;
    }
};");
            }

            public void SetText(string text)
                => Interop.ExecuteJavaScript("_opensilver.clipboard.writeText($0)", text);

            public Task SetTextAsync(string text)
            {
                SetText(text);
                return Task.CompletedTask;
            }

            public string GetText()
                => Convert.ToString(Interop.ExecuteJavaScript("_opensilver.clipboard.readText()"));

            public Task<string> GetTextAsync() => Task.FromResult(GetText());

            public bool ContainsText()
                => Convert.ToBoolean(Interop.ExecuteJavaScript("!!_opensilver.clipboard.readText()"));

            public Task<bool> ContainsTextAsync() => Task.FromResult(ContainsText());
        }
    }
}
