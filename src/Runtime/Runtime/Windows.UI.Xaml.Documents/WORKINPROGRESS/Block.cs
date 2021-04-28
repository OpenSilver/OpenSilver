
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

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	/// <summary>
	/// An abstract class that provides a base for all block-level content elements.
	/// </summary>
    [OpenSilver.NotImplemented]
	public abstract partial class Block : TextElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Block" /> class. 
		/// </summary>
        [OpenSilver.NotImplemented]
		protected Block()
		{
		}

		/// <summary>
		/// Identifies the <see cref="Block.LineHeight" /> dependency property.
		/// </summary>
#if WORKINPROGRESS
		public static readonly DependencyProperty LineHeightProperty =
			DependencyProperty.Register(
				"LineHeight",
				typeof(double),
				typeof(Block),
				new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
#else
		public static readonly DependencyProperty LineHeightProperty =
			DependencyProperty.Register(
				"LineHeight",
				typeof(double),
				typeof(Block),
				new PropertyMetadata(0d));
#endif

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
			get { return (double)this.GetValue(LineHeightProperty); }
			set { this.SetValue(LineHeightProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Block.LineStackingStrategy" /> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty LineStackingStrategyProperty =
			DependencyProperty.Register(
				"LineStackingStrategy",
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
			get { return (LineStackingStrategy)this.GetValue(LineStackingStrategyProperty); }
			set { this.SetValue(LineStackingStrategyProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Block.TextAlignment" /> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty TextAlignmentProperty =
			DependencyProperty.Register(
				"TextAlignment",
				typeof(TextAlignment),
				typeof(Block),
				new PropertyMetadata(TextAlignment.Left));

		/// <summary>
		/// Gets or sets the horizontal alignment of the text content. 
		/// The default is <see cref="TextAlignment.Left" />.
		/// </summary>
        [OpenSilver.NotImplemented]
		public TextAlignment TextAlignment
		{
			get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
			set { this.SetValue(TextAlignmentProperty, value); }
		}
	}
}
#endif