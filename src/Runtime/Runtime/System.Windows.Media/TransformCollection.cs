
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

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    public sealed partial class TransformCollection : PresentationFrameworkCollection<Transform>
    {
        private TransformGroup _parentTransform;

        public TransformCollection() : base(false)
        {
        }

        internal override void AddOverride(Transform value)
        {
            this.SubscribeToChangedEvent(value);

            this.AddDependencyObjectInternal(value);
        }

        internal override void ClearOverride()
        {
            foreach (Transform t in this)
            {
                this.UnsubscribeToChangedEvent(t);
            }

            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, Transform value)
        {
            this.SubscribeToChangedEvent(value);

            this.InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.UnsubscribeToChangedEvent(this[index]);

            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override Transform GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, Transform value)
        {
            this.UnsubscribeToChangedEvent(this[index]);
            this.SubscribeToChangedEvent(value);

            this.SetItemDependencyObjectInternal(index, value);
        }

        internal void SetParentTransform(TransformGroup transform)
        {
            this._parentTransform = transform;
        }

        private void SubscribeToChangedEvent(Transform transform)
        {
            transform.Changed += new EventHandler(this.TransformChanged);
        }

        private void UnsubscribeToChangedEvent(Transform transform)
        {
            transform.Changed -= new EventHandler(this.TransformChanged);
        }

        private void TransformChanged(object sender, EventArgs e)
        {
            this._parentTransform?.RaiseTransformChanged();
        }
    }
}
