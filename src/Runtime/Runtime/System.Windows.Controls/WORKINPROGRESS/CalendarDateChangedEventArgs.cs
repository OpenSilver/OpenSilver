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
    public class CalendarDateChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Getsor sets the date that was previously displayed.
        /// </summary>
        /// <returns>
        /// The date previously displayed. 
        /// </returns>		
        public DateTime? RemovedDate { get; private set; }

        /// <summary>
        /// Gets or sets the date to be newly displayed.
        /// </summary>
        /// <returns>
        /// The new date to display.
        /// </returns>		
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
            this.RemovedDate = removedDate;
            this.AddedDate = addedDate;
        }
    }
}
