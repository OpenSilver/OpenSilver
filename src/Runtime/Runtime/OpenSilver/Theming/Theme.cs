
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

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using OpenSilver.Internal;

namespace OpenSilver.Theming;

/// <summary>
/// Provides a base class for defining themes.
/// </summary>
public abstract class Theme : IResourceDictionaryOwner
{
    private readonly Cache _cache;
    private WeakReferenceList<Application> _apps;
    private ResourceDictionary _resources;
    private Theme _basedOn;

    /// <summary>
    /// Initializes a new instance of the <see cref="Theme"/> class.
    /// </summary>
    protected Theme()
    {
        _cache = new Cache();
    }

    /// <summary>
    /// Gets or sets the locally-defined resource dictionary.
    /// </summary>
    /// <returns>
    /// The current locally-defined dictionary of resources, where each resource can be accessed by key.
    /// </returns>
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

            // This theme is no longer an owner for the old ResourceDictionary
            oldValue?.RemoveOwner(this);

            if (value != null)
            {
                if (!value.ContainsOwner(this))
                {
                    // This theme is an owner for the new ResourceDictionary
                    value.AddOwner(this);
                }
            }

            // this notify all apps that Theme resources changed
            InvalidateOwners(new ResourcesChangeInfo(oldValue, value));
        }
    }

    /// <summary>
    /// Gets or sets a defined theme that is the basis of the current theme.
    /// </summary>
    /// <returns>
    /// A defined theme that is the basis of the current theme. The default value is null.
    /// </returns>
    public Theme BasedOn
    {
        get { return _basedOn; }
        set
        {
            CheckSealed();

            if (value == this)
            {
                // Basing on self is not allowed. This is a degenerate case of circular reference chain,
                // the full check for circular reference is done in Seal().
                throw new ArgumentException(string.Format(Strings.CannotBeBasedOnSelf, nameof(Theme)));
            }

            if (_basedOn is not null)
            {
                RemoveAllOwnersFromTheme(_basedOn);
            }

            _basedOn = value;

            if (_basedOn is not null)
            {
                AddAllOwnersToTheme(_basedOn);
            }
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the theme is read-only and cannot be changed.
    /// </summary>
    /// <returns>
    /// true if the theme is sealed; otherwise, false.
    /// </returns>
    public bool IsSealed { get; private set; }

    /// <summary>
    /// Locks the theme so it cannot be changed.
    /// </summary>
    public void Seal()
    {
        if (IsSealed)
        {
            return;
        }

        CheckForCircularBasedOnReferences();

        _basedOn?.Seal();

        IsSealed = true;

        OnSealed();
    }

    /// <summary>
    /// Throws an exception if the theme has already been sealed.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The theme has already been sealed.
    /// </exception>
    protected void CheckSealed()
    {
        if (IsSealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(Theme)));
        }
    }

    /// <summary>
    /// Invoked after this theme becomes sealed.
    /// </summary>
    protected virtual void OnSealed() { }

    /// <summary>
    /// Generates resources for a specific assembly.
    /// </summary>
    /// <param name="assembly">
    /// The assembly to generate resources for.
    /// </param>
    /// <returns>
    /// A <see cref="ResourceDictionary"/> that contains resources for <paramref name="assembly"/>.
    /// </returns>
    protected abstract ResourceDictionary GenerateResources(Assembly assembly);

    internal void AddOwner(Application application)
    {
        Debug.Assert(_apps is null || !_apps.Contains(application));

        _apps ??= new(1);
        _apps.Add(application);

        _basedOn?.AddOwner(application);
    }

    internal void RemoveOwner(Application application)
    {
        Debug.Assert(_apps is not null && _apps.Contains(application));

        _apps.Remove(application);
        if (_apps.Count == 0)
        {
            _apps = null;
        }

        _basedOn?.RemoveOwner(application);
    }

    private void AddAllOwnersToTheme(Theme theme)
    {
        if (_apps is not null)
        {
            foreach (Application app in _apps)
            {
                theme.AddOwner(app);
            }
        }
    }

    private void RemoveAllOwnersFromTheme(Theme theme)
    {
        if (_apps is not null)
        {
            foreach (Application app in _apps)
            {
                theme.RemoveOwner(app);
            }
        }
    }

    void IResourceDictionaryOwner.SetResources(ResourceDictionary resourceDictionary) { }

    void IResourceDictionaryOwner.OnResourcesChange(ResourcesChangeInfo info, bool shouldInvalidate, bool hasImplicitStyles)
    {
        if (shouldInvalidate)
        {
            InvalidateOwners(info);
        }
    }

    private void InvalidateOwners(ResourcesChangeInfo info)
    {
        if (_apps is not null)
        {
            foreach (Application app in _apps)
            {
                app.InvalidateResourceReferences(info);
            }
        }
    }

    internal bool TryGetResource(object resourceKey, out object resource)
    {
        if (_resources is not null && _resources.TryGetResource(resourceKey, out resource))
        {
            return true;
        }
        if (_basedOn is not null)
        {
            return _basedOn.TryGetResource(resourceKey, out resource);
        }
        resource = null;
        return false;
    }

    internal object GetTypedResource(Type typeKey) => _cache.GetOrAddResource(typeKey, FindDictionaryResource);

    private object FindDictionaryResource(Type typeKey)
    {
        Debug.Assert(typeKey is not null);

        if (GetOrCreateResourceDictionary(typeKey.Assembly) is ResourceDictionary resources)
        {
            return FetchResource(resources, typeKey);
        }

        return null;
    }

    private ResourceDictionary GetOrCreateResourceDictionary(Assembly assembly)
        => _cache.GetOrAddResourceDictionary(assembly, CreateResourceDictionary);

    private ResourceDictionary CreateResourceDictionary(Assembly assembly)
    {
        if (GenerateAssemblyResources(assembly) is ResourceDictionary resources)
        {
            return resources;
        }

        if (_basedOn is Theme basedOn)
        {
            return basedOn.GetOrCreateResourceDictionary(assembly);
        }

        return null;
    }

    private ResourceDictionary GenerateAssemblyResources(Assembly assembly)
    {
        ResourceDictionary resources = null;

        XamlResources.IsSystemResourcesParsing = true;

        try
        {
            resources = GenerateResources(assembly);
        }
        finally
        {
            XamlResources.IsSystemResourcesParsing = false;
        }

        return resources;
    }

    private static object FetchResource(ResourceDictionary resources, object key)
    {
        object resource = resources[key];

        if (resource is Style style)
        {
            style.Seal();

            // Check if the theme style has the OverridesDefaultStyle property set on any of its setters.
            // It is an error to specify the OverridesDefaultStyle in your own ThemeStyle.
            if (style.EffectiveValues.ContainsKey(FrameworkElement.OverridesDefaultStyleProperty.GlobalIndex))
            {
                throw new InvalidOperationException(Strings.CannotHaveOverridesDefaultStyleInThemeStyle);
            }
        }

        return resource;
    }

    /// <summary>
    /// This method checks to see if the BasedOn hierarchy contains a loop in the chain of references.
    /// </summary>
    private void CheckForCircularBasedOnReferences()
    {
        if (HasCircularBasedOnReferences(this))
        {
            // We've seen this Theme before. This means the BasedOn hierarchy contains a loop.
            throw new InvalidOperationException(string.Format(Strings.BasedOnHasLoop, nameof(Theme)));
        }

        // This does not really check for circular reference in all circumstances. This is accurate
        // only if the basedOn themes have no circular references. In our case, it is safe because we
        // seal basedOn themes first.
        static bool HasCircularBasedOnReferences(Theme theme)
        {
            for (Theme basedOn = theme._basedOn; basedOn is not null; basedOn = basedOn._basedOn)
            {
                if (basedOn == theme)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private sealed class Cache
    {
        private readonly ConcurrentDictionary<Assembly, ResourceDictionary> _dictionaries = new();
        private readonly ConcurrentDictionary<Type, object> _resources = new();

        public ResourceDictionary GetOrAddResourceDictionary(Assembly assembly, Func<Assembly, ResourceDictionary> valueFactory)
            => _dictionaries.GetOrAdd(assembly, valueFactory);

        public object GetOrAddResource(Type typeKey, Func<Type, object> valueFactory) => _resources.GetOrAdd(typeKey, valueFactory);
    }
}
