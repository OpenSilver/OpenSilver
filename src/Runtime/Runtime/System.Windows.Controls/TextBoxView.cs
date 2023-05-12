
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
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

#if MIGRATION
using System.Windows.Input;
#else
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal sealed class TextBoxView : FrameworkElement, ITextBoxView
    {
        private object _contentEditableDiv;
        private JavaScriptCallback _inputCallback;

        internal TextBoxView(TextBox host)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
        }

        internal TextBox Host { get; }

        internal object InputDiv => _contentEditableDiv;

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = AddContentEditableDomElement(parentRef, out domElementWhereToPlaceChildren);
            INTERNAL_InnerDomElement = _contentEditableDiv;
            return div;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            // Note about tabbing: In WPF and SL, the elements in the template other than the input
            // field can have focus but the input field will get any keypress not handled by them.
            // For example, you can set the focus on a button included in the template by clicking on
            // it and pressing space will cause a button press, but any character will be added to the
            // Text of the PasswordBox (but the button retains the focus).
            // WPF and SL are different in that in SL, every focusable control in the template can get
            // focus through tabbing while in WPF, the whole control is considered as a single tab stop
            // (tabbing into the PasswordBox will directly put the focus on the input element).
            // BUT in SL, the input will be first in tabOrder (unless maybe if tabIndex is specifically
            // set, I didn't try that) so if the template goes :
            //
            // <Stackpanel>
            //     <Button/>
            //     <ContentPresenter/>
            // </StackPanel>
            //
            // by tabbing it will go ContentPresenter first then the Button (example simplified without
            // the names, and also WPF and SL do not use a ContentPresenter but a ScrollViewer or
            // Decorator in which to put the input area).
            // In our case, tabbing will go through the elements accessible through tabbing, without
            // changing the order, and text will only be added when the <input> has focus. On click,
            // the focus will be redirected to the <input>, unless the click was on an element that
            // absorbs pointer events.

            UpdateDomText(Host.Text);

            if (FocusManager.GetFocusedElement() == Host)
            {
                INTERNAL_HtmlDomManager.SetFocusNative(_contentEditableDiv);
            }
        }

        public override void INTERNAL_AttachToDomEvents()
        {
            base.INTERNAL_AttachToDomEvents();

            _inputCallback = JavaScriptCallback.Create(OnInputNative, true);

            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
            string sInputCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_inputCallback);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($@"{sDiv}.addEventListener('input', {sInputCallback});");
        }

        public override void INTERNAL_DetachFromDomEvents()
        {
            base.INTERNAL_DetachFromDomEvents();

            _inputCallback?.Dispose();
            _inputCallback = null;
        }

        private void OnInputNative()
        {
            Host.UpdateTextProperty(GetText());
        }

        internal sealed override void AddEventListeners()
        {
            InputManager.Current.AddEventListeners(this, true);
        }

        internal bool OnKeyDownNative(object jsEventArgs)
        {
            if (_contentEditableDiv is not null)
            {
                string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                string sArgs = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(jsEventArgs);
                return OpenSilver.Interop.ExecuteJavaScriptBoolean($"document.textboxHelpers.onKeyDownNative({sElement}, {sArgs});");
            }

            return false;
        }

        internal sealed override UIElement KeyboardTarget => Host;

        internal override bool EnablePointerEventsCore => true;

        internal void OnAcceptsReturnChanged(bool acceptsReturn)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv != null)
            {
                //--- SIMULATOR ONLY: ---
                // Set the "data-accepts-return" property (that we have invented) so that the "keydown" JavaScript event can retrieve this value:
                INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript($@"
var element = document.getElementByIdSafe(""{((INTERNAL_HtmlDomElementReference)_contentEditableDiv).UniqueIdentifier}"");
element.setAttribute(""data-acceptsreturn"", ""{acceptsReturn.ToString().ToLower()}"");");
            }
        }

        internal void OnTextChanged(string text)
        {
            UpdateDomText(text);
        }

        internal void OnTextAlignmentChanged(TextAlignment alignment)
        {
            UpdateTextAlignment(INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this), alignment);
        }

        private static void UpdateTextAlignment(INTERNAL_HtmlDomStyleReference style, TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    style.textAlign = "center";
                    break;
                case TextAlignment.Left:
                    style.textAlign = "start";
                    break;
                case TextAlignment.Right:
                    style.textAlign = "end";
                    break;
                case TextAlignment.Justify:
                    style.textAlign = "justify";
                    break;
                default:
                    break;
            }
        }

        internal void OnTextWrappingChanged(TextWrapping textWrapping)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv != null)
            {
                TextBlock.ApplyTextWrapping(
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv),
                    textWrapping);
            }
        }

        internal void OnMaxLengthChanged(int maxLength)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv != null)
            {
                if (maxLength > 0)
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(_contentEditableDiv, "maxlength", maxLength);
                }
                else
                {
                    INTERNAL_HtmlDomManager.RemoveAttribute(_contentEditableDiv, "maxlength");
                }
            }
        }

