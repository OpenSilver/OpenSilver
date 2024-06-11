
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

    private TextViewManager()
    {
        _inputHandler = JavaScriptCallback.Create(OnInputNative);
        _scrollHandler = JavaScriptCallback.Create(OnScrollNative);
        string sInputHandler = Interop.GetVariableStringForJS(_inputHandler);
        string sScrollHandler = Interop.GetVariableStringForJS(_scrollHandler);
        Interop.ExecuteJavaScriptVoidAsync($"document.createTextviewManager({sInputHandler},{sScrollHandler})");
    }

    public static TextViewManager Instance { get; } = new();

    public void CreateTextView(string id, string parentId) =>
        Interop.ExecuteJavaScriptVoidAsync($"document.textviewManager.createTextView('{id}','{parentId}')");

    public void CreatePasswordView(string id, string parentId) =>
        Interop.ExecuteJavaScriptVoidAsync($"document.textviewManager.createPasswordView('{id}','{parentId}')");

    public bool OnKeyDown(TextBoxView textBoxView, KeyEventArgs e)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv is not null);
        Debug.Assert(e is not null);

        string sElement = Interop.GetVariableStringForJS(textBoxView.OuterDiv);
        string sArgs = Interop.GetVariableStringForJS(e.UIEventArg);
        return Interop.ExecuteJavaScriptBoolean($"document.textviewManager.onKeyDownNative({sElement}, {sArgs})");
    }

    public int GetSelectionStart(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv is not null);

        string sElement = Interop.GetVariableStringForJS(textBoxView.OuterDiv);
        return Interop.ExecuteJavaScriptInt32($"document.textviewManager.getSelectionStart({sElement})");
    }

    public void SetSelectionStart(TextBoxView textBoxView, int selectionStart)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv is not null);

        string sElement = Interop.GetVariableStringForJS(textBoxView.OuterDiv);
        Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectionStart({sElement}, {selectionStart.ToInvariantString()})");
    }

    public int GetSelectionLength(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv is not null);

        string sElement = Interop.GetVariableStringForJS(textBoxView.OuterDiv);
        return Interop.ExecuteJavaScriptInt32($"document.textviewManager.getSelectionLength({sElement})");
    }

    public void SetSelectionLength(TextBoxView textBoxView, int selectionLength)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv is not null);

        string sElement = Interop.GetVariableStringForJS(textBoxView.OuterDiv);
        Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectionLength({sElement}, {selectionLength.ToInvariantString()})");
    }

    public string GetSelectedText(TextBoxView textBoxView)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv is not null);

        string sElement = Interop.GetVariableStringForJS(textBoxView.OuterDiv);
        return Interop.ExecuteJavaScriptString($"document.textviewManager.getSelectedText({sElement})");
    }

    public void SetSelectedText(TextBoxView textBoxView, string text)
    {
        Debug.Assert(textBoxView is not null && textBoxView.OuterDiv is not null);

        string sElement = Interop.GetVariableStringForJS(textBoxView.OuterDiv);
        string sText = Interop.GetVariableStringForJS(text);
        Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectedText({sElement}, {sText})");
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

            string sDiv = Interop.GetVariableStringForJS(textview.OuterDiv);
            double scrollLeft = Interop.ExecuteJavaScriptDouble($"{sDiv}.scrollLeft");
            double scrollTop = Interop.ExecuteJavaScriptDouble($"{sDiv}.scrollTop");

            textview.UpdateOffsets(new Point(scrollLeft, scrollTop));
        }
    }
}
