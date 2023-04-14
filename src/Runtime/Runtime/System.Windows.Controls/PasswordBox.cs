
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
using OpenSilver.Internal.Controls;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
    [TemplatePart(Name = ContentElementName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    public class PasswordBox : Control
    {
        private const string ContentElementName = "ContentElement"; // Sl & UWP
        private const string ContentElementName_WPF = "PART_ContentHost"; // WPF

        private bool _isFocused;
        private FrameworkElement _contentElement;
        private ITextBoxViewHost<PasswordBoxView> _textViewHost;

        public PasswordBox()
        {
            DefaultStyleKey = typeof(PasswordBox);
            IsEnabledChanged += (o, e) => UpdateVisualStates();
        }

        internal sealed override object GetFocusTarget() => _textViewHost?.View?.InputDiv;

        /// <summary>
        /// The DependencyID for the PasswordChar property.
        /// Default Value: '•'
        /// </summary>
        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.Register(
                nameof(PasswordChar), 
                typeof(char), 
                typeof(PasswordBox), 
                new PropertyMetadata('•'));

        /// <summary>
        /// Character to display instead of the actual password.
        /// </summary>
        public char PasswordChar
        {
            get => (char)GetValue(PasswordCharProperty);
            set => SetValue(PasswordCharProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum length for passwords to be handled by this PasswordBox.
        /// </summary>
        /// <returns>
        /// An integer that specifies the maximum number of characters for passwords
        /// to be handled by this PasswordBox. A value of zero (0) means no limit. The
        /// default is 0 (no length limit).
        /// </returns>
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxLength dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(
                nameof(MaxLength),
                typeof(int),
                typeof(PasswordBox),
                new PropertyMetadata(0, OnMaxLengthChanged));

        private static void OnMaxLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pwb = (PasswordBox)d;
            if (pwb._textViewHost != null)
            {
                pwb._textViewHost.View.OnMaxLengthChanged((int)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets the password currently held by the <see cref="PasswordBox"/>.
        /// </summary>
        /// <returns>
        /// A string representing the password currently held by the <see cref="PasswordBox"/>.The
        /// default value is <see cref="string.Empty"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The property is set to a null value.
        /// </exception>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                SetValue(PasswordProperty, value); 
            }
        }
        
        /// <summary>
        /// Identifies the Password dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(PasswordBox),
                new PropertyMetadata(string.Empty, OnPasswordChanged, CoercePassword));

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pwb = (PasswordBox)d;
            if (pwb._textViewHost != null)
            {
                pwb._textViewHost.View.OnPasswordChanged((string)e.NewValue);
            }

            pwb.OnPasswordChanged(new RoutedEventArgs
            {
                OriginalSource = pwb
            });
        }

        private static object CoercePassword(DependencyObject d, object baseValue)
        {
            return baseValue ?? string.Empty;
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

#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseEnter(e);
#else
            base.OnPointerEntered(e);
#endif
            UpdateVisualStates();
        }

#if MIGRATION
        protected override void OnMouseLeave(MouseEventArgs e)
#else
        protected override void OnPointerExited(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeave(e);
#else
            base.OnPointerExited(e);
#endif
            UpdateVisualStates();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _isFocused = true;
            UpdateVisualStates();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _isFocused = false;
            UpdateVisualStates();
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="PasswordBox" /> 
        /// control when a new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (_contentElement != null)
            {
                ClearContentElement();
                _contentElement = null;
            }

            FrameworkElement contentElement = GetTemplateChild(ContentElementName) as FrameworkElement
                ?? GetTemplateChild(ContentElementName_WPF) as FrameworkElement;

            if (contentElement != null)
            {
                _contentElement = contentElement;
                InitializeContentElement();
            }

            UpdateVisualStates();
        }

        private PasswordBoxView CreateView()
        {
            return new PasswordBoxView(this);
        }

        private void InitializeContentElement()
        {
            _textViewHost = TextBox.GetContentHost<PasswordBoxView>(_contentElement);

            if (_textViewHost != null)
            {
                PasswordBoxView view = CreateView();

                _textViewHost.AttachView(view);
            }
        }

        private void ClearContentElement()
        {
            if (_textViewHost != null)
            {
                _textViewHost.DetachView();
                _textViewHost = null;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new PasswordBoxAutomationPeer(this);

        /// <summary>
        /// Selects all the character in the PasswordBox.
        /// </summary>
        public void SelectAll()
        {
            if (INTERNAL_InnerDomElement is null)
            {
                return;
            }

            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_InnerDomElement);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"{sDiv}.setSelectionRange(0, {sDiv}.value.length)");
        }

        internal override void UpdateVisualStates()
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, VisualStates.StateDisabled, false);
            }
            else if (IsPointerOver)
            {
                VisualStateManager.GoToState(this, VisualStates.StateMouseOver, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateNormal, false);
            }

            if (_isFocused)
            {
                VisualStateManager.GoToState(this, VisualStates.StateFocused, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateUnfocused, false);
            }
        }

        #region Not supported yet

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CaretBrushProperty = DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(PasswordBox), null);

        /// <summary>
        /// Gets or sets the brush that is used to render the vertical bar that indicates the insertion point.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Brush CaretBrush
        {
            get { return (Brush)this.GetValue(PasswordBox.CaretBrushProperty); }
            set { this.SetValue(PasswordBox.CaretBrushProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectionBackgroundProperty = DependencyProperty.Register("SelectionBackground", typeof(Brush), typeof(PasswordBox), null);

        /// <summary>
        /// Gets or sets the brush used to render the background for the selected text.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Brush SelectionBackground
        {
            get { return (Brush)this.GetValue(PasswordBox.SelectionBackgroundProperty); }
            set { this.SetValue(PasswordBox.SelectionBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PasswordBox.SelectionForeground"/> dependency
        /// property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register(
                nameof(SelectionForeground),
                typeof(Brush),
                typeof(PasswordBox),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the brush used for the selected text in the <see cref="PasswordBox"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Brush SelectionForeground
        {
            get { return (Brush)this.GetValue(SelectionForegroundProperty); }
            set { this.SetValue(SelectionForegroundProperty, value); }
        }

#endregion
    }
}
