
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using CSHTML5;

namespace TypeScriptDefinitionsSupport
{
    public class Promise : JSObject
    {
        public void then(Action onFulfilled, Action<object> onRejected)
        {
            Interop.ExecuteJavaScriptAsync("$0.then($1, $2)", UnderlyingJSInstance, onFulfilled, onRejected);
        }
    }

    public class Promise<T> : JSObject
    {
        public void then(Action<T> onFulfilled, Action<object> onRejected)
        {
            Interop.ExecuteJavaScriptAsync("$0.then($1, $2)", UnderlyingJSInstance, onFulfilled, onRejected);
        }
    }
}
