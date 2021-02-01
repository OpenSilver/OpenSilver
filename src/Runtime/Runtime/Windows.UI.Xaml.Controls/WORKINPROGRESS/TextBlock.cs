#if WORKINPROGRESS

using System.Windows;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public partial class TextBlock : Control
	{
		//
		// Summary:
		//     Gets or sets the height of each line of content.
		//
		// Returns:
		//     The height of each line in pixels. A value of 0 indicates that the line height
		//     is determined automatically from the current font characteristics. The default
		//     is 0.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     System.Windows.Controls.TextBlock.LineHeight is set to a non-positive value.
		public double LineHeight { get; set; }
		
		//
		// Summary:
		//     Gets or sets a value that indicates how a line box is determined for each line
		//     of text in the System.Windows.Controls.TextBlock.
		//
		// Returns:
		//     A value that indicates how a line box is determined for each line of text in
		//     the System.Windows.Controls.TextBlock. The default is System.Windows.LineStackingStrategy.MaxHeight.
		public LineStackingStrategy LineStackingStrategy { get; set; }

		public double BaselineOffset { get; private set; }

		public static readonly DependencyProperty CharacterSpacingProperty =
			DependencyProperty.Register(
				"CharacterSpacing",
				typeof(int),
				typeof(TextBlock),
				new PropertyMetadata(0));

		public int CharacterSpacing
		{
			get { return (int)this.GetValue(CharacterSpacingProperty); }
			set { this.SetValue(CharacterSpacingProperty, value); }
		}
	}
}

#endif