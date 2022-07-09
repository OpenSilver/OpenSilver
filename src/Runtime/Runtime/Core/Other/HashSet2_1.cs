

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


using System.ComponentModel;

namespace System.Collections.Generic
{
#if BRIDGE
    //[Obsolete]
    public class HashSet2<T> : HashSet<T>
    {
        public HashSet2() : base()
        {
        }

        public HashSet2(IEnumerable<T> source) : base(source)
        {
        }
    }
#else
    [Obsolete("Use System.Collections.Generic.HashSet instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class HashSet2<T> : ICollection<T>, IEnumerable<T>
#if UNIMPLEMENTED_MEMBERS
        , ISet2<T>
#endif
    {
        private Dictionary<T, object> _dictionary = new Dictionary<T,object>();

        public HashSet2()
        {
            
        }

        public HashSet2(IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                _dictionary.Add(item, null);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        public bool Add(T item)
        {
            if (!_dictionary.ContainsKey(item))
            {
                _dictionary.Add(item, null);
                return true;
            }
            else
            {
                return false;
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (T item in _dictionary.Keys)
            {
                array[i] = item;
                ++i;
            }
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return _dictionary.Remove(item);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            List<T> temp = new List<T>();
            foreach(T element in other)
            {
                if (_dictionary.ContainsKey(element)) //Note: we could have used this.Contains but it does the exact same thing and we avoid a call to a method this way.
                {
                    temp.Add(element);
                }
            }
            foreach(T element in temp)
            {
                _dictionary.Remove(element);
            }
        }

#if UNIMPLEMENTED_MEMBERS
        bool ISet2<T>.Add(T item)
        {
            throw new NotImplementedException();
        }
        
        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
#endif
    }

#endif
}
