
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
    internal class TextBoxView : FrameworkElement, ITextBoxView
    {
        private object _contentEditableDiv;

        internal TextBoxView(TextBox host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            Host = host;
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

            OpenSilver.Interop.ExecuteJavaScript(@"$0.addEventListener('click', $1)", this.INTERNAL_OuterDomElement, (Action<object>)TextBoxView_GotFocus);

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
                if (!IsRunningInJavaScript())
                {
                    //--- SIMULATOR ONLY: ---
                    // Set the "data-accepts-return" property (that we have invented) so that the "keydown" JavaScript event can retrieve this value:
                    INTERNAL_HtmlDomManager.ExecuteJavaScript($@"
var element = document.getElementByIdSafe(""{((INTERNAL_HtmlDomElementReference)_contentEditableDiv).UniqueIdentifier}"");
element.setAttribute(""data-acceptsreturn"", ""{acceptsReturn.ToString().ToLower()}"");");
                }
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

#if OPENSILVER
        private static void UpdateTextAlignment(INTERNAL_HtmlDomStyleReference style, TextAlignment alignment)
#elif BRIDGE
        private static void UpdateTextAlignment(dynamic style, TextAlignment alignment)
#endif
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
                if (!IsRunningInJavaScript())
                {
                    //--- SIMULATOR ONLY: ---
                    // Set the "data-maxlength" property (that we have made up) so that the "keydown" JavaScript event can retrieve this value:
                    INTERNAL_HtmlDomManager.ExecuteJavaScript($@"
var element = document.getElementByIdSafe(""{((INTERNAL_HtmlDomElementReference)_contentEditableDiv).UniqueIdentifier}"");
element.setAttribute(""data-maxlength"", ""{maxLength}"");");
                }
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
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && 
                INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(_contentEditableDiv))
            {
                OpenSilver.Interop.ExecuteJavaScriptAsync(
                    "$0.setAttribute(\"contentEditable\", $1);",
                    _contentEditableDiv, (!isReadOnly).ToString().ToLower()
                );

                if (!IsRunningInJavaScript())
                {
                    //--- SIMULATOR ONLY: ---
                    INTERNAL_HtmlDomManager.ExecuteJavaScript($@"
var element = document.getElementByIdSafe(""{((INTERNAL_HtmlDomElementReference)_contentEditableDiv).UniqueIdentifier}"");
element.setAttribute(""data-isreadonly"",""{isReadOnly.ToString().ToLower()}"");");
                }
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

            var globalIndexes = OpenSilver.Interop.ExecuteJavaScript(@"
(function(domElement){
var sel = window.getSelection();
var globalIndexes = {}; //this will hold indexes of start and end and booleans that specify if each has been found.
if(sel.rangeCount == 0)
{
    globalIndexes.startIndex = 0; //apparently, we get 0 and not -1 when nothing is selected.
    globalIndexes.endIndex = 0; //apparently, we get 0 and not -1 when nothing is selected.
}
else
{
    var range = sel.getRangeAt(0);
    document.getRangeGlobalStartAndEndIndexes(domElement, true, 0, range, globalIndexes);
}
return globalIndexes;
}($0))
", _contentEditableDiv);

            selectionStartIndex = CastToInt(OpenSilver.Interop.ExecuteJavaScript("($0.isStartFound ? $0.startIndex : 0)", globalIndexes)); //todo: if not "isStartFound", should we raise an exception? (eg. running "STAR" app in the Simulator and clicking the TextBox in the "Products and key performance measures" screen)
            int selectionLastIndex = CastToInt(CSHTML5.Interop.ExecuteJavaScript("($0.isEndFound ? $0.endIndex : ($0.isStartFound ? $0.startIndex : 0))", globalIndexes)); //todo: if not "isEndFound", should we raise an exception? (eg. running "STAR" app in the Simulator and clicking the TextBox in the "Products and key performance measures" screen)
            selectionLength = selectionLastIndex - selectionStartIndex;
        }

        internal void NEW_SET_SELECTION(int startIndex, int endIndex)
        {
            if (Input.FocusManager.GetFocusedElement() != this.Host)
                return;

            if (_contentEditableDiv == null || !INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                return;

            OpenSilver.Interop.ExecuteJavaScriptAsync(@"
var sel = window.getSelection()
var nodesAndOffsets = {}; //this will hold the nodes and offsets useful to set the range's start and end.
document.getRangeStartAndEnd($0, true, 0, $1, $2, nodesAndOffsets, false, false)
sel.setBaseAndExtent(nodesAndOffsets['startParent'], nodesAndOffsets['startOffset'], nodesAndOffsets['endParent'], nodesAndOffsets['endOffset'])
", _contentEditableDiv, startIndex, endIndex);
        }

        internal int GetCaretPosition()
        {

            if (_contentEditableDiv == null || !INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                return 0;
            }

            var result = OpenSilver.Interop.ExecuteJavaScript(@"document.getCaretPosition($0)", _contentEditableDiv);

            return CastToInt(result);
        }

        private object AddContentEditableDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            bool isReadOnly = this.Host.IsReadOnly;
            object outerDiv;
            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);
            string backgroundColor = "transparent"; //value when it is templated

            outerDivStyle.backgroundColor = backgroundColor;
            outerDivStyle.height = "100%";

            object middleDiv;
            var middleDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out middleDiv);
            middleDivStyle.width = "100%";
            middleDivStyle.height = "100%";

            object contentEditableDiv;
            var contentEditableDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", middleDiv, this, out contentEditableDiv);
            _contentEditableDiv = contentEditableDiv;
            if (INTERNAL_HtmlDomManager.IsInternetExplorer())
            {
                //set the class to remove margins on <p> inside the contentEditableDiv
                OpenSilver.Interop.ExecuteJavaScript(@"$0.classList.add(""ie_set_p_margins_to_zero"")", contentEditableDiv);
            }

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

            // Apply TextAlignment
            UpdateTextAlignment(contentEditableDivStyle, Host.TextAlignment);

            string isContentEditable = (!isReadOnly).ToString().ToLower();
            INTERNAL_HtmlDomManager.SetDomElementAttribute(contentEditableDiv, "contentEditable", isContentEditable);
            this.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth = contentEditableDiv;

            contentEditableDivStyle.minWidth = "14px";
            contentEditableDivStyle.minHeight = (Math.Floor(this.Host.FontSize * 1.5 * 1000) / 1000).ToInvariantString() + "px"; // Note: We multiply by 1000 and then divide by 1000 so as to only keep 3 decimals at the most. //Note: setting "minHeight" is for FireFox only, because other browsers don't seem to need it. The "1.5" factor is here to ensure that the resulting Height is the same as that of the PasswordBox.

            // Fix for Internet Explorer: when pressing Enter in the ContentEditable div, IE will create a new paragraph, which results in graphical issues to the distance between paragraphs. To fix this issue, we put an empty DIV inside by default. When IE detects that there are DIVs inside, it adds a new DIV instead of creating a new paragraph when the user presses Enter.
            if (INTERNAL_HtmlDomManager.IsInternetExplorer())
            {
                object divToImproveIELineBreaks;
                var divToImproveIELineBreaksStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", contentEditableDiv, this, out divToImproveIELineBreaks);
            }

            domElementWhereToPlaceChildren = contentEditableDiv;

            //-----------------------
            // Prepare to raise the "TextChanged" event and to update the value of the "Text" property when the DOM text changes:
            //-----------------------
            //todo: why did we put this here instead of in INTERNAL_AttachToDomEvents?
            if (IsRunningOnInternetExplorer())
            {
                //-----------------------
                // Fix "input" event not working under IE:
                //-----------------------
                this.GotFocus += InternetExplorer_GotFocus;
                this.LostFocus += InternetExplorer_LostFocus;
                INTERNAL_EventsHelper.AttachToDomEvents("textinput", contentEditableDiv, (Action<object>)(e =>
                {
                    InternetExplorer_RaiseTextChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("paste", contentEditableDiv, (Action<object>)(e =>
                {
                    InternetExplorer_RaiseTextChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("cut", contentEditableDiv, (Action<object>)(e =>
                {
                    InternetExplorer_RaiseTextChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("keyup", contentEditableDiv, (Action<object>)(e =>
                {
                    InternetExplorer_RaiseTextChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("delete", contentEditableDiv, (Action<object>)(e =>
                {
                    InternetExplorer_RaiseTextChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("mouseup", contentEditableDiv, (Action<object>)(e =>
                {
                    InternetExplorer_RaiseTextChangedIfNecessary();
                }));
            }

            //-----------------------
            // Prevent pressing Enter for line break when "AcceptsReturn" is false and prevent pressing any other key than backspace when "MaxLength" is reached:
            // Also prevent the default event when pressing tab and add "\t" when "AcceptsTab" is set to true:
            //-----------------------
            if (IsRunningInJavaScript())
            {
#if OPENSILVER
                throw new InvalidOperationException();
#elif BRIDGE
                //BRIDGETODO : here the code below works weird
                // instance = this
                // makes instance working properly, where it shouldn't
                //Note: I think the reason why instance = this works is because it is set with the correct context of "this",
                //      so instance = this TextBox. If it was inside the function defined as callback of addEventListener,
                //      "this" would be in the context of the event triggered, so it would be the contentEditable div.
                Bridge.Script.Write(@"
var instance = $1;
$0.addEventListener('keydown', function(e) {
    if (e.keyCode == 13 && instance.AcceptsReturn !== true)
    {
        e.preventDefault();
        return false;
    }

    var isAddingTabulation = e.keyCode == 9 && instance.AcceptsTab == true;
    var isRemovingTabulation = isAddingTabulation && e.shiftKey;
    if(isRemovingTabulation)
    {
        isAddingTabulation = false
    }

    if((isAddingTabulation || e.keyCode == 13 || e.keyCode == 32 || e.keyCode > 47) && instance.MaxLength != 0)
    {
        var textLength = instance.GetTextLengthIncludingNewLineCompensation();

        if (e.keyCode == 13)
        {
            textLength += 1; //because adding a new line takes 2 characters instead of 1.
        }
        if(textLength >= instance.MaxLength)
        {
            e.preventDefault();
            return false;
        }
    }

    if(isAddingTabulation)
    {
        //we need to add '\t' where the cursor is, prevent the event (which would change the focus) and dispatch the event for the text changed:
        var sel, range;
        if (window.getSelection) {
            sel = window.getSelection();
            if (sel.rangeCount) {
                range = sel.getRangeAt(0);
                range.deleteContents();
                range.insertNode(document.createTextNode('\t'));
                sel.collapseToEnd();
                range.collapse(false); //for IE
            }
        } else if (document.selection && document.selection.createRange) {
            range = document.selection.createRange();
            range.text = '\t';
            document.selection.collapseToEnd();
        }
        if (window.IS_EDGE)
        {
            sel.removeAllRanges();
            sel.addRange(range);
        }

        instance.TextAreaValueChanged(); //todo: test this.
        e.preventDefault();
            return false;
    }
    if (isRemovingTabulation)
    {
        var sel, range;
        if (window.getSelection) {
            sel = window.getSelection();
            if (sel.rangeCount) {
                range = sel.getRangeAt(0);
                if(range.collapsed)
                {
                    //if the previous character is a '\t', we want to remove it:
                    var rangeStartContainer = range.startContainer;
                    var rangeStartOffset = range.startOffset;

                    //we get the node that contains the text that needs changing and the index of the character to remove:
                    var textNodeToModify = undefined;
                    var indexToRemove = -1;
                    if(rangeStartContainer.nodeType == Node.TEXT_NODE && rangeStartOffset > 0)
                    {
        
                        textNodeToModify = rangeStartContainer;
                        indexToRemove = rangeStartOffset - 1;
                    }
                    else //The potential tab to remove is in the node that is right before the selection(caret) so we look at the last character of the 'previous sibling':
                    {
                        var previousSibling = undefined;

                        // we get the previous node:
                        if(rangeStartContainer.nodeType == Node.TEXT_NODE) //the caret was right before the first element of a textNode. I am not sure if we can arrive here since this case seems to be considered as 'the caret is between the nodes' instead of 'the caret is in the node but before the first character'
                        {
                            previousSibling = rangeStartContainer.previousSibling;
                        }
                        else //the caret is considered between nodes so the range container is the element that contains the text nodes.
                        {
                            if(rangeStartOffset > 0) //otherwise, we are at the first character inside a new div (or p), which means at the start of a new line so nothing to remove.
                            {
                                previousSibling = rangeStartContainer.childNodes[rangeStartOffset -1];
                            }
                        }

                        //We update the node and index to modify:
                        if(previousSibling != undefined && previousSibling.nodeType == Node.TEXT_NODE)
                        {
                            textNodeToModify = previousSibling;
                            indexToRemove = previousSibling.textContent.length - 1;
                        } //else, the previous node was not a text node so there has to be a new line between the caret and the previous text so nothing to remove here.
                    }
        
                    if(textNodeToModify != undefined && indexToRemove != -1)
                    {
                        //range.startContainer.textContent = range.startContainer.textContent.substring(rangeStartOffset)
                        var textContent = textNodeToModify.textContent;
                        if(textContent[indexToRemove] == '\t')
                        {
                            var resultingString = textContent.substring(0, indexToRemove); //note: text.substring(0,-1) returns ""
                            resultingString += textContent.substring(indexToRemove + 1); //note: 'aaa'.substring(25) returns ""
                            textNodeToModify.textContent = resultingString;
                        }
                    }
    
                }
            }
        }
        //todo: else.
        instance.TextAreaValueChanged(); //todo: test this.
        e.preventDefault();
            return false;
    }

}, false);", contentEditableDiv, this);//AcceptsReturn, MaxLength
#endif
            }
#if OPENSILVER
            else
            {
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

#if CSHTML5BLAZOR
                if (OpenSilver.Interop.IsRunningInTheSimulator_WorkAround)
#else
                if (!IsRunningInJavaScript())
#endif
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
            }
#endif

            //-----------------------
            // Enforce only Plain Text + prevent line breaks if "AcceptsReturn" is false. This is required because in a ContentEditable div, the user can paste rich text. Furthermore, on IE, pressing Enter will insert a new paragraph.
            //-----------------------
            if (IsRunningInJavaScript())
            {
#if OPENSILVER
                throw new InvalidOperationException();
#elif BRIDGE
                var acceptsReturn = this.Host.AcceptsReturn;
                //todo: shouldn't we add a check for maxlength in the script below like in the other versions of this addEventListenter (in the simulator version below and in the #if !BRIDGE part above)?
                Bridge.Script.Write(@"
{0}.addEventListener('paste', function(e) {
    var isReadOnly= {1};
    if(!isReadOnly)
    {
        var isSingleLine = ({2} !== true); // This is the current value at the time when the event is raised.
        // Chrome supports setting ContentEditable to PlainText-Only, so we try this first:
        {0}.setAttribute('contenteditable', 'PLAINTEXT-ONLY');
        if ({0}.contentEditable === 'plaintext-only') // If setting the attribute worked, we can read it back (in lowercase)
        {
          // --- CHROME: ---
          // Nothing else to do about rich text conversion to plain text.
          // However we still need to remove line breaks if AcceptsReturn is false:
          if (isSingleLine){
            e.preventDefault();
            var content = (e.originalEvent || e).clipboardData.getData('text/plain');
            content = content.replace(/\n/g, '').replace(/\r/g, '');
            document.execCommand('insertText', false, content);
          }
    }
    else
        {
          {0}.setAttribute('contenteditable', 'true');
          if (e.clipboardData){

            // --- FIREFOX: ---
            e.preventDefault();
            var content = (e.originalEvent || e).clipboardData.getData('text/plain');
            if (isSingleLine){
              content = content.replace(/\n/g, '').replace(/\r/g, '');
            }
            document.execCommand('insertText', false, content);
          }
          else if (window.clipboardData){

            // --- INTERNET EXPLORER: ---
            e.preventDefault();
            var content = window.clipboardData.getData('Text');
            if (window.getSelection)
            {
              var newDiv = document.createElement('div');
              if (isSingleLine){
                content = content.replace(/\n/g, '').replace(/\r/g, '');
              }
              content = content.replace(/\r\n/g, '<br />');
              newDiv.innerHTML = content.replace(/\n/g, '<br />');
              var range = window.getSelection().getRangeAt(0);
              range.deleteContents()
              range.insertNode( newDiv );
              //window.getSelection().getRangeAt(0).insertNode( document.createTextNode(content) );
            }
          }
      }
            }
  
}, false);", contentEditableDiv, isReadOnly, acceptsReturn);
                //BRIDGETODO : check the code up
#endif
            }
#if OPENSILVER
            else
            {
                // ---- SIMULATOR ----
                string uid = ((INTERNAL_HtmlDomElementReference)contentEditableDiv).UniqueIdentifier;

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
            }
#endif

            return outerDiv;
        }

        internal string GetText()
        {
            string text = INTERNAL_HtmlDomManager.GetTextBoxText(INTERNAL_InnerDomElement) ?? string.Empty;
            if (!Host.AcceptsReturn)
            {
                // This is just in case the user managed to enter multi-line text in a single-line textbox.
                // It can happen (due to a bug) when pasting multi-line text under Interned Explorer 10.
                text = text.Replace('\n', '\0').Replace('\r', '\0');
            }

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
        }

#if BRIDGE
        private int GetTextLengthIncludingNewLineCompensation()
        {
            var text = INTERNAL_HtmlDomManager.GetTextBoxText(this.INTERNAL_InnerDomElement);
            if (!Host.AcceptsReturn)
            {
                text = text.Replace("\n", "").Replace("\r", "");
            }

            return text.Length; //this is just assuming but since INTERNAL_HtmlDomManager.GetTextBoxText makes sure all new lines are "\r\n" it should be correct.
        }
#endif

        private void TextBoxView_GotFocus(object e)
        {
            bool ignoreEvent = Convert.ToBoolean(OpenSilver.Interop.ExecuteJavaScript("document.checkForDivsThatAbsorbEvents($0)", e));
            if (!ignoreEvent)
            {
                if (_contentEditableDiv != null)
                {
                    OpenSilver.Interop.ExecuteJavaScript(@"
if($1.target != $0) {
$0.focus()
var range,selection;
    if(document.createRange)//Firefox, Chrome, Opera, Safari, IE 9+
    {
        range = document.createRange();//Create a range (a range is a like the selection but invisible)
        range.selectNodeContents($0);//Select the entire contents of the element with the range
        range.collapse(false);//collapse the range to the end point. false means collapse to end rather than the start
        selection = window.getSelection();//get the selection object (allows you to change selection)
        selection.removeAllRanges();//remove any selections already made
        selection.addRange(range);//make the range you have just created the visible selection
    }
    else if(document.selection)//IE 8 and lower
    { 
        range = document.body.createTextRange();//Create a range (a range is a like the selection but invisible)
        range.moveToElementText($0);//Select the entire contents of the element with the range
        range.collapse(false);//collapse the range to the end point. false means collapse to end rather than the start
        range.select();//Select the range (make it the visible selection
    }
}", _contentEditableDiv, e);
                }
            }
        }

#if BRIDGE
        private string previousInnerText = null;
#endif

        private void InternetExplorer_GotFocus(object sender, RoutedEventArgs e)
        {
#if BRIDGE
            previousInnerText = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("getTextAreaInnerText($0)", _contentEditableDiv));
#endif
        }

        private void InternetExplorer_LostFocus(object sender, RoutedEventArgs e)
        {
            InternetExplorer_RaiseTextChangedIfNecessary();
        }

        private void InternetExplorer_RaiseTextChangedIfNecessary()
        {
#if BRIDGE
            string newInnerText = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("getTextAreaInnerText($0)", _contentEditableDiv));
            if (newInnerText != previousInnerText)
            {
                TextAreaValueChanged();
                previousInnerText = newInnerText;
            }
#endif
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

#if BRIDGE
        [Bridge.Template("{value}")]
#endif
        private static int CastToInt(object value)
        {
            return Convert.ToInt32(value);
        }

#if BRIDGE
        [Bridge.Template("window.IE_VERSION")]
#endif
        private static bool IsRunningOnInternetExplorer()
        {
            return false;
        }

#if BRIDGE
        [Bridge.Template("true")]
#endif
        private static bool IsRunningInJavaScript()
        {
            return false;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement).UniqueIdentifier;
            Size TextSize = Application.Current.TextMeasurementService.MeasureTextBlock(uniqueIdentifier, Host.TextWrapping, Margin, availableSize.Width);
            return TextSize;
        }
    }
}
