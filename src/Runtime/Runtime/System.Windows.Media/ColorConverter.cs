
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
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI
#endif
{
    /// <summary>
    /// ColorConverter Parses a color.
    /// </summary>
    internal sealed class ColorConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// TypeConverter method override.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="destinationType">Type to convert to</param>
        /// <returns>true if conversion is possible</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// ConvertFrom - attempt to convert to a Color from the given object
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// A NotSupportedException is thrown if the example object is null or is not a valid type
        /// which can be converted to a Color.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null == value)
            {
                throw GetConvertFromException(value);
            }

            string s = value as string;

            if (null == s)
            {
                throw new ArgumentException(string.Format("The object passed to '{0}' is not a valid type.", "ConvertFrom"), nameof(value));
            }

            return Parsers.ParseColor(s, culture);
        }

        /// <summary>
        /// TypeConverter method implementation.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// An NotSupportedException is thrown if the example object is null or is not a Color,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="culture">current culture (see CLR specs)</param>
        /// <param name="value">value to convert from</param>
        /// <param name="destinationType">Type to convert to</param>
        /// <returns>converted value</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string))
            {
                if (value is Color color)
                {
                    return color.ToString(culture);
                }
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}
