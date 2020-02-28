

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Text;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using CSHTML5;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that can be used to display single-format, multi-line
    /// text.
    /// </summary>
    /// <example>
    /// <code lang="XAML" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    /// <TextBox x:Name="TextBoxName" Text="Some text"/>
    /// </code>
    /// <code lang="C#">
    ///     TextBoxName.Text = "Some text";
    /// </code>
    /// </example>
    public partial class TextBox : Control
    {
        object _contentEditableDiv;
        Control TextAreaContainer = null;

        /// <summary>
        /// The name of the ExpanderButton template part.
        /// </summary>
        private readonly string[] TextAreaContainerNames = { "ContentElement", "PART_ContentHost" };
        //                                                  Sl & UWP                WPF

        //static TextBox()
        //{
        //    Control.FontStyleProperty.OverrideMetadata(typeof(TextBox), new PropertyMetadata(FontStyle.Normal)
        //    {
        //        GetCSSEquivalent = (instance) =>
        //        {
        //            return new CSSEquivalent()
        //            {
        //                DomElement = ((TextBox)instance)._contentEditableDiv,
        //                Value = (inst, value) =>
        //                {
        //                    if (value != null)
        //                    {
        //                        return ((FontStyle)value).ToString().ToLower();
        //                    }
        //                    else
        //                    {
        //                        return "";
        //                    }
        //                },
        //                Name = new List<string> { "fontStyle" },
        //                ApplyAlsoWhenThereIsAControlTemplate = true // (See comment where this property is defined)
        //            };
        //        }
        //    }
        //    );
        //}
        ///
        public TextBox()
        {
            UseSystemFocusVisuals = true;
        }

        internal sealed override bool INTERNAL_GetFocusInBrowser
        {
            get { return true; }
        }

        #region AcceptsReturn

        /// <summary>
        /// Gets or sets the value that determines whether the text box allows and displays
        /// the newline or return characters.
        /// </summary>
        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }
        /// <summary>
        /// Identifies the AcceptsReturn dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptsReturnProperty =
            DependencyProperty.Register("AcceptsReturn", typeof(bool), typeof(TextBox), new PropertyMetadata(false, AcceptsReturn_Changed));
        private static void AcceptsReturn_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(textBox) && INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(textBox._contentEditableDiv))
            {
                if (e.NewValue != e.OldValue && e.NewValue is bool)
                {
                    bool acceptsReturn = (bool)e.NewValue;

                    if (!IsRunningInJavaScript())
                    {
                        //--- SIMULATOR ONLY: ---
                        // Set the "data-accepts-return" property (that we have invented) so that the "keydown" JavaScript event can retrieve this value:
                        INTERNAL_HtmlDomManager.ExecuteJavaScript(string.Format(@"
var element = document.getElementById(""{0}"");
element.setAttribute(""data-acceptsreturn"", ""{1}"");
", ((INTERNAL_HtmlDomElementReference)textBox._contentEditableDiv).UniqueIdentifier, acceptsReturn.ToString().ToLower()));
                    }
                }
            }
        }

        #endregion

        #region PlaceholderText

        /// <summary>
        /// Gets or sets the text that is displayed in the control until the value is changed by a user action or some other operation.
        /// </summary>
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }
        /// <summary>
        /// Identifies the PlaceholderText dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(TextBox), new PropertyMetadata(string.Empty) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        #endregion

        #region Text

        /// <summary>
        /// Gets or sets the text displayed in the TextBox.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBox), new PropertyMetadata(string.Empty, Text_Changed) { CoerceValueCallback = CoerceText, MethodToUpdateDom = UpdateDomText, CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });
        static void Text_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)d;
            if (!textBox._isUserChangingText)
            {
                //textBox._isCodeProgrammaticallyChangingText = true; // So that when the c# caller sets the text programmatically, it does not get set multiple times.
                //string newText = e.NewValue as string ?? string.Empty;
                //if (INTERNAL_VisualTreeManager.IsElementInVisualTree(textBox) && textBox._contentEditableDiv != null)
                //{
                //INTERNAL_HtmlDomManager.SetContentString(textBox, newText);
                textBox.OnTextChanged(new TextChangedEventArgs() { OriginalSource = textBox });
                //}
                //textBox._isCodeProgrammaticallyChangingText = false;
            }
        }

        private static void UpdateDomText(DependencyObject d, object newValue)
        {
            var textBox = (TextBox)d;
            if (!textBox._isUserChangingText)
            {
                string displayedText = INTERNAL_HtmlDomManager.GetTextBoxText(textBox.INTERNAL_InnerDomElement);
                string text = textBox.Text ?? string.Empty;
                if (displayedText != text)
                {
                    textBox._isCodeProgrammaticallyChangingText = true;
                    INTERNAL_HtmlDomManager.SetContentString(textBox, text);
                    textBox._isCodeProgrammaticallyChangingText = false;
                }
            }
        }

        internal override object GetDomElementToSetContentString()
        {
            if (_contentEditableDiv != null)
            {
                return _contentEditableDiv;
            }
            else
            {
                return base.GetDomElementToSetContentString();
            }
        }

        static object CoerceText(DependencyObject d, object value)
        {
            if (value == null)
            {
                return String.Empty;
            }
            return value;
        }

        #endregion

        #region TextAlignment

        /// <summary>
        /// Gets or sets how the text should be aligned in the text box.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        /// <summary>
        /// Identifies the TextAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TextBox), new PropertyMetadata(TextAlignment.Left) { MethodToUpdateDom = TextAlignment_MethodToUpdateDom });

        static void TextAlignment_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var textBlock = (TextBox)d;
            TextAlignment newTextAlignment;
            if (newValue is TextAlignment)
            {
                newTextAlignment = (TextAlignment)newValue;
            }
            else
            {
                newTextAlignment = TextAlignment.Left;
            }
            switch (newTextAlignment)
            {
                case TextAlignment.Center:
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(textBlock).textAlign = "center";
                    break;
                case TextAlignment.Left:
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(textBlock).textAlign = "left";
                    break;
                case TextAlignment.Right:
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(textBlock).textAlign = "right";
                    break;
                case TextAlignment.Justify:
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(textBlock).textAlign = "justify";
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region text changed event

        /// <summary>
        /// Occurs when the text is changed.
        /// </summary>
        public event TextChangedEventHandler TextChanged;

        /// <summary>
        /// Raises the TextChanged event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnTextChanged(TextChangedEventArgs eventArgs)
        {
            if (TextChanged != null)
            {
                TextChanged(this, eventArgs);
            }
        }

        #endregion

        #region CaretBrush

        /// <summary>
        /// Gets or sets the brush that is used to render the vertical bar that indicates
        /// the insertion point.
        /// </summary>
        public Brush CaretBrush
        {
            get { return (Brush)GetValue(CaretBrushProperty); }
            set { SetValue(CaretBrushProperty, value); }
        }
        /// <summary>
        /// Identify the CaretBrush dependency property
        /// </summary>
        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(TextBox), new PropertyMetadata(new SolidColorBrush(Colors.Black))
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Name = new List<String> { "caretColor", "colorAlpha" },
                        ApplyAlsoWhenThereIsAControlTemplate = false,
                    };
                }
            });
        #endregion

        public override void INTERNAL_AttachToDomEvents()
        {
            base.INTERNAL_AttachToDomEvents();
        }

        bool _isCodeProgrammaticallyChangingText = false;
        bool _isUserChangingText = false;
        private void TextAreaValueChanged()
        {
            if (!_isCodeProgrammaticallyChangingText)
            {
                //we get the value:
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    string text = INTERNAL_HtmlDomManager.GetTextBoxText(this.INTERNAL_InnerDomElement);
                    if (!AcceptsReturn) // This is just in case the user managed to enter multi-line text in a single-line textbox. It can happen (due to a bug) when pasting multi-line text under Interned Explorer 10.
                        text = text.Replace("\n", "").Replace("\r", "");
                    _isUserChangingText = true; //To prevent reentrance (infinite loop) when user types some text.
                    //Text = text;
                    SetLocalValue(TextProperty, text); //we call SetLocalvalue directly to avoid replacing the BindingExpression
                    _isUserChangingText = false;
                    OnTextChanged(new TextChangedEventArgs() { OriginalSource = this });
                }
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            // Note: we use 3 divs instead of 1 due to the way HorizontalAlignment/VerticalAlignment works in the FrameworkElement class. In fact, if "display" is "Table" or "TableCell", the "contentEditable" attribute does not work properly.
            return AddContentEditableDomElement(parentRef, out domElementWhereToPlaceChildren, false);
        }

        private object AddContentEditableDomElement(object parentRef, out object domElementWhereToPlaceChildren, bool isTemplated)
        {
            bool isReadOnly = this.IsReadOnly;
            object outerDiv;
            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);
            string backgroundColor = "transparent"; //value when it is templated
            if (!isTemplated)
            {
                outerDivStyle.borderColor = "#ABADB3";
                outerDivStyle.borderStyle = "solid";
                outerDivStyle.borderWidth = "1px";
                outerDivStyle.boxSizing = "border-box"; //this is so that the borderWidth we set does not increase the size of the whole thing.
                backgroundColor = isReadOnly ? "#DDDDDD" : "White";
            }
            else //if the TextBox is templated, we don't want contentEditable div to have a border:
            {
                outerDivStyle.borderWidth = "0px";
                outerDivStyle.width = "100%";
                outerDivStyle.height = "100%";
            }
            outerDivStyle.backgroundColor = backgroundColor;
            object middleDiv;
            var middleDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out middleDiv);
            middleDivStyle.width = "100%";
            middleDivStyle.height = "100%";

            object contentEditableDiv;
            var contentEditableDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", middleDiv, this, out contentEditableDiv);
            _contentEditableDiv = contentEditableDiv;
            contentEditableDivStyle.width = "100%";
            contentEditableDivStyle.height = "100%";
            contentEditableDivStyle.whiteSpace = "pre-wrap"; // Because by default we have decided to make the TextBox wrap, because the no-wrap mode does not work well (it enlarges the parent container, as of 2015.08.06)
            contentEditableDivStyle.overflowX = "hidden"; // Note: this is because the default HorizontalScrollBarVisibility in "Hidden"
            contentEditableDivStyle.overflowY = "hidden"; // Note: this is because the default VerticalScrollBarVisibility in "Hidden"
            if (isTemplated) //When templated, we remove the outlining that apears when the element has the focus:
            {
                contentEditableDivStyle.outline = "solid transparent"; // Note: this is to avoind having the weird border when it has the focus. I could have used outlineWidth = "0px" but or some reason, this causes the caret to not work when there is no text.
                contentEditableDivStyle.background = "solid transparent";
            }
            contentEditableDivStyle.cursor = "text";

            string isContentEditable = (!isReadOnly).ToString().ToLower();
            INTERNAL_HtmlDomManager.SetDomElementAttribute(contentEditableDiv, "contentEditable", isContentEditable);
            this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus = contentEditableDiv;
            this.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth = contentEditableDiv;

            contentEditableDivStyle.minWidth = "14px";
            contentEditableDivStyle.minHeight = (Math.Floor(this.FontSize * 1.5 * 1000) / 1000).ToString() + "px"; // Note: We multiply by 1000 and then divide by 1000 so as to only keep 3 decimals at the most. //Note: setting "minHeight" is for FireFox only, because other browsers don't seem to need it. The "1.5" factor is here to ensure that the resulting Height is the same as that of the PasswordBox.

            // Fix for Internet Explorer: when pressing Enter in the ContentEditable div, IE will create a new paragraph, which results in graphical issues to the distance between paragraphs. To fix this issue, we put an empty DIV inside by default. When IE detects that there are DIVs inside, it adds a new DIV instead of creating a new paragraph when the user presses Enter.
            if (INTERNAL_HtmlDomManager.IsInternetExplorer())
            {
                object divToImproveIELineBreaks;
                var divToImproveIELineBreaksStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", contentEditableDiv, this, out divToImproveIELineBreaks);
            }

            domElementWhereToPlaceChildren = contentEditableDiv;

            // Set the mark saying that the pointer events must be "absorbed" by the TextBox:
            INTERNAL_HtmlDomManager.SetDomElementProperty(outerDiv, "data-absorb-events", true);


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
            else
            {
                //-----------------------
                // Modern browsers
                //-----------------------
                INTERNAL_EventsHelper.AttachToDomEvents("input", contentEditableDiv, (Action<object>)(e =>
                {
                    TextAreaValueChanged();
                }));
            }


            //-----------------------
            // Prevent pressing Enter for line break when "AcceptsReturn" is false and prevent pressing any other key than backspace when "MaxLength" is reached:
            //-----------------------
            if (IsRunningInJavaScript())
            {
#if !BRIDGE
                //todo: handle the case where there is text that is selected (that's basically getting the selected range and if it is more than one character (or two if pushing on enter) accept the entry).
                JSIL.Verbatim.Expression(@"
var instance = this;
$0.addEventListener('keydown', function(e) {
    if (e.keyCode == 13 && instance['get_AcceptsReturn']() !== true)
    {
        e.preventDefault();
        return false;
    }

    if((e.keyCode == 13 || e.keyCode == 32 || e.keyCode > 47) && instance['get_MaxLength']() != 0)
    {
        var textLength = instance.GetTextLengthIncludingNewLineCompensation();

        if (e.keyCode == 13)
        {
            textLength += 1; //because adding a new line takes 2 characters instead of 1.
        }
        if(textLength >= instance['get_MaxLength']())
        {
            e.preventDefault();
            return false;
        }
    }
}, false);", contentEditableDiv);
#else
                //BRIDGETODO : here the code below works weird
                // instance = this
                // makes instance working properly, where it shouldn't
                //Note: I think the reason why instance = this works is because it is set with the correct context of "this",
                //      so instance = this TextBox. If it was inside the function defined as callback of addEventListener,
                //      "this" would be in the context of the event triggered, so it would be the contentEditable div.
                Script.Write(@"
var instance = $1;
$0.addEventListener('keydown', function(e) {
    if (e.keyCode == 13 && instance.AcceptsReturn !== true)
    {
        e.preventDefault();
        return false;
    }

    if((e.keyCode == 13 || e.keyCode == 32 || e.keyCode > 47) && instance.MaxLength != 0)
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
}, false);", contentEditableDiv, this);//AcceptsReturn, MaxLength
#endif
            }
#if !BRIDGE
            else
            {
                // ---- SIMULATOR ----

                // Set the "data-accepts-return" property (that we have invented) so that the "KeyDown" and "Paste" JavaScript events can retrieve this value:
                // also set the "data-maxlength" and the "data-isreadonly" 
                INTERNAL_HtmlDomManager.ExecuteJavaScript(string.Format(@"
var element = document.getElementById(""{0}"");
element.setAttribute(""data-acceptsreturn"", ""{1}"");
element.setAttribute(""data-maxlength"", ""{2}"");
element.setAttribute(""data-isreadonly"",""{3}"");
", ((INTERNAL_HtmlDomElementReference)contentEditableDiv).UniqueIdentifier, this.AcceptsReturn.ToString().ToLower(), this.MaxLength, isReadOnly.ToString().ToLower()));

                // Register the "keydown" javascript event:
                INTERNAL_HtmlDomManager.ExecuteJavaScript(string.Format(@"
var element_OutsideEventHandler = document.getElementById(""{0}"");
element_OutsideEventHandler.addEventListener('keydown', function(e) {{

    var element_InsideEventHandler = document.getElementById(""{0}""); // For some reason we need to get again the reference to the element.
    var acceptsReturn = element_InsideEventHandler.getAttribute(""data-acceptsreturn"");
    var maxLength = element_InsideEventHandler.getAttribute(""data-maxlength"");

    if (e.keyCode == 13)
    {{
        if(acceptsReturn != ""true"")
        {{
            e.preventDefault();
            return false;
        }}
    }}

    if((e.keyCode == 13 || e.keyCode == 32 || e.keyCode > 47) && maxLength != 0)
    {{
        var text = element_InsideEventHandler.innerText;

        if (!acceptsReturn) {{
            text = text.replace(""\n"", """").replace(""\r"", """");
        }}

        var correctionDueToNewLines = text.split(""\n"").length;
        --correctionDueToNewLines; //for n lines, we have n-1 ""\r\n""
            
        if (e.keyCode == 13)
        {{
            ++correctionDueToNewLines; //because adding a new line takes 2 characters instead of 1.
        }}
        if(text.length + correctionDueToNewLines >= maxLength)
        {{
            e.preventDefault();
            return false;
        }}
    }}
}}, false);
", ((INTERNAL_HtmlDomElementReference)contentEditableDiv).UniqueIdentifier));//comma added on purpose because we need to get maxLength somehow (see how we did for acceptsReturn).
            }
#endif

            //-----------------------
            // Enforce only Plain Text + prevent line breaks if "AcceptsReturn" is false. This is required because in a ContentEditable div, the user can paste rich text. Furthermore, on IE, pressing Enter will insert a new paragraph.
            //-----------------------
            if (IsRunningInJavaScript())
            {
#if !BRIDGE
                JSIL.Verbatim.Expression(@"
var instance = this;
$0.addEventListener('paste', function(e) {
    var isReadOnly= instance['get_IsReadOnly']();
    if(!isReadOnly)
    {
        var isSingleLine = (instance['get_AcceptsReturn']() !== true); // This is the current value at the time when the event is raised.
        var maxLength = instance['get_MaxLength']();
        var textBoxTextLength = instance.GetTextLengthIncludingNewLineCompensation();

        // Chrome supports setting ContentEditable to PlainText-Only, so we try this first:
        $0.setAttribute('contenteditable', 'PLAINTEXT-ONLY');
        if ($0.contentEditable === 'plaintext-only') // If setting the attribute worked, we can read it back (in lowercase)
        {
          // --- CHROME: ---
          // Nothing else to do about rich text conversion to plain text.
          // However we still need to remove line breaks if AcceptsReturn is false:
          if (isSingleLine){
            e.preventDefault();
            var content = (e.originalEvent || e).clipboardData.getData('text/plain');
            content = content.replace(/\n/g, '').replace(/\r/g, '');
            if(maxLength != 0) {
                var lengthComparison = maxLength - (content.length + textBoxTextLength);
                if(lengthComparison < 0) {
                    content = content.substr(0, content.length + lengthComparison);
                }
            }
            
            document.execCommand('insertText', false, content);
          }
        }
        else
        {
          $0.setAttribute('contenteditable', 'true');
          if (e.clipboardData){

            // --- FIREFOX: ---
            e.preventDefault();
            var content = (e.originalEvent || e).clipboardData.getData('text/plain');
            if (isSingleLine){
              content = content.replace(/\n/g, '').replace(/\r/g, '');
            }
            if(maxLength != 0) {
                var lengthComparison = maxLength - (content.length + textBoxTextLength);
                if(lengthComparison < 0) {
                    content = content.substr(0, content.length + lengthComparison);
                }
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
            if(maxLength != 0) {
                var lengthComparison = maxLength - (content.length + textBoxTextLength);
                if(lengthComparison < 0) {
                    content = content.substr(0, content.length + lengthComparison);
                }
            }
              newDiv.innerHTML = content.replace(/\n/g, '<br />');
              window.getSelection().getRangeAt(0).insertNode( newDiv );
              //window.getSelection().getRangeAt(0).insertNode( document.createTextNode(content) );
            }
          }
      }
    } 
  
}, false);", contentEditableDiv);
#else
                var acceptsReturn = this.AcceptsReturn;
                Script.Write(@"
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
              newDiv.innerHTML = content.replace(/\n/g, '<br />');
              window.getSelection().getRangeAt(0).insertNode( newDiv );
              //window.getSelection().getRangeAt(0).insertNode( document.createTextNode(content) );
            }
          }
      }
            }
  
}, false);", contentEditableDiv, isReadOnly, acceptsReturn);
                //BRIDGETODO : check the code up
#endif
            }
#if !BRIDGE
            else
            {
                // ---- SIMULATOR ----

                // The simulator uses Chrome, so we can set "ContentEditable" to plain-text only:
                // We still need to prevent prevent line breaks in the pasted text if "AcceptsReturn" is false:
                INTERNAL_HtmlDomManager.ExecuteJavaScript(string.Format(@"
var element_OutsideEventHandler = document.getElementById(""{0}"");
element_OutsideEventHandler.addEventListener('paste', function(e) {{
    var element_InsideEventHandler = document.getElementById(""{0}""); // For some reason we need to get again the reference to the element.
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
                var text = element_InsideEventHandler.innerText;
                if (!acceptsReturn) {{
                    text = text.replace(""\n"", """").replace(""\r"", """");
                }}
                var correctionDueToNewLines = text.split(""\n"").length;
                --correctionDueToNewLines; //for n lines, we have n-1 ""\r\n""
                var textBoxTextLength = text.length + correctionDueToNewLines;
                var lengthComparison = maxLength - (content.length + textBoxTextLength);
                if(lengthComparison < 0) {{
                    content = content.substr(0, content.length+lengthComparison);
                }}
            }}
            document.execCommand('insertText', false, content);
        
        }}
    }}
    

}}, false);
", ((INTERNAL_HtmlDomElementReference)contentEditableDiv).UniqueIdentifier));
            }
