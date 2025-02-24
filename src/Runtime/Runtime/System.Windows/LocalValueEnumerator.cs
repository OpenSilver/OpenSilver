// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Provides enumeration support for the local values of any dependency properties that exist on a <see cref="DependencyObject"/>.
/// </summary>
public struct LocalValueEnumerator : IEnumerator
{
    private readonly LocalValueEntry[] _snapshot;
    private int _index;

    internal LocalValueEnumerator(LocalValueEntry[] snapshot, int count)
    {
        Count = count;
        _snapshot = snapshot;
        _index = -1;
    }

    /// <summary>
    /// Gets an empty enumerator.
    /// </summary>
    public static readonly LocalValueEnumerator Empty = new([], 0);

    /// <summary>
    /// Gets the number of items that are represented in the collection.
    /// </summary>
    /// <returns>
    /// The number of items in the collection.
    /// </returns>
    public int Count { get; }

    /// <summary>
    /// Gets the current element in the collection.
    /// </summary>
    /// <returns>
    /// The current <see cref="LocalValueEntry"/> in the collection.
    /// </returns>
    public LocalValueEntry Current
    {
        get
        {
            if (_index == -1)
            {
                throw new InvalidOperationException(Strings.LocalValueEnumerationReset);
            }

            if (_index >= Count)
            {
                throw new InvalidOperationException(Strings.LocalValueEnumerationOutOfBounds);
            }

            return _snapshot[_index];
        }
    }

    /// <inheritdoc />
    object IEnumerator.Current => Current;

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is LocalValueEnumerator other && Equals(other);

    /// <summary>
    /// Determines whether the provided <see cref="LocalValueEnumerator"/> is equivalent to this <see cref="LocalValueEnumerator"/>.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="LocalValueEnumerator"/> to compare with the current <see cref="LocalValueEnumerator"/>.
    /// </param>
    /// <returns>
    /// true if the specified <see cref="LocalValueEnumerator"/> is equal to the current <see cref="LocalValueEnumerator"/>; otherwise, false.
    /// </returns>
    public bool Equals(LocalValueEnumerator obj) => Count == obj.Count && _index == obj._index && _snapshot == obj._snapshot;

    /// <summary>
    /// Returns a hash code for the current <see cref="LocalValueEnumerator"/>.
    /// </summary>
    /// <returns>
    /// A 32-bit integer hash code.
    /// </returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>
    /// Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns>
    /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
    /// </returns>
    public bool MoveNext()
    {
        _index++;
        return _index < Count;
    }

    /// <summary>
    /// Sets the enumerator to its initial position, which is before the first element in the collection.
    /// </summary>
    public void Reset() => _index = -1;

    /// <summary>
    /// Compares whether two specified <see cref="LocalValueEnumerator"/> objects are the same.
    /// </summary>
    /// <param name="obj1">
    /// The first object to compare.
    /// </param>
    /// <param name="obj2">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// true if the obj1 <see cref="LocalValueEnumerator"/> is equal to the obj2 <see cref="LocalValueEnumerator"/>; otherwise, false.
    /// </returns>
    public static bool operator ==(LocalValueEnumerator obj1, LocalValueEnumerator obj2) => obj1.Equals(obj2);

    /// <summary>
    /// Compares two specified <see cref="LocalValueEnumerator"/> objects to determine whether they are not the same.
    /// </summary>
    /// <param name="obj1">
    /// The first object to compare.
    /// </param>
    /// <param name="obj2">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// true if the instances are not equal; otherwise, false.
    /// </returns>
    public static bool operator !=(LocalValueEnumerator obj1, LocalValueEnumerator obj2) => !obj1.Equals(obj2);
}

/// <summary>
/// Represents a property identifier and the property value for a locally set dependency property.
/// </summary>
public readonly struct LocalValueEntry
{
    internal LocalValueEntry(DependencyProperty dp, object value)
    {
        Property = dp;
        Value = value;
    }

    /// <summary>
    /// Gets the identifier for the locally set dependency property that is represented by this entry.
    /// </summary>
    /// <returns>
    /// The identifier for the locally set dependency property.
    /// </returns>
    public DependencyProperty Property { get; }

    /// <summary>
    /// Gets the value of the locally set dependency property.
    /// </summary>
    /// <returns>
    /// The value of the locally set dependency property as an object.
    /// </returns>
    public object Value { get; }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is LocalValueEntry other && Equals(other);

    /// <summary>
    /// Determines whether two <see cref="LocalValueEntry"/> instances are equal.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="LocalValueEntry"/> to compare with the current <see cref="LocalValueEntry"/>.
    /// </param>
    /// <returns>
    /// This <see cref="operator ==(LocalValueEntry, LocalValueEntry)"/> implementation compares the values of the 
    /// <see cref="Property"/>, and potentially compares the values of <see cref="Value"/>. The <see cref="Property"/> 
    /// component of a <see cref="LocalValueEntry"/> is a value type, so will always be a bitwise comparison. For the 
    /// <see cref="Value"/> component, this implementation employs a bitwise comparison if it is a value type. For 
    /// locally set properties that have reference types, the behavior is deferred to that type's equality determination 
    /// mechanisms, because it just uses the == operator on the two values internally. By default, this would be a 
    /// reference equality of the values and thus the equality of the entire <see cref="LocalValueEntry"/> would become 
    /// a reference equality.
    /// </returns>
    public bool Equals(LocalValueEntry obj) => Property == obj.Property && Value == obj.Value;

    /// <summary>
    /// Returns the hash code for this <see cref="LocalValueEntry"/>.
    /// </summary>
    /// <returns>
    /// A signed 32-bit integer value.
    /// </returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>
    /// Compares the specified <see cref="LocalValueEntry"/> instances to determine whether they are the same.
    /// </summary>
    /// <param name="obj1">
    /// The first instance to compare.
    /// </param>
    /// <param name="obj2">
    /// The second instance to compare.
    /// </param>
    /// <returns>
    /// true if the obj1 <see cref="LocalValueEntry"/> is equal to the obj2 <see cref="LocalValueEntry"/>; otherwise, false.
    /// </returns>
    public static bool operator ==(LocalValueEntry obj1, LocalValueEntry obj2) => obj1.Equals(obj2);

    /// <summary>
    /// Compares the specified <see cref="LocalValueEnumerator"/> instances to determine whether they are different.
    /// </summary>
    /// <param name="obj1">
    /// The first instance to compare.
    /// </param>
    /// <param name="obj2">
    /// The second instance to compare.
    /// </param>
    /// <returns>
    /// This implementation compares the values of the <see cref="Property"/> and <see cref="Value"/> components of a 
    /// <see cref="LocalValueEntry"/>. The <see cref="Property"/> component of a <see cref="LocalValueEntry"/> is always 
    /// a value type, so this comparison will always be a bitwise comparison. For the <see cref="Value"/> component, 
    /// this implementation employs a bitwise comparison if it is a value type. For locally set properties that have
    /// reference types, the behavior is deferred to that type's equality determination mechanisms, because it uses the 
    /// == operator on the two values internally. By default, this is a reference equality of the values.
    /// </returns>
    public static bool operator !=(LocalValueEntry obj1, LocalValueEntry obj2) => !obj1.Equals(obj2);
}
