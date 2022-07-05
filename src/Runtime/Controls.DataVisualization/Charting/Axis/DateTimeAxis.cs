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
    [TemplatePart(Name = "AxisGrid", Type = typeof(Grid))]
    [StyleTypedProperty(Property = "GridLineStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "MajorTickMarkStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "MinorTickMarkStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "AxisLabelStyle", StyleTargetType = typeof(DateTimeAxisLabel))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    public class DateTimeAxis : RangeAxis
    {
        /// <summary>Identifies the ActualMaximum dependency property.</summary>
        public static readonly DependencyProperty ActualMaximumProperty = DependencyProperty.Register(nameof(ActualMaximum), typeof(DateTime?), typeof(DateTimeAxis), (PropertyMetadata)null);
        /// <summary>Identifies the ActualMinimum dependency property.</summary>
        public static readonly DependencyProperty ActualMinimumProperty = DependencyProperty.Register(nameof(ActualMinimum), typeof(DateTime?), typeof(DateTimeAxis), (PropertyMetadata)null);
        /// <summary>Identifies the Maximum dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(DateTime?), typeof(DateTimeAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxis.OnMaximumPropertyChanged)));
        /// <summary>Identifies the Minimum dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(DateTime?), typeof(DateTimeAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxis.OnMinimumPropertyChanged)));
        /// <summary>Identifies the Interval dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(nameof(Interval), typeof(double?), typeof(DateTimeAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(DateTimeAxis.OnIntervalPropertyChanged)));
        /// <summary>Identifies the ActualInterval dependency property.</summary>
        public static readonly DependencyProperty ActualIntervalProperty = DependencyProperty.Register(nameof(ActualInterval), typeof(double), typeof(DateTimeAxis), new PropertyMetadata((object)double.NaN));
        /// <summary>
        /// Identifies the InternalIntervalType dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalTypeProperty = DependencyProperty.Register(nameof(IntervalType), typeof(DateTimeIntervalType), typeof(DateTimeAxis), new PropertyMetadata((object)DateTimeIntervalType.Auto, new PropertyChangedCallback(DateTimeAxis.OnIntervalTypePropertyChanged)));
        /// <summary>
        /// Identifies the ActualIntervalType dependency property.
        /// </summary>
        private static readonly DependencyProperty ActualIntervalTypeProperty = DependencyProperty.Register(nameof(ActualIntervalType), typeof(DateTimeIntervalType), typeof(DateTimeAxis), new PropertyMetadata((object)DateTimeIntervalType.Auto));

        /// <summary>Gets the actual maximum value plotted on the chart.</summary>
        public DateTime? ActualMaximum
        {
            get
            {
                return (DateTime?)this.GetValue(DateTimeAxis.ActualMaximumProperty);
            }
            private set
            {
                this.SetValue(DateTimeAxis.ActualMaximumProperty, (object)value);
            }
        }

        /// <summary>Gets the actual maximum value plotted on the chart.</summary>
        public DateTime? ActualMinimum
        {
            get
            {
                return (DateTime?)this.GetValue(DateTimeAxis.ActualMinimumProperty);
            }
            private set
            {
                this.SetValue(DateTimeAxis.ActualMinimumProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the maximum value plotted on the axis.</summary>
        [TypeConverter(typeof(NullableConverter<DateTime>))]
        public DateTime? Maximum
        {
            get
            {
                return (DateTime?)this.GetValue(DateTimeAxis.MaximumProperty);
            }
            set
            {
                this.SetValue(DateTimeAxis.MaximumProperty, (object)value);
            }
        }

        /// <summary>MaximumProperty property changed handler.</summary>
        /// <param name="d">DateTimeAxis2 that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis)d).OnMaximumPropertyChanged((DateTime?)e.NewValue);
        }

        /// <summary>MaximumProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnMaximumPropertyChanged(DateTime? newValue)
        {
            this.ProtectedMaximum = (IComparable)newValue;
        }

        /// <summary>Gets or sets the minimum value to plot on the axis.</summary>
        [TypeConverter(typeof(NullableConverter<DateTime>))]
        public DateTime? Minimum
        {
            get
            {
                return (DateTime?)this.GetValue(DateTimeAxis.MinimumProperty);
            }
            set
            {
                this.SetValue(DateTimeAxis.MinimumProperty, (object)value);
            }
        }

        /// <summary>MinimumProperty property changed handler.</summary>
        /// <param name="d">DateTimeAxis2 that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis)d).OnMinimumPropertyChanged((DateTime?)e.NewValue);
        }

        /// <summary>MinimumProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnMinimumPropertyChanged(DateTime? newValue)
        {
            this.ProtectedMinimum = (IComparable)newValue;
        }

        /// <summary>Gets or sets the axis interval.</summary>
        [TypeConverter(typeof(NullableConverter<double>))]
        public double? Interval
        {
            get
            {
                return (double?)this.GetValue(DateTimeAxis.IntervalProperty);
            }
            set
            {
                this.SetValue(DateTimeAxis.IntervalProperty, (object)value);
            }
        }

        /// <summary>IntervalProperty property changed handler.</summary>
        /// <param name="d">DateTimeAxis2 that changed its Interval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis)d).OnIntervalPropertyChanged();
        }

        /// <summary>IntervalProperty property changed handler.</summary>
        private void OnIntervalPropertyChanged()
        {
            this.Invalidate();
        }

        /// <summary>Gets the actual interval.</summary>
        public double ActualInterval
        {
            get
            {
                return (double)this.GetValue(DateTimeAxis.ActualIntervalProperty);
            }
            private set
            {
                this.SetValue(DateTimeAxis.ActualIntervalProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the interval to use for the axis.</summary>
        public DateTimeIntervalType IntervalType
        {
            get
            {
                return (DateTimeIntervalType)this.GetValue(DateTimeAxis.IntervalTypeProperty);
            }
            set
            {
                this.SetValue(DateTimeAxis.IntervalTypeProperty, (object)value);
            }
        }

        /// <summary>IntervalTypeProperty property changed handler.</summary>
        /// <param name="d">DateTimeAxis that changed its InternalIntervalType.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIntervalTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeAxis)d).OnIntervalTypePropertyChanged((DateTimeIntervalType)e.NewValue);
        }

        /// <summary>IntervalTypeProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        private void OnIntervalTypePropertyChanged(DateTimeIntervalType newValue)
        {
            this.ActualIntervalType = newValue;
            this.Invalidate();
        }

        /// <summary>Gets or sets the actual interval type.</summary>
        private DateTimeIntervalType ActualIntervalType
        {
            get
            {
                return (DateTimeIntervalType)this.GetValue(DateTimeAxis.ActualIntervalTypeProperty);
            }
            set
            {
                this.SetValue(DateTimeAxis.ActualIntervalTypeProperty, (object)value);
            }
        }

        /// <summary>Gets the origin value on the axis.</summary>
        protected override IComparable Origin
        {
            get
            {
                return (IComparable)null;
            }
        }

        /// <summary>
        /// Instantiates a new instance of the DateTimeAxis2 class.
        /// </summary>
        public DateTimeAxis()
        {
            int year = DateTime.Now.Year;
            this.ActualRange = new Range<IComparable>((IComparable)new DateTime(year, 1, 1), (IComparable)new DateTime(year + 1, 1, 1));
        }

        /// <summary>
        /// Creates a new instance of the DateTimeAxisLabel class.
        /// </summary>
        /// <returns>Returns  a new instance of the DateTimeAxisLabel class.</returns>
        protected override Control CreateAxisLabel()
        {
            return (Control)new DateTimeAxisLabel();
        }

        /// <summary>
        /// Prepares an instance of the DateTimeAxisLabel class by setting its
        /// IntervalType property.
        /// </summary>
        /// <param name="label">An instance of the DateTimeAxisLabel class.</param>
        /// <param name="dataContext">The data context to assign to the label.</param>
        protected override void PrepareAxisLabel(Control label, object dataContext)
        {
            DateTimeAxisLabel dateTimeAxisLabel = label as DateTimeAxisLabel;
            if (dateTimeAxisLabel != null)
                dateTimeAxisLabel.IntervalType = this.ActualIntervalType;
            base.PrepareAxisLabel(label, dataContext);
        }

        /// <summary>Gets the actual range of DateTime values.</summary>
        protected Range<DateTime> ActualDateTimeRange { get; private set; }

        /// <summary>
        /// Updates the typed actual maximum and minimum properties when the
        /// actual range changes.
        /// </summary>
        /// <param name="range">The actual range.</param>
        protected override void OnActualRangeChanged(Range<IComparable> range)
        {
            this.ActualDateTimeRange = range.ToDateTimeRange();
            if (range.HasData)
            {
                this.ActualMaximum = new DateTime?((DateTime)range.Maximum);
                this.ActualMinimum = new DateTime?((DateTime)range.Minimum);
            }
            else
            {
                this.ActualMaximum = new DateTime?();
                this.ActualMinimum = new DateTime?();
            }
            base.OnActualRangeChanged(range);
        }

        /// <summary>Returns a value indicating whether a value can plot.</summary>
        /// <param name="value">The value to plot.</param>
        /// <returns>A value indicating whether a value can plot.</returns>
        public override bool CanPlot(object value)
        {
            DateTime dateTimeValue;
            return ValueHelper.TryConvert(value, out dateTimeValue);
        }

        /// <summary>Returns the plot area coordinate of a value.</summary>
        /// <param name="value">The value to plot.</param>
        /// <param name="length">The length of the axis.</param>
        /// <returns>The plot area coordinate of a value.</returns>
        protected override UnitValue GetPlotAreaCoordinate(object value, double length)
        {
            return DateTimeAxis.GetPlotAreaCoordinate(value, this.ActualDateTimeRange, length);
        }

        /// <summary>Returns the plot area coordinate of a value.</summary>
        /// <param name="value">The value to plot.</param>
        /// <param name="currentRange">The range to use determine the coordinate.</param>
        /// <param name="length">The length of the axis.</param>
        /// <returns>The plot area coordinate of a value.</returns>
        protected override UnitValue GetPlotAreaCoordinate(object value, Range<IComparable> currentRange, double length)
        {
            return DateTimeAxis.GetPlotAreaCoordinate(value, currentRange.ToDateTimeRange(), length);
        }

        /// <summary>Returns the plot area coordinate of a value.</summary>
        /// <param name="value">The value to plot.</param>
        /// <param name="currentRange">The range to use determine the coordinate.</param>
        /// <param name="length">The length of the axis.</param>
        /// <returns>The plot area coordinate of a value.</returns>
        private static UnitValue GetPlotAreaCoordinate(object value, Range<DateTime> currentRange, double length)
        {
            if (!currentRange.HasData)
                return UnitValue.NaN();
            DateTime dateTime1 = ValueHelper.ToDateTime(value);
            DateTime dateTime2 = currentRange.Maximum;
            double oaDate1 = dateTime2.ToOADate();
            dateTime2 = currentRange.Minimum;
            double oaDate2 = dateTime2.ToOADate();
            double num1 = oaDate1 - oaDate2;
            double num2 = Math.Max(length - 1.0, 0.0);
            double oaDate3 = dateTime1.ToOADate();
            dateTime2 = currentRange.Minimum;
            double oaDate4 = dateTime2.ToOADate();
            return new UnitValue((oaDate3 - oaDate4) * (num2 / num1), Unit.Pixels);
        }

        /// <summary>
        /// Returns the actual interval to use to determine which values are
        /// displayed in the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The actual interval to use to determine which values are
        /// displayed in the axis.
        /// </returns>
        private double CalculateActualInterval(Size availableSize)
        {
            if (this.Interval.HasValue)
                return this.Interval.Value;
            Range<DateTime> actualDateTimeRange = this.ActualDateTimeRange;
            DateTime minimum = actualDateTimeRange.Minimum;
            actualDateTimeRange = this.ActualDateTimeRange;
            DateTime maximum = actualDateTimeRange.Maximum;
            Size availableSize1 = availableSize;
            DateTimeIntervalType type;
            double dateTimeInterval = this.CalculateDateTimeInterval(minimum, maximum, out type, availableSize1);
            this.ActualIntervalType = type;
            return dateTimeInterval;
        }

        /// <summary>Returns a sequence of major values.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of major values.</returns>
        protected virtual IEnumerable<DateTime> GetMajorAxisValues(Size availableSize)
        {
            Range<IComparable> actualRange = this.ActualRange;
            int num;
            if (actualRange.HasData)
            {
                actualRange = this.ActualRange;
                IComparable minimum = actualRange.Minimum;
                actualRange = this.ActualRange;
                IComparable maximum = actualRange.Maximum;
                if (ValueHelper.Compare(minimum, maximum) != 0)
                {
                    num = this.GetLength(availableSize) != 0.0 ? 1 : 0;
                    goto label_4;
                }
            }
            num = 0;
        label_4:
            if (num != 0)
            {
                this.ActualInterval = this.CalculateActualInterval(availableSize);
                DateTime date = this.ActualDateTimeRange.Minimum;
                DateTime start = DateTimeAxis.AlignIntervalStart(date, this.ActualInterval, this.ActualIntervalType);
                while (start < date)
                    start = this.IncrementDateTime(start, this.ActualInterval);
                IEnumerable<DateTime> intermediateDates = EnumerableFunctions.Iterate<DateTime>(start, (Func<DateTime, DateTime>)(next => this.IncrementDateTime(next, this.ActualInterval))).TakeWhile<DateTime>((Func<DateTime, bool>)(current => this.ActualDateTimeRange.Contains(current)));
                foreach (DateTime dateTime in intermediateDates)
                    yield return dateTime;
            }
        }

        /// <summary>
        /// Returns a sequence of values to create major tick marks for.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to create major tick marks for.</returns>
        protected override IEnumerable<IComparable> GetMajorTickMarkValues(Size availableSize)
        {
            return this.GetMajorAxisValues(availableSize).CastWrapper<IComparable>();
        }

        /// <summary>Returns a sequence of values to plot on the axis.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        protected override IEnumerable<IComparable> GetLabelValues(Size availableSize)
        {
            return this.GetMajorAxisValues(availableSize).CastWrapper<IComparable>();
        }

        /// <summary>This method accepts a date time and increments it.</summary>
        /// <param name="date">A date time.</param>
        /// <param name="interval">The interval used to increment the date time.</param>
        /// <returns>The new date time.</returns>
        private DateTime IncrementDateTime(DateTime date, double interval)
        {
            DateTimeIntervalType actualIntervalType = this.ActualIntervalType;
            TimeSpan timeSpan = new TimeSpan(0L);
            if (actualIntervalType == DateTimeIntervalType.Days)
                timeSpan = TimeSpan.FromDays(interval);
            else if (actualIntervalType == DateTimeIntervalType.Hours)
                timeSpan = TimeSpan.FromHours(interval);
            else if (actualIntervalType == DateTimeIntervalType.Milliseconds)
                timeSpan = TimeSpan.FromMilliseconds(interval);
            else if (actualIntervalType == DateTimeIntervalType.Seconds)
                timeSpan = TimeSpan.FromSeconds(interval);
            else if (actualIntervalType == DateTimeIntervalType.Minutes)
                timeSpan = TimeSpan.FromMinutes(interval);
            else if (actualIntervalType == DateTimeIntervalType.Weeks)
                timeSpan = TimeSpan.FromDays(7.0 * interval);
            else if (actualIntervalType == DateTimeIntervalType.Months)
            {
                bool flag = false;
                if (date.Day == DateTime.DaysInMonth(date.Year, date.Month))
                    flag = true;
                date = date.AddMonths((int)Math.Floor(interval));
                timeSpan = TimeSpan.FromDays(30.0 * (interval - Math.Floor(interval)));
                if (flag && timeSpan.Ticks == 0L)
                {
                    int num = DateTime.DaysInMonth(date.Year, date.Month);
                    date = date.AddDays((double)(num - date.Day));
                }
            }
            else if (actualIntervalType == DateTimeIntervalType.Years)
            {
                date = date.AddYears((int)Math.Floor(interval));
                timeSpan = TimeSpan.FromDays(365.0 * (interval - Math.Floor(interval)));
            }
            return date.Add(timeSpan);
        }

        /// <summary>
        /// Adjusts the beginning of the first interval depending on the type and size.
        /// </summary>
        /// <param name="start">Original start point.</param>
        /// <param name="intervalSize">Interval size.</param>
        /// <param name="type">Type of the interval (Month, Year, ...).</param>
        /// <returns>Adjusted interval start position.</returns>
        private static DateTime AlignIntervalStart(DateTime start, double intervalSize, DateTimeIntervalType type)
        {
            if (type == DateTimeIntervalType.Auto)
                return start;
            DateTime dateTime1 = start;
            if (intervalSize > 0.0 && intervalSize != 1.0 && (type == DateTimeIntervalType.Months && intervalSize <= 12.0 && intervalSize > 1.0))
            {
                DateTime dateTime2 = dateTime1;
                for (DateTime dateTime3 = new DateTime(dateTime1.Year, 1, 1, 0, 0, 0); dateTime3 < dateTime1; dateTime3 = dateTime3.AddMonths((int)intervalSize))
                    dateTime2 = dateTime3;
                return dateTime2;
            }
            switch (type)
            {
                case DateTimeIntervalType.Milliseconds:
                    int millisecond = (int)((double)(int)((double)dateTime1.Millisecond / intervalSize) * intervalSize);
                    dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime1.Hour, dateTime1.Minute, dateTime1.Second, millisecond);
                    break;
                case DateTimeIntervalType.Seconds:
                    int second = (int)((double)(int)((double)dateTime1.Second / intervalSize) * intervalSize);
                    dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime1.Hour, dateTime1.Minute, second, 0);
                    break;
                case DateTimeIntervalType.Minutes:
                    int minute = (int)((double)(int)((double)dateTime1.Minute / intervalSize) * intervalSize);
                    dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime1.Hour, minute, 0);
                    break;
                case DateTimeIntervalType.Hours:
                    int hour = (int)((double)(int)((double)dateTime1.Hour / intervalSize) * intervalSize);
                    dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, hour, 0, 0);
                    break;
                case DateTimeIntervalType.Days:
                    int day = (int)((double)(int)((double)dateTime1.Day / intervalSize) * intervalSize);
                    if (day <= 0)
                        day = 1;
                    dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, day, 0, 0, 0);
                    break;
                case DateTimeIntervalType.Weeks:
                    dateTime1 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, 0, 0, 0);
                    dateTime1 = dateTime1.AddDays((double)-(int)dateTime1.DayOfWeek);
                    break;
                case DateTimeIntervalType.Months:
                    int month = (int)((double)(int)((double)dateTime1.Month / intervalSize) * intervalSize);
                    if (month <= 0)
                        month = 1;
                    dateTime1 = new DateTime(dateTime1.Year, month, 1, 0, 0, 0);
                    break;
                case DateTimeIntervalType.Years:
                    int year = (int)((double)(int)((double)dateTime1.Year / intervalSize) * intervalSize);
                    if (year <= 0)
                        year = 1;
                    dateTime1 = new DateTime(year, 1, 1, 0, 0, 0);
                    break;
            }
            return dateTime1;
        }

        /// <summary>Returns the value range given a plot area coordinate.</summary>
        /// <param name="value">The position.</param>
        /// <returns>A range of values at that plot area coordinate.</returns>
        protected override IComparable GetValueAtPosition(UnitValue value)
        {
            if (!this.ActualRange.HasData || this.ActualLength == 0.0)
                return (IComparable)null;
            double num1 = value.Value;
            if (value.Unit != Unit.Pixels)
                throw new NotImplementedException();
            DateTime dateTime = this.ActualDateTimeRange.Minimum;
            double oaDate = dateTime.ToOADate();
            dateTime = this.ActualDateTimeRange.Maximum;
            double num2 = dateTime.ToOADate() - oaDate;
            return (IComparable)DateTime.FromOADate(num1 * (num2 / this.ActualLength) + oaDate);
        }

        /// <summary>
        /// Recalculates a DateTime interval obtained from maximum and minimum.
        /// </summary>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="type">Date time interval type.</param>
        /// <param name="availableSize">The available size.</param>
        /// <returns>Auto Interval.</returns>
        private double CalculateDateTimeInterval(DateTime minimum, DateTime maximum, out DateTimeIntervalType type, Size availableSize)
        {
            DateTime dateTime = minimum;
            TimeSpan timeSpan = maximum.Subtract(dateTime);
            double num1 = this.Orientation == AxisOrientation.X ? 6.4 : 8.0;
            double num2 = this.GetLength(availableSize) / (2000.0 / num1);
            timeSpan = new TimeSpan((long)((double)timeSpan.Ticks / num2));
            double totalMinutes = timeSpan.TotalMinutes;
            if (totalMinutes <= 1.0)
            {
                double totalMilliseconds = timeSpan.TotalMilliseconds;
                if (totalMilliseconds <= 10.0)
                {
                    type = DateTimeIntervalType.Milliseconds;
                    return 1.0;
                }
                if (totalMilliseconds <= 50.0)
                {
                    type = DateTimeIntervalType.Milliseconds;
                    return 4.0;
                }
                if (totalMilliseconds <= 200.0)
                {
                    type = DateTimeIntervalType.Milliseconds;
                    return 20.0;
                }
                if (totalMilliseconds <= 500.0)
                {
                    type = DateTimeIntervalType.Milliseconds;
                    return 50.0;
                }
                double totalSeconds = timeSpan.TotalSeconds;
                if (totalSeconds <= 7.0)
                {
                    type = DateTimeIntervalType.Seconds;
                    return 1.0;
                }
                if (totalSeconds <= 15.0)
                {
                    type = DateTimeIntervalType.Seconds;
                    return 2.0;
                }
                if (totalSeconds <= 30.0)
                {
                    type = DateTimeIntervalType.Seconds;
                    return 5.0;
                }
                if (totalSeconds <= 60.0)
                {
                    type = DateTimeIntervalType.Seconds;
                    return 10.0;
                }
            }
            else
            {
                if (totalMinutes <= 2.0)
                {
                    type = DateTimeIntervalType.Seconds;
                    return 20.0;
                }
                if (totalMinutes <= 3.0)
                {
                    type = DateTimeIntervalType.Seconds;
                    return 30.0;
                }
                if (totalMinutes <= 10.0)
                {
                    type = DateTimeIntervalType.Minutes;
                    return 1.0;
                }
                if (totalMinutes <= 20.0)
                {
                    type = DateTimeIntervalType.Minutes;
                    return 2.0;
                }
                if (totalMinutes <= 60.0)
                {
                    type = DateTimeIntervalType.Minutes;
                    return 5.0;
                }
                if (totalMinutes <= 120.0)
                {
                    type = DateTimeIntervalType.Minutes;
                    return 10.0;
                }
                if (totalMinutes <= 180.0)
                {
                    type = DateTimeIntervalType.Minutes;
                    return 30.0;
                }
                if (totalMinutes <= 720.0)
                {
                    type = DateTimeIntervalType.Hours;
                    return 1.0;
                }
                if (totalMinutes <= 1440.0)
                {
                    type = DateTimeIntervalType.Hours;
                    return 4.0;
                }
                if (totalMinutes <= 2880.0)
                {
                    type = DateTimeIntervalType.Hours;
                    return 6.0;
                }
                if (totalMinutes <= 4320.0)
                {
                    type = DateTimeIntervalType.Hours;
                    return 12.0;
                }
                if (totalMinutes <= 14400.0)
                {
                    type = DateTimeIntervalType.Days;
                    return 1.0;
                }
                if (totalMinutes <= 28800.0)
                {
                    type = DateTimeIntervalType.Days;
                    return 2.0;
                }
                if (totalMinutes <= 43200.0)
                {
                    type = DateTimeIntervalType.Days;
                    return 3.0;
                }
                if (totalMinutes <= 87840.0)
                {
                    type = DateTimeIntervalType.Weeks;
                    return 1.0;
                }
                if (totalMinutes <= 219600.0)
                {
                    type = DateTimeIntervalType.Weeks;
                    return 2.0;
                }
                if (totalMinutes <= 527040.0)
                {
                    type = DateTimeIntervalType.Months;
                    return 1.0;
                }
                if (totalMinutes <= 1054080.0)
                {
                    type = DateTimeIntervalType.Months;
                    return 3.0;
                }
                if (totalMinutes <= 2108160.0)
                {
                    type = DateTimeIntervalType.Months;
                    return 6.0;
                }
            }
            type = DateTimeIntervalType.Years;
            double num3 = totalMinutes / 60.0 / 24.0 / 365.0;
            if (num3 < 5.0)
                return 1.0;
            if (num3 < 10.0)
                return 2.0;
            return Math.Floor(num3 / 5.0);
        }

        /// <summary>
        /// Overrides the actual range to ensure that it is never set to an
        /// empty range.
        /// </summary>
        /// <param name="range">The range to override.</param>
        /// <returns>The overridden range.</returns>
        protected override Range<IComparable> OverrideDataRange(Range<IComparable> range)
        {
            Range<IComparable> range1 = range;
            if (!range1.HasData)
            {
                int year = DateTime.Now.Year;
                return new Range<IComparable>((IComparable)new DateTime(year, 1, 1), (IComparable)new DateTime(year + 1, 1, 1));
            }
            if (ValueHelper.Compare(range1.Minimum, range1.Maximum) == 0)
            {
                DateTime dateTime = ValueHelper.ToDateTime((object)range1.Minimum);
                DateTime date = (DateTime.MinValue == dateTime ? DateTime.Now : dateTime).Date;
                return new Range<IComparable>((IComparable)date.AddMonths(-6), (IComparable)date.AddMonths(6));
            }
            if (range.HasData && this.ActualLength > 1.0)
            {
                IList<ValueMarginCoordinateAndOverlap> coordinateAndOverlapList = (IList<ValueMarginCoordinateAndOverlap>)new List<ValueMarginCoordinateAndOverlap>();
                foreach (ValueMargin valueMargin in this.RegisteredListeners.OfType<IValueMarginProvider>().SelectMany<IValueMarginProvider, ValueMargin>((Func<IValueMarginProvider, IEnumerable<ValueMargin>>)(provider => provider.GetValueMargins((IValueMarginConsumer)this))))
                    coordinateAndOverlapList.Add(new ValueMarginCoordinateAndOverlap()
                    {
                        ValueMargin = valueMargin
                    });
                if (coordinateAndOverlapList.Count > 0 && coordinateAndOverlapList.Select<ValueMarginCoordinateAndOverlap, double>((Func<ValueMarginCoordinateAndOverlap, double>)(valueMargin =>
                {
                    ValueMargin valueMargin1 = valueMargin.ValueMargin;
                    double lowMargin = valueMargin1.LowMargin;
                    valueMargin1 = valueMargin.ValueMargin;
                    double highMargin = valueMargin1.HighMargin;
                    return lowMargin + highMargin;
                })).MaxOrNullable<double>().Value <= this.ActualLength)
                {
                    Range<DateTime> range2 = range.ToDateTimeRange();
                    if (range2.Minimum == range2.Maximum)
                    {
                        int year = DateTime.Now.Year;
                        range2 = new Range<DateTime>(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1));
                    }
                    double actualLength = this.ActualLength;
                    this.UpdateValueMargins(coordinateAndOverlapList, range2.ToComparableRange());
                    ValueMarginCoordinateAndOverlap maxLeftOverlapValueMargin;
                    ValueMarginCoordinateAndOverlap maxRightOverlapValueMargin;
                    RangeAxis.GetMaxLeftAndRightOverlap(coordinateAndOverlapList, out maxLeftOverlapValueMargin, out maxRightOverlapValueMargin);
                    while (maxLeftOverlapValueMargin.LeftOverlap > 0.0 || maxRightOverlapValueMargin.RightOverlap > 0.0)
                    {
                        long num = range2.GetLength().Value.Ticks / (long)actualLength;
                        range2 = new Range<DateTime>(new DateTime(range2.Minimum.Ticks - (long)((maxLeftOverlapValueMargin.LeftOverlap + 0.5) * (double)num)), new DateTime(range2.Maximum.Ticks + (long)((maxRightOverlapValueMargin.RightOverlap + 0.5) * (double)num)));
                        this.UpdateValueMargins(coordinateAndOverlapList, range2.ToComparableRange());
                        RangeAxis.GetMaxLeftAndRightOverlap(coordinateAndOverlapList, out maxLeftOverlapValueMargin, out maxRightOverlapValueMargin);
                    }
                    return range2.ToComparableRange();
                }
            }
            return range;
        }
    }
}