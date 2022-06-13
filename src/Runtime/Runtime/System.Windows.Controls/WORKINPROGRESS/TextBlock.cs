using System.Windows;

#if MIGRATION
using System.Windows.Documents;
#else
using Windows.UI.Xaml.Documents;
#endif

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
		//     Gets or sets a value that indicates how a line box is determined for each line
		//     of text in the System.Windows.Controls.TextBlock.
		//
		// Returns:
		//     A value that indicates how a line box is determined for each line of text in
		//     the System.Windows.Controls.TextBlock. The default is System.Windows.LineStackingStrategy.MaxHeight.
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty LineStackingStrategyProperty =
			DependencyProperty.Register(
				"LineStackingStrategy",
				typeof(LineStackingStrategy),
				typeof(TextBlock),
				new PropertyMetadata(LineStackingStrategy.BlockLineHeight));

		[OpenSilver.NotImplemented]
		public LineStackingStrategy LineStackingStrategy
		{
			get { return (LineStackingStrategy)GetValue(LineStackingStrategyProperty); }
			set { SetValue(LineStackingStrategyProperty, value); }
		}

		[OpenSilver.NotImplemented]
		public double BaselineOffset { get; private set; }

        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty CharacterSpacingProperty =
			DependencyProperty.Register(
				"CharacterSpacing",
				typeof(int),
				typeof(TextBlock),
				new PropertyMetadata(0));

        [OpenSilver.NotImplemented]
		public int CharacterSpacing
		{
			get { return (int)this.GetValue(CharacterSpacingProperty); }
			set { this.SetValue(CharacterSpacingProperty, value); }
		}

		[OpenSilver.NotImplemented]
		public FontSource FontSource { get; set; }
	}
}
