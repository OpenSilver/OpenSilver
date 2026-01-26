
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
/// Converts instances of other types to and from instances of <see cref="ResponsiveThreshold"/>.
/// </summary>
public sealed class ResponsiveThresholdConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a given type to an instance of <see cref="ResponsiveThreshold"/>.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="ResponsiveThreshold"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether a <see cref="ResponsiveThreshold"/> can be converted to the specified type.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="ResponsiveThreshold"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if a <see cref="ResponsiveThreshold"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="ResponsiveThreshold"/>.
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
    /// The <see cref="ResponsiveThreshold"/> created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The specified object is NULL or is a type that cannot be converted to a <see cref="ResponsiveThreshold"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return ResponsiveThreshold.Parse(source, culture);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="ResponsiveThreshold"/> to the specified type.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="culture">
    /// Cultural information which is respected during conversion.
    /// </param>
    /// <param name="value">
    /// The <see cref="ResponsiveThreshold"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="ResponsiveThreshold"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="ResponsiveThreshold"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="destinationType"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="value"/> is null.
    /// -or-
    /// <paramref name="value"/> is not a <see cref="ResponsiveThreshold"/>.
    /// -or-
    /// The <paramref name="destinationType"/> is not one of the valid types for conversion.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string) && value is ResponsiveThreshold threshold)
        {
            return threshold.ToString(culture);
        }

        throw GetConvertToException(value, destinationType);
    }
}
