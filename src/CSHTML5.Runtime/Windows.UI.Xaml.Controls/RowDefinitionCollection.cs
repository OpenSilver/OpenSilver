
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



using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <exclude/>
    public sealed class RowDefinitionCollection : ObservableCollection<RowDefinition> //IList<RowDefinition>, IEnumerable<RowDefinition>
    {
        //Collection<RowDefinition> _collection;

        //int Count
        //{
        //    get { return _collection.Count; }
        //}

        //void Add(RowDefinition item)
        //{
        //    _collection.Add(item);
        //}

        //void Clear()
        //{
        //    _collection.Clear();
        //}
        //bool Contains(RowDefinition item)
        //{
        //    return _collection.Contains(item);
        //}
        //void CopyTo(RowDefinition[] array, int arrayIndex)
        //{
        //    _collection.CopyTo(array, arrayIndex);
        //}
        //int IndexOf(RowDefinition item)
        //{
        //    return _collection.IndexOf(item);
        //}
        //void Insert(int index, RowDefinition item)
        //{
        //    _collection.Insert(index, item);
        //}
        //bool Remove(RowDefinition item)
        //{
        //    return _collection.Remove(item);
        //}
        //void RemoveAt(int index)
        //{
        //    _collection.RemoveAt(index);
        //}


        //public IEnumerator<RowDefinition> GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerator<RowDefinition> IEnumerable<RowDefinition>.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //int IList<RowDefinition>.IndexOf(RowDefinition item)
        //{
        //    throw new NotImplementedException();
        //}

        //void IList<RowDefinition>.Insert(int index, RowDefinition item)
        //{
        //    throw new NotImplementedException();
        //}

        //void IList<RowDefinition>.RemoveAt(int index)
        //{
        //    throw new NotImplementedException();
        //}

        //public RowDefinition this[int index]
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //void ICollection<RowDefinition>.Add(RowDefinition item)
        //{
        //    throw new NotImplementedException();
        //}

        //void ICollection<RowDefinition>.Clear()
        //{
        //    throw new NotImplementedException();
        //}

        //bool ICollection<RowDefinition>.Contains(RowDefinition item)
        //{
        //    throw new NotImplementedException();
        //}

        //void ICollection<RowDefinition>.CopyTo(RowDefinition[] array, int arrayIndex)
        //{
        //    throw new NotImplementedException();
        //}

        //int ICollection<RowDefinition>.Count
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public bool IsReadOnly
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //bool ICollection<RowDefinition>.Remove(RowDefinition item)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
