using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
    //
    // Summary:
    //     Provides a thread-safe, read-only collection that contains objects of a type
    //     specified by the generic parameter as elements.
    //
    // Type parameters:
    //   T:
    //     The type of object contained as items in the thread-safe, read-only collection.
    [ComVisible(false)]
	[OpenSilver.NotImplemented]
    public partial class SynchronizedReadOnlyCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.SynchronizedReadOnlyCollection`1
        //     class from a specified enumerable list of elements and with the object used to
        //     synchronize access to the thread-safe, read-only collection.
        //
        // Parameters:
        //   syncRoot:
        //     The object used to synchronize access to the thread-safe, read-only collection.
        //
        //   list:
        //     The System.Collections.Generic.IEnumerable`1 collection of elements used to initialize
        //     the thread-safe, read-only collection.
		[OpenSilver.NotImplemented]
        public SynchronizedReadOnlyCollection(object syncRoot, IEnumerable<T> list) { }

        //
        // Summary:
        //     Gets an element from the thread-safe, read-only collection with a specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to be retrieved from the collection.
        //
        // Returns:
        //     The element from read-only collection with the specified index.
		[OpenSilver.NotImplemented]
        public T this[int index]
        {
            get { return default(T); }
        }

        T IList<T>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        //
        // Summary:
        //     Gets the number of elements contained in the thread-safe, read-only collection.
        //
        // Returns:
        //     The number of elements contained in the thread-safe, read-only collection.
		[OpenSilver.NotImplemented]
        public int Count { get; }
        //
        // Summary:
        //     Gets the list of elements contained in the thread-safe, read-only collection.
        //
        // Returns:
        //     The System.Collections.Generic.IList`1 of elements that are contained in the
        //     thread-safe, read-only collection.
		[OpenSilver.NotImplemented]
        protected IList<T> Items { get; }

        int ICollection<T>.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int ICollection.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //
        // Summary:
        //     Determines whether the collection contains an element with a specific value.
        //
        // Parameters:
        //   value:
        //     The object to locate in the collection.
        //
        // Returns:
        //     true if the element value is found in the collection; otherwise, false.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     value is not an object of the type contained in the collection.
		[OpenSilver.NotImplemented]
        public bool Contains(T value)
        {
            return false;
        }
        //
        // Summary:
        //     Copies the elements of the collection to a specified array, starting at a particular
        //     index.
        //
        // Parameters:
        //   array:
        //     The System.Array that is the destination for the elements copied from the collection.
        //
        //   index:
        //     The zero-based index in the array at which copying begins.
		[OpenSilver.NotImplemented]
        public void CopyTo(T[] array, int index)
        {

        }
        //
        // Summary:
        //     Returns an enumerator that iterates through the synchronized, read-only collection.
        //
        // Returns:
        //     An System.Collections.Generic.IEnumerator`1 for objects of the type stored in
        //     the collection.
		[OpenSilver.NotImplemented]
        public IEnumerator<T> GetEnumerator()
        {
            return null;
        }
        //
        // Summary:
        //     Returns the index of the first occurrence of a value in the collection.
        //
        // Parameters:
        //   value:
        //     The element whose index is being retrieved.
        //
        // Returns:
        //     The zero-based index of the first occurrence of the value in the collection.
		[OpenSilver.NotImplemented]
        public int IndexOf(T value)
        {
            return -1;
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        int IList<T>.IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}
