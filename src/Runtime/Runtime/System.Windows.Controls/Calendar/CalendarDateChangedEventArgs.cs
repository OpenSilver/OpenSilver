// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides data for the <see cref="Calendar.DisplayDateChanged" /> event.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class CalendarDateChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the date that was previously displayed.
        /// </summary>
        /// <value>
        /// The date previously displayed.
        /// </value>
        public DateTime? RemovedDate { get; private set; }

        /// <summary>
        /// Gets the date to be newly displayed.
        /// </summary>
        /// <value>The new date to display.</value>
        public DateTime? AddedDate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the CalendarDateChangedEventArgs
        /// class.
        /// </summary>
        /// <param name="removedDate">
        /// The date that was previously displayed.
        /// </param>
        /// <param name="addedDate">The date to be newly displayed.</param>
        internal CalendarDateChangedEventArgs(DateTime? removedDate, DateTime? addedDate)
        {
            RemovedDate = removedDate;
            AddedDate = addedDate;
        }
    }
}