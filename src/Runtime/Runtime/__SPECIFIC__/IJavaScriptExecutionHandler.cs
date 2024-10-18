
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
using OpenSilver.Internal;

namespace DotNetForHtml5;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IJavaScriptExecutionHandler
{
    void ExecuteJavaScript(string javaScriptToExecute);

    object ExecuteJavaScriptWithResult(string javaScriptToExecute);
}

[EditorBrowsable(EditorBrowsableState.Never)]
public interface INativeMethods : IJavaScriptExecutionHandler
{
    void InvokePendingJS(byte[] bytes, int length);
    object InvokeJS(string javascript, int referenceId, bool wantsResult);
    void WriteableBitmap_FillBufferInt32(int[] buffer);
    void WriteableBitmap_CreateFromBitmapSource(string data, Action<int, int, int> onSuccess, Action<string> onError);
    void WriteableBitmap_RenderUIElement(string id, int width, int height, string transform, Action<int, int, int> onSuccess, Action<string> onError);
}

[Obsolete(Helper.ObsoleteMemberMessage, true)]
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IWebAssemblyExecutionHandler : IJavaScriptExecutionHandler
{
    TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0);
    TResult InvokeUnmarshalled<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1);
    TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2);
}
