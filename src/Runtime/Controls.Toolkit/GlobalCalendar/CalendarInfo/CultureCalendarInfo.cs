// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using CultureCalendar = System.Globalization.Calendar;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides globalized calendar operations based on a specific culture.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public partial class CultureCalendarInfo : CalendarInfo
    {
        /// <summary>
        /// Gets the culture used to provide the calendar operations.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Gets the DateTimeFormatInfo to use for formatting.
        /// </summary>
        public override DateTimeFormatInfo DateFormatInfo
        {
            get { return Culture.DateTimeFormat; }
        }
        
        /// <summary>
        /// Initializes a new instance of the CultureCalendarInfo class.
        /// </summary>
        public CultureCalendarInfo()
            : this(CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CultureCalendarInfo class.
        /// </summary>
        /// <param name="culture">
        /// The culture used to provide the calendar operations.
        /// </param>
        public CultureCalendarInfo(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            Culture = culture;
        }

        /// <summary>
        /// Returns a DateTime that is the specified number of days away from
        /// the specified DateTime.
        /// </summary>
        /// <param name="day">The DateTime to which to add days.</param>
        /// <param name="days">The number of days to add.</param>
        /// <returns>
        /// The DateTime that results from adding the specified number of days
        /// to the specified DateTime.
        /// </returns>
        public override DateTime? AddDays(DateTime day, int days)
        {
            try
            {
                return Culture.Calendar.AddDays(day, days);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a DateTime that is the specified number of months away from
        /// the specified DateTime.
        /// </summary>
        /// <param name="day">The DateTime to which to add months.</param>
        /// <param name="months">The number of months to add.</param>
        /// <returns>
        /// The DateTime that results from adding the specified number of months
        /// to the specified DateTime.
        /// </returns>
        public override DateTime? AddMonths(DateTime day, int months)
        {
            try
            {
                return Culture.Calendar.AddMonths(day, months);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a DateTime that is the specified number of years away from
        /// the specified DateTime.
        /// </summary>
        /// <param name="day">The DateTime to which to add years.</param>
        /// <param name="years">The number of years to add.</param>
        /// <returns>
        /// The DateTime that results from adding the specified number of years
        /// to the specified DateTime.
        /// </returns>
        public override DateTime? AddYears(DateTime day, int years)
        {
            try
            {
                return Culture.Calendar.AddYears(day, years);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}