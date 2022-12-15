
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
using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    internal sealed class JavascriptCallback : IJavaScriptConvertible, IDisposable
    {
        private static readonly SynchronyzedStore<JavascriptCallback> _store = new SynchronyzedStore<JavascriptCallback>();

        public int Id { get; private set; }

        public Delegate Callback { get; private set; }

        public WeakReference<Delegate> CallbackWeakReference { get; private set; }

        public static JavascriptCallback Create(Delegate callback)
        {
            var jc = new JavascriptCallback
            {
                Callback = callback
            };
            jc.Id = _store.Add(jc);

            return jc;
        }

        public static JavascriptCallback CreateWeak(Delegate callback)
        {
            var jc = new JavascriptCallback
            {
                CallbackWeakReference = new WeakReference<Delegate>(callback)
            };
            jc.Id = _store.Add(jc);

            return jc;
        }

        public static JavascriptCallback Get(int index)
        {
            return _store.Get(index);
        }

        public Delegate GetCallback()
        {
            if (Callback != null)
            {
                return Callback;
            }

            if (CallbackWeakReference.TryGetTarget(out var callback))
            {
                return callback;
            }

            _store.Clean(Id);

            return null;
        }

        public void Dispose()
        {
            _store.Clean(Id);
        }

        string IJavaScriptConvertible.ToJavaScriptString()
        {
            bool isVoid = GetCallback().Method.ReturnType == typeof(void);
            return $"document.getCallbackFunc({Id}, {(!isVoid).ToString().ToLower()}, {(!OpenSilver.Interop.IsRunningInTheSimulator).ToString().ToLower()})";
        }
    }
}