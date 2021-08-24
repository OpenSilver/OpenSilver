// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a type converter to convert <see cref='System.TimeSpan'/> objects to and from
    /// various other representations.
    /// </summary>
    public class TimeSpanConverter : TypeConverter
    {
        /// <summary>
        /// Gets a value indicating whether this converter can
        /// convert an object in the given source type to a <see cref='System.TimeSpan'/> object using the
        /// specified context.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given
        /// destination type using the context.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to a <see cref='System.TimeSpan'/>
        /// object.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                text = text.Trim();
                try
                {
                    return FromString(text);
                }
                catch (FormatException e)
                {
                    throw new FormatException(string.Format("{0} is not a valid value for {1}.", (string)value, nameof(TimeSpan)), e);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given object to another type. The most common types to convert
        /// are to and from a string object. The default implementation will make a call
        /// to ToString on the object if the object is valid and if the destination
        /// type is string. If this cannot convert to the destination type, this will
        /// throw a NotSupportedException.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        internal static TimeSpan FromString(string source)
        {
            const string error_format = "Could not create TimeSpan from '{0}'. At least one of the numeric components is out of range or contains too many digits.";

            string stringValue = source.Trim();
            
            int d = 0, h = 0, m = 0, s = 0, ms = 0;
            
            string[] split = stringValue.Split(':');

            // We can't parse the value to a TimeSpan in this case
            if (split.Length > 3)
            {
                throw new FormatException($"Could not create TimeSpan from '{source}'.");
            }
            else if (split.Length == 1)
            {
                d = int.Parse(split[0]);
                if (d < 0)
                {
                    throw new FormatException(string.Format(error_format, source));
                }
            }
            else
            {
                for (int i = 0; i < split.Length; i++)
                {
                    if (i == 0)
                    {
                        // we check if the number of days is specified
                        string[] dd_hh = split[0].Split('.');
                        if (dd_hh.Length == 1) 
                        {
                            // number of days is not specified, so it is 0.
                            h = int.Parse(dd_hh[0]);
                        }
                        else
                        {
                            d = int.Parse(dd_hh[0]);
                            if (d < 0)
                            {
                                throw new FormatException(string.Format(error_format, source));
                            }
                            h = int.Parse(dd_hh[1]);
                        }
                        if (h < 0 || h > 23)
                        {
                            throw new FormatException(string.Format(error_format, source));
                        }
                    }
                    else if (i == 1) 
                    {
                        // In this case we try to get the minutes, so we just avec to parse
                        // split[i] to an int.

                        m = int.Parse(split[i]);
                        if (m < 0 || m > 59)
                        {
                            throw new FormatException(string.Format(error_format, source));
                        }
                    }
                    else if (i == 2) 
                    {
                        // Here we want to get the seconds and milliseconds if specified.

                        string[] s_ms = split[i].Split('.');
                        if (s_ms.Length == 1)
                        {
                            s = int.Parse(s_ms[0]);
                        }
                        else
                        {
                            // we need to check this because we can have to parse a string
                            // with the following format : "00:00:.5".
                            s = int.Parse((s_ms[0] == string.Empty ? "0" : s_ms[0]));

                            ms = int.Parse(s_ms[1]);
                            if (ms < 0 || ms > 9999999)
                            {
                                throw new FormatException(string.Format(error_format, source));
                            }
                        }
                        if (s < 0 || s > 59)
                        {
                            throw new FormatException(string.Format(error_format, source));
                        }
                    }
                }
            }

            long ticks = d * 24; // 24 hours a day
            ticks += h;
            ticks *= 60; // 60 minutes an hour
            ticks += m;
            ticks *= 60; // 60 seconds a minute
            ticks += s;
            ticks *= 10000000; // 10000000 ticks per second
            ticks += ms;
            
            return new TimeSpan(ticks);
        }
    }
}