
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
using System.Collections.Generic;
using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Represents an ordered collection of <see cref="InputBinding"/> objects.
/// </summary>
public sealed class InputBindingCollection : IList<InputBinding>, IList
{
    private readonly List<InputBinding> _innerBindingList = [];
    private readonly DependencyObject _owner;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputBindingCollection"/> class.
    /// </summary>
    public InputBindingCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputBindingCollection"/> class using the items in the
    /// specified <see cref="IList"/>.
    /// </summary>
    /// <param name="inputBindings">
    /// The collection whose items are copied to the new <see cref="InputBindingCollection"/>.
    /// </param>
    public InputBindingCollection(IList inputBindings)
    {
        if (inputBindings is not null && inputBindings.Count > 0)
        {
            AddRange(inputBindings);
        }
    }

    // internal constructor
    internal InputBindingCollection(DependencyObject owner)
    {
        Debug.Assert(owner is not null);
        _owner = owner;
    }

    /// <summary>
    /// Gets or sets the <see cref="InputBinding"/> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The position in the collection.
    /// </param>
    /// <returns>
    /// The <see cref="InputBinding"/> at the specified index.
    /// </returns>
    public InputBinding this[int index]
    {
        get => _innerBindingList[index];
        set
        {
            InputBinding oldInputBinding = _innerBindingList[index];
            _innerBindingList[index] = value;
            if (_owner is DependencyObject owner)
            {
                owner.RemoveSelfAsInheritanceContext(oldInputBinding, null);
                owner.ProvideSelfAsInheritanceContext(value, null);
            }
        }
    }

    /// <summary>
    /// Gets the number of <see cref="InputBinding"/> items in this collection.
    /// </summary>
    /// <returns>
    /// The number of items in the collection.
    /// </returns>
    public int Count => _innerBindingList.Count;

    /// <summary>
    /// Gets a value that indicates whether this <see cref="InputBindingCollection"/> is read-only.
    /// </summary>
    /// <returns>
    /// true if the collection is read-only; otherwise, false. The default is false.
    /// </returns>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets a value that indicates whether this <see cref="InputBindingCollection"/> has a fixed size.
    /// </summary>
    /// <returns>
    /// true if the collection has a fixed size; otherwise, false. The default is false.
    /// </returns>
    public bool IsFixedSize => IsReadOnly;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="InputBindingCollection"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the <see cref="InputBindingCollection"/>.
    /// </returns>
    public object SyncRoot => this;

    /// <summary>
    /// Gets a value indicating whether access to this <see cref="InputBindingCollection"/> is synchronized (thread-safe).
    /// </summary>
    /// <returns>
    /// true if the collection is thread safe; otherwise, false. The default is false.
    /// </returns>
    public bool IsSynchronized => ((IList)_innerBindingList).IsSynchronized;

