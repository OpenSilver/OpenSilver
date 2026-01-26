
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
using System.Text;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Converts instances of other types to and from a <see cref="CornerRadius"/>.
/// </summary> 
public class CornerRadiusConverter : TypeConverter
{
    /// <summary>
    /// Indicates whether an object can be converted from a given type to a <see cref="CornerRadius"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The source <see cref="Type"/> that is being queried for conversion support.
    /// </param>
    /// <returns>
    /// true if sourceType is of type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // We can only handle strings, integral and floating types
        TypeCode tc = Type.GetTypeCode(sourceType);
        switch (tc)
        {
            case TypeCode.String:
            case TypeCode.Decimal:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Determines whether <see cref="CornerRadius"/> values can be converted to the specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type this <see cref="CornerRadius"/> is being evaluated to be converted to.
    /// </param>
    /// <returns>
    /// true if destinationType is of type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Converts the specified object to a <see cref="CornerRadius"/>.
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
    /// The <see cref="CornerRadius"/> created from converting source.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value != null)
        {
            if (value is string s)
            { 
                return FromString(s, culture); 
            }
            else
            { 
                return new CornerRadius(Convert.ToDouble(value, culture)); 
            }
        }

        throw GetConvertFromException(value);
    }

    /// <summary>
    /// Converts the specified <see cref="CornerRadius"/> to the specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="culture">
    /// Describes the <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The <see cref="CornerRadius"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert the <see cref="CornerRadius"/> to.
    /// </param>
    /// <returns>
    /// The object created from converting this <see cref="CornerRadius"/> (a string).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// value or destinationType is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// value is not null and is not a <see cref="CornerRadius"/>, or if destinationType is not one of the valid destination types.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is not CornerRadius cr)
        {
            throw new ArgumentException(string.Format(Strings.UnexpectedParameterType, value.GetType(), typeof(CornerRadius)), nameof(value));
        }

        if (destinationType == typeof(string))
        { 
            return ToString(cr, culture); 
        }

        throw GetConvertToException(value, destinationType);
    }

    internal static string ToString(CornerRadius cr, CultureInfo culture)
    {
        char listSeparator = TokenizerHelper.GetNumericListSeparator(culture);

        // Initial capacity [64] is an estimate based on a sum of:
        // 48 = 4x double (twelve digits is generous for the range of values likely)
        //  8 = 4x UnitType string (approx two characters)
        //  4 = 4x separator characters
        var sb = new StringBuilder(64);

        sb.Append(cr.TopLeft.ToString(culture));
        sb.Append(listSeparator);
        sb.Append(cr.TopRight.ToString(culture));
        sb.Append(listSeparator);
        sb.Append(cr.BottomRight.ToString(culture));
        sb.Append(listSeparator);
        sb.Append(cr.BottomLeft.ToString(culture));
        return sb.ToString();
    }

    internal static CornerRadius FromString(string s, CultureInfo cultureInfo)
    {
        var th = new TokenizerHelper(s, cultureInfo);
        double[] radii = new double[4];
        int i = 0;

        // Peel off each Length in the delimited list.
        while (th.NextToken())
        {
            if (i >= 4)
            {
                i = 5;    // Set i to a bad value. 
                break;
            }

            radii[i] = double.Parse(th.GetCurrentToken(), cultureInfo);
            i++;
        }

        // We have a reasonable interpreation for one value (all four edges)
        // and four values (left, top, right, bottom).
        switch (i)
        {
            case 1:
                return (new CornerRadius(radii[0]));

            case 4:
                return (new CornerRadius(radii[0], radii[1], radii[2], radii[3]));
        }

        throw new FormatException(string.Format(Strings.InvalidStringCornerRadius, s));
    }
}
