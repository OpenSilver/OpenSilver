
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

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows;

public partial class FrameworkElement
{
    private ResourceDictionary _resources;

    /// <summary>
    /// Gets the locally defined resource dictionary. In XAML, you can establish
    /// resource items as child object elements of a frameworkElement.Resources property
    /// element, through XAML implicit collection syntax.
    /// </summary>
    [Ambient]
    public ResourceDictionary Resources
    {
        get
        {
            if (_resources is null)
            {
                _resources = new ResourceDictionary();
                _resources.AddOwner(this);
            }
            return _resources;
        }
        set
        {
            if (_resources == value) return;

            ResourceDictionary oldValue = _resources;
            _resources = value;

            // This element is no longer an owner for the old RD
            oldValue?.RemoveOwner(this);

            if (value != null)
            {
                if (!value.ContainsOwner(this))
                {
                    // This element is an owner for the new RD
                    value.AddOwner(this);
                }
            }

            // Invalidate ResourceReference properties for this subtree
            TreeWalkHelper.InvalidateOnResourcesChange(this, new ResourcesChangeInfo(oldValue, value));
        }
    }

    /// <summary>
    ///     Check if resource is not empty.
    ///     Call HasResources before accessing resources every time you need
    ///     to query for a resource.
    /// </summary>
    internal bool HasResources => _resources is not null && !_resources.IsEmpty;

    void IResourceDictionaryOwner.SetResources(ResourceDictionary resourceDictionary)
    {
        // Propagate the HasImplicitStyles flag to the new owner
        if (resourceDictionary.HasImplicitStyles)
        {
            ShouldLookupImplicitStyles = true;
        }
    }

    void IResourceDictionaryOwner.OnResourcesChange(ResourcesChangeInfo info, bool shouldInvalidate, bool hasImplicitStyles)
    {
        // Set the HasImplicitStyles flag on the owner
        if (hasImplicitStyles)
        {
            ShouldLookupImplicitStyles = true;
        }

        // If this dictionary has been initialized fire an invalidation
        // to let the tree know of this change.
        if (shouldInvalidate)
        {
            TreeWalkHelper.InvalidateOnResourcesChange(this, info);
        }
    }

    /// <summary>
    /// Searches for a resource with the specified name and sets up a resource reference to it for the specified property.
    /// </summary>
    /// <param name="dp">
    /// The property to which the resource is bound.
    /// </param>
    /// <param name="name">
    /// The name of the resource.
    /// </param>
    /// <remarks>
    /// A resource reference is similar to the use of a <see cref="DynamicResourceExtension"/> Markup Extension in markup.
    /// The resource reference creates an internal expression that supplies the value of the specified property on a run-time 
    /// deferred basis. The expression will be re-evaluated whenever the resource dictionary indicates a changed value through 
    /// internal events, or whenever the current element is reparented (a parent change would change the dictionary lookup path).
    /// </remarks>
    public void SetResourceReference(DependencyProperty dp, object name) => SetValue(dp, new ResourceReferenceExpression(name));

    /// <summary>
    /// Searches for a resource with the specified key, and throws an exception if the requested resource is not found.
    /// </summary>
    /// <param name="resourceKey">
    /// The key identifier for the requested resource.
    /// </param>
    /// <returns>
    /// The requested resource. If no resource with the provided key was found, an exception is thrown. A <see cref="DependencyProperty.UnsetValue"/>
    /// value might also be returned in the exception case.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// resourceKey is null.
    /// </exception>
    /// <exception cref="ResourceReferenceKeyNotFoundException">
    /// resourceKey was not found.
    /// </exception>
    public object FindResource(object resourceKey)
    {
        ArgumentNullException.ThrowIfNull(resourceKey);

        object resource = FindResourceInternal(this, null, resourceKey, null, false);

        if (resource == DependencyProperty.UnsetValue)
        {
            // Resource not found in parent chain, app or system
            throw new ResourceReferenceKeyNotFoundException(Strings.MarkupExtensionResourceNotFound, resourceKey);
        }

        return resource;
    }

    /// <summary>
    /// Searches for a resource with the specified key, and returns that resource if found.
    /// </summary>
    /// <param name="resourceKey">
    /// The key identifier of the resource to be found.
    /// </param>
    /// <returns>
    /// The found resource, or null if no resource with the provided key is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// resourceKey is null.
    /// </exception>
    public object TryFindResource(object resourceKey)
    {
        ArgumentNullException.ThrowIfNull(resourceKey);

        object resource = FindResourceInternal(this, null, resourceKey, null, false);

        if (resource == DependencyProperty.UnsetValue)
        {
            // Resource not found in parent chain, app or system
            // This is where we translate DependencyProperty.UnsetValue to a null
            resource = null;
        }

        return resource;
    }

    internal static object FindResourceFromAppOrSystem(object resourceKey) => FindResourceInternal(null, null, resourceKey, null, false);

