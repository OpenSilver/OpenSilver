
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
    public sealed partial class ColumnDefinitionCollection : ObservableCollection<ColumnDefinition> //IList<ColumnDefinition>, IEnumerable<ColumnDefinition>
    {
        //Collection<ColumnDefinition> _collection;

        //int Count
        //{
        //    get { return _collection.Count; }
        //}

        //void Add(ColumnDefinition item)
        //{
        //    _collection.Add(item);
        //}

        //void Clear()
        //{
        //    _collection.Clear();
        //}
        //bool Contains(ColumnDefinition item)
        //{
        //    return _collection.Contains(item);
        //}
        //void CopyTo(ColumnDefinition[] array, int arrayIndex)
        //{
        //    _collection.CopyTo(array, arrayIndex);
        //}
        //int IndexOf(ColumnDefinition item)
        //{
        //    return _collection.IndexOf(item);
        //}
        //void Insert(int index, ColumnDefinition item)
        //{
        //    _collection.Insert(index, item);
        //}
        //bool Remove(ColumnDefinition item)
        //{
        //    return _collection.Remove(item);
        //}
        //void RemoveAt(int index)
        //{
        //    _collection.RemoveAt(index);
        //}


        //public IEnumerator<ColumnDefinition> GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //int IList<ColumnDefinition>.IndexOf(ColumnDefinition item)
        //{
        //    throw new NotImplementedException();
        //}

        //void IList<ColumnDefinition>.Insert(int index, ColumnDefinition item)
        //{
        //    throw new NotImplementedException();
        //}

        //void IList<ColumnDefinition>.RemoveAt(int index)
        //{
        //    throw new NotImplementedException();
        //}

        //public ColumnDefinition this[int index]
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

        //void ICollection<ColumnDefinition>.Add(ColumnDefinition item)
        //{
        //    throw new NotImplementedException();
        //}

        //void ICollection<ColumnDefinition>.Clear()
        //{
        //    throw new NotImplementedException();
        //}

        //bool ICollection<ColumnDefinition>.Contains(ColumnDefinition item)
        //{
        //    throw new NotImplementedException();
        //}

        //void ICollection<ColumnDefinition>.CopyTo(ColumnDefinition[] array, int arrayIndex)
        //{
        //    throw new NotImplementedException();
        //}

        //int ICollection<ColumnDefinition>.Count
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public bool IsReadOnly
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //bool ICollection<ColumnDefinition>.Remove(ColumnDefinition item)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
