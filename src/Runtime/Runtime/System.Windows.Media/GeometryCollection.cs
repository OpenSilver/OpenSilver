
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

using System.Diagnostics;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a collection of <see cref="Geometry"/> objects.
    /// </summary>
    public sealed class GeometryCollection : PresentationFrameworkCollection<Geometry>
    {
        private WeakReference<GeometryGroup> _ownerWeakRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryCollection"/> class.
        /// </summary>
        public GeometryCollection()
            : base(true)
        {
        }

        internal override void AddOverride(Geometry value)
        {
            ListenForPathChanges(value);
            AddDependencyObjectInternal(value);
        }

        internal override void ClearOverride()
        {
            foreach (Geometry geometry in InternalItems)
            {
                StopListeningForPathChanges(geometry);
            }

            ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, Geometry value)
        {
            ListenForPathChanges(value);
            InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            StopListeningForPathChanges(GetItemInternal(index));
            RemoveAtDependencyObjectInternal(index);
        }

        internal override Geometry GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, Geometry value)
        {
            StopListeningForPathChanges(GetItemInternal(index));
            ListenForPathChanges(value);
            SetItemDependencyObjectInternal(index, value);
        }

        internal void SetOwner(GeometryGroup owner) =>
            _ownerWeakRef = owner is null ? null : new WeakReference<GeometryGroup>(owner);

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
            if (e.AffectsMeasure && _ownerWeakRef.TryGetTarget(out GeometryGroup owner))
            {
                owner.RaisePathChanged();
            }
        }
    }
}
