
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
/// Converts a <see cref="Transform"/> object to or from another object type.
/// </summary>
public sealed class TransformConverter : TypeConverter
{
    /// <summary>
    /// Determines whether this class can convert an object of a specified type to a <see cref="Transform"/> type.
    /// </summary>
    /// <param name="context">
    /// The conversion context.
    /// </param>
    /// <param name="sourceType">
    /// The type from which to convert.
    /// </param>
    /// <returns>
    /// true if conversion is possible; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether this class can convert an object of a specified type to the specified destination type.
    /// </summary>
    /// <param name="context">
    /// The conversion context.
    /// </param>
    /// <param name="destinationType">
    /// The destination type.
    /// </param>
    /// <returns>
    /// true if conversion is possible; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Converts from an object of a specified type to a <see cref="Transform"/> object.
    /// </summary>
    /// <param name="context">
    /// The conversion context.
    /// </param>
    /// <param name="culture">
    /// The culture information that applies to the conversion.
    /// </param>
    /// <param name="value">
    /// The object to convert.
    /// </param>
    /// <returns>
    /// A new <see cref="Transform"/> object.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// value is null or cannot be converted to a <see cref="Transform"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return Transform.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Converts the specified <see cref="Transform"/> to the specified type by using the specified context and culture information.
    /// </summary>
    /// <param name="context">
    /// The conversion context.
    /// </param>
    /// <param name="culture">
    /// The culture information that applies to the conversion.
    /// </param>
    /// <param name="value">
    /// The <see cref="Transform"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The destination type that the value object is converted to.
    /// </param>
    /// <returns>
    /// An object that represents the converted value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value is not a <see cref="Transform"/>. -or- destinationType is not a valid destination type.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Transform instance)
            {
                return instance.ToString();
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
