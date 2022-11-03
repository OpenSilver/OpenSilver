
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

using CSHTML5.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

#if BRIDGE
using Bridge;
using DotNetBrowser;
#endif

#if OPENSILVER
using Microsoft.JSInterop;
#endif

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
            OnCallBackImpl.Instance.OnCallbackFromJavaScriptError(idWhereCallbackArgsAreStored);
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
            return OnCallBackImpl.Instance.OnCallbackFromJavaScript(
                callbackId, idWhereCallbackArgsAreStored, callbackArgsObject, false, returnValue);
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
