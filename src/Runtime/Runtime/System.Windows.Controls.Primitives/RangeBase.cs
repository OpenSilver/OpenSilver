
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents an element that has a value within a specific range, such as the 
    /// see cref="ProgressBar"/>, <see cref="ScrollBar"/>, and <see cref="Slider"/>
    /// controls.
    /// </summary>
    public abstract class RangeBase : Control
    {
        /// <summary>
        /// Gets or sets the <see cref="Minimum"/> possible <see cref="Value"/> of the range element.
        /// </summary>
        /// <returns>
        /// <see cref="Minimum"/> possible <see cref="Value"/> of the range element. The default is 0.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The new value is either <see cref="double.NaN"/> or <see cref="double.IsInfinity(double)"/>
        /// is true.
        /// </exception>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValueInternal(MinimumProperty, value); }
        }

        /// <summary> 
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(double),
                typeof(RangeBase),
                new PropertyMetadata(0.0d, OnMinimumPropertyChanged));

        /// <summary>
        /// MinimumProperty property changed handler.
        /// </summary> 
        /// <param name="d">RangeBase that changed its Minimum.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        /// 
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase range = d as RangeBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value 
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            range.CoerceValue(MaximumProperty);
            range.CoerceValue(ValueProperty);
            range.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the highest possible <see cref="Value"/> of the range element.
        /// </summary>
        /// <returns>
        /// The highest possible <see cref="Value"/> of the range element. The default is 1.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The new value is either <see cref="double.NaN"/> or <see cref="double.IsInfinity(double)"/>
        /// is true.
        /// </exception>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValueInternal(MaximumProperty, value); }
        }

        /// <summary> 
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(double),
                typeof(RangeBase),
                new PropertyMetadata(1.0d, OnMaximumPropertyChanged, CoerceMaximum));

        /// <summary>
        /// MaximumProperty property changed handler.
        /// </summary> 
        /// <param name="d">RangeBase that changed its Maximum.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase range = d as RangeBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            range.CoerceValue(ValueProperty);
            range.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            RangeBase ctrl = (RangeBase)d;
            double min = ctrl.Minimum;
            if ((double)value < min)
            {
                return min;
            }
            return value;
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="Value"/> of a 
        /// <see cref="RangeBase"/> control.
        /// </summary>
        /// <returns>
        /// Value to add to or subtract from the <see cref="Value"/> of the <see cref="RangeBase"/>
        /// element. The default is 1.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The new value is <see cref="double.NaN"/>, less than zero, or <see cref="double.IsInfinity(double)"/>
        /// is true.
        /// </exception>
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValueInternal(LargeChangeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LargeChange"/> dependency property.
        /// </summary> 
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register(
                nameof(LargeChange),
                typeof(double),
                typeof(RangeBase),
                new PropertyMetadata(1.0d, OnLargeChangePropertyChanged));

        /// <summary> 
        /// LargeChangeProperty property changed handler. 
        /// </summary>
        /// <param name="d">RangeBase that changed its LargeChange.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnLargeChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase range = d as RangeBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value 
            if (!IsValidChange(e.NewValue))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="Value"/> of 
        /// a <see cref="RangeBase"/> control.
        /// </summary>
        /// <returns>
        /// Value to add to or subtract from the <see cref="Value"/> of the <see cref="RangeBase"/> 
        /// element. The default is 0.1.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The new value is <see cref="double.NaN"/>, less than zero, or <see cref="double.IsInfinity(double)"/>
        /// is true.
        /// </exception>
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValueInternal(SmallChangeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SmallChange"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(
                nameof(SmallChange),
                typeof(double),
                typeof(RangeBase),
                new PropertyMetadata(0.1d, OnSmallChangePropertyChanged));

        /// <summary> 
        /// SmallChangeProperty property changed handler.
        /// </summary>
        /// <param name="d">RangeBase that changed its SmallChange.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSmallChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase range = d as RangeBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidChange(e.NewValue))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets or sets the current setting of the range control, which may be coerced.
        /// </summary>
        /// <returns>
        /// The current setting of the range control, which may be coerced. The default is 0.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The new value is either <see cref="double.NaN"/> or <see cref="double.IsInfinity(double)"/>
        /// is true.
        /// </exception>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValueInternal(ValueProperty, value); }
        }

        /// <summary> 
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(RangeBase),
                new PropertyMetadata(0.0d, OnValuePropertyChanged, CoerceValue));

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary> 
        /// <param name="d">RangeBase that changed its Value.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase range = d as RangeBase;
            Debug.Assert(range != null);

            // Ensure it's a valid value
            if (!IsValidDoubleValue(e.NewValue))
            {
                throw new ArgumentException();
            }

            range.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static object CoerceValue(DependencyObject d, object value)
        {
            RangeBase ctrl = (RangeBase)d;
            double min = ctrl.Minimum;
            double v = (double)value;
            if (v < min)
            {
                return min;
            }

            double max = ctrl.Maximum;
            if (v > max)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// Occurs when the range value changes.
        /// </summary> 
        public event RoutedPropertyChangedEventHandler<double> ValueChanged;

        /// <summary> 
        /// Initializes a new instance of the <see cref="RangeBase"/> class.
        /// </summary>
        protected RangeBase()
        {
        }

        /// <summary>
        /// Called when <see cref="Maximum"/> property changes.
        /// </summary>
        /// <param name="oldMaximum">
        /// Old value of the <see cref="Maximum"/> property.
        /// </param>
        /// <param name="newMaximum">
        /// New value of the <see cref="Maximum"/> property.
        /// </param>
        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        /// <summary>
        /// Called when the <see cref="Minimum"/> property changes.
        /// </summary>
        /// <param name="oldMinimum">
        /// Old value of the <see cref="Minimum"/> property.
        /// </param>
        /// <param name="newMinimum">
        /// New value of the <see cref="Minimum"/> property.
        /// </param>
        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> routed event.
        /// </summary>
        /// <param name="oldValue">
        /// Old value of the <see cref="Value"/> property.
        /// </param>
        /// <param name="newValue">
        /// New value of the <see cref="Value"/> property.
        /// </param>
        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
        }

        /// <summary> 
        /// Check if a value is a value double. 
        /// </summary>
        /// <param name="value">Value.</param> 
        /// <returns>true if a valid double; false otherwise.</returns>
        /// <remarks>
        /// This method is set to private, and is only expected to be 
        /// called from our property changed handlers.
        /// </remarks>
        private static bool IsValidDoubleValue(object value)
        {
            Debug.Assert(typeof(double).IsInstanceOfType(value));
            double number = (double)value;
            return !double.IsNaN(number) && !double.IsInfinity(number);
        }

        /// <summary>
        /// Check if a value is a valid change for the two change properties.
        /// </summary> 
        /// <param name="value">Value.</param> 
        /// <returns>true if a valid value; false otherwise.</returns>
        private static bool IsValidChange(object value)
        {
            return IsValidDoubleValue(value) && (((double)value) >= 0);
        }

        /// <summary>
        /// Provides a string representation of a <see cref="RangeBase"/> object.
        /// </summary>
        /// <returns>
        /// Returns the string representation of a <see cref="RangeBase"/> object.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, FormatString, base.ToString(), Minimum, Maximum, Value);
        }

        internal bool GoToState(bool useTransitions, string stateName)
        {
            Debug.Assert(stateName != null);
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        /// <summary>
        /// Format string for RangeBase
        /// </summary> 
        private const string FormatString = "{0} Minimum:{1} Maximum:{2} Value:{3}";
    }
}
