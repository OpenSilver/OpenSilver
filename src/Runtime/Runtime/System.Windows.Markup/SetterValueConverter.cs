
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xaml;
using System.Xaml.Schema;

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Markup
{
    internal sealed class SetterValueConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom()
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="sourceType">type to convert from</param>
        /// <returns>true if the given type can be converted, false otherwise</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // We can only convert from a string and that too only if we have all the contextual information
            // Note: Sometimes even the serializer calls CanConvertFrom in order 
            // to determine if it is a valid converter to use for serialization.
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
            return false;
        }

        /// <summary>
        /// ConvertFrom() -TypeConverter method override. using the givein name to return DependencyProperty
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="culture">CultureInfo</param>
        /// <param name="source">Object to convert from</param>
        /// <returns>instance of Command</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            return ResolveValue(context, null, culture, source);
        }

        /// <summary>
        /// ConvertTo() - Serialization purposes, returns the string from Command.Name by adding ownerType.FullName
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="culture">CultureInfo</param>
        /// <param name="value">the	object to convert from</param>
        /// <param name="destinationType">the type to convert to</param>
        /// <returns>string object, if the destination type is string</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw GetConvertToException(value, destinationType);
        }

        internal static object ResolveValue(ITypeDescriptorContext serviceProvider,
            DependencyProperty property, CultureInfo culture, object source)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // Only need to type convert strings
            if (source is not string)
            {
                return source;
            }

            if (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) is not IXamlSchemaContextProvider ixsc)
            {
                throw NotSupported();
            }

            XamlSchemaContext schemaContext = ixsc.SchemaContext;

            if (property == null)
            {
                if (serviceProvider.GetService(typeof(IAmbientProvider)) is not IAmbientProvider ambientProvider)
                {
                    throw new NotSupportedException(
                        string.Format("'{0}' requires '{1}' be implemented.",
                            nameof(SetterValueConverter),
                            nameof(IAmbientProvider)));
                }

                object firstAmbientValue = ambientProvider.GetFirstAmbientValue(schemaContext.GetXamlType(typeof(Setter)));
                if (firstAmbientValue != null)
                {
                    if (firstAmbientValue is not Setter setter)
                    {
                        throw new NotSupportedException(string.Format("Cannot convert {0}. Cannot find a {1}.", source, nameof(Setter)));
                    }

                    if (setter.Property != null)
                    {
                        property = setter.Property;
                    }
                    else
                    {
                        setter.ReceiveTypeConverter(serviceProvider, culture, source);
                    }
                }
            }

            if (property != null)
            {
                // Get XamlMember from dp
                XamlMember xamlProperty = schemaContext.GetXamlType(property.OwnerType).GetMember(property.Name)
                    ?? schemaContext.GetXamlType(property.OwnerType).GetAttachableMember(property.Name);

                XamlValueConverter<TypeConverter> typeConverter;
                if (xamlProperty != null)
                {
                    typeConverter = xamlProperty.TypeConverter ?? xamlProperty.Type.TypeConverter;
                }
                else
                {
                    typeConverter = schemaContext.GetXamlType(property.PropertyType).TypeConverter;
                }

                // No Type converter case...
                if (typeConverter.ConverterType == null)
                {
                    return source;
                }

                TypeConverter converter;
                if (xamlProperty != null && xamlProperty.Type.UnderlyingType == typeof(bool))
                {
                    if (source is string)
                    {
                        converter = new BooleanConverter();
                    }
                    else
                    {
                        throw NotSupported();
                    }
                }
                else
                {
                    converter = typeConverter.ConverterInstance;
                }

                return converter.ConvertFrom(serviceProvider, culture, source);
            }
            else
            {
                return source;
            }

            static NotSupportedException NotSupported()
            {
                return new NotSupportedException(
                    string.Format("Cannot convert the value in attribute '{0}' to object of type '{1}'.",
                        nameof(Setter.Value),
                        typeof(object).FullName));
            }
        }
    }
}
