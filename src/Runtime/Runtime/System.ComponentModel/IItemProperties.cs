// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace System.ComponentModel;

/// <summary>
/// Defines a property that provides information about an object's properties.
/// </summary>
public interface IItemProperties
{
    /// <summary>
    /// Gets a collection that contains information about the properties that are available on the items in a collection.
    /// </summary>
    /// <returns>
    /// A collection that contains information about the properties that are available on the items in a collection.
    /// </returns>
    ReadOnlyCollection<ItemPropertyInfo> ItemProperties { get; }
}

/// <summary>
/// Contains information about a property.
/// </summary>
public class ItemPropertyInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPropertyInfo"/> class.
    /// </summary>
    /// <param name="name">
    /// The name of the property.
    /// </param>
    /// <param name="type">
    /// The type of the property.
    /// </param>
    /// <param name="descriptor">
    /// An object that contains additional information about the property.
    /// </param>
    public ItemPropertyInfo(string name, Type type, object descriptor)
    {
        Name = name;
        PropertyType = type;
        Descriptor = descriptor;
    }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <returns>
    /// The name of the property.
    /// </returns>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    /// <returns>
    /// The type of the property.
    /// </returns>
    public Type PropertyType { get; }

    /// <summary>
    /// Get an object that contains additional information about the property.
    /// </summary>
    /// <returns>
    /// An object that contains additional information about the property.
    /// </returns>
    public object Descriptor { get; }
}