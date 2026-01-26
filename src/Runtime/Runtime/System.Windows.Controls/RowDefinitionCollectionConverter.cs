
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

using OpenSilver.Internal;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace System.Windows.Controls;

/// <summary>
/// Converts instances of other types to and from a <see cref="RowDefinitionCollection"/>.
/// </summary>
public sealed class RowDefinitionCollectionConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a specified type to an instance of a <see cref="RowDefinitionCollection"/>.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// The context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="RowDefinitionCollection"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of a <see cref="RowDefinitionCollection"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The needed type for which you are evaluating this <see cref="RowDefinitionCollection"/> for conversion.
    /// </param>
    /// <returns>
    /// true if this <see cref="RowDefinitionCollection"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="RowDefinitionCollection"/>.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// The context information of a type.
    /// </param>
    /// <param name="cultureInfo">
    /// The <see cref="CultureInfo"/> of the type you want to convert.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="RowDefinitionCollection"/> that is created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The specified object is null or is a type that cannot be converted to a <see cref="RowDefinitionCollection"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value)
    {
        if (value is string input)
        {
            var collection = new RowDefinitionCollection();

            var th = new TokenizerHelper(input, cultureInfo);
            while (th.NextToken())
            {
                collection.Add(new RowDefinition { Height = GridLengthConverter.FromString(th.GetCurrentToken(), cultureInfo) });
            }

            return collection;
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="RowDefinitionCollection"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> of the type you want to convert.
    /// </param>
    /// <param name="value">
    /// The <see cref="RowDefinitionCollection"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="RowDefinitionCollection"/> to.
    /// </param>
    /// <returns>
    /// The object that is created from converting this <see cref="RowDefinitionCollection"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value is not a <see cref="RowDefinitionCollection"/>; or the destinationType is not one of the valid types for conversion.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is not RowDefinitionCollection collection || destinationType != typeof(string))
        {
            throw GetConvertToException(value, destinationType);
        }

        string separator = TokenizerHelper.GetNumericListSeparator(culture).ToString();

        return string.Join(separator, collection.Select(row => GridLengthConverter.ToString(row.Height, culture)));
    }
}
