
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

namespace System.Windows.Media
{
    /// <summary>
    /// FontFamilyConverter - converter class for converting between the FontFamily
    /// and String types.
    /// </summary>
    internal sealed class FontFamilyConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom - Returns whether or not the given type can be converted to a
        /// FontFamily.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// CanConvertTo - Returns whether or not this class can convert to the specified type.
        /// Conversion is possible only if the source and destination types are FontFamily and
        /// string, respectively, and the font family is not anonymous (i.e., the Source propery
        /// is not null).
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="destinationType">Type to convert to</param>
        /// <returns>true if conversion is possible</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// ConvertFrom - Converts the specified object to a FontFamily.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                stringValue = stringValue.Trim();

                return new FontFamily(stringValue);
            }

            throw GetConvertFromException(value);
        }

        /// <summary>
        /// ConvertTo - Converts the specified object to an instance of the specified type.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (null == value)
            {
                throw new ArgumentNullException("value");
            }

            FontFamily fontFamily = value as FontFamily;
            if (fontFamily == null)
            {
                throw new ArgumentException(string.Format("Expected object of type '{0}'.", "FontFamily"), "value");
            }

            if (null == destinationType)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (destinationType == typeof(string))
            {
                if (fontFamily.Source != null)
                {
                    // Usual case: it's a named font family.
                    return fontFamily.Source;
                }
                else
                {
                    return string.Empty;
                }
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}
