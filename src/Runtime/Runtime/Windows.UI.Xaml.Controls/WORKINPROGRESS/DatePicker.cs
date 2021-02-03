﻿

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


#if WORKINPROGRESS

using System;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that allows the user to select a date.
    /// </summary>
    public partial class DatePicker
    {
        /// <summary>
        /// Gets or sets a collection of dates that are marked as not selectable.
        /// </summary>
        /// <returns>
        /// A collection of dates that cannot be selected. The default value is an empty collection.
        /// </returns>
        public CalendarBlackoutDatesCollection BlackoutDates { get; private set; }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DatePicker.SelectionBackground" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DatePicker.SelectionBackground" /> dependency property.
        /// </returns>
        public static readonly DependencyProperty SelectionBackgroundProperty = DependencyProperty.Register(nameof(SelectionBackground), typeof(Brush), typeof(DatePicker), null);

        /// <summary>
        /// Gets or sets the background used for selected dates.
        /// </summary>
        /// <returns>
        /// The background used for selected dates.
        /// </returns>
        public Brush SelectionBackground
        {
            get => (Brush)GetValue(SelectionBackgroundProperty);
            set => SetValue(SelectionBackgroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.DatePicker.DisplayDateStart" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.DatePicker.DisplayDateStart" /> dependency property.
        /// </returns>
        public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(nameof(DisplayDateStart), typeof(DateTime?), typeof(DatePicker), new PropertyMetadata(OnDisplayDateStartChanged));

        /// <summary>
        /// Gets or sets the first date to be displayed.
        /// </summary>
        /// <returns>
        /// The first date to display.
        /// </returns>
        public DateTime? DisplayDateStart
        {
            get => (DateTime?)GetValue(DisplayDateStartProperty);
            set => SetValue(DisplayDateStartProperty, value);
        }

        public DatePickerFormat SelectedDateFormat { get; set; }

        //
        // Summary:
        //     Occurs when the drop-down System.Windows.Controls.Calendar is closed.
        public event RoutedEventHandler CalendarClosed;
        

        private static void OnDisplayDateStartChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Occurs when the drop-down
        /// <see cref="T:System.Windows.Controls.Calendar" /> is opened.
        /// </summary>
        public event RoutedEventHandler CalendarOpened;

        #region DisplayDate
        /// <summary>
        /// Gets or sets the date to display.
        /// </summary>
        /// <value>
        /// The date to display. The default 
        /// <see cref="DateTime.Today" />.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified date is not in the range defined by
        /// <see cref="DatePicker.DisplayDateStart" />
        /// and
        /// <see cref="DatePicker.DisplayDateEnd" />.
        /// </exception>
        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DatePicker.DisplayDate" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="DatePicker.DisplayDate" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register(
            "DisplayDate",
            typeof(DateTime),
            typeof(DatePicker),
            new PropertyMetadata(OnDisplayDateChanged));

        /// <summary>
        /// DisplayDateProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its DisplayDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion DisplayDate

        #region DisplayDateEnd
        /// <summary>
        /// Gets or sets the last date to be displayed.
        /// </summary>
        /// <value>The last date to display.</value>
        public DateTime? DisplayDateEnd
        {
            get { return (DateTime?)GetValue(DisplayDateEndProperty); }
            set { SetValue(DisplayDateEndProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DatePicker.DisplayDateEnd" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="DatePicker.DisplayDateEnd" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(
            "DisplayDateEnd",
            typeof(DateTime?),
            typeof(DatePicker),
            new PropertyMetadata(OnDisplayDateEndChanged));

        /// <summary>
        /// DisplayDateEndProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its DisplayDateEnd.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion DisplayDateEnd
    }
}
#endif