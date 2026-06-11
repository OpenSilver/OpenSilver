
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
        ArgumentNullException.ThrowIfNull(property);

        _property = property;
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

    /// <summary>
    /// Gets or sets the converter to apply to the bound value (WPF compatibility).
    /// </summary>
    public Data.IValueConverter Converter { get; set; }

    /// <summary>
    /// Gets or sets the parameter passed to the <see cref="Converter"/>.
    /// </summary>
    public object ConverterParameter { get; set; }

    /// <summary>
    /// Gets or sets the culture passed to the <see cref="Converter"/>.
    /// </summary>
    public Globalization.CultureInfo ConverterCulture { get; set; }

    private TemplateBindingExpression CreateExpression(IInternalControl source, DependencyProperty dp)
    {
        var expression = new TemplateBindingExpression(source, dp);
        if (Converter is not null)
        {
            expression.Converter = Converter;
            expression.ConverterParameter = ConverterParameter;
            expression.ConverterCulture = ConverterCulture;
        }
        return expression;
    }

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
                return CreateExpression(source, dp);
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
                }

                if (dp is not null)
                {
                    return CreateExpression(source, dp);
                }
            }
        }

        return DependencyProperty.UnsetValue;
    }
}
