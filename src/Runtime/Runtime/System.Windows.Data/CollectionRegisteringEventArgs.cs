// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Data;

/// <summary>
/// Provides data for the <see cref="BindingOperations.CollectionRegistering"/> event.
/// </summary>
public class CollectionRegisteringEventArgs : EventArgs
{
    internal CollectionRegisteringEventArgs(IEnumerable collection, object parent = null)
    {
        Collection = collection;
        Parent = parent;
    }

    /// <summary>
    /// Gets the collection to be registered for cross-thread access.
    /// </summary>
    /// <returns>
    /// The collection to be registered for cross-thread access.
    /// </returns>
    public IEnumerable Collection { get; }

    /// <summary>
    /// Gets the parent of the collection to register.
    /// </summary>
    /// <returns>
    /// The parent of the collection to register.
    /// </returns>
    public object Parent { get; }
}