#if MIGRATION
        internal void OnTextDecorationsChanged(TextDecorationCollection tdc)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
            {
                ApplyTextDecorations(INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv), tdc);
            }
        }

        private static void ApplyTextDecorations(INTERNAL_HtmlDomStyleReference cssStyle, TextDecorationCollection tdc)
        {
            cssStyle.textDecoration = tdc?.ToHtmlString() ?? string.Empty;
        }
#else
        internal void OnTextDecorationsChanged(TextDecorations? tdc)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
            {
                ApplyTextDecorations(INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv), tdc);
            }
        }

        private static void ApplyTextDecorations(INTERNAL_HtmlDomStyleReference cssStyle, TextDecorations? tdc)
        {
            cssStyle.textDecoration = TextDecorationsToHtmlString(tdc);
        }

        private static string TextDecorationsToHtmlString(TextDecorations? tdc)
        {
            if (tdc.HasValue)
            {
                switch (tdc.Value)
                {
                    case TextDecorations.OverLine:
                        return "overline";
                    case TextDecorations.Strikethrough:
                        return "line-through";
                    case TextDecorations.Underline:
                        return "underline";
                    case TextDecorations.None:
                    default:
                        return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
#endif

        internal void OnIsReadOnlyChanged(bool isReadOnly)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv != null)
            {
                if (isReadOnly)
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(_contentEditableDiv, "readonly", "''");
                }
                else
                {
                    INTERNAL_HtmlDomManager.RemoveAttribute(_contentEditableDiv, "readonly");
                }
            }
        }

        internal void OnIsSpellCheckEnabledChanged(bool isSpellCheckEnabled)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_contentEditableDiv, "spellcheck", isSpellCheckEnabled);
            }
        }

        internal int SelectionStart
        {
            get
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
                {
                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                    return OpenSilver.Interop.ExecuteJavaScriptInt32($"{sElement}.selectionStart;");
                }

                return 0;
            }
            set
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
                {
                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                    OpenSilver.Interop.ExecuteJavaScriptVoid($"{sElement}.selectionStart = {value.ToInvariantString()};");
                }
            }
        }

        internal int SelectionLength
        {
            get
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
                {
                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                    return OpenSilver.Interop.ExecuteJavaScriptInt32($"{sElement}.selectionEnd - {sElement}.selectionStart;");
                }

                return 0;
            }
            set
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
                {
                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                    OpenSilver.Interop.ExecuteJavaScriptVoid($"{sElement}.selectionEnd = {sElement}.selectionStart + {value.ToInvariantString()};");
                }
            }
        }

        internal string SelectedText
        {
            get
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
                {
                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                    return OpenSilver.Interop.ExecuteJavaScriptString(
                        $"{sElement}.value.substring({sElement}.selectionStart, {sElement}.selectionEnd);") ?? string.Empty;
                }

                return string.Empty;
            }
            set
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
                {
                    string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                    string sText = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(value);
                    OpenSilver.Interop.ExecuteJavaScriptVoid(
                        $"{sElement}.setRangeText({sText}, {sElement}.selectionStart, {sElement}.selectionEnd, 'end');");
                    Host.UpdateTextProperty(GetText());
                }
            }
        }

        internal void SetSelectionRange(int start, int end)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
            {
                string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"{sElement}.setSelectionRange({start.ToInvariantString()}, {end.ToInvariantString()});");
            }
        }

        private object AddContentEditableDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var contentEditableDiv = INTERNAL_HtmlDomManager.CreateTextBoxViewDomElementAndAppendIt(parentRef, this);
            var contentEditableDivStyle = contentEditableDiv.Style;

            _contentEditableDiv = contentEditableDiv;

            // Apply Host.TextDecorations
            ApplyTextDecorations(contentEditableDivStyle, Host.TextDecorations);

            // Apply Host.TextWrapping
            TextBlock.ApplyTextWrapping(contentEditableDivStyle, Host.TextWrapping);

            // Apply Host.TextAlignment
            UpdateTextAlignment(contentEditableDivStyle, Host.TextAlignment);

            if (Host.IsReadOnly)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_contentEditableDiv, "readonly", "''");
            }

            int maxlength = Host.MaxLength;
            if (maxlength > 0)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_contentEditableDiv, "maxlength", maxlength);
            }

            // Disable spell check
            INTERNAL_HtmlDomManager.SetDomElementAttribute(contentEditableDiv, "spellcheck", Host.IsSpellCheckEnabled);

            domElementWhereToPlaceChildren = contentEditableDiv;

            // ---- SIMULATOR ----
            string uid = contentEditableDiv.UniqueIdentifier;

            // Set the "data-accepts-return" property (that we have invented) so that the "KeyDown" and "Paste" JavaScript events can retrieve this value:
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript($@"
var element = document.getElementByIdSafe('{uid}');
element.setAttribute('data-acceptsreturn', '{Host.AcceptsReturn.ToString().ToLower()}');
element.setAttribute('data-acceptstab', '{Host.AcceptsTab.ToString().ToLower()}');");

            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                // Register the "keydown" javascript event:
                INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript($@"
var element_OutsideEventHandler = document.getElementByIdSafe(""{uid}"");
element_OutsideEventHandler.addEventListener('keydown', function(e) {{

    var element_InsideEventHandler = document.getElementByIdSafe(""{uid}""); // For some reason we need to get again the reference to the element.
    var acceptsReturn = element_InsideEventHandler.getAttribute(""data-acceptsreturn"");
    var maxLength = element_InsideEventHandler.getAttribute(""maxlength"");
    var acceptsTab = element_InsideEventHandler.getAttribute(""data-acceptstab"");

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
}}, false);");//comma added on purpose because we need to get maxLength somehow (see how we did for acceptsReturn).
            }

            return contentEditableDiv;
        }

        internal string GetText()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
            {
                string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                return OpenSilver.Interop.ExecuteJavaScriptString($"{sElement}.value;") ?? string.Empty;
            }

            return string.Empty;
        }

        private void UpdateDomText(string text)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv is not null)
            {
                string sElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"{sElement}.value = \"{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(text)}\";");

                // This is the case when the text is changed by typing.
                InvalidateMeasure();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)INTERNAL_OuterDomElement).UniqueIdentifier;
            Size textSize = INTERNAL_ParentWindow.TextMeasurementService.MeasureText(
                uniqueIdentifier,
                Host.TextWrapping == TextWrapping.NoWrap ? "pre" : "pre-wrap",
                Host.TextWrapping == TextWrapping.NoWrap ? string.Empty : "break-word",
                availableSize.Width,
                "M");
            return textSize;
        }
    }
}
