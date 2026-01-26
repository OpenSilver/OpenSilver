
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
/// Represents a collection of <see cref="CommandBinding"/> objects.
/// </summary>
public sealed class CommandBindingCollection : IList<CommandBinding>, IList
{
    private readonly List<CommandBinding> _innerCBList = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBindingCollection"/> class.
    /// </summary>
    public CommandBindingCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBindingCollection"/> class using the items in the specified <see cref="IList"/>.
    /// </summary>
    /// <param name="commandBindings">
    /// The collection whose items are copied to the new <see cref="CommandBindingCollection"/>.
    /// </param>
    public CommandBindingCollection(IList commandBindings)
    {
        if (commandBindings is not null && commandBindings.Count > 0)
        {
            AddRange(commandBindings);
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="CommandBinding"/> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The position in the collection.
    /// </param>
    /// <returns>
    /// The binding at the specified index.
    /// </returns>
    public CommandBinding this[int index]
    {
        get => _innerCBList[index];
        set => _innerCBList[index] = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="CommandBindingCollection"/> has a fixed size.
    /// </summary>
    /// <returns>
    /// true if the collection has a fixed size; otherwise, false. The default value is false.
    /// </returns>
    public bool IsFixedSize => IsReadOnly;

    /// <summary>
    /// Gets a value indicating whether access to this <see cref="CommandBindingCollection"/> is synchronized (thread-safe).
    /// </summary>
    /// <returns>
    /// true if the collection is thread-safe; otherwise, false. The default value is false.
    /// </returns>
    public bool IsSynchronized => ((IList)_innerCBList).IsSynchronized;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the <see cref="CommandBindingCollection"/>.
    /// </returns>
    public object SyncRoot => this;

    /// <summary>
    /// Gets a value indicating whether this <see cref="CommandBindingCollection"/> is read-only.
    /// </summary>
    /// <returns>
    /// true if the collection is read-only; otherwise, false. The default value is false.
    /// </returns>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the number of <see cref="CommandBinding"/> items in this <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <returns>
    /// The number of bindings in the collection.
    /// </returns>
    public int Count => _innerCBList.Count;

    /// <summary>
    /// Adds the items of the specified <see cref="ICollection"/> to the end of this <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <param name="collection">
    /// The collection of items to add to the end of this <see cref="CommandBindingCollection"/>.
    /// </param>
    public void AddRange(ICollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (collection.Count > 0)
        {
            IEnumerator collectionEnum = collection.GetEnumerator();
            while (collectionEnum.MoveNext())
            {
                if (collectionEnum.Current is not CommandBinding cmdBinding)
                {
                    throw new NotSupportedException(Strings.CollectionOnlyAcceptsCommandBindings);
                }

                _innerCBList.Add(cmdBinding);
            }
        }
    }

    /// <summary>
    /// Adds the specified <see cref="CommandBinding"/> to this <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <param name="commandBinding">
    /// The binding to add to the collection.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// <paramref name="commandBinding"/> is null.
    /// </exception>
    public void Add(CommandBinding commandBinding)
    {
        if (commandBinding is null)
        {
            throw new NotSupportedException(Strings.CollectionOnlyAcceptsCommandBindings);
        }

        _innerCBList.Add(commandBinding);
    }

    /// <summary>
    /// Inserts the specified <see cref="CommandBinding"/> into this <see cref="CommandBindingCollection"/> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which to insert commandBinding.
    /// </param>
    /// <param name="commandBinding">
    /// The binding to insert.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// <paramref name="commandBinding"/> is null.
    /// </exception>
    public void Insert(int index, CommandBinding commandBinding)
    {
        if (commandBinding is null)
        {
            throw new NotSupportedException(Strings.CollectionOnlyAcceptsCommandBindings);
        }

        _innerCBList.Insert(index, commandBinding);
    }

    /// <summary>
    /// Removes the first occurrence of the specified <see cref="CommandBinding"/> from this <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <param name="commandBinding">
    /// The binding to remove.
    /// </param>
    /// <returns>
    /// true if item is successfully removed; otherwise, false.
    /// </returns>
    public bool Remove(CommandBinding commandBinding)
    {
        if (commandBinding is not null)
        {
            return _innerCBList.Remove(commandBinding);
        }
        return false;
    }

    /// <summary>
    /// Removes the specified <see cref="CommandBinding"/> at the specified index of this <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the <see cref="CommandBinding"/> to remove.
    /// </param>
    public void RemoveAt(int index) => _innerCBList.RemoveAt(index);

    /// <summary>
    /// Removes all items from this <see cref="CommandBindingCollection"/>.
    /// </summary>
    public void Clear() => _innerCBList.Clear();

    /// <summary>
    /// Searches for the first occurrence of the specified <see cref="CommandBinding"/> in this <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <param name="value">
    /// The binding to locate in the collection.
    /// </param>
    /// <returns>
    /// The index of the first occurrence of value, if found; otherwise, -1.
    /// </returns>
    public int IndexOf(CommandBinding value) => _innerCBList.IndexOf(value);

    /// <summary>
    /// Determines whether the specified <see cref="CommandBinding"/> is in this <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <param name="commandBinding">
    /// The binding to locate in the collection.
    /// </param>
    /// <returns>
    /// true if the specified <see cref="CommandBinding"/> is in the collection; otherwise, false.
    /// </returns>
    public bool Contains(CommandBinding commandBinding)
    {
        if (commandBinding is not null)
        {
            return _innerCBList.Contains(commandBinding);
        }
        return false;
    }

    /// <summary>
    /// Copies all of the items in the <see cref="CommandBindingCollection"/> to the specified one-dimensional array,
    /// starting at the specified index of the target array.
    /// </summary>
    /// <param name="commandBindings">
    /// The array into which the collection is copied.
    /// </param>
    /// <param name="index">
    /// The index position in commandBindings at which copying starts.
    /// </param>
    public void CopyTo(CommandBinding[] commandBindings, int index) => _innerCBList.CopyTo(commandBindings, index);

    /// <summary>
    /// Gets an enumerator that iterates through this <see cref="CommandBindingCollection"/>.
    /// </summary>
    /// <returns>
    /// The enumerator for this collection.
    /// </returns>
    public IEnumerator<CommandBinding> GetEnumerator() => _innerCBList.GetEnumerator();

    internal ICommand FindMatch(object targetElement, InputEventArgs inputEventArgs)
    {
        for (int i = 0; i < Count; i++)
        {
            CommandBinding commandBinding = this[i];
            if (commandBinding.Command is not RoutedCommand routedCommand)
            {
                continue;
            }

            InputGestureCollection inputGestures = routedCommand.InputGesturesInternal;
            if (inputGestures?.FindMatch(targetElement, inputEventArgs) is not null)
            {
                return routedCommand;
            }
        }

        return null;
    }

    internal CommandBinding FindMatch(ICommand command, ref int index)
    {
        while (index < Count)
        {
            CommandBinding commandBinding = this[index++];
            if (commandBinding.Command == command)
            {
                return commandBinding;
            }
        }

        return null;
    }

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index) => ((ICollection)_innerCBList).CopyTo(array, index);

    /// <inheritdoc />
    bool IList.Contains(object key) => Contains(key as CommandBinding);

    /// <inheritdoc />
    int IList.IndexOf(object value)
    {
        if (value is CommandBinding commandBinding)
        {
            return IndexOf(commandBinding);
        }
        return -1;
    }

    /// <inheritdoc />
    void IList.Insert(int index, object value) => Insert(index, value as CommandBinding);

    /// <inheritdoc />
    int IList.Add(object commandBinding)
    {
        Add(commandBinding as CommandBinding);
        return 0;
    }

    /// <inheritdoc />
    void IList.Remove(object commandBinding) => Remove(commandBinding as CommandBinding);

    /// <inheritdoc />
    object IList.this[int index]
    {
        get => this[index];
        set
        {
            if (value is not CommandBinding commandBinding)
            {
                throw new NotSupportedException(Strings.CollectionOnlyAcceptsCommandBindings);
            }

            this[index] = commandBinding;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => _innerCBList.GetEnumerator();
}
