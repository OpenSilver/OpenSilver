#if WORKINPROGRESS

using CSHTML5.Internal;
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
        [OpenSilver.NotImplemented]
		public double LineHeight { get; set; }
		
		//
		// Summary:
		//     Gets or sets a value that indicates how a line box is determined for each line
		//     of text in the System.Windows.Controls.TextBlock.
		//
		// Returns:
		//     A value that indicates how a line box is determined for each line of text in
		//     the System.Windows.Controls.TextBlock. The default is System.Windows.LineStackingStrategy.MaxHeight.
        [OpenSilver.NotImplemented]
		public LineStackingStrategy LineStackingStrategy { get; set; }

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

		private Size noWrapSize = Size.Empty;

		public override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
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
			if (noWrapSize == Size.Empty)
			{
				noWrapSize = Application.Current.TextMeasurementService.Measure(Text ?? String.Empty, FontSize, FontFamily, FontStyle, FontWeight, FontStretch, TextWrapping.NoWrap, Padding, Double.PositiveInfinity);
			}

			if (TextWrapping == TextWrapping.NoWrap || noWrapSize.Width <= availableSize.Width)
			{
				return noWrapSize;
			}

			return Application.Current.TextMeasurementService.MeasureTextBlock(Text ?? String.Empty, FontSize, FontFamily, FontStyle, FontWeight, FontStretch, TextWrapping, Padding, availableSize.Width);
		}
	}
}

#endif