
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
/// Converts instances of other types to and from a <see cref="ColumnDefinitionCollection"/>.
/// </summary>
public sealed class ColumnDefinitionCollectionConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a specified type to an instance of a <see cref="ColumnDefinitionCollection"/>.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// The context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="ColumnDefinitionCollection"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of a <see cref="ColumnDefinitionCollection"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The needed type for which you are evaluating this <see cref="ColumnDefinitionCollection"/> for conversion.
    /// </param>
    /// <returns>
    /// true if this <see cref="ColumnDefinitionCollection"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="ColumnDefinitionCollection"/>.
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
    /// The <see cref="ColumnDefinitionCollection"/> that is created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The specified object is null or is a type that cannot be converted to a <see cref="ColumnDefinitionCollection"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value)
    {
        if (value is string input)
        {
            var collection = new ColumnDefinitionCollection();

            var th = new TokenizerHelper(input, cultureInfo);
            while (th.NextToken())
            {
                collection.Add(new ColumnDefinition { Width = GridLengthConverter.FromString(th.GetCurrentToken(), cultureInfo) });
            }

            return collection;
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="ColumnDefinitionCollection"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> of the type you want to convert.
    /// </param>
    /// <param name="value">
    /// The <see cref="ColumnDefinitionCollection"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="ColumnDefinitionCollection"/> to.
    /// </param>
    /// <returns>
    /// The object that is created from converting this <see cref="ColumnDefinitionCollection"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// value is not a <see cref="ColumnDefinitionCollection"/>; or the destinationType is not one of the valid types for conversion.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType is null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        if (value is not ColumnDefinitionCollection collection || destinationType != typeof(string))
        {
            throw GetConvertToException(value, destinationType);
        }

        string separator = TokenizerHelper.GetNumericListSeparator(culture).ToString();

        return string.Join(separator, collection.Select(column => GridLengthConverter.ToString(column.Width, culture)));
    }
}
