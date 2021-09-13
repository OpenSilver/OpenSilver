

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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that enables a user to select a date by using a visual calendar display.
    /// </summary>
    public partial class Calendar
    {
        /// <summary>
        /// Gets or sets a collection of dates that are marked as not selectable.
        /// </summary>
        /// <returns>
        /// A collection of dates that cannot be selected. The default value is an empty collection.
        /// </returns>
        [OpenSilver.NotImplemented]
        public CalendarBlackoutDatesCollection BlackoutDates { get; private set; }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.Calendar.SelectedDate" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.Calendar.SelectedDate" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(Calendar), new PropertyMetadata(OnSelectedDateChanged));

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        /// <returns>
        /// The date currently selected. The default is null.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The given date is outside the range specified by <see cref="P:System.Windows.Controls.Calendar.DisplayDateStart" /> and <see cref="P:System.Windows.Controls.Calendar.DisplayDateEnd" />
        /// -or-The given date is in the <see cref="P:System.Windows.Controls.Calendar.BlackoutDates" /> collection.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// If set to anything other than null when <see cref="P:System.Windows.Controls.Calendar.SelectionMode" /> is set to <see cref="F:System.Windows.Controls.CalendarSelectionMode.None" />.
        /// </exception>
        [OpenSilver.NotImplemented]
        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        private static void OnSelectedDateChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.Calendar.DisplayDate" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.Calendar.DisplayDate" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof(DisplayDate), typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.Today, OnDisplayDateChanged));

        private static void OnDisplayDateChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        public Style CalendarDayButtonStyle { get; set; }
    }
}
