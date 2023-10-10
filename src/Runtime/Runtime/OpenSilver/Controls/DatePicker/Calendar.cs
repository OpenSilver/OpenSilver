
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
using CSHTML5;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Controls
{
    public partial class Calendar : INTERNAL_CalendarOrClockBase
    {
        bool isLoaded = false;
        public Calendar()
        {
            this.Loaded += DatePicker_Loaded;

            SelectedDates = new SelectedDatesCollection(this);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedDate" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="SelectedDate" /> dependency property.
        /// </returns>
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(Calendar), new PropertyMetadata(OnSelectedDateChanged));

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        /// <returns>
        /// The date currently selected. The default is null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given date is outside the range specified by <see cref="DisplayDateStart" /> and <see cref="DisplayDateEnd" />
        /// -or-The given date is in the <see cref="BlackoutDates" /> collection.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If set to anything other than null when <see cref="SelectionMode" /> is set to <see cref="CalendarSelectionMode.None" />.
        /// </exception>
        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        private static void OnSelectedDateChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            if (c.SelectedValue != c.SelectedDate)
            {
                c.SelectedValue = c.SelectedDate;
            }
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            DateTime defaultDate = SelectedValue == null ? DateTime.Today : SelectedValue.Value;

            isLoaded = true;

            // Get a reference to the HTML DOM representation of the control (must be in the Visual Tree):
            object div = INTERNAL_InnerDomElement;

            // Render the control: Calendar only
            _flatpickrInstance = Interop.ExecuteJavaScript(@"flatpickr($0, {
                inline: true, 
                dateFormat: ""YYYY-MM-DD HH:MM"",
                defaultDate: $1
                })", div, GetJsDate(defaultDate));

            // Register the JS events:
            Interop.ExecuteJavaScriptVoid(@"$0.config.onChange.push(function(args) {
                var date = args[0];
                if(date !== undefined)
                {
                    var month = date.getMonth() + 1;
                    var day = date.getDate();
                    var year = date.getFullYear();
                    $1(year, month, day);
                }
            });", flushQueue: false, _flatpickrInstance, (Action<object, object, object>)OnJavaScriptEvent_Change);

            Interop.ExecuteJavaScriptVoid(@"$0.config.onMonthChange.push(function(args) {
                $1();
            });", flushQueue: false, _flatpickrInstance, (Action)OnMonthChange);

            RefreshTodayHighlight(IsTodayHighlighted);

            // Enable click event
            Interop.ExecuteJavaScriptVoid(@"$0.calendarContainer.style.pointerEvents = 'auto'", flushQueue: false, _flatpickrInstance);
            // Hide the input area:
            Interop.ExecuteJavaScriptVoid(@"$0.style.display = 'none'", flushQueue: false, div);
        }

        void OnJavaScriptEvent_Change(object year, object month, object day)
        {
            // Reflect the value of "this.Selected" so that it is the same as the date that was picked in JS:

            int intYear = int.Parse(year.ToString());
            int intMonth = int.Parse(month.ToString());
            int intDay = int.Parse(day.ToString());

            var dateTime = new DateTime(intYear, intMonth, intDay);

            this.SelectedValue = (DateTime?)dateTime;
            this.SelectedDate = this.SelectedValue;
            RefreshTodayHighlight(IsTodayHighlighted);
        }

        private void OnMonthChange()
        {
            RefreshTodayHighlight(IsTodayHighlighted);
        }

        bool _isTodayHighlighted = true;
        /// <summary>
        /// Gets or sets a value indicating whether the current date is highlighted.
        /// </summary>
        public bool IsTodayHighlighted
        {
            get
            {
                return _isTodayHighlighted;
            }
            set
            {
                RefreshTodayHighlight(value);
                _isTodayHighlighted = value;
            }
        }

        /// <summary>
        /// This method hides or displays the highlighting around the date of today.
        /// </summary>
        /// <param name="isTodayHighlighted">True if there should be a circle highlighting the date of today in the calendar, False otherwise</param>
        void RefreshTodayHighlight(bool isTodayHighlighted)
        {
            if (isLoaded)
            {
                string borderColor = isTodayHighlighted ? "transparent" : "";
                Interop.ExecuteJavaScriptVoid(@"var todaySpan = $0.calendarContainer.querySelector('span.today');
if(todaySpan) todaySpan.style.borderColor = $1", flushQueue: false, _flatpickrInstance, borderColor);
            }
        }

        private DateTime? _displayDateStart;
        /// <summary>
        /// Gets or sets the first date to be displayed (enabled)
        /// </summary>
        public DateTime? DisplayDateStart
        {
            get { return _displayDateStart; }
            set { SetDisplayDateStart(value); _displayDateStart = value; }
        }

        private DateTime? _displayDateEnd;
        /// <summary>
        /// Gets or sets the last date to be displayed (enabled)
        /// </summary>
        public DateTime? DisplayDateEnd
        {
            get { return _displayDateEnd; }
            set { SetDisplayDateEnd(value); _displayDateEnd = value; }
        }

        private DateTime _displayDate;
        /// <summary>
        /// Gets or sets the date to display
        /// </summary>
        public DateTime DisplayDate
        {
            get { return _displayDate; }
            set { SetDisplayDate(value); _displayDate = value; }
        }

        void SetDisplayDateStart(DateTime? dateStart)
        {
            if (dateStart == null)
            {
                Interop.ExecuteJavaScriptVoid(@"$0.config.minDate = undefined", flushQueue: false, this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDateStart = (DateTime)dateStart;
                Interop.ExecuteJavaScriptVoid(@"$0.config.minDate = $1", flushQueue: false, this._flatpickrInstance, GetJsDate(nonNullDateStart));
            }
        }

        void SetDisplayDateEnd(DateTime? dateEnd)
        {
            if (dateEnd == null)
            {
                Interop.ExecuteJavaScriptVoid(@"$0.config.maxDate = undefined", flushQueue: false, this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDateEnd = (DateTime)dateEnd;
                Interop.ExecuteJavaScriptVoid(@"$0.config.maxDate = $1", flushQueue: false, this._flatpickrInstance, GetJsDate(nonNullDateEnd));
            }
        }

        void SetDisplayDate(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                Interop.ExecuteJavaScriptVoid(@"$0.config.maxDate = undefined", flushQueue: false, this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDate = (DateTime)dateTime;
                if ((DisplayDateStart != null && dateTime < DisplayDateStart) || (DisplayDateEnd != null && dateTime > DisplayDateEnd))
                {
                    throw new ArgumentOutOfRangeException("The given date is not in the range specified by System.Windows.Controls.Calendar.DisplayDateStart and System.Windows.Controls.Calendar.DisplayDateEnd");
                }
                Interop.ExecuteJavaScriptVoid(@"$0.jumpToDate($1)", flushQueue: false, this._flatpickrInstance, GetJsDate(nonNullDate));
            }
        }

        static object GetJsDate(DateTime dateTime)
        {
            using (var date = Interop.ExecuteJavaScript("new Date($0,$1,$2)", dateTime.Year, dateTime.Month - 1, dateTime.Day))
                return date;
        }
    }
}
