
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <exclude/>
    public sealed class SetterBaseCollection : List<Setter> //ObservableCollection<Setter> // IList<SetterBase>, IEnumerable<SetterBase>
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
