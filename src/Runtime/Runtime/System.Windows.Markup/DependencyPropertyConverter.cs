
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
using System.Linq;
using System.Reflection;
using System.Xaml;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace System.Windows.Markup
{
    /// <summary>
    /// Class for converting a given DependencyProperty to and from a string
    /// </summary>
    public sealed class DependencyPropertyConverter : TypeConverter
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
                throw new ArgumentNullException("context");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
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

            // Still don't have a Type so we need to loop up the chain and grab either Style.TargetType,
            // DataTemplate.DataType, or ControlTemplate.TargetType
            if (type == null)
            {
                if (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) is not IXamlSchemaContextProvider ixscp)
                {
                    throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}. IXamlSchemaContextProvider is null.");
                }
                XamlSchemaContext schemaContext = ixscp.SchemaContext;

                XamlType styleXType = schemaContext.GetXamlType(typeof(Style));
                XamlType frameworkTemplateXType = schemaContext.GetXamlType(typeof(FrameworkTemplate));
                XamlType dataTemplateXType = schemaContext.GetXamlType(typeof(DataTemplate));
                XamlType controlTemplateXType = schemaContext.GetXamlType(typeof(ControlTemplate));

                List<XamlType> ceilingTypes = new()
                {
                    //styleXType,
                    frameworkTemplateXType,
                    dataTemplateXType,
                    controlTemplateXType
                };

                // We don't look for DataTemplate's DataType since we want to use the TargetTypeInternal instead
                XamlMember styleTargetType = styleXType.GetMember("TargetType");
                XamlMember templateProperty = frameworkTemplateXType.GetMember("Template");
                XamlMember controlTemplateTargetType = controlTemplateXType.GetMember("TargetType");

                if (serviceProvider.GetService(typeof(IAmbientProvider)) is not IAmbientProvider ambientProvider)
                {
                    throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}. IAmbientProvider is null.");
                }
                AmbientPropertyValue firstAmbientValue = ambientProvider.GetFirstAmbientValue(ceilingTypes, styleTargetType, templateProperty, controlTemplateTargetType);

                if (firstAmbientValue != null)
                {
                    if (firstAmbientValue.Value is Type ambientType)
                    {
                        type = ambientType;
                    }
                    else if (firstAmbientValue.Value is TemplateContent tempContent)
                    {
                        // todo: implement OwnerTemplate or look for DataTemplate's DataType
                        //type = tempContent.OwnerTemplate.TargetTypeInternal;
                        throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}. Getting type from DataTemplate is not implemented yet.");
                    }
                    else
                    {
                        throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}. Cannot find a type.");
                    }
                }
            }

            if (type != null && property != null)
            {
                return FromName(property, type);
            }

            throw new NotSupportedException($"Cannot convert {source} to {typeof(DependencyProperty).FullName}.");
        }

        /// <summary>
        /// Find a property from name
        /// </summary>
        /// <remarks>
        ///     Search includes base classes of the provided type as well
        /// </remarks>
        /// <param name="name">Name of the property</param>
        /// <param name="ownerType">Owner type of the property</param>
        /// <returns>Dependency property</returns>
        private static DependencyProperty FromName(string name, Type ownerType)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (ownerType is null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            DependencyProperty dp = null;

            while ((dp == null) && (ownerType != null))
            {
                dp = ownerType.GetFields(BindingFlags.Public | BindingFlags.Static)
                              .Where(x => x.FieldType == typeof(DependencyProperty))
                              .Select(x => (DependencyProperty)x.GetValue(null))
                              .FirstOrDefault(x => x.Name == name);
                ownerType = ownerType.BaseType;
            }

            return dp;
        }
    }
}

