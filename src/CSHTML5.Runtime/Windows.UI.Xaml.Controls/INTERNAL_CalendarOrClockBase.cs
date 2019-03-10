
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
    public abstract class INTERNAL_CalendarOrClockBase : FrameworkElement
    {
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
            new PropertyMetadata(OnSelectedValueChanged));

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
                    if(calendarOrClock._flatpickrInstance != null) 
                    {
                        // Convert from C# DateTime to JS Date:
                        var newDateJS = Interop.ExecuteJavaScript("new Date($0, $1, $2, $3, $4)", newDate.Year, newDate.Month - 1, newDate.Day, newDate.Hour, newDate.Minute);

                        // Set the current date in the JS instance of the calendar:
                        Interop.ExecuteJavaScript("$0.setDate($1)", calendarOrClock._flatpickrInstance, newDateJS);
                    }
                }
                else
                {
                    //todo: implement this
                }
            }
        }

        /// <summary>
        /// Occurs when the collection returned by the property has changed.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
        {
            EventHandler<SelectionChangedEventArgs> handler = SelectionChanged;

            if (null != handler)
            {
                handler(this, e);
            }
        }

        #endregion


    }
}
