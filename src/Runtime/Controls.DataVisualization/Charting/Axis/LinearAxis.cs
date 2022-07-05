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
    /// <summary>An axis that displays numeric values.</summary>
    [TemplatePart(Name = "AxisTitle", Type = typeof(Title))]
    [StyleTypedProperty(Property = "GridLineStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "MajorTickMarkStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "MinorTickMarkStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "AxisLabelStyle", StyleTargetType = typeof(NumericAxisLabel))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [TemplatePart(Name = "AxisGrid", Type = typeof(Grid))]
    public class LinearAxis : NumericAxis
    {
        /// <summary>Identifies the Interval dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(nameof(Interval), typeof(double?), typeof(LinearAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(LinearAxis.OnIntervalPropertyChanged)));
        /// <summary>Identifies the ActualInterval dependency property.</summary>
        public static readonly DependencyProperty ActualIntervalProperty = DependencyProperty.Register(nameof(ActualInterval), typeof(double), typeof(LinearAxis), new PropertyMetadata((object)double.NaN));

        /// <summary>Gets or sets the axis interval.</summary>
        [TypeConverter(typeof(NullableConverter<double>))]
        public double? Interval
        {
            get
            {
                return (double?)this.GetValue(LinearAxis.IntervalProperty);
            }
            set
            {
                this.SetValue(LinearAxis.IntervalProperty, (object)value);
            }
        }

        /// <summary>IntervalProperty property changed handler.</summary>
        /// <param name="d">LinearAxis that changed its Interval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LinearAxis)d).OnIntervalPropertyChanged();
        }

        /// <summary>IntervalProperty property changed handler.</summary>
        private void OnIntervalPropertyChanged()
        {
            this.OnInvalidated(new RoutedEventArgs());
        }

        /// <summary>Gets the actual interval of the axis.</summary>
        public double ActualInterval
        {
            get
            {
                return (double)this.GetValue(LinearAxis.ActualIntervalProperty);
            }
            private set
            {
                this.SetValue(LinearAxis.ActualIntervalProperty, (object)value);
            }
        }

        /// <summary>Instantiates a new instance of the LinearAxis class.</summary>
        public LinearAxis()
        {
            this.ActualRange = new Range<IComparable>((IComparable)0.0, (IComparable)1.0);
        }

        /// <summary>Gets the actual range of double values.</summary>
        protected Range<double> ActualDoubleRange { get; private set; }

        /// <summary>Updates ActualDoubleRange when ActualRange changes.</summary>
        /// <param name="range">New ActualRange value.</param>
        protected override void OnActualRangeChanged(Range<IComparable> range)
        {
            this.ActualDoubleRange = range.ToDoubleRange();
            base.OnActualRangeChanged(range);
        }

        /// <summary>Returns the plot area coordinate of a value.</summary>
        /// <param name="value">The value to plot.</param>
        /// <param name="length">The length of axis.</param>
        /// <returns>The plot area coordinate of a value.</returns>
        protected override UnitValue GetPlotAreaCoordinate(object value, double length)
        {
            return LinearAxis.GetPlotAreaCoordinate(value, this.ActualDoubleRange, length);
        }

        /// <summary>Returns the plot area coordinate of a value.</summary>
        /// <param name="value">The value to plot.</param>
        /// <param name="currentRange">The range of values.</param>
        /// <param name="length">The length of axis.</param>
        /// <returns>The plot area coordinate of a value.</returns>
        protected override UnitValue GetPlotAreaCoordinate(object value, Range<IComparable> currentRange, double length)
        {
            return LinearAxis.GetPlotAreaCoordinate(value, currentRange.ToDoubleRange(), length);
        }

        /// <summary>Returns the plot area coordinate of a value.</summary>
        /// <param name="value">The value to plot.</param>
        /// <param name="currentRange">The range of values.</param>
        /// <param name="length">The length of axis.</param>
        /// <returns>The plot area coordinate of a value.</returns>
        private static UnitValue GetPlotAreaCoordinate(object value, Range<double> currentRange, double length)
        {
            if (!currentRange.HasData)
                return UnitValue.NaN();
            double num1 = ValueHelper.ToDouble(value);
            double num2 = Math.Max(length - 1.0, 0.0);
            double num3 = currentRange.Maximum - currentRange.Minimum;
            return new UnitValue((num1 - currentRange.Minimum) * (num2 / num3), Unit.Pixels);
        }

        /// <summary>
        /// Returns the actual interval to use to determine which values are
        /// displayed in the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>Actual interval to use to determine which values are
        /// displayed in the axis.
        /// </returns>
        protected virtual double CalculateActualInterval(Size availableSize)
        {
            if (this.Interval.HasValue)
                return this.Interval.Value;
            double num1 = (this.Orientation == AxisOrientation.X ? 0.8 : 1.0) * 8.0;
            double num2 = Math.Max(this.GetLength(availableSize) * num1 / 200.0, 1.0);
            double num3 = this.ActualDoubleRange.Maximum - this.ActualDoubleRange.Minimum;
            double d = num3 / num2;
            double num4 = Math.Pow(10.0, Math.Floor(Math.Log10(d)));
            int[] numArray = new int[4] { 10, 5, 2, 1 };
            foreach (int num5 in numArray)
            {
                double num6 = num4 * (double)num5;
                if (num2 >= num3 / num6)
                    d = num6;
                else
                    break;
            }
            return d;
        }

        /// <summary>
        /// Returns a sequence of values to create major tick marks for.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to create major tick marks for.</returns>
        protected override IEnumerable<IComparable> GetMajorTickMarkValues(Size availableSize)
        {
            return this.GetMajorValues(availableSize).CastWrapper<IComparable>();
        }

        /// <summary>Returns a sequence of major axis values.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of major axis values.</returns>
        private IEnumerable<double> GetMajorValues(Size availableSize)
        {
            Range<IComparable> actualRange = this.ActualRange;
            int num1;
            if (actualRange.HasData)
            {
                actualRange = this.ActualRange;
                IComparable minimum = actualRange.Minimum;
                actualRange = this.ActualRange;
                IComparable maximum = actualRange.Maximum;
                if (ValueHelper.Compare(minimum, maximum) != 0)
                {
                    num1 = this.GetLength(availableSize) != 0.0 ? 1 : 0;
                    goto label_5;
                }
            }
            num1 = 0;
        label_5:
            if (num1 != 0)
            {
                this.ActualInterval = this.CalculateActualInterval(availableSize);
                double startValue = LinearAxis.AlignToInterval(this.ActualDoubleRange.Minimum, this.ActualInterval);
                Range<double> actualDoubleRange;
                if (startValue < this.ActualDoubleRange.Minimum)
                {
                    actualDoubleRange = this.ActualDoubleRange;
                    startValue = LinearAxis.AlignToInterval(actualDoubleRange.Minimum + this.ActualInterval, this.ActualInterval);
                }
                double nextValue = startValue;
                int counter = 1;
                while (true)
                {
                    double num2 = nextValue;
                    actualDoubleRange = this.ActualDoubleRange;
                    double maximum = actualDoubleRange.Maximum;
                    if (num2 <= maximum)
                    {
                        yield return nextValue;
                        nextValue = startValue + (double)counter * this.ActualInterval;
                        ++counter;
                    }
                    else
                        break;
                }
            }
        }

        /// <summary>Returns a sequence of values to plot on the axis.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        protected override IEnumerable<IComparable> GetLabelValues(Size availableSize)
        {
            return this.GetMajorValues(availableSize).CastWrapper<IComparable>();
        }

        /// <summary>
        /// Aligns a value to the provided interval value.  The aligned value
        /// should always be smaller than or equal to than the provided value.
        /// </summary>
        /// <param name="value">The value to align to the interval.</param>
        /// <param name="interval">The interval to align to.</param>
        /// <returns>The aligned value.</returns>
        private static double AlignToInterval(double value, double interval)
        {
            double num = interval;
            return ValueHelper.RemoveNoiseFromDoubleMath(ValueHelper.RemoveNoiseFromDoubleMath(Math.Floor(value / num)) * num);
        }

        /// <summary>Returns the value range given a plot area coordinate.</summary>
        /// <param name="value">The plot area position.</param>
        /// <returns>The value at that plot area coordinate.</returns>
        protected override IComparable GetValueAtPosition(UnitValue value)
        {
            if (!this.ActualRange.HasData || this.ActualLength == 0.0)
                return (IComparable)null;
            if (value.Unit != Unit.Pixels)
                throw new NotImplementedException();
            double num1 = value.Value;
            Range<double> actualDoubleRange = this.ActualDoubleRange;
            double maximum = actualDoubleRange.Maximum;
            actualDoubleRange = this.ActualDoubleRange;
            double minimum1 = actualDoubleRange.Minimum;
            double num2 = maximum - minimum1;
            double num3 = num1 * (num2 / this.ActualLength);
            actualDoubleRange = this.ActualDoubleRange;
            double minimum2 = actualDoubleRange.Minimum;
            return (IComparable)(num3 + minimum2);
        }

        /// <summary>
        /// Function that uses the mid point of all the data values
        /// in the value margins to convert a length into a range
        /// of data with the mid point as the center of that range.
        /// </summary>
        /// <param name="midPoint">The mid point of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns>The range object.</returns>
        private static Range<double> LengthToRange(double midPoint, double length)
        {
            double num = length / 2.0;
            return new Range<double>(midPoint - num, midPoint + num);
        }

        /// <summary>
        /// Overrides the actual range to ensure that it is never set to an
        /// empty range.
        /// </summary>
        /// <param name="range">The range to override.</param>
        /// <returns>Returns the overridden range.</returns>
        protected override Range<IComparable> OverrideDataRange(Range<IComparable> range)
        {
            range = base.OverrideDataRange(range);
            if (!range.HasData)
                return new Range<IComparable>((IComparable)0.0, (IComparable)1.0);
            if (ValueHelper.Compare(range.Minimum, range.Maximum) == 0)
                return new Range<IComparable>((IComparable)(ValueHelper.ToDouble((object)range.Minimum) - 1.0), (IComparable)(ValueHelper.ToDouble((object)range.Maximum) + 1.0));
            if (range.HasData && this.ActualLength > 1.0)
            {
                bool flag = false;
                IList<ValueMarginCoordinateAndOverlap> coordinateAndOverlapList = (IList<ValueMarginCoordinateAndOverlap>)new List<ValueMarginCoordinateAndOverlap>();
                foreach (IValueMarginProvider valueMarginProvider in this.RegisteredListeners.OfType<IValueMarginProvider>())
                {
                    foreach (ValueMargin valueMargin in valueMarginProvider.GetValueMargins((IValueMarginConsumer)this))
                    {
                        IAnchoredToOrigin anchoredToOrigin = valueMarginProvider as IAnchoredToOrigin;
                        flag = anchoredToOrigin != null && anchoredToOrigin.AnchoredAxis == this;
                        coordinateAndOverlapList.Add(new ValueMarginCoordinateAndOverlap()
                        {
                            ValueMargin = valueMargin
                        });
                    }
                }
                if (coordinateAndOverlapList.Count > 0)
                {
                    double? nullable = coordinateAndOverlapList.Select<ValueMarginCoordinateAndOverlap, double>((Func<ValueMarginCoordinateAndOverlap, double>)(valueMargin =>
                    {
                        ValueMargin valueMargin1 = valueMargin.ValueMargin;
                        double lowMargin = valueMargin1.LowMargin;
                        valueMargin1 = valueMargin.ValueMargin;
                        double highMargin = valueMargin1.HighMargin;
                        return lowMargin + highMargin;
                    })).MaxOrNullable<double>();
                    if (nullable.Value > this.ActualLength)
                        return range;
                    Range<double> doubleRange = range.ToDoubleRange();
                    Range<double> range1 = range.ToDoubleRange();
                    if (range1.Minimum == range1.Maximum)
                        range1 = new Range<double>(range1.Maximum - 1.0, range1.Maximum + 1.0);
                    double actualLength = this.ActualLength;
                    this.UpdateValueMargins(coordinateAndOverlapList, range1.ToComparableRange());
                    ValueMarginCoordinateAndOverlap maxLeftOverlapValueMargin;
                    ValueMarginCoordinateAndOverlap maxRightOverlapValueMargin;
                    RangeAxis.GetMaxLeftAndRightOverlap(coordinateAndOverlapList, out maxLeftOverlapValueMargin, out maxRightOverlapValueMargin);
                    while (maxLeftOverlapValueMargin.LeftOverlap > 0.0 || maxRightOverlapValueMargin.RightOverlap > 0.0)
                    {
                        nullable = range1.GetLength();
                        double num = nullable.Value / actualLength;
                        range1 = new Range<double>(range1.Minimum - (maxLeftOverlapValueMargin.LeftOverlap + 0.5) * num, range1.Maximum + (maxRightOverlapValueMargin.RightOverlap + 0.5) * num);
                        this.UpdateValueMargins(coordinateAndOverlapList, range1.ToComparableRange());
                        RangeAxis.GetMaxLeftAndRightOverlap(coordinateAndOverlapList, out maxLeftOverlapValueMargin, out maxRightOverlapValueMargin);
                    }
                    if (flag)
                    {
                        if (doubleRange.Minimum >= 0.0 && range1.Minimum < 0.0)
                            range1 = new Range<double>(0.0, range1.Maximum);
                        else if (doubleRange.Maximum <= 0.0 && range1.Maximum > 0.0)
                            range1 = new Range<double>(range1.Minimum, 0.0);
                    }
                    return range1.ToComparableRange();
                }
            }
            return range;
        }
    }
}