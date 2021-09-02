
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

namespace System.ComponentModel
{
    internal sealed class ReflectedPropertyCollection : IReadOnlyList<ReflectedPropertyData>
    {
        private IDictionary<string, ReflectedPropertyData> _cachedFoundProperties;
        private readonly ReflectedPropertyData[] _properties;

        private readonly object _internalSyncObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectedPropertyCollection"/>
        /// class.
        /// </summary>
        public ReflectedPropertyCollection(ReflectedPropertyData[] properties)
        {
            if (properties == null)
            {
#if NETSTANDARD
                _properties = Array.Empty<ReflectedPropertyData>();
#else
                _properties = EmptyArray<ReflectedPropertyData>.Value;
#endif
            }
            else
            {
                _properties = properties;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectedPropertyCollection"/>
        /// class with an ordered list of properties.
        /// </summary>
        internal ReflectedPropertyCollection(IDictionary<string, ReflectedPropertyData> orderedProperties)
        {
            _cachedFoundProperties = orderedProperties;
            
            if (orderedProperties == null)
            {
#if NETSTANDARD
                _properties = Array.Empty<ReflectedPropertyData>();
#else
                _properties = EmptyArray<ReflectedPropertyData>.Value;
#endif
            }
            else
            {
                _properties = new ReflectedPropertyData[orderedProperties.Count];
                orderedProperties.Values.CopyTo(_properties, 0);
            }
        }

        /// <summary>
        /// Gets the property with the specified index number.
        /// </summary>
        public ReflectedPropertyData this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                return _properties[index];
            }
        }

        /// <summary>
        /// Gets the property with the specified name.
        /// </summary>
        public ReflectedPropertyData this[string name] => Find(name);

        /// <summary>
        /// Gets the number of properties in the collection.
        /// </summary>
        public int Count => _properties.Length;

        /// <summary>
        /// Gets an enumerator for this <see cref="ReflectedPropertyCollection"/>.
        /// </summary>
        public IEnumerator<ReflectedPropertyData> GetEnumerator() => new ReflectedPropertyCollectionEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private ReflectedPropertyData Find(string name)
        {
            lock (_internalSyncObject)
            {

                if (_cachedFoundProperties == null)
                {
                    _cachedFoundProperties = new Dictionary<string, ReflectedPropertyData>();
                }

                // first try to find it in the cache
                //
                if (_cachedFoundProperties.TryGetValue(name, out ReflectedPropertyData cached))
                {
                    return cached;
                }

                // Now start walking from where we last left off, filling
                // the cache as we go.
                //
                ReflectedPropertyData p = null;

                for (int i = 0; i < Count; i++)
                {
                    if (_properties[i].Name.Equals(name))
                    {
                        _cachedFoundProperties[name] = _properties[i];
                        p = _properties[i];
                        break;
                    }
                }

                return p;
            }
        }

        private struct ReflectedPropertyCollectionEnumerator : IEnumerator<ReflectedPropertyData>
        {
            private readonly ReflectedPropertyCollection _collection;
            private int _index;
            private ReflectedPropertyData _current;

            internal ReflectedPropertyCollectionEnumerator(ReflectedPropertyCollection collection)
            {
                _collection = collection;
                _index = 0;
                _current = null;
            }

            public ReflectedPropertyData Current => _current;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_index < _collection.Count)
                {
                    _current = _collection._properties[_index];
                    _index++;
                    return true;
                }

                _current = null;
                _index = _collection.Count + 1;
                return false;
            }

            public void Reset()
            {
                _index = 0;
                _current = null;
            }

            public void Dispose()
            {
            }
        }
    }
}
