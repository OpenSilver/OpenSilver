
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
public abstract class Theme
{
    private readonly Resources _resources;
    private Theme _basedOn;

    /// <summary>
    /// Initializes a new instance of the <see cref="Theme"/> class.
    /// </summary>
    protected Theme()
    {
        _resources = new Resources();
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
            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(Theme)));
            }

            if (value == this)
            {
                // Basing on self is not allowed. This is a degenerate case of circular reference chain,
                // the full check for circular reference is done in Seal().
                throw new ArgumentException(string.Format(Strings.CannotBeBasedOnSelf, nameof(Theme)));
            }

            _basedOn = value;
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

    internal object GetResource(Type typeKey) => _resources.GetOrAddResource(typeKey, FindDictionaryResource);

    private object FindDictionaryResource(Type typeKey)
    {
        Debug.Assert(typeKey is not null);

        ResourceDictionary resources = GetOrCreateResourceDictionary(typeKey.Assembly);
        return resources?[typeKey];
    }

    private ResourceDictionary GetOrCreateResourceDictionary(Assembly assembly)
        => _resources.GetOrAddResourceDictionary(assembly, CreateResourceDictionary);

    private ResourceDictionary CreateResourceDictionary(Assembly assembly)
    {
        if (GenerateAssemblyResources(assembly) is ResourceDictionary resources)
        {
            return resources;
        }

        if (BasedOn is Theme basedOn)
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

    private sealed class Resources
    {
        private readonly ConcurrentDictionary<Assembly, ResourceDictionary> _resourceDictionaries = new();
        private readonly ConcurrentDictionary<Type, object> _resourcesCache = new();

        public ResourceDictionary GetOrAddResourceDictionary(Assembly assembly, Func<Assembly, ResourceDictionary> valueFactory)
            => _resourceDictionaries.GetOrAdd(assembly, valueFactory);

        public object GetOrAddResource(Type typeKey, Func<Type, object> valueFactory) => _resourcesCache.GetOrAdd(typeKey, valueFactory);
    }
}
