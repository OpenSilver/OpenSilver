
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
using System.Windows.Markup;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// Implements base WPF support for the <see cref="INameScope"/> methods that store or retrieve name-object mappings 
/// into a particular XAML namescope. Adds attached property support to make it simpler to get or set XAML namescope 
/// names dynamically at the element level.
/// </summary>
public class NameScope : INameScopeDictionary
{
    private Dictionary<string, object> _nameMap;

    /// <summary>
    /// Returns the corresponding object in the XAML namescope maintained by this <see cref="NameScope"/>, based on a 
    /// provided name string.
    /// </summary>
    /// <param name="name">
    /// Name portion of an existing mapping to retrieve the object portion for.
    /// </param>
    /// <returns>
    /// The requested object that is mapped with <paramref name="name"/>. Can return null if <paramref name="name"/> was 
    /// provided as null or empty string, or if no matching object was found.
    /// </returns>
    public object FindName(string name)
    {
        if (_nameMap is null || string.IsNullOrEmpty(name))
        {
            return null;
        }

        if (_nameMap.TryGetValue(name, out object obj))
        {
            return obj;
        }

        return null;
    }

    /// <summary>
    /// Registers a new name-object pair into the current XAML namescope.
    /// </summary>
    /// <param name="name">
    /// The name to use for mapping the given object.
    /// </param>
    /// <param name="scopedElement">
    /// The object to be mapped to the provided name.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="name"/> or <paramref name="scopedElement"/> was provided as null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> was provided as empty string.-or-<paramref name="name"/> provided would result in a 
    /// duplicate name registration.
    /// </exception>
    public void RegisterName(string name, object scopedElement)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (scopedElement is null)
        {
            throw new ArgumentNullException(nameof(scopedElement));
        }

        if (name == string.Empty)
        {
            throw new ArgumentException(Strings.NameScopeNameNotEmptyString);
        }

