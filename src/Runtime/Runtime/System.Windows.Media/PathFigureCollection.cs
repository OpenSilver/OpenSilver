
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Shapes;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a collection of <see cref="PathFigure"/> objects that collectively
    /// make up the geometry of a <see cref="PathGeometry"/>.
    /// </summary>
    public sealed class PathFigureCollection : PresentationFrameworkCollection<PathFigure>
    {
        private Geometry _parentGeometry;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFigureCollection"/> class.
        /// </summary>
        public PathFigureCollection()
            : base(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFigureCollection"/> class that 
        /// can initially contain the specified number of <see cref="PathFigure"/> objects.
        /// </summary>
        /// <param name="capacity">
        /// The initial capacity of this <see cref="PathFigureCollection"/>.
        /// </param>
        public PathFigureCollection(int capacity)
            : base(capacity, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFigureCollection"/> class 
        /// that contains the specified <see cref="PathFigure"/> objects.
        /// </summary>
        /// <param name="figures">
        /// The collection of <see cref="PathFigure"/> objects which collectively make 
        /// up the geometry of the <see cref="Path"/>.
        /// </param>
        public PathFigureCollection(IEnumerable<PathFigure> figures)
            : base(figures, true)
        {
        }

        internal override void AddOverride(PathFigure figure)
        {
            SetParentGeometry(figure, _parentGeometry);
            AddDependencyObjectInternal(figure);
        }

        internal override void RemoveAtOverride(int index)
        {
            SetParentGeometry(GetItemInternal(index), null);
            RemoveAtDependencyObjectInternal(index);
        }

        internal override void InsertOverride(int index, PathFigure figure)
        {
            SetParentGeometry(figure, _parentGeometry);
            InsertDependencyObjectInternal(index, figure);
        }

        internal override void ClearOverride()
        {
            foreach (PathFigure figure in this)
            {
                SetParentGeometry(figure, null);
            }

            ClearDependencyObjectInternal();
        }

        internal override PathFigure GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, PathFigure figure)
        {
            SetParentGeometry(GetItemInternal(index), null);
            SetParentGeometry(figure, _parentGeometry);
            SetItemDependencyObjectInternal(index, figure);
        }

        internal void SetParentGeometry(Geometry geometry)
        {
            if (_parentGeometry == geometry) return;

            _parentGeometry = geometry;
            foreach (var figure in this)
            {
                figure.SetParentGeometry(geometry);
            }
        }

        private static void SetParentGeometry(PathFigure figure, Geometry geometry)
        {
            Debug.Assert(figure is not null);
            figure.SetParentGeometry(geometry);
        }
    }
}
