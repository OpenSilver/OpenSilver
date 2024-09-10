
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

namespace System.Windows;

/// <summary>
/// Converts instances of other types to and from a <see cref="Vector"/>.
/// </summary>
public sealed class VectorConverter : TypeConverter
{
    /// <summary>
    /// Indicates whether an object can be converted from a given type to an instance of a <see cref="Vector"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The source <see cref="Type"/> that is being queried for conversion support.
    /// </param>
    /// <returns>
    /// true if objects of the specified type can be converted to a <see cref="Vector"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether instances of <see cref="Vector"/> can be converted to the specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="Vector"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if instances of <see cref="Vector"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Converts the specified object to a <see cref="Vector"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Describes the <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="Vector"/> created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// value is null. -or- value cannot be converted to a <see cref="Vector"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is null)
        {
            throw GetConvertFromException(value);
        }

        if (value is string source)
        {
            return Vector.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Converts the specified <see cref="Vector"/> to the specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Describes the <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The <see cref="Vector"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="Vector"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="Vector"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// value is null. -or- value cannot be converted to a <see cref="Vector"/>. -or-
    /// destinationType is not one of the valid destination types.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is Vector vector)
        {
            // Delegate to the formatting/culture-aware ConvertToString method.
            return vector.ConvertToString(null, culture);
        }

        throw GetConvertToException(value, destinationType);
    }
}
