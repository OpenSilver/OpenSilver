

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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Collections;

#if MIGRATION
using System.Windows.Documents;
#else
using Windows.UI.Text;
using Windows.UI.Xaml.Navigation;
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
        private bool _isTextChanging;

        public TextBlock()
        {
            this.IsTabStop = false; //we want to avoid stopping on this element's div when pressing tab.
            this.Inlines = new InlineCollection(this);
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            dynamic div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
            dynamic divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            divStyle.whiteSpace = TextWrapping == TextWrapping.NoWrap ? "pre" : "pre-wrap";
#if !WORKINPROGRESS
            divStyle.overflow = "hidden"; //keeps the text from overflowing despite the TextBlock's size limitations.
#endif
            divStyle.textAlign = "left"; // this is the default value.
            domElementWhereToPlaceChildren = div;
            return div;
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
#if WORKINPROGRESS
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextPropertyChanged));
#else
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(TextBlock),
                new PropertyMetadata(string.Empty, OnTextPropertyChanged));
#endif

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)d;
            if (!textBlock._isTextChanging)
            {
                textBlock._isTextChanging = true;
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(new Run() { Text = (string)e.NewValue });
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
#if WORKINPROGRESS
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextBlock), new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure)
#else
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextBlock), new PropertyMetadata(TextWrapping.NoWrap)
#endif
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

#if WORKINPROGRESS
        public static readonly DependencyProperty TextTrimmingProperty = 
            DependencyProperty.Register(
                "TextTrimming",
                typeof(TextTrimming),
                typeof(TextBlock),
                new FrameworkPropertyMetadata(TextTrimming.None, FrameworkPropertyMetadataOptions.AffectsMeasure)
#else
        public static readonly DependencyProperty TextTrimmingProperty = 
            DependencyProperty.Register(
                "TextTrimming",
                typeof(TextTrimming),
                typeof(TextBlock),
                new PropertyMetadata(TextTrimming.None)
#endif

                {
                    MethodToUpdateDom = OnTextTrimmedChangedUpdateDOM
                });

        private static void OnTextTrimmedChangedUpdateDOM(DependencyObject d, object newValue)
        {
            dynamic style = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification((TextBlock)d);
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
    }
}
