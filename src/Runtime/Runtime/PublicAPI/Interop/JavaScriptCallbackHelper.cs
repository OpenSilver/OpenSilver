
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
        public static JavaScriptCallback CreateSelfDisposedJavaScriptCallback(Action action, bool sync = false)
            => new SelfDisposedJavaScriptCallback(action, sync).JSCallback;

        private sealed class SelfDisposedJavaScriptCallback
        {
            private readonly Action _action;
            public readonly JavaScriptCallback JSCallback;

            public SelfDisposedJavaScriptCallback(Action action, bool sync)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));

                JSCallback = JavaScriptCallback.Create(RunCallbackAndDispose, sync);
            }

            private void RunCallbackAndDispose()
            {
                JSCallback.Dispose();
                _action();
            }
        }
    }
}