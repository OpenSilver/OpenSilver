// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a dynamic series that uses axes to display data points.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [OpenSilver.NotImplemented]
    public abstract class DataPointSeriesWithAxes : DataPointSeries //, IDataProvider, IRangeProvider, IAxisListener, IValueMarginProvider
    {
        /// <summary>
        /// Retrieves the value for a given access from a data point.
        /// </summary>
        /// <param name="dataPoint">The data point to retrieve the value from.</param>
        /// <param name="axis">The axis to retrieve the value for.</param>
        /// <returns>A function that returns a value appropriate for the axis
        /// when provided a DataPoint.</returns>
        protected virtual object GetActualDataPointAxisValue(DataPoint dataPoint, IAxis axis)
        {
            if (axis == InternalActualIndependentAxis)
            {
                return dataPoint.ActualIndependentValue;
            }
            else if (axis == InternalActualDependentAxis)
            {
                return dataPoint.ActualDependentValue;
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the actual dependent axis.
        /// </summary>
        protected IAxis InternalActualDependentAxis { get; set; }

        #region public Axis InternalDependentAxis

        /// <summary>
        /// Stores the internal dependent axis.
        /// </summary>
        private IAxis _internalDependentAxis;

        /// <summary>
        /// Gets or sets the value of the internal dependent axis.
        /// </summary>
        protected IAxis InternalDependentAxis
        {
            get { return _internalDependentAxis; }
            set
            {
                if (_internalDependentAxis != value)
                {
                    IAxis oldValue = _internalDependentAxis;
                    _internalDependentAxis = value;
                    OnInternalDependentAxisPropertyChanged(oldValue, value);
                }
            }
        }

        /// <summary>
        /// DependentAxisProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnInternalDependentAxisPropertyChanged(IAxis oldValue, IAxis newValue)
        {
        }
        #endregion public Axis InternalDependentAxis

        /// <summary>
        /// Gets or sets the actual independent axis value.
        /// </summary>
        protected IAxis InternalActualIndependentAxis { get; set; }

        #region protected Axis InternalIndependentAxis

        /// <summary>
        /// The internal independent axis.
        /// </summary>
        private IAxis _internalIndependentAxis;

        /// <summary>
        /// Gets or sets the value of the internal independent axis.
        /// </summary>
        protected IAxis InternalIndependentAxis
        {
            get { return _internalIndependentAxis; }
            set
            {
                if (value != _internalIndependentAxis)
                {
                    IAxis oldValue = _internalIndependentAxis;
                    _internalIndependentAxis = value;
                    OnInternalIndependentAxisPropertyChanged(oldValue, value);
                }
            }
        }

        /// <summary>
        /// IndependentAxisProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnInternalIndependentAxisPropertyChanged(IAxis oldValue, IAxis newValue)
        {
        }
        #endregion protected Axis IndependentAxis

        /// <summary>
        /// Initializes a new instance of the DataPointSeriesWithAxes class.
        /// </summary>
        [OpenSilver.NotImplemented]
        protected DataPointSeriesWithAxes()
        {
        }

        /// <summary>
        /// Update the axes when the specified data point's ActualDependentValue property changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        [OpenSilver.NotImplemented]
        protected override void OnDataPointActualDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
        }

        /// <summary>
        /// Update the axes when the specified data point's DependentValue property changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        [OpenSilver.NotImplemented]
        protected override void OnDataPointDependentValueChanged(DataPoint dataPoint, IComparable oldValue, IComparable newValue)
        {
            base.OnDataPointDependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when the specified data point's effective dependent value changes.
        /// </summary>
        [OpenSilver.NotImplemented]
        private void UpdateActualDependentAxis()
        {
        }

        /// <summary>
        /// Update axes when the specified data point's actual independent value changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointActualIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            UpdateActualIndependentAxis();
            UpdateDataPoint(dataPoint);
            base.OnDataPointActualIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when the specified data point's independent value changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        [OpenSilver.NotImplemented]
        protected override void OnDataPointIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            base.OnDataPointIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when a data point's effective independent value changes.
        /// </summary>
        [OpenSilver.NotImplemented]
        private void UpdateActualIndependentAxis()
        {
        }

        /// <summary>
        /// Called after data points have been loaded from the items source.
        /// </summary>
        /// <param name="newDataPoints">New active data points.</param>
        /// <param name="oldDataPoints">Old inactive data points.</param>
        [OpenSilver.NotImplemented]
        protected override void OnDataPointsChanged(IList<DataPoint> newDataPoints, IList<DataPoint> oldDataPoints)
        {
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

        /// <summary>
        /// Only updates all data points if series has axes.
        /// </summary>
        /// <param name="dataPoints">A sequence of data points to update.
        /// </param>
        protected override void UpdateDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            if (InternalActualIndependentAxis != null && InternalActualDependentAxis != null)
            {
                base.UpdateDataPoints(dataPoints);
            }
        }

        /// <summary>
        /// Method called to get series to acquire the axes it needs.  Acquires
        /// no axes by default.
        /// </summary>
        [OpenSilver.NotImplemented]
        private void GetAxes()
        {
        }

        /// <summary>
        /// Method called to get series to acquire the axes it needs.  Acquires
        /// no axes by default.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected abstract void GetAxes(DataPoint firstDataPoint);

        /// <summary>
        /// Method called to get the axes that the series needs.
        /// </summary>
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
        [OpenSilver.NotImplemented]
        protected virtual void GetAxes(DataPoint firstDataPoint, Func<IAxis, bool> independentAxisPredicate, Func<IAxis> independentAxisFactory, Func<IAxis, bool> dependentAxisPredicate, Func<IAxis> dependentAxisFactory)
        {
        }
    }
}