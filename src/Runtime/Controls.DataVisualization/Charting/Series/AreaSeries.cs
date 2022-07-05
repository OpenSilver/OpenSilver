using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in X/Y
    /// line format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = "PlotArea", Type = typeof(Canvas))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(AreaDataPoint))]
    [StyleTypedProperty(Property = "PathStyle", StyleTargetType = typeof(Path))]
    public class AreaSeries : LineAreaBaseSeries<AreaDataPoint>, IAnchoredToOrigin
    {
        /// <summary>Identifies the Geometry dependency property.</summary>
        public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(AreaSeries), (PropertyMetadata)null);
        /// <summary>Identifies the PathStyle dependency property.</summary>
        public static readonly DependencyProperty PathStyleProperty = DependencyProperty.Register(nameof(PathStyle), typeof(Style), typeof(AreaSeries), (PropertyMetadata)null);

        /// <summary>Gets the geometry property.</summary>
        public Geometry Geometry
        {
            get
            {
                return this.GetValue(AreaSeries.GeometryProperty) as Geometry;
            }
            private set
            {
                this.SetValue(AreaSeries.GeometryProperty, (object)value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the Path object that follows the data
        /// points.
        /// </summary>
        public Style PathStyle
        {
            get
            {
                return this.GetValue(AreaSeries.PathStyleProperty) as Style;
            }
            set
            {
                this.SetValue(AreaSeries.PathStyleProperty, (object)value);
            }
        }

        /// <summary>Initializes a new instance of the AreaSeries class.</summary>
        public AreaSeries()
        {
            this.DefaultStyleKey = (object)typeof(AreaSeries);
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
            }), (Func<IAxis, bool>)(axis =>
            {
                IRangeAxis rangeAxis = axis as IRangeAxis;
                return rangeAxis != null && rangeAxis.Origin != null && axis.Orientation == AxisOrientation.Y;
            }), (Func<IAxis>)(() =>
            {
                DisplayAxis rangeAxisFromData = (DisplayAxis)DataPointSeriesWithAxes.CreateRangeAxisFromData((object)firstDataPoint.DependentValue);
                if (rangeAxisFromData == null || (rangeAxisFromData as IRangeAxis).Origin == null)
                    throw new InvalidOperationException("AreaSeries.GetAxes: No Suitable Axis Available For Plotting Dependent Value");
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
            UnitValue plotAreaCoordinate1 = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)this.ActualDependentRangeAxis.Origin);
            UnitValue plotAreaCoordinate2 = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)this.ActualDependentRangeAxis.Range.Maximum);
            if (points.Any<Point>() && ValueHelper.CanGraph(plotAreaCoordinate1.Value) && ValueHelper.CanGraph(plotAreaCoordinate2.Value))
            {
                double num1 = Math.Floor(plotAreaCoordinate1.Value);
                PathFigure pathFigure = new PathFigure();
                pathFigure.IsClosed = true;
                pathFigure.IsFilled = true;
                double num2 = plotAreaCoordinate2.Value;
                IEnumerator<Point> enumerator = points.GetEnumerator();
                enumerator.MoveNext();
                Point point = new Point(enumerator.Current.X, num2 - num1);
                pathFigure.StartPoint = point;
                Point current;
                do
                {
                    current = enumerator.Current;
                    pathFigure.Segments.Add((PathSegment)new LineSegment()
                    {
                        Point = enumerator.Current
                    });
                }
                while (enumerator.MoveNext());
                pathFigure.Segments.Add((PathSegment)new LineSegment()
                {
                    Point = new Point(current.X, num2 - num1)
                });
                if (pathFigure.Segments.Count <= 1)
                    return;
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(pathFigure);
                this.Geometry = (Geometry)pathGeometry;
            }
            else
                this.Geometry = (Geometry)null;
        }

        /// <summary>
        /// Remove value margins from the side of the data points to ensure
        /// that area chart is flush against the edge of the chart.
        /// </summary>
        /// <param name="consumer">The value margin consumer.</param>
        /// <returns>A sequence of value margins.</returns>
        protected override IEnumerable<ValueMargin> GetValueMargins(IValueMarginConsumer consumer)
        {
            if (consumer == this.ActualIndependentAxis)
                return Enumerable.Empty<ValueMargin>();
            return base.GetValueMargins(consumer);
        }

        IRangeAxis IAnchoredToOrigin.AnchoredAxis
        {
            get
            {
                return this.AnchoredAxis;
            }
        }

        /// <summary>Gets the axis to which the series is anchored.</summary>
        protected IRangeAxis AnchoredAxis
        {
            get
            {
                return this.ActualDependentRangeAxis;
            }
        }
    }
}

