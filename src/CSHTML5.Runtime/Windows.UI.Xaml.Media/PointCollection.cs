
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