#endif

            if (isTemplated)
            {
                //the following methods were ignored before because _contentEditableDiv was not defined due to the fact that we waited for the template to be made so we would know where to put it.
                //as a consequence, we call them here:
                OnAfterApplyHorizontalAlignmentAndWidth();
                OnAfterApplyVerticalAlignmentAndWidth();

                //register to focusin events on the OuterDomElement so that we can "reroute" the focus on the contentEditable element.
                CSHTML5.Interop.ExecuteJavaScript(@"$0.tabIndex = 32767; $0.addEventListener('focusin', $1)", this.INTERNAL_OuterDomElement, (Action<object>)TextBox_GotFocus);
                //Note on the line above: 32767 is the maximum value commonly allowed in browsers (and can be considered the default value)
                if (INTERNAL_HtmlDomManager.IsInternetExplorer())
                {
                    //workaround due to IE setting the focus at the end of the click, (or at least after the focusin event), which cancels the work done during focusin.
                    //if I'm not mistaken, the click event happens even when we click on a child of the element so we're all good. (We had to fix the case where a Button had a TextBox inside of it, which is why I assumed that).
                    CSHTML5.Interop.ExecuteJavaScript(@"$0.addEventListener('click', $1)", this.INTERNAL_OuterDomElement, (Action<object>)TextBox_GotFocus);
                }
            }
            return outerDiv;
        }

        private int GetTextLengthIncludingNewLineCompensation()
        {
            var text = INTERNAL_HtmlDomManager.GetTextBoxText(this.INTERNAL_InnerDomElement);
            if (!AcceptsReturn)
            {
                text = text.Replace("\n", "").Replace("\r", "");
            }
            //var correctionDueToNewLines = text.Split('\n').Length;
            //--correctionDueToNewLines; //for n lines, we have n-1 ""\r\n""

            //if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(Interop.ExecuteJavaScript("window.chrome")) && correctionDueToNewLines != 0)
            //{
            //    --correctionDueToNewLines; //on chrome, we have a \n right at the end for some reason.
            //}
            //else if (INTERNAL_HtmlDomManager.IsInternetExplorer() || INTERNAL_HtmlDomManager.IsEdge())
            //{
            //    correctionDueToNewLines *= 2; //IE already has 2 characters for new lines but they are doubled: we have ""\r\n\r\n"" instead of ""\r\n"".
            //}
            //return text.Length + correctionDueToNewLines;
            return text.Length; //this is just assuming but since INTERNAL_HtmlDomManager.GetTextBoxText makes sure all new lines are "\r\n" it should be correct.
        }

        void TextBox_GotFocus(object e)//object sender, RoutedEventArgs e)
        {
            if (_contentEditableDiv != null)
            {
                CSHTML5.Interop.ExecuteJavaScript(@"
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
                //NEW_SET_SELECTION(_tempSelectionStartIndex, _tempSelectionStartIndex + _tempSelectionLength);
            }
        }


        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.TextBox" /> control when a new
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            TextAreaContainer = null; //set it to null so we don't keep the old Template's container for the TextArea.

            int i = 0;
            while (TextAreaContainer == null && i < TextAreaContainerNames.Length)
            {
                TextAreaContainer = GetTemplateChild(TextAreaContainerNames[i]) as Control;
                ++i;
            }
            if (TextAreaContainer != null)
            {
                object domElementWheretoPlaceChildren; //I believe we can basically ignore this one as TextBox shouldn't have any Content (only the text).
                AddContentEditableDomElement(TextAreaContainer.INTERNAL_InnerDomElement, out domElementWheretoPlaceChildren, true);
                //AddTextAreaToVisualTree(TextAreaContainer.INTERNAL_InnerDomElement, out domElementWheretoPlaceChildren);
                //Remember that the InnerDomElement is now the _contentEditableDiv rather than what was created to contain the template (Note: we need to do this because the _contentEditableDiv was added outside of the usual place we usually set the innerdomElement).
                INTERNAL_InnerDomElement = _contentEditableDiv;
                string text = Text; //this is probably more efficient than to use the property itself on 3 occasions.
                if (!string.IsNullOrEmpty(text))
                {
                    Text_Changed(this, new DependencyPropertyChangedEventArgs(text, text, TextProperty));
                }
            }
        }

        #region Fix "input" event not working under IE.

        string previousInnerText = null;

        void InternetExplorer_GotFocus(object sender, RoutedEventArgs e)
        {
#if !CSHTML5NETSTANDARD //todo: fixme
            previousInnerText = this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus.innerText;
#endif
        }

        void InternetExplorer_LostFocus(object sender, RoutedEventArgs e)
        {
            InternetExplorer_RaiseTextChangedIfNecessary();
        }

        void InternetExplorer_RaiseTextChangedIfNecessary()
        {
#if !CSHTML5NETSTANDARD //todo: fixme
            string newInnerText = this.INTERNAL_OptionalSpecifyDomElementConcernedByFocus.innerText;
            if (newInnerText != previousInnerText)
            {
                TextAreaValueChanged();
                previousInnerText = newInnerText;
            }
#endif
        }

        #endregion

