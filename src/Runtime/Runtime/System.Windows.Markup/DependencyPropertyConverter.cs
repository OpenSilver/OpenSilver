
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

using OpenSilver.Internal;
using OpenSilver.Internal.Xaml;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Xaml;

namespace System.Windows.Markup;

/// <summary>
/// Converts from a string to a <see cref="DependencyProperty"/> object.
/// </summary>
internal sealed class DependencyPropertyConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of 
    /// <see cref="DependencyProperty"/>.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter 
    /// is being invoked.
    /// </param>
    /// <param name="sourceType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this converter can perform the operation; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // We can only convert from a string and that too only if we have all the contextual information
        // Note: Sometimes even the serializer calls CanConvertFrom in order 
        // to determine if it is a valid converter to use for serialization.
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Determines whether an instance of <see cref="DependencyProperty"/> can be converted to the 
    /// specified type.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter 
    /// is being invoked.
    /// </param>
    /// <param name="destinationType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// Always returns false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => false;

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="DependencyProperty"/>, using the 
    /// specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter 
    /// is being invoked.
    /// </param>
    /// <param name="culture">
    /// Culture specific information.
    /// </param>
    /// <param name="source">
    /// The object to convert.
    /// </param>
    /// <returns>
    /// The converted object. If the conversion is successful, this is a <see cref="DependencyProperty"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="context"/> or <paramref name="source"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="source"/> cannot be converted.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(source);

        if (ResolveProperty(context, null, source) is DependencyProperty property)
        {
            return property;
        }
        else
        {
            throw GetConvertFromException(source);
        }
    }

    /// <summary>
    /// Attempts to convert a <see cref="DependencyProperty"/> to the specified type, using the specified 
    /// context. Always throws an exception.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter 
    /// is being invoked.
    /// </param>
    /// <param name="culture">
    /// Culture specific information.
    /// </param>
    /// <param name="value">
    /// The object to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert the object to.
    /// </param>
    /// <returns>
    /// Always throws an exception.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// In all cases.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        throw GetConvertToException(value, destinationType);
    }

    internal static DependencyProperty ResolveProperty(IServiceProvider serviceProvider, string targetName, object source)
    {
        if (source is DependencyProperty dProperty)
        {
            return dProperty;
        }

        Type type = null;
        string property;

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
            throw NotSupported();
        }

        // We got additional info from either Trigger.SourceName or Setter.TargetName
        if (type is null && targetName is not null)
        {
            IAmbientProvider ambientProvider = serviceProvider.GetService(typeof(IAmbientProvider))
                as IAmbientProvider;
            XamlSchemaContext schemaContext = (serviceProvider.GetService(typeof(IXamlSchemaContextProvider))
                as IXamlSchemaContextProvider).SchemaContext;

            type = GetTypeFromName(schemaContext, ambientProvider, targetName);
        }

        // Still don't have a Type so we need to loop up the chain and grab Style.TargetType,
        if (type is null)
        {
            if (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) is not IXamlSchemaContextProvider ixscp)
            {
                throw NotSupported();
            }

            XamlSchemaContext schemaContext = ixscp.SchemaContext;

            XamlType styleXType = schemaContext.GetXamlType(typeof(Style));
            XamlType frameworkTemplateXType = schemaContext.GetXamlType(typeof(FrameworkTemplate));
            XamlType dataTemplateXType = schemaContext.GetXamlType(typeof(DataTemplate));
            XamlType controlTemplateXType = schemaContext.GetXamlType(typeof(ControlTemplate));

            XamlType[] ceilingTypes = [styleXType, frameworkTemplateXType, dataTemplateXType, controlTemplateXType];

            XamlMember styleTargetType = styleXType.GetMember(nameof(Style.TargetType));
            XamlMember templateProperty = frameworkTemplateXType.GetMember(nameof(FrameworkTemplate.Template));
            XamlMember controlTemplateTargetType = controlTemplateXType.GetMember(nameof(ControlTemplate.TargetType));

            if (serviceProvider.GetService(typeof(IAmbientProvider)) is not IAmbientProvider ambientProvider)
            {
                throw NotSupported();
            }

            AmbientPropertyValue firstAmbientValue = ambientProvider.GetFirstAmbientValue(ceilingTypes,
                styleTargetType, templateProperty, controlTemplateTargetType);

            if (firstAmbientValue is not null)
            {
                if (firstAmbientValue.Value is Type ambientType)
                {
                    type = ambientType;
                }
                else if (firstAmbientValue.Value is XamlTemplateContent templateContent)
                {
                    type = templateContent.OwnerTemplate.TargetTypeInternal;
                }
                else
                {
                    throw NotSupported();
                }
            }
        }

        if (type is not null && property is not null)
        {
            return DependencyProperty.FromName(property, type);
        }

        throw NotSupported();

        static NotSupportedException NotSupported()
        {
            return new NotSupportedException(
                string.Format(Strings.ParserCannotConvertPropertyValue, "Property", typeof(DependencyProperty).FullName));
        }
    }

    // Setters and triggers may have a sourceName which we need to resolve
    // This only works in templates and it works by looking up the mapping between 
    // name and type in the template.  We use ambient lookup to find the Template property
    // and then query it for the type.
    private static Type GetTypeFromName(XamlSchemaContext schemaContext, IAmbientProvider ambientProvider, string target)
    {
        XamlType frameworkTemplateXType = schemaContext.GetXamlType(typeof(FrameworkTemplate));
        XamlMember templateProperty = frameworkTemplateXType.GetMember(nameof(FrameworkTemplate.Template));

        AmbientPropertyValue ambientValue = ambientProvider.GetFirstAmbientValue([frameworkTemplateXType], templateProperty);

        if (ambientValue.Value is XamlTemplateContent templateContent)
        {
            return templateContent.GetTypeForName(target).UnderlyingType;
        }

        return null;
    }
}
