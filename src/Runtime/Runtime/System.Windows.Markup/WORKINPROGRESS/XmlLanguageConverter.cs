

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


#if WORKINPROGRESS
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Markup
{
    /// <summary>
    /// Converts a <see cref="T:System.Windows.Markup.XmlLanguage" /> object to and from other types.
    /// </summary>
    public class XmlLanguageConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether an object of the specified type can be converted to an instance of <see cref="T:System.Windows.Markup.XmlLanguage" />.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="sourceType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="sourceType" /> is of type <see cref="T:System.String" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Determines whether an instance of <see cref="T:System.Windows.Markup.XmlLanguage" /> can be converted to the specified type.
        /// </summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="destinationType">The type being evaluated for conversion.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="destinationType" /> is of type <see cref="T:System.String" />
        /// or <see cref="T:System.ComponentModel.Design.Serialization.InstanceDescriptor" />; otherwise, <see langword="false" />.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);
        }

        /// <summary>Converts the specified string value to the <see cref="T:System.Windows.Markup.XmlLanguage" /> type supported by this converter.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The object being converted.</param>
        /// <returns>The <see cref="T:System.Windows.Markup.XmlLanguage" /> created from converting <paramref name="value" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">Conversion could not be performed.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result;

            if (value is null)
            {
                throw GetConvertFromException(value);
            }
            else if (value is string)
            {
                result = XmlLanguage.GetLanguage(value.ToString());
            }
            else
            {
                result = base.ConvertFrom(context, culture, value);
            }

            return result;
        }

        /// <summary>Converts the specified <see cref="T:System.Windows.Markup.XmlLanguage" /> to the specified type.</summary>
        /// <param name="context">Describes the context information of a type.</param>
        /// <param name="culture">Describes the System.Globalization.CultureInfo of the type being converted.</param>
        /// <param name="value">The <see cref="T:System.Windows.Markup.XmlLanguage" /> to convert.</param>
        /// <param name="destinationType">The type to convert this <see cref="T:System.Windows.Markup.XmlLanguage" /> to.</param>
        /// <returns>The object created from converting this <see cref="T:System.Windows.Markup.XmlLanguage" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">Conversion could not be performed.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="destinationType" /> is <see langword="null" />.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            else if (value is XmlLanguage language)
            {
                if (destinationType == typeof(string))
                {
                    result = language.IetfLanguageTag;
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
#if BRIDGE
                    var method = typeof(XmlLanguage).GetMethod("GetLanguage", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, new Type[1]
                    {
                        typeof(string)
                    });
#else
                    var method = typeof(XmlLanguage).GetMethod("GetLanguage", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, new Type[1]
                    {
                        typeof(string)
                    }, null);
#endif
                    result = new InstanceDescriptor(method, new object[1] { language.IetfLanguageTag });
                }
            }

            if (result is null)
            {
                throw GetConvertToException(value, destinationType);
            }

            return result;
        }
    }
}

#endif