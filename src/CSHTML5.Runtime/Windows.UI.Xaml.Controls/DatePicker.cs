
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    public class DatePicker : INTERNAL_DateTimePickerBase
    {   
        public DatePicker()
        {
            _defaultText = ""; // the text displayed when no date is selected

            // Set default style:
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(DatePicker);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultDatePickerStyle.GetDefaultStyle());
#endif
        }

        protected override INTERNAL_CalendarOrClockBase GenerateCalendarOrClock()
        {
            return new Calendar();
        }

        protected override string SetTextFromDate(string newDate)
        {
            string[] split = newDate.Split(' ');
            string[] date = split[0].Split('/');
            return date[0] + "-" + date[1] + "-" + date[2];
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
            new PropertyMetadata(OnSelectedDateChanged));

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
