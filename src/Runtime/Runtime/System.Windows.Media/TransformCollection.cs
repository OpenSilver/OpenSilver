
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
    public sealed class TransformCollection : PresentationFrameworkCollection<Transform>
    {
        private WeakReference<TransformGroup> _ownerWeakRef;

        public TransformCollection()
            : base(true)
        {
        }

        internal override void AddOverride(Transform value)
        {
            SubscribeToChangedEvent(value);
            AddDependencyObjectInternal(value);
        }

        internal override void ClearOverride()
        {
            foreach (Transform t in this)
            {
                UnsubscribeToChangedEvent(t);
            }

            ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, Transform value)
        {
            SubscribeToChangedEvent(value);
            InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            UnsubscribeToChangedEvent(this[index]);
            RemoveAtDependencyObjectInternal(index);
        }

        internal override Transform GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, Transform value)
        {
            UnsubscribeToChangedEvent(this[index]);
            SubscribeToChangedEvent(value);
            SetItemDependencyObjectInternal(index, value);
        }

        internal void SetOwner(TransformGroup owner) =>
            _ownerWeakRef = owner is null ? null : new WeakReference<TransformGroup>(owner);

        private void SubscribeToChangedEvent(Transform transform)
        {
            Debug.Assert(transform is not null);
            transform.Changed += new EventHandler(TransformChanged);
        }

        private void UnsubscribeToChangedEvent(Transform transform)
        {
            Debug.Assert(transform is not null);
            transform.Changed -= new EventHandler(TransformChanged);
        }

        private void TransformChanged(object sender, EventArgs e)
        {
            if (_ownerWeakRef.TryGetTarget(out TransformGroup owner))
            {
                owner.RaiseTransformChanged();
            }
        }
    }
}
