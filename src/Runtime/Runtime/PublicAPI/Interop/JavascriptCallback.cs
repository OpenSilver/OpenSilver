
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
using System.Diagnostics;
using System.Windows;
using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    internal sealed class JavaScriptCallback : IJavaScriptConvertible, IDisposable
    {
        private static readonly SynchronyzedStore<JavaScriptCallback> _store = new();

        private readonly int _id;
        private readonly Delegate _callback;
        private readonly bool _handleExceptions;

        private JavaScriptCallback(Delegate callback, bool handleExceptions)
        {
            Debug.Assert(callback != null);
            _callback = callback;
            _handleExceptions = handleExceptions;
            _id = _store.Add(this);
        }

        public static JavaScriptCallback Create(Delegate callback, bool handleExceptions = true)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            return new JavaScriptCallback(callback, handleExceptions);
        }

        public static JavaScriptCallback Get(int index) => _store.Get(index);

        internal object Invoke(string idWhereCallbackArgsAreStored, object callbackArgs)
        {
            if (_handleExceptions)
            {
                return InvokeWithExceptionHandling(idWhereCallbackArgsAreStored, callbackArgs);
            }

            return InvokeImpl(idWhereCallbackArgsAreStored, callbackArgs);
        }

        private object InvokeWithExceptionHandling(string idWhereCallbackArgsAreStored, object callbackArgs)
        {
            try
            {
                return InvokeImpl(idWhereCallbackArgsAreStored, callbackArgs);
            }
            catch (Exception ex)
            {
                bool handled = Application.CallHandleException(ex);

                if (!handled)
                {
                    throw;
                }
            }

            return null;
        }

        private object InvokeImpl(string idWhereCallbackArgsAreStored, object callbackArgs)
            => OnCallBackImpl.Instance.OnCallbackFromJavaScript(_callback, idWhereCallbackArgsAreStored, callbackArgs);

        public void Dispose() => _store.Clean(_id);

        public override string ToString() => ToJavaScriptStringImpl();

        private string ToJavaScriptStringImpl()
            => $"document.getCallbackFunc({_id}, {GetSyncString()})";

        string IJavaScriptConvertible.ToJavaScriptString() => ToJavaScriptStringImpl();

        private static string GetSyncString() => OpenSilver.Interop.IsRunningInTheSimulator ? "false" : "true" ;
    }
}