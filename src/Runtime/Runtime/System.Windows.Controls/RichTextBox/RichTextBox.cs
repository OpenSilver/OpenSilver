
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
using System.Windows.Markup;
using System.Collections.Generic;
using OpenSilver.Internal.Controls;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [ContentProperty("Blocks")]
    public partial class RichTextBox : Control
    {
        private const string ContentElementName = "ContentElement";

        private readonly TextSelection _selection;
        private FrameworkElement _contentElement;
        private ScrollViewer _scrollViewer;
        private ITextBoxViewHost<RichTextBoxView> _textViewHost;
        private ScrollBarVisibility _verticalScrollBarVisibility = ScrollBarVisibility.Auto;
        private ScrollBarVisibility _horizontalScrollBarVisibility = ScrollBarVisibility.Hidden;        

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBox"/> class.
        /// </summary>
        public RichTextBox()
        {
            DefaultStyleKey = typeof(RichTextBox);
            Blocks = new BlockCollection(this, false);
            _selection = new TextSelection(this);
            IsEnabledChanged += (o, e) => _textViewHost?.View.SetEnable((bool)e.NewValue);
        }

        /// <summary>
        /// Occurs when the content changes in a <see cref="RichTextBox"/>.
        /// </summary>
        public event ContentChangedEventHandler ContentChanged;

        /// <summary>
        /// Occurs when the text selection has changed.
        /// </summary>
        public event RoutedEventHandler SelectionChanged;

        /// <summary>
        /// Gets or sets the visibility of the vertical scroll bar.
        /// </summary>
        /// <returns>
        /// The visibility of the vertical scroll bar. The default is <see cref="ScrollBarVisibility.Auto"/>.
        /// </returns>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => _verticalScrollBarVisibility;
            set
            {
                _verticalScrollBarVisibility = value;
                if (_scrollViewer != null)
                {
                    _scrollViewer.VerticalScrollBarVisibility = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the horizontal scroll bar.
        /// </summary>
        /// <returns>
        /// The visibility of the horizontal scroll bar. The default is <see cref="ScrollBarVisibility.Hidden"/>.
        /// </returns>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => _horizontalScrollBarVisibility;
            set
            {
                _horizontalScrollBarVisibility = value;
                if (_scrollViewer != null)
                {
                    _scrollViewer.HorizontalScrollBarVisibility = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a XAML representation of the content in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> object that is a XAML representation of the content in the <see cref="RichTextBox"/>.
        /// </returns>
        public string Xaml
        {
            get => _textViewHost?.View.GetContents();
            set
            {
                Blocks.Clear();
                _textViewHost?.View.Clear();
                foreach (var block in RichTextXamlParser.Parse(value))
                {
                    Blocks.Add(block);
                }
                SetContentsFromBlocks();
            }
        }

        /// <summary>
        /// Gets a value that represents the offset in pixels from the top of the content
        /// to the baseline of the first paragraph. The baseline of the paragraph is the
        /// baseline of the first line in it.
        /// </summary>
        /// <returns>
        /// The computed baseline for the first paragraph, or 0 if the <see cref="RichTextBox"/>
        /// is empty.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double BaselineOffset { get; }        

        /// <summary>
        /// Gets the <see cref="TextSelection"/> in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TextSelection"/> that represents the selected text in
        /// the <see cref="RichTextBox"/>.
        /// </returns>
        public TextSelection Selection
        {
            get
            {
                QuillRange range = _textViewHost?.View.GetSelection();
                if (range != null)
                {
                    _selection.UpdateSelection(range.Start, range.Length);
                }

                return _selection;
            }
        }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that indicates the end of content
        /// in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the end of content in the
        /// <see cref="RichTextBox"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer ContentEnd { get; }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that indicates the start of content
        /// in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the start of content in
        /// the <see cref="RichTextBox"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer ContentStart { get; }

        /// <summary>
        /// Gets the contents of the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="BlockCollection"/> that contains the contents of the
        /// <see cref="RichTextBox"/>.
        /// </returns>
        public BlockCollection Blocks { get; }        

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(RichTextBox),
                new PropertyMetadata(false, OnIsReadOnlyChanged));

        /// <summary>
        /// Gets or sets a value that determines whether the user can change the text in
        /// the <see cref="RichTextBox"/>.
        /// The default is false.
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBox)d).SetReadOnly((bool)e.NewValue);
        }

        /// <summary>
        /// Identifies the <see cref="LineHeight"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register(
                nameof(LineHeight),
                typeof(double),
                typeof(RichTextBox),
                new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the height of each line of content.
        /// </summary>
        /// <returns>
        /// The height of each line in pixels. A value of 0 indicates that the line height
        /// is determined automatically from the current font characteristics. The default
        /// is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double LineHeight
        {
            get => (double)GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AcceptsReturn"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty AcceptsReturnProperty =
            DependencyProperty.Register(
                nameof(AcceptsReturn),
                typeof(bool),
                typeof(RichTextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that determines whether the <see cref="RichTextBox"/>
        /// allows and displays the newline or return characters when the ENTER or RETURN
        /// keys are pressed.
        /// </summary>
        /// <returns>
        /// true if the <see cref="RichTextBox"/> allows newline characters; otherwise,
        /// false. The default is true.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool AcceptsReturn
        {
            get => (bool)GetValue(AcceptsReturnProperty);
            set => SetValue(AcceptsReturnProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CaretBrush"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.Register(
                nameof(CaretBrush),
                typeof(Brush),
                typeof(RichTextBox),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the brush that is used to render the vertical bar that indicates
        /// the insertion point.
        /// </summary>
        /// <returns>
        /// A brush that is used to render the vertical bar that indicates the insertion
        /// point.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Brush CaretBrush
        {
            get => (Brush)GetValue(CaretBrushProperty);
            set => SetValue(CaretBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LineStackingStrategy"/> dependency
        /// property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LineStackingStrategyProperty =
            DependencyProperty.Register(
                nameof(LineStackingStrategy),
                typeof(LineStackingStrategy),
                typeof(RichTextBox),
                new PropertyMetadata(LineStackingStrategy.MaxHeight));

        /// <summary>
        /// Gets or sets a value that indicates how a line box is determined for each line
        /// of text in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A value that indicates how a line box is determined for each line of text in
        /// the <see cref="RichTextBox"/>. The default is <see cref="LineStackingStrategy.MaxHeight"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public LineStackingStrategy LineStackingStrategy
        {
            get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
            set => SetValue(LineStackingStrategyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextAlignment"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(
                nameof(TextAlignment),
                typeof(TextAlignment),
                typeof(RichTextBox),
                new PropertyMetadata(TextAlignment.Left));

        /// <summary>
        /// Gets or sets how the text should be aligned in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="TextAlignment"/> enumeration values. The default is Left.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextWrapping"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(
                nameof(TextWrapping),
                typeof(TextWrapping),
                typeof(RichTextBox),
                new PropertyMetadata(TextWrapping.Wrap));

        /// <summary>
        /// Gets or sets how text wrapping occurs if a line of text extends beyond the available
        /// width of the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="TextWrapping"/> values. The default is <see cref="TextWrapping.Wrap"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        /// <summary>
        /// Returns a <see cref="TextPointer"/> that indicates the closest insertion
        /// position for the specified point.
        /// </summary>
        /// <param name="point">
        /// A point in the coordinate space of the <see cref="RichTextBox"/> for
        /// which the closest insertion position is retrieved.
        /// </param>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the closest insertion position
        /// for the specified point.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TextPointer GetPositionFromPoint(Point point) => null;

        /// <summary>
        /// Selects the entire contents in the <see cref="RichTextBox"/>.
        /// </summary>
        public void SelectAll() => _textViewHost?.View.SelectAll();

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
                _scrollViewer = null;
            }

            if (GetTemplateChild(ContentElementName) is FrameworkElement contentElement)
            {
                _contentElement = contentElement;
                _scrollViewer = contentElement as ScrollViewer;
                InitializeContentElement();
            }
        }

        internal RichTextBoxView View => _textViewHost?.View;

        private RichTextBoxView CreateView()
        {
            return new RichTextBoxView(this);
        }

        private void InitializeContentElement()
        {
            _textViewHost = TextBox.GetContentHost<RichTextBoxView>(_contentElement);

            if (_textViewHost != null)
            {
                RichTextBoxView view = CreateView();
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

            SetContentsFromBlocks();
            UpdateTabIndex(IsTabStop, TabIndex);
        }

        /// <summary>
        /// Returns a <see cref="RichTextBoxAutomationPeer"/> for use by
        /// the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="RichTextBoxAutomationPeer"/> for the <see cref="RichTextBox"/>
        /// object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new RichTextBoxAutomationPeer(this);

        /// <summary>
        /// Called before the <see cref="UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Called when the <see cref="UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
        protected override void OnKeyDown(KeyEventArgs e)
#else
		protected override void OnKeyDown(KeyRoutedEventArgs e)
#endif
        {
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Called before the <see cref="UIElement.KeyUp"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
        protected override void OnKeyUp(KeyEventArgs e)
#else
		protected override void OnKeyUp(KeyRoutedEventArgs e)
#endif
        {
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Called before the <see cref="UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Provides handling for the <see cref="UIElement.LostMouseCapture"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
        protected override void OnLostMouseCapture(MouseEventArgs e)
#else
		protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnLostMouseCapture(e);
#else
            base.OnPointerCaptureLost(e);
#endif
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
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
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
        protected internal override void OnMouseLeave(MouseEventArgs e)
#else
		protected internal override void OnPointerExited(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeave(e);
#else
            base.OnPointerExited(e);
#endif
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
		protected override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
		protected override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
#if MIGRATION
        protected override void OnMouseMove(MouseEventArgs e)
#else
		protected override void OnPointerMoved(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseMove(e);
#else
            base.OnPointerMoved(e);
#endif
        }

        /// <summary>
        /// Called before the <see cref="UIElement.TextInput"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
        }

        /// <summary>
        /// Called before the <see cref="UIElement.TextInputStart"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        protected override void OnTextInputStart(TextCompositionEventArgs e)
        {
            base.OnTextInputStart(e);
        }

        /// <summary>
        /// Called before the <see cref="UIElement.TextInputUpdate"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        [OpenSilver.NotImplemented]
        protected override void OnTextInputUpdate(TextCompositionEventArgs e)
        {
            base.OnTextInputUpdate(e);
        }

        internal void RaiseContentChanged()
            => ContentChanged?.Invoke(this, new ContentChangedEventArgs());

        internal void RaiseSelectionChanged()
            => SelectionChanged?.Invoke(this, new RoutedEventArgs());

        internal string GetRawText()
            => _textViewHost?.View.GetText() ?? string.Empty;

        internal void InsertText(string text)
            => _textViewHost?.View.InsertText(text);

        private void SetReadOnly(bool value)
            => _textViewHost?.View.SetReadOnly(value);

        internal void SetContentsFromBlocks()
        {
            if (_textViewHost == null)
            {
                return;
            }

            foreach (var block in Blocks)
            {
                ProcessBlock(block);
            }
        }

        private void ProcessInlines(Inline inline)
        {
            switch (inline)
            {
                case Run run:
                    var format = new Dictionary<string, object>();
                    if (run.FontSize > 0)
                        format.Add("font-size", run.FontSize);
                    if (run.FontFamily != null)
                        format.Add("font-family", run.FontFamily.ToString());
                    if (run.FontWeight != null)
                        format.Add("bold", run.FontWeight.Weight > 500);
                    if (run.Margin != null)
                        format.Add("margin", run.Margin.ToString());
                    if (run.Background is SolidColorBrush background)
                        format.Add("background", background.ConvertToCSSValue());
                    if (run.Foreground is SolidColorBrush foreground)
                        format.Add("color", foreground.ConvertToCSSValue());
                    _textViewHost.View.SetText(run.Text, format);
                    break;

                case Span span:
                    foreach (var innerSpanBlock in span.Inlines)
                    {
                        ProcessInlines(innerSpanBlock);
                    }
                    break;

                case LineBreak _:
                    _textViewHost.View.SetText("\n");
                    break;
            }
        }

        private void ProcessBlock(Block block)
        {
            switch (block)
            {
                case Section section:
                    foreach (var currentBlock in section.Blocks)
                    {
                        ProcessBlock(currentBlock);
                    }
                    break;

                case Paragraph paragraph:
                    foreach (var currentBlock in paragraph.Inlines)
                    {
                        ProcessInlines(currentBlock);
                    }
                    break;
            }
        }
    }
}
