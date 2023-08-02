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
		/// <summary>
		/// Identifies the <see cref="LineStackingStrategy"/> dependency property.
		/// </summary>
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty LineStackingStrategyProperty =
			DependencyProperty.Register(
				nameof(LineStackingStrategy),
				typeof(LineStackingStrategy),
				typeof(TextBlock),
				new PropertyMetadata(LineStackingStrategy.MaxHeight));

		/// <summary>
		/// Gets or sets a value that indicates how a line box is determined for each line
		/// of text in the <see cref="TextBlock"/>.
		/// </summary>
		/// <returns>
		/// A value that indicates how a line box is determined for each line of text in
		/// the <see cref="TextBlock"/>. The default is <see cref="LineStackingStrategy.MaxHeight"/>.
		/// </returns>
		[OpenSilver.NotImplemented]
		public LineStackingStrategy LineStackingStrategy
		{
			get { return (LineStackingStrategy)GetValue(LineStackingStrategyProperty); }
			set { SetValue(LineStackingStrategyProperty, value); }
		}

		[OpenSilver.NotImplemented]
		public double BaselineOffset { get; private set; }

		[OpenSilver.NotImplemented]
		public FontSource FontSource { get; set; }
	}
}
