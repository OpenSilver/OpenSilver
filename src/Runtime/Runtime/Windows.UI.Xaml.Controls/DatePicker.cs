

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
    public partial class DatePicker : INTERNAL_DateTimePickerBase
    {   
        public DatePicker()
        {
            _defaultText = ""; // the text displayed when no date is selected

            // Set default style:
            this.DefaultStyleKey = typeof(DatePicker);
        }

        private static DateTime? ParseText(string text)
        {
            if (DateTime.TryParse(text, out var dt))
            {
                return dt;
            }

            return null;
        }

        /// <summary>
        /// Updates selected date according to current Text property value
        /// </summary>
        private void SetSelectedDate()
        {
            var text = GetValue(TextProperty)?.ToString();
            if (text != null)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    if (SelectedDate.HasValue &&
                        SetTextFromDate(SelectedDate.Value.ToString()) == text)
                    {
                        return;
                    }

                    var dt = ParseText(text);
                    if (SelectedDate.Equals(dt))
                    {
                        return;
                    }

                    SetCurrentValue(SelectedDateProperty, dt);
                }
                else
                {
                    if (!SelectedDate.HasValue)
                    {
                        return;
                    }

                    //Behavior of Silverlight component
                    SetCurrentValue(SelectedDateProperty, new DateTime());
                }
            }
            else
            {
                var dt = ParseText(_defaultText);
                if (SelectedDate.Equals(dt))
                {
                    return;
                }

                SetCurrentValue(SelectedDateProperty, dt);
            }
        }

        protected override void OnTextChanged()
        {
            SetSelectedDate();
        }

        protected override INTERNAL_CalendarOrClockBase GenerateCalendarOrClock()
        {
            return new Calendar();
        }

        protected override string SetTextFromDate(string newDate)
        {
            var split = newDate.Split(' ');
            return split[0];
        }

        protected override void OnSelectionChanged(DateTime? newSelectedDate)
        {
            if (newSelectedDate.HasValue && SelectedDate.HasValue)
            {
                if (SelectedDate != newSelectedDate) // safety to avoid dependency loop
                    SelectedDate = newSelectedDate;
            }
            else if (newSelectedDate.HasValue && !SelectedDate.HasValue) // if the previous value is null, but not the new one 
            {
                SelectedDate = newSelectedDate;
            }
            else // if the new value is null
            {
                if (SelectedDate != null) // safety to avoid dependency loop
                    SelectedDate = null;
            }

            IsDropDownOpen = false; // close the popup
        }

#region dependency property Selection for DatePicker

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set 
            { 
                SetValue(SelectedDateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
            "SelectedDate",
            typeof(DateTime?),
            typeof(DatePicker),
            new PropertyMetadata(OnSelectedDateChanged)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">TemporalPicker that changed its SelectedDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DatePicker)d).INTERNAL_SelectedDate = (DateTime?)e.NewValue;
        }

#endregion


    }
}
