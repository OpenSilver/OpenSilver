

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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <exclude/>
    public sealed partial class SetterBaseCollection : List<Setter> //ObservableCollection<Setter> // IList<SetterBase>, IEnumerable<SetterBase>
    {
        //todo: once this will be changed into an ObservableCollection, so that it updates its _dictionaryOfSetters with the changes that are made on Style.Setters



        //List<SetterBase> _list = new List<SetterBase>();
        //public int IndexOf(SetterBase item)
        //{
        //    return _list.IndexOf(item);
        //}

        //public void Insert(int index, SetterBase item)
        //{
        //    _list.Insert(index,item);
        //}

        //public void RemoveAt(int index)
        //{
        //    _list.RemoveAt(index);
        //}

        //public SetterBase this[int index]
        //{
        //    get
        //    {
        //        return _list[index];
        //    }
        //    set
        //    {
        //        _list[index] = value;
        //    }
        //}

        //public void Add(SetterBase item)
        //{
        //    _list.Add(item);
        //}

        //public void Clear()
        //{
        //    _list.Clear();
        //}

        //public bool Contains(SetterBase item)
        //{
        //    return _list.Contains(item);
        //}

        //public void CopyTo(SetterBase[] array, int arrayIndex)
        //{
        //    _list.CopyTo(array, arrayIndex);
        //}

        //public int Count
        //{
        //    get { return _list.Count; }
        //}

        //public bool Remove(SetterBase item)
        //{
        //    return _list.Remove(item);
        //}

        //public IEnumerator<SetterBase> GetEnumerator()
        //{
        //    return _list.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return _list.GetEnumerator();
        //}


        //public bool IsReadOnly
        //{
        //    get { throw new NotImplementedException(); }
        //}
    }
}
