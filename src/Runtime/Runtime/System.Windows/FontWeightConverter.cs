
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
using FontweightsEnum = System.Windows.FontWeights.INTERNAL_FontweightsEnum;
#else
using FontweightsEnum = Windows.UI.Text.FontWeights.INTERNAL_FontweightsEnum;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Text
#endif
{
    /// <summary>
    /// FontWeightConverter class parses a font weight string.
    /// </summary>
    internal class FontWeightConverter : TypeConverter
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
        /// ConvertFrom - attempt to convert to a FontWeight from the given object
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// A NotSupportedException is thrown if the example object is null or is not a valid type
        /// which can be converted to a FontWeight.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                if (Enum.TryParse(s, true, out FontweightsEnum fontCode))
                {
                    return FontWeight.INTERNAL_ConvertFromUshort((ushort)fontCode);
                }
                else if (ushort.TryParse(s, out ushort code))
                {
                    return FontWeight.INTERNAL_ConvertFromUshort(code);
                }
                else
                {
                    throw new FormatException("Token is not valid.");
                }
            }

            throw GetConvertFromException(value);
        }

        /// <summary>
        /// TypeConverter method implementation.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// An NotSupportedException is thrown if the example object is null or is not a FontWeight,
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
                if (value is FontWeight c)
                {
                    return ((IFormattable)c).ToString(null, culture);
                }
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}