        if (_nameMap is null)
        {
            _nameMap = new Dictionary<string, object>
            {
                [name] = scopedElement,
            };
        }
        else
        {
            if (_nameMap.TryGetValue(name, out object nameContext))
            {
                if (scopedElement != nameContext)
                {
                    throw new ArgumentException(string.Format(Strings.NameScopeDuplicateNamesNotAllowed, name));
                }
            }
            else
            {
                _nameMap[name] = scopedElement;
            }
        }
    }

    /// <summary>
    /// Removes a name-object mapping from the XAML namescope.
    /// </summary>
    /// <param name="name">
    /// The name of the mapping to remove.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="name"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> was provided as empty string.-or-<paramref name="name"/> provided had not been registered.
    /// </exception>
    public void UnregisterName(string name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (name == string.Empty)
        {
            throw new ArgumentException(Strings.NameScopeNameNotEmptyString);
        }

        if (_nameMap is null || !_nameMap.Remove(name))
        {
            throw new ArgumentException(string.Format(Strings.NameScopeNameNotFound, name), nameof(name));
        }
    }

    /// <summary>
    /// Identifies the NameScope attached property.
    /// </summary>
    public static readonly DependencyProperty NameScopeProperty =
        DependencyProperty.RegisterAttached(
            "NameScope",
            typeof(INameScope),
            typeof(NameScope),
            null);

    /// <summary>
    /// Provides the attached property get accessor for the NameScope attached property.
    /// </summary>
    /// <param name="dependencyObject">
    /// The object to get the XAML namescope from.
    /// </param>
    /// <returns>
    /// A XAML namescope, as an <see cref="INameScope"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="dependencyObject"/> is null.
    /// </exception>
    public static INameScope GetNameScope(DependencyObject dependencyObject)
    {
        if (dependencyObject is null)
        {
            throw new ArgumentNullException(nameof(dependencyObject));
        }

        return (INameScope)dependencyObject.GetValue(NameScopeProperty);
    }

    /// <summary>
    /// Provides the attached property set accessor for the NameScope attached property.
    /// </summary>
    /// <param name="dependencyObject">
    /// Object to change XAML namescope for.
    /// </param>
    /// <param name="value">
    /// The new XAML namescope, using an interface cast.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="dependencyObject"/> is null.
    /// </exception>
    public static void SetNameScope(DependencyObject dependencyObject, INameScope value)
    {
        if (dependencyObject is null)
        {
            throw new ArgumentNullException(nameof(dependencyObject));
        }

        dependencyObject.SetValue(NameScopeProperty, value);
    }

    internal static INameScope GetNameScope(IDependencyObject dependencyObject)
    {
        if (dependencyObject is null)
        {
            throw new ArgumentNullException(nameof(dependencyObject));
        }

        return (INameScope)dependencyObject.GetValue(NameScopeProperty);
    }

    internal static void SetNameScope(IDependencyObject dependencyObject, INameScope value)
    {
        if (dependencyObject is null)
        {
            throw new ArgumentNullException(nameof(dependencyObject));
        }

        dependencyObject.SetValue(NameScopeProperty, value);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => GetEnumerator();

    private IEnumerator<KeyValuePair<string, object>> GetEnumerator() => new Enumerator(_nameMap);

    /// <summary>
    /// Returns the number of items in the collection of mapped names in this <see cref="NameScope"/>.
    /// </summary>
    /// <returns>
    /// The number of items in the collection.
    /// </returns>
    public int Count => _nameMap?.Count ?? 0;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    /// <returns>
    /// Always returns false.
    /// </returns>
    public bool IsReadOnly => false;

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear() => _nameMap = null;

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional array that is the destination of the elements copied from the collection The array must have 
    /// zero-based indexing.
    /// </param>
    /// <param name="arrayIndex">
    /// The zero-based index in <paramref name="array"/> at which copying begins.
    /// </param>
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        if (_nameMap is null)
        {
            return;
        }

        foreach (KeyValuePair<string, object> entry in _nameMap)
        {
            array[arrayIndex++] = entry;
        }
    }

    /// <summary>
    /// Removes the specific object from the collection.
    /// </summary>
    /// <param name="item">
    /// The object to remove from the collection, specified as a <see cref="KeyValuePair{TKey, TValue}"/>
    /// (key is <see cref="string"/>, value is <see cref="object"/>).
    /// </param>
    /// <returns>
    /// true if item was successfully removed from the collection, otherwise false. Also returns false if the item was 
    /// not found in the collection.
    /// </returns>
    public bool Remove(KeyValuePair<string, object> item)
    {
        if (item.Key is null)
        {
            throw new ArgumentException(string.Format(Strings.ReferenceIsNull, "item.Key"), nameof(item));
        }

        object value = FindName(item.Key);
        if (value is not null && item.Value == value)
        {
            UnregisterName(item.Key);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="item">
    /// A <see cref="KeyValuePair{TKey, TValue}"/> (key is <see cref="string"/>, value is <see cref="object"/>) that represents 
    /// the name mapping to add to the XAML namescope.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Either or both components of <paramref name="item"/> are null.
    /// </exception>
    public void Add(KeyValuePair<string, object> item)
    {
        if (item.Key is null)
        {
            throw new ArgumentException(string.Format(Strings.ReferenceIsNull, "item.Key"), nameof(item));
        }
        if (item.Value is null)
        {
            throw new ArgumentException(string.Format(Strings.ReferenceIsNull, "item.Value"), nameof(item));
        }

        Add(item.Key, item.Value);
    }

    /// <summary>
    /// Determines whether the collection contains a specified item.
    /// </summary>
    /// <param name="item">
    /// The item to find in the collection, specified as a <see cref="KeyValuePair{TKey, TValue}"/>
    /// (key is <see cref="string"/>, value is <see cref="object"/>).
    /// </param>
    /// <returns>
    /// true if the specified <see cref="KeyValuePair{TKey, TValue}"/> identifies an existing mapping in this <see cref="NameScope"/>.
    /// false if the specified <see cref="KeyValuePair{TKey, TValue}"/> does not exist in the current <see cref="NameScope"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="item"/> is null.
    /// </exception>
    public bool Contains(KeyValuePair<string, object> item)
    {
        if (item.Key is null)
        {
            throw new ArgumentException(string.Format(Strings.ReferenceIsNull, "item.Key"), nameof(item));
        }

        return ContainsKey(item.Key);
    }

    /// <summary>
    /// Gets or sets the item with the specified key.
    /// </summary>
    /// <param name="key">
    /// The string name for the XAML name mapping to get or set.
    /// </param>
    /// <returns>
    /// The value of the object mapped by the XAML name provided as <paramref name="key"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="key"/> is provided as null.-or-<paramref name="value"/> is provided as null for a set operation.
    /// </exception>
    public object this[string key]
    {
        get
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return FindName(key);
        }
        set
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            RegisterName(key, value);
        }
    }

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="key">
    /// The string key, which is the name of the XAML namescope mapping to add.
    /// </param>
    /// <param name="value">
    /// The object value, which is the object reference of the XAML namescope mapping to add.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="key"/> or <paramref name="value"/> is null.
    /// </exception>
    public void Add(string key, object value)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        RegisterName(key, value);
    }

    /// <summary>
    /// Returns whether a provided name already exists in this <see cref="NameScope"/>.
    /// </summary>
    /// <param name="key">
    /// The string key to find.
    /// </param>
    /// <returns>
    /// true if the specified <paramref name="key"/> identifies a name for an existing mapping in this <see cref="NameScope"/>.
    /// false if the specified <paramref name="key"/> does not exist in the current <see cref="NameScope"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="key"/> is null.
    /// </exception>
    public bool ContainsKey(string key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return FindName(key) is not null;
    }

    /// <summary>
    /// Removes a mapping for a specified name from the collection.
    /// </summary>
    /// <param name="key">
    /// The string key, which is the name of the XAML namescope mapping to remove.
    /// </param>
    /// <returns>
    /// true if item was successfully removed from the collection, otherwise false. Also returns false if the item was not 
    /// found in the collection.
    /// </returns>
    public bool Remove(string key)
    {
        if (ContainsKey(key))
        {
            UnregisterName(key);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">
    /// The key of the value to get.
    /// </param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key, if the key is found; otherwise, a null object.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// true if the <see cref="NameScope"/> contains a mapping for the name provided as <paramref name="key"/>. Otherwise, false.
    /// </returns>
    public bool TryGetValue(string key, out object value)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        value = FindName(key);
        return value is not null;
    }

    /// <summary>
    /// Gets a collection of the keys in the <see cref="NameScope"/> dictionary.
    /// </summary>
    /// <returns>
    /// A collection of the keys in the <see cref="NameScope"/> dictionary.
    /// </returns>
    public ICollection<string> Keys
    {
        get
        {
            if (_nameMap is null)
            {
                return null;
            }

            return [.. _nameMap.Keys];
        }
    }

    /// <summary>
    /// Gets a collection of the values in the <see cref="NameScope"/> dictionary.
    /// </summary>
    /// <returns>
    /// A collection of the values in the <see cref="NameScope"/> dictionary.
    /// </returns>
    public ICollection<object> Values
    {
        get
        {
            if (_nameMap is null)
            {
                return null;
            }

            return [.. _nameMap.Values];
        }
    }

    private sealed class Enumerator : IEnumerator<KeyValuePair<string, object>>
    {
        private readonly IEnumerator<KeyValuePair<string, object>> _enumerator;

        public Enumerator(Dictionary<string, object> nameMap)
        {
            if (nameMap is not null)
            {
                _enumerator = nameMap.GetEnumerator();
            }
        }

        public KeyValuePair<string, object> Current => _enumerator?.Current ?? default;

        public bool MoveNext() => _enumerator is not null && _enumerator.MoveNext();

        void IDisposable.Dispose() { }

        object IEnumerator.Current => Current;

        void IEnumerator.Reset() => _enumerator?.Reset();
    }
}
