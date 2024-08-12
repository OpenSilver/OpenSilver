
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
    /// Represents a collection of <see cref="Transform"/> objects that can be individually 
    /// accessed by index.
    /// </summary>
    public sealed class TransformCollection : PresentationFrameworkCollection<Transform>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformCollection"/> class.
        /// </summary>
        public TransformCollection()
        {
        }

        internal event EventHandler Changed;

        private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);

        internal override void AddOverride(Transform value)
        {
            SubscribeToChangedEvent(value);
            AddDependencyObjectInternal(value);

            OnChanged();
        }

        internal override void ClearOverride()
        {
            foreach (Transform t in InternalItems)
            {
                UnsubscribeToChangedEvent(t);
            }

            ClearDependencyObjectInternal();

            OnChanged();
        }

        internal override void InsertOverride(int index, Transform value)
        {
            SubscribeToChangedEvent(value);
            InsertDependencyObjectInternal(index, value);

            OnChanged();
        }

        internal override void RemoveAtOverride(int index)
        {
            UnsubscribeToChangedEvent(GetItemInternal(index));
            RemoveAtDependencyObjectInternal(index);

            OnChanged();
        }

        internal override Transform GetItemOverride(int index) => GetItemInternal(index);

        internal override void SetItemOverride(int index, Transform value)
        {
            UnsubscribeToChangedEvent(GetItemInternal(index));
            SubscribeToChangedEvent(value);
            SetItemDependencyObjectInternal(index, value);

            OnChanged();
        }

        private void SubscribeToChangedEvent(Transform transform)
        {
            Debug.Assert(transform is not null);
            transform.Changed += TransformChanged;
        }

        private void UnsubscribeToChangedEvent(Transform transform)
        {
            Debug.Assert(transform is not null);
            transform.Changed -= TransformChanged;
        }

        private void TransformChanged(object sender, EventArgs e) => OnChanged();
    }
}
