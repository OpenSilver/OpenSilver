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
using System;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// Represents a dynamic series that uses axes to display data points.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract class DataPointSeriesWithAxes : DataPointSeries, IDataProvider, IRangeProvider, IAxisListener, IValueMarginProvider
    {
        /// <summary>Stores the internal dependent axis.</summary>
        private IAxis _internalDependentAxis;
        /// <summary>The internal independent axis.</summary>
        private IAxis _internalIndependentAxis;

        /// <summary>Gets or sets the data points by dependent value.</summary>
        private OrderedMultipleDictionary<IComparable, DataPoint> DataPointsByActualDependentValue { get; set; }

        /// <summary>Creates the correct range axis based on the data.</summary>
        /// <param name="value">The value to evaluate to determine which type of
        /// axis to create.</param>
        /// <returns>The range axis appropriate that can plot the provided
        /// value.</returns>
        protected static IRangeAxis CreateRangeAxisFromData(object value)
        {
            double doubleValue;
            if (ValueHelper.TryConvert(value, out doubleValue))
                return (IRangeAxis)new LinearAxis();
            DateTime dateTimeValue;
            if (ValueHelper.TryConvert(value, out dateTimeValue))
                return (IRangeAxis)new DateTimeAxis();
            return (IRangeAxis)null;
        }

        /// <summary>
        /// Retrieves the value for a given access from a data point.
        /// </summary>
        /// <param name="dataPoint">The data point to retrieve the value from.</param>
        /// <param name="axis">The axis to retrieve the value for.</param>
        /// <returns>A function that returns a value appropriate for the axis
        /// when provided a DataPoint.</returns>
        protected virtual object GetActualDataPointAxisValue(DataPoint dataPoint, IAxis axis)
        {
            if (axis == this.InternalActualIndependentAxis)
                return dataPoint.ActualIndependentValue;
            if (axis == this.InternalActualDependentAxis)
                return (object)dataPoint.ActualDependentValue;
            return (object)null;
        }

        /// <summary>Gets or sets the actual dependent axis.</summary>
        protected IAxis InternalActualDependentAxis { get; set; }

        /// <summary>
        /// Gets or sets the value of the internal dependent axis.
        /// </summary>
        protected IAxis InternalDependentAxis
        {
            get
            {
                return this._internalDependentAxis;
            }
            set
            {
                if (this._internalDependentAxis == value)
                    return;
                IAxis internalDependentAxis = this._internalDependentAxis;
                this._internalDependentAxis = value;
                this.OnInternalDependentAxisPropertyChanged(internalDependentAxis, value);
            }
        }

        /// <summary>DependentAxisProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnInternalDependentAxisPropertyChanged(IAxis oldValue, IAxis newValue)
        {
            if (newValue == null || this.InternalActualDependentAxis == null || this.InternalActualDependentAxis == newValue || !this.InternalActualDependentAxis.RegisteredListeners.Contains((IAxisListener)this))
                return;
            this.InternalActualDependentAxis.RegisteredListeners.Remove((IAxisListener)this);
            this.InternalActualDependentAxis = (IAxis)null;
            this.GetAxes();
        }

        /// <summary>Gets or sets the actual independent axis value.</summary>
        protected IAxis InternalActualIndependentAxis { get; set; }

        /// <summary>
        /// Gets or sets the value of the internal independent axis.
        /// </summary>
        protected IAxis InternalIndependentAxis
        {
            get
            {
                return this._internalIndependentAxis;
            }
            set
            {
                if (value == this._internalIndependentAxis)
                    return;
                IAxis internalIndependentAxis = this._internalIndependentAxis;
                this._internalIndependentAxis = value;
                this.OnInternalIndependentAxisPropertyChanged(internalIndependentAxis, value);
            }
        }

        /// <summary>IndependentAxisProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnInternalIndependentAxisPropertyChanged(IAxis oldValue, IAxis newValue)
        {
            if (newValue == null || this.InternalActualIndependentAxis == null || this.InternalActualIndependentAxis == newValue || !this.InternalActualIndependentAxis.RegisteredListeners.Contains((IAxisListener)this))
                return;
            this.InternalActualIndependentAxis.RegisteredListeners.Remove((IAxisListener)this);
            this.InternalActualIndependentAxis = (IAxis)null;
            this.GetAxes();
        }

        /// <summary>
        /// Initializes a new instance of the DataPointSeriesWithAxes class.
        /// </summary>
        protected DataPointSeriesWithAxes()
        {
            this.DataPointsByActualDependentValue = new OrderedMultipleDictionary<IComparable, DataPoint>(false, (Comparison<IComparable>)((left, right) => left.CompareTo((object)right)), (Comparison<DataPoint>)((leftDataPoint, rightDataPoint) => RuntimeHelpers.GetHashCode((object)leftDataPoint).CompareTo(RuntimeHelpers.GetHashCode((object)rightDataPoint))));
        }

        /// <summary>
        /// Update the axes when the specified data point's ActualDependentValue property changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointActualDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
            if (oldValue != null && this.DataPointsByActualDependentValue.Remove(oldValue, dataPoint))
                this.DataPointsByActualDependentValue.Add(newValue, dataPoint);
            this.UpdateActualDependentAxis();
            this.UpdateDataPoint(dataPoint);
            base.OnDataPointActualDependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update the axes when the specified data point's DependentValue property changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
            if (null != this.InternalActualDependentAxis)
                dataPoint.BeginAnimation(DataPoint.ActualDependentValueProperty, "ActualDependentValue", (object)newValue, this.TransitionDuration, this.TransitionEasingFunction);
            else
                dataPoint.ActualDependentValue = newValue;
            base.OnDataPointDependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when the specified data point's effective dependent value changes.
        /// </summary>
        private void UpdateActualDependentAxis()
        {
            if (this.InternalActualDependentAxis == null)
                return;
            IDataConsumer actualDependentAxis1 = this.InternalActualDependentAxis as IDataConsumer;
            if (actualDependentAxis1 != null)
            {
                IDataProvider dataProvider = (IDataProvider)this;
                actualDependentAxis1.DataChanged(dataProvider, dataProvider.GetData(actualDependentAxis1));
            }
            IRangeConsumer actualDependentAxis2 = this.InternalActualDependentAxis as IRangeConsumer;
            if (actualDependentAxis2 != null)
            {
                IRangeProvider provider = (IRangeProvider)this;
                actualDependentAxis2.RangeChanged(provider, provider.GetRange(actualDependentAxis2));
            }
        }

        /// <summary>
        /// Update axes when the specified data point's actual independent value changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointActualIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            this.UpdateActualIndependentAxis();
            this.UpdateDataPoint(dataPoint);
            base.OnDataPointActualIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when the specified data point's independent value changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            if (this.InternalActualIndependentAxis != null && this.InternalActualIndependentAxis is IRangeAxis)
                dataPoint.BeginAnimation(DataPoint.ActualIndependentValueProperty, "ActualIndependentValue", newValue, this.TransitionDuration, this.TransitionEasingFunction);
            else
                dataPoint.ActualIndependentValue = newValue;
            base.OnDataPointIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when a data point's effective independent value changes.
        /// </summary>
        private void UpdateActualIndependentAxis()
        {
            if (this.InternalActualIndependentAxis == null)
                return;
            ICategoryAxis actualIndependentAxis1 = this.InternalActualIndependentAxis as ICategoryAxis;
            if (actualIndependentAxis1 != null)
            {
                IDataProvider dataProvider = (IDataProvider)this;
                actualIndependentAxis1.DataChanged(dataProvider, dataProvider.GetData((IDataConsumer)actualIndependentAxis1));
            }
            IRangeConsumer actualIndependentAxis2 = this.InternalActualIndependentAxis as IRangeConsumer;
            if (actualIndependentAxis2 != null)
            {
                IRangeProvider provider = (IRangeProvider)this;
                actualIndependentAxis2.RangeChanged(provider, provider.GetRange(actualIndependentAxis2));
            }
        }

        /// <summary>
        /// Called after data points have been loaded from the items source.
        /// </summary>
        /// <param name="newDataPoints">New active data points.</param>
        /// <param name="oldDataPoints">Old inactive data points.</param>
        protected override void OnDataPointsChanged(IList<DataPoint> newDataPoints, IList<DataPoint> oldDataPoints)
        {
            foreach (DataPoint newDataPoint in (IEnumerable<DataPoint>)newDataPoints)
                this.DataPointsByActualDependentValue.Add(newDataPoint.ActualDependentValue, newDataPoint);
            foreach (DataPoint oldDataPoint in (IEnumerable<DataPoint>)oldDataPoints)
                this.DataPointsByActualDependentValue.Remove(oldDataPoint.ActualDependentValue, oldDataPoint);
            this.GetAxes();
            if (this.InternalActualDependentAxis != null && this.InternalActualIndependentAxis != null)
                this.InvokeOnLayoutUpdated((Action)(() =>
                {
                    this.AxesInvalidated = false;
                    this.UpdatingAllAxes = true;
                    try
                    {
                        this.UpdateActualIndependentAxis();
                        this.UpdateActualDependentAxis();
                    }
                    finally
                    {
                        this.UpdatingAllAxes = false;
                    }
                    if (this.AxesInvalidated)
                        this.UpdateDataPoints(this.ActiveDataPoints);
                    else
                        this.UpdateDataPoints((IEnumerable<DataPoint>)newDataPoints);
                    this.AxesInvalidated = false;
                }));
            base.OnDataPointsChanged(newDataPoints, oldDataPoints);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to the axes are being
        /// updated.
        /// </summary>
        private bool UpdatingAllAxes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the axes have been
        /// invalidated.
        /// </summary>
        private bool AxesInvalidated { get; set; }

        /// <summary>Only updates all data points if series has axes.</summary>
        /// <param name="dataPoints">A sequence of data points to update.</param>
        protected override void UpdateDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            if (this.InternalActualIndependentAxis == null || this.InternalActualDependentAxis == null)
                return;
            base.UpdateDataPoints(dataPoints);
        }

        /// <summary>
        /// Method called to get series to acquire the axes it needs.  Acquires
        /// no axes by default.
        /// </summary>
        private void GetAxes()
        {
            if (this.SeriesHost == null)
                return;
            DataPoint firstDataPoint = this.ActiveDataPoints.FirstOrDefault<DataPoint>();
            if (firstDataPoint == null)
                return;
            this.GetAxes(firstDataPoint);
        }

        /// <summary>
        /// Method called to get series to acquire the axes it needs.  Acquires
        /// no axes by default.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected abstract void GetAxes(DataPoint firstDataPoint);

        /// <summary>Method called to get the axes that the series needs.</summary>
        /// <param name="firstDataPoint">The first data point.</param>
        /// <param name="independentAxisPredicate">A predicate that returns
        /// a value indicating whether an axis is an acceptable candidate for
        /// the series independent axis.</param>
        /// <param name="independentAxisFactory">A function that creates an
        /// acceptable independent axis.</param>
        /// <param name="dependentAxisPredicate">A predicate that returns
        /// a value indicating whether an axis is an acceptable candidate for
        /// the series dependent axis.</param>
        /// <param name="dependentAxisFactory">A function that creates an
        /// acceptable dependent axis.</param>
        protected virtual void GetAxes(DataPoint firstDataPoint, Func<IAxis, bool> independentAxisPredicate, Func<IAxis> independentAxisFactory, Func<IAxis, bool> dependentAxisPredicate, Func<IAxis> dependentAxisFactory)
        {
            Func<IAxis, bool> predicate1 = (Func<IAxis, bool>)(axis => independentAxisPredicate(axis) && axis.CanPlot(firstDataPoint.IndependentValue));
            IAxis axis1 = (IAxis)null;
            if (this.InternalActualIndependentAxis == null)
            {
                if (this.InternalIndependentAxis != null)
                {
                    if (!predicate1(this.InternalIndependentAxis))
                        throw new InvalidOperationException("DataPointSeriesWithAxes.GetAxes: Assigned Independent Axis Cannot Be Used");
                    axis1 = this.InternalIndependentAxis;
                }
                if (axis1 == null)
                    axis1 = this.SeriesHost.Axes.FirstOrDefault<IAxis>(predicate1);
                if (axis1 == null)
                    axis1 = independentAxisFactory();
                this.InternalActualIndependentAxis = axis1;
                if (!axis1.RegisteredListeners.Contains((IAxisListener)this))
                    axis1.RegisteredListeners.Add((IAxisListener)this);
                if (!this.SeriesHost.Axes.Contains(axis1))
                    this.SeriesHost.Axes.Add(axis1);
            }
            Func<IAxis, bool> predicate2 = (Func<IAxis, bool>)(axis => dependentAxisPredicate(axis) && axis.CanPlot((object)firstDataPoint.DependentValue));
            IAxis axis2 = (IAxis)null;
            if (this.InternalActualDependentAxis != null)
                return;
            if (this.InternalDependentAxis != null)
            {
                if (!predicate2(this.InternalDependentAxis))
                    throw new InvalidOperationException("DataPointSeriesWithAxes.GetAxes: Assigned Dependent Axis Cannot Be Used");
                axis2 = this.InternalDependentAxis;
            }
            if (axis2 == null)
                axis2 = this.InternalActualIndependentAxis.DependentAxes.Concat<IAxis>((IEnumerable<IAxis>)this.SeriesHost.Axes).FirstOrDefault<IAxis>(predicate2);
            if (axis2 == null)
                axis2 = dependentAxisFactory();
            this.InternalActualDependentAxis = axis2;
            if (!axis2.RegisteredListeners.Contains((IAxisListener)this))
                axis2.RegisteredListeners.Add((IAxisListener)this);
            if (!this.SeriesHost.Axes.Contains(axis2) && !this.InternalActualIndependentAxis.DependentAxes.Contains(axis2))
                this.SeriesHost.Axes.Add(axis2);
        }

        void IAxisListener.AxisInvalidated(IAxis axis)
        {
            if (this.InternalActualDependentAxis == null || this.InternalActualIndependentAxis == null || this.PlotArea == null)
                return;
            if (!this.UpdatingAllAxes)
                this.UpdateDataPoints(this.ActiveDataPoints);
            else
                this.AxesInvalidated = true;
        }

        /// <summary>Returns the actual range of data for a given axis.</summary>
        /// <param name="consumer">The axis to retrieve the range for.</param>
        /// <returns>The actual range of data.</returns>
        protected virtual Range<IComparable> GetRange(IRangeConsumer consumer)
        {
            if (consumer == null)
                throw new ArgumentNullException(nameof(consumer));
            if (consumer == this.InternalActualDependentAxis && this.DataPointsByActualDependentValue.Count > 0)
                return this.DataPointsByActualDependentValue.GetKeyRange();
            IAxis axis = consumer as IAxis;
            return axis != null ? this.ActiveDataPoints.Select<DataPoint, IComparable>((Func<DataPoint, IComparable>)(dataPoint => (IComparable)this.GetActualDataPointAxisValue(dataPoint, axis))).GetRange<IComparable>() : new Range<IComparable>();
        }

        /// <summary>Returns the value margins for a given axis.</summary>
        /// <param name="consumer">The axis to retrieve the value margins for.</param>
        /// <returns>A sequence of value margins.</returns>
        protected virtual IEnumerable<ValueMargin> GetValueMargins(IValueMarginConsumer consumer)
        {
            IAxis axis = consumer as IAxis;
            if (axis != null && this.ActiveDataPoints.Any<DataPoint>())
            {
                Func<DataPoint, IComparable> selector = (Func<DataPoint, IComparable>)null;
                DataPoint minimumPoint = (DataPoint)null;
                DataPoint maximumPoint = (DataPoint)null;
                double margin = 0.0;
                if (axis == this.InternalActualIndependentAxis)
                {
                    selector = (Func<DataPoint, IComparable>)(dataPoint => (IComparable)dataPoint.ActualIndependentValue);
                    minimumPoint = this.ActiveDataPoints.MinOrNull<DataPoint>(selector);
                    maximumPoint = this.ActiveDataPoints.MaxOrNull<DataPoint>(selector);
                    margin = minimumPoint.GetActualMargin(this.InternalActualIndependentAxis);
                }
                else if (axis == this.InternalActualDependentAxis)
                {
                    selector = (Func<DataPoint, IComparable>)(dataPoint => dataPoint.ActualDependentValue);
                    Tuple<DataPoint, DataPoint> andSmallestValues = this.DataPointsByActualDependentValue.GetLargestAndSmallestValues();
                    minimumPoint = andSmallestValues.Item1;
                    maximumPoint = andSmallestValues.Item2;
                    margin = minimumPoint.GetActualMargin(this.InternalActualDependentAxis);
                }
                yield return new ValueMargin((object)selector(minimumPoint), margin, margin);
                yield return new ValueMargin((object)selector(maximumPoint), margin, margin);
            }
        }

        IEnumerable<object> IDataProvider.GetData(IDataConsumer dataConsumer)
        {
            IAxis axis = (IAxis)dataConsumer;
            if (axis == null)
                throw new ArgumentNullException(nameof(dataConsumer));
            Func<DataPoint, object> selector = (Func<DataPoint, object>)null;
            if (axis == this.InternalActualIndependentAxis)
            {
                if (this.IndependentValueBinding == null)
                    return Enumerable.Range(1, this.ActiveDataPointCount).CastWrapper<object>();
                selector = (Func<DataPoint, object>)(dataPoint => dataPoint.ActualIndependentValue ?? (object)dataPoint.ActualDependentValue);
            }
            else if (axis == this.InternalActualDependentAxis)
                selector = (Func<DataPoint, object>)(dataPoint => (object)dataPoint.ActualDependentValue);
            return this.ActiveDataPoints.Select<DataPoint, object>(selector).Distinct<object>();
        }

        /// <summary>
        /// Called when the value of the SeriesHost property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new series host value.</param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            if (oldValue != null)
            {
                if (this.InternalActualIndependentAxis != null)
                {
                    this.InternalActualIndependentAxis.RegisteredListeners.Remove((IAxisListener)this);
                    this.InternalActualIndependentAxis = (IAxis)null;
                }
                if (this.InternalActualDependentAxis != null)
                {
                    this.InternalActualDependentAxis.RegisteredListeners.Remove((IAxisListener)this);
                    this.InternalActualDependentAxis = (IAxis)null;
                }
            }
            base.OnSeriesHostPropertyChanged(oldValue, newValue);
        }

        Range<IComparable> IRangeProvider.GetRange(IRangeConsumer rangeConsumer)
        {
            return this.GetRange(rangeConsumer);
        }

        IEnumerable<ValueMargin> IValueMarginProvider.GetValueMargins(IValueMarginConsumer axis)
        {
            return this.GetValueMargins(axis);
        }
    }
}
