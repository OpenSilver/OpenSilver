

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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;

#endif

using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public abstract partial class INTERNAL_CalendarOrClockBase : FrameworkElement
    {
        //Note: if we want to allow customization of the day cells: see event "onDayCreate" at https://flatpickr.js.org/events/

        protected object _flatpickrInstance;

        public INTERNAL_CalendarOrClockBase()
        {
            SelectedValue = null;
        }

        #region SelectedValue

        /// <summary>
        /// Gets or sets the currently selected date or time.
        /// </summary>
        public DateTime? SelectedValue
        {
            get { return (DateTime?)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
            "SelectedValue",
            typeof(DateTime?),
            typeof(INTERNAL_CalendarOrClockBase),
            new PropertyMetadata(OnSelectedValueChanged)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// SelectedValueProperty property changed handler.
        /// </summary>
        /// <param name="d">the calender or clock that changed its SelectedDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue) // safety against dependency loop
            {
                var calendarOrClock = (INTERNAL_CalendarOrClockBase)d;
                if (e.NewValue != null)
                {
                    DateTime newDate = (DateTime)e.NewValue;

                    var newSelectedDate = new SelectedDatesCollection(calendarOrClock);
                    newSelectedDate.SetDate(newDate); // calls OnSelectedDatesCollectionChanged

                    // when we set the default date of the picker before it exists, we don't try to set the date in JS
                    if (calendarOrClock._flatpickrInstance != null)
                    {
                        // Convert from C# DateTime to JS Date:
                        var newDateJS = CSHTML5.Interop.ExecuteJavaScript("new Date($0, $1, $2, $3, $4)", newDate.Year, newDate.Month - 1, newDate.Day, newDate.Hour, newDate.Minute);

                        // Set the current date in the JS instance of the calendar:
                        CSHTML5.Interop.ExecuteJavaScript("$0.setDate($1)", calendarOrClock._flatpickrInstance, newDateJS);
                    }
                }
                else
                {
                    if (calendarOrClock._flatpickrInstance != null)
                    {
                        CSHTML5.Interop.ExecuteJavaScript("$0.setDate(undefined)", calendarOrClock._flatpickrInstance);
                    }
                }
            }
        }

        [Obsolete("Use the SelectedDatesChanged event instead.")]
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        /// <summary>
        /// Occurs when the collection returned by the property has changed.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDatesChanged;

        internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
            if (SelectedDatesChanged != null)
            {
                SelectedDatesChanged(this, e);
            }
        }

        #endregion


    }
}
