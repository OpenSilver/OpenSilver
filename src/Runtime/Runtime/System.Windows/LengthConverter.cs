
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

namespace System.Windows
{
    /// <summary>
    /// LengthConverter - Converter class for converting instances of other types to and from double representing length.
    /// </summary> 
    internal sealed class LengthConverter : TypeConverter
    {
        //-------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------

        /// <summary>
        /// CanConvertFrom - Returns whether or not this class can convert from a given type.
        /// </summary>
        /// <returns>
        /// bool - True if thie converter can convert from the provided type, false if not.
        /// </returns>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="sourceType"> The Type being queried for support. </param>
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
        /// CanConvertTo - Returns whether or not this class can convert to a given type.
        /// </summary>
        /// <returns>
        /// bool - True if this converter can convert to the provided type, false if not.
        /// </returns>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="destinationType"> The Type being queried for support. </param>
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            // We can convert to an InstanceDescriptor or to a string.
            return destinationType == typeof(string);
        }

        /// <summary>
        /// ConvertFrom - Attempt to convert to a length from the given object
        /// </summary>
        /// <returns>
        /// The double representing the size in 1/96th of an inch.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the example object is not null and is not a valid type
        /// which can be converted to a double.
        /// </exception>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="source"> The object to convert to a double. </param>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object source)
        {
            if (source != null)
            {
                if (source is string str)
                    return FromString(str, cultureInfo);
                else
                    return Convert.ToDouble(source, cultureInfo);
            }

            throw GetConvertFromException(source);
        }

        /// <summary>
        /// ConvertTo - Attempt to convert a double to the given type
        /// </summary>
        /// <returns>
        /// The object which was constructed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the object is not null,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The double to convert. </param>
        /// <param name="destinationType">The type to which to convert the double. </param>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext,
                                         CultureInfo cultureInfo,
                                         object value,
                                         Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is double l && destinationType == typeof(string))
            {
                return ToString(l, cultureInfo);
            }

            throw GetConvertToException(value, destinationType);
        }

        //-------------------------------------------------------------------
        //
        //  Internal Methods
        //
        //-------------------------------------------------------------------

        // Parse a Length from a string given the CultureInfo.
        internal static double FromString(string s, CultureInfo cultureInfo)
        {
            string valueString = s.Trim();

            //Auto is represented and Double.NaN
            //properties that do not want Auto and NaN to be in their ligit values,
            //should disallow NaN in validation callbacks (same goes for negative values)
            if (valueString.Equals("auto", StringComparison.OrdinalIgnoreCase)) return double.NaN;

            // FormatException errors thrown by Convert.ToDouble are pretty uninformative.
            // Throw a more meaningful error in this case that tells that we were attempting
            // to create a Length instance from a string.  This addresses windows bug 968884
            try
            {
                return Convert.ToDouble(valueString, cultureInfo);
            }
            catch (FormatException)
            {
                throw new FormatException(string.Format("'{0}' string cannot be converted to Length.", valueString));
            }
        }

        internal static string ToString(double l, CultureInfo cultureInfo)
        {
            if (double.IsNaN(l)) return "Auto";
            return Convert.ToString(l, cultureInfo);
        }
    }
}
