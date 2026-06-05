
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

using OpenSilver.Internal;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents an ordered collection of <see cref="UIElement"/> objects.
    /// </summary>
    public class UIElementCollection : PresentationFrameworkCollection<UIElement>
    {
        private CollectionChangedHelper _collectionChanged;

        public UIElementCollection(UIElement visualParent, FrameworkElement logicalParent)
        {
            if (visualParent is null)
            {
                throw new ArgumentNullException(nameof(visualParent), string.Format(Strings.Panel_NoNullVisualParent, nameof(visualParent), GetType()));
            }

            VisualParent = visualParent;
            LogicalParent = logicalParent;
        }

        internal UIElement VisualParent { get; }

        internal FrameworkElement LogicalParent { get; }

        internal sealed override void AddOverride(UIElement value)
        {
            _collectionChanged?.CheckReentrancy();

            SetLogicalParent(value);
            SetVisualParent(value);

            AddInternal(value);

            VisualParent.InvalidateMeasure();

            _collectionChanged?.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, InternalCount - 1);
        }

        internal sealed override void ClearOverride()
        {
            _collectionChanged?.CheckReentrancy();

            int count = InternalCount;
            if (count > 0)
            {
                UIElement[] uies = InternalItems.ToArray();

                for (int i = 0; i < count; ++i)
                {
                    ClearVisualParent(uies[i]);
                    ClearLogicalParent(uies[i]);
                }

                ClearInternal();

                VisualParent.InvalidateMeasure();
            }

            _collectionChanged?.OnCollectionReset();
        }

        internal sealed override UIElement GetItemOverride(int index) => GetItemInternal(index);

        internal sealed override void InsertOverride(int index, UIElement value)
        {
            _collectionChanged?.CheckReentrancy();

            SetLogicalParent(value);
            SetVisualParent(value);

            InsertInternal(index, value);

            VisualParent.InvalidateMeasure();

            _collectionChanged?.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, index);
        }

        internal sealed override void RemoveAtOverride(int index)
        {
            _collectionChanged?.CheckReentrancy();

            UIElement oldChild = GetItemInternal(index);

            ClearVisualParent(oldChild);
            ClearLogicalParent(oldChild);
            RemoveAtInternal(index);
            
            VisualParent.InvalidateMeasure();

            _collectionChanged?.OnCollectionChanged(NotifyCollectionChangedAction.Remove, oldChild, index);
        }

        internal void RemoveNoVerify(UIElement uie)
        {
            _collectionChanged?.CheckReentrancy();

            int index = IndexOf(uie);
            UIElement oldChild = GetItemInternal(index);

            ClearVisualParent(oldChild);
            RemoveAtInternal(index);

            _collectionChanged?.OnCollectionChanged(NotifyCollectionChangedAction.Remove, oldChild, index);
        }

        internal sealed override void SetItemOverride(int index, UIElement value)
        {
            _collectionChanged?.CheckReentrancy();

            UIElement oldChild = GetItemInternal(index);
            if (oldChild != value)
            {
                ClearVisualParent(oldChild);
                ClearLogicalParent(oldChild);

                SetItemInternal(index, value);

                SetLogicalParent(value);
                SetVisualParent(value);

                VisualParent.InvalidateMeasure();
            }

            _collectionChanged?.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldChild, value, index);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                _collectionChanged ??= new(this);
                _collectionChanged.CollectionChanged += value;
            }
            remove
            {
                if (_collectionChanged is CollectionChangedHelper collectionChanged)
                {
                    collectionChanged.CollectionChanged -= value;
                }
            }
        }

        private void SetLogicalParent(UIElement child) => LogicalParent?.AddLogicalChild(child);

        private void ClearLogicalParent(UIElement child) => LogicalParent?.RemoveLogicalChild(child);

        private void SetVisualParent(UIElement child) => VisualParent.InternalAddVisualChild(child);

        private void ClearVisualParent(UIElement child) => VisualParent.InternalRemoveVisualChild(child);
    }
}
