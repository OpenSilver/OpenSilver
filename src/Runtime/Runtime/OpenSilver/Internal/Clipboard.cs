
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
            else
            {
                return new NavigatorClipboard();
            }
        }

        private static bool IsWPFClipboardAvailable()
            => Interop.IsRunningInTheSimulator && INTERNAL_Simulator.AsyncClipboard != null;

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

                Interop.ExecuteJavaScriptVoid($"osjs.clipboard.setText({sText}, {sCallback})", false);

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

                Interop.ExecuteJavaScriptVoid($"osjs.clipboard.getText({sCallback})", false);

                return tcs.Task;
            }

            [Obsolete]
            public bool ContainsText() => false;

            public Task<bool> ContainsTextAsync()
            {
                var tcs = new TaskCompletionSource<bool>();

                string sCallback = Interop.GetVariableStringForJS(
                    JavaScriptCallbackHelper.CreateSelfDisposedJavaScriptCallback<bool>(b => tcs.SetResult(b)));

                Interop.ExecuteJavaScriptVoid($"osjs.clipboard.containsText({sCallback})", false);

                return tcs.Task;
            }

            private static SecurityException ClipboardAccessNotAllowException()
                => new SecurityException("Clipboard access is not allowed");
        }
    }
}
