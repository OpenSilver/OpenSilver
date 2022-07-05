using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in X/Y
    /// line format.  A third binding determines the size of the data point.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(BubbleDataPoint))]
    [TemplatePart(Name = "PlotArea", Type = typeof(Canvas))]
    public class BubbleSeries : DataPointSingleSeriesWithAxes
    {
        /// <summary>
        /// Identifies the DependentRangeAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty DependentRangeAxisProperty = DependencyProperty.Register(nameof(DependentRangeAxis), typeof(IRangeAxis), typeof(BubbleSeries), new PropertyMetadata((object)null, new PropertyChangedCallback(BubbleSeries.OnDependentRangeAxisPropertyChanged)));
        /// <summary>Identifies the IndependentAxis dependency property.</summary>
        public static readonly DependencyProperty IndependentAxisProperty = DependencyProperty.Register(nameof(IndependentAxis), typeof(IAxis), typeof(BubbleSeries), new PropertyMetadata((object)null, new PropertyChangedCallback(BubbleSeries.OnIndependentAxisPropertyChanged)));
        /// <summary>
        /// Stores the range of ActualSize values for the BubbleDataPoints.
        /// </summary>
        private Range<double> _rangeOfActualSizeValues = new Range<double>();
        /// <summary>
        /// The maximum bubble size as a ratio of the smallest dimension.
        /// </summary>
        private const double MaximumBubbleSizeAsRatioOfSmallestDimension = 0.25;
        /// <summary>The binding used to identify the size value.</summary>
        private Binding _sizeValueBinding;

        /// <summary>
        /// Gets or sets the Binding to use for identifying the size of the bubble.
        /// </summary>
        public Binding SizeValueBinding
        {
            get
            {
                return this._sizeValueBinding;
            }
            set
            {
                if (this._sizeValueBinding == value)
                    return;
                this._sizeValueBinding = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the Binding Path to use for identifying the size of the bubble.
        /// </summary>
        public string SizeValuePath
        {
            get
            {
                return this.SizeValueBinding != null ? this.SizeValueBinding.Path.Path : (string)null;
            }
            set
            {
                if (null == value)
                    this.SizeValueBinding = (Binding)null;
                else
                    this.SizeValueBinding = new Binding(value);
            }
        }

        /// <summary>Creates a new instance of bubble data point.</summary>
        /// <returns>A new instance of bubble data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return (DataPoint)new BubbleDataPoint();
        }

        /// <summary>
        /// Returns the custom ResourceDictionary to use for necessary resources.
        /// </summary>
        /// <returns>ResourceDictionary to use for necessary resources.</returns>
        protected override IEnumerator<ResourceDictionary> GetResourceDictionaryEnumeratorFromHost()
        {
            return DataPointSeries.GetResourceDictionaryWithTargetType((IResourceDictionaryDispenser)this.SeriesHost, typeof(BubbleDataPoint), true);
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
                    throw new InvalidOperationException("BubbleSeries.GetAxes: NoSuitableAxisAvailableForPlottingDependentValue");
                rangeAxisFromData.ShowGridLines = true;
                rangeAxisFromData.Orientation = AxisOrientation.Y;
                return (IAxis)rangeAxisFromData;
            }));
        }

        /// <summary>
        /// Prepares a bubble data point by binding the size value binding to
        /// the size property.
        /// </summary>
        /// <param name="dataPoint">The data point to prepare.</param>
        /// <param name="dataContext">The data context of the data point.</param>
        protected override void PrepareDataPoint(DataPoint dataPoint, object dataContext)
        {
            base.PrepareDataPoint(dataPoint, dataContext);
            dataPoint.SetBinding(BubbleDataPoint.SizeProperty, this.SizeValueBinding ?? this.DependentValueBinding ?? this.IndependentValueBinding);
        }

        /// <summary>
        /// Attaches size change and actual size change event handlers to the
        /// data point.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        protected override void AttachEventHandlersToDataPoint(DataPoint dataPoint)
        {
            BubbleDataPoint bubbleDataPoint = (BubbleDataPoint)dataPoint;
            bubbleDataPoint.SizePropertyChanged += new RoutedPropertyChangedEventHandler<double>(this.BubbleDataPointSizePropertyChanged);
            bubbleDataPoint.ActualSizePropertyChanged += new RoutedPropertyChangedEventHandler<double>(this.BubbleDataPointActualSizePropertyChanged);
            base.AttachEventHandlersToDataPoint(dataPoint);
        }

        /// <summary>
        /// Detaches size change and actual size change event handlers from the
        /// data point.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        protected override void DetachEventHandlersFromDataPoint(DataPoint dataPoint)
        {
            BubbleDataPoint bubbleDataPoint = (BubbleDataPoint)dataPoint;
            bubbleDataPoint.SizePropertyChanged -= new RoutedPropertyChangedEventHandler<double>(this.BubbleDataPointSizePropertyChanged);
            bubbleDataPoint.ActualSizePropertyChanged -= new RoutedPropertyChangedEventHandler<double>(this.BubbleDataPointActualSizePropertyChanged);
            base.DetachEventHandlersFromDataPoint(dataPoint);
        }

        /// <summary>
        /// Updates all data points when the actual size property of a data
        /// point changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void BubbleDataPointActualSizePropertyChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.ActiveDataPoints.OfType<BubbleDataPoint>().Select<BubbleDataPoint, double>((Func<BubbleDataPoint, double>)(d => Math.Abs(d.ActualSize))).GetRange<double>() == this._rangeOfActualSizeValues)
                this.UpdateDataPoint((DataPoint)sender);
            else
                this.UpdateDataPoints(this.ActiveDataPoints);
        }

        /// <summary>
        /// Animates the value of the ActualSize property to the size property
        /// when it changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void BubbleDataPointSizePropertyChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((FrameworkElement)sender).BeginAnimation(BubbleDataPoint.ActualSizeProperty, "ActualSize", (object)e.NewValue, this.TransitionDuration, this.TransitionEasingFunction);
        }

        /// <summary>
        /// Calculates the range of ActualSize values of all active BubbleDataPoints.
        /// </summary>
        protected override void OnBeforeUpdateDataPoints()
        {
            this._rangeOfActualSizeValues = this.ActiveDataPoints.OfType<BubbleDataPoint>().Select<BubbleDataPoint, double>((Func<BubbleDataPoint, double>)(d => Math.Abs(d.ActualSize))).GetRange<double>();
        }

        /// <summary>
        /// Ensure that if any data points are updated, all data points are
        /// updated.
        /// </summary>
        /// <param name="dataPoints">The data points to update.</param>
        protected override void UpdateDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            base.UpdateDataPoints(this.ActiveDataPoints);
        }

        /// <summary>Updates the data point's visual representation.</summary>
        /// <param name="dataPoint">The data point.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            Size plotAreaSize = this.PlotAreaSize;
            double width = plotAreaSize.Width;
            plotAreaSize = this.PlotAreaSize;
            double height = plotAreaSize.Height;
            double num1 = Math.Min(width, height) * 0.25;
            BubbleDataPoint bubbleDataPoint = (BubbleDataPoint)dataPoint;
            double num2 = !this._rangeOfActualSizeValues.HasData || this._rangeOfActualSizeValues.Maximum == 0.0 || bubbleDataPoint.ActualSize < 0.0 ? 0.0 : Math.Abs(bubbleDataPoint.ActualSize) / this._rangeOfActualSizeValues.Maximum;
            bubbleDataPoint.Width = num2 * num1;
            bubbleDataPoint.Height = num2 * num1;
            double length1 = this.ActualIndependentAxis.GetPlotAreaCoordinate(bubbleDataPoint.ActualIndependentValue).Value - bubbleDataPoint.Width / 2.0;
            double length2 = this.PlotAreaSize.Height - bubbleDataPoint.Height / 2.0 - this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)bubbleDataPoint.ActualDependentValue).Value;
            if (ValueHelper.CanGraph(length1) && ValueHelper.CanGraph(length2))
            {
                dataPoint.Visibility = Visibility.Visible;
                Canvas.SetLeft((UIElement)bubbleDataPoint, length1);
                Canvas.SetTop((UIElement)bubbleDataPoint, length2);
            }
            else
                dataPoint.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Updates the value margins after all data points are updated.
        /// </summary>
        protected override void OnAfterUpdateDataPoints()
        {
            IValueMarginProvider provider = (IValueMarginProvider)this;
            IValueMarginConsumer dependentRangeAxis = this.ActualDependentRangeAxis as IValueMarginConsumer;
            if (dependentRangeAxis != null)
                dependentRangeAxis.ValueMarginsChanged(provider, this.GetValueMargins(dependentRangeAxis));
            IValueMarginConsumer actualIndependentAxis = this.ActualIndependentAxis as IValueMarginConsumer;
            if (actualIndependentAxis != null)
                actualIndependentAxis.ValueMarginsChanged(provider, this.GetValueMargins(actualIndependentAxis));
            base.OnAfterUpdateDataPoints();
        }

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
                return this.GetValue(BubbleSeries.DependentRangeAxisProperty) as IRangeAxis;
            }
            set
            {
                this.SetValue(BubbleSeries.DependentRangeAxisProperty, (object)value);
            }
        }

        /// <summary>DependentRangeAxisProperty property changed handler.</summary>
        /// <param name="d">BubbleSeries that changed its DependentRangeAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentRangeAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleSeries)d).OnDependentRangeAxisPropertyChanged((IRangeAxis)e.NewValue);
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

        /// <summary>Gets or sets independent range axis.</summary>
        public IAxis IndependentAxis
        {
            get
            {
                return this.GetValue(BubbleSeries.IndependentAxisProperty) as IAxis;
            }
            set
            {
                this.SetValue(BubbleSeries.IndependentAxisProperty, (object)value);
            }
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="d">BubbleSeries that changed its IndependentAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleSeries)d).OnIndependentAxisPropertyChanged((IAxis)e.NewValue);
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnIndependentAxisPropertyChanged(IAxis newValue)
        {
            this.InternalIndependentAxis = newValue;
        }

        /// <summary>The margins required for each value.</summary>
        /// <param name="consumer">The consumer to return the value margins for.</param>
        /// <returns>A sequence of margins for each value.</returns>
        protected override IEnumerable<ValueMargin> GetValueMargins(IValueMarginConsumer consumer)
        {
            IAxis axis = consumer as IAxis;
            if (axis != null)
                return this.ActiveDataPoints.Select<DataPoint, ValueMargin>((Func<DataPoint, ValueMargin>)(dataPoint =>
                {
                    double margin = dataPoint.GetMargin(axis);
                    return new ValueMargin(this.GetActualDataPointAxisValue(dataPoint, axis), margin, margin);
                }));
            return Enumerable.Empty<ValueMargin>();
        }
    }
}