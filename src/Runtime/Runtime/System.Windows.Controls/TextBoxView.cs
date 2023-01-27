﻿
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
using System.Text.Json;
using System.Text.Json.Serialization;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

#if !MIGRATION
using Windows.Foundation;
using Windows.UI.Text;
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

            // the following methods were ignored before because _contentEditableDiv was not defined due
            // to the fact that we waited for the template to be made so we would know where to put it.
            // as a consequence, we call them here:
            OnAfterApplyHorizontalAlignmentAndWidth();
            OnAfterApplyVerticalAlignmentAndWidth();

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
        }

        protected override void OnAfterApplyHorizontalAlignmentAndWidth()
        {
            // It is important that the "ContentEditable" div has the same CSS size properties as the outer div, so that the overflow and scrolling work properly:
            if (_contentEditableDiv != null) //this can be true in the case where a template has been defined, in which case we wait for the template to be created before adding the text area.
            {
                var outerDomStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);
                var contentEditableStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv);
                if (Host.HorizontalContentAlignment == HorizontalAlignment.Stretch)
                {
                    contentEditableStyle.width = "100%";
                    contentEditableStyle.maxWidth = outerDomStyle.maxWidth;
                }
                else //it is top, bottom or center so we don't want to stretch, but we want it to be limited as much as the outerDomElement
                {
                    if (double.IsNaN(Width))
                    {
                        if (!double.IsNaN(MaxWidth))
                        {
                            contentEditableStyle.maxWidth = outerDomStyle.maxWidth;
                        }//else, neither Width or maxWidth are set so we let it be.
                    }
                    else
                    {
                        double contentEditableMaxWidth = Math.Max(0, Width - Host.BorderThickness.Left - Host.BorderThickness.Right);
                        contentEditableStyle.maxWidth = contentEditableMaxWidth.ToInvariantString() + "px";  //note: this might be incorrect as it does not take into consideration any padding, margin, or other elements that happens between outerDomElement and contentEditableDiv.
                    }
                }
            }
        }

        protected override void OnAfterApplyVerticalAlignmentAndWidth()
        {
            // It is important that the "ContentEditable" div has the same CSS size properties as the outer div, so that the overflow and scrolling work properly:
            if (_contentEditableDiv != null) //this can be true in the case where a template has been defined, in which case we wait for the template to be created before adding the text area.
            {
                var outerDomStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);
                var contentEditableStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv);
                if (Host.VerticalContentAlignment == VerticalAlignment.Stretch)
                {
                    contentEditableStyle.height = "100%";
                    contentEditableStyle.maxHeight = outerDomStyle.maxHeight;
                }
                else
                {
                    if (double.IsNaN(Height))
                    {
                        if (!double.IsNaN(MaxHeight))
                        {
                            contentEditableStyle.maxHeight = outerDomStyle.maxHeight;
                        }//else, neither Height or maxHeight are set so we let it be.
                    }
                    else
                    {
                        contentEditableStyle.maxHeight = outerDomStyle.height; //note: this might be incorrect as it does not take into consideration any padding or margin that happens between outerDomElement and contentEditableDiv.
                    }
                }
            }
        }

        internal sealed override NativeEventsManager CreateEventsManager()
        {
            return new NativeEventsManager(this, this, Host, true);
        }

        internal override object GetDomElementToSetContentString()
        {
            if (_contentEditableDiv != null)
            {
                return _contentEditableDiv;
            }
                
            return base.GetDomElementToSetContentString();
        }

        internal override bool EnablePointerEventsCore => true;

        internal void OnAcceptsReturnChanged(bool acceptsReturn)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) &&
                INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(_contentEditableDiv))
            {
                //--- SIMULATOR ONLY: ---
                // Set the "data-accepts-return" property (that we have invented) so that the "keydown" JavaScript event can retrieve this value:
                INTERNAL_HtmlDomManager.ExecuteJavaScript($@"
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
                    style.textAlign = "left";
                    break;
                case TextAlignment.Right:
                    style.textAlign = "right";
                    break;
                case TextAlignment.Justify:
                    style.textAlign = "justify";
                    break;
                default:
                    break;
            }
        }

        internal void OnTextWrappingChanged(TextWrapping tw)
        {
            if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(_contentEditableDiv))
            {
                switch (tw)
                {
                    case TextWrapping.NoWrap:
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv).whiteSpace = "nowrap";
                        break;

                    case TextWrapping.Wrap:
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv).whiteSpace = "pre-wrap";
                        //todo: once we find how to make the note work, apply the same thing to the TextBlock.
                        //Note: the following line would be useful to break the words when they are too long without spaces.
                        //      unfortunately, it only works in chrome.
                        //      The other browsers have wordBreak = "break-all" but that doesn't take into account the spaces to break the string.
                        //          it means it will break words in two when it could have gone to the next line before starting the word that overflows in the line.
                        //INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBox._contentEditableDiv).wordBreak = "break-word";
                        break;

                    default:
                        break;
                }
            }
        }

        internal void OnHorizontalScrollBarVisibilityChanged(ScrollBarVisibility scrollVisibility)
        {
            if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(_contentEditableDiv))
            {
                string value = ScrollBarVisibilityToHtmlString(scrollVisibility);
                if (value != null)
                {
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv).overflowX = value;
                }
            }
        }

        internal void OnVerticalScrollBarVisibilityChanged(ScrollBarVisibility scrollVisibility)
        {
            if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(_contentEditableDiv))
            {
                string value = ScrollBarVisibilityToHtmlString(scrollVisibility);
                if (value != null)
                {
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv).overflowY = value;
                }
            }
        }

        internal void OnMaxLengthChanged(int maxLength)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && 
                INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(_contentEditableDiv))
            {
                //--- SIMULATOR ONLY: ---
                // Set the "data-maxlength" property (that we have made up) so that the "keydown" JavaScript event can retrieve this value:
                INTERNAL_HtmlDomManager.ExecuteJavaScript($@"
var element = document.getElementByIdSafe(""{((INTERNAL_HtmlDomElementReference)_contentEditableDiv).UniqueIdentifier}"");
element.setAttribute(""data-maxlength"", ""{maxLength}"");");
            }
        }

