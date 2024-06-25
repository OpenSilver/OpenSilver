
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

using System.Windows.Markup;
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal.Controls;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a rich text control that supports formatted text, hyperlinks, inline
    /// images, and other rich content.
    /// </summary>
    [ContentProperty(nameof(Blocks))]
    [TemplatePart(Name = ContentElementName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateReadOnly, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    public class RichTextBox : Control
    {
        private const string ContentElementName = "ContentElement";

        private BlockCollection _blocks;
        private bool _isFocused;
        private FrameworkElement _contentElement;
        private ScrollViewer _scrollViewer;
        private ITextViewHost<RichTextBoxView> _textViewHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBox"/> class.
        /// </summary>
        public RichTextBox()
        {
            DefaultStyleKey = typeof(RichTextBox);
            SetValueInternal(BlocksProperty, new BlockCollection(this));
            Selection = new TextSelection(this);
            ContentStart = new TextPointer(this, 0, LogicalDirection.Backward);
            ContentEnd = new TextPointer(this, 0, LogicalDirection.Forward);
            CoerceValue(HorizontalScrollBarVisibilityProperty);
            IsEnabledChanged += (o, e) => UpdateVisualStates();
        }

        internal RichTextBoxView View => _textViewHost?.View;

        internal sealed override INTERNAL_HtmlDomElementReference GetFocusTarget()
            => _textViewHost?.View?.OuterDiv ?? base.GetFocusTarget();

        /// <summary>
        /// Occurs when the content changes in a <see cref="RichTextBox"/>.
        /// </summary>
        public event ContentChangedEventHandler ContentChanged;

        internal void OnContentChanged()
        {
            int contentLength = View?.GetContentLength() ?? 0;
            ContentEnd = new TextPointer(this, contentLength, LogicalDirection.Forward);

            ContentChanged?.Invoke(this, new ContentChangedEventArgs());
        } 

        /// <summary>
        /// Occurs when the text selection has changed.
        /// </summary>
        public event RoutedEventHandler SelectionChanged;

        internal void UpdateSelection(int start, int length)
        {
            Selection.Update(start, length);
            SelectionChanged?.Invoke(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                nameof(VerticalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(RichTextBox),
                new PropertyMetadata(ScrollBarVisibility.Auto, OnVerticalScrollBarVisibilityChanged));

        /// <summary>
        /// Gets or sets the visibility of the vertical scroll bar.
        /// </summary>
        /// <returns>
        /// The visibility of the vertical scroll bar. The default is <see cref="ScrollBarVisibility.Auto"/>.
        /// </returns>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        private static void OnVerticalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rtb = (RichTextBox)d;
            if (rtb._scrollViewer is ScrollViewer sv)
            {
                sv.VerticalScrollBarVisibility = (ScrollBarVisibility)e.NewValue;
            }
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(
                nameof(HorizontalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(RichTextBox),
                new PropertyMetadata(ScrollBarVisibility.Hidden, OnHorizontalScrollBarVisibilityChanged, CoerceHorizontalScrollBarVisibility));

        /// <summary>
        /// Gets or sets the visibility of the horizontal scroll bar.
        /// </summary>
        /// <returns>
        /// The visibility of the horizontal scroll bar. The default is <see cref="ScrollBarVisibility.Hidden"/>.
        /// </returns>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        private static void OnHorizontalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rtb = (RichTextBox)d;
            if (rtb._scrollViewer is ScrollViewer sv)
            {
                sv.HorizontalScrollBarVisibility = (ScrollBarVisibility)e.NewValue;
            }
        }

        private static object CoerceHorizontalScrollBarVisibility(DependencyObject d, object baseValue)
        {
            var rtb = (RichTextBox)d;
            if (rtb.TextWrapping == TextWrapping.Wrap)
            {
                return ScrollBarVisibility.Disabled;
            }
            return baseValue;
        }

        /// <summary>
        /// Gets or sets a XAML representation of the content in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> object that is a XAML representation of the content in the <see cref="RichTextBox"/>.
        /// </returns>
        public string Xaml
        {
            get => View?.GetXaml() ?? string.Empty;
            set
            {
                Blocks.Clear();
                foreach (Block block in RichTextXamlParser.Parse(value))
                {
                    Blocks.Add(block);
                }

                if (View is RichTextBoxView view)
                {
                    view.SetContentsFromBlocks();
                }
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
        public TextSelection Selection { get; }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that indicates the start of content
        /// in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the start of content in
        /// the <see cref="RichTextBox"/>.
        /// </returns>
        public TextPointer ContentStart { get; private set; }

        /// <summary>
        /// Gets a <see cref="TextPointer"/> that indicates the end of content
        /// in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TextPointer"/> that indicates the end of content in the
        /// <see cref="RichTextBox"/>.
        /// </returns>
        public TextPointer ContentEnd { get; private set; }

        private static readonly DependencyProperty BlocksProperty =
            DependencyProperty.Register(
                nameof(Blocks),
                typeof(BlockCollection),
                typeof(RichTextBox),
                new PropertyMetadata(null, OnBlocksChanged));

        /// <summary>
        /// Gets the contents of the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="BlockCollection"/> that contains the contents of the
        /// <see cref="RichTextBox"/>.
        /// </returns>
        public BlockCollection Blocks => _blocks;

        private static void OnBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBox)d)._blocks = (BlockCollection)e.NewValue;
        }

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
            set => SetValueInternal(IsReadOnlyProperty, value);
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rtb = (RichTextBox)d;
            rtb.View?.OnIsReadOnlyChanged();
            rtb.UpdateVisualStates();
        }

        /// <summary>
        /// Identifies the <see cref="IsSpellCheckEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSpellCheckEnabledProperty =
            DependencyProperty.Register(
                nameof(IsSpellCheckEnabled),
                typeof(bool),
                typeof(RichTextBox),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsSpellCheckEnabledChanged));

        /// <summary>
        /// Gets or sets a value that specifies whether the <see cref="RichTextBox"/> input 
        /// interacts with a spell check engine.
        /// </summary>
        /// <returns>
        /// true if the <see cref="RichTextBox"/> input interacts with a spell check engine; 
        /// otherwise, false. The default is false.
        /// </returns>
        public bool IsSpellCheckEnabled
        {
            get => (bool)GetValue(IsSpellCheckEnabledProperty);
            set => SetValueInternal(IsSpellCheckEnabledProperty, value);
        }

        private static void OnIsSpellCheckEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBox)d).View?.OnIsSpellCheckEnabledChanged((bool)e.NewValue);
        }

        /// <summary>
        /// Identifies the <see cref="LineHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty =
            Block.LineHeightProperty.AddOwner(typeof(RichTextBox));

        /// <summary>
        /// Gets or sets the height of each line of content.
        /// </summary>
        /// <returns>
        /// The height of each line in pixels. A value of 0 indicates that the line height
        /// is determined automatically from the current font characteristics. The default
        /// is 0.
        /// </returns>
        public double LineHeight
        {
            get => (double)GetValue(LineHeightProperty);
            set => SetValueInternal(LineHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AcceptsReturn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptsReturnProperty =
            DependencyProperty.Register(
                nameof(AcceptsReturn),
                typeof(bool),
                typeof(RichTextBox),
                new PropertyMetadata(BooleanBoxes.TrueBox, OnAcceptsReturnChanged));

        /// <summary>
        /// Gets or sets a value that determines whether the <see cref="RichTextBox"/>
        /// allows and displays the newline or return characters when the ENTER or RETURN
        /// keys are pressed.
        /// </summary>
        /// <returns>
        /// true if the <see cref="RichTextBox"/> allows newline characters; otherwise,
        /// false. The default is true.
        /// </returns>
        public bool AcceptsReturn
        {
            get => (bool)GetValue(AcceptsReturnProperty);
            set => SetValueInternal(AcceptsReturnProperty, value);
        }

        private static void OnAcceptsReturnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBox)d).View?.OnAcceptsReturnChanged((bool)e.NewValue);
        }

        /// <summary>
        /// Identifies the <see cref="AcceptsReturn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptsTabProperty =
            DependencyProperty.Register(
                nameof(AcceptsTab),
                typeof(bool),
                typeof(RichTextBox),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnAcceptsTabChanged));

        /// <summary>
        /// Gets or sets a value that determines whether the <see cref="RichTextBox"/>
        /// allows and displays the tabulation character when the TAB key is pressed.
        /// </summary>
        /// <returns>
        /// true if the <see cref="RichTextBox"/> allows tabulation character; otherwise,
        /// false. The default is false.
        /// </returns>
        public bool AcceptsTab
        {
            get => (bool)GetValue(AcceptsTabProperty);
            set => SetValueInternal(AcceptsTabProperty, value);
        }

        private static void OnAcceptsTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBox)d).View?.OnAcceptsTabChanged((bool)e.NewValue);
        }

        /// <summary>
        /// Identifies the <see cref="CaretBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.Register(
                nameof(CaretBrush),
                typeof(Brush),
                typeof(RichTextBox),
                new PropertyMetadata(null, OnCaretBrushChanged));

        /// <summary>
        /// Gets or sets the brush that is used to render the vertical bar that indicates
        /// the insertion point.
        /// </summary>
        /// <returns>
        /// A brush that is used to render the vertical bar that indicates the insertion
        /// point.
        /// </returns>
        public Brush CaretBrush
        {
            get => (Brush)GetValue(CaretBrushProperty);
            set => SetValueInternal(CaretBrushProperty, value);
        }

        private static void OnCaretBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBox)d).View?.SetCaretBrush((Brush)e.NewValue);
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
            set => SetValueInternal(LineStackingStrategyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            Block.TextAlignmentProperty.AddOwner(typeof(RichTextBox));

        /// <summary>
        /// Gets or sets how the text should be aligned in the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="TextAlignment"/> enumeration values. The default is Left.
        /// </returns>
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValueInternal(TextAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextWrapping"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(
                nameof(TextWrapping),
                typeof(TextWrapping),
                typeof(RichTextBox),
                new FrameworkPropertyMetadata(TextWrapping.Wrap, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextWrappingChanged));

        /// <summary>
        /// Gets or sets how text wrapping occurs if a line of text extends beyond the available
        /// width of the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="TextWrapping"/> values. The default is <see cref="TextWrapping.Wrap"/>.
        /// </returns>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValueInternal(TextWrappingProperty, value);
        }

        private static void OnTextWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rtb = (RichTextBox)d;
            rtb.CoerceValue(HorizontalScrollBarVisibilityProperty);
            rtb.View?.OnTextWrappingChanged((TextWrapping)e.NewValue);
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
        public void SelectAll() => View?.SelectAll();

        public override void OnApplyTemplate()
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
                _scrollViewer = contentElement as ScrollViewer;
                InitializeScrollViewer();

                _contentElement = contentElement;
                InitializeContentElement();
            }

            UpdateVisualStates();
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
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _isFocused = true;
            UpdateVisualStates();
        }

        /// <summary>
        /// Called before the <see cref="UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _isFocused = false;
            UpdateVisualStates();
        }

        /// <summary>
        /// Called when the <see cref="UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            base.OnKeyDown(e);

            View?.ProcessKeyDown(e);
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateVisualStates();
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateVisualStates();
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.Handled)
            {
                return;
            }

            e.Handled = true;
            Focus();
            _textViewHost?.View.CaptureMouse();
        }

        /// <summary>
        /// Called before the <see cref="UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (e.Handled)
            {
                return;
            }

            e.Handled = true;
            _textViewHost?.View.ReleaseMouseCapture();
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

        internal override void UpdateVisualStates()
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, VisualStates.StateDisabled, false);
            }
            else if (IsReadOnly)
            {
                VisualStateManager.GoToState(this, VisualStates.StateReadOnly, false);
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

        private RichTextBoxView CreateView() => new RichTextBoxView(this);

        private void InitializeScrollViewer()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.HorizontalScrollBarVisibility = HorizontalScrollBarVisibility;
                _scrollViewer.VerticalScrollBarVisibility = VerticalScrollBarVisibility;
                _scrollViewer.IsTabStop = false;
            }
        }

        private void InitializeContentElement()
        {
            _textViewHost = TextViewHostProvider.From<RichTextBoxView>(_contentElement);

            if (_textViewHost != null)
            {
                RichTextBoxView view = CreateView();
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
    }
}
