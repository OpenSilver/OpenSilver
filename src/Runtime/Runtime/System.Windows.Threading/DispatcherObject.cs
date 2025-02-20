// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Threading;

/// <summary>
/// Represents an object that is associated with a <see cref="Dispatcher"/>.
/// </summary>
public abstract class DispatcherObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherObject"/> class.
    /// </summary>
    protected DispatcherObject()
    {
        Dispatcher = Dispatcher.CurrentDispatcher;
    }

    /// <summary>
    /// Gets the <see cref="Dispatcher"/> this <see cref="DispatcherObject"/> is associated with.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Dispatcher Dispatcher { get; }

    /// <summary>
    /// Determines whether the calling thread has access to this <see cref="DispatcherObject"/>.
    /// </summary>
    /// <returns>
    /// true if the calling thread has access to this object; otherwise, false.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool CheckAccess()
    {
        bool accessAllowed = true;

        if (Dispatcher is Dispatcher dispatcher)
        {
            accessAllowed = dispatcher.CheckAccess();
        }

        return accessAllowed;
    }

    /// <summary>
    /// Enforces that the calling thread has access to this <see cref="DispatcherObject"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// the calling thread does not have access to this <see cref="DispatcherObject"/>.
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void VerifyAccess() => Dispatcher?.VerifyAccess();
}