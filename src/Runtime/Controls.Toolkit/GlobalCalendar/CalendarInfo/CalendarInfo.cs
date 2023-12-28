// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using CultureCalendar = System.Globalization.Calendar;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides globalized calendar operations.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public abstract partial class CalendarInfo
    {
        /// <summary>
        /// Gets the DateTimeFormatInfo to use for formatting.
        /// </summary>
        public virtual DateTimeFormatInfo DateFormatInfo
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat; }
        }

        /// <summary>
        /// Gets the number of days in a week.
        /// </summary>
        public virtual int DaysInWeek
        {
            get { return 7; }
        }

        /// <summary>
        /// Gets the first day of the week.
        /// </summary>
        public virtual DayOfWeek FirstDayOfWeek
        {
            get { return EnsureDateFormatInfo().FirstDayOfWeek; }
        }

        /// <summary>
        /// Initializes a new instance of the CalendarInfo class.
        /// </summary>
        protected CalendarInfo()
        {
        }

        /// <summary>
        /// Get the DateFormatInfo and ensure it's not null.
        /// </summary>
        /// <returns>The DateFormatInfo.</returns>
        private DateTimeFormatInfo EnsureDateFormatInfo()
        {
            DateTimeFormatInfo info = DateFormatInfo;
            if (info == null)
            {
                throw new InvalidOperationException(Resource.CalendarInfo_DateFormatInfoRequired);
            }

            return info;
        }

        /// <summary>
        /// Compares two instances of DateTime and returns an integer that
        /// indicates whether the first instance is earlier than, the same as,
        /// or later than the second instance.
        /// </summary>
        /// <param name="first">The first DateTime.</param>
        /// <param name="second">The second DateTime.</param>
        /// <returns>
        /// Less than zero indicates that first is less than second, zero
        /// indicates that first equals second, and greater than zero indicated
        /// that second is greater than first.
        /// </returns>
        public virtual int Compare(DateTime first, DateTime second)
        {
            return DateTime.Compare(first, second);
        }

        /// <summary>
        /// Compares the days of two instances of DateTime and returns an
        /// integer that indicates whether the first instance is earlier than,
        /// the same as, or later than the second instance.
        /// </summary>
        /// <param name="first">The first DateTime.</param>
        /// <param name="second">The second DateTime.</param>
        /// <returns>
        /// Less than zero indicates that first is less than second, zero
        /// indicates that first equals second, and greater than zero indicated
        /// that second is greater than first.
        /// </returns>
        public virtual int CompareDays(DateTime first, DateTime second)
        {
            return Compare(first.Date, second.Date);
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
        public virtual DateTime? AddDays(DateTime day, int days)
        {
            try
            {
                return day.AddDays(days);
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
        public virtual DateTime? AddMonths(DateTime day, int months)
        {
            try
            {
                return day.AddMonths(months);
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
        public virtual DateTime? AddYears(DateTime day, int years)
        {
            try
            {
                return day.AddYears(years);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the number of months between two dates.
        /// </summary>
        /// <param name="first">The first date.</param>
        /// <param name="second">The second date.</param>
        /// <returns>The number of months between the two dates.</returns>
        public virtual int GetMonthDifference(DateTime first, DateTime second)
        {
            return 12 * (first.Year - second.Year) + (first.Month - second.Month);
        }

        /// <summary>
        /// Get the start of the DateTime's decade.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>The start of the DateTime's decade.</returns>
        public virtual int GetDecadeStart(DateTime day)
        {
            return day.Year - (day.Year % 10);
        }

        /// <summary>
        /// Get the end of the DateTime's decade.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>The end of the DateTime's decade.</returns>
        public virtual int GetDecadeEnd(DateTime day)
        {
            return GetDecadeStart(day) + 9;
        }

        /// <summary>
        /// Get the day of the week of a DateTime.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>The day of the week of a DateTime.</returns>
        public virtual DayOfWeek GetDayOfWeek(DateTime day)
        {
            return day.DayOfWeek;
        }

        /// <summary>
        /// Get the first day in the year of a DateTime.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>The first day in the year of a DateTime.</returns>
        public virtual DateTime GetFirstDayInYear(DateTime day)
        {
            return new DateTime(day.Year, 1, 1);
        }

        /// <summary>
        /// Get the first day in the month of a DateTime.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>The first day in the month of a DateTime.</returns>
        public virtual DateTime GetFirstDayInMonth(DateTime day)
        {
            return new DateTime(day.Year, day.Month, 1);
        }

        /// <summary>
        /// Get the shortest day name for a given day of week.
        /// </summary>
        /// <param name="dayOfWeekIndex">Index of the day of week.</param>
        /// <returns>The shortest day name for a given day of week.</returns>
        public virtual string GetShortestDayName(int dayOfWeekIndex)
        {
            return EnsureDateFormatInfo().ShortestDayNames[dayOfWeekIndex];
        }

        /// <summary>
        /// Get the abbreviated month name for a given month.
        /// </summary>
        /// <param name="monthIndex">Index of the month.</param>
        /// <returns>The abbreviated month name for a given month.</returns>
        public virtual string GetAbbreviatedMonthName(int monthIndex)
        {
            return EnsureDateFormatInfo().AbbreviatedMonthNames[monthIndex];
        }

        /// <summary>
        /// Convert the day of a DateTime to a string.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>String representation of the day of a DateTime.</returns>
        public virtual string DayToString(DateTime day)
        {
            return day.ToString("%d", EnsureDateFormatInfo());
        }

        /// <summary>
        /// Convert the year and month of a DateTime to a string.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>
        /// String representation of the year and month of a DateTime.
        /// </returns>
        public virtual string MonthAndYearToString(DateTime day)
        {
            return day.ToString("Y", EnsureDateFormatInfo());
        }

        /// <summary>
        /// Convert the year of a DateTime to a string.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>String representation of the year of a DateTime.</returns>
        public virtual string YearToString(DateTime day)
        {
            return day.ToString("yyyy", EnsureDateFormatInfo());
        }

        /// <summary>
        /// Convert a decade range to a string.
        /// </summary>
        /// <param name="decadeStart">The start of the decade.</param>
        /// <param name="decadeEnd">The end of the decade.</param>
        /// <returns>String representation of the decade range.</returns>
        public virtual string DecadeToString(int decadeStart, int decadeEnd)
        {
            // In the event that the decades are outside of DateTime.MinValue
            // and DateTime.MaxValue, we'll handle the ArgumentException and
            // just display the numbers as ints
            try
            {
                return string.Format(
                    EnsureDateFormatInfo(),
                    "{0:yyyy}-{1:yyyy}",
                    new DateTime(decadeStart, 12, 31),
                    new DateTime(decadeEnd, 1, 1));
            }
            catch (ArgumentException)
            {
                return string.Format(
                    EnsureDateFormatInfo(),
                    "{0}-{1}",
                    decadeStart,
                    decadeEnd);
            }
        }

        /// <summary>
        /// Convert a DateTime to a string.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>String representation of the date.</returns>
        public virtual string DateToString(DateTime day)
        {
            return day.ToString(EnsureDateFormatInfo());
        }

        /// <summary>
        /// Convert a DateTime to a long format string.
        /// </summary>
        /// <param name="day">The DateTime.</param>
        /// <returns>Long format string representation of the date.</returns>
        public virtual string DateToLongString(DateTime day)
        {
            DateTimeFormatInfo format = EnsureDateFormatInfo();
            return day.Date.ToString(format.LongDatePattern, format);
        }
    }
}