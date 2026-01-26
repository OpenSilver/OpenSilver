
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
/// Converts instances of other types to and from instances of <see cref="Thickness"/>.
/// </summary> 
public class ThicknessConverter : TypeConverter
{
    /// <summary>
    /// Determines whether the type converter can create an instance of <see cref="Thickness"/> from a specified type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The source type that the type converter is evaluating for conversion.
    /// </param>
    /// <returns>
    /// true if the type converter can create an instance of <see cref="Thickness"/> from the specified type; otherwise, false.
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
    /// Determines whether the type converter can convert an instance of <see cref="Thickness"/> to a different type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The type for which the type converter is evaluating this instance of <see cref="Thickness"/> for conversion.
    /// </param>
    /// <returns>
    /// true if the type converter can convert this instance of <see cref="Thickness"/> to the destinationType; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to create an instance of <see cref="Thickness"/> from a specified object.
    /// </summary>
    /// <param name="context">
    /// The context information for a type.
    /// </param>
    /// <param name="cultureInfo">
    /// The <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="source">
    /// The source Object being converted.
    /// </param>
    /// <returns>
    /// An instance of <see cref="Thickness"/> created from the converted source.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo cultureInfo, object source)
    {
        if (source != null)
        {
            return source switch
            {
                string s => FromString(s, cultureInfo),
                double d => new Thickness(d),
                _ => new Thickness(Convert.ToDouble(source, cultureInfo)),
            };
        }

        throw GetConvertFromException(source);
    }

    /// <summary>
    /// Attempts to convert an instance of <see cref="Thickness"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// The context information of a type.
    /// </param>
    /// <param name="cultureInfo">
    /// The <see cref="CultureInfo"/> of the type being converted.
    /// </param>
    /// <param name="value">
    /// The instance of <see cref="Thickness"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type that this instance of <see cref="Thickness"/> is converted to.
    /// </param>
    /// <returns>
    /// The type that is created when the type converter converts an instance of <see cref="Thickness"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// value or destinationType is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// value is not a <see cref="Thickness"/>.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is not Thickness thickness)
        {
            throw new ArgumentException(string.Format(Strings.UnexpectedParameterType, value.GetType(), typeof(Thickness)), nameof(value));
        }

        if (destinationType == typeof(string))
        {
            return ToString(thickness, cultureInfo);
        }

        throw GetConvertToException(value, destinationType);
    }

    internal static string ToString(Thickness th, CultureInfo cultureInfo)
    {
        char listSeparator = TokenizerHelper.GetNumericListSeparator(cultureInfo);

        // Initial capacity [64] is an estimate based on a sum of:
        // 48 = 4x double (twelve digits is generous for the range of values likely)
        //  8 = 4x Unit Type string (approx two characters)
        //  4 = 4x separator characters
        var sb = new StringBuilder(64);

        sb.Append(th.Left.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(th.Top.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(th.Right.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(th.Bottom.ToString(cultureInfo));
        return sb.ToString();
    }

    internal static Thickness FromString(string s, CultureInfo cultureInfo)
    {
        var th = new TokenizerHelper(s, cultureInfo);
        double[] lengths = new double[4];
        int i = 0;

        // Peel off each double in the delimited list.
        while (th.NextToken())
        {
            if (i >= 4)
            {
                i = 5;    // Set i to a bad value. 
                break;
            }

            lengths[i] = Convert.ToDouble(th.GetCurrentToken(), cultureInfo);
            i++;
        }

        // We have a reasonable interpreation for one value (all four edges), two values (horizontal, vertical),
        // and four values (left, top, right, bottom).
        switch (i)
        {
            case 1:
                return new Thickness(lengths[0]);

            case 2:
                return new Thickness(lengths[0], lengths[1], lengths[0], lengths[1]);

            case 4:
                return new Thickness(lengths[0], lengths[1], lengths[2], lengths[3]);
        }

        throw new FormatException(string.Format(Strings.InvalidStringThickness, s));
    }
}
