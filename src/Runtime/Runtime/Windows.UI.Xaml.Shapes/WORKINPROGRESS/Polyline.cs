#if WORKINPROGRESS

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Shapes
#else
namespace Windows.UI.Xaml.Shapes
#endif
{
	//
	// Summary:
	//     Draws a series of connected straight lines.
	[OpenSilver.NotImplemented]
	public sealed partial class Polyline : Shape
	{
		//
		// Summary:
		//     Identifies the System.Windows.Shapes.Polyline.FillRule dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Shapes.Polyline.FillRule dependency property.
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register("FillRule", typeof(FillRule), typeof(Polyline), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		//
		// Summary:
		//     Identifies the System.Windows.Shapes.Polyline.Points dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Shapes.Polyline.Points dependency property.
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(Polyline), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Shapes.Polyline class.
		[OpenSilver.NotImplemented]
		public Polyline()
		{
		}

		//
		// Summary:
		//     Gets or sets a value that specifies how the interior fill of the shape is determined.
		//
		// Returns:
		//     A value of the enumeration that specifies the fill behavior. The default is System.Windows.Media.FillRule.EvenOdd.
		[OpenSilver.NotImplemented]
		public FillRule FillRule
		{
			get
			{
				return (FillRule)this.GetValue(FillRuleProperty);
			}

			set
			{
				this.SetValue(FillRuleProperty, value);
			}
		}

		//
		// Summary:
		//     Gets or sets a collection that contains the vertex points of the System.Windows.Shapes.Polyline.
		//
		// Returns:
		//     A collection of System.Windows.Point structures that describe the vertex points
		//     of the System.Windows.Shapes.Polyline. The default is null.
		[OpenSilver.NotImplemented]
		public PointCollection Points
		{
			get
			{
				return (PointCollection)this.GetValue(PointsProperty);
			}

			set
			{
				this.SetValue(PointsProperty, value);
			}
		}
	}
}
#endif