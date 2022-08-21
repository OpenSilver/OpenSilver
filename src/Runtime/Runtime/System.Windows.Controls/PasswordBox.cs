
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
using System.Windows.Media;
#else
using Windows.Foundation;
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
    public class PasswordBox : Control
    {
        private FrameworkElement _contentElement;
        private ITextBoxViewHost<PasswordBoxView> _textViewHost;

        private readonly string[] TextAreaContainerNames = { "ContentElement", "PART_ContentHost" };

        public PasswordBox()
        {
            this.DefaultStyleKey = typeof(PasswordBox);
        }

        internal override object GetFocusTarget() => _textViewHost?.View?.InputDiv;

        internal sealed override bool INTERNAL_GetFocusInBrowser => true;

        /// <summary>
        /// The DependencyID for the PasswordChar property.
        /// Default Value: '●'
        /// </summary>
        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.Register(
                nameof(PasswordChar), 
                typeof(char), 
                typeof(PasswordBox), 
                new PropertyMetadata('●'));

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

            FrameworkElement contentElement = null;

            int i = 0;
            while (contentElement == null && i < TextAreaContainerNames.Length)
            {
                contentElement = GetTemplateChild(TextAreaContainerNames[i]) as FrameworkElement;
                ++i;
            }

            if (contentElement != null)
            {
                _contentElement = contentElement;
                InitializeContentElement();
            }
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
                view.Loaded += new RoutedEventHandler(OnViewLoaded);

                _textViewHost.AttachView(view);
            }
        }

        private void ClearContentElement()
        {
            if (_textViewHost != null)
            {
                _textViewHost.View.Loaded -= new RoutedEventHandler(OnViewLoaded);

                _textViewHost.DetachView();
                _textViewHost = null;
            }
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            UpdateTabIndex(IsTabStop, TabIndex);
        }

        /// <summary>
        /// Selects all the character in the PasswordBox.
        /// </summary>
        public void SelectAll()
        {
            OpenSilver.Interop.ExecuteJavaScriptAsync(@"$0.setSelectionRange(0, $0.value.length)", this.INTERNAL_InnerDomElement);
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

        protected override Size MeasureOverride(Size availableSize)
        {
            Size BorderThicknessSize = new Size(BorderThickness.Left + BorderThickness.Right, BorderThickness.Top + BorderThickness.Bottom);
            Size TextSize = Application.Current.TextMeasurementService.Measure(String.Empty, FontSize, FontFamily, FontStyle, FontWeight, /*FontStretch, */TextWrapping.NoWrap, Padding, (availableSize.Width - BorderThicknessSize.Width).Max(0));
            TextSize.Width = TextSize.Width + BorderThicknessSize.Width;
            TextSize.Height = TextSize.Height + BorderThicknessSize.Height;
            return TextSize;
        }
    }
}
