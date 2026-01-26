
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

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a collection of <see cref="Geometry"/> objects.
    /// </summary>
    public sealed class GeometryCollection : PresentationFrameworkCollection<Geometry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryCollection"/> class.
        /// </summary>
        public GeometryCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryCollection"/> class with the specified collection 
        /// of <see cref="Geometry"/> objects.
        /// </summary>
        /// <param name="collection">
        /// The collection of <see cref="Geometry"/> objects that make up the <see cref="GeometryCollection"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// collection is null.
        /// </exception>
        public GeometryCollection(IEnumerable<Geometry> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            foreach (Geometry value in collection)
            {
                Add(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryCollection"/> class with the specified capacity, or the 
        /// number of <see cref="Geometry"/> objects the collection is initially capable of storing.
        /// </summary>
        /// <param name="capacity">
        /// The number of <see cref="Geometry"/> objects that the collection is initially capable of storing.
        /// </param>
        public GeometryCollection(int capacity)
            : base(capacity)
        {
        }

        internal event EventHandler Changed;

        private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);

        internal override void AddOverride(Geometry value)
        {
            ListenForPathChanges(value);
            AddDependencyObjectInternal(value);

            OnChanged();
        }

        internal override void ClearOverride()
        {
            foreach (Geometry geometry in InternalItems)
            {
                StopListeningForPathChanges(geometry);
            }

            ClearDependencyObjectInternal();

            OnChanged();
        }

        internal override void InsertOverride(int index, Geometry value)
        {
            ListenForPathChanges(value);
            InsertDependencyObjectInternal(index, value);

            OnChanged();
        }

        internal override void RemoveAtOverride(int index)
        {
            StopListeningForPathChanges(GetItemInternal(index));
            RemoveAtDependencyObjectInternal(index);

            OnChanged();
        }

        internal override Geometry GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, Geometry value)
        {
            StopListeningForPathChanges(GetItemInternal(index));
            ListenForPathChanges(value);
            SetItemDependencyObjectInternal(index, value);

            OnChanged();
        }

        private void ListenForPathChanges(Geometry geometry)
        {
            Debug.Assert(geometry is not null);
            geometry.Invalidated += new EventHandler<GeometryInvalidatedEventsArgs>(OnChildrenPathChanged);
        }

        private void StopListeningForPathChanges(Geometry geometry)
        {
            Debug.Assert(geometry is not null);
            geometry.Invalidated -= new EventHandler<GeometryInvalidatedEventsArgs>(OnChildrenPathChanged);
        }

        private void OnChildrenPathChanged(object sender, GeometryInvalidatedEventsArgs e)
        {
            if (e.AffectsMeasure)
            {
                OnChanged();
            }
        }
    }
}
