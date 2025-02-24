// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Data;

/// <summary>
/// Provides data for the <see cref="BindingOperations.CollectionViewRegistering"/> event.
/// </summary>
public class CollectionViewRegisteringEventArgs : EventArgs
{
    internal CollectionViewRegisteringEventArgs(CollectionView view)
    {
        CollectionView = view;
    }

    /// <summary>
    /// Gets the collection view to be registered for cross-thread access.
    /// </summary>
    /// <returns>
    /// The collection view to be registered for cross-thread access.
    /// </returns>
    public CollectionView CollectionView { get; }
}