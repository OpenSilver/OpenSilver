
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
using OpenSilver.Internal;

namespace System.Windows.Media.Animation;

/// <summary>
/// Converts instances of other types to and from a <see cref="KeySpline"/>.
/// </summary>
internal sealed class KeySplineConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object can be converted from a given type to an instance of a 
    /// <see cref="KeySpline"/>.
    /// </summary>
    /// <param name="typeDescriptor">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the type can be converted to a <see cref="KeySpline"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptor, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Determines whether an instance of a <see cref="KeySpline"/> can be converted to a 
    /// different type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="KeySpline"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this <see cref="KeySpline"/> can be converted to destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="KeySpline"/>.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="cultureInfo">
    /// Cultural information to respect during conversion.
    /// </param>
    /// <param name="value">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The <see cref="KeySpline"/> created from converting value.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The specified object is NULL or is a type that cannot be converted to a <see cref="KeySpline"/>.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo cultureInfo, object value)
    {
        if (value is string source)
        {
            if (source.Length == 0)
            {
                return new KeySpline();
            }

            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(formatProvider), ' ' };
            string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 4)
            {
                return new KeySpline(
                    Convert.ToDouble(split[0], formatProvider),
                    Convert.ToDouble(split[1], formatProvider),
                    Convert.ToDouble(split[2], formatProvider),
                    Convert.ToDouble(split[3], formatProvider)
                );
            }
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert a <see cref="KeySpline"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// Provides contextual information required for conversion.
    /// </param>
    /// <param name="cultureInfo">
    /// Cultural information to respect during conversion.
    /// </param>
    /// <param name="value">
    /// The <see cref="KeySpline"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert this <see cref="KeySpline"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="KeySpline"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// value is null or is not a <see cref="KeySpline"/>, or destinationType is not one 
    /// of the valid types for conversion.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
    {
        if (value is KeySpline keySpline)
        {
            if (destinationType == typeof(string))
            {
                return string.Format(
                    cultureInfo,
                    "{0}{4}{1}{4}{2}{4}{3}",
                    keySpline.ControlPoint1.X,
                    keySpline.ControlPoint1.Y,
                    keySpline.ControlPoint2.X,
                    keySpline.ControlPoint2.Y,
                    cultureInfo != null ? cultureInfo.TextInfo.ListSeparator : CultureInfo.InvariantCulture.TextInfo.ListSeparator);
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
