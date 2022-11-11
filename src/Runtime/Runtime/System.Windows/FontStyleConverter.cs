
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

#if MIGRATION

using System.ComponentModel;
using System.Globalization;

namespace System.Windows
{
    /// <summary>
    /// FontStyleConverter class parses a font style string.
    /// </summary>
    internal sealed class FontStyleConverter : TypeConverter
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
        /// ConvertFrom - attempt to convert to a FontStyle from the given object
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// A NotSupportedException is thrown if the example object is null or is not a valid type
        /// which can be converted to a FontStyle.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                if (s.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                {
                    return FontStyles.Normal;
                }
                else if (s.Equals("Oblique", StringComparison.OrdinalIgnoreCase))
                {
                    return FontStyles.Oblique;
                }
                else if (s.Equals("Italic", StringComparison.OrdinalIgnoreCase))
                {
                    return FontStyles.Italic;
                }
            }

            throw GetConvertFromException(value);
        }

        /// <summary>
        /// TypeConverter method implementation.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// An NotSupportedException is thrown if the example object is null or is not a FontStyle,
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
                if (value is FontStyle c)
                {
                    return ((IFormattable)c).ToString(null, culture);
                }
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}

#endif