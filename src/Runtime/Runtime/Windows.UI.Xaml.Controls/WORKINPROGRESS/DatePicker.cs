

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

        private static void OnDisplayDateStartChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
#endif