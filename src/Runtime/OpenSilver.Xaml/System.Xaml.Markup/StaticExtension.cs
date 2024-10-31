// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Reflection;
using System.Xaml.Markup;
using OpenSilver.Internal;

namespace System.Windows.Markup;

/// <summary>
/// Implements a markup extension that returns static field and property references.
/// </summary>
[TypeConverter(typeof(StaticExtensionConverter))]
[MarkupExtensionReturnType(typeof(object))]
[ContentProperty(nameof(Member))] // workaround until positional parameter are supported in markup extensions
public class StaticExtension : MarkupExtension
{
    private string _member;
    private Type _memberType;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticExtension"/> class.
    /// </summary>
    public StaticExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticExtension"/> class using the provided member string.
    /// </summary>
    /// <param name="member">
    /// A string that identifies the member to make a reference to. This string uses the format prefix:typeName.fieldOrPropertyName.
    /// prefix is the mapping prefix for a XAML namespace, and is only required to reference static values that 
    /// are not mapped to the default XAML namespace.
    /// </param>
    public StaticExtension(string member)
    {
        _member = member ?? throw new ArgumentNullException(nameof(member));
    }

    /// <summary>
    /// Gets or sets a member name string that is used to resolve a static field or property based on the service-provided type resolver.
    /// </summary>
    /// <returns>
    /// A string that identifies the member to make a reference to.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Attempted to set <see cref="Member"/> to null.
    /// </exception>
    [ConstructorArgument("member")]
    public string Member
    {
        get => _member;
        set => _member = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the <see cref="Type"/> that defines the static member to return.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> that defines the static member to return.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Attempted to set <see cref="MemberType"/> to null.
    /// </exception>
    [DefaultValue(null)]
    public Type MemberType
    {
        get => _memberType;
        set => _memberType = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Returns an object value to set on the property where you apply this extension. For <see cref="StaticExtension"/>,
    /// the return value is the static value that is evaluated for the requested static member.
    /// </summary>
    /// <param name="serviceProvider">
    /// An object that can provide services for the markup extension. The service provider is expected to provide a 
    /// service that implements a type resolver (<see cref="IXamlTypeResolver"/>).
    /// </param>
    /// <returns>
    /// The static value to set on the property where the extension is applied.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The member value for the extension is null at the time of evaluation.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Some part of the member string did not parse properly -or- serviceProvider did not provide a service for 
    /// <see cref="IXamlTypeResolver"/> -or- member value did not resolve to a static member.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// serviceProvider is null.
    /// </exception>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (_member is null)
        {
            throw new InvalidOperationException(Strings.MarkupExtensionStaticMember);
        }

        Type type = MemberType;
        string fieldString;
        string typeNameForError = null;
        if (type != null)
        {
            fieldString = _member;
            typeNameForError = type.FullName;
        }
        else
        {
            // Validate the _member
            int dotIndex = _member.IndexOf('.');
            if (dotIndex < 0)
            {
                throw new ArgumentException(string.Format(Strings.MarkupExtensionBadStatic, _member));
            }

            // Pull out the type substring (this will include any XML prefix, e.g. "av:Button")
            string typeString = _member.Substring(0, dotIndex);
            if (string.IsNullOrEmpty(typeString))
            {
                throw new ArgumentException(string.Format(Strings.MarkupExtensionBadStatic, _member));
            }

            // Get the IXamlTypeResolver from the service provider

            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (serviceProvider.GetService(typeof(IXamlTypeResolver)) is not IXamlTypeResolver xamlTypeResolver)
            {
                throw new ArgumentException(string.Format(Strings.MarkupExtensionNoContext1, GetType().Name, nameof(IXamlTypeResolver)));
            }

            // Use the type resolver to get a Type instance.
            type = xamlTypeResolver.Resolve(typeString);

            // Get the member name substring.
            fieldString = _member.Substring(dotIndex + 1, _member.Length - dotIndex - 1);
            if (string.IsNullOrEmpty(typeString))
            {
                throw new ArgumentException(string.Format(Strings.MarkupExtensionBadStatic, _member));
            }
        }

        // Use the built-in parser for enum types.
        if (type.IsEnum)
        {
            return Enum.Parse(type, fieldString);
        }

        // For other types, reflect.
        if (GetFieldOrPropertyValue(type, fieldString, out object value))
        {
            return value;
        }

        throw new ArgumentException(string.Format(Strings.MarkupExtensionBadStatic, typeNameForError is not null ? $"{typeNameForError}.{_member}" : _member));
    }

    /// <summary>
    /// Return false if a public static field or property with the same
    /// name cannot be found.
    /// </summary>
    private bool GetFieldOrPropertyValue(Type type, string name, out object value)
    {
        Type currentType = type;
        do
        {
            if (currentType.GetField(name, BindingFlags.Public | BindingFlags.Static) is FieldInfo field)
            {
                value = field.GetValue(null);
                return true;
            }

            currentType = currentType.BaseType;
        } while (currentType != null);

        currentType = type;
        do
        {
            if (currentType.GetProperty(name, BindingFlags.Public | BindingFlags.Static) is PropertyInfo prop)
            {
                value = prop.GetValue(null, null);
                return true;
            }

            currentType = currentType.BaseType;
        } while (currentType != null);

        value = null;
        return false;
    }
}
