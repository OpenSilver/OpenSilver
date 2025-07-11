
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

using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal.Controls;

namespace System.Windows.Controls
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
        private const string ContentElementName = "ContentElement"; // SL
        private const string ContentElementName_WPF = "PART_ContentHost"; // WPF

        private bool _isProcessingInput;
        private bool _isFocused;
        private FrameworkElement _contentElement;
        private ITextViewHost<PasswordBoxView> _textViewHost;

        static PasswordBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordBox), new PropertyMetadata(typeof(PasswordBox)));
            IsEnabledProperty.OverrideMetadata(typeof(PasswordBox), new PropertyMetadata(OnVisualStatePropertyChanged));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordBox"/> class.
        /// </summary>
        public PasswordBox() { }

        internal sealed override INTERNAL_HtmlDomElementReference GetFocusTarget()
            => _textViewHost?.View?.OuterDiv ?? base.GetFocusTarget();

        /// <summary>
        /// Identifies the <see cref="PasswordChar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.Register(
                nameof(PasswordChar),
                typeof(char),
                typeof(PasswordBox),
                new PropertyMetadata('•'));

        /// <summary>
        /// Character to display instead of the actual password. The default value is '•'.
        /// </summary>
        public char PasswordChar
        {
            get => (char)GetValue(PasswordCharProperty);
            set => SetValueInternal(PasswordCharProperty, value);
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
            get => (int)GetValue(MaxLengthProperty);
            set => SetValueInternal(MaxLengthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MaxLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty =
            TextBox.MaxLengthProperty.AddOwner(
                typeof(PasswordBox),
                new PropertyMetadata(0, OnMaxLengthChanged));

        private static void OnMaxLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pwb = (PasswordBox)d;
            pwb._textViewHost?.View.OnMaxLengthChanged((int)e.NewValue);
        }

        /// <summary>
        /// Identifies the <see cref="CaretBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaretBrushProperty =
            TextBox.CaretBrushProperty.AddOwner(
                typeof(PasswordBox),
                new PropertyMetadata(Brushes.Black, OnCaretBrushChanged));

        /// <summary>
        /// Gets or sets the brush that is used to render the vertical bar that indicates the
        /// insertion point.
        /// </summary>
        /// <returns>
        /// The brush that is used to render the vertical bar that indicates the insertion point.
        /// </returns>
        public Brush CaretBrush
        {
            get => (Brush)GetValue(CaretBrushProperty);
            set => SetValueInternal(CaretBrushProperty, value);
        }

        private static void OnCaretBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PasswordBox)d)._textViewHost?.View.SetCaretBrush((Brush)e.NewValue);
        }

        /// <summary>
        /// Identifies the <see cref="SelectionBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBackgroundProperty =
            TextBox.SelectionBackgroundProperty.AddOwner(
                typeof(PasswordBox),
                new PropertyMetadata((object)null)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        ((PasswordBox)d).OuterDiv.Style.setProperty(
                            "--selection-bg-color",
                            newValue switch
                            {
                                SolidColorBrush scb => scb.ToHtmlString(),
                                _ => string.Empty,
                            });
                    },
                });

        /// <summary>
        /// Gets or sets the brush used to render the background for the selected text.
        /// </summary>
        /// <returns>
        /// The brush that fills the background of the selected text.
        /// </returns>
        public Brush SelectionBackground
        {
            get => (Brush)GetValue(SelectionBackgroundProperty);
            set => SetValueInternal(SelectionBackgroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectionForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionForegroundProperty =
            TextBox.SelectionForegroundProperty.AddOwner(
                typeof(PasswordBox),
                new PropertyMetadata((object)null)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        ((PasswordBox)d).OuterDiv.Style.setProperty(
                            "--selection-color",
                            newValue switch
                            {
                                SolidColorBrush scb => scb.ToHtmlString(),
                                _ => string.Empty,
                            });
                    },
                });

        /// <summary>
        /// Gets or sets the brush used for the selected text in the <see cref="PasswordBox"/>.
        /// </summary>
        /// <returns>
        /// The brush used for the selected text in the <see cref="PasswordBox"/>.
        /// </returns>
        public Brush SelectionForeground
        {
            get => (Brush)GetValue(SelectionForegroundProperty);
            set => SetValueInternal(SelectionForegroundProperty, value);
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
            get => (string)GetValue(PasswordProperty);
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                SetValueInternal(PasswordProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Password"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(PasswordBox),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.None,
                    OnPasswordChanged,
                    CoercePassword,
                    UpdateSourceTrigger.LostFocus));

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pwb = (PasswordBox)d;
            if (pwb._isProcessingInput)
            {
                // Clear the flag to allow changing the Text during TextChanged
                pwb._isProcessingInput = false;
            }
            else
            {
                pwb._textViewHost?.View.SetPasswordNative((string)e.NewValue);
            }

            pwb.OnPasswordChanged(new RoutedEventArgs { Source = pwb });
        }

        private static object CoercePassword(DependencyObject d, object baseValue) => baseValue ?? string.Empty;

        internal void UpdatePasswordProperty(string text)
        {
            _isProcessingInput = true;
            try
            {
                SetCurrentValueInternal(PasswordProperty, text);
            }
            finally
            {
                _isProcessingInput = false;
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
            PasswordChanged?.Invoke(this, eventArgs);
        }

        #endregion

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateVisualStates();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.Handled)
            {
                return;
            }

            e.Handled = true;
            Focus();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            e.Handled = true;
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="PasswordBox" /> control when 
        /// a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
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

        private PasswordBoxView CreateView() => new PasswordBoxView(this);

        private void InitializeContentElement()
        {
            _textViewHost = TextViewHostProvider.From<PasswordBoxView>(_contentElement);

            if (_textViewHost != null)
            {
                PasswordBoxView view = CreateView();
                _textViewHost.AttachView(view);

                if (_contentElement is ScrollViewer scrollViewer)
                {
                    scrollViewer.CanContentScroll = true;
                }
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
        public void SelectAll() => _textViewHost?.View.SelectNative();

        internal override void UpdateVisualStates(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, VisualStates.StateDisabled, useTransitions);
            }
            else if (IsMouseOver)
            {
                VisualStateManager.GoToState(this, VisualStates.StateMouseOver, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateNormal, useTransitions);
            }

            if (_isFocused)
            {
                VisualStateManager.GoToState(this, VisualStates.StateFocused, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateUnfocused, useTransitions);
            }
        }
    }
}
