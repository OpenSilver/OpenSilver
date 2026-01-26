
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
/// Converts instances of other types to and from instances of <see cref="Rect"/>.
/// </summary>
public sealed class RectConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a given type to an instance of <see cref="Rect"/>.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="Rect"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether a <see cref="Rect"/> can be converted to the specified type.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="Rect"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if a <see cref="Rect"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="Rect"/>.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information which is respected when converting.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="Rect"/> created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The specified object is NULL or is a type that cannot be converted to a <see cref="Rect"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return Rect.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="Rect"/> to the specified type.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information which is respected during conversion.
    /// </param>
    /// <param name="value">
    /// The <see cref="Rect"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="Rect"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="Rect"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value is null. -or- value is not a <see cref="Rect"/>. -or- The destinationType is not one of the valid types for conversion.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Rect rect)
            {
                return rect.ToString(culture);
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
