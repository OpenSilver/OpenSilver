
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
using System.Windows.Markup;
using System.Windows.Documents;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that displays read-only rich text.
    /// </summary>
    [ContentProperty(nameof(Blocks))]
    public sealed partial class RichTextBlock : FrameworkElement
    {
        static RichTextBlock()
        {
            IsHitTestableProperty.OverrideMetadata(typeof(RichTextBlock), new PropertyMetadata(BooleanBoxes.TrueBox));
        }

        private BlockCollection _blocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBlock"/> class.
        /// </summary>
        public RichTextBlock()
        {
            SetValueInternal(BlocksProperty, new BlockCollection(this));
        }

        internal sealed override bool EnablePointerEventsCore => true;

        private static readonly DependencyProperty BlocksProperty =
            DependencyProperty.Register(
                nameof(Blocks),
                typeof(BlockCollection),
                typeof(RichTextBlock),
                new PropertyMetadata(null, OnBlocksChanged));

        /// <summary>
        /// Gets the contents of the <see cref="RichTextBlock"/>.
        /// </summary>
        public BlockCollection Blocks => _blocks;

        private static void OnBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBlock)d)._blocks = (BlockCollection)e.NewValue;
        }

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBlock)d).SetPadding((Thickness)newValue),
                },
                IsPaddingValid);

        /// <summary>
        /// Gets or sets a value that indicates the thickness of padding space between the
        /// boundaries of the content area and the content displayed by a <see cref="RichTextBlock"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Thickness"/> structure that specifies the amount of padding to apply.
        /// </returns>
        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValueInternal(PaddingProperty, value);
        }

        private static bool IsPaddingValid(object value)
        {
            Thickness t = (Thickness)value;
            return Thickness.IsValid(t, false, false, false, false);
        }

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    11d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets or sets the size of the text in this control.
        /// The default is 11 (in pixels).
        /// </summary>
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValueInternal(FontSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    FontWeights.Normal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets or sets the thickness of the specified font.
        /// The default is <see cref="FontWeights.Normal"/>.
        /// </summary>
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValueInternal(FontWeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    FontStyles.Normal,
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the style in which the text is rendered.
        /// </summary>
        /// <returns>
        /// One of the values that specifies the style in which the text is rendered. The
        /// default is <see cref="FontStyles.Normal"/>.
        /// </returns>
        public FontStyle FontStyle
        {
            get => (FontStyle)GetValue(FontStyleProperty);
            set => SetValueInternal(FontStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(FontFamily.Default, FrameworkPropertyMetadataOptions.Inherits, OnFontFamilyChanged));

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// The default is the "Portable User Interface".
        /// </summary>
        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValueInternal(FontFamilyProperty, value);
        }

        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElementHelpers.InvalidateMeasureOnFontFamilyChanged((RichTextBlock)d, (FontFamily)e.NewValue);
        }

        /// <summary>
        /// Identifies the <see cref="TextWrapping"/> dependency
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(
                nameof(TextWrapping),
                typeof(TextWrapping),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(TextWrapping.Wrap, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBlock)d).SetTextWrapping((TextWrapping)newValue),
                });

        /// <summary>
        /// Gets or sets how text wrapping occurs if a line of text extends beyond the available
        /// width of the <see cref="RichTextBlock"/>.
        /// The default is <see cref="TextWrapping.Wrap"/>.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValueInternal(TextWrappingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            Block.TextAlignmentProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets or sets how the text should be aligned in the <see cref="RichTextBlock"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="Windows.TextAlignment"/> enumeration values.
        /// The default is <see cref="TextAlignment.Left"/>.
        /// </returns>
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValueInternal(TextAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextTrimming"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register(
                nameof(TextTrimming),
                typeof(TextTrimming),
                typeof(RichTextBlock),
                new PropertyMetadata(TextTrimming.None)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBlock)d).SetTextTrimming((TextTrimming)newValue),
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
            set => SetValueInternal(TextTrimmingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsTextSelectionEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTextSelectionEnabledProperty =
            DependencyProperty.Register(
                nameof(IsTextSelectionEnabled),
                typeof(bool),
                typeof(RichTextBlock),
                new PropertyMetadata(true)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBlock)d).SetTextSelection((bool)newValue),
                });

        /// <summary>
        /// Gets or sets a value that indicates whether text selection is enabled
        /// in <see cref="RichTextBlock"/>.
        /// </summary>
        public bool IsTextSelectionEnabled
        {
            get => (bool)GetValue(IsTextSelectionEnabledProperty);
            set => SetValueInternal(IsTextSelectionEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    TextElement.ForegroundProperty.DefaultMetadata.DefaultValue,
                    FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        /// <remarks>
        /// The brush that paints the foreground of the control.
        /// The default value is <see cref="Colors.Black"/>.
        /// </remarks>
        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValueInternal(ForegroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CharacterSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterSpacingProperty =
            TextElement.CharacterSpacingProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

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
            set => SetValueInternal(CharacterSpacingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LineHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty =
            Block.LineHeightProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///  Gets or sets the height of each line of content.
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
        /// Identifies the <see cref="LineStackingStrategy"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStackingStrategyProperty =
            Block.LineStackingStrategyProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    LineStackingStrategy.MaxHeight,
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ((RichTextBlock)d).SetLineStackingStrategy((LineStackingStrategy)newValue),
                });

        /// <summary>
        /// Gets or sets a value that indicates how a line box is determined for each line
        /// of text in the <see cref="RichTextBlock"/>.
        /// </summary>
        /// <returns>
        /// A value that indicates how a line box is determined for each line of text in
        /// the <see cref="RichTextBlock"/>. The default is <see cref="LineStackingStrategy.MaxHeight"/>.
        /// </returns>
        public LineStackingStrategy LineStackingStrategy
        {
            get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
            set => SetValueInternal(LineStackingStrategyProperty, value);
        }

        /// <summary>
        /// Gets a value that represents the offset in pixels from the top of the content
        /// to the baseline of the first paragraph. The baseline of the paragraph is the
        /// baseline of the first line in it.
        /// </summary>
        /// <returns>
        /// The computed baseline for the first paragraph, or 0 if the <see cref="RichTextBlock"/>
        /// is empty.
        /// </returns>
        public double BaselineOffset
        {
            get
            {
                if (Blocks.Count > 0 && Application.Current is Application app)
                {
                    return app.MainWindow.TextMeasurementService.MeasureBaseline(GetFonts(this));
                }

                return 0.0;

                static IEnumerable<FontProperties> GetFonts(RichTextBlock richtextblock)
                {
                    foreach (Block block in richtextblock.Blocks.InternalItems)
                    {
                        foreach (FontProperties font in GetFontsRecursive(block, block))
                        {
                            yield return font;
                        }
                    }

                    static IEnumerable<FontProperties> GetFontsRecursive(Block block, UIElement current)
                    {
                        int count = current.VisualChildrenCount;
                        for (int i = 0; i < count; i++)
                        {
                            switch (current.GetVisualChild(i))
                            {
                                case Run run:
                                    if (!string.IsNullOrEmpty(run.Text))
                                    {
                                        yield return new FontProperties
                                        {
                                            FontStyle = run.FontStyle,
                                            FontWeight = run.FontWeight,
                                            FontSize = run.FontSize,
                                            LineHeight = block.LineHeight,
                                            FontFamily = run.FontFamily,
                                        };
                                    }
                                    break;

                                case Block b:
                                    foreach (FontProperties font in GetFontsRecursive(b, b))
                                    {
                                        yield return font;
                                    }
                                    break;

                                case TextElement textElement:
                                    foreach (FontProperties font in GetFontsRecursive(block, textElement))
                                    {
                                        yield return font;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            return INTERNAL_HtmlDomManager.CreateTextBlockDomElementAndAppendIt(parentRef, this);
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();
            foreach (var block in Blocks.InternalItems)
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(block, this);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Thickness padding = Padding;
            double paddingWidth = padding.Left + padding.Right;
            double paddingHeight = padding.Top + padding.Bottom;
            bool wrap = TextWrapping == TextWrapping.Wrap;

            Size textSize = ParentWindow.TextMeasurementService.MeasureView(
                OuterDiv.UniqueIdentifier,
                wrap ? "pre-wrap" : "pre",
                wrap ? "break-word" : string.Empty,
                wrap ? Math.Max(0, availableSize.Width - paddingWidth) : double.PositiveInfinity,
                string.Empty);

            return new Size(textSize.Width + paddingWidth, textSize.Height + paddingHeight);
        }

        protected override Size ArrangeOverride(Size finalSize) => finalSize;

        internal sealed override int VisualChildrenCount => Blocks.Count;

        internal sealed override UIElement GetVisualChild(int index)
        {
            if (index < 0 || index >= VisualChildrenCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Blocks.InternalItems[index];
        }
    }
}
