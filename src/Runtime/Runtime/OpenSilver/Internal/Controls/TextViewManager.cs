
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

internal static class TextViewManager
{
    public static void CreateTextView(string id, string parentId) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.textviewManager.createTextView('{id}','{parentId}')");

    public static void CreatePasswordView(string id, string parentId) =>
        Interop.ExecuteJavaScriptVoidAsync($"osjs.textviewManager.createPasswordView('{id}','{parentId}')");

    public static bool OnKeyDown(TextBoxView textBoxView, KeyEventArgs e)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);
        Debug.Assert(e is not null);

        string sArgs = Interop.GetVariableStringForJS(e.UIEventArg);
        return Interop.ExecuteJavaScriptBoolean($"osjs.textviewManager.onKeyDownNative('{textBoxView.OuterDiv.Uid}', {sArgs})");
    }

    public static int GetSelectionStart(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        return Interop.ExecuteJavaScriptInt32($"osjs.textviewManager.getSelectionStart('{textBoxView.OuterDiv.Uid}')");
    }

    public static void SetSelectionStart(TextBoxView textBoxView, int selectionStart)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        Interop.ExecuteJavaScriptVoid($"osjs.textviewManager.setSelectionStart('{textBoxView.OuterDiv.Uid}', {selectionStart.ToInvariantString()})");
    }

    public static int GetSelectionLength(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        return Interop.ExecuteJavaScriptInt32($"osjs.textviewManager.getSelectionLength('{textBoxView.OuterDiv.Uid}')");
    }

    public static void SetSelectionLength(TextBoxView textBoxView, int selectionLength)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        Interop.ExecuteJavaScriptVoid($"osjs.textviewManager.setSelectionLength('{textBoxView.OuterDiv.Uid}', {selectionLength.ToInvariantString()})");
    }

    public static string GetSelectedText(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        return Interop.ExecuteJavaScriptString($"osjs.textviewManager.getSelectedText('{textBoxView.OuterDiv.Uid}')");
    }

    public static void SetSelectedText(TextBoxView textBoxView, string text)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        string sText = Interop.GetVariableStringForJS(text);
        Interop.ExecuteJavaScriptVoid($"osjs.textviewManager.setSelectedText('{textBoxView.OuterDiv.Uid}', {sText})");
    }

    internal static void OnInputNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is TextViewBase textview)
        {
            textview.OnInput();
        }
    }

    internal static void OnScrollNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is TextViewBase textview)
        {
            if (!textview.IsScrollClient) return;

            double scrollLeft = Interop.ExecuteJavaScriptDouble($"osjs.getProp('{textview.OuterDiv.Uid}','scrollLeft')");
            double scrollTop = Interop.ExecuteJavaScriptDouble($"osjs.getProp('{textview.OuterDiv.Uid}','scrollTop')");

            textview.UpdateOffsets(new Vector(scrollLeft, scrollTop));
        }
    }

    internal static void OnSelectionChangeNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is TextBoxView textview)
        {
            textview.OnSelectionChange();
        }
    }
}
