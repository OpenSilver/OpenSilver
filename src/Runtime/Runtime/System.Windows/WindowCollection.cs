
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

using System.Collections;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Represents a collection of <see cref="Window"/> instances.
/// </summary>
public sealed class WindowCollection : ICollection
{
    private readonly WeakReferenceList _windows;

    internal WindowCollection()
    {
        _windows = new();
    }

    /// <summary>
    /// Gets the number of windows in the collection.
    /// </summary>
    /// <returns>
    /// The number of windows in the collection.
    /// </returns>
    public int Count => _windows.Count;

    /// <summary>
    /// Gets a value that indicates whether access to the collection is synchronized (thread safe).
    /// </summary>
    /// <returns>
    /// Always returns false.
    /// </returns>
    public bool IsSynchronized => false;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="WindowCollection"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the <see cref="WindowCollection"/>.
    /// </returns>
    public object SyncRoot => _windows;

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator for the collection.
    /// </returns>
    public IEnumerator GetEnumerator() => _windows.GetEnumerator();

    void ICollection.CopyTo(Array array, int index)
    {
        int i = 0;
        foreach (var window in _windows)
        {
            array.SetValue(window, i++);
        }
    }

    internal void Add(Window window) => _windows.Add(window);

    internal void Remove(Window window) => _windows.Remove(window);
}