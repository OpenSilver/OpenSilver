
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

namespace CSHTML5.Internal
{
    internal static class JavaScriptCallbackHelper
    {
        public static JavascriptCallback CreateSelfDisposedJavaScriptCallback(Action action, bool sync = false)
        {
            if (sync)
            {
                return new SelfDisposedJavaScriptCallback(action).JSCallback;
            }
            else
            {
                return new SelfDisposedJavaScriptCallbackAsync(action).JSCallback;
            }
        }

        private sealed class SelfDisposedJavaScriptCallback
        {
            private readonly Action _action;
            public readonly JavascriptCallback JSCallback;

            public SelfDisposedJavaScriptCallback(Action action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));

                JSCallback = JavascriptCallback.Create(RunCallbackAndDispose);
            }

            private string RunCallbackAndDispose()
            {
                JSCallback.Dispose();
                _action();

                return string.Empty;
            }
        }

        private sealed class SelfDisposedJavaScriptCallbackAsync
        {
            private readonly Action _action;
            public readonly JavascriptCallback JSCallback;

            public SelfDisposedJavaScriptCallbackAsync(Action action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));

                JSCallback = JavascriptCallback.Create(RunCallbackAndDispose);
            }

            private void RunCallbackAndDispose()
            {
                JSCallback.Dispose();
                _action();
            }
        }
    }
}