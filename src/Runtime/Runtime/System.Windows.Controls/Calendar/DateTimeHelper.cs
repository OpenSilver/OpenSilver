// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Inherited code: Requires comment.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    internal static class DateTimeHelper
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="time">Inherited code: Requires comment 1.</param>
        /// <param name="days">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static DateTime? AddDays(DateTime time, int days)
        {
            var cal = new GregorianCalendar();
            try
            {
                return cal.AddDays(time, days);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="time">Inherited code: Requires comment 1.</param>
        /// <param name="months">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static DateTime? AddMonths(DateTime time, int months)
        {
            var cal = new GregorianCalendar();
            try
            {
                return cal.AddMonths(time, months);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="time">Inherited code: Requires comment 1.</param>
        /// <param name="years">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static DateTime? AddYears(DateTime time, int years)
        {
            var cal = new GregorianCalendar();
            try
            {
                return cal.AddYears(time, years);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="dt1">Inherited code: Requires comment 1.</param>
        /// <param name="dt2">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static int CompareDays(DateTime dt1, DateTime dt2)
        {
            return DateTime.Compare(DiscardTime(dt1).Value, DiscardTime(dt2).Value);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="dt1">Inherited code: Requires comment 1.</param>
        /// <param name="dt2">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static int CompareYearMonth(DateTime dt1, DateTime dt2)
        {
            return (dt1.Year - dt2.Year) * 12 + (dt1.Month - dt2.Month);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="date">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        public static int DecadeOfDate(DateTime date)
        {
            return date.Year - (date.Year % 10);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        public static DateTime DiscardDayTime(DateTime d)
        {
            int year = d.Year;
            int month = d.Month;
            DateTime newD = new DateTime(year, month, 1, 0, 0, 0);
            return newD;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        public static DateTime? DiscardTime(DateTime? d)
        {
            if (d == null)
            {
                return null;
            }
            return d.Value.Date;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="date">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        public static int EndOfDecade(DateTime date)
        {
            return DecadeOfDate(date) + 9;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <returns>Inherited code: Requires comment 1.</returns>
        public static DateTimeFormatInfo GetCurrentDateFormat()
        {
            if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat;
            }
            else
            {
                foreach (var cal in CultureInfo.CurrentCulture.OptionalCalendars)
                {
                    if (cal is GregorianCalendar)
                    {
                        // if the default calendar is not Gregorian, return the
                        // first supported GregorianCalendar dtfi
                        DateTimeFormatInfo dtfi = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
                        dtfi.Calendar = cal;
                        return dtfi;
                    }
                }

                // if there are no GregorianCalendars in the OptionalCalendars
                // list, use the invariant dtfi
                DateTimeFormatInfo dt = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
                dt.Calendar = new GregorianCalendar();
                return dt;
            }
        }

        /// <summary>
        /// Returns if the date is included in the range.
        /// </summary>
        /// <param name="date">Inherited code: Requires comment 1.</param>
        /// <param name="range">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static bool InRange(DateTime date, CalendarDateRange range)
        {
            Debug.Assert(DateTime.Compare(range.Start, range.End) < 1, "The range should start before it ends!");

            if (CompareDays(date, range.Start) > -1 && CompareDays(date, range.End) < 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a localized string for the specified date using the YearMonthPattern format.
        /// </summary>
        /// <param name="date">Date to convert.</param>
        /// <returns>Localized string.</returns>
        public static string ToYearMonthPatternString(DateTime date)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetCurrentDateFormat();

            if (format != null)
            {
                result = date.ToString(format.YearMonthPattern, format);
            }

            return result;
        }

        /// <summary>
        /// Gets a localized string for the specified date's year.
        /// </summary>
        /// <param name="date">Date to convert.</param>
        /// <returns>Localized string.</returns>
        public static string ToYearString(DateTime date)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetCurrentDateFormat();

            if (format != null)
            {
                result = date.Year.ToString(format);
            }

            return result;
        }
    }
}