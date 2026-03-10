
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
/// Converts instances of other types to and from a <see cref="PathFigureCollection"/>.
/// </summary>
public sealed class PathFigureCollectionConverter : TypeConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PathFigureCollection"/> class.
    /// </summary>
    public PathFigureCollectionConverter() { }

    /// <summary>
    /// Indicates whether an object can be converted from a given type to an instance of a <see cref="PathFigureCollection"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The source <see cref="Type"/> that is being queried for conversion support.
    /// </param>
    /// <returns>
    /// true if object of the specified type can be converted to a <see cref="PathFigureCollection"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether instances of <see cref="PathFigureCollection"/> can be converted to the specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="PathFigureCollection"/> is being evaluated to be converted to.
    /// </param>
    /// <returns>
    /// true if instances of <see cref="PathFigureCollection"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => false;

    /// <summary>
    /// Converts the specified object to a <see cref="PathFigureCollection"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Describes the <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="PathFigureCollection"/> created from converting value.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is null)
        {
            throw GetConvertFromException(value);
        }

        if (value is string source)
        {
            return PathFigureCollection.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Converts the specified <see cref="PathFigureCollection"/> to the specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Describes the <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The <see cref="PathFigureCollection"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert the <see cref="PathFigureCollection"/> to.
    /// </param>
    /// <returns>
    /// An <see cref="object"/> that represents the converted value.
    /// </returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        throw GetConvertToException(value, destinationType);
    }
}
