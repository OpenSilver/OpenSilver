
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
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides a common collection class for Silverlight collections.
    /// </summary>
    /// <typeparam name="T">Type constraint for type safety of the constrained collection implementation.</typeparam>
    public abstract partial class PresentationFrameworkCollection<T> : DependencyObject, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        private readonly List<T> _collection;
        private readonly bool _processCollectionChanged;
        private readonly SimpleMonitor _monitor = new SimpleMonitor();

        internal PresentationFrameworkCollection(bool processCollectionChanged)
        {
            this._processCollectionChanged = processCollectionChanged;
            this._collection = new List<T>();
            this.UpdateCountProperty();
        }

        internal PresentationFrameworkCollection(int capacity, bool processCollectionChanged)
        {
            this._processCollectionChanged = processCollectionChanged;
            this._collection = new List<T>(capacity);
            this.UpdateCountProperty();
        }

        internal PresentationFrameworkCollection(IEnumerable<T> source, bool processCollectionChanged)
        {
            this._processCollectionChanged = processCollectionChanged;
            this._collection = new List<T>(source);
            this.UpdateCountProperty();
        }

        private static readonly DependencyPropertyKey CountPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Count),
                typeof(int),
                typeof(PresentationFrameworkCollection<T>),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <see cref="Count"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CountProperty = CountPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="PresentationFrameworkCollection{T}"/>.
        /// </summary>
        public int Count
        {
            get { return this.CountInternal; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        public T this[int index]
        {
            get 
            {
                if (index < 0 || index >= this.CountInternal)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return this.GetItemOverride(index); 
            }
            set 
            {
                this.CheckReentrancy();

                if (null == value)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (index < 0 || index >= this.CountInternal)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                T originalItem = this[index];

                this.SetItemOverride(index, value);

                this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, value, index);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PresentationFrameworkCollection{T}"/>
        /// has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="PresentationFrameworkCollection{T}"/> has a fixed size;
        /// otherwise, false.
        /// </returns>
        public bool IsFixedSize
        {
            get
            {
                return this.IsFixedSizeImpl;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PresentationFrameworkCollection{T}"/>
        /// is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="PresentationFrameworkCollection{T}"/> is read-only; otherwise,
        /// false.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return this.IsReadOnlyImpl;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="PresentationFrameworkCollection{T}"/>
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="PresentationFrameworkCollection{T}"/> is synchronized
        /// (thread safe); otherwise, false.
        /// </returns>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="PresentationFrameworkCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="PresentationFrameworkCollection{T}"/>.
        /// </returns>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through
        /// the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.GetEnumeratorImpl();
        }

        /// <summary>
        /// Adds an item to the <see cref="PresentationFrameworkCollection{T}"/>.
        /// </summary>
        /// <param name="value">The object to add.</param>
        public void Add(T value)
        {
            this.CheckReentrancy();

            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.AddOverride(value);

            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, this.CountInternal - 1);
        }

        /// <summary>
        /// Removes all items from the <see cref="PresentationFrameworkCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            this.CheckReentrancy();

            this.ClearOverride();

            this.OnCollectionReset();
        }

        /// <summary>
        /// Determines whether the <see cref="PresentationFrameworkCollection{T}"/> contains
        /// a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="PresentationFrameworkCollection{T}"/>.</param>
        /// <returns>
        /// true if the object is found in the <see cref="PresentationFrameworkCollection{T}"/>;
        /// otherwise, false.
        /// </returns>
        public bool Contains(T value)
        {
            return this.ContainsImpl(value);
        }

        /// <summary>
        /// Copies the elements of the <see cref="PresentationFrameworkCollection{T}"/> to
        /// an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="PresentationFrameworkCollection{T}"/>. The <see cref="Array"/> must
        /// have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            this.CopyToImpl(array, index);
        }

        /// <summary>
        /// Copies the elements of the <see cref="PresentationFrameworkCollection{T}"/> to
        /// an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="PresentationFrameworkCollection{T}"/>. The <see cref="Array"/> must
        /// have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int index)
        {
            this.CopyToImpl(array, index);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="PresentationFrameworkCollection{T}"/>.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="PresentationFrameworkCollection{T}"/>.</param>
        /// <returns>The index of value if found in the list; otherwise, an exception.</returns>
        /// <exception cref="ArgumentException">
        /// The object was not found in the list.
        /// </exception>
        public int IndexOf(T value)
        {
            return this.IndexOfImpl(value);
        }

        /// <summary>
        /// Inserts an item to the <see cref="PresentationFrameworkCollection{T}"/> at the
        /// specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The object to insert into the <see cref="PresentationFrameworkCollection{T}"/>.</param>
        public void Insert(int index, T value)
        {
            this.CheckReentrancy();

            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (index < 0 || index > this.CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.InsertOverride(index, value);

            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, index);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="PresentationFrameworkCollection{T}"/>.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="PresentationFrameworkCollection{T}"/>.</param>
        /// <returns>true if an object was removed; otherwise, false.</returns>
        public bool Remove(T value)
        {
            int index = this.IndexOf(value);

            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            this.CheckReentrancy();
            
            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            T removedItem = this[index];

            this.RemoveAtOverride(index);

            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        #region Explicit Interface implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        int IList.Add(object value)
        {
            this.Add((T)value);
            return this.CountInternal;
        }

        void IList.Remove(object value)
        {
            this.Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (T)value);
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf((T)value);
        }

        bool IList.Contains(object value)
        {
            return this.Contains((T)value);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (T)value;
            }
        }

        #endregion

        #region Abstract/Virtual Members

        internal abstract void AddOverride(T value);

        internal abstract void ClearOverride();

        internal abstract void InsertOverride(int index, T value);

        internal abstract void RemoveAtOverride(int index);

        internal abstract void SetItemOverride(int index, T value);

        internal abstract T GetItemOverride(int index);

        internal virtual bool IsFixedSizeImpl
        {
            get { return false; }
        }

        internal virtual bool IsReadOnlyImpl
        {
            get { return false; }
        }

        /// <summary>
        /// Get the Count of the underlying <see cref="List{T}"/> collection.
        /// This property returns the same value as the Count property and is only here
        /// for performances.
        /// </summary>
        internal virtual int CountInternal
        {
            get { return this._collection.Count; }
        }

        internal virtual void CopyToImpl(Array array, int index)
        {
            for (int i = 0; i < this.CountInternal; ++i)
            {
                array.SetValue(this[i], index + i);
            }
        }

        internal virtual bool ContainsImpl(T value)
        {
            return this._collection.Contains(value);
        }

        internal virtual int IndexOfImpl(T value)
        {
            return this._collection.IndexOf(value);
        }

        internal virtual IEnumerator<T> GetEnumeratorImpl()
        {
            return this._collection.GetEnumerator();
        }

        #endregion

        #region Generic collection manipulation methods

        /// <summary>
        /// Call the Add method of underlying <see cref="List{T}"/> collection.
        /// </summary>
        /// <param name="value"></param>
        internal void AddInternal(T value)
        {
            this._collection.Add(value);
            this.UpdateCountProperty();
        }

        /// <summary>
        /// Call the Clear method of underlying <see cref="List{T}"/> collection.
        /// </summary>
        internal void ClearInternal()
        {
            this._collection.Clear();
            this.UpdateCountProperty();
        }

        /// <summary>
        /// Call the Insert method of underlying <see cref="List{T}"/> collection.
        /// </summary>
        internal void InsertInternal(int index, T value)
        {
            this._collection.Insert(index, value);
            this.UpdateCountProperty();
        }

        /// <summary>
        /// Call the RemoveAt method of underlying <see cref="List{T}"/> collection.
        /// </summary>
        internal void RemoveAtInternal(int index)
        {
            this._collection.RemoveAt(index);
            this.UpdateCountProperty();
        }

        /// <summary>
        /// Call the Remove method of underlying <see cref="List{T}"/> collection.
        /// </summary>
        internal bool RemoveInternal(T value)
        {
            if (this._collection.Remove(value))
            {
                this.UpdateCountProperty();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call the Indexer (getter) property of underlying <see cref="List{T}"/> collection.
        /// </summary>
        internal T GetItemInternal(int index)
        {
            return this._collection[index];
        }

        /// <summary>
        /// Call the Indexer (setter) property of underlying <see cref="List{T}"/> collection.
        /// </summary>
        internal void SetItemInternal(int index, T value)
        {
            this._collection[index] = value;
        }

        #endregion

        #region DependencyObject collection manipulation methods

        // ----------------------------------------------------------------- //
        // The following methods should be used when you want the elements 
        // of this collection to inherit context from this object. This 
        // enable binding for properties defined in DependencyObjects who are 
        // not FrameworkElements (and have no DataContext as a consequence).
        // ----------------------------------------------------------------- //

        private static DependencyObject CastDO(T value)
        {
            if (!(value is DependencyObject valueDO))
            {
                throw new ArgumentException(string.Format("'{0}' does not derive from {1}.", typeof(T).Name, typeof(DependencyObject).Name));
            }

            return valueDO;
        }

        /// <summary>
        /// Call the Add method of underlying <see cref="List{T}"/> collection
        /// and update inheritance context to match this object.
        /// </summary>
        /// <param name="value"></param>
        internal void AddDependencyObjectInternal(T value)
        {
            DependencyObject valueDO = CastDO(value);
            this.ProvideSelfAsInheritanceContext(valueDO, null);

            this.AddInternal(value);
        }

        /// <summary>
        /// Call the Clear method of underlying <see cref="List{T}"/> collection
        /// and update inheritance context to match this object.
        /// </summary>
        internal void ClearDependencyObjectInternal()
        {
            foreach (DependencyObject valueDO in this.Select(v => CastDO(v)))
            {
                this.RemoveSelfAsInheritanceContext(valueDO, null);
            }

            this.ClearInternal();
        }

        /// <summary>
        /// Call the Insert method of underlying <see cref="List{T}"/> collection
        /// and update inheritance context to match this object.
        /// </summary>
        internal void InsertDependencyObjectInternal(int index, T value)
        {
            DependencyObject valueDO = CastDO(value);
            this.ProvideSelfAsInheritanceContext(valueDO, null);

            this.InsertInternal(index, value);
        }

        /// <summary>
        /// Call the RemoveAt method of underlying <see cref="List{T}"/> collection
        /// and update inheritance context to match this object.
        /// </summary>
        internal void RemoveAtDependencyObjectInternal(int index)
        {
            DependencyObject removedDO = CastDO(this._collection[index]);
            this.RemoveSelfAsInheritanceContext(removedDO, null);

            this.RemoveAtInternal(index);
        }

        /// <summary>
        /// Call the Remove method of underlying <see cref="List{T}"/> collection
        /// and update inheritance context to match this object.
        /// </summary>
        internal bool RemoveDependencyObjectInternal(T value)
        {
            int index = this._collection.IndexOf(value);
            if (index > -1)
            {
                this.RemoveAtDependencyObjectInternal(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call the Indexer (setter) property of underlying <see cref="List{T}"/> collection
        /// and update inheritance context to match this object.
        /// </summary>
        internal void SetItemDependencyObjectInternal(int index, T value)
        {
            DependencyObject originalDO = CastDO(this._collection[index]);
            DependencyObject newDO = CastDO(value);
            this.RemoveSelfAsInheritanceContext(originalDO, null);
            this.ProvideSelfAsInheritanceContext(newDO, null);

            this.SetItemInternal(index, value);
        }

        #endregion

        internal void UpdateCountProperty()
        {
            this.SetValue(CountPropertyKey, this.CountInternal);
        }

        internal event NotifyCollectionChangedEventHandler CollectionChanged;

        internal void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this._processCollectionChanged)
            {
                this.__OnCollectionChanged(e);
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            if (this._processCollectionChanged)
            {
                this.__OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            if (this._processCollectionChanged)
            {
                this.__OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
            }
        }

        private void OnCollectionReset()
        {
            if (this._processCollectionChanged)
            {
                this.__OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void __OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = this.CollectionChanged;
            if (handler != null)
            {
                using (this.BlockReentrancy())
                {
                    handler(this, e);
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
        internal void CheckReentrancy()
        {
            if (!this._processCollectionChanged)
            {
                return;
            }

            if (this._monitor.Busy)
            {
                // we can allow changes if there's only one listener - the problem
                // only arises if reentrant changes make the original event args
                // invalid for later listeners.  This keeps existing code working
                // (e.g. Selector.SelectedItems).
                if (this.CollectionChanged?.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Reentrancy not allowed");
                }
            }
        }

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

            public bool Busy { get { return this._busyCount > 0; } }

            int _busyCount;
        }
    }
}
