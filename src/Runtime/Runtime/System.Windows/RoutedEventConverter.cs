// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xaml;

namespace System.Windows.Markup;

/// <summary>
/// Converts a <see cref="RoutedEvent"/> object from a string.
/// </summary>
public sealed class RoutedEventConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of <see cref="RoutedEvent"/>.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="sourceType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if this converter can perform the operation; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
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
    /// Determines whether an instance of <see cref="RoutedEvent"/> can be converted to the specified type.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="destinationType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// Always returns false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType) => false;

    /// <summary>
    /// Attempts to convert the specified object to a <see cref="RoutedEvent"/> object, using the specified context.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="cultureInfo">
    /// Culture specific information.
    /// </param>
    /// <param name="source">
    /// The object to convert.
    /// </param>
    /// <returns>
    /// The conversion result.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// source is not a string or cannot be converted.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
    {
        RoutedEvent routedEvent = null;

        if (source is string routedEventName)
        {
            routedEventName = routedEventName.Trim();

            if (typeDescriptorContext is IServiceProvider serviceProvider)
            {
                Type type = null;

                if (serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver resolver)
                {
                    // Verify that there's at least one period.  (A simple
                    //  but not foolproof check for "[class].[event]")
                    int lastIndex = routedEventName.IndexOf('.');
                    if (lastIndex != -1)
                    {
                        string typeName = routedEventName.Substring(0, lastIndex);
                        routedEventName = routedEventName.Substring(lastIndex + 1);

                        type = resolver.Resolve(typeName);
                    }
                }

                if (type is null)
                {
                    if (typeDescriptorContext.GetService(typeof(IXamlSchemaContextProvider)) is IXamlSchemaContextProvider schemaContextProvider &&
                        serviceProvider.GetService(typeof(IAmbientProvider)) is IAmbientProvider iapp)
                    {
                        XamlSchemaContext schemaContext = schemaContextProvider.SchemaContext;
                        XamlType styleXType = schemaContext.GetXamlType(typeof(Style));
                        XamlType[] ceilingTypes = [styleXType];
                        XamlMember styleTargetType = styleXType.GetMember("TargetType");

                        if (iapp.GetFirstAmbientValue(ceilingTypes, styleTargetType) is AmbientPropertyValue firstAmbientValue)
                        {
                            type = firstAmbientValue.Value as Type;
                        }

                        type ??= typeof(FrameworkElement);
                    }
                }

                if (type is not null)
                {
                    routedEvent = EventManager.GetRoutedEventFromName(routedEventName, type);
                }
            }
        }

        if (routedEvent is null)
        {
            // Falling through here means we are unable to perform the conversion.
            throw GetConvertFromException(source);
        }

        return routedEvent;
    }

    /// <summary>
    /// Attempts to convert a <see cref="RoutedEvent"/> to the specified type. Throws an exception in all cases.
    /// </summary>
    /// <param name="typeDescriptorContext">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="cultureInfo">
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
    /// value cannot be converted. This is not a functioning converter for a save path.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> or <paramref name="destinationType"/> is null.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(destinationType);

        throw GetConvertToException(value, destinationType);
    }
}
