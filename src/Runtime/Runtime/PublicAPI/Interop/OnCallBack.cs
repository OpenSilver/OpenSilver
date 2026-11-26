
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
using System.ComponentModel;
using Microsoft.JSInterop;

namespace CSHTML5.Internal
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class OnCallBack
    {
        static OnCallBack()
        {
            CheckIsRunningInBrowser();
        }

        [JSInvokable]
        public static void OnCallbackFromJavaScriptError(string idWhereCallbackArgsAreStored)
        {
            OnCallBackImpl.OnCallbackFromJavaScriptError(idWhereCallbackArgsAreStored);
        }

        // This method can be removed later. Now it is used for easier migration from old opensilver.js to new one
        [JSInvokable]
        public static object OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object[] callbackArgsObject)
        {
            return OnCallbackFromJavaScriptBrowser(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject,
                false);
        }

        [JSInvokable]
        public static object OnCallbackFromJavaScriptBrowser(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object[] callbackArgsObject,
            bool returnValue)
        {
            if (JavaScriptCallback.Get(callbackId) is not JavaScriptCallback jsCallback)
            {
                return null;
            }

            object result = jsCallback.Invoke(idWhereCallbackArgsAreStored, callbackArgsObject);

            // On the Mono WebAssembly interpreter, the value returned for a void callback is sometimes a stale
            // reference to an object used during the call rather than null, and Blazor then fails to serialize it.
            return returnValue && jsCallback.ReturnsValue ? result : null;
        }

        private static void CheckIsRunningInBrowser()
        {
            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                throw new InvalidOperationException($"'{nameof(OnCallBack)}' is not supported in the simulator.");
            }
        }
    }
}
