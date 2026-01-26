
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
/// Used to convert a <see cref="Brush"/> object to or from another object type.
/// </summary>
public sealed class BrushConverter : TypeConverter
{
    /// <summary>
    /// Determines whether this class can convert an object of a given type to a <see cref="Brush"/> object.
    /// </summary>
    /// <param name="context">
    /// The conversion context.
    /// </param>
    /// <param name="sourceType">
    /// The type from which to convert.
    /// </param>
    /// <returns>
    /// Returns true if conversion is possible (object is string type); otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether this class can convert an object of a given type to the specified destination type.
    /// </summary>
    /// <param name="context">
    /// The conversion context.
    /// </param>
    /// <param name="destinationType">
    /// The destination type.
    /// </param>
    /// <returns>
    /// Returns true if conversion is possible; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Converts from an object of a given type to a <see cref="Brush"/> object.
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
    /// Returns a new <see cref="Brush"/> object if successful; otherwise, NULL.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// value is NULL or cannot be converted to a <see cref="Brush"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return Brush.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Converts a <see cref="Brush"/> object to a specified type, using the specified context and culture information.
    /// </summary>
    /// <param name="context">
    /// The conversion context.
    /// </param>
    /// <param name="culture">
    /// The current culture information.
    /// </param>
    /// <param name="value">
    /// The <see cref="Brush"/> to convert.
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
    /// value is NULL or it is not a <see cref="Brush"/> -or- destinationType is not a valid destination type.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Brush instance)
            {
                return instance.ToString();
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
