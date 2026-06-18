
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
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using OpenSilver;
using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    internal sealed class JavaScriptCallback : IJavaScriptConvertible, IDisposable
    {
        private static readonly SynchronyzedStore<JavaScriptCallback> _store = new();

        private readonly int _id;
        private readonly Delegate _callback;
        private readonly bool _handleExceptions;
        private readonly bool _unwrapExceptions;

        private JavaScriptCallback(Delegate callback, bool handleExceptions)
        {
            Debug.Assert(callback != null);
            _callback = callback;
            _handleExceptions = handleExceptions;
            _unwrapExceptions = handleExceptions && !OnCallBackImpl.IsCommonType(callback);
            _id = _store.Add(this);
        }

        public static JavaScriptCallback Create(Delegate callback, bool handleExceptions = true)
        {
            ArgumentNullException.ThrowIfNull(callback);

            return new JavaScriptCallback(callback, handleExceptions);
        }

        public static JavaScriptCallback Get(int index) => _store.Get(index);

        internal object Invoke(string idWhereCallbackArgsAreStored, object callbackArgs)
        {
            if (OpenSilverCompatibilityPreferences.HandleJavaScriptCallbackExceptions)
            {
                if (_handleExceptions)
                {
                    return InvokeWithExceptionHandling(idWhereCallbackArgsAreStored, callbackArgs);
                }

                return InvokeImpl(idWhereCallbackArgsAreStored, callbackArgs);
            }
            else
            {
                return LegacyInvoke(idWhereCallbackArgsAreStored, callbackArgs);
            }
        }

        private object InvokeWithExceptionHandling(string idWhereCallbackArgsAreStored, object callbackArgs)
        {
            var oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(Dispatcher.CurrentDispatcher.DefaultSynchronizationContext);

            try
            {
                return InvokeImpl(idWhereCallbackArgsAreStored, callbackArgs);
            }
            catch (Exception ex)
            {
                bool handled = Application.CallHandleException(UnwrapException(ex));

                if (!handled)
                {
                    throw;
                }
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);
            }

            return null;
        }

        private Exception UnwrapException(Exception ex)
        {
            if (_unwrapExceptions && ex is TargetInvocationException tie && tie.InnerException is not null)
            {
                return tie.InnerException;
            }
            return ex;
        }

        private object InvokeImpl(string idWhereCallbackArgsAreStored, object callbackArgs)
            => OnCallBackImpl.OnCallbackFromJavaScript(_callback, idWhereCallbackArgsAreStored, callbackArgs);

        private object LegacyInvoke(string idWhereCallbackArgsAreStored, object callbackArgs)
        {
            try
            {
                return InvokeImpl(idWhereCallbackArgsAreStored, callbackArgs);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("DEBUG: OnCallBack: OnCallBackFromJavascript: " + ex);
                throw;
            }
        }

        public void Dispose() => _store.Clean(_id);

        public override string ToString() => ToJavaScriptStringImpl();

        private string ToJavaScriptStringImpl()
            => $"osjs.getCallbackFunc({_id}, {GetSyncString()})";

        string IJavaScriptConvertible.ToJavaScriptString() => ToJavaScriptStringImpl();

        private static string GetSyncString() => OpenSilver.Interop.IsRunningInTheSimulator ? "false" : "true" ;
    }
}