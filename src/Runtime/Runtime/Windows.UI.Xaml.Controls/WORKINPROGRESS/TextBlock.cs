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
			Size BorderThicknessSize = new Size(BorderThickness.Left + BorderThickness.Right, BorderThickness.Top + BorderThickness.Bottom);

			if (noWrapSize == Size.Empty)
			{
				noWrapSize = Application.Current.TextMeasurementService.MeasureTextBlock(Text ?? String.Empty, FontSize, FontFamily, FontStyle, FontWeight, FontStretch, TextWrapping.NoWrap, Padding, Double.PositiveInfinity);
				noWrapSize.Width = noWrapSize.Width + BorderThicknessSize.Width;
				noWrapSize.Height = noWrapSize.Height + BorderThicknessSize.Height;
			}

			if (TextWrapping == TextWrapping.NoWrap || noWrapSize.Width <= availableSize.Width)
			{
				return noWrapSize;
			}

			Size TextSize = Application.Current.TextMeasurementService.MeasureTextBlock(Text ?? String.Empty, FontSize, FontFamily, FontStyle, FontWeight, FontStretch, TextWrapping, Padding, (availableSize.Width - BorderThicknessSize.Width).Max(0));
			TextSize.Width = TextSize.Width + BorderThicknessSize.Width;
			TextSize.Height = TextSize.Height + BorderThicknessSize.Height;
			return TextSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			return finalSize;
		}
	}
}

#endif