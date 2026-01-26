
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

namespace System.Windows.Media;

/// <summary>
/// Converts instances of the <see cref="string"/> type to and from <see cref="FontFamily"/> instances.
/// </summary>
public class FontFamilyConverter : TypeConverter
{
    /// <summary>
    /// Determines whether a class can be converted from a given type to an instance of <see cref="FontFamily"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the converter can convert from the specified type to an instance of <see cref="FontFamily"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of <see cref="FontFamily"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type that this instance of <see cref="FontFamily"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the converter can convert this instance of <see cref="FontFamily"/> to the specified type; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert a specified object to an instance of <see cref="FontFamily"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Cultural-specific information that should be respected during conversion.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The instance of <see cref="FontFamily"/> that is created from the converted o parameter.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            stringValue = stringValue.Trim();

            return new FontFamily(stringValue);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a specified object to an instance of <see cref="FontFamily"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Cultural-specific information that should be respected during conversion.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <param name="destinationType">
    /// The type that this instance of <see cref="FontFamily"/> is converted to.
    /// </param>
    /// <returns>
    /// The object that is created from the converted instance of <see cref="FontFamily"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Occurs if value or destinationType is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Occurs if value or destinationType is not a valid type for conversion.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not FontFamily fontFamily)
        {
            throw new ArgumentException(string.Format(Strings.General_Expected_Type, nameof(FontFamily)), nameof(value));
        }

        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (fontFamily.Source != null)
            {
                // Usual case: it's a named font family.
                return fontFamily.Source;
            }
            else
            {
                return string.Empty;
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
