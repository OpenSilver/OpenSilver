
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

namespace System.Windows.Media.Media3D;

/// <summary>
/// Converts instances of other types to and from instances of <see cref="Matrix3D"/>.
/// </summary>
public sealed class Matrix3DConverter : TypeConverter
{
    /// <summary>
    /// Initializes an instance of the <see cref="Matrix3DConverter"/> class.
    /// </summary>
    public Matrix3DConverter() { }

    /// <summary>
    /// Returns a value that indicates whether this type converter can convert from a specified type.
    /// </summary>
    /// <param name="context">
    /// ITypeDescriptorContext for this call.
    /// </param>
    /// <param name="sourceType">
    /// Type being queried for support.
    /// </param>
    /// <returns>
    /// true if this converter can convert from the specified type; false otherwise.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Gets a value that indicates whether this type converter can convert to the given type.
    /// </summary>
    /// <param name="context">
    /// The ITypeDescriptorContext for this call.
    /// </param>
    /// <param name="destinationType">
    /// The Type being queried for support.
    /// </param>
    /// <returns>
    /// true if this converter can convert to the provided type; false if not.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert to a Matrix3D from the given object.
    /// </summary>
    /// <param name="context">
    /// The ITypeDescriptorContext for this call.
    /// </param>
    /// <param name="culture">
    /// The CultureInfo which is respected when converting.
    /// </param>
    /// <param name="value">
    /// The object to convert to an instance of Matrix3D.
    /// </param>
    /// <returns>
    /// Matrix3D that was constructed.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// A NotSupportedException is thrown if the example object is null or is not a valid type which can be 
    /// converted to a Matrix3D.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return Matrix3D.Parse(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert an instance of <see cref="Matrix3D"/> to the given type.
    /// </summary>
    /// <param name="context">
    /// The ITypeDescriptorContext for this call.
    /// </param>
    /// <param name="culture">
    /// The CultureInfo which is respected when converting.
    /// </param>
    /// <param name="value">
    /// The object to convert to an instance of destinationType.
    /// </param>
    /// <param name="destinationType">
    /// The type to which the Matrix3D instance will be converted.
    /// </param>
    /// <returns>
    /// Object that was constructed.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Throws NotSupportedException if the example object is null or is not a Matrix3D, or if the destinationType 
    /// isn't one of the valid destination types.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is Matrix3D matrix)
        {
            return matrix.ConvertToString(null, culture);
        }

        throw GetConvertToException(value, destinationType);
    }
}
