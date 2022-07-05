using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// This series serves as the base class for the column and bar series.
    /// </summary>
    /// <typeparam name="T">The type of the data point.</typeparam>
    public abstract class ColumnBarBaseSeries<T> : DataPointSingleSeriesWithAxes, IAnchoredToOrigin where T : DataPoint, new()
    {
        /// <summary>
        /// Identifies the DependentRangeAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty DependentRangeAxisProperty = DependencyProperty.Register(nameof(DependentRangeAxis), typeof(IRangeAxis), typeof(ColumnBarBaseSeries<T>), new PropertyMetadata((object)null, new PropertyChangedCallback(ColumnBarBaseSeries<T>.OnDependentRangeAxisPropertyChanged)));
        /// <summary>Identifies the IndependentAxis dependency property.</summary>
        public static readonly DependencyProperty IndependentAxisProperty = DependencyProperty.Register(nameof(IndependentAxis), typeof(IAxis), typeof(ColumnBarBaseSeries<T>), new PropertyMetadata((object)null, new PropertyChangedCallback(ColumnBarBaseSeries<T>.OnIndependentAxisPropertyChanged)));
        /// <summary>
        /// Keeps a list of DataPoints that share the same category.
        /// </summary>
        private IDictionary<object, IGrouping<object, DataPoint>> _categoriesWithMultipleDataPoints;
        /// <summary>The length of each data point.</summary>
        private double? _dataPointlength;

        /// <summary>Gets or sets the dependent range axis.</summary>
        public IRangeAxis DependentRangeAxis
        {
            get
            {
                return this.GetValue(ColumnBarBaseSeries<T>.DependentRangeAxisProperty) as IRangeAxis;
            }
            set
            {
                this.SetValue(ColumnBarBaseSeries<T>.DependentRangeAxisProperty, (object)value);
            }
        }

        /// <summary>DependentRangeAxisProperty property changed handler.</summary>
        /// <param name="d">ColumnBarBaseSeries that changed its DependentRangeAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentRangeAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnBarBaseSeries<T>)d).OnDependentRangeAxisPropertyChanged((IRangeAxis)e.NewValue);
        }

        /// <summary>DependentRangeAxisProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnDependentRangeAxisPropertyChanged(IRangeAxis newValue)
        {
            this.InternalDependentAxis = (IAxis)newValue;
        }

        /// <summary>Gets or sets the independent category axis.</summary>
        public IAxis IndependentAxis
        {
            get
            {
                return this.GetValue(ColumnBarBaseSeries<T>.IndependentAxisProperty) as IAxis;
            }
            set
            {
                this.SetValue(ColumnBarBaseSeries<T>.IndependentAxisProperty, (object)value);
            }
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="d">ColumnBarBaseSeries that changed its IndependentAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnBarBaseSeries<T>)d).OnIndependentAxisPropertyChanged((IAxis)e.NewValue);
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnIndependentAxisPropertyChanged(IAxis newValue)
        {
            this.InternalIndependentAxis = newValue;
        }

        /// <summary>Returns the group of data points in a given category.</summary>
        /// <param name="category">The category for which to return the data
        /// point group.</param>
        /// <returns>The group of data points in a given category.</returns>
        protected IGrouping<object, DataPoint> GetDataPointGroup(object category)
        {
            return this._categoriesWithMultipleDataPoints[category];
        }

        /// <summary>
        /// Returns a value indicating whether a data point corresponding to
        /// a category is grouped.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>A value indicating whether a data point corresponding to
        /// a category is grouped.</returns>
        protected bool GetIsDataPointGrouped(object category)
        {
            return this._categoriesWithMultipleDataPoints.ContainsKey(category);
        }

        /// <summary>Gets the dependent axis as a range axis.</summary>
        public IRangeAxis ActualDependentRangeAxis
        {
            get
            {
                return this.InternalActualDependentAxis as IRangeAxis;
            }
        }

        /// <summary>Gets the independent axis as a category axis.</summary>
        public IAxis ActualIndependentAxis
        {
            get
            {
                return this.InternalActualIndependentAxis;
            }
        }

        /// <summary>Method run before DataPoints are updated.</summary>
        protected override void OnBeforeUpdateDataPoints()
        {
            base.OnBeforeUpdateDataPoints();
            this.CalculateDataPointLength();
            this._categoriesWithMultipleDataPoints = (IDictionary<object, IGrouping<object, DataPoint>>)this.ActiveDataPoints.Where<DataPoint>((Func<DataPoint, bool>)(point => null != point.IndependentValue)).OrderBy<DataPoint, IComparable>((Func<DataPoint, IComparable>)(point => point.DependentValue)).GroupBy<DataPoint, object>((Func<DataPoint, object>)(point => point.IndependentValue)).Where<IGrouping<object, DataPoint>>((Func<IGrouping<object, DataPoint>, bool>)(grouping => 1 < CollectionHelper.Count(grouping))).ToDictionary<IGrouping<object, DataPoint>, object>((Func<IGrouping<object, DataPoint>, object>)(grouping => grouping.Key));
        }

        /// <summary>
        /// Returns the custom ResourceDictionary to use for necessary resources.
        /// </summary>
        /// <returns>ResourceDictionary to use for necessary resources.</returns>
        protected override IEnumerator<ResourceDictionary> GetResourceDictionaryEnumeratorFromHost()
        {
            return DataPointSeries.GetResourceDictionaryWithTargetType((IResourceDictionaryDispenser)this.SeriesHost, typeof(T), true);
        }

        /// <summary>
        /// Updates a data point when its actual dependent value has changed.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointActualDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
            this.UpdateDataPoint(dataPoint);
            base.OnDataPointActualDependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Redraws other column series to assure they allocate the right amount
        /// of space for their columns.
        /// </summary>
        /// <param name="seriesHost">The series host to update.</param>
        protected void RedrawOtherSeries(ISeriesHost seriesHost)
        {
            Type thisType = typeof(ColumnBarBaseSeries<T>);
            foreach (ColumnBarBaseSeries<T> columnBarBaseSeries in seriesHost.Series.Where<ISeries>((Func<ISeries, bool>)(series => thisType.IsAssignableFrom(series.GetType()))).OfType<ColumnBarBaseSeries<T>>().Where<ColumnBarBaseSeries<T>>((Func<ColumnBarBaseSeries<T>, bool>)(series => series != this)))
                columnBarBaseSeries.UpdateDataPoints(columnBarBaseSeries.ActiveDataPoints);
        }

        /// <summary>
        /// Called after data points have been loaded from the items source.
        /// </summary>
        /// <param name="newDataPoints">New active data points.</param>
        /// <param name="oldDataPoints">Old inactive data points.</param>
        protected override void OnDataPointsChanged(IList<DataPoint> newDataPoints, IList<DataPoint> oldDataPoints)
        {
            base.OnDataPointsChanged(newDataPoints, oldDataPoints);
            this.CalculateDataPointLength();
            if (this.SeriesHost == null)
                return;
            this.RedrawOtherSeries(this.SeriesHost);
        }

        /// <summary>
        /// Redraw other column series when removed from a series host.
        /// </summary>
        /// <param name="oldValue">The old value of the series host property.</param>
        /// <param name="newValue">The new value of the series host property.</param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            base.OnSeriesHostPropertyChanged(oldValue, newValue);
            if (newValue != null && oldValue == null)
                return;
            this.RedrawOtherSeries(oldValue);
        }

        /// <summary>Creates the bar data point.</summary>
        /// <returns>A bar data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return (DataPoint)Activator.CreateInstance<T>();
        }

        /// <summary>Calculates the length of the data points.</summary>
        protected void CalculateDataPointLength()
        {
            if (this.ActualIndependentAxis is ICategoryAxis)
                return;
            IEnumerable<UnitValue> list = (IEnumerable<UnitValue>)this.ActiveDataPoints.Select<DataPoint, UnitValue>((Func<DataPoint, UnitValue>)(dataPoint => this.ActualIndependentAxis.GetPlotAreaCoordinate(dataPoint.ActualIndependentValue))).Where<UnitValue>((Func<UnitValue, bool>)(value => ValueHelper.CanGraph(value.Value))).OrderBy<UnitValue, double>((Func<UnitValue, double>)(value => value.Value)).ToList<UnitValue>();
            this._dataPointlength = EnumerableFunctions.Zip<UnitValue, UnitValue, Range<double>>(list, list.Skip<UnitValue>(1), (Func<UnitValue, UnitValue, Range<double>>)((left, right) => new Range<double>(left.Value, right.Value))).Select<Range<double>, double>((Func<Range<double>, double>)(range => range.Maximum - range.Minimum)).MinOrNullable<double>();
        }

        /// <summary>Returns the value margins for a given axis.</summary>
        /// <param name="consumer">The axis to retrieve the value margins for.</param>
        /// <returns>A sequence of value margins.</returns>
        protected override IEnumerable<ValueMargin> GetValueMargins(IValueMarginConsumer consumer)
        {
            double dependentValueMargin = this.ActualHeight / 10.0;
            IAxis axis = consumer as IAxis;
            if (axis != null && this.ActiveDataPoints.Any<DataPoint>())
            {
                Func<DataPoint, IComparable> selector = (Func<DataPoint, IComparable>)null;
                if (axis == this.InternalActualIndependentAxis)
                {
                    selector = (Func<DataPoint, IComparable>)(dataPoint => (IComparable)dataPoint.ActualIndependentValue);
                    DataPoint minimumPoint = this.ActiveDataPoints.MinOrNull<DataPoint>(selector);
                    DataPoint maximumPoint = this.ActiveDataPoints.MaxOrNull<DataPoint>(selector);
                    double minimumMargin = minimumPoint.GetMargin(axis);
                    yield return new ValueMargin((object)selector(minimumPoint), minimumMargin, minimumMargin);
                    double maximumMargin = maximumPoint.GetMargin(axis);
                    yield return new ValueMargin((object)selector(maximumPoint), maximumMargin, maximumMargin);
                }
                else if (axis == this.InternalActualDependentAxis)
                {
                    selector = (Func<DataPoint, IComparable>)(dataPoint => dataPoint.ActualDependentValue);
                    DataPoint minimumPoint = this.ActiveDataPoints.MinOrNull<DataPoint>(selector);
                    DataPoint maximumPoint = this.ActiveDataPoints.MaxOrNull<DataPoint>(selector);
                    yield return new ValueMargin((object)selector(minimumPoint), dependentValueMargin, dependentValueMargin);
                    yield return new ValueMargin((object)selector(maximumPoint), dependentValueMargin, dependentValueMargin);
                }
            }
        }

        /// <summary>Gets a range in which to render a data point.</summary>
        /// <param name="category">The category to retrieve the range for.</param>
        /// <returns>The range in which to render a data point.</returns>
        protected Range<UnitValue> GetCategoryRange(object category)
        {
            ICategoryAxis actualIndependentAxis = (ICategoryAxis)(this.ActualIndependentAxis as CategoryAxis);
            if (actualIndependentAxis != null)
                return actualIndependentAxis.GetPlotAreaCoordinateRange(category);
            UnitValue plotAreaCoordinate = this.ActualIndependentAxis.GetPlotAreaCoordinate(category);
            if (!ValueHelper.CanGraph(plotAreaCoordinate.Value) || !this._dataPointlength.HasValue)
                return new Range<UnitValue>();
            double num = this._dataPointlength.Value / 2.0;
            return new Range<UnitValue>(new UnitValue(plotAreaCoordinate.Value - num, plotAreaCoordinate.Unit), new UnitValue(plotAreaCoordinate.Value + num, plotAreaCoordinate.Unit));
        }

        IRangeAxis IAnchoredToOrigin.AnchoredAxis
        {
            get
            {
                return this.ActualDependentRangeAxis;
            }
        }
    }
}
