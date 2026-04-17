
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

namespace System.Windows;

/// <summary>
/// Converts instances of other types to and from instances of a <see cref="double"/> that represent 
/// an object's length.
/// </summary> 
public class LengthConverter : TypeConverter
{
    /// <summary>
    /// Determines whether conversion is possible from a specified type to a <see cref="double"/> that 
    /// represents an object's length.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// Provides contextual information about a component.
    /// </param>
    /// <param name="sourceType">
    /// Identifies the data type to evaluate for conversion.
    /// </param>
    /// <returns>
    /// true if conversion is possible; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
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
    /// Determines whether conversion is possible to a specified type from a <see cref="double"/> that 
    /// represents an object's length.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// Provides contextual information about a component.
    /// </param>
    /// <param name="destinationType">
    /// Identifies the data type to evaluate for conversion.
    /// </param>
    /// <returns>
    /// true if conversion to the destinationType is possible; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        => destinationType == typeof(string);

    /// <summary>
    /// Converts instances of other data types into instances of <see cref="double"/> that represent an 
    /// object's length.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// Provides contextual information about a component.
    /// </param>
    /// <param name="cultureInfo">
    /// Represents culture-specific information that is maintained during a conversion.
    /// </param>
    /// <param name="source">
    /// Identifies the object that is being converted to <see cref="double"/>.
    /// </param>
    /// <returns>
    /// An instance of <see cref="double"/> that is the value of the conversion.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
    {
        if (source is not null)
        {
            if (source is string str)
            {
                return FromString(str, cultureInfo);
            }
            else
            {
                return Convert.ToDouble(source, cultureInfo);
            }
        }

        throw GetConvertFromException(source);
    }

    /// <summary>
    /// Converts other types into instances of <see cref="double"/> that represent an object's length.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// Describes context information of a component, such as its container and <see cref="PropertyDescriptor"/>.
    /// </param>
    /// <param name="cultureInfo">
    /// Identifies culture-specific information, including the writing system and the calendar that is used.
    /// </param>
    /// <param name="value">
    /// Identifies the <see cref="object"/> that is being converted.
    /// </param>
    /// <param name="destinationType">
    /// The data type that this instance of <see cref="double"/> is being converted to.
    /// </param>
    /// <returns>
    /// A new <see cref="object"/> that is the value of the conversion.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Occurs if the <paramref name="value"/> is null.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is double l && destinationType == typeof(string))
        {
            if (double.IsNaN(l))
            {
                return "Auto";
            }

            return Convert.ToString(l, cultureInfo);
        }

        throw GetConvertToException(value, destinationType);
    }

    // Parse a Length from a string given the CultureInfo.
    internal static double FromString(string s, CultureInfo cultureInfo)
    {
        string valueString = s.Trim();

        //Auto is represented and Double.NaN
        //properties that do not want Auto and NaN to be in their ligit values,
        //should disallow NaN in validation callbacks (same goes for negative values)
        if (valueString.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            return double.NaN;
        }

        (string length, double unitFactor) = ParseLength(valueString);

        if (string.IsNullOrEmpty(length))
        {
            return 0;
        }

        return ParseDouble(length, cultureInfo) * unitFactor;
    }

    private static (string Length, double UnitFactor) ParseLength(string value)
    {
        if (value.EndsWith("px", StringComparison.OrdinalIgnoreCase))
        {
            return (value.Substring(0, value.Length - 2), 1.0);
        }

        if (value.EndsWith("in", StringComparison.OrdinalIgnoreCase))
        {
            return (value.Substring(0, value.Length - 2), 96.0);
        }

        if (value.EndsWith("cm", StringComparison.OrdinalIgnoreCase))
        {
            return (value.Substring(0, value.Length - 2), 96.0 / 2.54);
        }

        if (value.EndsWith("pt", StringComparison.OrdinalIgnoreCase))
        {
            return (value.Substring(0, value.Length - 2), 96.0 / 72.0);
        }

        return (value, 1.0);
    }

    private static double ParseDouble(string value, CultureInfo cultureInfo)
    {
        // FormatException errors thrown by Convert.ToDouble are pretty uninformative.
        // Throw a more meaningful error in this case that tells that we were attempting
        // to create a Length instance from a string.  This addresses windows bug 968884
        try
        {
            return double.Parse(value, cultureInfo);
        }
        catch (FormatException)
        {
            throw new FormatException(string.Format(Strings.LengthFormatError, value));
        }
    }
}
