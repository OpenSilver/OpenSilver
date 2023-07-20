
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

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	/// <summary>
	/// An abstract class that provides a base for all block-level content elements.
	/// </summary>
	public abstract class Block : TextElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Block" /> class. 
		/// </summary>
		protected Block()
		{
		}

		/// <summary>
		/// Identifies the <see cref="LineHeight" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty LineHeightProperty =
			DependencyProperty.Register(
				nameof(LineHeight),
				typeof(double),
				typeof(Block),
				new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

		/// <summary>
		/// Gets or sets the height of each line of content.
		/// </summary>
		/// <returns>
		/// The height of each line in pixels. A value of 0 indicates that the line
		/// height is determined automatically from the current font characteristics. 
		/// The default is 0.
		/// </returns>
        [OpenSilver.NotImplemented]
		public double LineHeight
		{
			get => (double)GetValue(LineHeightProperty);
			set => SetValue(LineHeightProperty, value);
		}

		/// <summary>
		/// Identifies the <see cref="LineStackingStrategy" /> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty LineStackingStrategyProperty =
			DependencyProperty.Register(
				nameof(LineStackingStrategy),
				typeof(LineStackingStrategy),
				typeof(Block),
				new PropertyMetadata(LineStackingStrategy.MaxHeight));

		/// <summary>
		/// Gets or sets a value that indicates how a line box is determined for each 
		/// line of text in a <see cref="Block" />.
		/// The default is <see cref="LineStackingStrategy.MaxHeight" />.
		/// </summary>
        [OpenSilver.NotImplemented]
		public LineStackingStrategy LineStackingStrategy
		{
			get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
			set => SetValue(LineStackingStrategyProperty, value);
		}

		/// <summary>
		/// Identifies the <see cref="TextAlignment" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty TextAlignmentProperty =
			DependencyProperty.Register(
				nameof(TextAlignment),
				typeof(TextAlignment),
				typeof(Block),
				new PropertyMetadata(TextAlignment.Left)
				{
                    MethodToUpdateDom2 = static (d, oldValue, newValue) =>
                    {
                        var block = (Block)d;
                        var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(block.INTERNAL_OuterDomElement);
                        style.textAlign = (TextAlignment)newValue switch
                        {
                            TextAlignment.Center => "center",
                            TextAlignment.Right => "right",
                            TextAlignment.Justify => "justify",
                            _ => "left",
                        };
                    },
                });

		/// <summary>
		/// Gets or sets the horizontal alignment of the text content. 
		/// The default is <see cref="TextAlignment.Left" />.
		/// </summary>
		public TextAlignment TextAlignment
		{
			get => (TextAlignment)GetValue(TextAlignmentProperty);
			set => SetValue(TextAlignmentProperty, value);
		}

		internal abstract string GetContainerText();
	}
}
