
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

namespace System.Windows;

/// <summary>
/// Converts instances of <see cref="TextDecorationCollection"/> from other data types.
/// </summary>   
public sealed class TextDecorationCollectionConverter : TypeConverter
{
    /// <summary>
    /// Returns a value that indicates whether this converter can convert an object of the given type to an instance of <see cref="TextDecorationCollection"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the converter can convert the provided type to an instance of <see cref="TextDecorationCollection"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Determines whether an instance of <see cref="TextDecorationCollection"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// false is always returned because the <see cref="TextDecorationCollection"/> cannot be converted to another type.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert a specified object to an instance of <see cref="TextDecorationCollection"/>.
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
    /// The instance of <see cref="FontWeight"/> created from the converted input.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Occurs if input is null or is not a valid type for conversion.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string source)
        {
            return FromString(source);
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Attempts to convert an instance of <see cref="TextDecorationCollection"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Describes the <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The instance of <see cref="TextDecorationCollection"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type this instance of <see cref="TextDecorationCollection"/> is converted to.
    /// </param>
    /// <returns>
    /// null is always returned because <see cref="TextDecorationCollection"/> cannot be converted to any other type.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is TextDecorationCollection tdc)
            {
                return tdc.ToString();
            }
        }

        throw GetConvertToException(value, destinationType);
    }

    internal static TextDecorationCollection FromString(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        ReadOnlySpan<char> s = source.AsSpan().Trim();

        if (s.IsEmpty || s.Equals("None", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        else if (s.Equals("Underline", StringComparison.OrdinalIgnoreCase))
        {
            return TextDecorations.Underline;
        }
        else if (s.Equals("Strikethrough", StringComparison.OrdinalIgnoreCase))
        {
            return TextDecorations.Strikethrough;
        }
        else if (s.Equals("OverLine", StringComparison.OrdinalIgnoreCase))
        {
            return TextDecorations.OverLine;
        }
        else if (s.Equals("Baseline", StringComparison.OrdinalIgnoreCase))
        {
            return TextDecorations.Baseline;
        }
        else
        {
            throw new FormatException(string.Format(Strings.InvalidTextDecorationCollectionString, source));
        }
    }
}
