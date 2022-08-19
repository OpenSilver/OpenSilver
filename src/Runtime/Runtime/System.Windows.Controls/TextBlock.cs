

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Documents;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Documents;
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
    [ContentProperty("Inlines")]
    public partial class TextBlock : Control //todo: this is supposed to inherit from FrameworkElement but Control has the implementations of FontSize, FontWeight, Foreground, etc. Maybe use an intermediate class between FrameworkElement and Control or add the implementation here too.
    {
        internal override int VisualChildrenCount
        {
            get { return this.Inlines.Count; }
        }

        internal override UIElement GetVisualChild(int index)
        {
            if (index >= VisualChildrenCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return (Inline)this.Inlines[index];
        }

        private bool _isTextChanging;

        private Size _measuredSize;

        public TextBlock()
        {
            this.IsTabStop = false; //we want to avoid stopping on this element's div when pressing tab.
            this.Inlines = new InlineCollection(this);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new TextBlockAutomationPeer(this);

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = INTERNAL_HtmlDomManager.CreateTextBlockDomElementAndAppendIt(parentRef, this, TextWrapping == TextWrapping.NoWrap ? "pre" : "pre-wrap");
            domElementWhereToPlaceChildren = div;
            return div;
        }

        internal sealed override NativeEventsManager CreateEventsManager()
        {
            return new NativeEventsManager(this, this, this, false);
        }

        internal override string GetPlainText() => Text;

        internal override bool EnablePointerEventsCore
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Get or Set the Text property
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
                "Text",
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
            if (!this._isTextChanging)
            {
                this._isTextChanging = true;
                this.SetCurrentValue(TextProperty, text);
                this._isTextChanging = false;
            }
        }

        #region Properties for formatting (TextAlignment, TextWrapping)

        /// <summary>
        /// Gets or sets how the text should be aligned in the TextBlock.
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
                "TextAlignment", 
                typeof(TextAlignment), 
                typeof(TextBlock), 
                new PropertyMetadata(TextAlignment.Left) 
                { 
                    MethodToUpdateDom = TextAlignment_MethodToUpdateDom
                });

        static void TextAlignment_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var textBlock = (TextBlock)d;
            switch ((TextAlignment)newValue)
            {
                case TextAlignment.Center:
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(textBlock).textAlign = "center";
                    break;
                case TextAlignment.Left:
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(textBlock).textAlign = "left";
                    break;
                case TextAlignment.Right:
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(textBlock).textAlign = "right";
                    break;
                case TextAlignment.Justify:
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(textBlock).textAlign = "justify";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets or sets how the TextBlock wraps text.
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
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextBlock),
                new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Value = (inst, value) =>
                            {
                                TextWrapping newTextWrapping = (TextWrapping)value;
                                switch (newTextWrapping)
                                {
                                    case TextWrapping.Wrap:
                                        return "pre-wrap"; //wrap + preserve whitespaces
                                    case TextWrapping.NoWrap:
                                    default:
                                        return "pre"; //nowrap + preserve whitespaces
                                }
                            },
                        Name = new List<string> { "whiteSpace" },
                    };
                }
            }
            );

        #endregion

        public InlineCollection Inlines { get; }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            foreach (Inline child in this.Inlines)
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, this);
            }
        }

        internal override void UpdateTabIndex(bool isTabStop, int tabIndex)
        {
            // we don't do anything since TextBlock is not supposed to be a Control in the first place
            // and it is not supposed to be counted in tabbing
            return;
        }

        public static readonly DependencyProperty TextTrimmingProperty = 
            DependencyProperty.Register(
                "TextTrimming",
                typeof(TextTrimming),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(TextTrimming.None, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom = OnTextTrimmedChangedUpdateDOM
                });

        private static void OnTextTrimmedChangedUpdateDOM(DependencyObject d, object newValue)
        {
            var style = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification((TextBlock)d);
            switch ((TextTrimming)newValue)
            {
                case TextTrimming.CharacterEllipsis:
                case TextTrimming.WordEllipsis:
                    style.textOverflow = "ellipsis";
                    break;
                default:
                    style.textOverflow = "clip";
                    break;
            }
        }

        /// <summary>
        /// Gets or sets how the TextBlock trims text.
        /// </summary>
        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }


        // Note on LineHeight: We could make use of the browser's capabilities (see https://developer.mozilla.org/en-US/docs/Web/CSS/line-height) to improve this property.
        //      For that, we would need a class with an implicit cast from/to double (so the original way of using it would still work), and that would be able to parse strings such as 125% (in that case, getting its value as a double would return the FontSize * 1.25)

        // Returns:
        //     The height of each line in pixels. A value of 0 indicates that the line height
        //     is determined automatically from the current font characteristics. The default
        //     is 0.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     System.Windows.Controls.TextBlock.LineHeight is set to a non-positive value.
        //[OpenSilver.NotImplemented]
        /// <summary>
        /// Gets or sets the height of each line of content.
        /// </summary>
        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }
        /// <summary>
        /// Identifies the LineHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register("LineHeight", typeof(double), typeof(TextBlock), new PropertyMetadata(0d)
            {
                MethodToUpdateDom = (instance, value) =>
                {
                    double valueAsDouble = (double)value;
                    if(valueAsDouble < 0)
                    {
                        throw new ArgumentException("TextBlock.LineHeight is set to a non-positive value.");
                    }
                    if(valueAsDouble > 0)
                    {
                        ((TextBlock)instance).UpdateCSSLineHeight(value.ToString() + "px");
                    }
                    else
                    {
                        ((TextBlock)instance).UpdateCSSLineHeight("normal"); //todo: adapt this to what the value would exactly be in Silverlight (probably something like "125%" but I'm not sure of the exact value)
                    }
                }
            });

        private void UpdateCSSLineHeight(string value)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                var domStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this);
                domStyle.lineHeight = value ?? "";
            }
        }

		private Size noWrapSize = Size.Empty;

		internal override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			// Skip when loading or changed on TextMeasurement Div.
			if (this.INTERNAL_OuterDomElement == null || Application.Current.TextMeasurementService.IsTextMeasureDivID(((INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement).UniqueIdentifier))
				return;

			FrameworkPropertyMetadata metadata = e.Property.GetMetadata(GetType()) as FrameworkPropertyMetadata;

			if (metadata != null)
			{
				if (metadata.AffectsMeasure)
				{
					noWrapSize = Size.Empty;
				}
			}
			base.OnPropertyChanged(e);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
            if (!double.IsNaN(this.Width) && !double.IsNaN(this.Height))
            {
                return new Size(this.Width, this.Height);
            }
            //Size actualSize = this.INTERNAL_GetActualWidthAndHeight();
            //return actualSize;
            Size BorderThicknessSize = new Size(BorderThickness.Left + BorderThickness.Right, BorderThickness.Top + BorderThickness.Bottom);

            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement).UniqueIdentifier;

            if (noWrapSize == Size.Empty)
            {
                noWrapSize = Application.Current.TextMeasurementService.MeasureTextBlock(uniqueIdentifier, TextWrapping.NoWrap, Padding, Double.PositiveInfinity);
                noWrapSize = noWrapSize.Add(BorderThicknessSize);
            }

            if (TextWrapping == TextWrapping.NoWrap || noWrapSize.Width <= availableSize.Width)
            {
                _measuredSize = noWrapSize;
                return noWrapSize;
            }

            Size TextSize = Application.Current.TextMeasurementService.MeasureTextBlock(uniqueIdentifier, TextWrapping, Padding, (availableSize.Width - BorderThicknessSize.Width).Max(0));
            TextSize = TextSize.Add(BorderThicknessSize);

            _measuredSize = TextSize;
            return TextSize;
        }

		protected override Size ArrangeOverride(Size finalSize)
		{
            double w = Math.Max(_measuredSize.Width, finalSize.Width);
            double h = Math.Max(_measuredSize.Height, finalSize.Height);

            return new Size(w, h);
		}
    }
}
