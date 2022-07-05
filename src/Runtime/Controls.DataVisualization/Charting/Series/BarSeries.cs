
using System.Collections.Generic;
using System.Linq;


namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in bar format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = "PlotArea", Type = typeof(Canvas))]
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(BarDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    public class BarSeries : ColumnBarBaseSeries<BarDataPoint>
    {
        /// <summary>
        /// Acquire a horizontal category axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            this.GetAxes(firstDataPoint, (Func<IAxis, bool>)(axis => axis.Orientation == AxisOrientation.Y), (Func<IAxis>)(() =>
            {
                return (IAxis)new CategoryAxis()
                {
                    Orientation = AxisOrientation.Y
                };
            }), (Func<IAxis, bool>)(axis =>
            {
                IRangeAxis rangeAxis = axis as IRangeAxis;
                return rangeAxis != null && rangeAxis.Origin != null && axis.Orientation == AxisOrientation.X;
            }), (Func<IAxis>)(() =>
            {
                IRangeAxis rangeAxisFromData = DataPointSeriesWithAxes.CreateRangeAxisFromData((object)firstDataPoint.DependentValue);
                rangeAxisFromData.Orientation = AxisOrientation.X;
                if (rangeAxisFromData == null || rangeAxisFromData.Origin == null)
                    throw new InvalidOperationException("BarSeries.GetAxes: No Suitable Axis Available For Plotting Dependent Value");
                DisplayAxis displayAxis = rangeAxisFromData as DisplayAxis;
                if (displayAxis != null)
                    displayAxis.ShowGridLines = true;
                return (IAxis)rangeAxisFromData;
            }));
        }

        /// <summary>Updates each point.</summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            if (this.SeriesHost == null || this.PlotArea == null)
                return;
            object category = dataPoint.ActualIndependentValue ?? (object)(this.ActiveDataPoints.IndexOf((object)dataPoint) + 1);
            Range<UnitValue> categoryRange = this.GetCategoryRange(category);
            if (!categoryRange.HasData)
                return;
            UnitValue unitValue = categoryRange.Maximum;
            int num1;
            if (unitValue.Unit == Unit.Pixels)
            {
                unitValue = categoryRange.Minimum;
                num1 = unitValue.Unit == Unit.Pixels ? 1 : 0;
            }
            else
                num1 = 0;
            if (num1 == 0)
                throw new InvalidOperationException("BarSeries.UpdateDataPoint: This Series Does Not Support Radial Axes");
            unitValue = categoryRange.Minimum;
            double num2 = unitValue.Value;
            unitValue = categoryRange.Maximum;
            double num3 = unitValue.Value;
            IEnumerable<BarSeries> barSerieses = this.SeriesHost.Series.OfType<BarSeries>().Where<BarSeries>((Func<BarSeries, bool>)(series => series.ActualIndependentAxis == this.ActualIndependentAxis));
            int num4 = CollectionHelper.Count(barSerieses);
            double num5 = num3 - num2;
            double a1 = num5 * 0.8 / (double)num4;
            int num6 = barSerieses.IndexOf((object)this);
            unitValue = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)ValueHelper.ToDouble((object)dataPoint.ActualDependentValue));
            double val1 = unitValue.Value;
            unitValue = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)this.ActualDependentRangeAxis.Origin);
            double val2 = unitValue.Value;
            double num7 = (double)num6 * Math.Round(a1) + num5 * 0.1;
            double a2 = num2 + num7;
            if (this.GetIsDataPointGrouped(category))
            {
                IGrouping<object, DataPoint> dataPointGroup = this.GetDataPointGroup(category);
                int num8 = dataPointGroup.IndexOf((object)dataPoint);
                a2 += (double)num8 * (a1 * 0.2) / (double)(CollectionHelper.Count(dataPointGroup) - 1);
                a1 *= 0.8;
                Canvas.SetZIndex((UIElement)dataPoint, -num8);
            }
            if (ValueHelper.CanGraph(val1) && ValueHelper.CanGraph(a2) && ValueHelper.CanGraph(val2))
            {
                dataPoint.Visibility = Visibility.Visible;
                double length1 = Math.Round(a2);
                double num8 = Math.Round(a1);
                double length2 = Math.Round(Math.Min(val1, val2) - 0.5);
                double num9 = Math.Round(Math.Max(val1, val2) - 0.5) - length2 + 1.0;
                Canvas.SetLeft((UIElement)dataPoint, length2);
                Canvas.SetTop((UIElement)dataPoint, length1);
                dataPoint.Width = num9;
                dataPoint.Height = num8;
            }
            else
                dataPoint.Visibility = Visibility.Collapsed;
        }
    }
}
