
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
/// Converts instances of other types to and from a <see cref="Point"/>.
/// </summary>
public sealed class PointConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a given type to an instance of a <see cref="Point"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="Point"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of a <see cref="Point"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="Point"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this <see cref="Point"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="Point"/>.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information to respect during conversion.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="Point"/> created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The specified object is NULL or is a type that cannot be converted to a <see cref="Point"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return Point.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="Point"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information to respect during conversion.
    /// </param>
    /// <param name="value">
    /// The <see cref="Point"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="Point"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="Point"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value is null or is not a <see cref="Point"/>, or destinationType is not one of the valid types for conversion.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Point instance)
            {
                return instance.ToString(culture);
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
