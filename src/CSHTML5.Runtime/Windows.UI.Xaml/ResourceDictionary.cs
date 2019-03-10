
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Markup;
#else
using Windows.UI.Xaml.Markup;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Defines a dictionary that contains resources used by components of the app.
    /// This dictionary is oriented toward defining the resources in XAML, and then
    /// retrieving them through XAML references via the StaticResource markup extension.
    /// Alternatively you can access resources by traversing the dictionary at run
    /// time.
    /// </summary>
    public class ResourceDictionary : DependencyObject,

        // TODOBRIDGE: Fix that
#if !BRIDGE
        IDictionary, 
#endif

        IDictionary<object, object>, IEnumerable<KeyValuePair<object, object>>, INameScope
    {
        Dictionary<object, object> _resourcesContainer;
        ObservableCollection<ResourceDictionary> _mergedDictionaries;
        Dictionary<object, ResourceDictionary> _themeDictionaries;
        /// <summary>
        /// Do not use. Says whether this instance of ResourceDictionary contains the definition of at least one implicit style (including in the MergedDictionaries)
        /// </summary>
        public bool INTERNAL_HasImplicitStyles = false; //Unfortunately I had to make this public since we want to set it in the .xaml.g.cs

        /// <summary>
        /// Initializes a new instance of the ResourceDictionary class.
        /// </summary>
        public ResourceDictionary()
        {
            _resourcesContainer = new Dictionary<object, object>();
        }

        /// <summary>
        /// Gets the amount of elements in the ResourceDictionary
        /// </summary>
        public int Count
        {
            get { return _resourcesContainer.Count; }
        }


        /// <summary>
        /// Returns a list of the keys in the ResourceDictionary
        /// </summary>
        public ICollection<object> Keys { get { return _resourcesContainer.Keys; } }


        ///<summary>
        /// Gets a collection of the ResourceDictionary dictionaries that constitute
        /// the various resource dictionaries in the merged dictionaries.
        ///</summary>
        public ObservableCollection<ResourceDictionary> MergedDictionaries
        {
            get
            {
                if (_mergedDictionaries == null)
                {
                    _mergedDictionaries = new ObservableCollection<ResourceDictionary>();
                    _mergedDictionaries.CollectionChanged += _mergedDictionaries_CollectionChanged;
                }
                return _mergedDictionaries;
            }
        }

        void _mergedDictionaries_CollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //todo: support removing items from the MergedDictionary? (might be complicated or maybe not supported in the MS version)

            foreach (var item in e.NewItems)
            {
                if (item is ResourceDictionary)
                {
                    var resourceDictionary = (ResourceDictionary)item;
                    this.INTERNAL_HasImplicitStyles = this.INTERNAL_HasImplicitStyles || resourceDictionary.INTERNAL_HasImplicitStyles;
                    foreach (KeyValuePair<object, object> keyValuePair in resourceDictionary)
                    {
                        this[keyValuePair.Key] = keyValuePair.Value; // Note: we use the dictionary["key"] = value syntax instead of the Add("key", value") syntax because in case of multiple dictionaries in the "MergedDictionaries" property, we want to override existing elements with the same key if any.
                    }
                }
                else
                    throw new Exception("The property ResourceDictionary.MergedDictionaries can only contain objects of type ResourceDictionary.");
            }
        }

        /// <summary>
        /// Gets a collection of merged resource dictionaries that are specifically keyed and composed to address theme scenarios
        /// </summary>
        public IDictionary<object, ResourceDictionary> ThemeDictionaries //todo: this is supposed to be IDictionary<object, object> but casts from Dictionary<object, ResourceDictionary> to IDictionary<object, object> are not allowed apparently.
        {
            get
            {
                if (_themeDictionaries == null)
                {
                    _themeDictionaries = new Dictionary<object, ResourceDictionary>();
                }
                return (IDictionary<object, ResourceDictionary>)_themeDictionaries;
            }
        }

        /// <summary>
        /// Gets or sets a Uniform Resource Identifier (URI) that provides the source
        /// location of a merged resource dictionary.
        /// </summary>
        public Uri Source { get; set; }   // NOTE: This is used during COMPILE-TIME only.


        // Summary:
        //     Gets a collection of merged resource dictionaries that are specifically keyed
        //     and composed to address theme scenarios, for example supplying theme values
        //     for HighContrast.
        //
        // Returns:
        //     A dictionary of ResourceDictionary theme dictionaries. Each must be keyed
        //     with x:Key.
        //public IDictionary<object, object> ThemeDictionaries { get; }
        /// <summary>
        /// Gets an ICollection containing a List of the values in this ResourceDictionary.
        /// </summary>
        public ICollection<object> Values
        {
            get
            {
#if BRIDGE
                return INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_resourcesContainer).ToList();
#else
                return _resourcesContainer.Values;
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the given key.
        /// </summary>
        /// <param name="key">The desired key to get or set.</param>
        /// <returns>Value of the key.</returns>
        public object this[object key]
        {
            get
            {
                return _resourcesContainer[key];
            }
            set
            {
                _resourcesContainer[key] = value;
            }
        }

        /// <summary>
        /// Adds the specified key and value to the ResourceDictionary.
        /// </summary>
        /// <param name="item">A KeyValuePair that contains the key and the value of the element to add.</param>
        public void Add(KeyValuePair<object, object> item)
        {
            _resourcesContainer.Add(item.Key, item.Value);
        }
        /// <summary>
        /// Adds the specified key and value to the ResourceDictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public void Add(object key, object value)
        {
            _resourcesContainer.Add(key, value);
        }
        /// <summary>
        /// Removes all items from this ResourceDictionary.
        /// </summary>
        public void Clear()
        {
            _resourcesContainer.Clear();
        }
        /// <summary>
        /// Determines whether the ResourceDictionary contains a specified KeyValuePair using the default equality comparer.
        /// </summary>
        /// <param name="item">The KeyValuePair of which presence is checked whithin the ResourceDictionary.</param>
        /// <returns>true if the KeyValuePair was found, false otherwise.</returns>
        public bool Contains(KeyValuePair<object, object> item)
        {
            return _resourcesContainer.Contains(item);
        }

        /// <summary>
        /// Determines whether the ResourceDictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the ResourceDictionary.</param>
        /// <returns>
        /// true if the ResourceDictionary contains an
        /// element with the specified key; otherwise, false.
        /// </returns>
        public bool ContainsKey(object key)
        {
            return _resourcesContainer.ContainsKey(key);
        }

        /// <summary>
        /// Copies the ResourceDictionary.KeyCollection
        /// elements to an existing one-dimensional System.Array, starting at the specified
        /// array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements
        /// copied from ResourceDictionary.KeyCollection.
        /// The System.Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            if (_resourcesContainer != null)
            {
                for (int i = 0; i < _resourcesContainer.Count; ++i)
                {
                    array[arrayIndex + i] = _resourcesContainer.ElementAt(i);
                }
            }
        }

        /// <summary>
        /// Removes the specified KeyValuePair from the ResourceDictionary.
        /// </summary>
        /// <param name="item">The KeyValuePair to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false.
        /// </returns>
        public bool Remove(KeyValuePair<object, object> item)
        {
            return _resourcesContainer.Remove(item.Key);
        }
        /// <summary>
        /// Removes the value with the specified key from the ResourceDictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false.
        /// This method returns false if key is not found in the ResourceDictionary.
        /// </returns>
        public bool Remove(object key)
        {
            return _resourcesContainer.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified
        /// key, if the key is found; otherwise, the default value for the type of the
        /// value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the ResourceDictionary contains an
        /// element with the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(object key, out object value)
        {
            return _resourcesContainer.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the ResourceDictionary.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.Dictionary{object,object}.Enumerator structure for the ResourceDictionary.
        /// </returns>
        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            //todo (maybe): change the enumerator so that it also goes through the elements of the merged Dictionaries

#if BRIDGE
            return INTERNAL_BridgeWorkarounds.GetDictionaryEnumerator(_resourcesContainer);
#else
            return _resourcesContainer.GetEnumerator();
#endif
        }

        /// <summary>
        /// Returns an enumerator that iterates through the ResourceDictionary.
        /// </summary>
        /// <returns>A System.Collections.Generic.Dictionary{object,object}.Enumerator structure for the ResourceDictionary.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            //todo (maybe): change the enumerator so that it also goes through the elements of the merged Dictionaries

#if BRIDGE
            return INTERNAL_BridgeWorkarounds.GetDictionaryEnumerator(_resourcesContainer);
#else
            return _resourcesContainer.GetEnumerator();
#endif
        }

        /// <summary>
        /// Adds the specified key and value to the IDictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        void IDictionary<object, object>.Add(object key, object value)
        {
            Add(key, value);
        }

        /// <summary>
        /// Determines whether the IDictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the IDictionary.</param>
        /// <returns>
        /// true if the IDictionary contains an element with the specified key; otherwise, false.
        /// </returns>
        bool IDictionary<object, object>.ContainsKey(object key)
        {
            return ContainsKey(key);
        }

        ICollection<object> IDictionary<object, object>.Keys
        {
            get { return Keys; }
        }

        bool IDictionary<object, object>.Remove(object key)
        {
            return Remove(key);
        }

        bool IDictionary<object, object>.TryGetValue(object key, out object value)
        {
            return TryGetValue(key, out value);
        }

        ICollection<object> IDictionary<object, object>.Values
        {
            get
            {
#if BRIDGE
                return INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_resourcesContainer).ToList();
#else
                return _resourcesContainer.Values;
#endif
            }
        }


