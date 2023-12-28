// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides data for the
    /// <see cref="E:System.Windows.Controls.Calendar.DisplayDateChanged" />
    /// event.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public class GlobalCalendarDateChangedEventArgs : RoutedEventArgs
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
        /// Initializes a new instance of the GlobalCalendarDateChangedEventArgs
        /// class.
        /// </summary>
        /// <param name="removedDate">
        /// The date that was previously displayed.
        /// </param>
        /// <param name="addedDate">The date to be newly displayed.</param>
        internal GlobalCalendarDateChangedEventArgs(DateTime? removedDate, DateTime? addedDate)
        {
            RemovedDate = removedDate;
            AddedDate = addedDate;
        }
    }
}