#if MIGRATION
        internal void OnTextDecorationsChanged(TextDecorationCollection tdc)
        {
            string cssValue = tdc?.ToHtmlString() ?? string.Empty;
            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv).textDecoration = cssValue;
        }
#else
        internal void OnTextDecorationsChanged(TextDecorations? tdc)
        {
            string cssValue = TextDecorationsToHtmlString(tdc);
            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv).textDecoration = cssValue;
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
                string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"{sDiv}.setAttribute(\"contentEditable\", \"{(!isReadOnly).ToString().ToLower()}\");"
                );

                //--- SIMULATOR ONLY: ---
                OpenSilver.Interop.ExecuteJavaScriptFastAsync($@"
var element = document.getElementByIdSafe(""{((INTERNAL_HtmlDomElementReference)_contentEditableDiv).UniqueIdentifier}"");
element.setAttribute(""data-isreadonly"",""{isReadOnly.ToString().ToLower()}"");");
            }
        }

        internal void OnIsSpellCheckEnabledChanged(bool isSpellCheckEnabled)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && _contentEditableDiv != null)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_contentEditableDiv, "spellcheck", isSpellCheckEnabled);
            }
        }

        internal void NEW_GET_SELECTION(out int selectionStartIndex, out int selectionLength)
        {
            //todo: fix the that happens (at least in chrome) that makes the index returned be 0 when the caret is on the last line when it's empty
            //what I think happens: the range gives the index of the <br> in the childNodes of _contentEditableDiv which makes it not find the range.startContainer, which is actually _contentEditableDiv.
            //todo: (probably in the documant.getRangeStartAndEnd and document.getRangeGlobalStartAndEndIndexes methods), fix the bad count of characters in the simulator when copy/pasting a multiline text.

            if (_contentEditableDiv == null || !INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                selectionStartIndex = 0;
                selectionLength = 0;
                return;
            }

            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
            GlobalIndexes globalIndexes = JsonSerializer.Deserialize<GlobalIndexes>(
                OpenSilver.Interop.ExecuteJavaScriptString(
$@"(function(e){{
 var s = window.getSelection();
 var gi = {{}};
 if (s.rangeCount == 0) {{ gi.startIndex = 0; gi.endIndex = 0; }} 
 else {{ document.getRangeGlobalStartAndEndIndexes(e, true, 0, s.getRangeAt(0), gi); }}
 return JSON.stringify(gi);
}}({sDiv}))"));

            selectionStartIndex = globalIndexes.IsStartFound ? globalIndexes.StartIndex : 0;
            int selectionLastIndex = globalIndexes.IsEndFound ? globalIndexes.EndIndex : (globalIndexes.IsStartFound ? globalIndexes.StartIndex : 0);
            selectionLength = selectionLastIndex - selectionStartIndex;
        }

        private struct GlobalIndexes
        {
            [JsonPropertyName("startIndex")]
            public int StartIndex { get; set; }

            [JsonPropertyName("endIndex")]
            public int EndIndex { get; set; }

            [JsonPropertyName("isStartFound")]
            public bool IsStartFound { get; set; }

            [JsonPropertyName("isEndFound")]
            public bool IsEndFound { get; set; }
        }

        internal void NEW_SET_SELECTION(int startIndex, int endIndex)
        {
            if (Input.FocusManager.GetFocusedElement() != this.Host)
                return;

            if (_contentEditableDiv == null || !INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                return;

            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($@"
var sel = window.getSelection()
var nodesAndOffsets = {{}}; //this will hold the nodes and offsets useful to set the range's start and end.
document.getRangeStartAndEnd({sDiv}, true, 0, {startIndex.ToInvariantString()}, {endIndex.ToInvariantString()}, nodesAndOffsets, false, false)
sel.setBaseAndExtent(nodesAndOffsets['startParent'], nodesAndOffsets['startOffset'], nodesAndOffsets['endParent'], nodesAndOffsets['endOffset'])
");
        }

        internal int GetCaretPosition()
        {

            if (_contentEditableDiv == null || !INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                return 0;
            }

            return OpenSilver.Interop.ExecuteJavaScriptInt32(
                $"document.getCaretPosition({CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_contentEditableDiv)})");
        }

        private object AddContentEditableDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            bool isReadOnly = Host.IsReadOnly;

            var contentEditableDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out object contentEditableDiv);
            _contentEditableDiv = contentEditableDiv;

            contentEditableDivStyle.width = "100%";
            contentEditableDivStyle.height = "100%";

            // Apply Host.TextWrapping
            contentEditableDivStyle.whiteSpace = Host.TextWrapping == TextWrapping.NoWrap ? "nowrap" : "pre-wrap";

            // Apply Host.HorizontalScrollBarVisibility
            contentEditableDivStyle.overflowX = ScrollBarVisibilityToHtmlString(Host.HorizontalScrollBarVisibility) ?? "hidden";

            // Apply Host.VerticalScrollBarVisibility
            contentEditableDivStyle.overflowY = ScrollBarVisibilityToHtmlString(Host.VerticalScrollBarVisibility) ?? "hidden";
            
            contentEditableDivStyle.outline = "solid transparent"; // Note: this is to avoind having the weird border when it has the focus. I could have used outlineWidth = "0px" but or some reason, this causes the caret to not work when there is no text.
            contentEditableDivStyle.background = "solid transparent";
            contentEditableDivStyle.cursor = "text";

            // Disable spell check
            INTERNAL_HtmlDomManager.SetDomElementAttribute(contentEditableDiv, "spellcheck", this.Host.IsSpellCheckEnabled);

            // Apply TextAlignment
            UpdateTextAlignment(contentEditableDivStyle, Host.TextAlignment);

            string isContentEditable = (!isReadOnly).ToString().ToLower();
            INTERNAL_HtmlDomManager.SetDomElementAttribute(contentEditableDiv, "contentEditable", isContentEditable);
            this.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth = contentEditableDiv;

            contentEditableDivStyle.minWidth = "14px";
            contentEditableDivStyle.minHeight = (Math.Floor(this.Host.FontSize * 1.5 * 1000) / 1000).ToInvariantString() + "px"; // Note: We multiply by 1000 and then divide by 1000 so as to only keep 3 decimals at the most. //Note: setting "minHeight" is for FireFox only, because other browsers don't seem to need it. The "1.5" factor is here to ensure that the resulting Height is the same as that of the PasswordBox.

            domElementWhereToPlaceChildren = contentEditableDiv;

            // ---- SIMULATOR ----
            string uid = ((INTERNAL_HtmlDomElementReference)contentEditableDiv).UniqueIdentifier;

            // Set the "data-accepts-return" property (that we have invented) so that the "KeyDown" and "Paste" JavaScript events can retrieve this value:
            // also set the "data-maxlength" and the "data-isreadonly" 
            INTERNAL_HtmlDomManager.ExecuteJavaScript($@"
var element = document.getElementByIdSafe(""{uid}"");
element.setAttribute(""data-acceptsreturn"", ""{this.Host.AcceptsReturn.ToString().ToLower()}"");
element.setAttribute(""data-maxlength"", ""{this.Host.MaxLength}"");
element.setAttribute(""data-isreadonly"",""{isReadOnly.ToString().ToLower()}"");
element.setAttribute(""data-acceptstab"", ""{this.Host.AcceptsTab.ToString().ToLower()}"");");

            if (OpenSilver.Interop.IsRunningInTheSimulator)
            {
                // Register the "keydown" javascript event:
                INTERNAL_HtmlDomManager.ExecuteJavaScript($@"
var element_OutsideEventHandler = document.getElementByIdSafe(""{uid}"");
element_OutsideEventHandler.addEventListener('keydown', function(e) {{

    var element_InsideEventHandler = document.getElementByIdSafe(""{uid}""); // For some reason we need to get again the reference to the element.
    var acceptsReturn = element_InsideEventHandler.getAttribute(""data-acceptsreturn"");
    var maxLength = element_InsideEventHandler.getAttribute(""data-maxlength"");
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
        var text = getTextAreaInnerText(element_InsideEventHandler);
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

        instance.TextAreaValueChanged(); //todo: test this.
        e.preventDefault();
            return false;
    }}
}}, false);");//comma added on purpose because we need to get maxLength somehow (see how we did for acceptsReturn).
            }

            //-----------------------
            // Enforce only Plain Text + prevent line breaks if "AcceptsReturn" is false. This is required because in a ContentEditable div, the user can paste rich text. Furthermore, on IE, pressing Enter will insert a new paragraph.
            //-----------------------

            // The simulator uses Chrome, so we can set "ContentEditable" to plain-text only:
            // We still need to prevent prevent line breaks in the pasted text if "AcceptsReturn" is false:
            INTERNAL_HtmlDomManager.ExecuteJavaScript($@"
var element_OutsideEventHandler = document.getElementByIdSafe(""{uid}"");
element_OutsideEventHandler.addEventListener('paste', function(e) {{
    var element_InsideEventHandler = document.getElementByIdSafe(""{uid}""); // For some reason we need to get again the reference to the element.
    var isReadOnly= element_InsideEventHandler.getAttribute(""data-isreadonly"");
    if(isReadOnly !=""true"")
    {{
        var acceptsReturn = element_InsideEventHandler.getAttribute(""data-acceptsreturn"");
        var maxLength = element_InsideEventHandler.getAttribute(""data-maxlength"");
        element_InsideEventHandler.setAttribute(""contentEditable"", ""PLAINTEXT-ONLY"");
        if (acceptsReturn != ""true""){{
            e.preventDefault();
            var content = (e.originalEvent || e).clipboardData.getData('text/plain');
           if(content !== undefined){{
                content = content.replace(/\n/g, '').replace(/\r/g, '');
            }}
            if(maxLength != 0) {{
                var text = getTextAreaInnerText(element_InsideEventHandler);
                //var text = element_InsideEventHandler.innerText;
                if (!acceptsReturn) {{
                    text = text.replace(""\n"", """").replace(""\r"", """");
                }}

                var correctionDueToNewLines = 0;
                var textBoxTextLength = text.length + correctionDueToNewLines;
                var lengthComparison = maxLength - (content.length + textBoxTextLength);
                if(lengthComparison < 0) {{
                    content = content.substr(0, content.length+lengthComparison);
                }}
            }}
            document.execCommand('insertText', false, content);
        
        }}
    }}
}}, false);");

            return contentEditableDiv;
        }

        internal string GetText()
        {
            string text = INTERNAL_HtmlDomManager.GetTextBoxText(INTERNAL_InnerDomElement) ?? string.Empty;
            // This is the case when the text is changed by backspace or paste.
            InvalidateMeasure();
            return text;
        }

        private void UpdateDomText(string text)
        {
            if (INTERNAL_OuterDomElement != null &&
                Application.Current.TextMeasurementService.IsTextMeasureDivID(((INTERNAL_HtmlDomElementReference)INTERNAL_OuterDomElement).UniqueIdentifier))
            {
                if (_contentEditableDiv == null)
                    return;

                INTERNAL_HtmlDomManager.SetContentString(this, text);
                return;
            }

            if (_contentEditableDiv == null)
                return;

            INTERNAL_HtmlDomManager.SetContentString(this, Host.Text);

            // This is the case when the text is changed by typing.
            InvalidateMeasure();
        }

        private static string ScrollBarVisibilityToHtmlString(ScrollBarVisibility scrollVisibility)
        {
            switch (scrollVisibility)
            {
                case ScrollBarVisibility.Disabled:
                    return "hidden";
                case ScrollBarVisibility.Auto:
                    return "auto";
                case ScrollBarVisibility.Hidden:
                    return "hidden";
                case ScrollBarVisibility.Visible:
                    return "scroll";
                default:
                    return null;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement).UniqueIdentifier;
            Size TextSize = Application.Current.TextMeasurementService.MeasureTextBlock(uniqueIdentifier, Host.TextWrapping, Margin, availableSize.Width, "M");
            return TextSize;
        }
    }
}
