
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
using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a collection of <see cref="PathSegment"/> objects that can
    /// be individually accessed by index.
    /// </summary>
    public sealed class PathSegmentCollection : PresentationFrameworkCollection<PathSegment>
    {
        private Geometry _parentGeometry;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathSegmentCollection"/> class.
        /// </summary>
        public PathSegmentCollection()
            : base(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathSegmentCollection"/> class with 
        /// the specified capacity, or the number of <see cref="PathSegment"/> objects the 
        /// collection is initially capable of storing.
        /// </summary>
        /// <param name="capacity">
        /// The number of <see cref="PathSegment"/> objects that the collection is initially 
        /// capable of storing.
        /// </param>
        public PathSegmentCollection(int capacity)
            : base(capacity, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathSegmentCollection"/> class with 
        /// the specified collection of <see cref="PathSegment"/> objects.
        /// </summary>
        /// <param name="segments">
        /// The collection of <see cref="PathSegment"/> objects that make up the 
        /// <see cref="PathSegmentCollection"/>.
        /// </param>
        public PathSegmentCollection(IEnumerable<PathSegment> segments)
            : base(segments, true)
        {
        }

        internal override void AddOverride(PathSegment segment)
        {
            SetParentGeometry(segment, _parentGeometry);
            AddDependencyObjectInternal(segment);
        }

        internal override void RemoveAtOverride(int index)
        {
            SetParentGeometry(GetItemInternal(index), null);
            RemoveAtDependencyObjectInternal(index);
        }

        internal override void InsertOverride(int index, PathSegment segment)
        {
            SetParentGeometry(segment, _parentGeometry);
            InsertDependencyObjectInternal(index, segment);
        }

        internal override void ClearOverride()
        {
            foreach (var segment in this)
            {
                SetParentGeometry(segment, null);
            }
            ClearDependencyObjectInternal();
        }

        internal override PathSegment GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, PathSegment segment)
        {
            SetParentGeometry(GetItemInternal(index), null);
            SetParentGeometry(segment, _parentGeometry);
            SetItemDependencyObjectInternal(index, segment);
        }

        internal void SetParentGeometry(Geometry geometry)
        {
            if (_parentGeometry == geometry) return;

            _parentGeometry = geometry;
            foreach (var segment in this)
            {
                segment.SetParentGeometry(geometry);
            }
        }

        private static void SetParentGeometry(PathSegment segment, Geometry geometry)
        {
            Debug.Assert(segment is not null);
            segment.SetParentGeometry(geometry);
        }
    }
}