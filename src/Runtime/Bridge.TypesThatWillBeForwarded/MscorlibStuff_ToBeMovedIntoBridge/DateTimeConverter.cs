﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.ComponentModel
{
    public class DateTimeConverter : TypeConverter
    {
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object in the given source type to a <see cref='System.DateTime'/>
        ///       object using the
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
        /// <para>Converts the given value object to a <see cref='System.DateTime'/>
        /// object.</para>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string text = ((string)value).Trim();
                if (text.Length == 0)
                {
                    return DateTime.MinValue;
                }
                try
                {
                    // See if we have a culture info to parse with.  If so, then use it.
                    //
                    DateTimeFormatInfo formatInfo = null;

                    if (culture != null)
                    {
                        formatInfo = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
                    }

                    if (formatInfo != null)
                    {
                        return DateTime.Parse(text, formatInfo);
                    }
                    else
                    {
                        return DateTime.Parse(text, culture);
                    }
                }
                catch (FormatException e)
                {
                    throw new FormatException(string.Format("{0} is not a valid value for {1}.", (string)value, "DateTime"), e);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <devdoc>
        /// <para>Converts the given value object to a <see cref='System.DateTime'/>
        /// object
        /// using the arguments.</para>
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is DateTime)
            {
                DateTime dt = (DateTime)value;
                if (dt == DateTime.MinValue)
                {
                    return string.Empty;
                }

                if (culture == null)
                {
                    culture = CultureInfo.CurrentCulture;
                }

                DateTimeFormatInfo formatInfo = null;
                formatInfo = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));

                string format;
                if (culture == CultureInfo.InvariantCulture)
                {
                    if (dt.TimeOfDay.TotalSeconds == 0)
                    {
                        return dt.ToString("yyyy-MM-dd", culture);
                    }
                    else
                    {
                        return dt.ToString(null, culture);
                    }
                }
                if (dt.TimeOfDay.TotalSeconds == 0)
                {
                    format = formatInfo.ShortDatePattern;
                }
                else
                {
                    format = formatInfo.ShortDatePattern + " " + formatInfo.ShortTimePattern;
                }

                return dt.ToString(format, CultureInfo.CurrentCulture);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
