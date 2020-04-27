

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
    /// Represents a collection of PathFigure objects that collectively make up the
    /// geometry of a PathGeometry.
    /// </summary>
    public sealed partial class PathFigureCollection : PresentationFrameworkCollection<PathFigure>
    {
        #region Data

        private Path _parentPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance that is empty.
        /// </summary>
        public PathFigureCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">int - The number of elements that the new list is initially capable of storing.</param>
        public PathFigureCollection(int capacity) : base(capacity)
        {

        }

        /// <summary>
        /// Creates a PathFigureCollection with all of the same elements as collection
        /// </summary>
        public PathFigureCollection(IEnumerable<PathFigure> figures) : base(figures)
        {

        }

        #endregion

        #region Overriden Methods

        internal override void AddOverride(PathFigure figure)
        {
            if (figure == null)
            {
                throw new ArgumentNullException("figure");
            }
            if (this._parentPath != null)
            {
                figure.SetParentPath(this._parentPath);
            }
            this.AddInternal(figure);
            this.NotifyCollectionChanged();
        }

        internal override bool RemoveOverride(PathFigure figure)
        {
            if (this.RemoveInternal(figure))
            {
                figure.SetParentPath(null);
                this.NotifyCollectionChanged();
                return true;
            }
            return false;
        }

        internal override void RemoveAtOverride(int index)
        {
            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this.GetItemInternal(index).SetParentPath(null);
            this.RemoveAtInternal(index);
            this.NotifyCollectionChanged();
        }

        internal override void InsertOverride(int index, PathFigure figure)
        {
            if (figure == null)
            {
                throw new ArgumentNullException("figure");
            }
            if (this._parentPath != null)
            {
                figure.SetParentPath(this._parentPath);
            }
            this.InsertInternal(index, figure);
            this.NotifyCollectionChanged();
        }

        internal override void ClearOverride()
        {
            foreach (PathFigure figure in this)
            {
                figure.SetParentPath(null);
            }
            this.ClearInternal();
            this.NotifyCollectionChanged();
        }

        internal override void SetItemOverride(int index, PathFigure figure)
        {
            if (this._parentPath != null)
            {
                PathFigure oldItem = this.GetItemInternal(index);
                oldItem.SetParentPath(null);
                figure.SetParentPath(this._parentPath);
            }
            this.SetItemInternal(index, figure);
            this.NotifyCollectionChanged();
        }

        #endregion

        #region Internal Methods

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

        private void NotifyCollectionChanged()
        {
            if (this._parentPath != null)
            {
                this._parentPath.ScheduleRedraw();
            }
        }

        #endregion
    }
}
