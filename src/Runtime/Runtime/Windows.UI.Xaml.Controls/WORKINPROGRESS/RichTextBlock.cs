
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

#if WORKINPROGRESS

using System;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Documents;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that displays read-only rich text.
    /// </summary>
    [ContentProperty("Blocks")]
    [OpenSilver.NotImplemented]
    public sealed class RichTextBlock : FrameworkElement
    {
        #region Public Properties

        /// <summary>
        /// Gets the contents of the <see cref="RichTextBlock"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public BlockCollection Blocks { get; }

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="RichTextBlock.FontSize"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
                "FontSize",
                typeof(double),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(11d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the size of the text in this control.
        /// The default is 11 (in pixels).
        /// </summary>
        [OpenSilver.NotImplemented]
        public double FontSize
        {
            get { return (double)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RichTextBlock.FontWeight"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(
                "FontWeight",
                typeof(FontWeight),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the thickness of the specified font.
        /// The default is <see cref="FontWeights.Normal"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public FontWeight FontWeight
        {
            get { return (FontWeight)this.GetValue(FontWeightProperty); }
            set { this.SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RichTextBlock.FontFamily"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                "FontFamily",
                typeof(FontFamily),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// The default is the "Portable User Interface".
        /// </summary>
        [OpenSilver.NotImplemented]
        public FontFamily FontFamily
        {
            get { return (FontFamily)this.GetValue(FontFamilyProperty); }
            set { this.SetValue(FontFamilyProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty OverflowContentTargetProperty =
            DependencyProperty.Register(
                "OverflowContentTarget",
                typeof(RichTextBlockOverflow),
                typeof(RichTextBlock),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the <see cref="RichTextBlockOverflow"/> that will consume
        /// the overflow content of this <see cref="RichTextBlock"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public RichTextBlockOverflow OverflowContentTarget
        {
            get { return (RichTextBlockOverflow)this.GetValue(OverflowContentTargetProperty); }
            set { this.SetValue(OverflowContentTargetProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RichTextBlock.TextWrapping"/> dependency
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(
                "TextWrapping",
                typeof(TextWrapping),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(TextWrapping.Wrap, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets how text wrapping occurs if a line of text extends beyond the available
        /// width of the <see cref="RichTextBlock"/>.
        /// The default is <see cref="TextWrapping.Wrap"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)this.GetValue(TextWrappingProperty); }
            set { this.SetValue(TextWrappingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RichTextBlock.LineHeight"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register(
                "LineHeight",
                typeof(double),
                typeof(RichTextBlock),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///  Gets or sets the height of each line of content.
        /// </summary>
        /// <returns>
        /// The height of each line in pixels. A value of 0 indicates that the line height
        /// is determined automatically from the current font characteristics. The default
        /// is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double LineHeight
        {
            get { return (double)this.GetValue(LineHeightProperty); }
            set { this.SetValue(LineHeightProperty, value); }
        }

        #endregion Dependency Properties

        #endregion Public Properties
    }
}

#endif