
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
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Represents an ordered collection of <see cref="InputGesture"/> objects.
/// </summary>
public sealed class InputGestureCollection : IList<InputGesture>, IList
{
    private readonly List<InputGesture> _innerGestureList = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="InputGestureCollection"/> class.
    /// </summary>
    public InputGestureCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputGestureCollection"/> class using the elements in the specified <see cref="IList"/>.
    /// </summary>
    /// <param name="inputGestures">
    /// The collection whose elements are copied to the new <see cref="InputGestureCollection"/>.
    /// </param>
    public InputGestureCollection(IList inputGestures)
    {
        if (inputGestures is not null && inputGestures.Count > 0)
        {
            AddRange(inputGestures);
        }
    }

    /// <summary>
    /// Gets or set the <see cref="InputGesture"/> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The position in the collection.
    /// </param>
    /// <returns>
    /// The gesture at the specified index.
    /// </returns>
    public InputGesture this[int index]
    {
        get => _innerGestureList[index];
        set
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
            }

            _innerGestureList[index] = value;
        }
    }

    /// <summary>
    /// Gets the number of <see cref="InputGesture"/> items in this <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <returns>
    /// The number of gestures in the collection.
    /// </returns>
    public int Count => _innerGestureList.Count;

    /// <summary>
    /// Gets a value that indicates whether this <see cref="InputGestureCollection"/> is synchronized (thread safe).
    /// </summary>
    /// <returns>
    /// true if the collection is thread-safe; otherwise, false. The default value is false.
    /// </returns>
    public bool IsSynchronized => ((IList)_innerGestureList).IsSynchronized;

    /// <summary>
    /// Gets an object that can be used to synchronize access to this <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <returns>
    /// The object that can be used to synchronize access to the collection.
    /// </returns>
    public object SyncRoot => ((IList)_innerGestureList).SyncRoot;

    /// <summary>
    /// Gets a value that indicates whether this <see cref="InputGestureCollection"/> is read-only. The default value is false.
    /// </summary>
    /// <returns>
    /// true if the collection read-only; otherwise, false.
    /// </returns>
    public bool IsReadOnly { get; private set; }

    /// <summary>
    /// Gets a value that indicates whether this <see cref="InputGestureCollection"/> has a fixed size.
    /// </summary>
    /// <returns>
    /// true if the collection has a fixed size; otherwise, false. The default value is false.
    /// </returns>
    public bool IsFixedSize => IsReadOnly;

    /// <summary>
    /// Gets an enumerator that iterates through this <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <returns>
    /// The enumerator for this collection.
    /// </returns>
    public IEnumerator<InputGesture> GetEnumerator() => _innerGestureList.GetEnumerator();

    /// <summary>
    /// Searches for the first occurrence of the specified <see cref="InputGesture"/> in this <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <param name="value">
    /// The gesture to locate in the collection.
    /// </param>
    /// <returns>
    /// The index of the first occurrence of value, if found; otherwise, -1.
    /// </returns>
    public int IndexOf(InputGesture value) => _innerGestureList.IndexOf(value);

    /// <summary>
    /// Removes the specified <see cref="InputGesture"/> at the specified index of this <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the gesture to remove.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// the collection is read-only.
    /// </exception>
    public void RemoveAt(int index)
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        _innerGestureList.RemoveAt(index);
    }

    /// <summary>
    /// Adds the specified <see cref="InputGesture"/> to this <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <param name="inputGesture">
    /// The gesture to add to the collection.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// the collection is read-only.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="inputGesture"/> is null.
    /// </exception>
    public void Add(InputGesture inputGesture)
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        if (inputGesture is null)
        {
            throw new ArgumentNullException(nameof(inputGesture));
        }

        _innerGestureList.Add(inputGesture);
    }

    /// <summary>
    /// Inserts the specified <see cref="InputGesture"/> into this <see cref="InputGestureCollection"/> at the specified index.
    /// </summary>
    /// <param name="index">
    /// Index at which to insert inputGesture.
    /// </param>
    /// <param name="inputGesture">
    /// The gesture to insert.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// <paramref name="inputGesture"/> is null.
    /// </exception>
    public void Insert(int index, InputGesture inputGesture)
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        if (inputGesture is null)
        {
            throw new NotSupportedException(Strings.CollectionOnlyAcceptsInputGestures);
        }

        _innerGestureList.Insert(index, inputGesture);
    }

    /// <summary>
    /// Removes the first occurrence of the specified <see cref="InputGesture"/> from this <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <param name="inputGesture">
    /// The gesture to remove.
    /// </param>
    /// <returns>
    /// true if item is successfully removed; otherwise, false.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The collection is read-only.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="inputGesture"/> is null.
    /// </exception>
    public bool Remove(InputGesture inputGesture)
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        if (inputGesture is null)
        {
            throw new ArgumentNullException(nameof(inputGesture));
        }

        return _innerGestureList.Remove(inputGesture);
    }

    /// <summary>
    /// Removes all elements from the <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// The collection is read only.
    /// </exception>
    public void Clear()
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        _innerGestureList.Clear();
    }

    /// <summary>
    /// Determines whether the specified <see cref="InputGesture"/> is in the collection.
    /// </summary>
    /// <param name="key">
    /// The gesture to locate in the collection.
    /// </param>
    /// <returns>
    /// true if the gesture is in the collection; otherwise, false.
    /// </returns>
    public bool Contains(InputGesture key)
    {
        if (key is not null)
        {
            return _innerGestureList.Contains(key);
        }
        return false;
    }

    /// <summary>
    /// Copies all of the items in the <see cref="InputGestureCollection"/> to the specified one-dimensional array,
    /// starting at the specified index of the target array.
    /// </summary>
    /// <param name="inputGestures">
    /// An array into which the collection is copied.
    /// </param>
    /// <param name="index">
    /// The index position in the inputGestures at which copying begins.
    /// </param>
    public void CopyTo(InputGesture[] inputGestures, int index) => _innerGestureList.CopyTo(inputGestures, index);

    /// <summary>
    /// Adds the elements of the specified <see cref="ICollection"/> to the end of this <see cref="InputGestureCollection"/>.
    /// </summary>
    /// <param name="collection">
    /// The collection of items to add to the end of this <see cref="InputGestureCollection"/>.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// The <paramref name="collection"/> is read-only.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="collection"/> to add is null.
    /// </exception>
    public void AddRange(ICollection collection)
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (collection.Count > 0)
        {
            IEnumerator collectionEnum = collection.GetEnumerator();
            while (collectionEnum.MoveNext())
            {
                if (collectionEnum.Current is not InputGesture inputGesture)
                {
                    throw new NotSupportedException(Strings.CollectionOnlyAcceptsInputGestures);
                }

                _innerGestureList.Add(inputGesture);
            }
        }
    }

    /// <summary>
    /// Sets this <see cref="InputGestureCollection"/> to read-only.
    /// </summary>
    public void Seal() => IsReadOnly = true;

    internal InputGesture FindMatch(object targetElement, InputEventArgs inputEventArgs)
    {
        for (int i = 0; i < Count; i++)
        {
            InputGesture inputGesture = this[i];
            if (inputGesture.Matches(targetElement, inputEventArgs))
            {
                return inputGesture;
            }
        }

        return null;
    }

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index) => ((ICollection)_innerGestureList).CopyTo(array, index);

    /// <inheritdoc />
    bool IList.Contains(object key) => Contains(key as InputGesture);

    /// <inheritdoc />
    int IList.IndexOf(object value)
    {
        if (value is InputGesture inputGesture)
        {
            return IndexOf(inputGesture);
        }
        return -1;
    }

    /// <inheritdoc />
    void IList.Insert(int index, object value)
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        Insert(index, value as InputGesture);
    }

    /// <inheritdoc />
    int IList.Add(object inputGesture)
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        Add(inputGesture as InputGesture);
        return 0;
    }

    /// <inheritdoc />
    void IList.Remove(object inputGesture)
    {
        if (IsReadOnly)
        {
            throw new NotSupportedException(Strings.ReadOnlyInputGesturesCollection);
        }

        Remove(inputGesture as InputGesture);
    }

    /// <inheritdoc />
    object IList.this[int index]
    {
        get => this[index];
        set
        {
            if (value is not InputGesture inputGesture)
            {
                throw new NotSupportedException(Strings.CollectionOnlyAcceptsInputGestures);
            }

            this[index] = inputGesture;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => _innerGestureList.GetEnumerator();
}
