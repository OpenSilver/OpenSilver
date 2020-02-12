
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
    public partial class Clock : INTERNAL_CalendarOrClockBase
    {
        public Clock()
        {
            this.Loaded += TimePicker_Loaded;
        }

        private void TimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime defaultDate = SelectedValue == null ? DateTime.Now : SelectedValue.Value;

            // Get a reference to the HTML DOM representation of the control (must be in the Visual Tree):
            object div = CSHTML5.Interop.GetDiv(this);

            // Render the control: clock only (defaultDate is required)
            _flatpickrInstance = CSHTML5.Interop.ExecuteJavaScript(@"flatpickr($0, {
                    inline: true,
                    enableTime: true,
                    noCalendar: true,
                    time_24hr: true,
                    dateFormat: ""YYYY-MM-DD HH:MM"",
                    defaultDate: new Date(1, 1, 1, $1, $2)
                    })", div, defaultDate.Hour, defaultDate.Minute - (defaultDate.Minute % 5));


            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
                // Register the JS events:
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.onChange.push(function(args) {
                var date = args[0];
                var hours = date.getHours();
                var minutes = date.getMinutes();
                $1(hours, minutes);
                });", _flatpickrInstance, (Action<object, object>)OnJavaScriptEvent_Change);
            }
            else
            {
                // In JS there is a bug that prevents us from using the exact same code as the Simulator. The bug has to do with the "this" keyword when used inside the callback from the JS interop: it returns the "onChange" array because the callback was added with "onChange.push".

                // Force capturing the "this" instance now because in JS "this" keyword has a different meaning when retrieved later:
                var clock = CSHTML5.Interop.ExecuteJavaScript(@"$0", this);

                // Register the JS events:
                CSHTML5.Interop.ExecuteJavaScript(@"$0.config.onChange.push(function(args) {
                var date = args[0];
                var hours = date.getHours();
                var minutes = date.getMinutes();
                $1($2, hours, minutes);
                });", _flatpickrInstance, (Action<object, object, object>)WorkaroundJSILBug, clock);
            }

            // Hide the input area:
            CSHTML5.Interop.ExecuteJavaScript(@"$0.style.display = 'none'", div);
        }

        static void WorkaroundJSILBug(object clockInstance, object hours, object minutes)
        {
            // cf. comment above.
            ((Clock)clockInstance).OnJavaScriptEvent_Change(hours, minutes);
        }

        void OnJavaScriptEvent_Change(object hours, object minutes)
        {
            // Reflect the value of "this.Selected" so that it is the same as the time that was picked in JS:

            DateTime newDate = new DateTime(2016, 10, 20, int.Parse(hours.ToString()), int.Parse(minutes.ToString()), 20);
            this.SelectedValue = (DateTime?)newDate;
        }
    }
}
