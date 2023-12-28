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
    /// Provides globalized calendar operations based on the GregorianCalendar.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public partial class GregorianCalendarInfo : CalendarInfo
    {
        /// <summary>
        /// The Calendar that provides our date operations.
        /// </summary>
        private GregorianCalendar _calendar;

        /// <summary>
        /// The DateTimeFormatInfo to use for formatting.
        /// </summary>
        private DateTimeFormatInfo _formatInfo;

        /// <summary>
        /// Gets the DateTimeFormatInfo to use for formatting.
        /// </summary>
        public override DateTimeFormatInfo DateFormatInfo
        {
            get { return _formatInfo; }
        }

        /// <summary>
        /// Initializes a new instance of the GregorianCalendarInfo class.
        /// </summary>
        /// <remarks>
        /// Uses the most appropriate GregorianCalendar for
        /// CultureInfo.CurrentCulture.
        /// </remarks>
        public GregorianCalendarInfo()
            : this(CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GregorianCalendarInfo class.
        /// </summary>
        /// <param name="culture">
        /// The culture used to provide the GregorianCalendar and date
        /// formatting information.
        /// </param>
        public GregorianCalendarInfo(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            // Try the culture's default calendar
            _calendar = culture.Calendar as GregorianCalendar;
            if (_calendar != null)
            {
                _formatInfo = culture.DateTimeFormat;
            }
            else
            {
                // If the default calendar is not Gregorian, use the first
                // supported GregorianCalendar
                foreach (CultureCalendar calendar in culture.OptionalCalendars)
                {
                    _calendar = calendar as GregorianCalendar;
                    if (_calendar != null)
                    {
                        _formatInfo = new CultureInfo(culture.Name).DateTimeFormat;
                        _formatInfo.Calendar = calendar;
                    }
                }

                // If there are no supported Gregorian calendars among the
                // OptionalCalendars, create one using the InvariantCulture.
                if (_calendar == null)
                {
                    _calendar = new GregorianCalendar();
                    _formatInfo = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
                    _formatInfo.Calendar = new GregorianCalendar();
                }
            }
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
                return _calendar.AddDays(day, days);
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
                return _calendar.AddMonths(day, months);
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
                return _calendar.AddYears(day, years);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}