
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
using System.Linq;
using System.Windows.Markup;
using OpenSilver.Internal.Controls;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
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
        private FrameworkElement _contentElement;
        private ITextBoxViewHost _textViewHost;

        /// <summary>
        /// The name of the ExpanderButton template part.
        /// </summary>
        private readonly string[] TextAreaContainerNames = { "ContentElement", "PART_ContentHost" };
        //                                                  Sl & UWP                WPF

        public TextBox()
        {
            this.DefaultStyleKey = typeof(TextBox);
        }

        internal sealed override bool INTERNAL_GetFocusInBrowser
        {
            get { return true; }
        }

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
            DependencyProperty.Register(
                nameof(AcceptsReturn), 
                typeof(bool), 
                typeof(TextBox), 
                new PropertyMetadata(false, OnAcceptsReturnChanged));

        private static void OnAcceptsReturnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnAcceptsReturnChanged((bool)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether pressing tab while the TextBox has the focus will add a tabulation in the text or set the focus to the next element.
        /// True to add a tabulation, false to set the focus to the next element.
        /// </summary>
        public bool AcceptsTab
        {
            get { return (bool)GetValue(AcceptsTabProperty); }
            set { SetValue(AcceptsTabProperty, value); }
        }

        /// <summary>
        /// Identifies the AcceptsReturn dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptsTabProperty =
            DependencyProperty.Register(
                nameof(AcceptsTab), 
                typeof(bool), 
                typeof(TextBox), 
                new PropertyMetadata(false));

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
            DependencyProperty.Register(
                nameof(PlaceholderText), 
                typeof(string), 
                typeof(TextBox), 
                new PropertyMetadata(string.Empty));

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
            DependencyProperty.Register(
                nameof(Text), 
                typeof(string), 
                typeof(TextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextChanged, CoerceText));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnTextChanged((string)e.NewValue);
            }

            tb.OnTextChanged(new TextChangedEventArgs() { OriginalSource = tb });
        }

        private static object CoerceText(DependencyObject d, object value)
        {
            return value ?? string.Empty;
        }

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
            DependencyProperty.Register(
                nameof(TextAlignment), 
                typeof(TextAlignment), 
                typeof(TextBox), 
                new PropertyMetadata(TextAlignment.Left, OnTextAlignmentChanged));

        private static void OnTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnTextAlignmentChanged((TextAlignment)e.NewValue);
            }
        }

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
            DependencyProperty.Register(
                nameof(CaretBrush), 
                typeof(Brush), 
                typeof(TextBox), 
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

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
            DependencyProperty.Register(
                nameof(TextWrapping),
                typeof(TextWrapping),
                typeof(TextBox),
                new FrameworkPropertyMetadata(TextWrapping.Wrap, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextWrappingChanged));

        private static void OnTextWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnTextWrappingChanged((TextWrapping)e.NewValue);
            }
        }

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
            DependencyProperty.Register(
                nameof(HorizontalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(TextBox),
                new PropertyMetadata(ScrollBarVisibility.Hidden, OnHorizontalScrollBarVisibilityChanged));

        private static void OnHorizontalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnHorizontalScrollBarVisibilityChanged((ScrollBarVisibility)e.NewValue);
            }
        }

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
            DependencyProperty.Register(
                nameof(VerticalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(TextBox),
                new PropertyMetadata(ScrollBarVisibility.Hidden, OnVerticalScrollBarVisibilityChanged));

        private static void OnVerticalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnVerticalScrollBarVisibilityChanged((ScrollBarVisibility)e.NewValue);
            }
        }

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
            DependencyProperty.Register(
                nameof(MaxLength),
                typeof(int),
                typeof(TextBox),
                new PropertyMetadata(0, OnMaxLengthChanged));

        private static void OnMaxLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnMaxLengthChanged((int)e.NewValue);
            }
        }

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
        public new static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register(
                nameof(TextDecorations),
                typeof(TextDecorationCollection),
                typeof(TextBox),
                new PropertyMetadata(null, OnTextDecorationsChanged));

        private static void OnTextDecorationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnTextDecorationsChanged((TextDecorationCollection)e.NewValue);
            }
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
            DependencyProperty.Register(
                nameof(TextDecorations), 
                typeof(TextDecorations?), 
                typeof(TextBox), 
                new PropertyMetadata(null, OnTextDecorationsChanged));

        private static void OnTextDecorationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnTextDecorationsChanged((TextDecorations?)e.NewValue);
            }
        }
