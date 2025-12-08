// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Implements an underlying type cache for all <see cref="DependencyObject"/> derived types.
/// </summary>
/// <remarks>
/// <see cref="DependencyObjectType"/> represents a specific underlying system (CLR) <see cref="Type"/> 
/// of a <see cref="DependencyObject"/>. <see cref="DependencyObjectType"/> is essentially a wrapper for 
/// the (CLR) <see cref="Type"/> so that it can extend some of its capabilities.
/// This class has no public constructor. Instances of this class can only be obtained through properties 
/// on other objects (such as <see cref="DependencyObject.DependencyObjectType"/>), or through the static 
/// method <see cref="FromSystemType"/>.
/// </remarks>
public sealed class DependencyObjectType
{
    // Cached metadata lookup - provides O(1) access to PropertyMetadata for this DependencyObjectType.
    // This is a major performance optimization as GetMetadata is called on every SetValue.
    // The array is indexed by DependencyProperty.GlobalIndex.
    // A null entry means the metadata hasn't been cached yet.
    // We use a sentinel value to distinguish "not cached" from "cached but returned DefaultMetadata".
    private PropertyMetadata[] _metadataCache;
    /// <summary>
    /// Returns a <see cref="DependencyObjectType"/> that represents a given system
    /// (CLR) type.
    /// </summary>
    /// <param name="systemType">
    /// The system (CLR) type to convert.
    /// </param>
    /// <returns>
    /// A <see cref="DependencyObjectType"/> that represents the system (CLR) type.
    /// </returns>
    public static DependencyObjectType FromSystemType(Type systemType)
    {
        if (systemType is null)
        {
            throw new ArgumentNullException(nameof(systemType));
        }

        if (!typeof(DependencyObject).IsAssignableFrom(systemType))
        {
            throw new ArgumentException(string.Format(Strings.DTypeNotSupportForSystemType, systemType.Name));
        }

        return FromSystemTypeInternal(systemType);
    }

    /// <summary>
    /// Helper method for the public FromSystemType call but without
    /// the expensive IsAssignableFrom parameter validation.
    /// </summary>
    internal static DependencyObjectType FromSystemTypeInternal(Type systemType)
    {
        Debug.Assert(systemType != null && typeof(DependencyObject).IsAssignableFrom(systemType), "Invalid systemType argument");

        DependencyObjectType retVal;

        lock (_lock)
        {
            // Recursive routine to (set up if necessary) and use the
            //  DTypeFromCLRType hashtable that is used for the actual lookup.
            retVal = FromSystemTypeRecursive(systemType);
        }

        return retVal;
    }

    // The caller must wrap this routine inside a locked block.
    // This recursive routine manipulates the static hashtable DTypeFromCLRType
    // and it must not be allowed to do this across multiple threads
    // simultaneously.
    private static DependencyObjectType FromSystemTypeRecursive(Type systemType)
    {
        DependencyObjectType dType;

        // Map a CLR Type to a DependencyObjectType
        if (!DTypeFromCLRType.TryGetValue(systemType, out dType))
        {
            // No DependencyObjectType found, create
            dType = new DependencyObjectType();

            // Store CLR type
            dType._systemType = systemType;

            // Store reverse mapping
            DTypeFromCLRType[systemType] = dType;

            // Establish base DependencyObjectType and base property count
            if (systemType != typeof(DependencyObject))
            {
                // Get base type
                dType._baseDType = FromSystemTypeRecursive(systemType.BaseType);
            }

            // Store DependencyObjectType zero-based Id
            dType._id = DTypeCount++;
        }

        return dType;
    }

    /// <summary>
    /// Gets a zero-based unique identifier for constant-time array lookup operations.
    /// </summary>
    /// <remarks>
    /// There is no guarantee on this value. It can vary between application runs.
    /// </remarks>
    public int Id { get { return _id; } }

    /// <summary>
    /// Gets the common language runtime (CLR) system type represented by this 
    /// <see cref="DependencyObjectType"/>.
    /// </summary>
    public Type SystemType { get { return _systemType; } }

    /// <summary>
    /// Gets the <see cref="DependencyObjectType"/> of the immediate base class of the
    /// current <see cref="DependencyObjectType"/>.
    /// </summary>
    public DependencyObjectType BaseType
    {
        get
        {
            return _baseDType;
        }
    }

    /// <summary>
    /// Gets the name of the represented common language runtime (CLR) system type.
    /// </summary>
    public string Name { get { return SystemType.Name; } }