#if !BRIDGE // in bridge, this function is translated exactly as public object this[object key] previously defined
        object IDictionary<object, object>.this[object key]
        {
            get
            {
                return _resourcesContainer[key];
            }
            set
            {
                _resourcesContainer[key] = value;
            }
        }
#endif

        void ICollection<KeyValuePair<object, object>>.Add(KeyValuePair<object, object> item)
        {
            Add(item);
        }

        void ICollection<KeyValuePair<object, object>>.Clear()
        {
            Clear();
        }

        bool ICollection<KeyValuePair<object, object>>.Contains(KeyValuePair<object, object> item)
        {
            return Contains(item);
        }

        void ICollection<KeyValuePair<object, object>>.CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets a value indicating whether the ResourceDictionary can be changed or not.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; } //todo: implement this?
        }

        bool ICollection<KeyValuePair<object, object>>.Remove(KeyValuePair<object, object> item)
        {
            return Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo(array, index);
        }

        /// <summary>
        /// Gets a value indicating whether access to the System.Collections.ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; } //todo: implement this?
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the System.Collections.ICollection.
        /// </summary>
        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

#region IDictionary implementation
        /// <summary>
        /// Determines whether the System.Collections.IDictionary object contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the System.Collections.IDictionary object.</param>
        /// <returns>true if the System.Collections.IDictionary contains an element with the key; otherwise, false.</returns>
        public bool Contains(object key)
        {
            return _resourcesContainer.ContainsKey(key);
        }

        // TODO  : disable because of IDictionary bridge
