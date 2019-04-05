
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
    public sealed class PointCollection : List<Point>//: IList<Point>, IEnumerable<Point>
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