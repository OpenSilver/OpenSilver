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

namespace System.Windows.Markup;

/// <summary>
/// Provides type conversion for the <see cref="XmlLanguage"/> class.
/// </summary> 
public class XmlLanguageConverter : TypeConverter
{
    /// <summary>
    /// Returns whether this converter can convert an object of one type to the <see cref="XmlLanguage"/> type supported by this converter.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="sourceType">
    /// A type that represents the type you want to convert from.
    /// </param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType) => sourceType == typeof(string);

    /// <summary>
    /// Returns whether this converter can convert the object to the specified type.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="destinationType">
    /// The type you want to convert to.
    /// </param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType) => destinationType == typeof(string);

    /// <summary>
    /// Converts the specified string value to the <see cref="XmlLanguage"/> type supported by this converter.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="cultureInfo">
    /// The <see cref="CultureInfo"/> to use as the current culture.
    /// </param>
    /// <param name="source">
    /// The string to convert.
    /// </param>
    /// <returns>
    /// An object that represents the converted value.
    /// </returns>
    public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
    {
        if (source is string ietfLanguageTag)
        {
            return XmlLanguage.GetLanguage(ietfLanguageTag);
        }

        throw GetConvertFromException(source);
    }

    /// <summary>
    /// Converts the specified <see cref="XmlLanguage"/> to the specified type.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="cultureInfo">
    /// The <see cref="CultureInfo"/> to use as the current culture.
    /// </param>
    /// <param name="value">
    /// The object to convert. This is expected to be type <see cref="XmlLanguage"/>.
    /// </param>
    /// <param name="destinationType">
    /// A type that represents the type you want to convert to.
    /// </param>
    /// <returns>
    /// An object that represents the converted value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// destinationType is null.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is XmlLanguage xmlLanguage)
        {
            if (destinationType == typeof(string))
            {
                return xmlLanguage.IetfLanguageTag;
            }
        }

        throw GetConvertToException(value, destinationType);
    }
}
