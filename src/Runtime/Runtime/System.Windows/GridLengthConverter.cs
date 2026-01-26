
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
/// Converts instances of other types to and from <see cref="GridLength"/> instances.
/// </summary> 
public class GridLengthConverter : TypeConverter
{
    /// <summary>
    /// Determines whether a class can be converted from a given type to an instance of <see cref="GridLength"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="sourceType">
    /// The type of the source that is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the converter can convert from the specified type to an instance of <see cref="GridLength"/>; otherwise, false.
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
    /// Determines whether an instance of <see cref="GridLength"/> can be converted to a different type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="destinationType">
    /// The desired type that this instance of <see cref="GridLength"/> is being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if the converter can convert this instance of <see cref="GridLength"/> to the specified type; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Attempts to convert a specified object to an instance of <see cref="GridLength"/>.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="cultureInfo">
    /// Cultural specific information that should be respected during conversion.
    /// </param>
    /// <param name="source">
    /// The object being converted.
    /// </param>
    /// <returns>
    /// The instance of <see cref="GridLength"/> that is created from the converted source.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo cultureInfo, object source)
    {
        if (source != null)
        {
            if (source is string s)
            {
                return (FromString(s, cultureInfo));
            }
            else
            {
                //  conversion from numeric type
                double value;
                GridUnitType type;

                value = Convert.ToDouble(source, cultureInfo);

                if (double.IsNaN(value))
                {
                    //  this allows for conversion from Width / Height = "Auto" 
                    value = 1.0;
                    type = GridUnitType.Auto;
                }
                else
                {
                    type = GridUnitType.Pixel;
                }

                return new GridLength(value, type);
            }
        }

        throw GetConvertFromException(source);
    }

    /// <summary>
    /// Attempts to convert an instance of <see cref="GridLength"/> to a specified type.
    /// </summary>
    /// <param name="context">
    /// Describes the context information of a type.
    /// </param>
    /// <param name="cultureInfo">
    /// Cultural specific information that should be respected during conversion.
    /// </param>
    /// <param name="value">
    /// The instance of <see cref="GridLength"/> to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type that this instance of <see cref="GridLength"/> is converted to.
    /// </param>
    /// <returns>
    /// The object that is created from the converted instance of <see cref="GridLength"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string))
        {
            if (value is GridLength gl)
            {
                return ToString(gl, cultureInfo);
            }
        }

        throw GetConvertToException(value, destinationType);
    }

    /// <summary>
    /// Converts a GridLength instance to a String given the CultureInfo.
    /// </summary>
    /// <param name="gl">GridLength instance to convert.</param>
    /// <param name="cultureInfo">Culture Info.</param>
    /// <returns>String representation of the object.</returns>
    internal static string ToString(GridLength gl, CultureInfo cultureInfo)
    {
        switch (gl.GridUnitType)
        {
            //  for Auto print out "Auto". value is always "1.0"
            case (GridUnitType.Auto):
                return ("Auto");

            //  Star has one special case when value is "1.0".
            //  in this case drop value part and print only "Star"
            case (GridUnitType.Star):
                return (
                    gl.Value == 1.0
                    ? "*"
                    : Convert.ToString(gl.Value, cultureInfo) + "*");

            //  for Pixel print out the numeric value. "px" can be omitted.
            default:
                return (Convert.ToString(gl.Value, cultureInfo));
        }
    }

    /// <summary>
    /// Parses a GridLength from a string given the CultureInfo.
    /// </summary>
    /// <param name="s">String to parse from.</param>
    /// <param name="cultureInfo">Culture Info.</param>
    /// <returns>Newly created GridLength instance.</returns>
    /// <remarks>
    /// Formats: 
    /// "[value][unit]"
    ///     [value] is a double
    ///     [unit] is a string in GridLength._unitTypes connected to a GridUnitType
    /// "[value]"
    ///     As above, but the GridUnitType is assumed to be GridUnitType.Pixel
    /// "[unit]"
    ///     As above, but the value is assumed to be 1.0
    ///     This is only acceptable for a subset of GridUnitType: Auto
    /// </remarks>
    internal static GridLength FromString(string s, CultureInfo cultureInfo)
    {
        double value;
        GridUnitType unit;

        string source = s.Trim().ToLower();
        if (source == "auto")
        {
            value = 1.0;
            unit = GridUnitType.Auto;
        }
        else if (source.EndsWith("*"))
        {
            value = ReadDouble(source, cultureInfo, 1.0);
            unit = GridUnitType.Star;
        }
        else
        {
            value = ReadDouble(source, cultureInfo, 0.0);
            unit = GridUnitType.Pixel;
        }

        return (new GridLength(value, unit));
    }

    private static double ReadDouble(string source, CultureInfo culture, double defaultValue)
    {
        // flag used to keep track of dots in the sequence.
        // If we weet more than 1 dot, just ignore the rest of the sequence.
        bool isFloat = false;

        int i = 0;
        for (; i < source.Length; i++)
        {
            char c = source[i];
            if (c == '.')
            {
                if (isFloat)
                {
                    break;
                }

                isFloat = true;
                continue;
            }
            else if (!char.IsDigit(c))
            {
                break;
            }
        }

        if (i == 0)
        {
            return defaultValue;
        }
        else if (i == 1 && source[0] == '.')
        {
            return 0.0;
        }

        return Convert.ToDouble(source.Substring(0, i), culture);
    }
}
