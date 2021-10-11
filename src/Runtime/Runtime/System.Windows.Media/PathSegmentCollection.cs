
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

#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
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
    public sealed partial class PathSegmentCollection : PresentationFrameworkCollection<PathSegment>
    {
        private Path _parentPath;

        /// <summary>
        /// Initializes a new instance that is empty.
        /// </summary>
        public PathSegmentCollection() : base(false)
        {
        }

        /// <summary>
        /// Initializes a new instance that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity"> int - The number of elements that the new list is initially capable of storing. </param>
        public PathSegmentCollection(int capacity) : base(capacity, false)
        {
        }

        /// <summary>
        /// Creates a PathSegmentCollection with all of the same elements as collection
        /// </summary>
        public PathSegmentCollection(IEnumerable<PathSegment> segments) : base(segments, false)
        {
        }

        internal override void AddOverride(PathSegment segment)
        {
            if (this._parentPath != null)
            {
                segment.SetParentPath(this._parentPath);
            }

            this.AddDependencyObjectInternal(segment);
            this.NotifyParent();
        }

        internal override void RemoveAtOverride(int index)
        {
            this.GetItemInternal(index).SetParentPath(null);
            this.RemoveAtDependencyObjectInternal(index);
            this.NotifyParent();
        }

        internal override void InsertOverride(int index, PathSegment segment)
        {
            if (this._parentPath != null)
            {
                segment.SetParentPath(this._parentPath);
            }

            this.InsertDependencyObjectInternal(index, segment);
            this.NotifyParent();
        }

        internal override void ClearOverride()
        {
            foreach (PathSegment segment in this)
            {
                segment.SetParentPath(null);
            }

            this.ClearDependencyObjectInternal();
            this.NotifyParent();
        }

        internal override PathSegment GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, PathSegment segment)
        {
            if (this._parentPath != null)
            {
                PathSegment oldItem = this[index];
                oldItem.SetParentPath(null);
                segment.SetParentPath(this._parentPath);
            }

            this.SetItemDependencyObjectInternal(index, segment);
            this.NotifyParent();
        }

        internal void SetParentPath(Path path)
        {
            if (this._parentPath != path)
            {
                this._parentPath = path;
                foreach (PathSegment segment in this)
                {
                    segment.SetParentPath(path);
                }
            }
        }

        private void NotifyParent()
        {
            if (this._parentPath != null)
            {
                this._parentPath.ScheduleRedraw();
            }
        }
    }
}