// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Data;

namespace System.ComponentModel;

/// <summary>
/// Defines methods and properties that a <see cref="CollectionView"/> implements to enable specifying 
/// adding items of a specific type.
/// </summary>
public interface IEditableCollectionViewAddNewItem : IEditableCollectionView
{
    /// <summary>
    /// Gets a value that indicates whether a specified object can be added to the collection.
    /// </summary>
    /// <returns>
    /// true if a specified object can be added to the collection; otherwise, false.
    /// </returns>
    bool CanAddNewItem { get; }

    /// <summary>
    /// Adds the specified object to the collection.
    /// </summary>
    /// <param name="newItem">
    /// The object to add to the collection.
    /// </param>
    /// <returns>
    /// The object that is added to the collection.
    /// </returns>
    object AddNewItem(object newItem);
}