
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
/// Converts instances of other types to and from a <see cref="DoubleCollection"/>.
/// </summary>
public sealed class DoubleCollectionConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a specified type to an instance of a <see cref="DoubleCollection"/>.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="DoubleCollection"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of a <see cref="DoubleCollection"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The needed type for which you are evaluating this <see cref="DoubleCollection"/> for conversion.
    /// </param>
    /// <returns>
    /// true if this <see cref="DoubleCollection"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="DoubleCollection"/>.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> of the type you want to convert.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="DoubleCollection"/> that is created from converting value.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return DoubleCollection.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="DoubleCollection"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> of the type you want to convert.
    /// </param>
    /// <param name="value">
    /// The <see cref="DoubleCollection"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="DoubleCollection"/> to.
    /// </param>
    /// <returns>
    /// The object you create when you convert this <see cref="DoubleCollection"/>.
    /// </returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is DoubleCollection instance)
            {
                return instance.ToString();
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
