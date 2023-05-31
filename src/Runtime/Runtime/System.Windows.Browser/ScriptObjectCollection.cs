
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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Browser.Internal;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides a thread-safe wrapper for obtaining and iterating Document Object Model
    /// (DOM) collections.
    /// </summary>
    public sealed class ScriptObjectCollection : ScriptObject, IEnumerable<ScriptObject>, IEnumerable
    {
        internal ScriptObjectCollection(IJSObjectRef jsObject)
            : base(jsObject)
        {
        }

        /// <summary>
        /// Gets an item from the collection at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to be retrieved.
        /// </param>
        /// <returns>
        /// A reference to an HTML element.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is invalid.
        /// </exception>
        public ScriptObject this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return GetProperty(index) as ScriptObject;
            }
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        /// <returns>
        /// A count of items in the collection.
        /// </returns>
        public int Count
        {
            get { return (int)GetProperty("length"); }
        }

        IEnumerator<ScriptObject> IEnumerable<ScriptObject>.GetEnumerator()
        {
            return new ScriptObjectCollectionEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ScriptObjectCollectionEnumerator(this);
        }

        private sealed class ScriptObjectCollectionEnumerator : IEnumerator<ScriptObject>
        {
            private readonly ScriptObjectCollection _collection;
            private readonly int _count;
            private int _index;

            public ScriptObjectCollectionEnumerator(ScriptObjectCollection collection)
            {
                Debug.Assert(collection != null);
                _collection = collection;
                _index = -1;
                _count = collection.Count;
            }

            public ScriptObject Current => _collection[_index];

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                _index++;
                return _index < _count;
            }

            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
