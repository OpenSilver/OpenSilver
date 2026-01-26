
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
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Converts instances of <see cref="FontWeight"/> to and from other data types.
/// </summary>
public sealed class FontWeightConverter : TypeConverter
{
    /// <summary>
    /// Returns a value that indicates whether this converter can convert an object of the given type to an instance of <see cref="FontWeight"/>.
    /// </summary>
    /// <param name="context">
    /// Context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the converter can convert the provided type to an instance of <see cref="FontWeight"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of <see cref="FontWeight"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// Context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type that this instance of <see cref="FontWeight"/> is being evaluated for conversion to.
    /// </param>
    /// <returns>
    /// true if the converter can convert this instance of <see cref="FontWeight"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert a specified object to an instance of <see cref="FontWeight"/>.
    /// </summary>
    /// <param name="context">
    /// Context information of a type.
    /// </param>
    /// <param name="culture">
    /// <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The instance of <see cref="FontWeight"/> created from the converted value.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is null)
        {
            throw GetConvertFromException(value);
        }

        if (value is not string s)
        {
            throw new ArgumentException(string.Format(Strings.General_BadType, nameof(ConvertFrom)), nameof(value));
        }

        if (!FontWeights.FontWeightStringToKnownWeight(s, culture, out FontWeight fontWeight))
        {
            throw new FormatException(Strings.Parsers_IllegalToken);
        }

        return fontWeight;
    }

    /// <summary>
    /// Attempts to convert an instance of <see cref="FontWeight"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Describes the <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The instance of <see cref="FontWeight"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type this instance of <see cref="FontWeight"/> is converted to.
    /// </param>
    /// <returns>
    /// The object created from the converted instance of <see cref="FontWeight"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value is not an instance of <see cref="FontWeight"/> -or- destinationType is not a valid destination type.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is FontWeight c)
            {
                return ((IFormattable)c).ToString(null, culture);
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
