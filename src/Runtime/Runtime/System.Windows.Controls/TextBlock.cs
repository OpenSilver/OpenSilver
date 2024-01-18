
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
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides a lightweight control for displaying small amounts of text.
    /// </summary>
    /// <example>
    /// <code lang="XAML" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    /// <TextBlock x:Name="TextBlockName" Text="Some text."/>
    /// </code>
    /// <code lang="C#">
    /// TextBlockName.Text = "Some text.";
    /// </code>
    /// </example>
    [ContentProperty(nameof(Inlines))]
    public class TextBlock : FrameworkElement
    {
        private Size _noWrapSize = Size.Empty;
        private bool _textContentChanging;

        public TextBlock()
        {
            SetValue(InlinesProperty, new InlineCollection(this));
        }

        /// <summary>
        /// Identifies the <see cref="CharacterSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterSpacingProperty =
            TextElement.CharacterSpacingProperty.AddOwner(
                typeof(TextBlock),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets or sets the distance between characters of text in the control measured
        /// in 1000ths of the font size.
        /// </summary>
        /// <returns>
        /// The distance between characters of text in the control measured in 1000ths of
        /// the font size. The default is 0.
        /// </returns>
        public int CharacterSpacing
        {
            get => (int)GetValue(CharacterSpacingProperty);
            set => SetValue(CharacterSpacingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(
                typeof(TextBlock),
                new FrameworkPropertyMetadata(FontFamily.Default, FrameworkPropertyMetadataOptions.Inherits, OnFontFamilyChanged));

        /// <summary>
        /// Gets or sets the preferred top-level font family for the text content in this
        /// element.
        /// </summary>
        /// <returns>
        /// A <see cref="Media.FontFamily"/> object that specifies the preferred font family,
        /// or a primary preferred font family with one or more fallback font families. For
        /// information about defaults, see the <see cref="Media.FontFamily"/> class topic.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// FontFamily is null.
        /// </exception>
        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElementHelpers.InvalidateMeasureOnFontFamilyChanged((TextBlock)d, (FontFamily)e.NewValue);
        }

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(
                typeof(TextBlock),
                new FrameworkPropertyMetadata(11d, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the font size for the text content in this element.
        /// </summary>
        /// <returns>
        /// A non-negative value that specifies the font size, measured in pixels. The default is 11.
        /// </returns>
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontStretch"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontStretchProperty =
            TextElement.FontStretchProperty.AddOwner(typeof(TextBlock));

        /// <summary>
        /// Gets or sets the font stretch for the text content in this element.
        /// </summary>
        /// <returns>
        /// The requested font stretch, which is a <see cref="Windows.FontStretch"/> that is obtained
        /// from one of the <see cref="FontStretches"/> property values. The default is
        /// <see cref="FontStretches.Normal"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public FontStretch FontStretch
        {
            get => (FontStretch)GetValue(FontStretchProperty);
            set => SetValue(FontStretchProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(
                typeof(TextBlock),
                new FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the font style for the content in this element.
        /// </summary>
        /// <returns>
        /// The requested font style, which is a <see cref="Windows.FontStyle"/> that is obtained
        /// from one of the <see cref="FontStyles"/> property values. The default is <see cref="FontStyles.Normal"/>.
        /// </returns>
        public FontStyle FontStyle
        {
            get => (FontStyle)GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(
                typeof(TextBlock),
                new FrameworkPropertyMetadata(
                    FontWeights.Normal,
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the top-level font weight for the <see cref="TextBlock"/>.
        /// </summary>
        /// <returns>
        /// The requested font weight, which is a <see cref="Windows.FontWeight"/> that is obtained
        /// from one of the <see cref="FontWeights"/> property values. The default is <see cref="FontWeights.Normal"/>.
        /// </returns>
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(
                typeof(TextBlock),
                new FrameworkPropertyMetadata(
                    TextElement.ForegroundProperty.DefaultMetadata.DefaultValue,
                    FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> to apply to the text contents of
        /// the <see cref="TextBlock"/>.
        /// </summary>
        /// <returns>
        /// The brush used to apply to the text contents. The default is a <see cref="SolidColorBrush"/>
        /// with a <see cref="SolidColorBrush.Color"/> value of <see cref="Colors.Black"/>.
        /// </returns>
        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LineHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty =
            Block.LineHeightProperty.AddOwner(
                typeof(TextBlock),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBlock)d).SetLineHeight((double)newValue),
                });

        /// <summary>
        /// Gets or sets the height of each line of content.
        /// </summary>
        /// <returns>
        /// The height of each line in pixels. A value of 0 indicates that the line height
        /// is determined automatically from the current font characteristics. The default
        /// is 0.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <see cref="LineHeight"/> is set to a non-positive value.
        /// </exception>
        public double LineHeight
        {
            get => (double)GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LineStackingStrategy"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LineStackingStrategyProperty =
            DependencyProperty.Register(
                nameof(LineStackingStrategy),
                typeof(LineStackingStrategy),
                typeof(TextBlock),
                new PropertyMetadata(LineStackingStrategy.MaxHeight));

        /// <summary>
        /// Gets or sets a value that indicates how a line box is determined for each line
        /// of text in the <see cref="TextBlock"/>.
        /// </summary>
        /// <returns>
        /// A value that indicates how a line box is determined for each line of text in
        /// the <see cref="TextBlock"/>. The default is <see cref="LineStackingStrategy.MaxHeight"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public LineStackingStrategy LineStackingStrategy
        {
            get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
            set => SetValue(LineStackingStrategyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBlock)d).SetPadding((Thickness)newValue),
                },
                IsPaddingValid);

        /// <summary>
        /// Gets or sets a value that indicates the thickness of padding space between the
        /// boundaries of the content area and the content displayed by a <see cref="TextBlock"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Thickness"/> structure that specifies the amount of padding to apply.
        /// </returns>
        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        private static bool IsPaddingValid(object value)
        {
            Thickness t = (Thickness)value;
            return Thickness.IsValid(t, false, false, false, false);
        }

        /// <summary>
        /// Identifies the <see cref="TextAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            Block.TextAlignmentProperty.AddOwner(
                typeof(TextBlock),
                new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.Inherits)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBlock)d).SetTextAlignment((TextAlignment)newValue),
                });

        /// <summary>
        /// Gets or sets a value that indicates the horizontal alignment of text content.
        /// </summary>
        /// <returns>
        /// The text alignment. The default is <see cref="TextAlignment.Left"/>.
        /// </returns>
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextDecorations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty =
            Inline.TextDecorationsProperty.AddOwner(typeof(TextBlock));

        /// <summary>
        /// Gets or sets a value that specifies the text decorations that are applied to
        /// the content in a <see cref="TextBlock"/> element.
        /// </summary>
        /// <returns>
        /// A <see cref="TextDecorationCollection"/>, or null if no text decorations are
        /// applied.
        /// </returns>
        public TextDecorationCollection TextDecorations
        {
            get => (TextDecorationCollection)GetValue(TextDecorationsProperty);
            set => SetValue(TextDecorationsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnTextChanged,
                    CoerceText));

        /// <summary>
        /// Gets or sets the text contents of a <see cref="TextBlock"/>.
        /// </summary>
        /// <returns>
        /// A string that specifies the text contents of this <see cref="TextBlock"/>.
        /// The default is an empty string.
        /// </returns>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = (TextBlock)d;
            var text = (string)e.NewValue;
            if (tb._textContentChanging)
            {
                // The update originated in a TextContainer change -- don't update
                // the TextContainer a second time.
                return;
            }

            tb._textContentChanging = true;

            try
            {
                if (tb.Inlines.Count == 1 && tb.Inlines[0] is Run singleRun)
                {
                    singleRun.Text = text;
                }
                else
                {
                    tb.Inlines.Clear();
                    tb.Inlines.Add(new Run { Text = text });
                }
            }
            finally
            {
                tb._textContentChanging = false;
            }
        }

        private static object CoerceText(DependencyObject d, object value)
        {
            return (string)value ?? string.Empty;
        }

        internal void OnTextContentChanged()
        {
            if (!_textContentChanging)
            {
                _textContentChanging = true;
                try
                {
                    SetCurrentValue(TextProperty, Inlines.TextContainer.Text);
                }
                finally
                {
                    _textContentChanging = false;
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="TextTrimming"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register(
                nameof(TextTrimming),
                typeof(TextTrimming),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(TextTrimming.None, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBlock)d).SetTextTrimming((TextTrimming)newValue),
                });

        /// <summary>
        /// Gets or sets the text trimming behavior to employ when content overflows the
        /// content area.
        /// </summary>
        /// <returns>
        /// One of the <see cref="Windows.TextTrimming"/> values that specifies the text trimming
        /// behavior to employ. The default is <see cref="TextTrimming.None"/>.
        /// </returns>
        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextWrapping"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(
                nameof(TextWrapping),
                typeof(TextWrapping),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextBlock)d).SetTextWrapping((TextWrapping)newValue),
                });

        /// <summary>
        /// Gets or sets how the <see cref="TextBlock"/> wraps text.
        /// </summary>
        /// <returns>
        /// A value that indicates how the <see cref="TextBlock"/> wraps text.
        /// The default is <see cref="TextWrapping.NoWrap"/>.
        /// </returns>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        private static readonly DependencyProperty InlinesProperty =
            DependencyProperty.Register(
                nameof(Inlines),
                typeof(InlineCollection),
                typeof(TextBlock),
                null);

        /// <summary>
        /// Gets the collection of inline text elements within a <see cref="TextBlock"/>.
        /// </summary>
        /// <returns>
        /// A collection that holds all inline text elements from the <see cref="TextBlock"/>.The
        /// default is an empty collection.
        /// </returns>
        public InlineCollection Inlines => (InlineCollection)GetValue(InlinesProperty);

        /// <summary>
        /// Returns a value by which each line of text is offset from a baseline.
        /// </summary>
        /// <returns>
        /// The amount by which each line of text is offset from the baseline, in device
        /// independent pixels. <see cref="double.NaN"/> indicates that an optimal baseline offset
        /// is automatically calculated from the current font characteristics. The default
        /// is 0.0.
        /// </returns>
        public double BaselineOffset => GetBaseLineOffset(this);

        private static double GetBaseLineOffset(TextBlock tb)
        {
            if (!string.IsNullOrEmpty(tb.Text))
            {
                return tb.GetBaseLineOffset();
            }

            return 0.0;
        }

        /// <summary>
        /// Gets or sets the font source that is applied to the text for rendering content.
        /// </summary>
        /// <returns>
        /// The font source that is used to render content in the text box. The default is null.
        /// </returns>
        [OpenSilver.NotImplemented]
        public FontSource FontSource { get; set; }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new TextBlockAutomationPeer(this);

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = INTERNAL_HtmlDomManager.CreateTextBlockDomElementAndAppendIt(parentRef, this, TextWrapping == TextWrapping.Wrap);
            domElementWhereToPlaceChildren = div;
            return div;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            foreach (Inline child in Inlines)
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Thickness padding = Padding;
            double paddingWidth = padding.Left + padding.Right;
            double paddingHeight = padding.Top + padding.Bottom;

            if (string.IsNullOrEmpty(Text))
            {
                return new Size(paddingWidth, paddingHeight);
            }

            if (_noWrapSize == Size.Empty)
            {
                _noWrapSize = ParentWindow.TextMeasurementService.MeasureText(
                    OuterDiv.UniqueIdentifier,
                    "pre",
                    string.Empty,
                    double.PositiveInfinity,
                    string.Empty);
            }

            if (TextWrapping == TextWrapping.NoWrap || (_noWrapSize.Width + paddingWidth) <= availableSize.Width)
            {
                var desiredSize = new Size(_noWrapSize.Width + paddingWidth, _noWrapSize.Height + paddingHeight);

                if (TextTrimming != TextTrimming.None)
                {
                    // Note: Silverlight does not clip here, however we need to do it for the
                    // css 'text-overflow' property to work.
                    desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
                    desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);
                }

                return desiredSize;
            }

            Size textSize = ParentWindow.TextMeasurementService.MeasureText(
                OuterDiv.UniqueIdentifier,
                "pre-wrap",
                "break-word",
                Math.Max(0, availableSize.Width - paddingWidth),
                string.Empty);

            return new Size(textSize.Width + paddingWidth, textSize.Height + paddingHeight);
        }

        protected override Size ArrangeOverride(Size finalSize) => finalSize;

        internal override bool EnablePointerEventsCore => true;

        internal override int VisualChildrenCount => Inlines.Count;

        internal override UIElement GetVisualChild(int index)
        {
            if (index >= VisualChildrenCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Inlines[index];
        }

        internal sealed override void AddEventListeners() => InputManager.Current.AddEventListeners(this, false);

        internal override string GetPlainText() => Text;

        internal static void ApplyTextWrapping(INTERNAL_HtmlDomStyleReference cssStyle, TextWrapping textWrapping)
        {
            Debug.Assert(cssStyle != null);
            switch (textWrapping)
            {
                case TextWrapping.Wrap:
                    cssStyle.whiteSpace = "pre-wrap";
                    cssStyle.overflowWrap = "break-word";
                    break;

                case TextWrapping.NoWrap:
                default:
                    cssStyle.whiteSpace = "pre";
                    cssStyle.overflowWrap = string.Empty;
                    break;
            }
        }

        internal override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Metadata is FrameworkPropertyMetadata metadata)
            {
                if (metadata.AffectsMeasure)
                {
                    _noWrapSize = Size.Empty;
                }
            }
        }

        internal void InvalidateCacheAndMeasure()
        {
            _noWrapSize = Size.Empty;
            InvalidateMeasure();
        }
    }
}
