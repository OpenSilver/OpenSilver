

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


using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control for entering passwords.
    /// </summary>
    public partial class PasswordBox : Control
    {
        private bool _isUserChangingPassword = false;
        private bool _isCodeProgrammaticallyChangingPassword = false;

        Control TextAreaContainer = null;
        object _passwordInputField; //todo: use this

        private string[] TextAreaContainerNames = { "ContentElement", "PART_ContentHost" };

        internal sealed override bool INTERNAL_GetFocusInBrowser
        {
            get { return true; }
        }

        public PasswordBox()
        {
            UseSystemFocusVisuals = true;
        }

        // Returns:
        //     An integer that specifies the maximum number of characters for passwords
        //     to be handled by this PasswordBox. A value of zero (0) means no limit. The
        //     default is 0 (no length limit).
        /// <summary>
        /// Gets or sets the maximum length for passwords to be handled by this PasswordBox.
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
            DependencyProperty.Register("MaxLength", typeof(int), typeof(PasswordBox), new PropertyMetadata(null) { MethodToUpdateDom = MaxLength_MethodToUpdateDom });

        static void MaxLength_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var passwordBox = (PasswordBox)d;
            int newValueInt = (int)newValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(passwordBox))
                INTERNAL_HtmlDomManager.SetDomElementProperty(passwordBox.INTERNAL_InnerDomElement, "maxLength", newValueInt);
        }




        /// <summary>
        /// Gets or sets the password currently held by the PasswordBox.
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        /// <summary>
        /// Identifies the Password dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBox), new PropertyMetadata(string.Empty, Password_Changed));
        static void Password_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = (PasswordBox)d;
            if (!passwordBox._isUserChangingPassword)
            {
                passwordBox._isCodeProgrammaticallyChangingPassword = true; // So that when the c# caller sets the password programmatically, it does not get set multiple times.
                string newPassword = e.NewValue as string ?? string.Empty;
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(passwordBox))
                    INTERNAL_HtmlDomManager.SetUIElementContentString(passwordBox, newPassword);
                passwordBox._isCodeProgrammaticallyChangingPassword = false;
            }
        }


        #region password changed event

        /// <summary>
        /// Occurs when the value of the Password property changes.
        /// </summary>
        public event RoutedEventHandler PasswordChanged;

        /// <summary>
        /// Raises the PasswordChanged event
        /// </summary>
        protected void OnPasswordChanged(RoutedEventArgs eventArgs)
        {
            if (PasswordChanged != null)
            {
                PasswordChanged(this, eventArgs);
            }
        }

        #endregion


        private void PasswordAreaValueChanged()
        {
            if (!_isCodeProgrammaticallyChangingPassword)
            {
                //we get the value:
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    string text = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0['value'] || ''", this.INTERNAL_InnerDomElement));
                    _isUserChangingPassword = true; //To prevent reentrance (infinite loop) when user types some text.
                    //Text = text;
                    SetLocalValue(PasswordProperty, text); //we call SetLocalvalue directly to avoid replacing the BindingExpression
                    _isUserChangingPassword = false;
                    OnPasswordChanged(new RoutedEventArgs() { OriginalSource = this });
                }
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return AddPasswordInputDomElement(parentRef, out domElementWhereToPlaceChildren, false);
        }

        private object AddPasswordInputDomElement(object parentRef, out object domElementWhereToPlaceChildren, bool isTemplated)
        {
            dynamic passwordField;
            var passwordFieldStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("input", parentRef, this, out passwordField);
            //dynamic passwordField = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("input", parentRef, this);

            _passwordInputField = passwordField;

            domElementWhereToPlaceChildren = passwordField; // Note: this value is used by the Padding_Changed method to set the padding of the PasswordBox.

            if (isTemplated) //When templated, we remove the outlining that apears when the element has the focus:
            {
                passwordFieldStyle.border = "transparent"; // This removes the border. We do not need it since we are templated
                passwordFieldStyle.outline = "solid transparent"; // Note: this is to avoind having the weird border when it has the focus. I could have used outlineWidth = "0px" but or some reason, this causes the caret to not work when there is no text.
                passwordFieldStyle.backgroundColor = "transparent";
                passwordFieldStyle.fontSize = "inherit"; // Not inherited by default for "input" DOM elements
            }

            passwordFieldStyle.width = "100%";
            passwordFieldStyle.height = "100%";

            INTERNAL_HtmlDomManager.SetDomElementAttribute(passwordField, "type", "password", forceSimulatorExecuteImmediately: true);
            //passwordField.type = "password";

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
                INTERNAL_EventsHelper.AttachToDomEvents("textinput", passwordField, (Action<object>)(e =>
                {
                    InternetExplorer_RaisePasswordChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("paste", passwordField, (Action<object>)(e =>
                {
                    InternetExplorer_RaisePasswordChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("cut", passwordField, (Action<object>)(e =>
                {
                    InternetExplorer_RaisePasswordChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("keyup", passwordField, (Action<object>)(e =>
                {
                    InternetExplorer_RaisePasswordChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("delete", passwordField, (Action<object>)(e =>
                {
                    InternetExplorer_RaisePasswordChangedIfNecessary();
                }));
                INTERNAL_EventsHelper.AttachToDomEvents("mouseup", passwordField, (Action<object>)(e =>
                {
                    InternetExplorer_RaisePasswordChangedIfNecessary();
                }));
            }
            else
            {
                //-----------------------
                // Modern browsers
                //-----------------------
                INTERNAL_EventsHelper.AttachToDomEvents("input", passwordField, (Action<object>)(e =>
                {
                    PasswordAreaValueChanged();
                }));
            }

            //textArea.style.resize = "none"; //to avoid letting the posibility to the user to resize the TextBox

            if (isTemplated)
            {
                //the following methods were ignored before because _contentEditableDiv was not defined due to the fact that we waited for the template to be made so we would know where to put it.
                //as a consequence, we call them here:
                OnAfterApplyHorizontalAlignmentAndWidth();
                OnAfterApplyVerticalAlignmentAndWidth();

                //register to focusin events on the OuterDomElement so that we can "reroute" the focus on the contentEditable element.
                CSHTML5.Interop.ExecuteJavaScript(@"$0.tabIndex = 32767; $0.addEventListener('focusin', $1)", this.INTERNAL_OuterDomElement, (Action<object>)PasswordBox_GotFocus);
                //Note on the line above: 32767 is the maximum value commonly allowed in browsers (and can be considered the default value)
                if (INTERNAL_HtmlDomManager.IsInternetExplorer())
                {
                    //workaround due to IE setting the focus at the end of the click, (or at least after the focusin event), which cancels the work done during focusin.
                    //if I'm not mistaken, the click event happens even when we click on a child of the element so we're all good. (We had to fix the case where a Button had a TextBox inside of it, which is why I assumed that).
                    CSHTML5.Interop.ExecuteJavaScript(@"$0.addEventListener('click', $1)", this.INTERNAL_OuterDomElement, (Action<object>)PasswordBox_GotFocus);
                }
            }

            return passwordField;
        }

        void PasswordBox_GotFocus(object e)//object sender, RoutedEventArgs e)
        {
            if (_passwordInputField != null)
            {
                CSHTML5.Interop.ExecuteJavaScript(@"
if($1.target != $0) {
$0.focus()
}", _passwordInputField, e);
                //NEW_SET_SELECTION(_tempSelectionStartIndex, _tempSelectionStartIndex + _tempSelectionLength);
            }
        }

        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.PasswordBox" /> control when a new
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
                AddPasswordInputDomElement(TextAreaContainer.INTERNAL_InnerDomElement, out domElementWheretoPlaceChildren, true);
                //AddTextAreaToVisualTree(TextAreaContainer.INTERNAL_InnerDomElement, out domElementWheretoPlaceChildren);
                //Remember that the InnerDomElement is now the _contentEditableDiv rather than what was created to contain the template (Note: we need to do this because the _contentEditableDiv was added outside of the usual place we usually set the innerdomElement).
                INTERNAL_InnerDomElement = _passwordInputField;
                string text = Password; //this is probably more efficient than to use the property itself on 3 occasions.
                if (!string.IsNullOrEmpty(text))
                {
                    Password_Changed(this, new DependencyPropertyChangedEventArgs(text, text, PasswordProperty));
                }
            }
        }

        #region Fix "input" event not working under IE.

        string previousInnerText = null;

        void InternetExplorer_GotFocus(object sender, RoutedEventArgs e)
        {
#if !CSHTML5NETSTANDARD //todo: fixme
            previousInnerText = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0['value'] || ''", this.INTERNAL_InnerDomElement));
#endif
        }

        void InternetExplorer_LostFocus(object sender, RoutedEventArgs e)
        {
            InternetExplorer_RaisePasswordChangedIfNecessary();
        }

        void InternetExplorer_RaisePasswordChangedIfNecessary()
        {
#if !CSHTML5NETSTANDARD //todo: fixme
            string newInnerText = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0['value'] || ''", this.INTERNAL_InnerDomElement));
            if (newInnerText != previousInnerText)
            {
                PasswordAreaValueChanged();
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

        /// <summary>
        /// Selects all the character in the PasswordBox.
        /// </summary>
        public void SelectAll()
        {
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.setSelectionRange(0, $0.value.length)", this.INTERNAL_InnerDomElement);
        }
#if WORKINPROGRESS
        #region Not supported yet

        public static readonly DependencyProperty CaretBrushProperty = DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(PasswordBox), null);

        /// <summary>
        /// Gets or sets the brush that is used to render the vertical bar that indicates the insertion point.
        /// </summary>
        public Brush CaretBrush
        {
            get { return (Brush)this.GetValue(PasswordBox.CaretBrushProperty); }
            set { this.SetValue(PasswordBox.CaretBrushProperty, value); }
        }

        public static readonly DependencyProperty SelectionBackgroundProperty = DependencyProperty.Register("SelectionBackground", typeof(Brush), typeof(PasswordBox), null);

        /// <summary>
        /// Gets or sets the brush used to render the background for the selected text.
        /// </summary>
        public Brush SelectionBackground
        {
            get { return (Brush)this.GetValue(PasswordBox.SelectionBackgroundProperty); }
            set { this.SetValue(PasswordBox.SelectionBackgroundProperty, value); }
        }

        #endregion
#endif

        //// Summary:
        ////     Gets or sets a value that determines whether the visual UI of the PasswordBox
        ////     should include a button element that toggles showing or hiding the typed
        ////     characters.
        ////
        //// Returns:
        ////     True to show a password reveal button; false to not show a password reveal
        ////     button.
        //public bool IsPasswordRevealButtonEnabled { get; set; }
        ////
        //// Summary:
        ////     Identifies the IsPasswordRevealButtonEnabled dependency property.
        ////
        //// Returns:
        ////     The identifier for the IsPasswordRevealButtonEnabled dependency property.
        //public static DependencyProperty IsPasswordRevealButtonEnabledProperty { get; }

        //// Summary:
        ////     Gets or sets the masking character for the PasswordBox.
        ////
        //// Returns:
        ////     A masking character to echo when the user enters text into the PasswordBox.
        ////     The default value is a bullet character (●).
        //public string PasswordChar { get; set; }
        ////
        //// Summary:
        ////     Identifies the PasswordChar dependency property.
        ////
        //// Returns:
        ////     The identifier for the PasswordChar dependency property.
        ////public static DependencyProperty PasswordCharProperty { get; }


        //// Summary:
        ////     Occurs when the system processes an interaction that displays a context menu.
        //public event ContextMenuOpeningEventHandler ContextMenuOpening;
    }
}
