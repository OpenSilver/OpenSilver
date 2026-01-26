
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

namespace System.Windows.Media;

/// <summary>
/// Converts instances of other types to and from instances of <see cref="Geometry"/>.
/// </summary>
public sealed class GeometryConverter : TypeConverter
{
    /// <summary>
    /// Indicates whether an object can be converted from a given type to an instance of a <see cref="Geometry"/>.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="sourceType">
    /// The source <see cref="Type"/> that is being queried for conversion support.
    /// </param>
    /// <returns>
    /// true if object of the specified type can be converted to a <see cref="Geometry"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether instances of <see cref="Geometry"/> can be converted to the specified type.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="Geometry"/> is being evaluated to be converted to.
    /// </param>
    /// <returns>
    /// true if instances of <see cref="Geometry"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Converts the specified object to a <see cref="Geometry"/>.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information respected during conversion.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="Geometry"/> created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// value is null or is not a valid type that can be converted to a <see cref="Geometry"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return Geometry.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Converts the specified <see cref="Geometry"/> to the specified type.
    /// </summary>
    /// <param name="context">
    /// Context information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information respected during conversion.
    /// </param>
    /// <param name="value">
    /// The <see cref="Geometry"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert the <see cref="Geometry"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="Geometry"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value is not a <see cref="Geometry"/>, or the destinationType cannot be converted into a <see cref="Geometry"/>.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Geometry instance)
            {
                return instance.ToString();
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
