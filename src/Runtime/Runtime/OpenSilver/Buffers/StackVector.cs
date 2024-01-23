
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
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenSilver.Buffers
{
    internal struct StackVector<T> where T : struct
    {
        private T[] _originalArray;

        public StackVector(int capacity, int initialLength = 0)
        {
            capacity = Math.Max(capacity, initialLength);
            if (capacity > 0)
            {
                AllocateInner(capacity);
            }
            Count = initialLength;
        }

        public int Count { get; private set; }

        public Span<T> Span => _originalArray.AsSpan(0, Count);

        public ref T this[int index] => ref _originalArray[index];

        public StackVectorEnumerator GetEnumerator() => new(this);

        public void Dispose()
        {
            if (_originalArray is not null)
            {
                Array.Clear(_originalArray, 0, Count);
                ArrayPool<T>.Shared.Return(_originalArray, false);
                _originalArray = null;
            }
        }

        public void PushBack(T value)
        {
            var newLength = Count + 1;
            Resize(newLength);
            _originalArray[newLength - 1] = value;
        }

        private void Resize(int newCount)
        {
            if (_originalArray is null || _originalArray.Length < newCount)
            {
                AllocateInner(newCount);
            }

            Count = newCount;
        }

        private void AllocateInner(int newSize)
        {
            Debug.Assert(newSize > 0);

            var newArray = ArrayPool<T>.Shared.Rent(newSize);

            if (_originalArray is not null)
            {
                Span.CopyTo(newArray);
                Array.Clear(_originalArray, 0, Count);
                ArrayPool<T>.Shared.Return(_originalArray, false);
            }

            _originalArray = newArray;
        }

        public struct StackVectorEnumerator : IEnumerator<T>
        {
            private readonly StackVector<T> _owner;
            private int _index = -1;

            public StackVectorEnumerator(StackVector<T> owner)
            {
                _owner = owner;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _owner.Count;
            }

            public void Reset() => _index = -1;

            public T Current => _owner._originalArray[_index];

            object IEnumerator.Current => _owner._originalArray[_index];

            public void Dispose() { }
        }
    }
}
