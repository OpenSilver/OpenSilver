
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Controls;

internal sealed class PasswordBoxView : TextViewBase
{
    internal PasswordBoxView(PasswordBox host)
        : base(host)
    {
    }

    internal new PasswordBox Host => (PasswordBox)base.Host;

    /// <inheritdoc />
    protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        return INTERNAL_HtmlDomManager.CreatePasswordBoxViewDomElementAndAppendIt(parent, this);
    }

    protected internal override void INTERNAL_OnAttachedToVisualTree()
    {
        base.INTERNAL_OnAttachedToVisualTree();

        SetProperties();

        if (FocusManager.GetFocusedElement() == Host)
        {
            InputManager.SetFocusNative(OuterDiv);
        }
    }

    internal protected sealed override void OnInput()
    {
        Host.UpdatePasswordProperty(GetPassword());
        InvalidateMeasure();
    }

    protected sealed override Size MeasureContent(Size constraint)
    {
        int pwdLength = Host.Password.Length;

        return ParentWindow.TextMeasurementService.MeasureView(
            OuterDiv.Uid,
            "pre",
            string.Empty,
            constraint.Width,
            pwdLength > 0 ? new string('•', pwdLength) : "M");
    }

    internal void SelectNative()
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            Interop.ExecuteJavaScriptVoid($"document.textviewManager.select('{OuterDiv.Uid}')");
        }
    }

    internal void OnMaxLengthChanged(int maxLength)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            if (maxLength > 0)
            {
                OuterDiv.SetAttribute("maxlength", maxLength);
            }
            else
            {
                OuterDiv.RemoveAttribute("maxlength");
            }
        }
    }

    internal void SetCaretBrush(Brush brush)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            this.SetCaretColor(brush);
        }
    }

    internal void SetPasswordNative(string text)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            Interop.ExecuteJavaScriptVoid(
                $"document.setProp('{OuterDiv.Uid}','value',\"{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text)}\")");

            InvalidateMeasure();
        }
    }

    private string GetPassword()
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            return Interop.ExecuteJavaScriptString($"document.getProp('{OuterDiv.Uid}','value')") ?? string.Empty;
        }

        return string.Empty;
    }

    private void SetProperties()
    {
        PasswordBox host = Host;

        this.SetCaretColor(host.CaretBrush);
        int maxLength = host.MaxLength;
        if (maxLength > 0)
        {
            OuterDiv.SetAttribute("maxlength", maxLength);
        }
        SetPasswordNative(host.Password);
    }
}