    /// <summary>
    /// Determines whether the specified object is an instance of the current 
    /// <see cref="DependencyObjectType"/>.
    /// </summary>
    /// <param name="dependencyObject">
    /// The object to compare with the current <see cref="DependencyObjectType"/>.
    /// </param>
    /// <returns>
    /// true if the class represented by the current <see cref="DependencyObjectType"/>
    /// is in the inheritance hierarchy of the <see cref="DependencyObject"/> passed
    /// as d; otherwise, false.
    /// </returns>
    public bool IsInstanceOfType(DependencyObject dependencyObject)
    {
        if (dependencyObject != null)
        {
            DependencyObjectType dType = dependencyObject.DependencyObjectType;

            do
            {
                if (dType.Id == Id)
                {
                    return true;
                }

                dType = dType._baseDType;
            }
            while (dType != null);
        }
        return false;
    }

    /// <summary>
    /// Determines whether the current <see cref="DependencyObjectType"/> derives from
    /// the specified <see cref="DependencyObjectType"/>.
    /// </summary>
    /// <param name="dependencyObjectType">
    /// The <see cref="DependencyObjectType"/> to compare.
    /// </param>
    /// <returns>
    /// true if the dependencyObjectType parameter and the current <see cref="DependencyObjectType"/>
    /// represent types of classes, and the class represented by the current <see cref="DependencyObjectType"/>
    /// derives from the class represented by dependencyObjectType. Otherwise, false.
    /// This method also returns false if dependencyObjectType and the current <see cref="DependencyObjectType"/>
    /// represent the same class.
    /// </returns>
    public bool IsSubclassOf(DependencyObjectType dependencyObjectType)
    {
        // Check for null and return false, since this type is never a subclass of null.
        if (dependencyObjectType != null)
        {
            // A DependencyObjectType isn't considered a subclass of itself, so start with base type
            DependencyObjectType dType = _baseDType;

            while (dType != null)
            {
                if (dType.Id == dependencyObjectType.Id)
                {
                    return true;
                }

                dType = dType._baseDType;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the hash code for this <see cref="DependencyObjectType"/>.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer hash code.
    /// </returns>
    public override int GetHashCode()
    {
        return _id;
    }

    /// <summary>
    /// Gets the cached PropertyMetadata for the specified DependencyProperty on this type.
    /// This provides O(1) lookup instead of walking the metadata override chain.
    /// </summary>
    /// <param name="dp">The dependency property to get metadata for.</param>
    /// <param name="metadata">The cached metadata if found.</param>
    /// <returns>True if a cached value was found, false otherwise.</returns>
    internal bool TryGetCachedMetadata(DependencyProperty dp, out PropertyMetadata metadata)
    {
        int globalIndex = dp.GlobalIndex;
        PropertyMetadata[] cache = _metadataCache;
        
        if (cache != null && globalIndex < cache.Length)
        {
            metadata = cache[globalIndex];
            if (metadata != null)
            {
                return true;
            }
        }
        
        metadata = null;
        return false;
    }

    /// <summary>
    /// Caches the PropertyMetadata for the specified DependencyProperty on this type.
    /// </summary>
    /// <param name="dp">The dependency property.</param>
    /// <param name="metadata">The metadata to cache.</param>
    internal void SetCachedMetadata(DependencyProperty dp, PropertyMetadata metadata)
    {
        int globalIndex = dp.GlobalIndex;
        PropertyMetadata[] cache = _metadataCache;
        
        // Ensure the cache array is large enough
        if (cache == null || globalIndex >= cache.Length)
        {
            // Need to grow the cache. Use the current registered property count + some buffer.
            int newSize = Math.Max(globalIndex + 1, DependencyProperty.RegisteredPropertyCount + 16);
            PropertyMetadata[] newCache = new PropertyMetadata[newSize];
            
            if (cache != null)
            {
                Array.Copy(cache, newCache, cache.Length);
            }
            
            _metadataCache = newCache;
            cache = newCache;
        }
        
        // Store the metadata
        cache[globalIndex] = metadata;
    }

    // DTypes may not be constructed outside of FromSystemType
    private DependencyObjectType()
    {
    }

    private int _id;
    private Type _systemType;
    private DependencyObjectType _baseDType;

    // Synchronized: Covered by DispatcherLock
    private static readonly Dictionary<Type, DependencyObjectType> DTypeFromCLRType = new Dictionary<Type, DependencyObjectType>();

    // Synchronized: Covered by DispatcherLock
    private static int DTypeCount = 0;

    private static readonly object _lock = new object();
}
