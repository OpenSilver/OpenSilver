

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
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a collection of Point values that can be individually accessed
    /// by index.
    /// </summary>
    public sealed partial class PointCollection : List<Point>//: IList<Point>, IEnumerable<Point>
    {
        ///// <summary>
        ///// Initializes a new instance of the PointCollection class.
        ///// </summary>
        //public PointCollection() { }



        //public int Count { get; }
        //public bool IsReadOnly { get; }

        //public Point this[int index] { get; set; }

        //public void Add(Point item);
        ///// <summary>
        ///// Removes all items from the collection.
        ///// </summary>
        //public void Clear();
        //public bool Contains(Point item);
        //public void CopyTo(Point[] array, int arrayIndex);
        //public int IndexOf(Point item);
        //public void Insert(int index, Point item);
        //public bool Remove(Point item);
        //public void RemoveAt(int index);
    }
}