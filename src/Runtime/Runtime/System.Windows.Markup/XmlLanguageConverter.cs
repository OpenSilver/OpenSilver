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

namespace System.Windows.Markup
{
    /// <summary>
    /// XmlLanuageConverter - Converter class for converting instances of other types to and from XmlLanguage
    /// in a way that does not depend on the current user's language settings.
    /// </summary> 
    internal sealed class XmlLanguageConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom - Returns whether or not this class can convert from a given type.
        /// </summary>
        /// <returns>
        /// bool - True if this converter can convert from the provided type, false if not.
        /// </returns>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="sourceType"> The Type being queried for support. </param>
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            // We can only handle strings.
            return sourceType == typeof(string);
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
        /// ConvertFrom - Attempt to convert to a CultureInfo from the given object
        /// </summary>
        /// <returns>
        /// A CultureInfo object based on the specified culture name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the example object is not null and is not a valid type
        /// which can be converted to a CultureInfo.
        /// </exception>
        /// <param name="typeDescriptorContext">The ITypeDescriptorContext for this call.</param>
        /// <param name="cultureInfo">The CultureInfo which is respected when converting.</param>
        /// <param name="source">The object to convert to a CultureInfo.</param>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object source)
        {
            string ietfLanguageTag = source as string;
            if (ietfLanguageTag != null)
            {
                return XmlLanguage.GetLanguage(ietfLanguageTag);
            }

            throw GetConvertFromException(source);
        }

        /// <summary>
        /// ConvertTo - Attempt to convert a CultureInfo to the given type
        /// </summary>
        /// <returns>
        /// The object which was constructed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the example object is not null and is not a CultureInfo,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The XmlLanguage to convert. </param>
        /// <param name="destinationType">The type to which to convert the CultureInfo. </param>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext,
                                         CultureInfo cultureInfo,
                                         object value,
                                         Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            XmlLanguage xmlLanguage = value as XmlLanguage;
            if (xmlLanguage != null)
            {
                if (destinationType == typeof(string))
                {
                    return xmlLanguage.IetfLanguageTag;
                }
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}