#if !BRIDGE
        [JSReplacement("window.IE_VERSION")]
#else
        [Template("window.IE_VERSION")]
#endif
        static bool IsRunningOnInternetExplorer()
        {
            return false;
        }

#if !BRIDGE
        [JSReplacement("true")]
#else
        [Template("true")]
#endif
        static bool IsRunningInJavaScript()
        {
            return false;
        }

        #region Text Selection


        public int SelectionStart
        {
            get
            {
                int selectionStartIndex; int selectionLength;
                NEW_GET_SELECTION(out selectionStartIndex, out selectionLength);
                return selectionStartIndex;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("SelectionStart cannot be lower than 0");
                }
                RefreshSelection(value, SelectionLength);
            }
        }


        private void RefreshSelection(int selectionStart, int selectionLength)
        {
            NEW_SET_SELECTION(selectionStart, selectionStart + selectionLength);
        }

        private void NEW_SET_SELECTION(int startIndex, int endIndex)
        {
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
var range = document.createRange();
var sel = window.getSelection();
var nodesAndOffsets = {}; //this will hold the nodes and offsets useful to set the range's start and end.
document.getRangeStartAndEnd($0, true, 0, $1, $2, nodesAndOffsets, false, false)
range.setStart(nodesAndOffsets['startParent'], nodesAndOffsets['startOffset']);
range.setEnd(nodesAndOffsets['endParent'], nodesAndOffsets['endOffset']);
sel.removeAllRanges();
sel.addRange(range);
", _contentEditableDiv, startIndex, endIndex);
        }

        private void NEW_GET_SELECTION(out int selectionStartIndex, out int selectionLength)
        {
            //todo: fix the that happens (at least in chrome) that makes the index returned be 0 when the caret is on the last line when it's empty
            //what I think happens: the range gives the index of the <br> in the childNodes of _contentEditableDiv which makes it not find the range.startContainer, which is actually _contentEditableDiv.
            //todo: (probably in the documant.getRangeStartAndEnd and document.getRangeGlobalStartAndEndIndexes methods), fix the bad count of characters in the simulator when copy/pasting a multiline text.

            var globalIndexes = CSHTML5.Interop.ExecuteJavaScript(@"
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

            selectionStartIndex = CastToInt(CSHTML5.Interop.ExecuteJavaScript("($0.isStartFound ? $0.startIndex : 0)", globalIndexes)); //todo: if not "isStartFound", should we raise an exception? (eg. running "STAR" app in the Simulator and clicking the TextBox in the "Products and key performance measures" screen)
            selectionLength = CastToInt(CSHTML5.Interop.ExecuteJavaScript("($0.isEndFound ? $0.endIndex : ($0.isStartFound ? $0.startIndex : 0))", globalIndexes)); //todo: if not "isEndFound", should we raise an exception? (eg. running "STAR" app in the Simulator and clicking the TextBox in the "Products and key performance measures" screen)
        }

#if !BRIDGE
        [JSReplacement("$value")]
#else
        [Template("{value}")]
#endif
        static int CastToInt(object value) //Bridge : must be static to work properly
        {
            return Convert.ToInt32(((CSHTML5.Types.INTERNAL_JSObjectReference)value).Value);
        }

        public int SelectionLength
        {
            get
            {
                int selectionStartIndex; int selectionLength;
                NEW_GET_SELECTION(out selectionStartIndex, out selectionLength);
                return selectionLength;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("SelectionLength cannot be lower than 0");
                }
                RefreshSelection(SelectionStart, value);
            }
        }


        //todo: SelectedText

        /// <summary>
        /// Selects all text in the text box.
        /// </summary>
        public void SelectAll()
        {
            this.SelectionStart = 0;
            this.SelectionLength = this.Text.Length;
        }

        #endregion

        #region TextWrapping

        /// <summary>
        /// Gets or sets how the TextBow wraps text.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }
        /// <summary>
        /// Identifies the TextWrapping dependency property.
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextBox), new PropertyMetadata(
                TextWrapping.Wrap) // Note: we have made "Wrap" the default value because the no-wrap mode does not work well (it enlarges the parent container, as of 2015.08.06)
            {
                MethodToUpdateDom = TextWrapping_MethodToUpdateDom
            });

        static void TextWrapping_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var textBox = (TextBox)d;
            TextWrapping newTextWrapping;
            if (newValue is TextWrapping)
            {
                newTextWrapping = (TextWrapping)newValue;
            }
            else
            {
                newTextWrapping = (TextWrapping)TextWrappingProperty.GetTypeMetaData(typeof(TextBox)).DefaultValue; // Note: "TypeMetadata" is not null because declared above.
            }
            if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(textBox._contentEditableDiv))
            {
                switch (newTextWrapping)
                {
                    case TextWrapping.NoWrap:
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBox._contentEditableDiv).whiteSpace = "nowrap";
                        break;
                    case TextWrapping.Wrap:
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBox._contentEditableDiv).whiteSpace = "normal";
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

        #endregion

        #region HorizontalScrollBarVisibility

        /// <summary>
        /// Gets or sets a value that indicates whether a horizontal ScrollBar should
        /// be displayed.
        /// 
        /// Returns a ScrollBarVisibility value that indicates whether a horizontal ScrollBar
        /// should be displayed. The default value is Hidden.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the HorizontalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(TextBox), new PropertyMetadata(
                ScrollBarVisibility.Hidden)
            {
                MethodToUpdateDom = HorizontalScrollBarVisibility_MethodToUpdateDom
            });

        static void HorizontalScrollBarVisibility_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var textBox = (TextBox)d;
            ScrollBarVisibility newVisibility = (ScrollBarVisibility)newValue;

            if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(textBox._contentEditableDiv))
            {
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBox._contentEditableDiv);

                switch (newVisibility)
                {
                    case ScrollBarVisibility.Disabled:
                        style.overflowX = "hidden";
                        break;
                    case ScrollBarVisibility.Auto:
                        style.overflowX = "auto";
                        break;
                    case ScrollBarVisibility.Hidden:
                        style.overflowX = "hidden";
                        break;
                    case ScrollBarVisibility.Visible:
                        style.overflowX = "scroll";
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region VerticalScrollBarVisibility

        /// <summary>
        /// Gets or sets a value that indicates whether a vertical ScrollBar should be displayed.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the VerticalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(TextBox), new PropertyMetadata(
                ScrollBarVisibility.Hidden)
            {
                MethodToUpdateDom = VerticalScrollBarVisibility_MethodToUpdateDom
            });

        static void VerticalScrollBarVisibility_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var textBox = (TextBox)d;
            ScrollBarVisibility newVisibility = (ScrollBarVisibility)newValue;

            if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(textBox._contentEditableDiv))
            {
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBox._contentEditableDiv);

                switch (newVisibility)
                {
                    case ScrollBarVisibility.Disabled:
                        style.overflowY = "hidden";
                        break;
                    case ScrollBarVisibility.Auto:
                        style.overflowY = "auto";
                        break;
                    case ScrollBarVisibility.Hidden:
                        style.overflowY = "hidden";
                        break;
                    case ScrollBarVisibility.Visible:
                        style.overflowY = "scroll";
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the value that determines the maximum number of characters allowed
        /// for user input.
        /// </summary>
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxLength dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBox), new PropertyMetadata(0, MaxLength_Changed));
        //Note: Setting MaxLength only keeps the user from typing any more characters when the count is reached. It does NOT remove any character that are already typed in.
        //      It also does NOT keep the text from being programmatically changed (so you can programmatically set the Text property to a longer text than normally authorized).
        //      if acceptsReturn is true, the new lines count as 2 characters.
        private static void MaxLength_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(textBox) && INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(textBox._contentEditableDiv))
            {
                if (e.NewValue != e.OldValue && e.NewValue is int)
                {
                    int maxlength = (int)e.NewValue;

                    if (!IsRunningInJavaScript())
                    {
                        //--- SIMULATOR ONLY: ---
                        // Set the "data-maxlength" property (that we have made up) so that the "keydown" JavaScript event can retrieve this value:
                        INTERNAL_HtmlDomManager.ExecuteJavaScript(string.Format(@"
var element = document.getElementById(""{0}"");
element.setAttribute(""data-maxlength"", ""{1}"");
", ((INTERNAL_HtmlDomElementReference)textBox._contentEditableDiv).UniqueIdentifier, maxlength));
                    }
                }
            }
        }


        #region TextDecorations
#if MIGRATION
        /// <summary>
        /// Gets or sets the text decorations (underline, strikethrough...).
        /// </summary>
        public new TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }
        /// <summary>
        /// Identifies the TextDecorations dependency property.
        /// </summary>
        public new static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register("TextDecorations",
                                                                                                        typeof(TextDecorationCollection),
                                                                                                        typeof(TextBox),
                                                                                                        new PropertyMetadata(null) { MethodToUpdateDom = TextDecorations_MethodToUpdateDom });

        static void TextDecorations_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var textBox = (TextBox)d;
            TextDecorationCollection newTextDecorations = (TextDecorationCollection)newValue;

            string cssValue;
            if (newTextDecorations == System.Windows.TextDecorations.OverLine)
            {
                cssValue = "overline";
            }
            else if (newTextDecorations == System.Windows.TextDecorations.Strikethrough)
            {
                cssValue = "line-through";
            }
            else if (newTextDecorations == System.Windows.TextDecorations.Underline)
            {
                cssValue = "underline";
            }
            else
            {
                cssValue = string.Empty; // Note: this will reset the value.
            }
            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBox.INTERNAL_OptionalSpecifyDomElementConcernedByFocus).textDecoration = cssValue;
        }
