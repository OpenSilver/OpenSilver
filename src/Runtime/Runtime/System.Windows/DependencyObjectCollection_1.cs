
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace System.Windows
{
    /// <summary>
    /// Represents a collection of <see cref="DependencyObject"/> instances of a specified
    /// type.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items in the collection.
    /// </typeparam>
    public class DependencyObjectCollection<T> : DependencyObject, IList<T>, IList, INotifyCollectionChanged
    {
        private readonly DependencyObjectCollectionInternal _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObjectCollection{T}"/>
        /// class.
        /// </summary>
        public DependencyObjectCollection()
        {
            _collection = new DependencyObjectCollectionInternal(this);
            _collection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        /// <summary>
        /// Occurs when items in the collection are added, removed, or replaced.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the object to get or set.
        /// </param>
        /// <returns>
        /// The object at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero or greater than the number of items in the collection.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified value when setting this property is not a <see cref="DependencyObject"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The property is being set in a <see cref="CollectionChanged"/> event handler.
        /// </exception>
        public T this[int index]
        {
            get => AsT(_collection[index]);
            set => _collection[index] = AsDependencyObject(value);
        }

        /// <summary>
        /// Gets the number of objects in the collection.
        /// </summary>
        public int Count => _collection.CountInternal;

        /// <summary>
        /// Gets a value that indicates whether the collection can be modified.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds the specified object to the end of the collection.
        /// </summary>
        /// <param name="item">
        /// The object to add to the collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// item is not a <see cref="DependencyObject"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is being called in a <see cref="CollectionChanged"/> 
        /// event handler.
        /// </exception>
        public void Add(T item) => _collection.Add(AsDependencyObject(item));

        /// <summary>
        /// Removes all objects from the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The method is being called in a <see cref="CollectionChanged"/> 
        /// event handler. 
        /// </exception>
        public void Clear() => _collection.Clear();

        /// <summary>
        /// Gets a value that indicates whether the specified object is in the collection.
        /// </summary>
        /// <param name="item">
        /// The object to check for in the collection.
        /// </param>
        /// <returns>
        /// true if the object is in the collection; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// item is not a <see cref="DependencyObject"/>.
        /// </exception>
        public bool Contains(T item) => _collection.Contains(AsDependencyObject(item));

        /// <summary>
        /// Gets an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator for the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() => _collection.Select(d => AsT(d)).GetEnumerator();

        /// <summary>
        /// Gets the index of the specified object within the collection, or -1 if the object
        /// is not in the collection.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the collection.
        /// </param>
        /// <returns>
        /// The index of the first occurrence of item within the collection, or -1 if item
        /// is not in the collection.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// item is not a <see cref="DependencyObject"/>.
        /// </exception>
        public int IndexOf(T item) => _collection.IndexOf(AsDependencyObject(item));

        /// <summary>
        /// Adds the specified object to the collection at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index at which to add the object.
        /// </param>
        /// <param name="item">
        /// The object to add.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero or greater than the number of items in the collection.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// item is not a <see cref="DependencyObject"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is being called in a <see cref="CollectionChanged"/> event handler.
        /// </exception>
        public void Insert(int index, T item) => _collection.Insert(index, AsDependencyObject(item));

        /// <summary>
        /// Removes the specified object from the collection.
        /// </summary>
        /// <param name="item">
        /// The object to remove.
        /// </param>
        /// <returns>
        /// true if the object was removed; false if the object was not found in the collection.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// item is not a <see cref="DependencyObject"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is being called in a <see cref="CollectionChanged"/> event handler.
        /// </exception>
        public bool Remove(T item) => _collection.Remove(AsDependencyObject(item));

        /// <summary>
        /// Removes the object at the specified index from the collection.
        /// </summary>
        /// <param name="index">
        /// The index of the object to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero or greater than the highest index in the collection.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is being called in a <see cref="CollectionChanged"/> event handler.
        /// </exception>
        public void RemoveAt(int index) => _collection.RemoveAt(index);

        /// <summary>
        /// Copies the objects in the collection to the specified array, starting at the
        /// specified index.
        /// </summary>
        /// <param name="array">
        /// The destination array.
        /// </param>
        /// <param name="arrayIndex">
        /// The index of the first object to copy.
        /// </param>
        public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        object IList.this[int index]
        {
            get => _collection[index];
            set => _collection[index] = AsDependencyObject(value);
        }

        bool IList.IsFixedSize => false;

        object ICollection.SyncRoot => _collection;

        bool ICollection.IsSynchronized => false;

        void ICollection.CopyTo(Array array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        int IList.Add(object item)
        {
            _collection.Add(AsDependencyObject(item));
            return Count;
        }

        bool IList.Contains(object item) => _collection.Contains(AsDependencyObject(item));

        int IList.IndexOf(object value) => _collection.IndexOf(AsDependencyObject(value));

        void IList.Insert(int index, object value) => _collection.Insert(index, AsDependencyObject(value));

        void IList.Remove(object value) => _collection.Remove(AsDependencyObject(value));

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        private static DependencyObject AsDependencyObject(object item)
        {
            if (!(item is DependencyObject) && item != null)
            {
                throw new ArgumentException("item is not a DependencyObject.");
            }

            return (DependencyObject)item;
        }

        private static T AsT(object item)
        {
            return (T)item;
        }
    }

    internal sealed class DependencyObjectCollectionInternal : PresentationFrameworkCollection<DependencyObject>
    {
        internal DependencyObjectCollectionInternal(DependencyObject owner) : base(true)
        {
            owner.ProvideSelfAsInheritanceContext(this, null);
            IsInheritanceContextSealed = true;
        }

        internal override void AddOverride(DependencyObject value) => AddDependencyObjectInternal(value);

        internal override void ClearOverride() => ClearDependencyObjectInternal();

        internal override void InsertOverride(int index, DependencyObject value) => InsertDependencyObjectInternal(index, value);

        internal override void RemoveAtOverride(int index) => RemoveAtDependencyObjectInternal(index);

        internal override DependencyObject GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, DependencyObject value) => SetItemDependencyObjectInternal(index, value);
    }
}
