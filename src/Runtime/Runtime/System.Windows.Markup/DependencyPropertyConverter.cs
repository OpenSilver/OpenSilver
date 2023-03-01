
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

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Markup
{
    /// <summary>
    /// Class for converting a given DependencyProperty to and from a string
    /// </summary>
    internal sealed class DependencyPropertyConverter : TypeConverter
    {
        #region Public Methods

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
            if (sourceType == typeof(string))
            {
                return true;
            }

            return false;
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
        /// ConvertFrom() -TypeConverter method override. using the given name to return DependencyProperty
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="culture">CultureInfo</param>
        /// <param name="source">Object to convert from</param>
        /// <returns>instance of Command</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            DependencyProperty property = ResolveProperty(context, source);

            if (property != null)
            {
                return property;
            }
            else
            {
                throw GetConvertFromException(source);
            }
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

        #endregion Public Methods

        private static DependencyProperty ResolveProperty(IServiceProvider serviceProvider, object source)
        {
            Type type = null;
            string property;

            if (source is DependencyProperty dProperty)
            {
                return dProperty;
            }

            if (source is string value)
            {
                value = value.Trim();
                // If it contains a . it means that it is a full name with type and property.
                if (value.Contains("."))
                {
                    // Prefixes could have .'s so we take the last one and do a type resolve against that
                    int lastIndex = value.LastIndexOf('.');
                    string typeName = value.Substring(0, lastIndex);
                    property = value.Substring(lastIndex + 1);

                    IXamlTypeResolver resolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
                    type = resolver.Resolve(typeName);
                }
                else
                {
                    // Only have the property name
                    // Strip prefixes if there are any, v3 essentially discards the prefix in this case
                    int lastIndex = value.LastIndexOf(':');
                    property = value.Substring(lastIndex + 1);
                }
            }
            else
            {
                throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}. The source is not a string.");
            }

            // Still don't have a Type so we need to loop up the chain and grab Style.TargetType,
            if (type == null)
            {
                if (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) is not IXamlSchemaContextProvider ixscp)
                {
                    throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}. IXamlSchemaContextProvider is null.");
                }

                XamlSchemaContext schemaContext = ixscp.SchemaContext;
                XamlType styleXType = schemaContext.GetXamlType(typeof(Style));
                var ceilingTypes = new List<XamlType>(1) { styleXType };
                XamlMember styleTargetType = styleXType.GetMember(nameof(Style.TargetType));

                if (serviceProvider.GetService(typeof(IAmbientProvider)) is not IAmbientProvider ambientProvider)
                {
                    throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}. IAmbientProvider is null.");
                }

                AmbientPropertyValue firstAmbientValue = ambientProvider.GetFirstAmbientValue(ceilingTypes, styleTargetType);
                if (firstAmbientValue != null)
                {
                    if (firstAmbientValue.Value is Type ambientType)
                    {
                        type = ambientType;
                    }
                    else
                    {
                        throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}. Cannot find a type.");
                    }
                }
            }

            if (type != null && property != null)
            {
                return DependencyProperty.FromName(property, type);
            }

            throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}.");
        }
    }
}