#if !BRIDGE
        /// <summary>
        /// Returns an System.Collections.IDictionaryEnumerator object for the System.Collections.IDictionary object.
        /// </summary>
        /// <returns>
        /// An System.Collections.IDictionaryEnumerator object for the System.Collections.IDictionary object.
        /// </returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator)_resourcesContainer.GetEnumerator();
        }
#endif

        /// <summary>
        /// Gets a value indicating whether the System.Collections.IDictionary object has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; } //todo: implement this?
        }

#if !BRIDGE
        /// <summary>
        /// Gets an System.Collections.ICollection object containing the keys of the System.Collections.IDictionary object.
        /// </summary>
        ICollection IDictionary.Keys
        {
            get { return (ICollection)_resourcesContainer.Keys; }
        }
#endif

        /// <summary>
        /// Removes the element with the specified key from the System.Collections.IDictionary object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>

        //BRIDGETODO  : disable because of IDictionary bridge
#if !BRIDGE
        void IDictionary.Remove(object key) 
        {
            _resourcesContainer.Remove(key);
        }

#endif
        /// <summary>
        /// Gets an System.Collections.ICollection object containing the values in the System.Collections.IDictionary object.
        /// </summary>
#if !BRIDGE
        ICollection IDictionary.Values
        {
            get { return (ICollection)_resourcesContainer.Values; }
        }
#endif

        #endregion

        #region ---------- INameScope implementation ----------

        Dictionary<string, object> _nameScopeDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Finds the UIElement with the specified name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The object with the specified name if any; otherwise null.</returns>
        public new object FindName(string name)
        {
            if (_nameScopeDictionary.ContainsKey(name))
                return _nameScopeDictionary[name];
            else
                return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            if (_nameScopeDictionary.ContainsKey(name) && _nameScopeDictionary[name] != scopedElement)
                throw new ArgumentException(string.Format("Cannot register duplicate name '{0}' in this scope.", name));

            _nameScopeDictionary[name] = scopedElement;
        }

        public void UnregisterName(string name)
        {
            if (!_nameScopeDictionary.ContainsKey(name))
                throw new ArgumentException(string.Format("Name '{0}' was not found.", name));

            _nameScopeDictionary.Remove(name);
        }

#endregion
    }
}