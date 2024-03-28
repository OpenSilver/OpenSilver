
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
using System.Threading;
using Stop = (double Offset, System.Windows.Media.Color Color);

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a collection of <see cref="GradientStop"/> objects that can be individually accessed by index.
    /// </summary>
    public sealed class GradientStopCollection : PresentationFrameworkCollection<GradientStop>
    {
        private WeakReference<GradientBrush> _ownerWeakRef;
        private Stop[] _sortedStops;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStopCollection"/> class.
        /// </summary>
        public GradientStopCollection()
            : base(true)
        {
        }

        internal Stop[] GetSortedCollection()
        {
            if (_sortedStops is null)
            {
                List<GradientStop> stops = InternalItems;
                if (stops.Count == 0)
                {
                    _sortedStops = Array.Empty<Stop>();
                }
                else
                {
                    _sortedStops = new Stop[stops.Count];
                    for (int i = 0; i < stops.Count; i++)
                    {
                        GradientStop stop = stops[i];
                        _sortedStops[i] = (stop.Offset, stop.Color);
                    }

                    Array.Sort(_sortedStops, static (l, r) => l.Offset.CompareTo(r.Offset));
                }
            }

            return _sortedStops;
        }

        internal override void AddOverride(GradientStop gradientStop)
        {
            _sortedStops = null;
            SubscribeToChangedEvent(gradientStop);
            AddDependencyObjectInternal(gradientStop);
        }

        internal override void ClearOverride()
        {
            _sortedStops = null;
            foreach (GradientStop gs in InternalItems)
            {
                UnsubscribeToChangedEvent(gs);
            }

            ClearDependencyObjectInternal();
        }

        internal override void RemoveAtOverride(int index)
        {
            _sortedStops = null;
            UnsubscribeToChangedEvent(GetItemInternal(index));
            RemoveAtDependencyObjectInternal(index);
        }

        internal override void InsertOverride(int index, GradientStop gradientStop)
        {
            _sortedStops = null;
            SubscribeToChangedEvent(gradientStop);
            InsertDependencyObjectInternal(index, gradientStop);
        }

        internal override GradientStop GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, GradientStop gradientStop)
        {
            _sortedStops = null;
            UnsubscribeToChangedEvent(GetItemInternal(index));
            SubscribeToChangedEvent(gradientStop);
            SetItemDependencyObjectInternal(index, gradientStop);
        }

        internal void SetOwner(GradientBrush owner) =>
            _ownerWeakRef = owner is null ? null : new WeakReference<GradientBrush>(owner);

        private void SubscribeToChangedEvent(GradientStop gradientStop)
        {
            Debug.Assert(gradientStop is not null);
            gradientStop.Changed += GradientStopChanged;
        }

        private void UnsubscribeToChangedEvent(GradientStop gradientStop)
        {
            Debug.Assert(gradientStop is not null);
            gradientStop.Changed -= GradientStopChanged;
        }

        private void GradientStopChanged(object sender, EventArgs e)
        {
            _sortedStops = null;
            if (_ownerWeakRef.TryGetTarget(out GradientBrush owner))
            {
                owner.RaiseChanged();
            }
        }
    }
}
