// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Collections.Generic;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Utility
{
    // This enum controls the growth to successively more complex storage models
    internal enum FrugalMapStoreState
    {
        Success,
        ThreeObjectMap,
        SixObjectMap,
        Array,
        SortedArray,
        Hashtable
    }

    abstract class FrugalMapBase
    {
        public abstract FrugalMapStoreState InsertEntry(int key, object value);

        public abstract void RemoveEntry(int key);

        /// <summary>
        /// Looks for an entry that contains the given key, null is returned if the
        /// key is not found.
        /// </summary>
        public abstract object Search(int key);

        /// <summary>
        /// A routine used by enumerators that need a sorted map
        /// </summary>
        public abstract void Sort();

        /// <summary>
        /// A routine used by enumerators to iterate through the map
        /// </summary>
        public abstract void GetKeyValuePair(int index, out int key, out object value);

        /// <summary>
        /// A routine used to iterate through all the entries in the map
        /// </summary>
        public abstract void Iterate(List<object> list, FrugalMapIterationCallback callback);

        /// <summary>
        /// Promotes the key/value pairs in the current collection to the next larger
        /// and more complex storage model.
        /// </summary>
        public abstract void Promote(FrugalMapBase newMap);

        /// <summary>
        /// Size of this data store
        /// </summary>
        public abstract int Count
        {
            get;
        }

        protected const int INVALIDKEY = 0x7FFFFFFF;

        internal struct Entry
        {
            public int Key;
            public object Value;
        }
    }

    /// <summary>
    /// A simple class to handle a single key/value pair
    /// </summary>
    internal sealed class SingleObjectMap : FrugalMapBase
    {
        public SingleObjectMap()
        {
            _loneEntry.Key = INVALIDKEY;
            _loneEntry.Value = DependencyProperty.UnsetValue;
        }

        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            // If we don't have any entries or the existing entry is being overwritten,
            // then we can use this map.  Otherwise we have to promote.
            if ((INVALIDKEY == _loneEntry.Key) || (key == _loneEntry.Key))
            {
                Debug.Assert(INVALIDKEY != key);

                _loneEntry.Key = key;
                _loneEntry.Value = value;
                return FrugalMapStoreState.Success;
            }
            else
            {
                // Entry already used, move to an ThreeObjectMap
                return FrugalMapStoreState.ThreeObjectMap;
            }
        }

        public override void RemoveEntry(int key)
        {
            // Wipe out the info in the only entry if it matches the key.
            if (key == _loneEntry.Key)
            {
                _loneEntry.Key = INVALIDKEY;
                _loneEntry.Value = DependencyProperty.UnsetValue;
            }
        }

        public override object Search(int key)
        {
            if (key == _loneEntry.Key)
            {
                return _loneEntry.Value;
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
            // Single items are already sorted.
        }

        public override void GetKeyValuePair(int index, out int key, out Object value)
        {
            if (0 == index)
            {
                value = _loneEntry.Value;
                key = _loneEntry.Key;
            }
            else
            {
                value = DependencyProperty.UnsetValue;
                key = INVALIDKEY;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Iterate(List<object> list, FrugalMapIterationCallback callback)
        {
            if (Count == 1)
            {
                callback(list, _loneEntry.Key, _loneEntry.Value);
            }
        }

        public override void Promote(FrugalMapBase newMap)
        {
            if (FrugalMapStoreState.Success == newMap.InsertEntry(_loneEntry.Key, _loneEntry.Value))
            {
            }
            else
            {
                // newMap is smaller than previous map
                throw new ArgumentException(
                    string.Format("Cannot promote from '{0}' to '{1}' because the target map is too small.", ToString(), newMap.ToString()),
                    nameof(newMap));
            }
        }

        // Size of this data store
        public override int Count
        {
            get
            {
                if (INVALIDKEY != _loneEntry.Key)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private Entry _loneEntry;
    }

    internal sealed class HashObjectMap : FrugalMapBase
    {
        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            Debug.Assert(INVALIDKEY != key);

            if (null != _entries)
            {
                // This is done because forward branches
                // default prediction is not to be taken
                // making this a CPU win because insert
                // is a common operation.
            }
            else
            {
                _entries = new Dictionary<int, object>(MINSIZE);
            }

            _entries[key] = ((value != NullValue) && (value != null)) ? value : NullValue;
            return FrugalMapStoreState.Success;
        }

        public override void RemoveEntry(int key)
        {
            _entries.Remove(key);
        }

        public override object Search(int key)
        {
            return _entries.TryGetValue(key, out object value) && (value != NullValue) ?
                value :
                DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
            // Always sorted.
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < _entries.Count)
            {
                Dictionary<int, object>.Enumerator myEnumerator = _entries.GetEnumerator();

                // Move to first valid value
                myEnumerator.MoveNext();

                for (int i = 0; i < index; ++i)
                {
                    myEnumerator.MoveNext();
                }

                KeyValuePair<int, object> current = myEnumerator.Current;
                key = current.Key;
                value = (current.Value != NullValue) ?
                    current.Value :
                    DependencyProperty.UnsetValue;
            }
            else
            {
                value = DependencyProperty.UnsetValue;
                key = INVALIDKEY;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Iterate(List<object> list, FrugalMapIterationCallback callback)
        {
            foreach (KeyValuePair<int, object> entry in _entries)
            {
                object value = (entry.Value != NullValue) ?
                    entry.Value :
                    DependencyProperty.UnsetValue;

                callback(list, entry.Key, value);
            }
        }

        public override void Promote(FrugalMapBase newMap)
        {
            // Should never get here
            throw new InvalidOperationException("Cannot promote from Hashtable.");
        }

        // Size of this data store
        public override int Count
        {
            get
            {
                return _entries.Count;
            }
        }

        // 163 is chosen because it is the first prime larger than 128, the MAXSIZE of SortedObjectMap
        internal const int MINSIZE = 163;

        // Hashtable will return null from its indexer if the key is not
        // found OR if the value is null.  To distinguish between these
        // two cases we insert NullValue instead of null.
        private static object NullValue = new object();

        internal Dictionary<int, object> _entries;
    }

    // A sorted array of key/value pairs. A binary search is used to minimize the cost of insert/search.

    internal sealed class LargeSortedObjectMap : FrugalMapBase
    {
        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            bool found;

            Debug.Assert(INVALIDKEY != key);

            // Check to see if we are updating an existing entry
            int index = FindInsertIndex(key, out found);
            if (found)
            {
                _entries[index].Value = value;
                return FrugalMapStoreState.Success;
            }
            else
            {
                // New key/value pair
                if (null != _entries)
                {
                    if (_entries.Length > _count)
                    {
                        // Have empty entries, just set the first available
                    }
                    else
                    {
                        int size = _entries.Length;
                        Entry[] destEntries = new Entry[size + (size >> 1)];

                        // Copy old array
                        Array.Copy(_entries, 0, destEntries, 0, _entries.Length);
                        _entries = destEntries;
                    }
                }
                else
                {
                    _entries = new Entry[MINSIZE];
                }

                // Inserting into the middle of the existing entries?
                if (index < _count)
                {
                    // Move higher valued keys to make room for the new key
                    Array.Copy(_entries, index, _entries, index + 1, (_count - index));
                }
                else
                {
                    _lastKey = key;
                }

                // Stuff in the new key/value pair
                _entries[index].Key = key;
                _entries[index].Value = value;
                ++_count;
                return FrugalMapStoreState.Success;
            }
        }

        public override void RemoveEntry(int key)
        {
            bool found;

            Debug.Assert(INVALIDKEY != key);

            int index = FindInsertIndex(key, out found);

            if (found)
            {
                // Shift entries down
                int numToCopy = (_count - index) - 1;
                if (numToCopy > 0)
                {
                    Array.Copy(_entries, index + 1, _entries, index, numToCopy);
                }
                else
                {
                    // If we're not copying anything, then it means we are 
                    //  going to remove the last entry.  Update _lastKey so
                    //  that it reflects the key of the new "last entry"
                    if (_count > 1)
                    {
                        // Next-to-last entry will be the new last entry
                        _lastKey = _entries[_count - 2].Key;
                    }
                    else
                    {
                        // Unless there isn't a next-to-last entry, in which
                        //  case the key is reset to INVALIDKEY.
                        _lastKey = INVALIDKEY;
                    }
                }

                // Wipe out the last entry
                _entries[_count - 1].Key = INVALIDKEY;
                _entries[_count - 1].Value = DependencyProperty.UnsetValue;

                --_count;
            }
        }

        public override object Search(int key)
        {
            bool found;

            int index = FindInsertIndex(key, out found);
            if (found)
            {
                return _entries[index].Value;
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
            // Always sorted.
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < _count)
            {
                value = _entries[index].Value;
                key = _entries[index].Key;
            }
            else
            {
                value = DependencyProperty.UnsetValue;
                key = INVALIDKEY;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Iterate(List<object> list, FrugalMapIterationCallback callback)
        {
            if (_count > 0)
            {
                for (int i = 0; i < _count; i++)
                {
                    callback(list, _entries[i].Key, _entries[i].Value);
                }
            }
        }

        public override void Promote(FrugalMapBase newMap)
        {
            for (int index = 0; index < _entries.Length; ++index)
            {
                if (FrugalMapStoreState.Success == newMap.InsertEntry(_entries[index].Key, _entries[index].Value))
                {
                    continue;
                }
                // newMap is smaller than previous map
                throw new ArgumentException(
                    string.Format("Cannot promote from '{0}' to '{1}' because the target map is too small.", ToString(), newMap.ToString()),
                    nameof(newMap));
            }
        }

        private int FindInsertIndex(int key, out bool found)
        {
            int iLo = 0;

            // Only do the binary search if there is a chance of finding the key
            // This also speeds insertion because we tend to insert at the end.
            if ((_count > 0) && (key <= _lastKey))
            {
                // The array index used for insertion is somewhere between 0 
                //  and _count-1 inclusive
                int iHi = _count - 1;

                // Do a binary search to find the insertion point
                do
                {
                    int iPv = (iHi + iLo) / 2;
                    if (key <= _entries[iPv].Key)
                    {
                        iHi = iPv;
                    }
                    else
                    {
                        iLo = iPv + 1;
                    }
                }
                while (iLo < iHi);
                found = (key == _entries[iLo].Key);
            }
            else
            {
                // Insert point is at the end
                iLo = _count;
                found = false;
            }
            return iLo;
        }

        public override int Count
        {
            get
            {
                return _count;
            }
        }

        // MINSIZE chosen to be small, growth rate of 1.5 is slow at small sizes, but increasingly agressive as
        // the array grows
        private const int MINSIZE = 2;

        // The number of items in the map.
        internal int _count;

        private int _lastKey = INVALIDKEY;
        private Entry[] _entries;
    }

    // This is a variant of FrugalMap that always uses an array as the underlying store.
    // This avoids the virtual method calls that are present when the store morphs through
    // the size efficient store classes normally used. It is appropriate only when we know the
    // store will always be populated and individual elements will be accessed in a tight loop.
    internal struct InsertionSortMap
    {
        public object this[int key]
        {
            get
            {
                // If no entry, DependencyProperty.UnsetValue is returned
                if (null != _mapStore)
                {
                    return _mapStore.Search(key);
                }
                return DependencyProperty.UnsetValue;
            }

            set
            {
                if (value != DependencyProperty.UnsetValue)
                {
                    // If not unset value, ensure write success
                    if (null != _mapStore)
                    {
                        // This is done because forward branches
                        // default prediction is not to be taken
                        // making this a CPU win because set is
                        // a common operation.
                    }
                    else
                    {
                        _mapStore = new LargeSortedObjectMap();
                    }

                    FrugalMapStoreState myState = _mapStore.InsertEntry(key, value);
                    if (FrugalMapStoreState.Success == myState)
                    {
                        return;
                    }
                    else
                    {
                        // Need to move to a more complex storage
                        LargeSortedObjectMap newStore;

                        if (FrugalMapStoreState.SortedArray == myState)
                        {
                            newStore = new LargeSortedObjectMap();
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot promote from Hashtable.");
                        }

                        // Extract the values from the old store and insert them into the new store
                        _mapStore.Promote(newStore);

                        // Insert the new value
                        _mapStore = newStore;
                        _mapStore.InsertEntry(key, value);
                    }
                }
                else
                {
                    // DependencyProperty.UnsetValue means remove the value
                    if (null != _mapStore)
                    {
                        _mapStore.RemoveEntry(key);
                        if (_mapStore.Count == 0)
                        {
                            // Map Store is now empty ... throw it away
                            _mapStore = null;
                        }
                    }
                }
            }
        }

        public void Sort()
        {
            if (null != _mapStore)
            {
                _mapStore.Sort();
            }
        }

        public void GetKeyValuePair(int index, out int key, out object value)
        {
            if (null != _mapStore)
            {
                _mapStore.GetKeyValuePair(index, out key, out value);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public void Iterate(List<object> list, FrugalMapIterationCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (_mapStore != null)
            {
                _mapStore.Iterate(list, callback);
            }
        }

        public int Count
        {
            get
            {
                if (null != _mapStore)
                {
                    return _mapStore.Count;
                }
                return 0;
            }
        }

        internal LargeSortedObjectMap _mapStore;
    }

    /// <summary>
    ///     FrugalMapIterationCallback
    /// </summary>
    internal delegate void FrugalMapIterationCallback(List<object> list, int key, object value);
}
