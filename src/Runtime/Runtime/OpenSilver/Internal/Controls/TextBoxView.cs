
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
        TextElement.CharacterSpacingProperty.AddOwner(
            typeof(TextBoxView),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetCharacterSpacing((int)newValue),
            });

        TextElement.FontFamilyProperty.AddOwner(
            typeof(TextBoxView),
            new FrameworkPropertyMetadata(FontFamily.Default, FrameworkPropertyMetadataOptions.Inherits, OnFontFamilyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetFontFamily((FontFamily)newValue),
            });

        TextElement.FontSizeProperty.AddOwner(
            typeof(TextBoxView),
            new FrameworkPropertyMetadata(11d, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetFontSize((double)newValue),
            });

        TextElement.FontStyleProperty.AddOwner(
            typeof(TextBoxView),
            new FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetFontStyle((FontStyle)newValue),
            });

        TextElement.FontWeightProperty.AddOwner(
           typeof(TextBoxView),
           new FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
           {
               MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetFontWeight((FontWeight)newValue),
           });

        TextElement.ForegroundProperty.AddOwner(
            typeof(TextBoxView),
            new FrameworkPropertyMetadata(
                TextElement.ForegroundProperty.DefaultMetadata.DefaultValue,
                FrameworkPropertyMetadataOptions.Inherits,
                OnForegroundChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBoxView)d).SetForeground(oldValue as Brush, (Brush)newValue),
            });

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

        IsHitTestableProperty.OverrideMetadata(typeof(TextBoxView), new PropertyMetadata(BooleanBoxes.TrueBox));
    }

    private WeakEventListener<TextBoxView, Brush, EventArgs> _foregroundChangedListener;

    internal TextBoxView(TextBox host)
        : base(host)
    {
    }

    internal new TextBox Host => (TextBox)base.Host;

    public sealed override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
    {
        var textarea = INTERNAL_HtmlDomManager.CreateTextBoxViewDomElementAndAppendIt(parentRef, this);
        domElementWhereToPlaceChildren = textarea;
        return textarea;
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

    protected sealed override void OnInput()
    {
        Host.UpdateTextProperty(GetText());
        InvalidateMeasure();
    }

    internal void SetTextNative(string text)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            string sElement = Interop.GetVariableStringForJS(OuterDiv);
            Interop.ExecuteJavaScriptVoid(
                $"{sElement}.value = \"{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text)}\";");

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
            INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "readonly", string.Empty);
        }

        int maxlength = host.MaxLength;
        if (maxlength > 0)
        {
            INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "maxlength", maxlength);
        }

        // Disable spell check
        INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "spellcheck", host.IsSpellCheckEnabled);

        // Set the "data-accepts-return" property (that we have invented) so that the
        // "KeyDown" and "Paste" JavaScript events can retrieve this value:
        INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "data-acceptsreturn", host.AcceptsReturn);
        INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "data-acceptstab", host.AcceptsTab);

        if (Interop.IsRunningInTheSimulator)
        {
            // ---- SIMULATOR ----
            string uid = OuterDiv.UniqueIdentifier;

            // Register the "keydown" javascript event:
            Interop.ExecuteJavaScriptVoidAsync($@"
var element_OutsideEventHandler = document.getElementByIdSafe(""{uid}"");
element_OutsideEventHandler.addEventListener('keydown', function(e) {{

    var element_InsideEventHandler = document.getElementByIdSafe(""{uid}""); // For some reason we need to get again the reference to the element.
    var acceptsReturn = element_InsideEventHandler.getAttribute(""data-acceptsreturn"");
    var maxLength = element_InsideEventHandler.getAttribute(""maxlength"");
    var acceptsTab = element_InsideEventHandler.getAttribute(""data-acceptstab"");

    if (maxLength == null) maxLength = 0;
    if (e.keyCode == 13)
    {{
        if(acceptsReturn != ""true"")
        {{
            e.preventDefault();
            return false;
        }}
    }}

    var isAddingTabulation = e.keyCode == 9 && acceptsTab == ""true"";

    if((isAddingTabulation || e.keyCode == 13 || e.keyCode == 32 || e.keyCode > 47) && maxLength != 0)
    {{
        var text = element_InsideEventHandler.value;
        if (!acceptsReturn) {{
            text = text.replace(""\n"", """").replace(""\r"", """");
        }}

        var correctionDueToNewLines = 0;
        if (e.keyCode == 13)
        {{
            ++correctionDueToNewLines; //because adding a new line takes 2 characters instead of 1.
        }}
        if(text.length + correctionDueToNewLines >= maxLength)
        {{
            if (!window.getSelection().toString()) {{
                e.preventDefault();
                return false;
            }}
        }}
    }}

    if(isAddingTabulation)
    {{
        //we need to add '\t' where the cursor is, prevent the event (which would change the focus) and dispatch the event for the text changed:
        var sel, range;
        if (window.getSelection) {{
            sel = window.getSelection();
            if (sel.rangeCount) {{
                range = sel.getRangeAt(0);
                range.deleteContents();
                range.insertNode(document.createTextNode('\t'));
                sel.collapseToEnd();
                range.collapse(false); //for IE
            }}
        }} else if (document.selection && document.selection.createRange) {{
            range = document.selection.createRange();
            range.text = '\t';
            document.selection.collapseToEnd();
        }}
        if (window.IS_EDGE)
        {{
            sel.removeAllRanges();
            sel.addRange(range);
        }}

        e.preventDefault();
            return false;
    }}
}}, false)");//comma added on purpose because we need to get maxLength somehow (see how we did for acceptsReturn).
        }

        SetTextNative(host.Text);
    }

    internal void ProcessKeyDown(KeyEventArgs e)
    {
        if (OuterDiv is null) return;

        string sElement = Interop.GetVariableStringForJS(OuterDiv);
        string sArgs = Interop.GetVariableStringForJS(e.UIEventArg);
        if (Interop.ExecuteJavaScriptBoolean($"document.textviewManager.onKeyDownNative({sElement}, {sArgs});"))
        {
            e.Handled = true;
            e.Cancellable = false;
        }
    }

    internal void OnAcceptsReturnChanged(bool acceptsReturn)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv != null)
        {
            // Set the "data-accepts-return" property (that we have invented)
            // so that the "keydown" JavaScript event can retrieve this value:
            INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "data-acceptsreturn", acceptsReturn);
        }
    }

    internal void OnTextWrappingChanged(TextWrapping textWrapping)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv != null)
        {
            this.SetTextWrapping(textWrapping);
        }
    }

    internal void OnMaxLengthChanged(int maxLength)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv != null)
        {
            if (maxLength > 0)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "maxlength", maxLength);
            }
            else
            {
                INTERNAL_HtmlDomManager.RemoveAttribute(OuterDiv, "maxlength");
            }
        }
    }

    internal void OnTextDecorationsChanged(TextDecorationCollection tdc)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            this.SetTextDecorations(tdc);
        }
    }

    internal void OnIsReadOnlyChanged(bool isReadOnly)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv != null)
        {
            if (isReadOnly)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "readonly", string.Empty);
            }
            else
            {
                INTERNAL_HtmlDomManager.RemoveAttribute(OuterDiv, "readonly");
            }
        }
    }

    internal void OnIsSpellCheckEnabledChanged(bool isSpellCheckEnabled)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv != null)
        {
            INTERNAL_HtmlDomManager.SetDomElementAttribute(OuterDiv, "spellcheck", isSpellCheckEnabled);
        }
    }

    internal void SetCaretBrush(Brush brush)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            this.SetCaretColor(brush);
        }
    }

    internal int SelectionStart
    {
        get
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
            {
                string sElement = Interop.GetVariableStringForJS(OuterDiv);
                return Interop.ExecuteJavaScriptInt32($"document.textviewManager.getSelectionStart({sElement})");
            }

            return 0;
        }
        set
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
            {
                string sElement = Interop.GetVariableStringForJS(OuterDiv);
                Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectionStart({sElement}, {value.ToInvariantString()})");
            }
        }
    }

    internal int SelectionLength
    {
        get
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
            {
                string sElement = Interop.GetVariableStringForJS(OuterDiv);
                return Interop.ExecuteJavaScriptInt32($"document.textviewManager.getSelectionLength({sElement})");
            }

            return 0;
        }
        set
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
            {
                string sElement = Interop.GetVariableStringForJS(OuterDiv);
                Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectionLength({sElement}, {value.ToInvariantString()})");
            }
        }
    }

    internal string SelectedText
    {
        get
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
            {
                string sElement = Interop.GetVariableStringForJS(OuterDiv);
                return Interop.ExecuteJavaScriptString($"document.textviewManager.getSelectedText({sElement})");
            }

            return string.Empty;
        }
        set
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
            {
                string sElement = Interop.GetVariableStringForJS(OuterDiv);
                string sText = Interop.GetVariableStringForJS(value);
                Interop.ExecuteJavaScriptVoid($"document.textviewManager.setSelectedText({sElement}, {sText})");
                
                Host.UpdateTextProperty(GetText());
                InvalidateMeasure();
            }
        }
    }

    internal void SetSelectionRange(int start, int end)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            string sElement = Interop.GetVariableStringForJS(OuterDiv);
            Interop.ExecuteJavaScriptVoid(
                $"{sElement}.setSelectionRange({start.ToInvariantString()}, {end.ToInvariantString()});");
        }
    }

    private string GetText()
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && OuterDiv is not null)
        {
            string sElement = Interop.GetVariableStringForJS(OuterDiv);
            return Interop.ExecuteJavaScriptString($"{sElement}.value;") ?? string.Empty;
        }

        return string.Empty;
    }

    protected sealed override Size MeasureContent(Size constraint)
    {
        return ParentWindow.TextMeasurementService.MeasureView(
            OuterDiv.UniqueIdentifier,
            Host.TextWrapping == TextWrapping.NoWrap ? "pre" : "pre-wrap",
            Host.TextWrapping == TextWrapping.NoWrap ? string.Empty : "break-word",
            constraint.Width,
            "M");
    }

    private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UIElementHelpers.InvalidateMeasureOnFontFamilyChanged((TextBoxView)d, (FontFamily)e.NewValue);
    }

    private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (TextBoxView)d;

        if (view._foregroundChangedListener != null)
        {
            view._foregroundChangedListener.Detach();
            view._foregroundChangedListener = null;
        }

        if (e.NewValue is Brush newBrush)
        {
            view._foregroundChangedListener = new(view, newBrush)
            {
                OnEventAction = static (instance, sender, args) => instance.OnForegroundChanged(sender, args),
                OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
            };
            newBrush.Changed += view._foregroundChangedListener.OnEvent;
        }
    }

    private void OnForegroundChanged(object sender, EventArgs e)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        {
            var foreground = (Brush)sender; 
            this.SetForeground(foreground, foreground);
        }
    }
}
