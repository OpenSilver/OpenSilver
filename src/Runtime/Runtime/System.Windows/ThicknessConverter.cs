
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// ThicknessConverter - Converter class for converting instances of other types to and from Thickness instances.
    /// </summary> 
    internal sealed class ThicknessConverter : TypeConverter
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
        /// ConvertFrom - Attempt to convert to a Thickness from the given object
        /// </summary>
        /// <returns>
        /// The Thickness which was constructed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the example object is not null and is not a valid type
        /// which can be converted to a Thickness.
        /// </exception>
        /// <param name="context"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="source"> The object to convert to a Thickness. </param>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo cultureInfo, object source)
        {
            if (source != null)
            {
                if (source is string)
                { 
                    return FromString((string)source, cultureInfo); 
                }
                else if (source is double)
                { 
                    return new Thickness((double)source); 
                }
                else
                { 
                    return new Thickness(Convert.ToDouble(source, cultureInfo)); 
                }
            }

            throw GetConvertFromException(source);
        }

        /// <summary>
        /// ConvertTo - Attempt to convert a Thickness to the given type
        /// </summary>
        /// <returns>
        /// The object which was constructoed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the object is not null and is not a Thickness,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        /// <param name="context"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The Thickness to convert. </param>
        /// <param name="destinationType">The type to which to convert the Thickness instance. </param>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (null == destinationType)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (!(value is Thickness))
            {
                throw new ArgumentException(
                    string.Format("Parameter is unexpected type '{0}'. Expected type is '{1}'.", value.GetType(), typeof(Thickness)), 
                    nameof(value)
                );
            }

            if (destinationType == typeof(string))
            { 
                return ToString((Thickness)value, cultureInfo); 
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
            StringBuilder sb = new StringBuilder(64);

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
            if (s != null)
            {
                char[] separator = new char[2] { TokenizerHelper.GetNumericListSeparator(cultureInfo), ' ' };

                string[] split = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                double[] lengths = new double[4];
                int i = 0;

                for (; i < split.Length; i++)
                {
                    if (i >= 4)
                    {
                        i = 5;    // Set i to a bad value. 
                        break;
                    }

                    lengths[i] = Convert.ToDouble(split[i], cultureInfo);
                }

                // We have a reasonable interpreation for one value (all four edges)
                // and four values (left, top, right, bottom).
                switch (i)
                {
                    case 1:
                        return (new Thickness(lengths[0]));

                    case 2:
                        return (new Thickness(lengths[0], lengths[1], lengths[0], lengths[1]));

                    case 4:
                        return (new Thickness(lengths[0], lengths[1], lengths[2], lengths[3]));
                }
            }

            throw new FormatException(
                string.Format("'{0}' value is not valid. It must contain one, two, or four delimited Lengths.", s)
            );
        }
    }
}
