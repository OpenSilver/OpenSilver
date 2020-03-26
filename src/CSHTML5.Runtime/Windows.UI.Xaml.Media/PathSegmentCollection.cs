

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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a collection of PathSegment objects that can be individually accessed
    /// by index.
    /// </summary>
    public sealed partial class PathSegmentCollection : List<PathSegment>// IList<PathSegment>, IEnumerable<PathSegment>
    {
        /// <summary>
        /// Initializes a new instance of the PathSegmentCollection class.
        /// </summary>
        public PathSegmentCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity"> int - The number of elements that the new list is initially capable of storing. </param>
        public PathSegmentCollection(int capacity) : base(capacity)
        {

        }

        internal void SetParentPath(Path path)
        {
            foreach (PathSegment segment in this)
            {
                segment.SetParentPath(path);
            }
        }

               
        //public int Count { get; }
        //public bool IsReadOnly { get; }
               
        //public PathSegment this[int index] { get; set; }
               
        //public void Add(PathSegment item);
        ///// <summary>
        ///// Removes all items from the collection.
        ///// </summary>
        //public void Clear();
        //public bool Contains(PathSegment item);
        //public void CopyTo(PathSegment[] array, int arrayIndex);
        //public int IndexOf(PathSegment item);
        //public void Insert(int index, PathSegment item);
        //public bool Remove(PathSegment item);
        //public void RemoveAt(int index);


    }
}