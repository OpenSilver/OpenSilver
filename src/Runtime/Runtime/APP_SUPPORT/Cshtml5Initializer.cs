
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

using CSHTML5.Internal;
using DotNetForHtml5.Core;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DotNetForHtml5;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class Cshtml5Initializer
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Initialize(INativeMethods nativeMethods) => Initialize((IJavaScriptExecutionHandler)nativeMethods);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use Cshtml5Initializer.Initialize(IWebAssemblyExecutionHandler2) instead.", true)]
    public static void Initialize(IWebAssemblyExecutionHandler executionHandler)
    {
        Initialize((IJavaScriptExecutionHandler)executionHandler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Initialize(IJavaScriptExecutionHandler executionHandler)
    {
        INTERNAL_Simulator.JavaScriptExecutionHandler = executionHandler;
        InitializeRuntime();
    }

    private static void InitializeRuntime()
    {
        var inputManagerCallback = JavaScriptCallback.Create(InputManager.ProcessInputNative);
        var inputManagerPointerCallback = JavaScriptCallback.Create(InputManager.ProcessPointerInputNative);
        var textViewManagerInputCallback = JavaScriptCallback.Create(TextViewManager.OnInputNative);
        var textViewManagerScrollCallback = JavaScriptCallback.Create(TextViewManager.OnScrollNative);
        var textViewManagerSelectionChangeCallback = JavaScriptCallback.Create(TextViewManager.OnSelectionChangeNative);
        var richTextViewManagerSelectionChangedCallback = JavaScriptCallback.Create(RichTextViewManager.OnSelectionChangedNative);
        var richTextViewManagerContentChangedCallback = JavaScriptCallback.Create(RichTextViewManager.OnContentChangedNative);
        var richTextViewManagerScrollCallback = JavaScriptCallback.Create(RichTextViewManager.OnScrollNative);
        var imageLoadCallback = JavaScriptCallback.Create(Image.OnLoadNative);
        var imageErrorCallback = JavaScriptCallback.Create(Image.OnErrorNative);
        var resizeObserverSizeChangedCallback = JavaScriptCallback.Create(ResizeObserver.OnSizeChangedNative);
        var hyperlinkClickCallback = JavaScriptCallback.Create(Hyperlink.OnClickNative);

        OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
            $"""
            osjs.initialize(
              {OpenSilver.Interop.GetVariableStringForJS(inputManagerCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(inputManagerPointerCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(textViewManagerInputCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(textViewManagerScrollCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(textViewManagerSelectionChangeCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(richTextViewManagerSelectionChangedCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(richTextViewManagerContentChangedCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(richTextViewManagerScrollCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(imageLoadCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(imageErrorCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(resizeObserverSizeChangedCallback)},
              {OpenSilver.Interop.GetVariableStringForJS(hyperlinkClickCallback)})
            """);
    }
}