#else
        /// <summary>
        /// Gets or sets the text decorations (underline, strikethrough...).
        /// </summary>
        public new TextDecorations? TextDecorations
        {
            get { return (TextDecorations?)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }
        /// <summary>
        /// Identifies the TextDecorations dependency property.
        /// </summary>
        public new static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register("TextDecorations", typeof(TextDecorations?), typeof(TextBox), new PropertyMetadata(null) { MethodToUpdateDom = TextDecorations_MethodToUpdateDom });

        static void TextDecorations_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var textBox = (TextBox)d;
            TextDecorations? newTextDecorations = (TextDecorations?)newValue;

            string cssValue;
            if (newTextDecorations.HasValue)
            {
                switch (newTextDecorations.Value)
                {
                    case Windows.UI.Text.TextDecorations.OverLine:
                        cssValue = "overline";
                        break;
                    case Windows.UI.Text.TextDecorations.Strikethrough:
                        cssValue = "line-through";
                        break;
                    case Windows.UI.Text.TextDecorations.Underline:
                        cssValue = "underline";
                        break;
                    case Windows.UI.Text.TextDecorations.None:
                    default:
                        cssValue = ""; // Note: this will reset the value.
                        break;
                }
            }
            else
            {
                cssValue = ""; // Note: this will reset the value.
            }
            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(textBox.INTERNAL_OptionalSpecifyDomElementConcernedByFocus).textDecoration = cssValue;
        }
