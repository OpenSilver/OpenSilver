

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
        #region Data

        private Path _parentPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance that is empty.
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

        /// <summary>
        /// Creates a PathSegmentCollection with all of the same elements as collection
        /// </summary>
        public PathSegmentCollection(IEnumerable<PathSegment> segments) : base(segments)
        {

        }

        #endregion

        #region Overriden Methods

        internal override void AddOverride(PathSegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException("value");
            }
            if (this._parentPath != null)
            {
                segment.SetParentPath(this._parentPath);
            }
            this.AddDependencyObjectInternal(segment);
            this.NotifyCollectionChanged();
        }

        internal override bool RemoveOverride(PathSegment segment)
        {
            if (this.RemoveDependencyObjectInternal(segment))
            {
                segment.SetParentPath(null);
                this.NotifyCollectionChanged();
                return true;
            }
            return false;
        }

        internal override void RemoveAtOverride(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this.GetItemInternal(index).SetParentPath(null);
            this.RemoveAtDependencyObjectInternal(index);
            this.NotifyCollectionChanged();
        }

        internal override void InsertOverride(int index, PathSegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException("value");
            }
            if (this._parentPath != null)
            {
                segment.SetParentPath(this._parentPath);
            }
            this.InsertDependencyObjectInternal(index, segment);
            this.NotifyCollectionChanged();
        }

        internal override void ClearOverride()
        {
            foreach (PathSegment segment in this)
            {
                segment.SetParentPath(null);
            }
            this.ClearDependencyObjectInternal();
            this.NotifyCollectionChanged();
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
            this.NotifyCollectionChanged();
        }

        #endregion

        #region Internal Methods

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

        private void NotifyCollectionChanged()
        {
            if (this._parentPath != null)
            {
                this._parentPath.ScheduleRedraw();
            }
        }

        #endregion
    }

#if no
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
    }
#endif
}