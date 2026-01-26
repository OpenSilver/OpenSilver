
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
/// Converts instances of other types to and from an instance of <see cref="Color"/>.
/// </summary>
public sealed class ColorConverter : TypeConverter
{
    /// <summary>
    /// Attempts to convert a string to a <see cref="Color"/>.
    /// </summary>
    /// <param name="value">
    /// The string to convert to a <see cref="Color"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Color"/> that represents the converted text.
    /// </returns>
    /// <exception cref="FormatException">
    /// value does not correspond to the string representation of a color.
    /// </exception>
    public static new object ConvertFromString(string value)
    {
        if (value is null)
        {
            return null;
        }

        return Parsers.ParseColor(value, null);
    }

    /// <summary>
    /// Determines whether an object can be converted from a given type to an instance of a <see cref="Color"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="Color"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of a <see cref="Color"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="Color"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this <see cref="Color"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="Color"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Cultural information to respect during conversion.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="Color"/> created from converting value.
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

        return Parsers.ParseColor(s, culture);
    }

    /// <summary>
    /// Attempts to convert a <see cref="Color"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Describes the <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The <see cref="Color"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="Color"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="Color"/>.
    /// </returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Color color)
            {
                return color.ToString(culture);
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
