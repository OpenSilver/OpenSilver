
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
    /// Represents a collection of <see cref="GradientStop"/> objects that can be individually accessed by index.
    /// </summary>
    public sealed class GradientStopCollection : PresentationFrameworkCollection<GradientStop>
    {
        private WeakReference<GradientBrush> _ownerWeakRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStopCollection"/> class.
        /// </summary>
        public GradientStopCollection()
            : base(true)
        {
        }

        internal override void AddOverride(GradientStop gradientStop)
        {
            SubscribeToChangedEvent(gradientStop);
            AddDependencyObjectInternal(gradientStop);
        }

        internal override void ClearOverride()
        {
            foreach (GradientStop gs in this)
            {
                UnsubscribeToChangedEvent(gs);
            }

            ClearDependencyObjectInternal();
        }

        internal override void RemoveAtOverride(int index)
        {
            UnsubscribeToChangedEvent(this[index]);
            RemoveAtDependencyObjectInternal(index);
        }

        internal override void InsertOverride(int index, GradientStop gradientStop)
        {
            SubscribeToChangedEvent(gradientStop);
            InsertDependencyObjectInternal(index, gradientStop);
        }

        internal override GradientStop GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, GradientStop gradientStop)
        {
            UnsubscribeToChangedEvent(this[index]);
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
            if (_ownerWeakRef.TryGetTarget(out GradientBrush owner))
            {
                owner.RaiseBrushChanged();
            }
        }
    }
}
