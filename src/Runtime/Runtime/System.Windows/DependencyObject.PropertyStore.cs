
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

using System.Runtime.CompilerServices;

namespace System.Windows;

public partial class DependencyObject
{
    private sealed class PropertyStore<TValue>
    {
        private const int DefaultCapacity = 4;
        private TValue[] _entries;
        private int _count;

        public PropertyStore()
        {
            _entries = null;
            _count = 0;
        }

        public PropertyStore(int capactity)
        {
            _entries = new TValue[capactity];
            _count = 0;
        }

        public int Count => _count;

        public ref TValue this[int index]
        {
            get
            {
                if (index >= _count)
                {
                    throw new IndexOutOfRangeException();
                }
                return ref _entries[index];
            }
        }

        public ReadOnlySpan<TValue> Span => _entries.AsSpan(0, _count);

        public bool Remove(int targetIndex)
        {
            int index = LookupEntry(targetIndex);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (_entries is null)
            {
                throw new IndexOutOfRangeException();
            }

            Array.Copy(_entries, index + 1, _entries, index, _count - index - 1);
            _count--;
            _entries[_count] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int LookupEntry(int targetIndex)
        {
            int lo = 0;
            int hi = _count - 1;

            while (lo <= hi)
            {
                int i = (hi + lo) >> 1;
                int entryId = _entries[i].GetHashCode();

                if (entryId == targetIndex)
                {
                    return i;
                }

                if (entryId < targetIndex)
                {
                    lo = i + 1;
                }
                else
                {
                    hi = i - 1;
                }
            }

            return ~lo;
        }

        internal void InsertEntry(TValue entry, int index)
        {
            if (_count > 0)
            {
                if (_count == _entries.Length)
                {
                    int newSize = _count <= DefaultCapacity ? _count * 2 : (int)(_count * 1.5);

                    var destEntries = new TValue[newSize];

                    Array.Copy(_entries, 0, destEntries, 0, index);

                    destEntries[index] = entry;

                    Array.Copy(_entries, index, destEntries, index + 1, _count - index);

                    _entries = destEntries;
                }
                else
                {
                    Array.Copy(
                        _entries,
                        index,
                        _entries,
                        index + 1,
                        _count - index);

                    _entries[index] = entry;
                }
            }
            else
            {
                _entries ??= new TValue[DefaultCapacity];
                _entries[0] = entry;
            }

            _count++;
        }
    }
}
