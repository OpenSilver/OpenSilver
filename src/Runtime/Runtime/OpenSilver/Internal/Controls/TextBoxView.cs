
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Controls;

internal sealed class TextBoxView : TextViewBase
{
    static TextBoxView()
    {
        Block.LineHeightProperty.AddOwner(
            typeof(TextBoxView),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetLineHeight((double)newValue),
            });

        Block.TextAlignmentProperty.AddOwner(
            typeof(TextBoxView),
            new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.Inherits)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetTextAlignment((TextAlignment)newValue),
            });
    }

    internal TextBoxView(TextBox host)
        : base(host)
    {
    }

    internal new TextBox Host => (TextBox)base.Host;

    /// <inheritdoc />
    protected internal sealed override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        return INTERNAL_HtmlDomManager.CreateTextBoxViewDomElementAndAppendIt(parent, this);
    }

    protected sealed internal override void INTERNAL_OnAttachedToVisualTree()
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
        Host.UpdateTextProperty(GetText());
        InvalidateMeasure();
    }

    internal void OnSelectionChange() => Host.RaiseEvent(new RoutedEventArgs(TextBox.SelectionChangedEvent));

    internal void SetTextNative(string text)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            Interop.ExecuteJavaScriptVoid(
                $"osjs.setProp('{OuterDiv.Uid}','value',\"{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text)}\")");

            InvalidateMeasure();
        }
    }

    private void SetProperties()
    {
        TextBox host = Host;

        this.SetTextDecorations(host.TextDecorations);
        this.SetTextWrapping(host.TextWrapping);
        this.SetCaretColor(host.CaretBrush);

        if (host.IsReadOnly)
        {
            OuterDiv.SetAttribute("readonly", string.Empty);
        }

        int maxlength = host.MaxLength;
        if (maxlength > 0)
        {
            OuterDiv.SetAttribute("maxlength", maxlength);
        }

        // Disable spell check
        OuterDiv.SetAttribute("spellcheck", host.IsSpellCheckEnabled);

        // Set the "data-accepts-return" property (that we have invented) so that the
        // "KeyDown" and "Paste" JavaScript events can retrieve this value:
        OuterDiv.SetAttribute("data-acceptsreturn", host.AcceptsReturn);
        OuterDiv.SetAttribute("data-acceptstab", host.AcceptsTab);

        if (Interop.IsRunningInTheSimulator)
        {
            Interop.ExecuteJavaScriptVoidAsync($"osjs.textviewManager.handleKeyDownFromSimulator('{OuterDiv.Uid}')");
        }

        SetTextNative(host.Text);
    }

    internal void ProcessKeyDown(KeyEventArgs e)
    {
        if (!OuterDiv.IsConnected) return;

        if (TextViewManager.OnKeyDown(this, e))
        {
            e.Handled = true;
            e.Cancellable = false;
        }
    }

    internal void OnAcceptsReturnChanged(bool acceptsReturn)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            // Set the "data-accepts-return" property (that we have invented)
            // so that the "keydown" JavaScript event can retrieve this value:
            OuterDiv.SetAttribute("data-acceptsreturn", acceptsReturn);
        }
    }

    internal void OnTextWrappingChanged(TextWrapping textWrapping)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            this.SetTextWrapping(textWrapping);
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

    internal void OnTextDecorationsChanged(TextDecorationCollection tdc)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            this.SetTextDecorations(tdc);
        }
    }

    internal void OnIsReadOnlyChanged(bool isReadOnly)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            if (isReadOnly)
            {
                OuterDiv.SetAttribute("readonly", string.Empty);
            }
            else
            {
                OuterDiv.RemoveAttribute("readonly");
            }
        }
    }

    internal void OnIsSpellCheckEnabledChanged(bool isSpellCheckEnabled)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            OuterDiv.SetAttribute("spellcheck", isSpellCheckEnabled);
        }
    }

    internal void SetCaretBrush(Brush brush)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            this.SetCaretColor(brush);
        }
    }

    internal int SelectionStart
    {
        get
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
            {
                return TextViewManager.GetSelectionStart(this);
            }

            return 0;
        }
        set
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
            {
                TextViewManager.SetSelectionStart(this, value);
            }
        }
    }

    internal int SelectionLength
    {
        get
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
            {
                return TextViewManager.GetSelectionLength(this);
            }

            return 0;
        }
        set
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
            {
                TextViewManager.SetSelectionLength(this, value);
            }
        }
    }

    internal string SelectedText
    {
        get
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
            {
                return TextViewManager.GetSelectedText(this);
            }

            return string.Empty;
        }
        set
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
            {
                TextViewManager.SetSelectedText(this, value);
                
                Host.UpdateTextProperty(GetText());
                InvalidateMeasure();
            }
        }
    }

    internal void SetSelectionRange(int start, int end)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            Interop.ExecuteJavaScriptVoid(
                $"osjs.textviewManager.setSelectionRange('{OuterDiv.Uid}', {start.ToInvariantString()}, {end.ToInvariantString()})");
        }
    }

    private string GetText()
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv.IsConnected)
        {
            return Interop.ExecuteJavaScriptString($"osjs.getProp('{OuterDiv.Uid}','value')") ?? string.Empty;
        }

        return string.Empty;
    }

    protected sealed override Size MeasureContent(Size constraint)
    {
        (string whiteSpace, string overflowWrap) = UIElementHelpers.ToCssTextWrapping(Host.TextWrapping);

        return ParentWindow.TextMeasurementService.MeasureView(
            OuterDiv.Uid,
            whiteSpace,
            overflowWrap,
            constraint.Width,
            "M");
    }
}
