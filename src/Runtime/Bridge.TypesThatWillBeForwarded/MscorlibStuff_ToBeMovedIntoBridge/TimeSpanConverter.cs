using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.ComponentModel
{
    public class TimeSpanConverter : TypeConverter
    {
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object in the given source type to a <see cref='System.TimeSpan'/> object using the
        ///       specified context.</para>
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        /// <devdoc>
        /// <para>Converts the given object to a <see cref='System.TimeSpan'/>
        /// object.</para>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string text = ((string)value).Trim();
                try
                {
                    return TimeSpanParse(text);
                }
                catch (FormatException e)
                {
                    throw new FormatException(string.Format("{0} is not a valid value for {1}.", (string)value, "TimeSpan"), e);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <devdoc>
        ///      Converts the given object to another type.  The most common types to convert
        ///      are to and from a string object.  The default implementation will make a call
        ///      to ToString on the object if the object is valid and if the destination
        ///      type is string.  If this cannot convert to the desitnation type, this will
        ///      throw a NotSupportedException.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static TimeSpan TimeSpanParse(string timeSpanAsString, bool canBeNegative = true)
        {
            //-----------------------------------------------------
            // Note: initially Bridge.NET did not support TimeSpan.Parse at all, so we created the workaround below.
            // Then Bridge.NET added implementation but only for the default representation: https://github.com/bridgedotnet/Bridge/pull/3544
            // So there are still issues with the built-in Bridge.NET implementation. For example, the following code fails: TimeSpan ts = TimeSpan.Parse("0");
            // To reproduce: in the app of the client linked to CSHTML5 ZenDesk ticket #828: Go to "Children" -> "Child Profile" window. An exception appears because TimeSpan.Parse("0") fails.
            // Therefore we still use the code below for now.
            //-----------------------------------------------------

            TimeSpan timeSpan;
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            int fractionalSeconds = 0;
            //string's format:
            //[ws][-]{ d | [d.]hh:mm[:ss[.ff]] }[ws]
            //we get rid of the white spaces at the beginning and at the end:
            string timeAsString = timeSpanAsString.Trim();
            int signKeeper = 1;
            if (timeAsString[0] == '-')
            {
                signKeeper = -1;
                timeAsString = timeAsString.Substring(1);
            }
            string[] splittedTime = timeAsString.Split(':');
            // We can't parse timeSpanAsString to a TimeSpan in this case
            if (splittedTime.Length > 3)
            {
                throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\".");
            }
            else if (splittedTime.Length == 1)
            {
                days = int.Parse(splittedTime[0]);
                if (days < 0)
                {
                    throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                }
            }
            else
            {
                for (int i = 0; i < splittedTime.Length; i++)
                {
                    if (i == 0)
                    {
                        // we check if the number of days is specified
                        string[] daysAndHours = splittedTime[0].Split('.');
                        if (daysAndHours.Length == 1) // number of days is not specified, so it is 0.
                        {
                            hours = int.Parse(daysAndHours[0]);
                        }
                        else
                        {
                            days = int.Parse(daysAndHours[0]);
                            if (days < 0)
                            {
                                throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                            }
                            hours = int.Parse(daysAndHours[1]);
                        }
                        if (hours < 0 || hours > 23)
                        {
                            throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                        }
                    }
                    else if (i == 1) // In this case we try to get the minutes, so we just avec to parse splittedTime[i] to an int.
                    {
                        minutes = int.Parse(splittedTime[i]);
                        if (minutes < 0 || minutes > 59)
                        {
                            throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                        }
                    }
                    else if (i == 2) // Here we want to get the seconds and milliseconds if specified.
                    {
                        string[] secondsAndFractionalSeconds = splittedTime[i].Split('.');
                        if (secondsAndFractionalSeconds.Length == 1)
                        {
                            seconds = int.Parse(secondsAndFractionalSeconds[0]);
                        }
                        else
                        {
                            seconds = int.Parse((secondsAndFractionalSeconds[0] == string.Empty ? "0" : secondsAndFractionalSeconds[0])); // we need to check this because we can have to parse a string with the following format : "00:00:.5".
                            fractionalSeconds = int.Parse(secondsAndFractionalSeconds[1]);
                            if (fractionalSeconds < 0 || fractionalSeconds > 9999999)
                            {
                                throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                            }
                        }
                        if (seconds < 0 || seconds > 59)
                        {
                            throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                        }
                    }
                }
            }
            //we're done parsing, we can create the TimeSpan:
            long ticks = days * 24; //24 hours a day
            ticks += hours;
            ticks *= 60; //60 minutes an hour
            ticks += minutes;
            ticks *= 60; //60 seconds a minute
            ticks += seconds;
            ticks *= 10000000; // 10 000 000 ticks per second
            ticks += fractionalSeconds;

            if (signKeeper == -1)
            {
                if (canBeNegative)
                {
                    timeSpan = new TimeSpan(ticks * signKeeper);
                }
                else
                {
                    timeSpan = new TimeSpan();
                }
            }
            else
            {
                timeSpan = new TimeSpan(ticks);
            }
            return timeSpan;
        }
    }
}
