

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
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
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
        #region Data
        private readonly List<T> _collection;

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count",
                                                                                              typeof(int),
                                                                                              typeof(PresentationFrameworkCollection<T>),
                                                                                              new PropertyMetadata(0));
        #endregion

        #region Constructor

        internal PresentationFrameworkCollection()
        {
            this._collection = new List<T>();
            this.UpdateCountProperty();
        }

        internal PresentationFrameworkCollection(int capacity)
        {
            this._collection = new List<T>(capacity);
            this.UpdateCountProperty();
        }

        internal PresentationFrameworkCollection(IEnumerable<T> source)
        {
            this._collection = new List<T>(source);
            this.UpdateCountProperty();
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get { return (int)this.GetValue(CountProperty); }
        }

        public T this[int index]
        {
            get { return this.GetItemInternal(index); }
            set { this.SetItemOverride(index, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the System.Windows.PresentationFrameworkCollection`1
        /// has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the System.Windows.PresentationFrameworkCollection`1 has a fixed size;
        /// otherwise, false.
        /// </returns>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the System.Windows.PresentationFrameworkCollection`1
        /// is read-only.
        /// </summary>
        /// <returns>
        /// true if the System.Windows.PresentationFrameworkCollection`1 is read-only; otherwise,
        /// false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the System.Windows.PresentationFrameworkCollection`1
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the System.Windows.PresentationFrameworkCollection`1 is synchronized
        /// (thread safe); otherwise, false.
        /// </returns>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the System.Windows.PresentationFrameworkCollection`1.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the System.Windows.PresentationFrameworkCollection`1.
        /// </returns>
        public object SyncRoot
        {
            get { return this; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through
        /// the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._collection.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the System.Windows.PresentationFrameworkCollection`1.
        /// </summary>
        /// <param name="value">The object to add.</param>
        public void Add(T value)
        {
            this.AddOverride(value);
        }

        /// <summary>
        /// Removes all items from the System.Windows.PresentationFrameworkCollection`1.
        /// </summary>
        public void Clear()
        {
            this.ClearOverride();
        }


        /// <summary>
        /// Determines whether the System.Windows.PresentationFrameworkCollection`1 contains
        /// a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the System.Windows.PresentationFrameworkCollection`1.</param>
        /// <returns>
        /// true if the object is found in the System.Windows.PresentationFrameworkCollection`1;
        /// otherwise, false.
        /// </returns>
        public bool Contains(T value)
        {
            return this._collection.Contains(value);
        }

        /// <summary>
        /// Copies the elements of the System.Windows.PresentationFrameworkCollection`1 to
        /// an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements copied
        /// from the System.Windows.PresentationFrameworkCollection`1. The System.Array must
        /// have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)_collection).CopyTo(array, index);
        }

        /// <summary>
        /// Copies the elements of the System.Windows.PresentationFrameworkCollection`1 to
        /// an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements copied
        /// from the System.Windows.PresentationFrameworkCollection`1. The System.Array must
        /// have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int index)
        {
            this._collection.CopyTo(array, index);
        }

        /// <summary>
        /// Determines the index of a specific item in the System.Windows.PresentationFrameworkCollection`1.
        /// </summary>
        /// <param name="value">The object to locate in the System.Windows.PresentationFrameworkCollection`1.</param>
        /// <returns>The index of value if found in the list; otherwise, an exception.</returns>
        /// <exception cref="System.ArgumentException">
        /// The object was not found in the list.
        /// </exception>
        public int IndexOf(T value)
        {
            return this._collection.IndexOf(value);
        }

        /// <summary>
        /// Inserts an item to the System.Windows.PresentationFrameworkCollection`1 at the
        /// specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The object to insert into the System.Windows.PresentationFrameworkCollection`1.</param>
        public void Insert(int index, T value)
        {
            this.InsertOverride(index, value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Windows.PresentationFrameworkCollection`1.
        /// </summary>
        /// <param name="value">The object to remove from the System.Windows.PresentationFrameworkCollection`1.</param>
        /// <returns>true if an object was removed; otherwise, false.</returns>
        public bool Remove(T value)
        {
            return this.RemoveOverride(value);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            this.RemoveAtOverride(index);
        }

        #endregion

        #region Explicit Interface implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        int IList.Add(object value)
        {
            this.Add((T)value);
            return this.Count;
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

        #region Internal API

        #region Abstract Methods

        internal abstract void AddOverride(T value);

        internal abstract void ClearOverride();

        internal abstract void InsertOverride(int index, T value);

        internal abstract void RemoveAtOverride(int index);

        internal abstract bool RemoveOverride(T value);

        internal abstract void SetItemOverride(int index, T value);

        #endregion

        internal int CountInternal
        {
            get { return this._collection.Count; }
        }

        internal void AddInternal(T value)
        {
            this._collection.Add(value);
            this.UpdateCountProperty();
        }

        internal void ClearInternal()
        {
            this._collection.Clear();
            this.UpdateCountProperty();
        }

        internal void InsertInternal(int index, T value)
        {
            this._collection.Insert(index, value);
            this.UpdateCountProperty();
        }

        internal void RemoveAtInternal(int index)
        {
            this._collection.RemoveAt(index);
            this.UpdateCountProperty();
        }

        internal bool RemoveInternal(T value)
        {
            if (this._collection.Remove(value))
            {
                this.UpdateCountProperty();
                return true;
            }
            return false;
        }

        internal T GetItemInternal(int index)
        {
            return this._collection[index];
        }

        internal void SetItemInternal(int index, T value)
        {
            this._collection[index] = value;
        }

        private void UpdateCountProperty()
        {
            this.SetValue(CountProperty, this.CountInternal);
        }

        #endregion
    }
}