#endif

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(TextBox),
                new PropertyMetadata(false, OnIsReadOnlyChanged));

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBox)d;
            if (tb._textViewHost != null)
            {
                tb._textViewHost.View.OnIsReadOnlyChanged((bool)e.NewValue);
            }

            tb.UpdateVisualStates();
        }

        public int SelectionStart
        {
            get
            {
                int selectionStartIndex; int selectionLength;
                if (_textViewHost != null)
                {
                    _textViewHost.View.NEW_GET_SELECTION(out selectionStartIndex, out selectionLength);
                    return selectionStartIndex;
                }

                return 0;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("SelectionStart cannot be lower than 0");
                }

                if (_textViewHost != null)
                {
                    _textViewHost.View.NEW_SET_SELECTION(value, value + SelectionLength);
                }
            }
        }

        public int SelectionLength
        {
            get
            {
                int selectionStartIndex; int selectionLength;
                if (_textViewHost != null)
                {
                    _textViewHost.View.NEW_GET_SELECTION(out selectionStartIndex, out selectionLength);
                    return selectionLength;
                }

                return 0;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("SelectionLength cannot be lower than 0");
                }

                if (_textViewHost != null)
                {
                    _textViewHost.View.NEW_SET_SELECTION(SelectionStart, SelectionStart + value);
                }
            }
        }        

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

        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="TextBox" /> control when a new
        /// template is applied.
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

        /// <summary>
        /// Selects all text in the text box.
        /// </summary>
        public void SelectAll()
        {
            this.SelectionStart = 0;
            this.SelectionLength = this.Text.Length;
        }

        public void Select(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            SelectionStart = start;
            SelectionLength = length;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size BorderThicknessSize = new Size(BorderThickness.Left + BorderThickness.Right, BorderThickness.Top + BorderThickness.Bottom);
            Size TextSize = Application.Current.TextMeasurementService.Measure(Text ?? String.Empty, FontSize, FontFamily, FontStyle, FontWeight, /*FontStretch, */TextWrapping, Padding, (availableSize.Width - BorderThicknessSize.Width).Max(0));
            TextSize.Width = TextSize.Width + BorderThicknessSize.Width;
            TextSize.Height = TextSize.Height + BorderThicknessSize.Height;
            return TextSize;
        }

        internal override void UpdateVisualStates()
        {
            if (!IsEnabled)
            {
                GoToState(VisualStates.StateDisabled);
            }
            else if (IsReadOnly)
            {
                GoToState(VisualStates.StateReadOnly);
            }
            else
            {
                GoToState(VisualStates.StateNormal);
            }
        }

        private TextBoxView CreateView()
        {
            return new TextBoxView(this);
        }

        private void InitializeContentElement()
        {
            _textViewHost = GetContentHost(_contentElement);

            if (_textViewHost != null)
            {
                TextBoxView view = CreateView();
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

        private static ITextBoxViewHost GetContentHost(FrameworkElement contentElement)
        {
            if (contentElement is ContentControl cc)
            {
                return new TextBoxViewHost_ContentControl(cc);
            }
            else if (contentElement is ContentPresenter cp)
            {
                return new TextBoxViewHost_ContentPresenter(cp);
            }
            else if (contentElement is Border border)
            {
                return new TextBoxViewHost_Border(border);
            }
            else if (contentElement is UserControl uc)
            {
                return new TextBoxViewHost_UserControl(uc);
            }
            else if (contentElement is Panel panel)
            {
                return new TextBoxViewHost_Panel(panel);
            }
            else if (contentElement is ItemsControl ic)
            {
                return new TextBoxViewHost_ItemsControl(ic);
            }
            else if (IsContentPropertyHost(contentElement, out string contentPropertyName))
            {
                return new TextBoxViewHost_ContentProperty(contentElement, contentPropertyName);
            }

            return null;
        }

        private static bool IsContentPropertyHost(FrameworkElement host, out string contentPropertyName)
        {
            ContentPropertyAttribute contentProp = (ContentPropertyAttribute)host
                .GetType()
                .GetCustomAttributes(typeof(ContentPropertyAttribute), true)
                .FirstOrDefault();

            if (contentProp != null)
            {
                contentPropertyName = contentProp.Name;
                return true;
            }

            contentPropertyName = null;
            return false;
        }

        [OpenSilver.NotImplemented]
        public event RoutedEventHandler SelectionChanged;

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register(
                nameof(SelectionForeground),
                typeof(Brush),
                typeof(TextBox),
                null);

        [OpenSilver.NotImplemented]
        public Brush SelectionForeground
        {
            get { return (Brush)this.GetValue(TextBox.SelectionForegroundProperty); }
            set { this.SetValue(TextBox.SelectionForegroundProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectionBackgroundProperty =
            DependencyProperty.Register(
                nameof(SelectionBackground),
                typeof(Brush),
                typeof(TextBox),
                null);

        [OpenSilver.NotImplemented]
        public Brush SelectionBackground
        {
            get { return (Brush)GetValue(SelectionBackgroundProperty); }
            set { SetValue(SelectionBackgroundProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public string SelectedText { get; set; }

        [OpenSilver.NotImplemented]
        public double LineHeight { get; set; }
    }
}
