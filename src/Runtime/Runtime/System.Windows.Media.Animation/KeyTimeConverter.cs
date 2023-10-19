
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

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class KeyTimeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether or not this class can convert from a given type
        /// to an instance of a KeyTime.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Returns whether or not this class can convert from an instance of a
        /// KeyTime to a given type.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// 
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                stringValue = stringValue.Trim();

                if (stringValue == "Uniform" || stringValue == "Paced")
                {
                    throw new NotSupportedException(
                        $"The '{typeof(KeyTime)}.{stringValue}' property is not supported yet."
                    );
                }
                else if (stringValue.Length > 0 &&
                         stringValue[stringValue.Length - 1] == '%')
                {
                    throw new NotSupportedException(
                        $"Percentage values for '{typeof(KeyTime)}' are not supported yet."
                    );
                }
                else
                {
                    TimeSpan timeSpanValue = (TimeSpan)TypeConverterHelper.GetConverter(
                        typeof(TimeSpan)).ConvertFrom(
                            context,
                            culture,
                            stringValue);

                    return KeyTime.FromTimeSpan(timeSpanValue);
                }
            }

            throw GetConvertFromException(value);
        }

        /// <summary>
        /// 
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string))
            {
                if (value is KeyTime keyTime)
                {
                    return TypeConverterHelper.GetConverter(
                        typeof(TimeSpan)).ConvertTo(
                            context,
                            culture,
                            keyTime.TimeSpan,
                            destinationType);
                }
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}
