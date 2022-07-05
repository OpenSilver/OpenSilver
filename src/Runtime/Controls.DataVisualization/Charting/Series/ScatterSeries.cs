using System.Collections.Generic;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in X/Y scatter format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = "PlotArea", Type = typeof(Canvas))]
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(ScatterDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    public class ScatterSeries : DataPointSingleSeriesWithAxes
    {
        /// <summary>
        /// Identifies the DependentRangeAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty DependentRangeAxisProperty = DependencyProperty.Register(nameof(DependentRangeAxis), typeof(IRangeAxis), typeof(ScatterSeries), new PropertyMetadata((object)null, new PropertyChangedCallback(ScatterSeries.OnDependentRangeAxisPropertyChanged)));
        /// <summary>Identifies the IndependentAxis dependency property.</summary>
        public static readonly DependencyProperty IndependentAxisProperty = DependencyProperty.Register(nameof(IndependentAxis), typeof(IAxis), typeof(ScatterSeries), new PropertyMetadata((object)null, new PropertyChangedCallback(ScatterSeries.OnIndependentAxisPropertyChanged)));

        /// <summary>Gets the dependent axis as a range axis.</summary>
        public IRangeAxis ActualDependentRangeAxis
        {
            get
            {
                return this.InternalActualDependentAxis as IRangeAxis;
            }
        }

        /// <summary>Gets or sets the dependent range axis.</summary>
        public IRangeAxis DependentRangeAxis
        {
            get
            {
                return this.GetValue(ScatterSeries.DependentRangeAxisProperty) as IRangeAxis;
            }
            set
            {
                this.SetValue(ScatterSeries.DependentRangeAxisProperty, (object)value);
            }
        }

        /// <summary>DependentRangeAxisProperty property changed handler.</summary>
        /// <param name="d">ScatterSeries that changed its DependentRangeAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentRangeAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScatterSeries)d).OnDependentRangeAxisPropertyChanged((IRangeAxis)e.NewValue);
        }

        /// <summary>DependentRangeAxisProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnDependentRangeAxisPropertyChanged(IRangeAxis newValue)
        {
            this.InternalDependentAxis = (IAxis)newValue;
        }

        /// <summary>Gets the independent axis as a range axis.</summary>
        public IAxis ActualIndependentAxis
        {
            get
            {
                return this.InternalActualIndependentAxis;
            }
        }

        /// <summary>Gets or sets the independent range axis.</summary>
        public IAxis IndependentAxis
        {
            get
            {
                return this.GetValue(ScatterSeries.IndependentAxisProperty) as IAxis;
            }
            set
            {
                this.SetValue(ScatterSeries.IndependentAxisProperty, (object)value);
            }
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="d">ScatterSeries that changed its IndependentAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScatterSeries)d).OnIndependentAxisPropertyChanged((IAxis)e.NewValue);
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnIndependentAxisPropertyChanged(IAxis newValue)
        {
            this.InternalIndependentAxis = newValue;
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
                    throw new InvalidOperationException("ScatterSeries.GetAxes: No Suitable Axis Available For Plotting Dependent Value");
                rangeAxisFromData.ShowGridLines = true;
                rangeAxisFromData.Orientation = AxisOrientation.Y;
                return (IAxis)rangeAxisFromData;
            }));
        }

        /// <summary>Creates a new scatter data point.</summary>
        /// <returns>A scatter data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return (DataPoint)new ScatterDataPoint();
        }

        /// <summary>
        /// Returns the custom ResourceDictionary to use for necessary resources.
        /// </summary>
        /// <returns>ResourceDictionary to use for necessary resources.</returns>
        protected override IEnumerator<ResourceDictionary> GetResourceDictionaryEnumeratorFromHost()
        {
            return DataPointSeries.GetResourceDictionaryWithTargetType((IResourceDictionaryDispenser)this.SeriesHost, typeof(ScatterDataPoint), true);
        }

        /// <summary>This method updates a single data point.</summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            double num1 = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)this.ActualDependentRangeAxis.Range.Maximum).Value;
            double num2 = this.ActualIndependentAxis.GetPlotAreaCoordinate(dataPoint.ActualIndependentValue).Value;
            double num3 = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)dataPoint.ActualDependentValue).Value;
            if (ValueHelper.CanGraph(num2) && ValueHelper.CanGraph(num3))
            {
                dataPoint.Visibility = Visibility.Visible;
                Canvas.SetLeft((UIElement)dataPoint, Math.Round(num2 - dataPoint.ActualWidth / 2.0));
                Canvas.SetTop((UIElement)dataPoint, Math.Round(num1 - (num3 + dataPoint.ActualHeight / 2.0)));
            }
            else
                dataPoint.Visibility = Visibility.Collapsed;
        }
    }
}

