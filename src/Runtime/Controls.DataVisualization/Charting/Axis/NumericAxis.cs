// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
using System;
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>An axis that displays numeric values.</summary>
    public abstract class NumericAxis : RangeAxis
    {
        /// <summary>Identifies the ActualMaximum dependency property.</summary>
        public static readonly DependencyProperty ActualMaximumProperty = DependencyProperty.Register(nameof(ActualMaximum), typeof(double?), typeof(NumericAxis), (PropertyMetadata)null);
        /// <summary>Identifies the ActualMinimum dependency property.</summary>
        public static readonly DependencyProperty ActualMinimumProperty = DependencyProperty.Register(nameof(ActualMinimum), typeof(double?), typeof(NumericAxis), (PropertyMetadata)null);
        /// <summary>Identifies the Maximum dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double?), typeof(NumericAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(NumericAxis.OnMaximumPropertyChanged)));
        /// <summary>Identifies the Minimum dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double?), typeof(NumericAxis), new PropertyMetadata((object)null, new PropertyChangedCallback(NumericAxis.OnMinimumPropertyChanged)));
        /// <summary>
        /// Identifies the ExtendRangeToOrigin dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtendRangeToOriginProperty = DependencyProperty.Register(nameof(ExtendRangeToOrigin), typeof(bool), typeof(NumericAxis), new PropertyMetadata((object)false, new PropertyChangedCallback(NumericAxis.OnExtendRangeToOriginPropertyChanged)));

        /// <summary>Gets the actual maximum value plotted on the chart.</summary>
        public double? ActualMaximum
        {
            get
            {
                return (double?)this.GetValue(NumericAxis.ActualMaximumProperty);
            }
            private set
            {
                this.SetValue(NumericAxis.ActualMaximumProperty, (object)value);
            }
        }

        /// <summary>Gets the actual maximum value plotted on the chart.</summary>
        public double? ActualMinimum
        {
            get
            {
                return (double?)this.GetValue(NumericAxis.ActualMinimumProperty);
            }
            private set
            {
                this.SetValue(NumericAxis.ActualMinimumProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the maximum value plotted on the axis.</summary>
        [TypeConverter(typeof(NullableConverter<double>))]
        public double? Maximum
        {
            get
            {
                return (double?)this.GetValue(NumericAxis.MaximumProperty);
            }
            set
            {
                this.SetValue(NumericAxis.MaximumProperty, (object)value);
            }
        }

        /// <summary>MaximumProperty property changed handler.</summary>
        /// <param name="d">NumericAxis that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericAxis)d).OnMaximumPropertyChanged((double?)e.NewValue);
        }

        /// <summary>MaximumProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        protected virtual void OnMaximumPropertyChanged(double? newValue)
        {
            this.ProtectedMaximum = (IComparable)newValue;
        }

        /// <summary>Gets or sets the minimum value to plot on the axis.</summary>
        [TypeConverter(typeof(NullableConverter<double>))]
        public double? Minimum
        {
            get
            {
                return (double?)this.GetValue(NumericAxis.MinimumProperty);
            }
            set
            {
                this.SetValue(NumericAxis.MinimumProperty, (object)value);
            }
        }

        /// <summary>MinimumProperty property changed handler.</summary>
        /// <param name="d">NumericAxis that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericAxis)d).OnMinimumPropertyChanged((double?)e.NewValue);
        }

        /// <summary>MinimumProperty property changed handler.</summary>
        /// <param name="newValue">New value.</param>
        protected virtual void OnMinimumPropertyChanged(double? newValue)
        {
            this.ProtectedMinimum = (IComparable)newValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to always show the origin.
        /// </summary>
        public bool ExtendRangeToOrigin
        {
            get
            {
                return (bool)this.GetValue(NumericAxis.ExtendRangeToOriginProperty);
            }
            set
            {
                this.SetValue(NumericAxis.ExtendRangeToOriginProperty, (object)value);
            }
        }

        /// <summary>ExtendRangeToOriginProperty property changed handler.</summary>
        /// <param name="d">NumericAxis that changed its ExtendRangeToOrigin.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnExtendRangeToOriginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericAxis)d).OnExtendRangeToOriginPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>ExtendRangeToOriginProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnExtendRangeToOriginPropertyChanged(bool oldValue, bool newValue)
        {
            this.ActualRange = this.OverrideDataRange(this.ActualRange);
        }

        /// <summary>Gets the origin value on the axis.</summary>
        protected override IComparable Origin
        {
            get
            {
                return (IComparable)0.0;
            }
        }

        /// <summary>
        /// Updates the typed actual maximum and minimum properties when the
        /// actual range changes.
        /// </summary>
        /// <param name="range">The actual range.</param>
        protected override void OnActualRangeChanged(Range<IComparable> range)
        {
            if (range.HasData)
            {
                this.ActualMaximum = new double?((double)range.Maximum);
                this.ActualMinimum = new double?((double)range.Minimum);
            }
            else
            {
                this.ActualMaximum = new double?();
                this.ActualMinimum = new double?();
            }
            base.OnActualRangeChanged(range);
        }

        /// <summary>Returns a value indicating whether a value can plot.</summary>
        /// <param name="value">The value to plot.</param>
        /// <returns>A value indicating whether a value can plot.</returns>
        public override bool CanPlot(object value)
        {
            double doubleValue;
            return ValueHelper.TryConvert(value, out doubleValue);
        }

        /// <summary>Returns a numeric axis label.</summary>
        /// <returns>A numeric axis label.</returns>
        protected override Control CreateAxisLabel()
        {
            return (Control)new NumericAxisLabel();
        }

        /// <summary>
        /// Overrides the data value range and returns a range that takes the
        /// margins of the values into account.
        /// </summary>
        /// <param name="range">The range of data values.</param>
        /// <returns>A range that can store both the data values and their
        /// margins.</returns>
        protected override Range<IComparable> OverrideDataRange(Range<IComparable> range)
        {
            range = base.OverrideDataRange(range);
            if (!this.ExtendRangeToOrigin)
                return range;
            Range<double> doubleRange = range.ToDoubleRange();
            if (!doubleRange.HasData)
                return new Range<IComparable>((IComparable)0.0, (IComparable)0.0);
            double num1 = doubleRange.Minimum;
            double num2 = doubleRange.Maximum;
            if (num1 > 0.0)
                num1 = 0.0;
            else if (num2 < 0.0)
                num2 = 0.0;
            return new Range<IComparable>((IComparable)num1, (IComparable)num2);
        }
    }
}