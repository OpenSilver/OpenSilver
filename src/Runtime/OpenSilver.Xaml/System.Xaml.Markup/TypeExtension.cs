// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Xaml.Markup;
using OpenSilver.Internal;

namespace System.Windows.Markup;

/// <summary>
/// Implements a markup extension that returns a <see cref="Type"/> based on a string input.
/// </summary>
[TypeConverter(typeof(TypeExtensionConverter))]
[MarkupExtensionReturnType(typeof(Type))]
[ContentProperty(nameof(TypeName))] // workaround until positional parameter are supported in markup extensions
public class TypeExtension : MarkupExtension
{
    private string _typeName;
    private Type _type;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeExtension"/> class.
    /// </summary>
    public TypeExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeExtension"/> class, initializing the <see cref="TypeName"/> 
    /// value based on the provided typeName string.
    /// </summary>
    /// <param name="typeName">
    /// A string that identifies the type to make a reference to. This string uses the format prefix:className. prefix 
    /// is the mapping prefix for a XAML namespace, and is only required to reference types that are not mapped to the 
    /// default XAML namespace.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Attempted to specify typeName as null.
    /// </exception>
    public TypeExtension(string typeName)
    {
        _typeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeExtension"/> class, declaring the type directly.
    /// </summary>
    /// <param name="type">
    /// The type to be represented by this <see cref="TypeExtension"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// type is null
    /// </exception>
    public TypeExtension(Type type)
    {
        _type = type ?? throw new ArgumentNullException(nameof(type));
    }

    /// <summary>
    /// Gets or sets the type information for this extension.
    /// </summary>
    /// <returns>
    /// The established type. For runtime purposes, this may be null for get access, but cannot be set to null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Attempted to set to null.
    /// </exception>
    [DefaultValue(null)]
    [ConstructorArgument("type")]
    public Type Type
    {
        get => _type;
        set
        {
            // Reset the type name so ProvideValue does not use the existing type name.
            _type = value ?? throw new ArgumentNullException(nameof(value));
            _typeName = null;
        }
    }

    /// <summary>
    /// Gets or sets the type name represented by this markup extension.
    /// </summary>
    /// <returns>
    /// A string that identifies the type. This string uses the format prefix:className. (prefix is the mapping prefix
    /// for an XML namespace and is only required to reference types that are not mapped to the default XML namespace 
    /// for WPF (http://schemas.microsoft.com/winfx/2006/xaml/presentation).
    /// </returns>
    public string TypeName
    {
        get => _typeName;
        set
        {
            // Reset the type so ProvideValue does not use the existing type.
            _typeName = value ?? throw new ArgumentNullException(nameof(value));
            _type = null;
        }
    }

    /// <summary>
    /// Returns an object that should be set on the property where this extension is applied. For <see cref="TypeExtension"/>,
    /// this is the <see cref="Type"/> value as evaluated for the requested type name.
    /// </summary>
    /// <param name="serviceProvider">
    /// Object that can provide services for the markup extension. The provider is expected to provide a service for 
    /// <see cref="IXamlTypeResolver"/>.
    /// </param>
    /// <returns>
    /// The <see cref="Type"/> to set on the property where the extension is applied.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// member value for the extension is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Some part of the typeName string did not parse properly. -or- serviceProvider did not provide a service for 
    /// <see cref="IXamlTypeResolver"/> -or- typeName value did not resolve to a type.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// serviceProvider is null.
    /// </exception>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        // If a type was supplied, no context nor type name are needed
        if (_type is not null)
        {
            return _type;
        }

        // Validate the initialization.
        if (_typeName is null)
        {
            throw new InvalidOperationException(Strings.MarkupExtensionTypeName);
        }

        // Get the IXamlTypeResolver from the service provider
        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        if (serviceProvider.GetService(typeof(IXamlTypeResolver)) is not IXamlTypeResolver xamlTypeResolver)
        {
            throw new InvalidOperationException(string.Format(Strings.MarkupExtensionNoContext1, GetType().Name, nameof(IXamlTypeResolver)));
        }

        // Get the type
        _type = xamlTypeResolver.Resolve(_typeName);
        if (_type is null)
        {
            throw new InvalidOperationException(string.Format(Strings.MarkupExtensionTypeNameBad, _typeName));
        }

        return _type;
    }
}
