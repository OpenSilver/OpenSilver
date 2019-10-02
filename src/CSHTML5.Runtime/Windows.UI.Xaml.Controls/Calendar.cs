
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



using Bridge;
using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public class Calendar : INTERNAL_CalendarOrClockBase
    {
        bool isLoaded = false;
        public Calendar()
        {
            this.Loaded += DatePicker_Loaded;
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime defaultDate = SelectedValue == null ? DateTime.Today : SelectedValue.Value;

            isLoaded = true;

            // Get a reference to the HTML DOM representation of the control (must be in the Visual Tree):
            object div = Interop.GetDiv(this);

            // Render the control: Calendar only
            _flatpickrInstance = Interop.ExecuteJavaScript(@"flatpickr($0, {
                inline: true, 
                dateFormat: ""YYYY-MM-DD HH:MM"",
                defaultDate: $1
                })", div, GetJsDate(defaultDate));

            // Register the JS events:
            if (Interop.IsRunningInTheSimulator)
            {
                Interop.ExecuteJavaScript(@"$0.config.onChange.push(function(args) {
                    var date = args[0];
                    if(date !== undefined)
                    {
                        var month = date.getMonth() + 1;
                        var day = date.getDate();
                        var year = date.getFullYear();
                        $1(year, month, day);
                    }
                });", _flatpickrInstance, (Action<object, object, object>)OnJavaScriptEvent_Change);

                Interop.ExecuteJavaScript(@"$0.config.onMonthChange.push(function(args) {
                    $1();
                });", _flatpickrInstance, (Action)OnMonthChange);


            }
            else
            {
                // In JS there is a bug that prevents us from using the exact same code as the Simulator. The bug has to do with the "this" keyword when used inside the callback from the JS interop: it returns the "onChange" array because the callback was added with "onChange.push".

                // Force capturing the "this" instance now because in JS "this" keyword has a different meaning when retrieved later:
                var calendar = Interop.ExecuteJavaScript(@"$0", this);

                // Register the JS events:
                Interop.ExecuteJavaScript(@"$0.config.onChange.push(function(args) {
                    var date = args[0];
                    if(date !== undefined)
                    {
                        var month = date.getMonth() + 1;
                        var day = date.getDate();
                        var year = date.getFullYear();
                        $1($2, year, month, day);
                    }
                });", _flatpickrInstance, (Action<object, object, object, object>)WorkaroundJSILBug, calendar);

                Interop.ExecuteJavaScript(@"$0.config.onMonthChange.push(function(args) {
                    $1($2);
                });", _flatpickrInstance, (Action<object>)WorkaroundJSILBugForOnMonthChange, calendar);
            }

            RefreshTodayHighlight(IsTodayHighlighted);

            // Hide the input area:
            Interop.ExecuteJavaScript(@"$0.style.display = 'none'", div);
        }

        static void WorkaroundJSILBug(object calendarInstance, object year, object month, object day)
        {
            // cf. comment above.
            ((Calendar)calendarInstance).OnJavaScriptEvent_Change(year, month, day);
        }

        void OnJavaScriptEvent_Change(object year, object month, object day)
        {
            // Reflect the value of "this.Selected" so that it is the same as the date that was picked in JS:

            int intYear = int.Parse(year.ToString());
            int intMonth = int.Parse(month.ToString());
            int intDay = int.Parse(day.ToString());

            var dateTime = new DateTime(intYear, intMonth, intDay);

            this.SelectedValue = (DateTime?)dateTime;
            RefreshTodayHighlight(IsTodayHighlighted);
        }

        static void WorkaroundJSILBugForOnMonthChange(object calendarInstance)
        {
            // We're doing like in WorkaroundJSILBug for the "onChange" event
            ((Calendar)calendarInstance).OnMonthChange();
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
                string c = "transparent";
                if (isTodayHighlighted)
                {
                    c = "";
                }
                var todaySpan = Interop.ExecuteJavaScript(@"$0.calendarContainer.querySelector('span.today')", this._flatpickrInstance);
                if (todaySpan != null)
                {
                    Interop.ExecuteJavaScript(@"$0.style.borderColor = $1", todaySpan, c);
                }
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

        private DateTime? _displayDate;
        /// <summary>
        /// Gets or sets the date to display
        /// </summary>
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The given date is not in the range specified by System.Windows.Controls.Calendar.DisplayDateStart
        //     and System.Windows.Controls.Calendar.DisplayDateEnd.
        public DateTime? DisplayDate
        {
            get { return _displayDate; }
            set { SetDisplayDate(value); _displayDate = value; }
        }

        void SetDisplayDateStart(DateTime? dateStart)
        {
            if (dateStart == null)
            {
                Interop.ExecuteJavaScript(@"$0.config.minDate = undefined", this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDateStart = (DateTime)dateStart;
                Interop.ExecuteJavaScript(@"$0.config.minDate = $1", this._flatpickrInstance, GetJsDate(nonNullDateStart));
            }
        }

        void SetDisplayDateEnd(DateTime? dateEnd)
        {
            if (dateEnd == null)
            {
                Interop.ExecuteJavaScript(@"$0.config.maxDate = undefined", this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDateEnd = (DateTime)dateEnd;
                Interop.ExecuteJavaScript(@"$0.config.maxDate = $1", this._flatpickrInstance, GetJsDate(nonNullDateEnd));
            }
        }

        void SetDisplayDate(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                Interop.ExecuteJavaScript(@"$0.config.maxDate = undefined", this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDate = (DateTime)dateTime;
                if ((DisplayDateStart != null && dateTime < DisplayDateStart) || (DisplayDateEnd != null && dateTime > DisplayDateEnd))
                {
                    throw new ArgumentOutOfRangeException("The given date is not in the range specified by System.Windows.Controls.Calendar.DisplayDateStart and System.Windows.Controls.Calendar.DisplayDateEnd");
                }
                Interop.ExecuteJavaScript(@"$0.jumpToDate($1)", this._flatpickrInstance, GetJsDate(nonNullDate));
            }
        }

        //todo: could be nice to have the same as the Bridge.Template for JSIL
#if BRIDGE
        [Template("new Date(System.DateTime.getYear({dateTime}), ((System.DateTime.getMonth({dateTime}) - 1) | 0), System.DateTime.getDay({dateTime}))")]
#endif
        static object GetJsDate(DateTime dateTime)
        {
            var date = Interop.ExecuteJavaScript("new Date($0,$1,$2)", dateTime.Year, dateTime.Month - 1, dateTime.Day);
            return date;
        }

    }
}
