﻿
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
        public static JavaScriptCallback CreateSelfDisposedJavaScriptCallback(Action action)
            => new SelfDisposedJavaScriptCallback(action).JSCallback;

        public static JavaScriptCallback CreateSelfDisposedJavaScriptCallback<T>(Action<T> action)
            => new SelfDisposedJavaScriptCallback<T>(action).JSCallback;

        public static JavaScriptCallback CreateSelfDisposedJavaScriptCallback<T1, T2>(Action<T1, T2> action)
           => new SelfDisposedJavaScriptCallback<T1, T2>(action).JSCallback;

        private sealed class SelfDisposedJavaScriptCallback
        {
            private readonly Action _action;
            public readonly JavaScriptCallback JSCallback;

            public SelfDisposedJavaScriptCallback(Action action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));

                JSCallback = JavaScriptCallback.Create(RunCallbackAndDispose);
            }

            private void RunCallbackAndDispose()
            {
                JSCallback.Dispose();
                _action();
            }
        }

        private sealed class SelfDisposedJavaScriptCallback<T>
        {
            private readonly Action<T> _action;
            public readonly JavaScriptCallback JSCallback;

            public SelfDisposedJavaScriptCallback(Action<T> action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));

                JSCallback = JavaScriptCallback.Create(RunCallbackAndDispose);
            }

            private void RunCallbackAndDispose(T arg0)
            {
                JSCallback.Dispose();
                _action(arg0);
            }
        }

        private sealed class SelfDisposedJavaScriptCallback<T1, T2>
        {
            private readonly Action<T1, T2> _action;
            public readonly JavaScriptCallback JSCallback;

            public SelfDisposedJavaScriptCallback(Action<T1, T2> action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));

                JSCallback = JavaScriptCallback.Create(RunCallbackAndDispose);
            }

            private void RunCallbackAndDispose(T1 arg1, T2 arg2)
            {
                JSCallback.Dispose();
                _action(arg1, arg2);
            }
        }
    }
}