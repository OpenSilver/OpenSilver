

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
    /// Represents a collection of ObjectKeyFrame objects that can be individually
    /// accessed by index.
    /// </summary>
#if WORKINPROGRESS
    public sealed partial class ObjectKeyFrameCollection : PresentationFrameworkCollection<ObjectKeyFrame>
#else
    public sealed partial class ObjectKeyFrameCollection : List<ObjectKeyFrame> //: IList<ObjectKeyFrame>, IEnumerable<ObjectKeyFrame>
#endif
    {
        //// Summary:
        ////     Initializes a new instance of the ObjectKeyFrameCollection class.
        //public ObjectKeyFrameCollection();

        //int Count { get; }
        //bool IsReadOnly { get; }

        //ObjectKeyFrame this[int index] { get; set; }

        //void Add(ObjectKeyFrame item);
        ////
        //// Summary:
        ////     Removes all items from the collection.
        //void Clear();
        //bool Contains(ObjectKeyFrame item);
        //void CopyTo(ObjectKeyFrame[] array, int arrayIndex);
        //int IndexOf(ObjectKeyFrame item);
        //void Insert(int index, ObjectKeyFrame item);
        //bool Remove(ObjectKeyFrame item);
        //void RemoveAt(int index);
    }
}