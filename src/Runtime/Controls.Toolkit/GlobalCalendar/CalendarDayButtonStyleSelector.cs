// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides a way to apply GlobalCalendarDayButton Styles based on custom
    /// logic. 
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public abstract class CalendarDayButtonStyleSelector
    {
        /// <summary>
        /// Initializes a new instance of the CalendarDayButtonStyleSelector
        /// class.
        /// </summary>
        protected CalendarDayButtonStyleSelector()
        {
        }

        /// <summary>
        /// When overridden in a derived class, returns a
        /// GlobalCalendarDayButton Style based on custom logic.
        /// </summary>
        /// <param name="day">The day to select a Style for.</param>
        /// <param name="container">The GlobalCalendarDayButton.</param>
        /// <returns>A Style for the GlobalCalendarDayButton.</returns>
        public abstract Style SelectStyle(DateTime day, GlobalCalendarDayButton container);
    }
}