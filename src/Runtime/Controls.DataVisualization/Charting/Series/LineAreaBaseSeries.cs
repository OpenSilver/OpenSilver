// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls.DataVisualization.Collections;

#if MIGRATION
#else
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// A base class that contains methods used by both the line and area series.
    /// </summary>
    /// <typeparam name="T">The type of data point used by the series.</typeparam>
    public abstract class LineAreaBaseSeries<T> : DataPointSingleSeriesWithAxes where T : DataPoint, new()
    {
        /// <summary>
        /// Identifies the DependentRangeAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty DependentRangeAxisProperty = DependencyProperty.Register(nameof(DependentRangeAxis), typeof(IRangeAxis), typeof(LineAreaBaseSeries<T>), new PropertyMetadata((object)null, new PropertyChangedCallback(LineAreaBaseSeries<T>.OnDependentRangeAxisPropertyChanged)));
        /// <summary>Identifies the IndependentAxis dependency property.</summary>
        public static readonly DependencyProperty IndependentAxisProperty = DependencyProperty.Register(nameof(IndependentAxis), typeof(IAxis), typeof(LineAreaBaseSeries<T>), new PropertyMetadata((object)null, new PropertyChangedCallback(LineAreaBaseSeries<T>.OnIndependentAxisPropertyChanged)));

        /// <summary>Gets or sets the dependent range axis.</summary>
        public IRangeAxis DependentRangeAxis
        {
            get
            {
                return this.GetValue(LineAreaBaseSeries<T>.DependentRangeAxisProperty) as IRangeAxis;
            }
            set
            {
                this.SetValue(LineAreaBaseSeries<T>.DependentRangeAxisProperty, (object)value);
            }
        }

        /// <summary>DependentRangeAxisProperty property changed handler.</summary>
        /// <param name="d">LineAreaBaseSeries that changed its DependentRangeAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentRangeAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineAreaBaseSeries<T>)d).OnDependentRangeAxisPropertyChanged((IRangeAxis)e.NewValue);
        }

        /// <summary>DependentRangeAxisProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnDependentRangeAxisPropertyChanged(IRangeAxis newValue)
        {
            this.InternalDependentAxis = (IAxis)newValue;
        }

        /// <summary>Gets or sets the independent range axis.</summary>
        public IAxis IndependentAxis
        {
            get
            {
                return this.GetValue(LineAreaBaseSeries<T>.IndependentAxisProperty) as IAxis;
            }
            set
            {
                this.SetValue(LineAreaBaseSeries<T>.IndependentAxisProperty, (object)value);
            }
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="d">LineAreaBaseSeries that changed its IndependentAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineAreaBaseSeries<T>)d).OnIndependentAxisPropertyChanged((IAxis)e.NewValue);
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnIndependentAxisPropertyChanged(IAxis newValue)
        {
            this.InternalIndependentAxis = newValue;
        }

        /// <summary>
        /// Gets data points collection sorted by independent value.
        /// </summary>
        internal OrderedMultipleDictionary<IComparable, DataPoint> DataPointsByIndependentValue { get; private set; }

        /// <summary>Gets the independent axis as a range axis.</summary>
        public IAxis ActualIndependentAxis
        {
            get
            {
                return this.InternalActualIndependentAxis;
            }
        }

        /// <summary>Gets the dependent axis as a range axis.</summary>
        public IRangeAxis ActualDependentRangeAxis
        {
            get
            {
                return this.InternalActualDependentAxis as IRangeAxis;
            }
        }

        /// <summary>
        /// Initializes a new instance of the LineAreaBaseSeries class.
        /// </summary>
        protected LineAreaBaseSeries()
        {
            this.DataPointsByIndependentValue = new OrderedMultipleDictionary<IComparable, DataPoint>(false, (Comparison<IComparable>)((left, right) => left.CompareTo((object)right)), (Comparison<DataPoint>)((leftDataPoint, rightDataPoint) => RuntimeHelpers.GetHashCode((object)leftDataPoint).CompareTo(RuntimeHelpers.GetHashCode((object)rightDataPoint))));
        }

        /// <summary>Creates a DataPoint for determining the line color.</summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (null == this.PlotArea)
                return;
            Grid grid = new Grid();
            DataPoint dataPoint = this.CreateDataPoint();
            dataPoint.Visibility = Visibility.Collapsed;
            dataPoint.Loaded += (RoutedEventHandler)delegate
            {
                dataPoint.SetStyle(this.ActualDataPointStyle);
                this.Background = dataPoint.Background;
                if (null == this.PlotArea)
                    return;
                this.PlotArea.Children.Remove((UIElement)grid);
            };
            grid.Children.Add((UIElement)dataPoint);
            this.PlotArea.Children.Add((UIElement)grid);
        }

        /// <summary>
        /// Called after data points have been loaded from the items source.
        /// </summary>
        /// <param name="newDataPoints">New active data points.</param>
        /// <param name="oldDataPoints">Old inactive data points.</param>
        protected override void OnDataPointsChanged(IList<DataPoint> newDataPoints, IList<DataPoint> oldDataPoints)
        {
            base.OnDataPointsChanged(newDataPoints, oldDataPoints);
            if (!(this.ActualIndependentAxis is IRangeAxis))
                return;
            foreach (DataPoint oldDataPoint in (IEnumerable<DataPoint>)oldDataPoints)
                this.DataPointsByIndependentValue.Remove((IComparable)oldDataPoint.IndependentValue, oldDataPoint);
            foreach (DataPoint newDataPoint in (IEnumerable<DataPoint>)newDataPoints)
                this.DataPointsByIndependentValue.Add((IComparable)newDataPoint.IndependentValue, newDataPoint);
        }

        /// <summary>
        /// This method executes after all data points have been updated.
        /// </summary>
        protected override void OnAfterUpdateDataPoints()
        {
            if (this.InternalActualDependentAxis == null || this.InternalActualIndependentAxis == null)
                return;
            this.UpdateShape();
        }

        /// <summary>
        /// Repositions line data point in the sorted collection if the actual
        /// independent axis is a range axis.
        /// </summary>
        /// <param name="dataPoint">The data point that has changed.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            if (this.ActualIndependentAxis is IRangeAxis && !oldValue.Equals(newValue) && this.DataPointsByIndependentValue.Remove((IComparable)oldValue, dataPoint))
                this.DataPointsByIndependentValue.Add((IComparable)newValue, dataPoint);
            base.OnDataPointIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>Creates a new line data point.</summary>
        /// <returns>A line data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return (DataPoint)Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Returns the custom ResourceDictionary to use for necessary resources.
        /// </summary>
        /// <returns>ResourceDictionary to use for necessary resources.</returns>
        protected override IEnumerator<ResourceDictionary> GetResourceDictionaryEnumeratorFromHost()
        {
            return DataPointSeries.GetResourceDictionaryWithTargetType((IResourceDictionaryDispenser)this.SeriesHost, typeof(T), true);
        }

        /// <summary>Updates the visual representation of the data point.</summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            if (this.ActualDependentRangeAxis == null)
            {
                return;
            }
            double num1 = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)this.ActualDependentRangeAxis.Range.Maximum).Value;
            if (ValueHelper.CanGraph(num1))
            {
                double num2 = this.ActualIndependentAxis.GetPlotAreaCoordinate(dataPoint.ActualIndependentValue).Value;
                double num3 = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)dataPoint.ActualDependentValue).Value;
                if (ValueHelper.CanGraph(num2) && ValueHelper.CanGraph(num3))
                {
                    dataPoint.Visibility = Visibility.Visible;
                    double length1 = Math.Round(num1 - (num3 + dataPoint.ActualHeight / 2.0));
                    Canvas.SetTop((UIElement)dataPoint, length1);
                    double length2 = Math.Round(num2 - dataPoint.ActualWidth / 2.0);
                    Canvas.SetLeft((UIElement)dataPoint, length2);
                }
                else
                    dataPoint.Visibility = Visibility.Collapsed;
            }
            if (this.UpdatingDataPoints)
                return;
            this.UpdateShape();
        }

        /// <summary>Updates the Series shape object.</summary>
        protected virtual void UpdateShape()
        {
            double maximum = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)this.ActualDependentRangeAxis.Range.Maximum).Value;
            Func<DataPoint, Point> selector = (Func<DataPoint, Point>)(dataPoint =>
            {
                UnitValue plotAreaCoordinate = this.ActualIndependentAxis.GetPlotAreaCoordinate(dataPoint.ActualIndependentValue);
                double x = plotAreaCoordinate.Value;
                double num1 = maximum;
                plotAreaCoordinate = this.ActualDependentRangeAxis.GetPlotAreaCoordinate((object)dataPoint.ActualDependentValue);
                double num2 = plotAreaCoordinate.Value;
                double y = num1 - num2;
                return new Point(x, y);
            });
            IEnumerable<Point> points = Enumerable.Empty<Point>();
            if (ValueHelper.CanGraph(maximum))
                points = !(this.ActualIndependentAxis is IRangeAxis) ? (IEnumerable<Point>)this.ActiveDataPoints.Select<DataPoint, Point>(selector).OrderBy<Point, double>((Func<Point, double>)(point => point.X)) : this.DataPointsByIndependentValue.Select<DataPoint, Point>(selector);
            this.UpdateShapeFromPoints(points);
        }

        /// <summary>
        /// Updates the Series shape object from a collection of Points.
        /// </summary>
        /// <param name="points">Collection of Points.</param>
        protected abstract void UpdateShapeFromPoints(IEnumerable<Point> points);
    }
}
