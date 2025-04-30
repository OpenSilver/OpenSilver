
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace OpenSilver.Internal.ComponentModel;

/// <summary>
///     A type description provider provides metadata for types.  It allows a type
///     to define its own semantic layer for properties, events and attributes.
///     
///     Note: This class can stay internal.  To utilize it, the following 
///     metadata attribute should be added to DependencyObject:
///     
///     [TypeDescriptionProvider(typeof(DependencyObjectProvider))]
///     public class DependencyObject {}
/// </summary>
internal sealed class DependencyObjectProvider : TypeDescriptionProvider
{
    /// <summary>
    ///     The ctor for this class needs to be public because it is created
    ///     by TypeDescriptor using reflection.
    /// </summary>
    public DependencyObjectProvider()
        : base(TypeDescriptor.GetProvider(typeof(DependencyObject)))
    {
        // We keep a lot of caches around.  When TypeDescriptor gets a refresh
        // we clear our caches.  We only need to do this if the refresh
        // contains type information, because we only keep static per-type
        // caches.

        TypeDescriptor.Refreshed += delegate (RefreshEventArgs args)
        {
            if (args.TypeChanged != null && typeof(DependencyObject).IsAssignableFrom(args.TypeChanged))
            {
                ClearCache();
                DependencyObjectPropertyDescriptor.ClearCache();
                DPCustomTypeDescriptor.ClearCache();
                DependencyPropertyDescriptor.ClearCache();
            }
        };
    }

    /// <summary>
    ///     Returns a custom type descriptor suitable for querying about the
    ///     given object type and instance.
    /// </summary>
    public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) =>
        new DPCustomTypeDescriptor(base.GetTypeDescriptor(objectType, instance), objectType, instance);

    /// <summary>
    ///     Returns a custom type descriptor suitable for querying about "extended"
    ///     properties.  Extended properties are are attached properties in our world.
    /// </summary>
    public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
    {
        ICustomTypeDescriptor descriptor = base.GetExtendedTypeDescriptor(instance);

        // It is possible that a Type object worked its way in here as an instance.
        // If it did, we don't need our own descriptor because we don't support
        // attached properties on type instances.

        if (instance != null && instance is not Type)
        {
            descriptor = new APCustomTypeDescriptor(descriptor, instance);
        }

        return descriptor;
    }

    /// <summary>
    ///     Returns a caching layer type descriptor will use to store
    ///     computed metadata.
    /// </summary>
    public override IDictionary GetCache(object instance)
    {
        // This should never happen because we are bound only
        // to dependency object types.  However, in case it
        // does, simply invoke the base and get out.

        if (instance is not DependencyObject d)
        {
            return base.GetCache(instance);
        }

        // The cache we return is used by TypeDescriptor to
        // store cached metadata information.  We demand create
        // it here.
        // If the DependencyObject is Sealed we cannot store
        // the cache on it - if no cache exists already we
        // will return null.

        IDictionary cache = _cacheSlot.GetValue(d);
        if (cache == null && !d.IsSealed)
        {
            cache = new Dictionary<object, object>();
            _cacheSlot.SetValue(d, cache);
        }

        return cache;
    }

    /// <summary>
    ///     This method is called when we should clear our cached state.  The cache
    ///     may become invalid if someone adds additional type description providers.
    /// </summary>
    private static void ClearCache()
    {
        lock (_propertyMap)
        {
            _propertyMap.Clear();
        }

        lock (_propertyKindMap)
        {
            _propertyKindMap.Clear();
        }

        lock (_attachInfoMap)
        {
            _attachInfoMap.Clear();
        }
    }

    /// <summary>
    ///     This method calculates the attach rules defined on 
    ///     this dependency property.  It always returns a valid
    ///     AttachInfo, but the fields in AttachInfo may be null.
    /// </summary>
    internal static AttachInfo GetAttachInfo(DependencyProperty dp)
    {
        // Have we already seen this DP?
        if (!_attachInfoMap.TryGetValue(dp, out AttachInfo info))
        {
            info = new AttachInfo(dp);

            lock (_attachInfoMap)
            {
                _attachInfoMap[dp] = info;
            }
        }

        return info;
    }

    /// <summary>
    ///     This method returns an attached property descriptor for the given DP and target type.
    /// </summary>
    internal static DependencyObjectPropertyDescriptor GetAttachedPropertyDescriptor(DependencyProperty dp, Type targetType)
    {
        lock (_propertyMap)
        {
            var key = new PropertyKey(targetType, dp);

            if (!_propertyMap.TryGetValue(key, out DependencyObjectPropertyDescriptor dpProp))
            {
                dpProp = new DependencyObjectPropertyDescriptor(dp, targetType);
                _propertyMap[key] = dpProp;
            }

            return dpProp;
        }
    }

    /// <summary>
    ///     This method returns a DependencyPropertyKind object which can
    ///     be used to tell if a given DP / target type combination represents
    ///     an attached or direct property.
    /// </summary>
    internal static DependencyPropertyKind GetDependencyPropertyKind(DependencyProperty dp, Type targetType)
    {
        lock (_propertyKindMap)
        {
            var key = new PropertyKey(targetType, dp);

            if (!_propertyKindMap.TryGetValue(key, out DependencyPropertyKind kind))
            {
                kind = new DependencyPropertyKind(dp, targetType);
                _propertyKindMap[key] = kind;
            }

            return kind;
        }
    }

    private static readonly UncommonField<IDictionary> _cacheSlot = new();

    // Synchronized by "_propertyMap"
    private static readonly Dictionary<PropertyKey, DependencyObjectPropertyDescriptor> _propertyMap = [];

    // Synchronized by "_propertyKindMap"
    private static readonly Dictionary<PropertyKey, DependencyPropertyKind> _propertyKindMap = [];

    // Synchronized by "_attachInfoMap"
    private static readonly Dictionary<DependencyProperty, AttachInfo> _attachInfoMap = [];
}
