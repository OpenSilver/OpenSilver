
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
using System.Threading.Tasks;
using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    internal sealed class JavascriptCallback : IJavaScriptConvertible, IDisposable {
        private static object _lock = new object();
        private static readonly Dictionary<Delegate, JavascriptCallback> _store = new Dictionary<Delegate, JavascriptCallback>();
        private static readonly Dictionary<int, JavascriptCallback> _idToCallback = new Dictionary<int, JavascriptCallback>(); 

        private static int _nextId = 0;
        private bool _isVoid;
        public int Id { get; private set; }

        public Delegate Callback { get; private set; }

        public WeakReference<Delegate> CallbackWeakReference { get; private set; }

        static JavascriptCallback() {
        }

        public static void Remove(Delegate d) {
            lock (_lock) {
                if (_store.TryGetValue(d, out var cb)) {
                    _idToCallback.Remove(cb.Id);
                    _store.Remove(d);
                }
            }
        }
        public static void Remove(int id) {
            lock (_lock) {
                if (_idToCallback.TryGetValue(id, out var cb)) {
                    _store.Remove(cb.GetCallback());
                    _idToCallback.Remove(id);
                }
            }
        }

        public static JavascriptCallback Create(Delegate callback)
        {
            bool isVoid = callback.Method.ReturnType == typeof(void);
            lock (_lock) {
                if (_store.TryGetValue(callback, out var existingJc))
                    return existingJc;

                var id = ++_nextId;
                var jc = new JavascriptCallback
                {
                    Callback = callback,
                    _isVoid = isVoid,
                    Id = id,
                };
                _store.Add(callback, jc);
                _idToCallback.Add(id, jc);
                return jc;
            }
        }

        public static JavascriptCallback CreateWeak(Delegate callback)
        {
            bool isVoid = callback.Method.ReturnType == typeof(void);
            lock (_lock) {
                if (_store.TryGetValue(callback, out var existingJc))
                    return existingJc;

                var id = ++_nextId;
                var jc = new JavascriptCallback
                {
                    CallbackWeakReference = new WeakReference<Delegate>(callback), 
                    _isVoid = isVoid,
                    Id = id,
                };
                _store.Add(callback, jc);
                _idToCallback.Add(id, jc);
                return jc;
            }
        }

        public static JavascriptCallback Get(int index)
        {
            lock (_lock) {
                if (_idToCallback.TryGetValue(index, out var cb))
                    return cb;
            }
            return null;
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

            return null;
        }

        public void Dispose()
        {
            Remove(Id);
        }

        private string _jsCache;
        public string ToJavaScriptString() => _jsCache ??= $"document.getCallbackFunc({Id}, {(!_isVoid).ToString().ToLower()}, {(!OpenSilver.Interop.IsRunningInTheSimulator).ToString().ToLower()})";
    }
}