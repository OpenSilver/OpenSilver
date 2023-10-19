
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
using System.Text;
using OpenSilver.Internal;

namespace System.Windows
{
    /// <summary>
    /// CornerRadiusConverter - Converter class for converting instances of other types to and from CornerRadius instances.
    /// </summary> 
    internal sealed class CornerRadiusConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom - Returns whether or not this class can convert from a given type.
        /// </summary>
        /// <returns>
        /// bool - True if thie converter can convert from the provided type, false if not.
        /// </returns>
        /// <param name="context"> The ITypeDescriptorContext for this call. </param>
        /// <param name="sourceType"> The Type being queried for support. </param>
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
        /// CanConvertTo - Returns whether or not this class can convert to a given type.
        /// </summary>
        /// <returns>
        /// bool - True if this converter can convert to the provided type, false if not.
        /// </returns>
        /// <param name="context"> The ITypeDescriptorContext for this call. </param>
        /// <param name="destinationType"> The Type being queried for support. </param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// ConvertFrom - Attempt to convert to a CornerRadius from the given object
        /// </summary>
        /// <returns>
        /// The CornerRadius which was constructed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the example object is not null and is not a valid type
        /// which can be converted to a CornerRadius.
        /// </exception>
        /// <param name="context"> The ITypeDescriptorContext for this call. </param>
        /// <param name="culture"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The object to convert to a CornerRadius. </param>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null)
            {
                if (value is string)
                { 
                    return FromString((string)value, culture); 
                }
                else
                { 
                    return new CornerRadius(Convert.ToDouble(value, culture)); 
                }
            }

            throw GetConvertFromException(value);
        }

        /// <summary>
        /// ConvertTo - Attempt to convert a CornerRadius to the given type
        /// </summary>
        /// <returns>
        /// The object which was constructoed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the object is not null and is not a CornerRadius,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        /// <param name="context"> The ITypeDescriptorContext for this call. </param>
        /// <param name="culture"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The CornerRadius to convert. </param>
        /// <param name="destinationType">The type to which to convert the CornerRadius instance. </param>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (null == destinationType)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (!(value is CornerRadius))
            {
                throw new ArgumentException(
                    string.Format("Parameter is unexpected type '{0}'. Expected type is '{1}'.", value.GetType(), typeof(CornerRadius)), 
                    nameof(value)
                );
            }

            CornerRadius cr = (CornerRadius)value;
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
            StringBuilder sb = new StringBuilder(64);

            sb.Append(cr.TopLeft.ToString(culture));
            sb.Append(listSeparator);
            sb.Append(cr.TopRight.ToString(culture));
            sb.Append(listSeparator);
            sb.Append(cr.BottomRight.ToString(culture));
            sb.Append(listSeparator);
            sb.Append(cr.BottomLeft.ToString(culture));
            return sb.ToString();
        }

        internal static CornerRadius FromString(string s, CultureInfo culture)
        {
            if (s != null)
            {
                char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(culture), ' ' };

                string[] split = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                double[] radii = new double[4];
                int i = 0;

                for (; i < split.Length; i++)
                {
                    if (i >= 4)
                    {
                        i = 5;    // Set i to a bad value. 
                        break;
                    }

                    radii[i] = double.Parse(split[i], culture);
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
            }

            throw new FormatException(
                string.Format("'{0}' value is not valid. It must contain one or four delimited Lengths.", s)
            );
        }
    }
}
