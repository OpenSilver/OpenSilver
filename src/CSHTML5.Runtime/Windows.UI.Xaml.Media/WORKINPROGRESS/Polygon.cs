#if WORKINPROGRESS

#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	//
	// Summary:
	//     Draws a polygon, which is a connected series of lines that form a closed shape.
	public sealed class Polygon : Shape
	{
		//
		// Summary:
		//     Identifies the System.Windows.Shapes.Polygon.FillRule dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Shapes.Polygon.FillRule dependency property.
		public static readonly DependencyProperty FillRuleProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Shapes.Polygon.Points dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Shapes.Polygon.Points dependency property.
		public static readonly DependencyProperty PointsProperty;

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Shapes.Polygon class.
		public Polygon()
		{
			
		}

		//
		// Summary:
		//     Gets or sets a value that specifies how the interior fill of the shape is determined.
		//
		// Returns:
		//     A value of the enumeration. The default is System.Windows.Media.FillRule.EvenOdd.
		public FillRule FillRule { get; set; }
		//
		// Summary:
		//     Gets or sets a collection that contains the vertex points of the polygon.
		//
		// Returns:
		//     A collection of System.Windows.Point structures that describes the vertex points
		//     of the polygon. The default is null. The value can be expressed as a string as
		//     described in "pointSet Grammar" below.
		public PointCollection Points { get; set; }
	}
}
#endif