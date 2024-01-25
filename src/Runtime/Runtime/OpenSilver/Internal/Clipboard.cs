
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
using System.Security;
using System.Threading.Tasks;
using CSHTML5.Internal;
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
            => Interop.IsRunningInTheSimulator && INTERNAL_Simulator.AsyncClipboard != null;

        private static bool IsNavigatorClipboardAvailable()
            => Interop.ExecuteJavaScriptBoolean("!!navigator.clipboard", false);

        private class WPFClipboard : IAsyncClipboard
        {
            public void SetText(string text) => INTERNAL_Simulator.AsyncClipboard.SetText(text);

            public Task SetTextAsync(string text) => INTERNAL_Simulator.AsyncClipboard.SetTextAsync(text);

            public string GetText() => INTERNAL_Simulator.AsyncClipboard.GetText();

            public Task<string> GetTextAsync() => INTERNAL_Simulator.AsyncClipboard.GetTextAsync();

            public bool ContainsText() => INTERNAL_Simulator.AsyncClipboard.ContainsText();

            public Task<bool> ContainsTextAsync() => INTERNAL_Simulator.AsyncClipboard.ContainsTextAsync();
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

                string sCallback = Interop.GetVariableStringForJS(
                    JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback<bool>(success =>
                    {
                        if (success)
                        {
                            tcs.SetResult(null);
                        }
                        else
                        {
                            tcs.SetException(ClipboardAccessNotAllowException());
                        }
                    }));

                string sText = Interop.GetVariableStringForJS(text);

                Interop.ExecuteJavaScriptVoid(
                    $"navigator.clipboard.writeText({sText}).then(() => {sCallback}(true), () => {sCallback}(false));",
                    false);

                return tcs.Task;
            }

            [Obsolete]
            public string GetText() => string.Empty;

            public Task<string> GetTextAsync()
            {
                var tcs = new TaskCompletionSource<string>();

                string sCallback = Interop.GetVariableStringForJS(
                    JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback<string, bool>((content, success) =>
                    {
                        if (success)
                        {
                            tcs.SetResult(content);
                        }
                        else
                        {
                            tcs.SetException(ClipboardAccessNotAllowException());
                        }
                    }));

                Interop.ExecuteJavaScriptVoid(
                    $"navigator.clipboard.readText().then(text => {sCallback}(text, true), () => {sCallback}('', false));",
                    false);

                return tcs.Task;
            }

            [Obsolete]
            public bool ContainsText() => false;

            public Task<bool> ContainsTextAsync()
            {
                var tcs = new TaskCompletionSource<bool>();

                string sCallback = Interop.GetVariableStringForJS(
                    JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback<bool>(b => tcs.SetResult(b)));

                Interop.ExecuteJavaScriptVoid(
                    $"navigator.clipboard.readText().then(text => {sCallback}(!!text), () => {sCallback}(false));",
                    false);

                return tcs.Task;
            }

            private static SecurityException ClipboardAccessNotAllowException()
                => new SecurityException("Clipboard access is not allowed");
        }

        private class ExecCommandClipboard : IAsyncClipboard
        {
            static ExecCommandClipboard()
            {
                Interop.ExecuteJavaScriptVoid(@"_opensilver.clipboard = {
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
            {
                string escapedText = Interop.GetVariableStringForJS(text);
                Interop.ExecuteJavaScriptVoid($"_opensilver.clipboard.writeText({escapedText})");
            }

            public Task SetTextAsync(string text)
            {
                SetText(text);
                return Task.CompletedTask;
            }

            public string GetText()
                => Interop.ExecuteJavaScriptString("_opensilver.clipboard.readText()");

            public Task<string> GetTextAsync() => Task.FromResult(GetText());

            public bool ContainsText()
                => Interop.ExecuteJavaScriptBoolean("!!_opensilver.clipboard.readText()");

            public Task<bool> ContainsTextAsync() => Task.FromResult(ContainsText());
        }
    }
}
