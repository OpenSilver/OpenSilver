// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endif

#if !DEFINITION_SERIES_COMPATIBILITY_MODE

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in X/Y
    /// line format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(LineDataPoint))]
    [TemplatePart(Name = "PlotArea", Type = typeof(Canvas))]
    [StyleTypedProperty(Property = "PolylineStyle", StyleTargetType = typeof(Polyline))]
    public class LineSeries : LineAreaBaseSeries<LineDataPoint>
    {
        /// <summary>Identifies the Points dependency property.</summary>
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(LineSeries), (PropertyMetadata)null);
        /// <summary>Identifies the PolylineStyle dependency property.</summary>
        public static readonly DependencyProperty PolylineStyleProperty = DependencyProperty.Register(nameof(PolylineStyle), typeof(Style), typeof(LineSeries), (PropertyMetadata)null);

        /// <summary>Gets the collection of points that make up the line.</summary>
        public PointCollection Points
        {
            get
            {
                return this.GetValue(LineSeries.PointsProperty) as PointCollection;
            }
            private set
            {
                this.SetValue(LineSeries.PointsProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the Polyline object that follows the data
        /// points.
        /// </summary>
        public Style PolylineStyle
        {
            get
            {
                return this.GetValue(LineSeries.PolylineStyleProperty) as Style;
            }
            set
            {
                this.SetValue(LineSeries.PolylineStyleProperty, (object)value);
            }
        }

        /// <summary>Initializes a new instance of the LineSeries class.</summary>
        public LineSeries()
        {
            this.DefaultStyleKey = (object)typeof(LineSeries);
        }

        /// <summary>
        /// Acquire a horizontal linear axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            this.GetAxes(firstDataPoint, (Func<IAxis, bool>)(axis => axis.Orientation == AxisOrientation.X), (Func<IAxis>)(() =>
            {
                IAxis axis = (IAxis)DataPointSeriesWithAxes.CreateRangeAxisFromData(firstDataPoint.IndependentValue) ?? (IAxis)new CategoryAxis();
                axis.Orientation = AxisOrientation.X;
                return axis;
            }), (Func<IAxis, bool>)(axis => axis.Orientation == AxisOrientation.Y && axis is IRangeAxis), (Func<IAxis>)(() =>
            {
                DisplayAxis rangeAxisFromData = (DisplayAxis)DataPointSeriesWithAxes.CreateRangeAxisFromData((object)firstDataPoint.DependentValue);
                if (rangeAxisFromData == null)
                    throw new InvalidOperationException("LineSeries.GetAxes: No Suitable Axis Available For Plotting Dependent Value");
                rangeAxisFromData.ShowGridLines = true;
                rangeAxisFromData.Orientation = AxisOrientation.Y;
                return (IAxis)rangeAxisFromData;
            }));
        }

        /// <summary>
        /// Updates the Series shape object from a collection of Points.
        /// </summary>
        /// <param name="points">Collection of Points.</param>
        protected override void UpdateShapeFromPoints(IEnumerable<Point> points)
        {
            if (points.Any<Point>())
            {
                PointCollection pointCollection = new PointCollection();
                foreach (Point point in points)
                    pointCollection.Add(point);
                this.Points = pointCollection;
            }
            else
                this.Points = (PointCollection)null;
        }
    }
}

#endif
