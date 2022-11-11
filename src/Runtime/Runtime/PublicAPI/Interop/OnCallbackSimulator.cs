

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

#if BRIDGE
using Bridge;
using DotNetBrowser;
#endif

#if OPENSILVER
#endif

namespace CSHTML5.Internal
{
    internal sealed class OnCallbackSimulator
    {
        public OnCallbackSimulator()
        {
            CheckIsRunningInTheSimulator();
        }

        public void OnCallbackFromJavaScriptError(string idWhereCallbackArgsAreStored)
        {
            OnCallBackImpl.Instance.OnCallbackFromJavaScriptError(idWhereCallbackArgsAreStored);
        }

        // This method can be removed later. Now it is used for easier migration from old cshtml5.js to new one
        public object OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object callbackArgsObject)
        {
            return OnCallbackFromJavaScript(callbackId, idWhereCallbackArgsAreStored, callbackArgsObject,
                false);
        }

        public object OnCallbackFromJavaScript(
            int callbackId,
            string idWhereCallbackArgsAreStored,
            object callbackArgsObject,
            bool returnValue)
        {
            return OnCallBackImpl.Instance.OnCallbackFromJavaScript(
                callbackId, idWhereCallbackArgsAreStored, callbackArgsObject, true, returnValue);
        }

        private static void CheckIsRunningInTheSimulator()
        {
            if (!OpenSilver.Interop.IsRunningInTheSimulator)
            {
                throw new InvalidOperationException($"'{nameof(OnCallbackSimulator)}' is not supported in the browser.");
            }
        }
    }
}
