using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.ComponentModel
{
    public class CharConverter : TypeConverter
    {
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object in the given source type to a Unicode character object using
        ///       the specified context.</para>
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
        ///      Converts the given object to another type.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is char)
            {
                if ((char)value == (char)0)
                {
                    return "";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <devdoc>
        ///    <para>Converts the given object to a Unicode character object.</para>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string text = (string)value;
                if (text.Length > 1)
                {
                    text = text.Trim();
                }

                if (text != null && text.Length > 0)
                {
                    if (text.Length != 1)
                    {
                        throw new FormatException(string.Format("{0} is not a valid value for {1}.", text, "Char"));
                    }
                    return text[0];
                }

                return '\0';
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
