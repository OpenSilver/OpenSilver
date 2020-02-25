
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
