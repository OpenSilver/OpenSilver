
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
    /// Represents a collection of PathFigure objects that collectively make up the
    /// geometry of a PathGeometry.
    /// </summary>
    public sealed partial class PathFigureCollection : PresentationFrameworkCollection<PathFigure>
    {
        private Path _parentPath;

        /// <summary>
        /// Initializes a new instance that is empty.
        /// </summary>
        public PathFigureCollection() : base(false)
        {
        }

        /// <summary>
        /// Initializes a new instance that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">int - The number of elements that the new list is initially capable of storing.</param>
        public PathFigureCollection(int capacity) : base(capacity, false)
        {
        }

        /// <summary>
        /// Creates a PathFigureCollection with all of the same elements as collection
        /// </summary>
        public PathFigureCollection(IEnumerable<PathFigure> figures) : base(figures, false)
        {
        }

        internal override void AddOverride(PathFigure figure)
        {
            figure.SetParentPath(this._parentPath);
            this.AddDependencyObjectInternal(figure);
            this.NotifyParent();
        }

        internal override void RemoveAtOverride(int index)
        {
            this.GetItemInternal(index).SetParentPath(null);
            this.RemoveAtDependencyObjectInternal(index);
            this.NotifyParent();
        }

        internal override void InsertOverride(int index, PathFigure figure)
        {
            figure.SetParentPath(this._parentPath);
            this.InsertDependencyObjectInternal(index, figure);
            this.NotifyParent();
        }

        internal override void ClearOverride()
        {
            foreach (PathFigure figure in this)
            {
                figure.SetParentPath(null);
            }

            this.ClearDependencyObjectInternal();
            this.NotifyParent();
        }

        internal override PathFigure GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, PathFigure figure)
        {
            PathFigure oldItem = this.GetItemInternal(index);
            oldItem.SetParentPath(null);
            figure.SetParentPath(this._parentPath);
            this.SetItemDependencyObjectInternal(index, figure);
            this.NotifyParent();
        }

        internal void SetParentPath(Path path)
        {
            if (this._parentPath != path)
            {
                this._parentPath = path;
                foreach (PathFigure figure in this)
                {
                    figure.SetParentPath(path);
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
