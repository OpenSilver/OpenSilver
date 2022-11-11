
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

using System;
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// GridLengthConverter - Converter class for converting 
    /// instances of other types to and from GridLength instances.
    /// </summary> 
    internal sealed class GridLengthConverter : TypeConverter
    {
        /// <summary>
        /// Checks whether or not this class can convert from a given type.
        /// </summary>
        /// <param name="context">The ITypeDescriptorContext 
        /// for this call.</param>
        /// <param name="sourceType">The Type being queried for support.</param>
        /// <returns>
        /// <c>true</c> if thie converter can convert from the provided type, 
        /// <c>false</c> otherwise.
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
        /// Checks whether or not this class can convert to a given type.
        /// </summary>
        /// <param name="context">The ITypeDescriptorContext 
        /// for this call.</param>
        /// <param name="destinationType">The Type being queried for support.</param>
        /// <returns>
        /// <c>true</c> if this converter can convert to the provided type, 
        /// <c>false</c> otherwise.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Attempts to convert to a GridLength from the given object.
        /// </summary>
        /// <param name="context">The ITypeDescriptorContext for this call.</param>
        /// <param name="cultureInfo">The CultureInfo which is respected when converting.</param>
        /// <param name="source">The object to convert to a GridLength.</param>
        /// <returns>
        /// The GridLength instance which was constructed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the example object is not null 
        /// and is not a valid type which can be converted to a GridLength.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo cultureInfo, object source)
        {
            if (source != null)
            {
                if (source is string)
                {
                    return (FromString((string)source, cultureInfo));
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
        /// Attempts to convert a GridLength instance to the given type.
        /// </summary>
        /// <param name="context">The ITypeDescriptorContext for this call.</param>
        /// <param name="cultureInfo">The CultureInfo which is respected when converting.</param>
        /// <param name="value">The GridLength to convert.</param>
        /// <param name="destinationType">The type to which to convert the GridLength instance.</param>
        /// <returns>
        /// The object which was constructed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the object is not null and is not a GridLength,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

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
}
