

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
    public sealed partial class TransitionCollection : List<Transition> //: IList<Transition>, IEnumerable<Transition>
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

