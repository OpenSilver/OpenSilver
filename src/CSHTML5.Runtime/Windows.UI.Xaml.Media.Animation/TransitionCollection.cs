
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Represents a collection of Transition objects.
    /// </summary>
    public sealed class TransitionCollection : List<Transition> //: IList<Transition>, IEnumerable<Transition>
    {
        ///// <summary>
        ///// Initializes a new instance of the TransitionCollection class.
        ///// </summary>
        //public TransitionCollection();

        //int Count { get; }
        //bool IsReadOnly { get; }

        //Transition this[int index] { get; set; }

        //void Add(Transition item);
        ////
        //// Summary:
        ////     Removes all items from the collection.
        //void Clear();
        //bool Contains(Transition item);
        //void CopyTo(Transition[] array, int arrayIndex);
        //int IndexOf(Transition item);
        //void Insert(int index, Transition item);
        //bool Remove(Transition item);
        //void RemoveAt(int index);
    }
}

