// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if MIGRATION
using System.Windows.Shapes;
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// An axis that displays numeric values.
    /// </summary>
    [StyleTypedProperty(Property = "GridLineStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "MajorTickMarkStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "MinorTickMarkStyle", StyleTargetType = typeof(Line))]
    //[StyleTypedProperty(Property = "AxisLabelStyle", StyleTargetType = typeof(NumericAxisLabel))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [TemplatePart(Name = AxisGridName, Type = typeof(Grid))]
    [TemplatePart(Name = AxisTitleName, Type = typeof(Title))]
    [OpenSilver.NotImplemented]
    public class LinearAxis : NumericAxis
    {
        #region public double? Interval
        /// <summary>
        /// Gets or sets the axis interval.
        /// </summary>
        [TypeConverter(typeof(NullableConverter<double>))]
        public double? Interval
        {
            get { return (double?)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Identifies the Interval dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval",
                typeof(double?),
                typeof(LinearAxis),
                new PropertyMetadata(null, OnIntervalPropertyChanged));

        /// <summary>
        /// IntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">LinearAxis that changed its Interval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearAxis source = (LinearAxis)d;
            source.OnIntervalPropertyChanged();
        }

        /// <summary>
        /// IntervalProperty property changed handler.
        /// </summary>
        private void OnIntervalPropertyChanged()
        {
            OnInvalidated(new RoutedEventArgs());
        }
        #endregion public double? Interval

        #region public double ActualInterval
        /// <summary>
        /// Gets the actual interval of the axis.
        /// </summary>
        public double ActualInterval
        {
            get { return (double)GetValue(ActualIntervalProperty); }
            private set { SetValue(ActualIntervalProperty, value); }
        }

        /// <summary>
        /// Identifies the ActualInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualIntervalProperty =
            DependencyProperty.Register(
                "ActualInterval",
                typeof(double),
                typeof(LinearAxis),
                new PropertyMetadata(double.NaN));
        #endregion public double ActualInterval

        /// <summary>
        /// Instantiates a new instance of the LinearAxis class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public LinearAxis()
        {
            //this.ActualRange = new Range<IComparable>(0.0, 1.0);
        }

        /// <summary>
        /// Returns the actual interval to use to determine which values are 
        /// displayed in the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>Actual interval to use to determine which values are 
        /// displayed in the axis.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected virtual double CalculateActualInterval(Size availableSize)
        {
            return 0;
        }

        /// <summary>
        /// Returns a sequence of values to create major tick marks for.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to create major tick marks for.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected override IEnumerable<IComparable> GetMajorTickMarkValues(Size availableSize)
        {
            return Enumerable.Empty<IComparable>();
        }

        /// <summary>
        /// Returns a sequence of major axis values.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of major axis values.
        /// </returns>
        [OpenSilver.NotImplemented]
        private IEnumerable<double> GetMajorValues(Size availableSize)
        {
            return Enumerable.Empty<double>();
        }

        /// <summary>
        /// Returns a sequence of values to plot on the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        [OpenSilver.NotImplemented]
        protected override IEnumerable<IComparable> GetLabelValues(Size availableSize)
        {
            return Enumerable.Empty<IComparable>();
        }

        /// <summary>
        /// Aligns a value to the provided interval value.  The aligned value
        /// should always be smaller than or equal to than the provided value.
        /// </summary>
        /// <param name="value">The value to align to the interval.</param>
        /// <param name="interval">The interval to align to.</param>
        /// <returns>The aligned value.</returns>
        [OpenSilver.NotImplemented]
        private static double AlignToInterval(double value, double interval)
        {
            return 0;
        }
    }
}