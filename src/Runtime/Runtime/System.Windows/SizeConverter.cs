
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
/// Converts instances of other types to and from instances of the <see cref="Size"/> class.
/// </summary>
public sealed class SizeConverter : TypeConverter
{
    /// <summary>
    /// Determines whether a class can be converted from a given type to an instance of <see cref="Size"/>.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information about a component.
    /// </param>
    /// <param name="sourceType">
    /// Identifies the data type to evaluate for conversion.
    /// </param>
    /// <returns>
    /// true if the sourceType can be converted to an instance of <see cref="Size"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of <see cref="Size"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information about a component.
    /// </param>
    /// <param name="destinationType">
    /// Identifies the data type to evaluate for conversion.
    /// </param>
    /// <returns>
    /// true if this instance of <see cref="Size"/> can be converted to the destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert a specified object to an instance of <see cref="Size"/>.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information about a component.
    /// </param>
    /// <param name="culture">
    /// Culture-specific information that should be respected during conversion.
    /// </param>
    /// <param name="value">
    /// The source object that is being converted.
    /// </param>
    /// <returns>
    /// The instance of <see cref="Size"/> that is created from the converted source.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// value is null. -or- value cannot be converted to a <see cref="Size"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return Size.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert an instance of <see cref="Size"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information about a component.
    /// </param>
    /// <param name="culture">
    /// Culture-specific information that should be respected during conversion.
    /// </param>
    /// <param name="value">
    /// The instance of <see cref="Size"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type that this instance of <see cref="Size"/> is converted to.
    /// </param>
    /// <returns>
    /// The object that is created from the converted instance of <see cref="Size"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value is null or is not an instance of <see cref="Size"/>, or if the destinationType is not one of the valid destination types.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is Size instance)
            {
                return instance.ConvertToString(null, culture);
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
