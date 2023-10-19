
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
using System.Windows.Documents;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that displays read-only rich text.
    /// </summary>
    [ContentProperty(nameof(Blocks))]
    public sealed partial class RichTextBlock : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBlock"/> class.
        /// </summary>
        public RichTextBlock()
        {
            Blocks = new BlockCollection(this, false);
        }

        internal sealed override bool EnablePointerEventsCore => true;

        /// <summary>
        /// Gets the contents of the <see cref="RichTextBlock"/>.
        /// </summary>
        public BlockCollection Blocks { get; }

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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var domStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        var padding = (Thickness)newValue;
                        domStyle.padding = $"{padding.Top.ToInvariantString()}px {padding.Right.ToInvariantString()}px {padding.Bottom.ToInvariantString()}px {padding.Left.ToInvariantString()}px";
                    }
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
            set => SetValue(PaddingProperty, value);
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
            TextElementProperties.FontSizeProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    11d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        style.fontSize = $"{((double)newValue).ToInvariantString()}px";
                    },
                });

        /// <summary>
        /// Gets or sets the size of the text in this control.
        /// The default is 11 (in pixels).
        /// </summary>
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            TextElementProperties.FontWeightProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    FontWeights.Normal,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        style.fontWeight = ((FontWeight)newValue).ToOpenTypeWeight().ToInvariantString();
                    },
                });

        /// <summary>
        /// Gets or sets the thickness of the specified font.
        /// The default is <see cref="FontWeights.Normal"/>.
        /// </summary>
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register(
                nameof(FontStyle),
                typeof(FontStyle),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        style.fontStyle = ((FontStyle)newValue).ToString().ToLower();
                    },
                });

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
            set => SetValue(FontStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            TextElementProperties.FontFamilyProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(FontFamily.Default, FrameworkPropertyMetadataOptions.Inherits, OnFontFamilyChanged)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        style.fontFamily = ((FontFamily)newValue).GetFontFace(rtb).CssFontName;
                    },
                });

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// The default is the "Portable User Interface".
        /// </summary>
        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextElementProperties.InvalidateMeasureOnFontFamilyChanged((RichTextBlock)d, (FontFamily)e.NewValue);
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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => TextBlock.ApplyTextWrapping(
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(((RichTextBlock)d).INTERNAL_OuterDomElement),
                        (TextWrapping)newValue),
                });

        /// <summary>
        /// Gets or sets how text wrapping occurs if a line of text extends beyond the available
        /// width of the <see cref="RichTextBlock"/>.
        /// The default is <see cref="TextWrapping.Wrap"/>.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TextAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(
                nameof(TextAlignment),
                typeof(TextAlignment),
                typeof(RichTextBlock),
                new PropertyMetadata(TextAlignment.Left)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        style.textAlign = (TextAlignment)newValue switch
                        {
                            TextAlignment.Center => "center",
                            TextAlignment.Right => "end",
                            TextAlignment.Justify => "justify",
                            _ => "start",
                        };
                    },
                });

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
            set => SetValue(TextAlignmentProperty, value);
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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        style.textOverflow = (TextTrimming)newValue switch
                        {
                            TextTrimming.WordEllipsis or TextTrimming.CharacterEllipsis => "ellipsis",
                            _ => "clip",
                        };
                    },
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
        /// Identifies the <see cref="IsTextSelectionEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTextSelectionEnabledProperty =
            DependencyProperty.Register(
                nameof(IsTextSelectionEnabled),
                typeof(bool),
                typeof(RichTextBlock),
                new PropertyMetadata(true)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        style.userSelect = (bool)newValue ? "auto" : "none";
                    },
                });

        /// <summary>
        /// Gets or sets a value that indicates whether text selection is enabled
        /// in <see cref="RichTextBlock"/>.
        /// </summary>
        public bool IsTextSelectionEnabled
        {
            get => (bool)GetValue(IsTextSelectionEnabledProperty);
            set => SetValue(IsTextSelectionEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                nameof(Foreground),
                typeof(Brush),
                typeof(RichTextBlock),
                new PropertyMetadata(new SolidColorBrush(Colors.Black))
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(rtb);
                        switch (newValue)
                        {
                            case SolidColorBrush scb:
                                style.color = scb.INTERNAL_ToHtmlString();
                                break;

                            case null:
                                style.color = string.Empty;
                                break;

                            default:
                                // GradientBrush, ImageBrush and custom brushes are not supported.
                                // Keep using old brush.
                                break;
                        }
                    },
                });

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
            set => SetValue(ForegroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CharacterSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterSpacingProperty =
            TextElementProperties.CharacterSpacingProperty.AddOwner(
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var rtb = (RichTextBlock)d;
                        double value = (int)newValue / 1000.0;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(rtb.INTERNAL_OuterDomElement);
                        style.letterSpacing = $"{value.ToInvariantString()}em";
                    },
                });

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
        /// Gets a value that represents the offset in pixels from the top of the content
        /// to the baseline of the first paragraph. The baseline of the paragraph is the
        /// baseline of the first line in it.
        /// </summary>
        /// <returns>
        /// The computed baseline for the first paragraph, or 0 if the <see cref="RichTextBlock"/>
        /// is empty.
        /// </returns>
        public double BaselineOffset => GetBaseLineOffset(this);

        private static double GetBaseLineOffset(RichTextBlock rtb)
        {
            if (rtb.Blocks.Count > 0)
            {
                return TextElementProperties.GetBaseLineOffsetNative(rtb);
            }

            return 0.0;
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = INTERNAL_HtmlDomManager.CreateTextBlockDomElementAndAppendIt(parentRef, this, TextWrapping == TextWrapping.Wrap);
            domElementWhereToPlaceChildren = div;
            return div;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();
            foreach (var block in Blocks)
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

            Size textSize = INTERNAL_ParentWindow.TextMeasurementService.MeasureText(
                ((INTERNAL_HtmlDomElementReference)INTERNAL_OuterDomElement).UniqueIdentifier,
                wrap ? "pre-wrap" : "pre",
                wrap ? "break-word" : string.Empty,
                wrap ? Math.Max(0, availableSize.Width - paddingWidth) : double.PositiveInfinity,
                string.Empty);

            return new Size(textSize.Width + paddingWidth, textSize.Height + paddingHeight);
        }

        protected override Size ArrangeOverride(Size finalSize) => finalSize;
    }
}
