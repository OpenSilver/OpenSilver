
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    public class PasswordBox : Control
    {
        private bool _isUserChangingPassword = false;
        private bool _isCodeProgrammaticallyChangingPassword = false;

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
                INTERNAL_HtmlDomManager.SetDomElementProperty(passwordBox.INTERNAL_OuterDomElement, "maxLength", newValueInt);
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
                    string text = (string)CSHTML5.Interop.ExecuteJavaScript("$0['value'] || ''", this.INTERNAL_OuterDomElement);
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
            dynamic passwordField = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("input", parentRef, this);
            domElementWhereToPlaceChildren = passwordField; // Note: this value is used by the Padding_Changed method to set the padding of the PasswordBox.

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

            return passwordField;
        }

        #region Fix "input" event not working under IE.

        string previousInnerText = null;

        void InternetExplorer_GotFocus(object sender, RoutedEventArgs e)
        {
#if !CSHTML5NETSTANDARD //todo: fixme
            previousInnerText = this.INTERNAL_OuterDomElement.value;
#endif
        }

        void InternetExplorer_LostFocus(object sender, RoutedEventArgs e)
        {
            InternetExplorer_RaisePasswordChangedIfNecessary();
        }

        void InternetExplorer_RaisePasswordChangedIfNecessary()
        {
#if !CSHTML5NETSTANDARD //todo: fixme
            string newInnerText = this.INTERNAL_OuterDomElement.value;
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
            Interop.ExecuteJavaScriptAsync(@"$0.setSelectionRange(0, $0.value.length)", this.INTERNAL_OuterDomElement);
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
