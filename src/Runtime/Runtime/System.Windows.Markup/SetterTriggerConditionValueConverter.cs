
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
using System.Xaml;
using System.Xaml.Schema;
using OpenSilver.Internal;

namespace System.Windows.Markup;

/// <summary>
/// Provides type conversion analogous behavior for <see cref="Setter"/>, <see cref="Trigger"/> and 
/// <see cref="Condition"/> types that deal with DependencyProperty values. This converter only supports 
/// ConvertFrom.
/// </summary>
public sealed class SetterTriggerConditionValueConverter : TypeConverter
{
    /// <summary>
    /// Returns a value that indicates whether the converter can convert from a source object to a 
    /// side-effect-produced <see cref="Setter"/>, <see cref="Trigger"/> or <see cref="Condition"/>.
    /// </summary>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="sourceType">
    /// The type to convert from.
    /// </param>
    /// <returns>
    /// true if the converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // We can only convert from a string and that too only if we have all the contextual information
        // Note: Sometimes even the serializer calls CanConvertFrom in order 
        // to determine if it is a valid converter to use for serialization.
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Returns a value that indicates whether the converter can convert to the specified destination 
    /// type. Always returns false.
    /// </summary>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert to.
    /// </param>
    /// <returns>
    /// Always returns false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => false;

    /// <summary>
    /// Converts the converted source value if an underlying type converter can be obtained from context.
    /// Otherwise returns an unconverted source.
    /// </summary>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> to use as the current culture.
    /// </param>
    /// <param name="source">
    /// The object to convert.
    /// </param>
    /// <returns>
    /// The converter object, or possibly an unconverted source.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="context"/> or <paramref name="source"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// No <see cref="IXamlSchemaContextProvider"/> service available.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
    {
        return ResolveValue(context, null, culture, source);
    }

    /// <summary>
    /// Converts the specified object to the specified type. Always throws an exception.
    /// </summary>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> to use as the current culture.
    /// </param>
    /// <param name="value">
    /// The object to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert to.
    /// </param>
    /// <returns>
    /// Always throws an exception.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown in all cases.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        throw GetConvertToException(value, destinationType);
    }

    internal static object ResolveValue(ITypeDescriptorContext serviceProvider,
        DependencyProperty property, CultureInfo culture, object source)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(source);

        // Only need to type convert strings and byte[]
        if (source is not string && source is not byte[])
        {
            return source;
        }

        if (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) is not IXamlSchemaContextProvider ixsc)
        {
            throw NotSupported();
        }

        XamlSchemaContext schemaContext = ixsc.SchemaContext;

        if (property is not null)
        {
            // Get XamlMember from dp
            XamlMember xamlProperty = schemaContext.GetXamlType(property.OwnerType).GetMember(property.Name)
                ?? schemaContext.GetXamlType(property.OwnerType).GetAttachableMember(property.Name);

            XamlValueConverter<TypeConverter> typeConverter;
            if (xamlProperty is not null)
            {
                typeConverter = xamlProperty.TypeConverter ?? xamlProperty.Type.TypeConverter;
            }
            else
            {
                typeConverter = schemaContext.GetXamlType(property.PropertyType).TypeConverter;
            }

            // No Type converter case...
            if (typeConverter.ConverterType is null)
            {
                return source;
            }

            TypeConverter converter;

            if (xamlProperty is not null && xamlProperty.Type.UnderlyingType == typeof(bool))
            {
                if (source is string)
                {
                    converter = new BooleanConverter();
                }
                else if (source is byte[] bytes)
                {
                    if (bytes.Length != 1)
                    {
                        throw NotSupported();
                    }

                    return bytes[0] != 0;
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

        return source;

        static NotSupportedException NotSupported()
        {
            return new NotSupportedException(
                string.Format(Strings.ParserCannotConvertPropertyValue, "Value", typeof(object).FullName));
        }
    }
}
