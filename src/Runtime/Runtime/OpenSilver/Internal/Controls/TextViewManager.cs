
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

internal sealed class TextViewManager
{
    private readonly JavaScriptCallback _inputHandler;
    private readonly JavaScriptCallback _scrollHandler;
    private readonly JavaScriptCallback _selectionChangeHandler;

    private TextViewManager()
    {
        _inputHandler = JavaScriptCallback.Create(OnInputNative);
        _scrollHandler = JavaScriptCallback.Create(OnScrollNative);
        _selectionChangeHandler = JavaScriptCallback.Create(OnSelectionChangeNative);
        string sInputHandler = Interop.GetVariableStringForJS(_inputHandler);
        string sScrollHandler = Interop.GetVariableStringForJS(_scrollHandler);
        string sSelectionChangeHandler = Interop.GetVariableStringForJS(_selectionChangeHandler);
        Interop.ExecuteJavaScriptVoidAsync($"document.createTextviewManager({sInputHandler},{sScrollHandler},{sSelectionChangeHandler})");
    }

    public static TextViewManager Instance { get; } = new();

    public void CreateTextView(string id, string parentId) =>
        Interop.ExecuteJavaScriptVoidAsync($"document.textviewManager.createTextView('{id}','{parentId}')");

    public void CreatePasswordView(string id, string parentId) =>
        Interop.ExecuteJavaScriptVoidAsync($"document.textviewManager.createPasswordView('{id}','{parentId}')");

    public bool OnKeyDown(TextBoxView textBoxView, KeyEventArgs e)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);
        Debug.Assert(e is not null);

        string sArgs = Interop.GetVariableStringForJS(e.UIEventArg);
        return Interop.ExecuteJavaScriptBoolean($"document.textviewManager.onKeyDownNative('{textBoxView.OuterDiv.Uid}', {sArgs})");
    }

    public int GetSelectionStart(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        return Interop.ExecuteJavaScriptInt32($"document.textviewManager.getSelectionStart('{textBoxView.OuterDiv.Uid}')");
    }

    public void SetSelectionStart(TextBoxView textBoxView, int selectionStart)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectionStart('{textBoxView.OuterDiv.Uid}', {selectionStart.ToInvariantString()})");
    }

    public int GetSelectionLength(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        return Interop.ExecuteJavaScriptInt32($"document.textviewManager.getSelectionLength('{textBoxView.OuterDiv.Uid}')");
    }

    public void SetSelectionLength(TextBoxView textBoxView, int selectionLength)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectionLength('{textBoxView.OuterDiv.Uid}', {selectionLength.ToInvariantString()})");
    }

    public string GetSelectedText(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        return Interop.ExecuteJavaScriptString($"document.textviewManager.getSelectedText('{textBoxView.OuterDiv.Uid}')");
    }

    public void SetSelectedText(TextBoxView textBoxView, string text)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv.IsConnected);

        string sText = Interop.GetVariableStringForJS(text);
        Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectedText('{textBoxView.OuterDiv.Uid}', {sText})");
    }

    private static void OnInputNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is TextViewBase textview)
        {
            textview.OnInput();
        }
    }

    private static void OnScrollNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is TextViewBase textview)
        {
            if (!textview.IsScrollClient) return;

            double scrollLeft = Interop.ExecuteJavaScriptDouble($"document.getProp('{textview.OuterDiv.Uid}','scrollLeft')");
            double scrollTop = Interop.ExecuteJavaScriptDouble($"document.getProp('{textview.OuterDiv.Uid}','scrollTop')");

            textview.UpdateOffsets(new Vector(scrollLeft, scrollTop));
        }
    }

    private static void OnSelectionChangeNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is TextBoxView textview)
        {
            textview.OnSelectionChange();
        }
    }
}
