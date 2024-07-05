
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

using System.Collections.Generic;
using System.Globalization;
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
        private bool _isModelInvalidated;
        private int _notificationsSuspended;
        private int _changesCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBox"/> class.
        /// </summary>
        public RichTextBox()
        {
            DefaultStyleKey = typeof(RichTextBox);
            SetValueInternal(BlocksPropertyKey, new BlockCollection(this));
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
                using (DeferRefresh())
                {
                    _blocks.Clear();
                    foreach (Block block in RichTextXamlParser.Parse(value))
                    {
                        _blocks.Add(block);
                    }
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

        private static readonly DependencyPropertyKey BlocksPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Blocks),
                typeof(BlockCollection),
                typeof(RichTextBox),
                new ReadOnlyPropertyMetadata(null, GetBlocks, OnBlocksChanged));

        /// <summary>
        /// Gets the contents of the <see cref="RichTextBox"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="BlockCollection"/> that contains the contents of the
        /// <see cref="RichTextBox"/>.
        /// </returns>
        public BlockCollection Blocks => (BlockCollection)GetValue(BlocksPropertyKey.DependencyProperty);

        internal BlockCollection GetBlocksCache() => _blocks;

        private static object GetBlocks(DependencyObject d)
        {
            var richTextBox = (RichTextBox)d;
            if (richTextBox._isModelInvalidated)
            {
                richTextBox.Resync();
                richTextBox._isModelInvalidated = false;
            }

            return richTextBox._blocks;
        }

        private static void OnBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBox)d)._blocks = (BlockCollection)e.NewValue;

            if (e.OldValue is BlockCollection oldBlocks)
            {
                oldBlocks.IsModel = false;
            }
            if (e.NewValue is BlockCollection newBlocks)
            {
                newBlocks.IsModel = true;
            }
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

        public IDisposable DeferRefresh() => new DeferHelper(this);

        private void EndRefresh()
        {
            if (--_notificationsSuspended == 0)
            {
                if (_changesCount > 0)
                {
                    _changesCount = 0;
                    View?.SetContentsFromBlocks();
                }
            }
        }

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

        internal void InvalidateUI()
        {
            if (_isModelInvalidated)
            {
                return;
            }

            if (_notificationsSuspended > 0)
            {
                _changesCount++;
                return;
            }

            View?.InvalidateUI();
        }

        internal void InvalidateModel()
        {
            if (_notificationsSuspended == 0)
            {
                _isModelInvalidated = true;
            }
        }

        private void Resync()
        {
            _blocks.Clear();

            if (View is RichTextBoxView view)
            {
                var parser = new QuillContentParser(view.GetContents());

                while (parser.MoveToNextBlock())
                {
                    _blocks.Add(CreateParagraph(parser.BlockFormat, parser.Inlines));
                }
            }
        }

        private Paragraph CreateParagraph(QuillRangeFormat format, IEnumerable<QuillDelta> deltas)
        {
            var paragraph = new Paragraph();

            if (!string.IsNullOrEmpty(format.TextAlignment))
            {
                paragraph.TextAlignment = format.TextAlignment switch
                {
                    "center" => TextAlignment.Center,
                    "end" => TextAlignment.Right,
                    "justify" => TextAlignment.Justify,
                    _ => TextAlignment.Left,
                };
            }

            if (!string.IsNullOrEmpty(format.LineHeight))
            {
                if (format.LineHeight == "normal")
                {
                    paragraph.LineHeight = 0.0;
                }
                else if (format.LineHeight.EndsWith("px") && double.TryParse(
                    format.LineHeight.Substring(0, format.LineHeight.Length - 2),
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture,
                    out double lineHeight))
                {
                    paragraph.LineHeight = lineHeight;
                }
            }

            foreach (QuillDelta delta in deltas)
            {
                if (!string.IsNullOrEmpty(delta.Text))
                {
                    paragraph.Inlines.Add(CreateRun(delta));
                }
            }

            return paragraph;
        }

        private Run CreateRun(QuillDelta delta)
        {
            var run = new Run();

            run.Text = delta.Text;

            if (delta.Attributes is QuillRangeFormat format)
            {
                if (!string.IsNullOrEmpty(format.FontFamily))
                {
                    run.FontFamily = new FontFamily(format.FontFamily);
                }

                if (!string.IsNullOrEmpty(format.FontWeight))
                {
                    if (FontWeights.FontWeightStringToKnownWeight(format.FontWeight, CultureInfo.InvariantCulture, out FontWeight fontWeight))
                    {
                        run.FontWeight = fontWeight;
                    }
                }

                if (!string.IsNullOrEmpty(format.FontStyle))
                {
                    run.FontStyle = format.FontStyle switch
                    {
                        "italic" => FontStyles.Italic,
                        "oblique" => FontStyles.Oblique,
                        _ => FontStyles.Normal,
                    };
                }

                if (!string.IsNullOrEmpty(format.FontSize))
                {
                    if (format.FontSize.EndsWith("px") && double.TryParse(
                        format.FontSize.Substring(0, format.FontSize.Length - 2),
                        NumberStyles.Float | NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture,
                        out double fontSize))
                    {
                        run.FontSize = fontSize;
                    }
                }

                if (!string.IsNullOrEmpty(format.Foreground))
                {
                    if (RichTextBoxView.TryParseCssColor(format.Foreground, out Color color))
                    {
                        run.Foreground = new SolidColorBrush(color);
                    }
                }

                if (!string.IsNullOrEmpty(format.CharacterSpacing))
                {
                    if (format.CharacterSpacing == "normal")
                    {
                        run.CharacterSpacing = 0;
                    }
                    else if (format.CharacterSpacing.EndsWith("em") && double.TryParse(
                        format.CharacterSpacing.Substring(0, format.CharacterSpacing.Length - 2),
                        NumberStyles.Float | NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture,
                        out double spacing))
                    {
                        run.CharacterSpacing = (int)(spacing * 1000);
                    }
                }

                if (!string.IsNullOrEmpty(format.TextDecorations))
                {
                    run.TextDecorations = format.TextDecorations switch
                    {
                        "underline" => Windows.TextDecorations.Underline,
                        "line-through" => Windows.TextDecorations.Strikethrough,
                        "overline" => Windows.TextDecorations.OverLine,
                        _ => null,
                    };
                }
            }

            return run;
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

        private sealed class DeferHelper : IDisposable
        {
            private readonly RichTextBox _richTextBox;

            public DeferHelper(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
                _richTextBox._notificationsSuspended++;
            }

            ~DeferHelper() => Dispose(false);

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            private void Dispose(bool isDisposing) => _richTextBox.EndRefresh();
        }
    }
}
