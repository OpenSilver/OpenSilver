
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

namespace System.Windows.Media.Animation;

/// <summary>
/// Converts instances of <see cref="KeyTime"/> to and from other types.
/// </summary>
public class KeyTimeConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a given type to an instance of a <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="context">
    /// Contextual information required for conversion.
    /// </param>
    /// <param name="sourceType">
    /// Type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this type can be converted; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines if a given type can be converted to an instance of <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="context">
    /// Contextual information required for conversion.
    /// </param>
    /// <param name="destinationType">
    /// Type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this type can be converted; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert a given object to an instance of <see cref="KeyTime"/>.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information that is respected during conversion.
    /// </param>
    /// <param name="value">
    /// The object being converted to an instance of <see cref="KeyTime"/>.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="KeyTime"/>, based on the supplied value.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            stringValue = stringValue.Trim();

            if (stringValue == "Uniform")
            {
                return KeyTime.Uniform;
            }
            else if (stringValue == "Paced")
            {
                throw new NotSupportedException(
                    $"The '{typeof(KeyTime)}.{stringValue}' property is not supported yet.");
            }
            else if (stringValue.Length > 0 &&
                     stringValue[stringValue.Length - 1] == '%')
            {
                throw new NotSupportedException(
                    $"Percentage values for '{typeof(KeyTime)}' are not supported yet.");
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
    /// Attempts to convert an instance of <see cref="KeyTime"/> to another type.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information that is respected during conversion.
    /// </param>
    /// <param name="value">
    /// <see cref="KeyTime"/> value to convert from.
    /// </param>
    /// <param name="destinationType">
    /// Type being evaluated for conversion.
    /// </param>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is KeyTime keyTime)
            {
                switch (keyTime.Type)
                {
                    case KeyTimeType.Uniform:
                        return "Uniform";

                    case KeyTimeType.TimeSpan:
                        return TypeConverterHelper.GetConverter(typeof(TimeSpan))
                            .ConvertTo(context, culture, keyTime.TimeSpan, destinationType);
                };
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
