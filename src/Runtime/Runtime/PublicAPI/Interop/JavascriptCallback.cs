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
using System.Diagnostics;
using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    internal sealed class JavaScriptCallback : IJavaScriptConvertible, IDisposable
    {
        private static readonly SynchronyzedStore<JavaScriptCallback> _store = new();

        private readonly int _id;
        private readonly Delegate _callback;

        private JavaScriptCallback(Delegate callback)
        {
            Debug.Assert(callback != null);
            _callback = callback;
            _id = _store.Add(this);
        }

        public static JavaScriptCallback Create(Delegate callback)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            return new JavaScriptCallback(callback);
        }

        public static JavaScriptCallback Get(int index) => _store.Get(index);

        public Delegate GetCallback() => _callback;

        public void Dispose() => _store.Clean(_id);

        public override string ToString() => ToJavaScriptStringImpl();

        private string ToJavaScriptStringImpl()
            => $"document.getCallbackFunc({_id}, {GetSyncString()})";

        string IJavaScriptConvertible.ToJavaScriptString() => ToJavaScriptStringImpl();

        private static string GetSyncString() => OpenSilver.Interop.IsRunningInTheSimulator ? "false" : "true" ;
    }
}