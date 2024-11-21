
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

using DotNetForHtml5;
using System.Runtime.InteropServices.JavaScript;

namespace OpenSilver.WebAssembly;

internal sealed partial class NativeMethods : INativeMethods
{
    private NativeMethods() { }

    public static NativeMethods Instance { get; } = new();

    public void ExecuteJavaScript(string javaScriptToExecute) => InvokeJSVoidImpl(javaScriptToExecute);

    public object ExecuteJavaScriptWithResult(string javaScriptToExecute) => InvokeJSImpl(javaScriptToExecute, -1);

    public object? InvokeJS(string javascript, int referenceId, bool wantsResult)
    {
        if (wantsResult)
        {
            return InvokeJSImpl(javascript, referenceId);
        }

        InvokeJSVoidImpl(javascript);
        return null;
    }

    public void InvokePendingJS(byte[] bytes, int length) => InvokePendingJSImpl(bytes.AsSpan(0, length));

    public void WriteableBitmap_CreateFromBitmapSource(string data, Action<int, int, int> onSuccess, Action<string> onError)
        => WriteableBitmap_CreateFromBitmapSourceImpl(data, onSuccess, onError);

    public void WriteableBitmap_RenderUIElement(string id, int width, int height, string transform, Action<int, int, int> onSuccess, Action<string> onError)
        => WriteableBitmap_RenderUIElementImpl(id, width, height, transform, onSuccess, onError);

    public void WriteableBitmap_FillBufferInt32(int[] buffer) => WriteableBitMap_FillInt32BufferImpl(buffer);

    [JSImport("globalThis._openSilverRuntime.invokePendingJS")]
    private static partial void InvokePendingJSImpl([JSMarshalAs<JSType.MemoryView>] Span<byte> bytes);

    [JSImport("globalThis._openSilverRuntime.invokeJS")]
    [return: JSMarshalAs<JSType.Any>]
    private static partial object InvokeJSImpl(string javascript, int referenceId);

    [JSImport("globalThis._openSilverRuntime.invokeJSVoid")]
    private static partial void InvokeJSVoidImpl(string javascript);

    [JSImport("globalThis._openSilverRuntime.WBM.fillInt32Buffer")]
    private static partial void WriteableBitMap_FillInt32BufferImpl([JSMarshalAs<JSType.MemoryView>] Span<int> buffer);

    [JSImport("globalThis._openSilverRuntime.WBM.renderUIElement")]
    private static partial void WriteableBitmap_RenderUIElementImpl(
        string id,
        int width,
        int height,
        string transform,
        [JSMarshalAs<JSType.Function<JSType.Number, JSType.Number, JSType.Number>>] Action<int, int, int> onSuccess,
        [JSMarshalAs<JSType.Function<JSType.String>>] Action<string> onError);

    [JSImport("globalThis._openSilverRuntime.WBM.createFromBitmapSource")]
    private static partial void WriteableBitmap_CreateFromBitmapSourceImpl(
        string data,
        [JSMarshalAs<JSType.Function<JSType.Number, JSType.Number, JSType.Number>>] Action<int, int, int> onSuccess,
        [JSMarshalAs<JSType.Function<JSType.String>>] Action<string> onError);
}
