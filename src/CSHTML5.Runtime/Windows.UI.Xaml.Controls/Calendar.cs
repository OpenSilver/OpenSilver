
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
        public Calendar()
        {
            this.Loaded += DatePicker_Loaded;
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime defaultDate = SelectedValue == null ? DateTime.Today : SelectedValue.Value;

            // Get a reference to the HTML DOM representation of the control (must be in the Visual Tree):
            object div = Interop.GetDiv(this);

            // Render the control: Calendar only
            _flatpickrInstance = Interop.ExecuteJavaScript(@"flatpickr($0, {
                inline: true, 
                dateFormat: ""YYYY-MM-DD HH:MM"",
                defaultDate: new Date($1, $2, $3)
                })", div, defaultDate.Year, defaultDate.Month - 1, defaultDate.Day);

            // Register the JS events:
            if (Interop.IsRunningInTheSimulator)
            {
                Interop.ExecuteJavaScript(@"$0.config.onChange.push(function(args) {
                    var date = args[0];
                    var month = date.getMonth() + 1;
                    var day = date.getDate();
                    var year = date.getFullYear();
                    $1(year, month, day);
                });", _flatpickrInstance, (Action<object, object, object>)OnJavaScriptEvent_Change);
            }
            else
            {
                // In JS there is a bug that prevents us from using the exact same code as the Simulator. The bug has to do with the "this" keyword when used inside the callback from the JS interop: it returns the "onChange" array because the callback was added with "onChange.push".

                // Force capturing the "this" instance now because in JS "this" keyword has a different meaning when retrieved later:
                var calendar = Interop.ExecuteJavaScript(@"$0", this);

                // Register the JS events:
                Interop.ExecuteJavaScript(@"$0.config.onChange.push(function(args) {
                    var date = args[0];
                    var month = date.getMonth() + 1;
                    var day = date.getDate();
                    var year = date.getFullYear();
                    $1($2, year, month, day);
                });", _flatpickrInstance, (Action<object, object, object, object>)WorkaroundJSILBug, calendar);
            }

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
        }

    }
}
