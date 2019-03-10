
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents an element that has a value within a specific range, such as the ProgressBar, ScrollBar, and Slider controls.
    /// </summary>
    public class RangeBase : Control
    {
        /// <summary>
        /// Provides base class initialization behavior for RangeBase-derived classes.
        /// </summary>
        protected RangeBase()
        {
        }


        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the Value of a RangeBase control. The default is 1.
        /// </summary>
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        /// <summary>
        /// Identifies the LargeChange dependency property.
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register("LargeChange", typeof(double), typeof(RangeBase), new PropertyMetadata(1d) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });



        /// <summary>
        /// Gets or sets the highest possible Value of the range element. The default is 1.
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeBase), new PropertyMetadata(1d, Maximum_Changed) { CoerceValueCallback = CoerceMaximum, CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static object CoerceMaximum(DependencyObject d, object baseValue)
        {
            RangeBase rangeBase = (RangeBase)d;
            double returnValue = baseValue is double ? (double)baseValue : double.NaN;
            double minimum = rangeBase.Minimum;
            if (!double.IsNaN(minimum) && returnValue < minimum)
            {
                return minimum;
            }
            return returnValue;
        }

        private static void Maximum_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeBase = (RangeBase)d;
            object newValue = e.NewValue;

            double newMax = newValue is double ? (double)newValue : double.NaN;
            double oldMax = e.OldValue is double ? (double)e.OldValue : double.NaN;

            rangeBase.Coerce(ValueProperty);
            rangeBase.OnMaximumChanged(oldMax, newMax);
        }


        /// <summary>
        /// Gets or sets the Minimum possible Value of the range element. The default is 0.
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeBase), new PropertyMetadata(0d, Minimum_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Minimum_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeBase = (RangeBase)d;
            object newValue = e.NewValue;
            object oldValue = e.OldValue;
            double oldMin = oldValue is double ? (double)oldValue : double.NaN;
            double newMin = newValue is double ? (double)newValue : double.NaN;

            rangeBase.Coerce(MaximumProperty);
            rangeBase.Coerce(ValueProperty);
            rangeBase.OnMinimumChanged(oldMin, newMin);
        }



        /// <summary>
        /// Gets or sets a Value to be added to or subtracted from the Value of a RangeBase control. The default is 0.1.
        /// </summary>
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        /// <summary>
        /// The identifier for the SmallChange dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(RangeBase), new PropertyMetadata(0.1d) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });



        /// <summary>
        /// Gets or sets the current setting of the range control, which may be coerced. The default is 0.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// The identifier for the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(RangeBase), new PropertyMetadata(0d, Value_Changed) { CoerceValueCallback = CoerceValue, CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            RangeBase rangeBase = (RangeBase)d;
            double returnValue = baseValue is double ? (double)baseValue : double.NaN;
            double minimum = rangeBase.Minimum;
            if (!double.IsNaN(minimum) && returnValue < minimum)
            {
                return minimum;
            }
            double maximum = rangeBase.Maximum;
            if (!double.IsNaN(maximum) && returnValue > maximum)
            {
                return maximum;
            }
            return returnValue;
        }

        private static void Value_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeBase = (RangeBase)d;
            object newValue = e.NewValue;
            object oldValue = e.OldValue;
            rangeBase.OnValueChanged(newValue is double ? (double)newValue : double.NaN, oldValue is double ? (double)oldValue : double.NaN);
        }

        /// <summary>
        /// Occurs when the range value changes.
        /// </summary>
#if MIGRATION
        public event RoutedPropertyChangedEventHandler<double> ValueChanged;
#else
        public event RangeBaseValueChangedEventHandler ValueChanged;
#endif

        /// <summary>
        /// Called when the Maximum property changes.
        /// </summary>
        /// <param name="oldMaximum">Old value of the Maximum property.</param>
        /// <param name="newMaximum">New value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        /// <summary>
        /// Called when the Minimum property changes.
        /// </summary>
        /// <param name="oldMinimum">Old value of the Minimum property.</param>
        /// <param name="newMinimum">New value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        /// <summary>
        /// Fires the ValueChanged routed event.
        /// </summary>
        /// <param name="oldValue">Old value of the Value property.</param>
        /// <param name="newValue">New value of the Value property.</param>
        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            if (this.ValueChanged != null)
#if MIGRATION
                this.ValueChanged(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
#else
                this.ValueChanged(this, new RangeBaseValueChangedEventArgs(oldValue, newValue));
#endif
        }
    }
}