#endif
        #endregion


        protected override void OnAfterApplyHorizontalAlignmentAndWidth()
        {
            // It is important that the "ContentEditable" div has the same CSS size properties as the outer div, so that the overflow and scrolling work properly:
            if (_contentEditableDiv != null) //this can be true in the case where a template has been defined, in which case we wait for the template to be created before adding the text area.
            {
                var outerDomStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);
                var contentEditableStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_contentEditableDiv);
                if (TextAreaContainer != null && HorizontalContentAlignment == HorizontalAlignment.Stretch)
                {
                    contentEditableStyle.width = "100%";
                    contentEditableStyle.maxWidth = outerDomStyle.maxWidth;
                }
                else //it is top, bottom or center so we don't want to stretch, but we want it to be limited as much as the outerDomElement
                {
                    double maxWidth = Width;
                    if (double.IsNaN(Width))
                    {
                        if (!double.IsNaN(MaxWidth))
                        {
                            contentEditableStyle.maxWidth = outerDomStyle.maxWidth;
                        }//else, neither Width or maxWidth are set so we let it be.
                    }
                    else
                    {
                        contentEditableStyle.maxWidth = outerDomStyle.width;  //note: this might be incorrect as it does not take into consideration any padding, margin, or other elements that happens between outerDomElement and contentEditableDiv.
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
                if (TextAreaContainer != null && VerticalContentAlignment == VerticalAlignment.Stretch)
                {
                    contentEditableStyle.height = "100%";
                    contentEditableStyle.maxHeight = outerDomStyle.maxHeight;
                }
                else
                {
                    double maxHeight = Height;
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



        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextBox), new PropertyMetadata(false, IsReadOnly_changed));

        private static void IsReadOnly_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(textBox) && INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(textBox._contentEditableDiv))
            {
                if (e.NewValue != e.OldValue && e.NewValue is bool)
                {
                    bool isReadOnly = (bool)e.NewValue;
                    bool canChangeBackground = textBox.Template == null;
                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.setAttribute(""contentEditable"", $1);
if($2)
    $0.style.backgroundColor=$2;
", textBox._contentEditableDiv, (!isReadOnly).ToString().ToLower(), canChangeBackground.ToString().ToLower(), isReadOnly ? "#DDDDDD" : "#FFFFFF");

                    if (!IsRunningInJavaScript())
                    {
                        //--- SIMULATOR ONLY: ---
                        INTERNAL_HtmlDomManager.ExecuteJavaScript(string.Format(@"
var element = document.getElementById(""{0}"");
element.setAttribute(""data-isreadonly"",""{1}"");
", ((INTERNAL_HtmlDomElementReference)textBox._contentEditableDiv).UniqueIdentifier, isReadOnly.ToString().ToLower()));
                    }
                }
            }
        }

        #region Not implemented yet (should we move this in WORKINPROGRESS ?)

        public event RoutedEventHandler SelectionChanged;

        public static readonly DependencyProperty SelectionForegroundProperty = DependencyProperty.Register("SelectiongoreGround", typeof(Brush), typeof(TextBox), null);

        public Brush SelectionForeground
        {
            get { return (Brush)this.GetValue(TextBox.SelectionForegroundProperty); }
            set { this.SetValue(TextBox.SelectionForegroundProperty, value); }
        }

        #endregion

#if WORKINPROGRESS
        #region SelectionBackground
        public static readonly DependencyProperty SelectionBackgroundProperty = DependencyProperty.Register("SelectionBackground", typeof(Brush), typeof(TextBox), null);

        public Brush SelectionBackground
        {
            get { return (Brush)GetValue(SelectionBackgroundProperty); }
            set { SetValue(SelectionBackgroundProperty, value); }
        }
        #endregion

        public void Select(int start, int length)
        {

        }

        public string SelectedText { get; set; }
#endif

    }
}
