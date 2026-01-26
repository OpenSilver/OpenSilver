
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
/// Converts instances of other types to and from a <see cref="Matrix"/>.
/// </summary>
public sealed class MatrixConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a specific type to an instance of a <see cref="Matrix"/>.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="Matrix"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of a <see cref="Matrix"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="Matrix"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this <see cref="Matrix"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="Matrix"/>.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="Matrix"/> created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The specified object is null or is a type that cannot be converted to a <see cref="Matrix"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return Matrix.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="Matrix"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The <see cref="Matrix"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="Matrix"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="Matrix"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The value is not a <see cref="Matrix"/>, or the destinationType is not a valid conversion type.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Matrix instance)
            {
                return instance.ConvertToString(null, culture);
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
