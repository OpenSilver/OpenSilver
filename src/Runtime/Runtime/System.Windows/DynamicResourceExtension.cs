
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

using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml.Markup;

namespace System.Windows;

/// <summary>
/// Implements a markup extension that supports dynamic resource references made from XAML.
/// </summary>
[ContentProperty(nameof(ResourceKey))]
public class DynamicResourceExtension : MarkupExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicResourceExtension"/> class.
    /// </summary>
    public DynamicResourceExtension() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicResourceExtension"/> class,
    /// with the provided initial key.
    /// </summary>
    /// <param name="resourceKey">
    /// The key of the resource that this markup extension references.
    /// </param>
    public DynamicResourceExtension(object resourceKey)
    {
        _resourceKey = resourceKey ?? throw new ArgumentNullException(nameof(resourceKey));
    }

    /// <summary>
    /// Returns an object that should be set on the property where this extension is
    /// applied. For <see cref="DynamicResourceExtension"/>, this is the object found
    /// in a resource dictionary in the current parent chain that is keyed by the 
    /// <see cref="ResourceKey"/>.
    /// </summary>
    /// <param name="serviceProvider">
    /// Object that can provide services for the markup extension.
    /// </param>
    /// <returns>
    /// The object to set on the property where the extension is applied. Rather than the 
    /// actual value, this will be an expression that will be evaluated at a later time.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Attempted to provide a value for an extension that did not provide a resourceKey.
    /// </exception>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (ResourceKey is null)
        {
            throw new InvalidOperationException("Resource markup extension must have ResourceKey property set before ProvideValue can be called.");
        }

        if (serviceProvider is not null)
        {
            // DynamicResourceExtensions are not allowed On CLR props except for Setter
            BindingBase.CheckCanReceiveMarkupExtension(this, serviceProvider, out _, out _);
        }

        return new ResourceReferenceExpression(ResourceKey);
    }

    /// <summary>
    /// Gets or sets the key specified by this dynamic resource reference. The key is
    /// used to lookup a resource in resource dictionaries, by means of an intermediate
    /// expression.
    /// </summary>
    /// <returns>
    /// The resource key that this dynamic resource reference specifies.
    /// </returns>
    [ConstructorArgument("resourceKey")]
    public object ResourceKey
    {
        get => _resourceKey;
        set => _resourceKey = value ?? throw new ArgumentNullException(nameof(value));
    }

    private object _resourceKey;
}