    /// <summary>
    /// Adds the items of the specified <see cref="ICollection"/> to the end of this <see cref="InputBindingCollection"/>.
    /// </summary>
    /// <param name="collection">
    /// The collection of items to add to the end of this <see cref="InputBindingCollection"/>.
    /// </param>
    public void AddRange(ICollection collection)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (collection.Count > 0)
        {
            IEnumerator collectionEnum = collection.GetEnumerator();
            while (collectionEnum.MoveNext())
            {
                if (collectionEnum.Current is not InputBinding inputBinding)
                {
                    throw new NotSupportedException(Strings.CollectionOnlyAcceptsInputBindings);
                }

                _innerBindingList.Add(inputBinding);
                _owner?.ProvideSelfAsInheritanceContext(inputBinding, null);
            }
        }
    }

    /// <summary>
    /// Adds the specified <see cref="InputBinding"/> to this <see cref="InputBindingCollection"/>.
    /// </summary>
    /// <param name="inputBinding">
    /// The binding to add to the collection.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// <paramref name="inputBinding"/> is null.
    /// </exception>
    public void Add(InputBinding inputBinding)
    {
        if (inputBinding is null)
        {
            throw new NotSupportedException(Strings.CollectionOnlyAcceptsInputBindings);
        }

        _innerBindingList.Add(inputBinding);
        _owner?.ProvideSelfAsInheritanceContext(inputBinding, null);
    }

    /// <summary>
    /// Searches for the first occurrence of the specified <see cref="InputBinding"/> in this <see cref="InputBindingCollection"/>.
    /// </summary>
    /// <param name="value">
    /// The object to locate in the collection.
    /// </param>
    /// <returns>
    /// The index of the first occurrence of value, if found; otherwise, -1.
    /// </returns>
    public int IndexOf(InputBinding value) => _innerBindingList.IndexOf(value);

    /// <summary>
    /// Inserts the specified <see cref="InputBinding"/> into this <see cref="InputBindingCollection"/> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which to insert inputBinding.
    /// </param>
    /// <param name="inputBinding">
    /// The binding to insert.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// <paramref name="inputBinding"/> is null.
    /// </exception>
    public void Insert(int index, InputBinding inputBinding)
    {
        if (inputBinding is null)
        {
            throw new NotSupportedException(Strings.CollectionOnlyAcceptsInputBindings);
        }

        _innerBindingList.Insert(index, inputBinding);
        _owner?.ProvideSelfAsInheritanceContext(inputBinding, null);
    }

    /// <summary>
    /// Removes the first occurrence of the specified <see cref="InputBinding"/> from this <see cref="InputBindingCollection"/>.
    /// </summary>
    /// <param name="inputBinding">
    /// The binding to remove.
    /// </param>
    /// <returns>
    /// true if item is successfully removed; otherwise, false.
    /// </returns>
    public bool Remove(InputBinding inputBinding)
    {
        if (inputBinding is not null)
        {
            if (_innerBindingList.Remove(inputBinding))
            {
                _owner?.RemoveSelfAsInheritanceContext(inputBinding, null);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes the specified <see cref="InputBinding"/> at the specified index of this <see cref="InputBindingCollection"/>.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the <see cref="InputBinding"/> to remove.
    /// </param>
    public void RemoveAt(int index)
    {
        InputBinding oldInputBinding = _innerBindingList[index];
        _innerBindingList.RemoveAt(index);
        _owner?.RemoveSelfAsInheritanceContext(oldInputBinding, null);
    }

    /// <summary>
    /// Removes all items from this <see cref="InputBindingCollection"/>.
    /// </summary>
    public void Clear()
    {
        if (_innerBindingList.Count > 0)
        {
            InputBinding[] oldInputBindings = _owner is not null ? [.. _innerBindingList] : [];

            _innerBindingList.Clear();

            foreach (InputBinding inputBinding in oldInputBindings)
            {
                _owner.RemoveSelfAsInheritanceContext(inputBinding, null);
            }
        }
    }

    /// <summary>
    /// Gets an enumerator that iterates through this <see cref="InputBindingCollection"/>.
    /// </summary>
    /// <returns>
    /// The enumerator for this collection.
    /// </returns>
    public IEnumerator<InputBinding> GetEnumerator() => _innerBindingList.GetEnumerator();

    /// <summary>
    /// Determines whether the specified <see cref="InputBinding"/> is in this <see cref="InputBindingCollection"/>.
    /// </summary>
    /// <param name="key">
    /// The binding to locate in the collection.
    /// </param>
    /// <returns>
    /// true if the specified <see cref="InputBinding"/> is in the collection; otherwise, false.
    /// </returns>
    public bool Contains(InputBinding key)
    {
        if (key is not null)
        {
            return _innerBindingList.Contains(key);
        }

        return false;
    }

    /// <summary>
    /// Copies all of the items in the <see cref="InputBindingCollection"/> to the specified one-dimensional array,
    /// starting at the specified index of the target array.
    /// </summary>
    /// <param name="inputBindings">
    /// The array into which the collection is copied.
    /// </param>
    /// <param name="index">
    /// The index position in inputBindings at which copying starts.
    /// </param>
    public void CopyTo(InputBinding[] inputBindings, int index) => _innerBindingList.CopyTo(inputBindings, index);

    internal InputBinding FindMatch(object targetElement, InputEventArgs inputEventArgs)
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            InputBinding inputBinding = this[i];
            if (inputBinding.Command is not null &&
                inputBinding.Gesture is not null &&
                inputBinding.Gesture.Matches(targetElement, inputEventArgs))
            {
                return inputBinding;
            }
        }

        return null;
    }

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index) => ((ICollection)_innerBindingList).CopyTo(array, index);

    /// <inheritdoc />
    bool IList.Contains(object key) => Contains(key as InputBinding);

    /// <inheritdoc />
    int IList.IndexOf(object value)
    {
        if (value is InputBinding inputBinding)
        {
            return IndexOf(inputBinding);
        }
        return -1;
    }

    /// <inheritdoc />
    void IList.Insert(int index, object value) => Insert(index, value as InputBinding);

    /// <inheritdoc />
    int IList.Add(object inputBinding)
    {
        Add(inputBinding as InputBinding);
        return 0;
    }

    /// <inheritdoc />
    void IList.Remove(object inputBinding) => Remove(inputBinding as InputBinding);

    /// <inheritdoc />
    object IList.this[int index]
    {
        get => this[index];
        set
        {
            if (value is not InputBinding inputBinding)
            {
                throw new NotSupportedException(Strings.CollectionOnlyAcceptsInputBindings);
            }

            this[index] = inputBinding;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => _innerBindingList.GetEnumerator();
}