    internal static object FindResourceInternal(
        FrameworkElement fe,
        DependencyProperty dp,
        object resourceKey,
        DependencyObject boundaryElement,
        bool isImplicitStyleLookup)
    {
        object value;
        InheritanceBehavior inheritanceBehavior = InheritanceBehavior.Default;

        if (fe is not null)
        {
            // First try to find the resource in the tree
            value = FindResourceInTree(
                fe,
                dp,
                resourceKey,
                boundaryElement,
                out inheritanceBehavior);

            if (value != DependencyProperty.UnsetValue)
            {
                return value;
            }
        }

        if ((inheritanceBehavior == InheritanceBehavior.Default || inheritanceBehavior == InheritanceBehavior.SkipToAppNow) &&
            (!isImplicitStyleLookup || boundaryElement is null)) // Silverlight does not try to find an implicit style in
                                                                 // the app resources when there is a boundary element
        {
            // Then we try to find the resource in the App's Resources
            if (Application.Current is Application app)
            {
                value = isImplicitStyleLookup ? app.FindImplicitResource(resourceKey) : app.FindResourceInternal(resourceKey);
                if (value is not null)
                {
                    return value;
                }
            }
        }

        // Then we try to find the resource in the SystemResources but that is only if we aren't
        // doing an implicit style lookup. Implicit style lookup will stop at the app.
        if (!isImplicitStyleLookup &&
            inheritanceBehavior != InheritanceBehavior.SkipAllNow)
        {
            value = XamlResources.FindResourceInGenericXaml(resourceKey);
            if (value is not null)
            {
                return value;
            }
        }

        return DependencyProperty.UnsetValue;
    }

    internal static object FindResourceInTree(
        FrameworkElement feStart,
        DependencyProperty dp,
        object resourceKey,
        DependencyObject boundaryElement,
        out InheritanceBehavior inheritanceBehavior)
    {
        FrameworkElement fe = feStart;
        inheritanceBehavior = InheritanceBehavior.Default;
        object value;
        Style style;
        FrameworkTemplate frameworkTemplate;
        Style themeStyle;
        int loopCount = 0;

        while (fe is not null)
        {
            if (loopCount > LayoutManager.LayoutRecursionLimit)
            {
                // We suspect a loop here because the loop count
                // has exceeded the MAX_TREE_DEPTH expected
                throw new InvalidOperationException(Strings.LogicalTreeLoop);
            }
            else
            {
                loopCount++;
            }

            inheritanceBehavior = fe.ResourceLookupMode;
            if (inheritanceBehavior != InheritanceBehavior.Default)
            {
                break;
            }

            // -------------------------------------------
            //  Lookup ResourceDictionary on the current instance
            // -------------------------------------------

            style = null;
            frameworkTemplate = null;
            themeStyle = null;

            value = fe.FindResourceOnSelf(resourceKey);
            if (value != DependencyProperty.UnsetValue)
            {
                return value;
            }

            if (fe != feStart || StyleHelper.ShouldGetValueFromStyle(dp))
            {
                style = fe.Style;
            }
            // Fetch the Template
            if (fe != feStart || StyleHelper.ShouldGetValueFromTemplate(dp))
            {
                frameworkTemplate = fe.TemplateInternal;
            }
            // Fetch the ThemeStyle
            if (fe != feStart || StyleHelper.ShouldGetValueFromThemeStyle(dp))
            {
                themeStyle = fe.ThemeStyle;
            }

            if (style is not null)
            {
                value = style.FindResource(resourceKey);
                if (value != DependencyProperty.UnsetValue)
                {
                    return value;
                }
            }

            if (frameworkTemplate is not null)
            {
                value = frameworkTemplate.FindResource(resourceKey);
                if (value != DependencyProperty.UnsetValue)
                {
                    return value;
                }
            }

            if (themeStyle is not null)
            {
                value = themeStyle.FindResource(resourceKey);
                if (value != DependencyProperty.UnsetValue)
                {
                    return value;
                }
            }

            fe = (fe.Parent ?? VisualTreeHelper.GetParent(fe)) as FrameworkElement;

            // If the current element that has been searched is the boundary element
            // then we need to progress no further
            if (boundaryElement is not null && fe == boundaryElement)
            {
                break;
            }
        }

        return DependencyProperty.UnsetValue;
    }

    internal object FindResourceOnSelf(object resourceKey)
    {
        if (_resources is not null && _resources.TryGetResource(resourceKey, out object value))
        {
            return value;
        }
        return DependencyProperty.UnsetValue;
    }

    internal event EventHandler ResourcesChanged;

    private void OnResourcesChanged(ResourcesChangeInfo info)
    {
        bool containsTypeOfKey = info.Contains(DependencyObjectType.SystemType, true /*isImplicitStyleKey*/);

        // If a resource dictionary changed above this node then we need to
        // synchronize the ShouldLookupImplicitStyles flag with respect to
        // our parent here.

        if (info.IsResourceAddOperation || info.IsCatastrophicDictionaryChange)
        {
            SetShouldLookupImplicitStyles();
        }

        // Invalidate implicit and explicit resource references on current instance

        // if the change affects implicit data templates, notify ContentPresenters
        if (info.IsImplicitDataTemplateChange)
        {
            if (this is ContentPresenter contentPresenter)
            {
                contentPresenter.ReevaluateTemplate();
            }
        }

        if (containsTypeOfKey)
        {
            HasStyleInvalidated = false;
            UpdateStyleProperty();
        }

        ResourcesChanged?.Invoke(this, new ResourcesChangedEventArgs(info));
    }
}
