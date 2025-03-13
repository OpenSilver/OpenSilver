
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
/// Converts instances of <see cref="RepeatBehavior"/> to and from other data types.
/// </summary>
public sealed class RepeatBehaviorConverter : TypeConverter
{
    private static readonly char[] _iterationCharacter = ['x', 'X'];

    /// <summary>
    /// Determines whether or not conversion from a specified data type is possible.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="sourceType">
    /// Type to evaluate for conversion.
    /// </param>
    /// <returns>
    /// true if conversion is supported; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines if conversion to a specified type is possible.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="destinationType">
    /// Destination type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if conversion is possible; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Converts a given string value to an instance of <see cref="RepeatBehaviorConverter"/>.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information to respect during conversion.
    /// </param>
    /// <param name="value">
    /// Object being evaluated for conversion.
    /// </param>
    /// <returns>
    /// A new <see cref="RepeatBehavior"/> object based on value.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        string stringValue = value as string;

        if (stringValue != null)
        {
            stringValue = stringValue.Trim();

            if (string.Equals(stringValue, "Forever", StringComparison.OrdinalIgnoreCase))
            {
                return RepeatBehavior.Forever;
            }
            else if (stringValue.Length > 0 &&
                     char.ToLowerInvariant(stringValue[stringValue.Length - 1]) == _iterationCharacter[0])
            {
                string stringDoubleValue = stringValue.TrimEnd(_iterationCharacter);

                double doubleValue = (double)TypeConverterHelper.GetConverter(
                    typeof(double)).ConvertFrom(
                        context,
                        culture,
                        stringDoubleValue);

                return new RepeatBehavior(doubleValue);
            }
        }

        // The value is not Forever or an iteration count so it's either a TimeSpan
        // or we'll let the TimeSpanConverter raise the appropriate exception.

        TimeSpan timeSpanValue = (TimeSpan)TypeConverterHelper.GetConverter(
            typeof(TimeSpan)).ConvertFrom(
                context,
                culture,
                stringValue);

        return new RepeatBehavior(timeSpanValue);
    }

    /// <summary>
    /// Converts an instance of <see cref="RepeatBehavior"/> to a supported destination type.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information to respect during conversion.
    /// </param>
    /// <param name="value">
    /// Object being evaluated for conversion.
    /// </param>
    /// <param name="destinationType">
    /// Destination type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// The only supported destination type is <see cref="string"/>.
    /// </returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType is null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        if (destinationType == typeof(string))
        {
            if (value is RepeatBehavior repeatBehavior)
            {
                return repeatBehavior.ToString(culture);
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
