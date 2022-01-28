

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


#if BRIDGE
using Bridge;
#endif
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
    public partial class Calendar : INTERNAL_CalendarOrClockBase
    {
        bool isLoaded = false;
        public Calendar()
        {
            this.Loaded += DatePicker_Loaded;
        }
        
        
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        [OpenSilver.NotImplemented]
        /// <summary>
        /// Identifies the <see cref="Control.FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty;


        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime defaultDate = SelectedValue == null ? DateTime.Today : SelectedValue.Value;

            isLoaded = true;

            // Get a reference to the HTML DOM representation of the control (must be in the Visual Tree):
            object div = CSHTML5.Interop.GetDiv(this);

            // Render the control: Calendar only
            _flatpickrInstance = CSHTML5.Interop.ExecuteJavaScript(@"flatpickr($0, {
                inline: true, 
                dateFormat: ""YYYY-MM-DD HH:MM"",
                defaultDate: $1
                })", div, GetJsDate(defaultDate));

            // Register the JS events:
#if OPENSILVER
            if (true)
#elif BRIDGE
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.onChange.push(function(args) {
                    var date = args[0];
                    if(date !== undefined)
                    {
                        var month = date.getMonth() + 1;
                        var day = date.getDate();
                        var year = date.getFullYear();
                        $1(year, month, day);
                    }
                });", _flatpickrInstance, (Action<object, object, object>)OnJavaScriptEvent_Change);

                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.onMonthChange.push(function(args) {
                    $1();
                });", _flatpickrInstance, (Action)OnMonthChange);


            }
            else
            {
                // In JS there is a bug that prevents us from using the exact same code as the Simulator. The bug has to do with the "this" keyword when used inside the callback from the JS interop: it returns the "onChange" array because the callback was added with "onChange.push".

                // Force capturing the "this" instance now because in JS "this" keyword has a different meaning when retrieved later:
                var calendar = CSHTML5.Interop.ExecuteJavaScript(@"$0", this);

                // Register the JS events:
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.onChange.push(function(args) {
                    var date = args[0];
                    if(date !== undefined)
                    {
                        var month = date.getMonth() + 1;
                        var day = date.getDate();
                        var year = date.getFullYear();
                        $1($2, year, month, day);
                    }
                });", _flatpickrInstance, (Action<object, object, object, object>)WorkaroundJSILBug, calendar);

                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.onMonthChange.push(function(args) {
                    $1($2);
                });", _flatpickrInstance, (Action<object>)WorkaroundJSILBugForOnMonthChange, calendar);
            }

            RefreshTodayHighlight(IsTodayHighlighted);

            // Enable click event
            CSHTML5.Interop.ExecuteJavaScript(@"$0.calendarContainer.style.pointerEvents = 'auto'", _flatpickrInstance);
            // Hide the input area:
            CSHTML5.Interop.ExecuteJavaScript(@"$0.style.display = 'none'", div);
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
                string borderColor = isTodayHighlighted ? "transparent" : "";
                CSHTML5.Interop.ExecuteJavaScript(@"var todaySpan = $0.calendarContainer.querySelector('span.today'); 
                            if(todaySpan) todaySpan.style.borderColor = $1", _flatpickrInstance, borderColor);
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
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The given date is not in the range specified by System.Windows.Controls.Calendar.DisplayDateStart
        //     and System.Windows.Controls.Calendar.DisplayDateEnd.
        public DateTime DisplayDate
        {
            get { return _displayDate; }
            set { SetDisplayDate(value); _displayDate = value; }
        }

        void SetDisplayDateStart(DateTime? dateStart)
        {
            if (dateStart == null)
            {
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.minDate = undefined", this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDateStart = (DateTime)dateStart;
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.minDate = $1", this._flatpickrInstance, GetJsDate(nonNullDateStart));
            }
        }

        void SetDisplayDateEnd(DateTime? dateEnd)
        {
            if (dateEnd == null)
            {
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.maxDate = undefined", this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDateEnd = (DateTime)dateEnd;
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.maxDate = $1", this._flatpickrInstance, GetJsDate(nonNullDateEnd));
            }
        }

        void SetDisplayDate(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.maxDate = undefined", this._flatpickrInstance);
            }
            else
            {
                DateTime nonNullDate = (DateTime)dateTime;
                if ((DisplayDateStart != null && dateTime < DisplayDateStart) || (DisplayDateEnd != null && dateTime > DisplayDateEnd))
                {
                    throw new ArgumentOutOfRangeException("The given date is not in the range specified by System.Windows.Controls.Calendar.DisplayDateStart and System.Windows.Controls.Calendar.DisplayDateEnd");
                }
                CSHTML5.Interop.ExecuteJavaScript(@"$0.jumpToDate($1)", this._flatpickrInstance, GetJsDate(nonNullDate));
            }
        }

        //todo: could be nice to have the same as the Bridge.Template for JSIL
#if BRIDGE
        [Template("new Date(System.DateTime.getYear({dateTime}), ((System.DateTime.getMonth({dateTime}) - 1) | 0), System.DateTime.getDay({dateTime}))")]
#endif
        static object GetJsDate(DateTime dateTime)
        {
            var date = CSHTML5.Interop.ExecuteJavaScript("new Date($0,$1,$2)", dateTime.Year, dateTime.Month - 1, dateTime.Day);
            return date;
        }

    }
}
