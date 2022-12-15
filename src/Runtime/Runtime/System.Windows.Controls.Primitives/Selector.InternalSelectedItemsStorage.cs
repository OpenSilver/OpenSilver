
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

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    public partial class Selector
    {
        internal sealed class InternalSelectedItemsStorage : IEnumerable<ItemInfo>
        {
            internal InternalSelectedItemsStorage(int capacity, IEqualityComparer<ItemInfo> equalityComparer)
            {
                _equalityComparer = equalityComparer;
                _list = new List<ItemInfo>(capacity);
                _set = new Dictionary<ItemInfo, ItemInfo>(capacity, equalityComparer);
            }

            internal InternalSelectedItemsStorage(InternalSelectedItemsStorage collection, IEqualityComparer<ItemInfo> equalityComparer = null)
            {
                _equalityComparer = equalityComparer ?? collection._equalityComparer;

                _list = new List<ItemInfo>(collection._list);

                if (collection.UsesItemHashCodes)
                {
                    _set = new Dictionary<ItemInfo, ItemInfo>(collection._set, _equalityComparer);
                }

                _resolvedCount = collection._resolvedCount;
                _unresolvedCount = collection._unresolvedCount;
            }

            public void Add(object item, DependencyObject container, int index)
            {
                Add(new ItemInfo(item, container, index));
            }

            public void Add(ItemInfo info)
            {
                if (_set != null)
                {
                    _set.Add(info, info);
                }
                _list.Add(info);

                if (info.IsResolved)
                    ++_resolvedCount;
                else
                    ++_unresolvedCount;
            }

            public bool Remove(ItemInfo e)
            {
                bool removed = false;
                bool isResolved = false;
                if (_set != null)
                {
                    ItemInfo realInfo;
                    if (_set.TryGetValue(e, out realInfo))
                    {
                        removed = true;
                        isResolved = realInfo.IsResolved;
                        _set.Remove(e);     // remove from hash table

                        if (RemoveIsDeferred)
                        {
                            // mark as removed - the real removal comes later
                            realInfo.Container = ItemInfo.RemovedContainer;
                            ++_batchRemove.RemovedCount;
                        }
                        else
                        {
                            RemoveFromList(e);
                        }
                    }
                }
                else
                {
                    removed = RemoveFromList(e);
                }

                if (removed)
                {
                    if (isResolved)
                        --_resolvedCount;
                    else
                        --_unresolvedCount;
                }

                return removed;
            }

            private bool RemoveFromList(ItemInfo e)
            {
                bool removed = false;
                int index = LastIndexInList(e); // removals tend to happen from the end of the list
                if (index >= 0)
                {
                    _list.RemoveAt(index);
                    removed = true;
                }
                return removed;
            }

            public bool Contains(ItemInfo e)
            {
                if (_set != null)
                {
                    return _set.ContainsKey(e);
                }
                else
                {
                    return (IndexInList(e) >= 0);
                }
            }

            public ItemInfo this[int index]
            {
                get
                {
                    return _list[index];
                }
            }

            public void Clear()
            {
                _list.Clear();
                if (_set != null)
                {
                    _set.Clear();
                }

                _resolvedCount = _unresolvedCount = 0;
            }

            public int Count
            {
                get
                {
                    return _list.Count;
                }
            }

            public bool RemoveIsDeferred { get { return _batchRemove != null && _batchRemove.IsActive; } }

            // using (storage.DeferRemove()) {...} defers the actual removal
            // of entries from _list until leaving the scope.   At that point,
            // the removal can be done more efficiently.
            public IDisposable DeferRemove()
            {
                if (_batchRemove == null)
                {
                    _batchRemove = new BatchRemoveHelper(this);
                }

                _batchRemove.Enter();
                return _batchRemove;
            }

            // do the actual removal of entries marked as Removed
            private void DoBatchRemove()
            {
                int j = 0, n = _list.Count;

                // copy the surviving entries to the front of the list
                for (int i = 0; i < n; ++i)
                {
                    ItemInfo info = _list[i];
                    if (!info.IsRemoved)
                    {
                        if (j < i)
                        {
                            _list[j] = _list[i];
                        }
                        ++j;
                    }
                }

                // remove the remaining unneeded entries
                _list.RemoveRange(j, n - j);
            }

            public int ResolvedCount { get { return _resolvedCount; } }
            public int UnresolvedCount { get { return _unresolvedCount; } }

            IEnumerator<ItemInfo> IEnumerable<ItemInfo>.GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            // If the underlying items don't implement GetHashCode according to
            // guidelines (i.e. if an item's hashcode can change during the item's
            // lifetime) we can't use any hash-based data structures like Dictionary,
            // Hashtable, etc.  The principal offender is DataRowView.  (bug 1583080)
            public bool UsesItemHashCodes
            {
                get { return _set != null; }
                set
                {
                    if (value == true && _set == null)
                    {
                        _set = new Dictionary<ItemInfo, ItemInfo>(_list.Count);
                        for (int i = 0; i < _list.Count; ++i)
                        {
                            _set.Add(_list[i], _list[i]);
                        }
                    }
                    else if (value == false)
                    {
                        _set = null;
                    }
                }
            }

            public ItemInfo FindMatch(ItemInfo info)
            {
                ItemInfo result;

                if (_set != null)
                {
                    if (!_set.TryGetValue(info, out result))
                    {
                        result = null;
                    }
                }
                else
                {
                    int index = IndexInList(info);
                    result = (index < 0) ? null : _list[index];
                }

                return result;
            }

            // like IndexOf, but uses the equality comparer
            private int IndexInList(ItemInfo info)
            {
                return _list.FindIndex((ItemInfo x) => { return _equalityComparer.Equals(info, x); });
            }

            // like LastIndexOf, but uses the equality comparer
            private int LastIndexInList(ItemInfo info)
            {
                return _list.FindLastIndex((ItemInfo x) => { return _equalityComparer.Equals(info, x); });
            }

            private List<ItemInfo> _list;
            private Dictionary<ItemInfo, ItemInfo> _set;
            private IEqualityComparer<ItemInfo> _equalityComparer;
            private int _resolvedCount, _unresolvedCount;
            private BatchRemoveHelper _batchRemove;

            private class BatchRemoveHelper : IDisposable
            {
                public BatchRemoveHelper(InternalSelectedItemsStorage owner)
                {
                    _owner = owner;
                }

                public bool IsActive { get { return _level > 0; } }
                public int RemovedCount { get; set; }

                public void Enter()
                {
                    ++_level;
                }

                public void Leave()
                {
                    if (_level > 0)
                    {
                        if (--_level == 0 && RemovedCount > 0)
                        {
                            _owner.DoBatchRemove();
                            RemovedCount = 0;
                        }
                    }
                }

                public void Dispose()
                {
                    Leave();
                }

                InternalSelectedItemsStorage _owner;
                int _level;
            }
        }
    }
}
