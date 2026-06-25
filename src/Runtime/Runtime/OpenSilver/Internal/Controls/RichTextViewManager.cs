
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

using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Controls;

internal static class RichTextViewManager
{
    public static void CreateView(string id, string parentId) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.richTextViewManager.createView('{id}','{parentId}')");

    public static bool OnKeyDown(RichTextBoxView richTextBoxView, KeyEventArgs e)
    {
        Debug.Assert(richTextBoxView is not null && richTextBoxView.OuterDiv.IsConnected);
        Debug.Assert(e is not null);

        string sArgs = Interop.GetVariableStringForJS(e.UIEventArg);
        return Interop.ExecuteJavaScriptBoolean($"osjs.richTextViewManager.onKeyDownNative('{richTextBoxView.OuterDiv.Uid}', {sArgs})");
    }

    internal static void OnSelectionChangedNative(string id, int start, int length)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is RichTextBoxView view)
        {
            view.Host.UpdateSelection(start, length);
        }
    }

    internal static void OnContentChangedNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is RichTextBoxView view)
        {
            view.OnInput();
        }
    }

    internal static void OnScrollNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is RichTextBoxView view)
        {
            if (!view.IsScrollClient) return;

            double scrollLeft = Interop.ExecuteJavaScriptDouble($"osjs.getProp('{view.OuterDiv.Uid}','scrollLeft')");
            double scrollTop = Interop.ExecuteJavaScriptDouble($"osjs.getProp('{view.OuterDiv.Uid}','scrollTop')");

            view.UpdateOffsets(new Vector(scrollLeft, scrollTop));
        }
    }
}
