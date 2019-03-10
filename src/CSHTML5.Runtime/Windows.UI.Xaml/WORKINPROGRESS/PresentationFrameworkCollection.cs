
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
#if WORKINPROGRESS
    // Summary:
    //     Provides a common collection class for Silverlight collections.
    //
    // Type parameters:
    //   T:
    //     Type constraint for type safety of the constrained collection implementation.
    public abstract class PresentationFrameworkCollection<T> : DependencyObject, INotifyPropertyChanged, INotifyCollectionChanged, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        public PresentationFrameworkCollection()
        {
            _collection = new List<T>();
        }

        // Summary:
        //     Identifies the System.Windows.PresentationFrameworkCollection<T>.Count dependency
        //     property.
        //
        // Returns:
        //     The identifier for the System.Windows.PresentationFrameworkCollection<T>.Count
        //     dependency property.
        public static readonly DependencyProperty CountProperty;

        private List<T> _collection;
        private List<T> Collection
        {
            get
            {
                if(_collection == null)
                {
                    _collection = new List<T>();
                }
                return _collection;
            }
            set 
            {
                _collection = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raises the event to call when the collection has changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged((object)this, e);
            }
        }

        /// <summary>
        /// Raises the event to call when the property has changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged((object)this, e);
            }
        }

        /// <summary>
        /// Helper to raise a PropertyChanged event  />).
        /// </summary>
        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        // Summary:
        //     Gets the number of elements contained in the System.Windows.PresentationFrameworkCollection<T>.
        //
        // Returns:
        //     The number of elements contained in the System.Windows.PresentationFrameworkCollection<T>.
        public int Count
        {
            get
            {
                return Collection.Count;
            }
        }

        // Summary:
        //     Gets a value indicating whether the System.Windows.PresentationFrameworkCollection<T>
        //     has a fixed size.
        //
        // Returns:
        //     true if the System.Windows.PresentationFrameworkCollection<T> has a fixed
        //     size; otherwise, false.
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        // Summary:
        //     Gets a value indicating whether the System.Windows.PresentationFrameworkCollection<T>
        //     is read-only.
        //
        // Returns:
        //     true if the System.Windows.PresentationFrameworkCollection<T> is read-only;
        //     otherwise, false.
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        // Summary:
        //     Gets a value indicating whether access to the System.Windows.PresentationFrameworkCollection<T>
        //     is synchronized (thread safe).
        //
        // Returns:
        //     true if access to the System.Windows.PresentationFrameworkCollection<T> is
        //     synchronized (thread safe); otherwise, false.
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        // Summary:
        //     Gets an object that can be used to synchronize access to the System.Windows.PresentationFrameworkCollection<T>
        //     .
        //
        // Returns:
        //     An object that can be used to synchronize access to the System.Windows.PresentationFrameworkCollection<T>.
        public object SyncRoot
        {
            get
            {
                return (object)this;
            }
        }

        // Summary:
        //     Gets or sets the element at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to get or set.
        //
        // Returns:
        //     The element at the specified index.
        public T this[int index]
        {
            get
            {
                return Collection[index];
            }
            set
            {
                Collection[index] = value;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)value, index));
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)value, index));
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
            }
        }

        // Summary:
        //     Adds an item to the System.Windows.PresentationFrameworkCollection<T>.
        //
        // Parameters:
        //   value:
        //     The object to add.
        public void Add(T value)
        {
            this.AddInternal(value);
        }

        internal virtual void AddInternal(T value)
        {
            Collection.Add(value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)value));
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
        }

        // Summary:
        //     Removes all items from the System.Windows.PresentationFrameworkCollection<T>.
        public void Clear()
        {
            Collection.Clear();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
        }

        /// <summary>
        /// Removes all items from System.Windows.PresentationFrameworkCollection<T>.
        /// </summary>
        protected virtual void ClearItems()
        {
            Clear();
        }

        // Summary:
        //     Determines whether the System.Windows.PresentationFrameworkCollection<T>
        //     contains a specific value.
        //
        // Parameters:
        //   value:
        //     The object to locate in the System.Windows.PresentationFrameworkCollection<T>.
        //
        // Returns:
        //     true if the object is found in the System.Windows.PresentationFrameworkCollection<T>;
        //     otherwise, false.
        public bool Contains(T value)
        {
            return Collection.Contains(value);
        }

        // Summary:
        //     Copies the elements of the System.Windows.PresentationFrameworkCollection<T>
        //     to an System.Array, starting at a particular System.Array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from the System.Windows.PresentationFrameworkCollection<T>. The System.Array
        //     must have zero-based indexing.
        //
        //   index:
        //     The zero-based index in array at which copying begins.
        public void CopyTo(Array array, int index)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                array.SetValue((object)Collection[i], index + i);
            }
        }

        // Summary:
        //     Copies the elements of the System.Windows.PresentationFrameworkCollection<T>
        //     to an System.Array, starting at a particular System.Array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from the System.Windows.PresentationFrameworkCollection<T>. The System.Array
        //     must have zero-based indexing.
        //
        //   index:
        //     The zero-based index in array at which copying begins.
        public void CopyTo(T[] array, int index)
        {
            Collection.CopyTo(array, index);
        }

        // Summary:
        //     Returns an enumerator that iterates through a collection.
        //
        // Returns:
        //     An System.Collections.IEnumerator object that can be used to iterate through
        //     the collection.
        public IEnumerator<T> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        // Summary:
        //     Determines the index of a specific item in the System.Windows.PresentationFrameworkCollection<T>.
        //
        // Parameters:
        //   value:
        //     The object to locate in the System.Windows.PresentationFrameworkCollection<T>.
        //
        // Returns:
        //     The index of value if found in the list; otherwise, an exception.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The object was not found in the list.
        public int IndexOf(T value)
        {
            return Collection.IndexOf(value);
        }

        // Summary:
        //     Inserts an item to the System.Windows.PresentationFrameworkCollection<T>
        //     at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index at which value should be inserted.
        //
        //   value:
        //     The object to insert into the System.Windows.PresentationFrameworkCollection<T>.
        public void Insert(int index, T value)
        {
            Collection.Insert(index, value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)value, index));
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
        }

        // Summary:
        //     Removes the first occurrence of a specific object from the System.Windows.PresentationFrameworkCollection<T>.
        //
        // Parameters:
        //   value:
        //     The object to remove from the System.Windows.PresentationFrameworkCollection<T>.
        //
        // Returns:
        //     true if an object was removed; otherwise, false.
        public bool Remove(T value)
        {
            bool hasChanged = Collection.Remove(value);
            if (hasChanged)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)value));
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
            }
            return hasChanged;
        }

        // Summary:
        //     Removes the item at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the item to remove.
        public void RemoveAt(int index)
        {
            Collection.RemoveAt(index);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, index));
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
        }

        int IList.Add(object value)
        {
            if(!(value is T))
            {
                throw new ArgumentException("Invalid argument", "value");
            }
            Collection.Add((T)value);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            return 1;
        }

        bool IList.Contains(object value)
        {
            if (value is T)
            {
                return Collection.Contains((T)value);
            }
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (value is T)
            {
                return Collection.IndexOf((T)value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (value is T)
            {
                Collection.Insert(index, (T)value);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
            }
        }

        void IList.Remove(object value)
        {
            if (value is T)
            {
                if (Collection.Remove((T)value))
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
                    OnPropertyChanged("Count");
                    OnPropertyChanged("Item[]");
                }
            }
        }

        object IList.this[int index]
        {
            get
            {
                return (object)Collection[index];
            }
            set
            {
                if (value is T)
                {
                    Collection[index] = (T)value;
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
                    OnPropertyChanged("Count");
                    OnPropertyChanged("Item[]");
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
#endif
}
