

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
using CSHTML5;

namespace TypeScriptDefinitionsSupport
{
    public class Promise : JSObject
    {
        public void then(Action onFulfilled, Action<object> onRejected)
        {
            OpenSilver.Interop.ExecuteJavaScriptAsync("$0.then($1, $2)", UnderlyingJSInstance, onFulfilled, onRejected);
        }
    }

    public class Promise<T> : JSObject
    {
        public void then(Action<T> onFulfilled, Action<object> onRejected)
        {
            OpenSilver.Interop.ExecuteJavaScriptAsync("$0.then($1, $2)", UnderlyingJSInstance, onFulfilled, onRejected);
        }
    }
}
