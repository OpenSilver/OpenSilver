

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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endif

namespace OpenSilver.Controls
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
            return new Clock() { MinuteInterval = PopupMinutesInterval };
        }

        protected override string SetTextFromDate(DateTime? newDate)
        {
            if (newDate == null)
                return null;

            return newDate.Value.ToString("HH:mm");
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

        #region public int PopupMinutesInterval
        /// <summary>
        /// Gets or sets the minutes interval between time values allowed by the TimePickerPopup.
        /// </summary>
        public int PopupMinutesInterval
        {
            get => (int)GetValue(PopupMinutesIntervalProperty);
            set => SetValue(PopupMinutesIntervalProperty, value);
        }

        /// <summary>
        /// Identifies the PopupMinutesInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupMinutesIntervalProperty =
            DependencyProperty.Register(
                "PopupMinutesInterval",
                typeof(int),
                typeof(TimePicker),
                new PropertyMetadata(5, OnPopupMinutesIntervalPropertyChanged));

        /// <summary>
        /// PopupMinutesIntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// TimePicker that changed its PopupMinutesInterval.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private static void OnPopupMinutesIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var timePicker = d as TimePicker;
                ((Clock)timePicker._calendarOrClock).MinuteInterval = (int)e.NewValue;
            }
        }

        #endregion
        #endregion

    }
}
