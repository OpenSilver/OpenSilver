
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
    public sealed class PathSegmentCollection : List<PathSegment>// IList<PathSegment>, IEnumerable<PathSegment>
    {
        ///// <summary>
        ///// Initializes a new instance of the PathSegmentCollection class.
        ///// </summary>
        //public PathSegmentCollection() { }


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