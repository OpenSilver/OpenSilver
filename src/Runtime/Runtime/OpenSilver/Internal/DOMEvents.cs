
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
using CSHTML5.Internal;

namespace OpenSilver.Internal;

internal static class DOMEvents
{
    internal static class Window
    {
        internal static JavaScriptCallback AddEventListener(string eventName, Delegate d)
        {
            var callback = JavaScriptCallback.Create(d);
            Interop.ExecuteJavaScriptVoidAsync(
                $"window.addEventListener('{eventName}', {Interop.GetVariableStringForJS(callback)})");
            return callback;
        }
    }

    internal static class Document
    {
        internal static JavaScriptCallback AddEventListener(string eventName, Delegate d)
        {
            var callback = JavaScriptCallback.Create(d);
            Interop.ExecuteJavaScriptVoidAsync(
                $"document.addEventListener('{eventName}', {Interop.GetVariableStringForJS(callback)})");
            return callback;
        }
    }
}
