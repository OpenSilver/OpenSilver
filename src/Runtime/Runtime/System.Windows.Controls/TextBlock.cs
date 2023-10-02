﻿
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
using System.Diagnostics;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Input;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
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
    public partial class TextBlock : Control //todo: this is supposed to inherit from FrameworkElement but Control has the implementations of FontSize, FontWeight, Foreground, etc. Maybe use an intermediate class between FrameworkElement and Control or add the implementation here too.
    {
        private bool _isTextChanging;
        private Size _noWrapSize = Size.Empty;

        internal override int VisualChildrenCount => Inlines.Count;

        internal override UIElement GetVisualChild(int index)
        {
            if (index >= VisualChildrenCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return (Inline)Inlines[index];
        }
        
        public TextBlock()
        {
            IsTabStop = false; //we want to avoid stopping on this element's div when pressing tab.
            Inlines = new InlineCollection(this);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new TextBlockAutomationPeer(this);

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = INTERNAL_HtmlDomManager.CreateTextBlockDomElementAndAppendIt(parentRef, this, TextWrapping == TextWrapping.Wrap);
            domElementWhereToPlaceChildren = div;
            return div;
        }

        internal sealed override void AddEventListeners() => InputManager.Current.AddEventListeners(this, false);

        internal override string GetPlainText() => Text;

        internal override bool EnablePointerEventsCore => true;

        /// <summary>
        /// Get or Set the Text property
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)d;
            if (!textBlock._isTextChanging)
            {
                textBlock._isTextChanging = true;
                if (textBlock.Inlines.Count == 1 && textBlock.Inlines[0] as Run != null)
                {
                    (textBlock.Inlines[0] as Run).Text = (string)e.NewValue;
                }
                else
                {
                    textBlock.Inlines.Clear();
                    textBlock.Inlines.Add(new Run() { Text = (string)e.NewValue });
                }
                textBlock._isTextChanging = false;
            }
        }

        internal void SetTextPropertyNoCallBack(string text)
        {
            if (!_isTextChanging)
            {
                _isTextChanging = true;
                SetCurrentValue(TextProperty, text);
                _isTextChanging = false;
            }
        }

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var tb = (TextBlock)d;
                        var domStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(tb.INTERNAL_OuterDomElement);
                        var padding = (Thickness)newValue;
                        domStyle.padding = $"{padding.Top.ToInvariantString()}px {padding.Right.ToInvariantString()}px {padding.Bottom.ToInvariantString()}px {padding.Left.ToInvariantString()}px";
                    }
                },
                IsPaddingValid);

        /// <summary>
        /// Gets or sets a value that indicates the thickness of padding space between the
        /// boundaries of the content area and the content displayed by a <see cref="TextBlock"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Thickness"/> structure that specifies the amount of padding to apply.
        /// </returns>
        public new Thickness Padding
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
            DependencyProperty.Register(
                nameof(TextAlignment),
                typeof(TextAlignment),
                typeof(TextBlock),
                new PropertyMetadata(TextAlignment.Left)
                {
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var tb = (TextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(tb.INTERNAL_OuterDomElement);
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
        /// Gets or sets how the text should be aligned in the TextBlock.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) => ApplyTextWrapping(
                        INTERNAL_HtmlDomManager.GetDomElementStyleForModification(((TextBlock)d).INTERNAL_OuterDomElement),
                        (TextWrapping)newValue),
                });

        /// <summary>
        /// Gets or sets how the TextBlock wraps text.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

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

        public InlineCollection Inlines { get; }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            foreach (Inline child in Inlines)
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
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
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var tb = (TextBlock)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(tb.INTERNAL_OuterDomElement);
                        style.textOverflow = (TextTrimming)newValue switch
                        {
                            TextTrimming.WordEllipsis or TextTrimming.CharacterEllipsis => "ellipsis",
                            _ => "clip",
                        };
                    },
                });

        /// <summary>
        /// Gets or sets how the TextBlock trims text.
        /// </summary>
        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LineHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register(
                nameof(LineHeight),
                typeof(double),
                typeof(TextBlock),
                new PropertyMetadata(0d)
                {
                    MethodToUpdateDom = (instance, value) =>
                    {
                        double valueAsDouble = (double)value;
                        if (valueAsDouble < 0)
                        {
                            throw new ArgumentException("TextBlock.LineHeight is set to a non-positive value.");
                        }
                        if (valueAsDouble > 0)
                        {
                            ((TextBlock)instance).UpdateCSSLineHeight(value.ToString() + "px");
                        }
                        else
                        {
                            ((TextBlock)instance).UpdateCSSLineHeight("normal"); //todo: adapt this to what the value would exactly be in Silverlight (probably something like "125%" but I'm not sure of the exact value)
                        }
                    }
                });

        /// <summary>
        /// Gets or sets the height of each line of content.
        /// </summary>
        public double LineHeight
        {
            get => (double)GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value); 
        }

        private void UpdateCSSLineHeight(string value)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);
                domStyle.lineHeight = value ?? "";
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

        protected override Size MeasureOverride(Size availableSize)
        {
            Thickness padding = Padding;
            double paddingWidth = padding.Left + padding.Right;
            double paddingHeight = padding.Top + padding.Bottom;

            if (string.IsNullOrEmpty(Text))
            {
                return new Size(paddingWidth, paddingHeight);
            }

            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement).UniqueIdentifier;

            if (_noWrapSize == Size.Empty)
            {
                _noWrapSize = INTERNAL_ParentWindow.TextMeasurementService.MeasureText(
                    uniqueIdentifier,
                    "pre",
                    string.Empty,
                    double.PositiveInfinity,
                    string.Empty);
            }

            if (TextWrapping == TextWrapping.NoWrap || (_noWrapSize.Width + paddingWidth) <= availableSize.Width)
            {
                return new Size(_noWrapSize.Width + paddingWidth, _noWrapSize.Height + paddingHeight);
            }

            Size textSize = INTERNAL_ParentWindow.TextMeasurementService.MeasureText(
                uniqueIdentifier,
                "pre-wrap",
                "break-word",
                Math.Max(0, availableSize.Width - paddingWidth),
                string.Empty);

            return new Size(textSize.Width + paddingWidth, textSize.Height + paddingHeight);
        }

        protected override Size ArrangeOverride(Size finalSize) => finalSize;
    }
}
