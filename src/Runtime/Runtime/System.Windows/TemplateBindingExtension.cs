
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

using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Controls;
using OpenSilver.Internal.Xaml;

namespace System.Windows;

/// <summary>
/// Implements a markup extension that supports the binding between the value of a property 
/// in a template and the value of some other exposed property on the templated control.
/// </summary>
[ContentProperty(nameof(Path))]
public class TemplateBindingExtension : MarkupExtension
{
    private DependencyProperty _property;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateBindingExtension"/> class.
    /// </summary>
    public TemplateBindingExtension() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateBindingExtension"/> class with 
    /// the specified dependency property that is the source of the binding.
    /// </summary>
    /// <param name="property">
    /// The identifier of the property being bound.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="property"/> is null.
    /// </exception>
    public TemplateBindingExtension(DependencyProperty property)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateBindingExtension"/> class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TemplateBindingExtension(string path)
    {
        Path = path;
    }

    /// <summary>
    /// Gets or sets the property being bound to.
    /// </summary>
    /// <returns>
    /// Identifier of the dependency property being bound.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> is null.
    /// </exception>
    public DependencyProperty Property
    {
        get => _property;
        set => _property = value ?? throw new ArgumentNullException(nameof(value));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Path { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string DependencyPropertyName { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Type DependencyPropertyOwnerType { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(ITemplateOwnerProvider)) is ITemplateOwnerProvider templateOwnerProvider)
        {
            return ProvideValueImpl(templateOwnerProvider, serviceProvider);
        }

        return LegacyProvideValue(serviceProvider);
    }

    private object ProvideValueImpl(ITemplateOwnerProvider templateOwnerProvider, IServiceProvider serviceProvider)
    {
        if (templateOwnerProvider.GetTemplateOwner() is IInternalControl source)
        {
            DependencyProperty dp = _property;
            if (dp is null)
            {
                if (DependencyPropertyName is not null)
                {
                    Type type = DependencyPropertyOwnerType ?? source.GetType();
                    dp = DependencyProperty.FromName(DependencyPropertyName, type);

                    // Property can be registered on the source type even if defined on a different owner type.
                    if (dp is null && DependencyPropertyName is not null)
                    {
                        dp = DependencyProperty.FromName(DependencyPropertyName, source.GetType());
                    }
                }
                else if (Path is not null)
                {
                    int index = Path.IndexOf('.');
                    if (index > -1)
                    {
                        if (serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver)
                        {
                            string typeName = Path.Substring(0, index);
                            if (typeResolver.Resolve(typeName) is Type type)
                            {
                                string propertyName = Path.Substring(index + 1);
                                dp = DependencyProperty.FromName(propertyName, type);
                            }
                        }
                    }
                    else
                    {
                        Type type = source.GetType();
                        dp = DependencyProperty.FromName(Path, type);
                    }
                }
            }

            if (dp is not null)
            {
                return new TemplateBindingExpression(source, dp);
            }
        }

        return DependencyProperty.UnsetValue;
    }

    private object LegacyProvideValue(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)
        {
            if (provideValueTarget.TargetObject is IInternalControl source)
            {
                if (_property is not DependencyProperty dp)
                {
                    string propertyName = DependencyPropertyName ?? Path;
                    Type type = DependencyPropertyOwnerType ?? source.GetType();
                    dp = DependencyProperty.FromName(propertyName, type);

                    // Property can be registered on the source type even if defined on a different owner type.
                    if (dp is null && DependencyPropertyName is not null)
                    {
                        dp = DependencyProperty.FromName(propertyName, source.GetType());
                    }
                }

                if (dp is not null)
                {
                    return new TemplateBindingExpression(source, dp);
                }
            }
        }

        return DependencyProperty.UnsetValue;
    }
}
