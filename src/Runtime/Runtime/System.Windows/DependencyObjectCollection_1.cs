
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    // Note : The constraint 'where T : DependencyObject' is not present in Silverlight.
    // However, the code will crash if we try to do operations on the collection like adding 
    // an object that is not a DependencyObject.

    /// <summary>
    /// Represents a collection of System.Windows.DependencyObject instances of a specified
    /// type.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public partial class DependencyObjectCollection<T> : DependencyObject, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged where T : DependencyObject
    {
        #region Data

        private SimpleMonitor _monitor = new SimpleMonitor();
        private readonly DependencyObjectCollectionInternal _collection;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObjectCollection{T}"/>
        /// class.
        /// </summary>
        public DependencyObjectCollection()
        {
            this._collection = new DependencyObjectCollectionInternal(this);
        }

        #endregion

        #region Events (CollectionChanged)

        /// <summary>
        /// Occurs when items in the collection are added, removed, or replaced.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Public Properties

        public T this[int index]
        {
            get
            {
                return (T)this._collection[index];
            }
            set
            {
                this.CheckReentrancy();
                object originalItem = this._collection[index]; // skips unnecessary cast
                this._collection[index] = value;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, originalItem, index));
            }
        }

        /// <summary>
        /// Gets the number of objects in the collection.
        /// </summary>
        public int Count
        {
            get { return this._collection.CountInternal; }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection can be modified.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Public Methods

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
        /// The method is being called in a <see cref="DependencyObjectCollection{T}.CollectionChanged"/> 
        /// event handler.
        /// </exception>
        public void Add(T item)
        {
            this.CheckReentrancy();
            this._collection.Add(item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, this.Count - 1));
        }

        /// <summary>
        /// Removes all objects from the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The method is being called in a <see cref="DependencyObjectCollection{T}.CollectionChanged"/> 
        /// event handler. 
        /// </exception>
        public void Clear()
        {
            this.CheckReentrancy();
            this._collection.Clear();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Gets a value that indicates whether the specified object is in the collection.
        /// </summary>
        /// <param name="item">The object to check for in the collection.</param>
        /// <returns>true if the object is in the collection; otherwise, false.</returns>
        /// <exception cref="ArgumentException">item is not a <see cref="DependencyObject"/>.</exception>
        public bool Contains(T item)
        {
            if (!(item is DependencyObject))
            {
                throw new ArgumentException("item is not a DependencyObject !");
            }
            return this._collection.Contains(item);
        }

        /// <summary>
        /// Gets an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._collection.Select(d => (T)d).GetEnumerator();
        }

        /// <summary>
        /// Gets the index of the specified object within the collection, or -1 if the object
        /// is not in the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>
        /// The index of the first occurrence of item within the collection, or -1 if item
        /// is not in the collection.
        /// </returns>
        /// <exception cref="ArgumentException">item is not a <see cref="DependencyObject"/>.</exception>
        public int IndexOf(T item)
        {
            if (!(item is DependencyObject))
            {
                throw new ArgumentException("item is not a DependencyObject !");
            }
            return this._collection.IndexOf(item);
        }

        /// <summary>
        /// Adds the specified object to the collection at the specified index.
        /// </summary>
        /// <param name="index">The index at which to add the object.</param>
        /// <param name="item">The object to add.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than zero or greater than the number of items in the collection.</exception>
        /// <exception cref="ArgumentException">item is not a <see cref="DependencyObject"/>.</exception>
        /// <exception cref="InvalidOperationException">The method is being called in a <see cref="DependencyObjectCollection{T}.CollectionChanged"/> event handler.</exception>
        public void Insert(int index, T item)
        {
            this.CheckReentrancy();
            this._collection.Insert(index, item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Removes the specified object from the collection.
        /// </summary>
        /// <param name="item">The object to remove.</param>
        /// <returns>true if the object was removed; false if the object was not found in the collection.</returns>
        /// <exception cref="ArgumentException">item is not a <see cref="DependencyObject"/>.</exception>
        /// <exception cref="InvalidOperationException">The method is being called in a <see cref="DependencyObjectCollection{T}.CollectionChanged"/> event handler.</exception>
        public bool Remove(T item)
        {
            this.CheckReentrancy();
            int index = this.IndexOf(item);
            if (index > -1)
            {
                object removedItem = this._collection[index]; // skips unnecessary cast
                this._collection.RemoveAt(index);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the object at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index of the object to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than zero or greater than the highest index in the collection.</exception>
        /// <exception cref="InvalidOperationException">The method is being called in a <see cref="DependencyObjectCollection{T}.CollectionChanged"/> event handler.</exception>
        public void RemoveAt(int index)
        {
            this.CheckReentrancy();
            T removedItem = this[index];
            this._collection.RemoveAt(index);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        #endregion

        #region Explicit interface implementation

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = this.Cast(value); }
        }

        bool IList.IsFixedSize
        {
            get { return this.IsReadOnly; }
        }

        object ICollection.SyncRoot
        {
            get { return this._collection; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            this._collection.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            this._collection.CopyTo(array, arrayIndex);
        }

        int IList.Add(object item)
        {
            this.Add(this.Cast(item));
            return this.Count;
        }

        bool IList.Contains(object item)
        {
            return this.Contains(this.Cast(item));
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf(this.Cast(value));
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, this.Cast(value));
        }

        void IList.Remove(object value)
        {
            this.Remove(this.Cast(value));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Internal Methods

        private T Cast(object item)
        {
            if (!(item is T castedItem))
            {
                throw new ArgumentException(string.Format("item is not a {0}.", typeof(DependencyObject)));
            }
            return castedItem;
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                using (this.BlockReentrancy())
                {
                    this.CollectionChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Disallow reentrant attempts to change this collection. E.g. a event handler
        /// of the CollectionChanged event is not allowed to make changes to this collection.
        /// </summary>
        /// <remarks>
        /// typical usage is to wrap e.g. a OnCollectionChanged call with a using() scope:
        /// <code>
        ///         using (BlockReentrancy())
        ///         {
        ///             CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
        ///         }
        /// </code>
        /// </remarks>
        private IDisposable BlockReentrancy()
        {
            this._monitor.Enter();
            return this._monitor;
        }

        /// <summary> Check and assert for reentrant attempts to change this collection. </summary>
        /// <exception cref="InvalidOperationException"> raised when changing the collection
        /// while another collection change is still being notified to other listeners </exception>
        private void CheckReentrancy()
        {
            if (this._monitor.Busy)
            {
                // we can allow changes if there's only one listener - the problem
                // only arises if reentrant changes make the original event args
                // invalid for later listeners.  This keeps existing code working
                // (e.g. Selector.SelectedItems).
                if ((this.CollectionChanged != null) && (this.CollectionChanged.GetInvocationList().Length > 1))
                {
                    throw new InvalidOperationException("DependencyObjectCollection Reentrancy not allowed");
                }
            }
        }

        #endregion

        #region Private classes

        private class SimpleMonitor : IDisposable
        {
            public void Enter()
            {
                ++this._busyCount;
            }

            public void Dispose()
            {
                --this._busyCount;
            }

            public bool Busy
            {
                get { return this._busyCount > 0; }
            }

            private int _busyCount;
        }

        #endregion

    }

    internal class DependencyObjectCollectionInternal : PresentationFrameworkCollection<DependencyObject>
    {
        internal DependencyObjectCollectionInternal(DependencyObject owner) : base(false)
        {
            owner.ProvideSelfAsInheritanceContext(this, null);
            this.IsInheritanceContextSealed = true;
        }

        internal override void AddOverride(DependencyObject value)
        {
            this.AddDependencyObjectInternal(value);
        }

        internal override void ClearOverride()
        {
            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, DependencyObject value)
        {
            this.InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override DependencyObject GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, DependencyObject value)
        {
            this.SetItemDependencyObjectInternal(index, value);
        }
    }
}
