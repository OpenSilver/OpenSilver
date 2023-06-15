// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

/// <summary>
/// Provides a dependency property identifier for limited write access to a read-only
/// dependency property.
/// </summary>
/// <remarks>
/// This object can have a transient state upon creation where the _dp 
/// field can be null until initialized.  However in use _dp needs to always
/// be non-null.  Otherwise it is treated as a key that can't unlock anything.
/// (When needed, this property is available via the static constant NoAccess.
/// </remarks>
internal sealed class DependencyPropertyKey
{
    /// <summary>
    /// Gets the dependency property identifier associated with this specialized read-only
    /// dependency property identifier.
    /// </summary>
    public DependencyProperty DependencyProperty => _dp;

    internal DependencyPropertyKey(DependencyProperty dp)
    {
        _dp = dp;
    }

    /// <summary>
    /// Overrides the metadata of a read-only dependency property that is represented
    /// by this dependency property identifier.
    /// </summary>
    /// <param name="forType">
    /// The type on which this dependency property exists and metadata should be overridden.
    /// </param>
    /// <param name="typeMetadata">
    /// Metadata supplied for this type.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Attempted metadata override on a read-write dependency property (cannot be done
    /// using this signature).
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Metadata was already established for the property as it exists on the provided
    /// type.
    /// </exception>
    public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
    {
        if (_dp == null)
        {
            throw new InvalidOperationException();
        }

        _dp.OverrideMetadata(forType, typeMetadata, this);
    }

    // This is not a property setter because we can't have a public
    //  property getter and a internal property setter on the same property.
    internal void SetDependencyProperty(DependencyProperty dp)
    {
        Debug.Assert(_dp == null, "This should only be used when we need a placeholder and have a temporary value of null. It should not be used to change this property.");
        _dp = dp;
    }

    private DependencyProperty _dp = null;
}
