
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class PasswordBoxView : FrameworkElement, ITextBoxView
    {
        private object _passwordInputField;
        private bool _isUpdatingDOM;

        internal PasswordBoxView(PasswordBox host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            Host = host;
        }

        internal PasswordBox Host { get; }

        internal object InputDiv => _passwordInputField;

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = AddPasswordInputDomElement(parentRef, out domElementWhereToPlaceChildren, false);
            INTERNAL_InnerDomElement = _passwordInputField;
            return div;
        }
        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            // Note about tabbing: In WPF and SL, the elements in the template other than the input
            // field can have focus but the input field will get any keypress not handled by them
            // For example, you can set the focus on a button included in the template by clicking
            // on it and pressing space will cause a button press, but any character will be added
            // to the Text of the PasswordBox (but the button retains the focus).
            // WPF and SL are different in that in SL, every focusable control in the template can
            // get focus through tabbing while in WPF, the whole control is considered as a single
            // tab stop (tabbing into the PasswordBox will directly put the focus on the input element)
            // BUT in SL, the input will be first in tabOrder (unless maybe if tabIndex is specifically
            // set, I didn't try that) so if the template goes
            //
            // <Stackpanel>
            //     <Button/>
            //     <ContentPresenter/>
            // </StackPanel>
            //
            // by tabbing it will go ContentPresenter first then the Button (example simplified without
            // the names, and also WPF and SL do not use a ContentPresenter but a ScrollViewer or
            // Decorator in which to put the input area)
            //
            // In our case, tabbing will go through the elements accessible through tabbing, without
            // changing the order, and text will only be added when the <input> has focus. On click,
            // the focus will be redirected to the <input>, unless the click was on an element that
            // absorbs pointer events.

            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
            string sCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS((Action<object>)PasswordBox_GotFocus);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sDiv}.addEventListener('click', {sCallback})");

            UpdateDOMPassword(Host.Password);
        }

        internal sealed override NativeEventsManager CreateEventsManager()
        {
            return new NativeEventsManager(this, this, Host, true);
        }

        internal override bool EnablePointerEventsCore => true;

        internal void OnMaxLengthChanged(int maxLength)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(INTERNAL_InnerDomElement, "maxLength", maxLength);
            }
        }

        internal void OnPasswordChanged(string pwd)
        {
            UpdateDOMPassword(pwd);
        }

        private void UpdateDOMPassword(string pwd)
        {
            if (_isUpdatingDOM || _passwordInputField == null)
            {
                return;
            }

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                INTERNAL_HtmlDomManager.SetUIElementContentString(this, pwd);
            }
        }

        private object AddPasswordInputDomElement(object parentRef, out object domElementWhereToPlaceChildren, bool isTemplated)
        {
            object passwordField;
            var passwordFieldStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("input", parentRef, this, out passwordField);

            _passwordInputField = passwordField;

            domElementWhereToPlaceChildren = passwordField; // Note: this value is used by the Padding_Changed method to set the padding of the PasswordBox.

            passwordFieldStyle.border = "transparent"; // This removes the border. We do not need it since we are templated
            passwordFieldStyle.outline = "solid transparent"; // Note: this is to avoind having the weird border when it has the focus. I could have used outlineWidth = "0px" but or some reason, this causes the caret to not work when there is no text.
            passwordFieldStyle.backgroundColor = "transparent";
            passwordFieldStyle.fontSize = "inherit"; // Not inherited by default for "input" DOM elements
            passwordFieldStyle.color = "inherit"; //This is to inherit the foreground value from parent div.
            passwordFieldStyle.width = "100%";
            passwordFieldStyle.height = "100%";

            INTERNAL_HtmlDomManager.SetDomElementAttribute(passwordField, "type", "password");

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

            return passwordField;
        }

#if BRIDGE
        [Bridge.Template("window.IE_VERSION")]
#endif
        private static bool IsRunningOnInternetExplorer()
        {
            return false;
        }

#if BRIDGE
        private string previousInnerText = null;
#endif

        private void InternetExplorer_GotFocus(object sender, RoutedEventArgs e)
        {
#if BRIDGE //todo: fixme
            previousInnerText = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("$0['value'] || ''", this.INTERNAL_InnerDomElement));
#endif
        }

        private void InternetExplorer_LostFocus(object sender, RoutedEventArgs e)
        {
            InternetExplorer_RaisePasswordChangedIfNecessary();
        }

        private void InternetExplorer_RaisePasswordChangedIfNecessary()
        {
#if BRIDGE //todo: fixme
            string newInnerText = Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("$0['value'] || ''", this.INTERNAL_InnerDomElement));
            if (newInnerText != previousInnerText)
            {
                PasswordAreaValueChanged();
                previousInnerText = newInnerText;
            }
#endif
        }

        private void PasswordAreaValueChanged()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                string text = OpenSilver.Interop.ExecuteJavaScriptString(
                    $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_InnerDomElement)}['value'] || ''");

                _isUpdatingDOM = true;

                try
                {
                    Host.SetCurrentValue(PasswordBox.PasswordProperty, text);
                }
                finally
                {
                    _isUpdatingDOM = false;
                }
            }
        }

        private void PasswordBox_GotFocus(object e)//object sender, RoutedEventArgs e)
        {
            bool ignoreEvent = OpenSilver.Interop.ExecuteJavaScriptBoolean(
                $"document.checkForDivsThatAbsorbEvents({CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(e)})");
            if (!ignoreEvent)
            {
                if (_passwordInputField != null)
                {
                    string sInput = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_passwordInputField);
                    string sEventArg = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(e);
                    OpenSilver.Interop.ExecuteJavaScriptVoid(
                        $"if ({sEventArg}.target != {sInput}) {{ {sInput}.focus(); }}");
                }
            }
        }
    }
}
