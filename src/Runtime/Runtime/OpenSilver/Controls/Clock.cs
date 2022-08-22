

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


using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Controls
{
    public partial class Clock : INTERNAL_CalendarOrClockBase
    {
        public Clock()
        {
            this.Loaded += TimePicker_Loaded;
        }

        public static DependencyProperty MinuteIntervalProperty = DependencyProperty.Register(
            "MinuteInterval",
            typeof(int),
            typeof(Clock),
            new PropertyMetadata(5, OnMinuteIntervalChanged ));

        public int MinuteInterval
        {
            get => (int)GetValue(MinuteIntervalProperty);
            set => SetValue(MinuteIntervalProperty, value);
        }

        public void ChangeOption(string option, object value)
        {
            if (_flatpickrInstance != null)
            {
                CSHTML5.Interop.ExecuteJavaScript("$0.set($1, $2)", _flatpickrInstance, option, value);
            }
        }

        private void TimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

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
                    minuteIncrement: $3,
                    defaultDate: new Date(1, 1, 1, $1, $2)
                    })", div, defaultDate.Hour, defaultDate.Minute - (defaultDate.Minute % MinuteInterval), MinuteInterval);


#if OPENSILVER
            if (true)
#elif BRIDGE
            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
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

        private static void OnMinuteIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != e.NewValue)
            {
                var clock = d as Clock;
                clock.ChangeOption("minuteIncrement", e.NewValue);
            }
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
