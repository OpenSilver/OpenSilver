using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies values that describe the available selection modes for a <see cref="Calendar" />.
    /// </summary>
    public enum CalendarSelectionMode
    {
        /// <summary>
        /// Only a single date can be selected. Use the <see cref="Calendar.SelectedDate" /> property to retrieve the selected date.
        /// </summary>		
        SingleDate,
        /// <summary>
        /// A single range of dates can be selected. Use the <see cref="Calendar.SelectedDates" /> property to retrieve the selected dates.
        /// </summary>
        SingleRange,
        /// <summary>
        /// Multiple non-contiguous ranges of dates can be selected. Use the <see cref="Calendar.SelectedDates" /> property to retrieve the selected dates.
        /// </summary>
        MultipleRange,
        /// <summary>
        /// No selections are allowed.
        /// </summary>
        None
    }
}
