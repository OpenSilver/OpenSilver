

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


#define NO_SUPPORT_FOR_MILLISECONDS


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class INTERNAL_DateTimeHelpers
{
    public static string FromDateTime(DateTime value)
    {
        // The following is not used because it always produces the UTC date format. Although the resulting date is the same, the fact that it is UTC means that if the client sends a date to the server in UTC, the "DateTime.Kind" of the received date on the server will be UTC, and if the client asks the date back from the server, this leads to a different result when doing "DateTime.ToString", because "DateTime.ToString" does not take the time zone into account.
        /*
        DateTime dateTimeUtc = (value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value);
        TimeSpan timeSince1970 = (dateTimeUtc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        double millisecondsSince1970 = timeSince1970.TotalMilliseconds;
        var jsDate = JSIL.Verbatim.Expression("new Date($0)", millisecondsSince1970);
        string serializedDate = JSIL.Verbatim.Expression("$0.toJSON()", jsDate);
        return serializedDate;
        */

        TimeSpan ts = value - value.Date;
        ts = ts - new TimeSpan(value.Hour, value.Minute, value.Second);
        string tsString = ts.Ticks > 1000000 ? ts.Ticks.ToString() : "0" + ts.Ticks;
        if (tsString.EndsWith("0000"))
        {
            tsString = tsString.Substring(0, tsString.Length - 4);
        }
        string secondString = "" + value.Second;
        if (secondString.Length == 1)
            secondString = 0 + secondString;
        string minuteString = "" + value.Minute;
        if (minuteString.Length == 1)
            minuteString = 0 + minuteString;
        string hourString = "" + value.Hour;
        if (hourString.Length == 1)
            hourString = 0 + hourString;
        string dayString = "" + value.Day;
        if (dayString.Length == 1)
            dayString = 0 + dayString;
        string monthString = "" + value.Month;
        if (monthString.Length == 1)
            monthString = 0 + monthString;
        string yearString = "" + value.Year;
        while (yearString.Length < 4)
            yearString = 0 + yearString;


        string str = yearString + "-" + monthString + "-" + dayString + "T" + hourString + ":" + minuteString + ":" + secondString;
        if (ts.Ticks != 0)
        {
            str += "." + tsString;
        }

        //we add the Timezone if the DateTimeKind is Local
        if (value.Kind == DateTimeKind.Local)
        {
            TimeSpan changeFromUTC = value - value.ToUniversalTime();
            string minuteStringForTimeZone = "" + changeFromUTC.Minutes;
            if (minuteStringForTimeZone.Length == 1)
                minuteStringForTimeZone = 0 + minuteStringForTimeZone;
            string hourStringForTimeZone = "" + Math.Abs(changeFromUTC.Hours); //getting the absolute value because it allows us to easily add the 0 in case of a 1 digit hour (so that we get 07 instead of 7 and the '-' doesn't get in the way in case of a negative timezone).
            if (hourStringForTimeZone.Length == 1)
                hourStringForTimeZone = 0 + hourStringForTimeZone;
            string timeZone = hourStringForTimeZone + ":" + minuteStringForTimeZone;
            if (changeFromUTC.Ticks >= 0)
            {
                timeZone = "+" + timeZone;
            }
            else
            {
                timeZone = "-" + timeZone;
            }
            str += timeZone;

        }
        else if (value.Kind == DateTimeKind.Utc)
            str += "Z";
        return str;
    }

    public static DateTime ToDateTime(string dateTimeAsString)
    {
        // The following is not used because it does not allow to populate the "DateTime.Kind" property, which is important because "DateTime.ToString" produces a different result (ie. if the same date is in UTC or in Local timezone, "ToString" does not display the same thing, it displays the date without taking the timezone into account).
        /*
        var jsDate = JSIL.Verbatim.Expression("new Date($0)", dateTimeAsString);

        int year = Convert.ToInt32(JSIL.Verbatim.Expression("$0.getFullYear()", jsDate));
        int month = Convert.ToInt32(JSIL.Verbatim.Expression("$0.getMonth()", jsDate)) + 1; // Note: month index in JS starts at "0".
        int day = Convert.ToInt32(JSIL.Verbatim.Expression("$0.getDate()", jsDate));
        int hour = Convert.ToInt32(JSIL.Verbatim.Expression("$0.getHours()", jsDate));
        int minute = Convert.ToInt32(JSIL.Verbatim.Expression("$0.getMinutes()", jsDate));
        int second = Convert.ToInt32(JSIL.Verbatim.Expression("$0.getSeconds()", jsDate));
        int millisecond = Convert.ToInt32(JSIL.Verbatim.Expression("$0.getMilliseconds()", jsDate));

        DateTime dateTime = new DateTime(year, month, day, hour, minute, second, millisecond);

        return dateTime;
        */

        // it looks like that:<DateTime>2015-02-23T16:54:17.273+01:00</DateTime> (only the part inside the tags)
        int year, month, day, hour, minute, second, timezoneHour, timezoneMinute;
        DateTime dt;
        DateTimeKind kind = DateTimeKind.Unspecified;
        string[] split = dateTimeAsString.Split('T');
        string datePart = split[0];
        string[] splittedDate = datePart.Split('-');
        year = int.Parse(splittedDate[0]);
        month = int.Parse(splittedDate[1]);
        day = int.Parse(splittedDate[2]);

        string hourPart = split[1];
        char[] timezoneSplitters = { '+', '-' };
        string[] hourSplittedFromTimeZone = hourPart.Split(timezoneSplitters);
        char[] splitters = { ':', '.' };
        string[] splittedHour = hourSplittedFromTimeZone[0].Split(splitters);
        hour = int.Parse(splittedHour[0]);
        minute = int.Parse(splittedHour[1]);
        string secondString = splittedHour[2];
        if (secondString.EndsWith("Z"))
        {
            kind = DateTimeKind.Utc;
            secondString = secondString.Substring(0, secondString.Length - 1);
        }
        second = int.Parse(secondString);

#if NO_SUPPORT_FOR_MILLISECONDS
            if (hourSplittedFromTimeZone.Length >= 2)
#else
        if (hourSplittedFromTimeZone.Length == 2)
#endif
        {
            char sign;
            if (hourPart.Contains("+"))
                sign = '+';
            else
                sign = '-';
            string[] splittedTimezone = hourSplittedFromTimeZone[1].Split(':');
            timezoneHour = int.Parse(sign + splittedTimezone[0]);
            timezoneMinute = int.Parse(splittedTimezone[1]);
            //we convert into the UTC hour:
            hour -= timezoneHour;
            minute -= timezoneMinute;
            DateTime datetimeForTimeZoneOffset = DateTime.Now;
            TimeSpan ts = datetimeForTimeZoneOffset - datetimeForTimeZoneOffset.ToUniversalTime();
            hour += ts.Hours;
            minute += ts.Minutes;
            kind = DateTimeKind.Local;
        }
#if !NO_SUPPORT_FOR_MILLISECONDS
        if (splittedHour.Length == 4)
        {
            string millisecondsString = splittedHour[3];
            if (millisecondsString.EndsWith("Z"))
            {
                kind = DateTimeKind.Utc;
                millisecondsString = millisecondsString.Substring(0, millisecondsString.Length - 1);
            }

            tickOrMillisecond = int.Parse(millisecondsString);

            if (tickOrMillisecond > 1000) //it's in ticks, so we have to use the DateTime(long ticks) constructor.
            {
                long ticks = ymdToTicks(year, month, day);
                ticks += hmsToTicks(hour, minute, second);
                ticks += tickOrMillisecond;
                dt = new DateTime(ticks, kind); //todo: datetimekind and timezone
            }
            else //tickOrMillisecond is in Milliseconds so we can simply use DateTime(int year, ..., int millisecond); 
            {
                dt = new DateTime(year, month, day, hour, minute, second, tickOrMillisecond, kind);
            }
        }
        else
        {
#endif
            //the datetime stops at the second
            dt = new DateTime(year, month, day, hour, minute, second, kind);
#if !NO_SUPPORT_FOR_MILLISECONDS
        }
#endif
        return dt;
    }
}
