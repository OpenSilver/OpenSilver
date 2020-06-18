

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


#if MIGRATION
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class TimePicker : INTERNAL_DateTimePickerBase
    {   
        public TimePicker()
        {
            _defaultText = ""; // the text displayed when no time is selected

            // Set default style:
            this.DefaultStyleKey = typeof(TimePicker);
        }

        protected override INTERNAL_CalendarOrClockBase GenerateCalendarOrClock()
        {
            return new Clock();
        }

        protected override string SetTextFromDate(string newDate)
        {
            string[] split = newDate.Split(' ');
            string[] time = split[1].Split(':');
            return time[0] + " : " + time[1];
        }

        protected override void OnSelectionChanged(DateTime? newSelectedDate)
        {
            if (newSelectedDate.HasValue && Value.HasValue)
            {
                if (Value != newSelectedDate) // safety to avoid depandency loop
                    Value = newSelectedDate;
            }
            else if (newSelectedDate.HasValue && !Value.HasValue) // if the previous value is null, but not the new one 
            {
                Value = newSelectedDate;
            }
            else // if the new value is null
            {
                if (Value != null) // safety to avoid dependency loop
                    Value = null;               
            }

        }

#region dependency property Selection for TimePicker

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set 
            { 
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
            "Value",
            typeof(DateTime?),
            typeof(TimePicker),
            new PropertyMetadata(OnValueChanged)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">TemporalPicker that changed its SelectedDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePicker)d).INTERNAL_SelectedDate = (DateTime?)e.NewValue;
        }

#endregion

    }
}
