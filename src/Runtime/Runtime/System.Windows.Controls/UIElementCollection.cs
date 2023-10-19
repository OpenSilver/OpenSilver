
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

using System.Collections.Specialized;

namespace System.Windows.Controls
{
    /// <exclude/>
    public class UIElementCollection : PresentationFrameworkCollection<UIElement>
    {
        internal UIElementCollection(UIElement visualParent, FrameworkElement logicalParent) : base(true)
        {
            if (visualParent == null)
            {
                throw new ArgumentNullException($"'{nameof(visualParent)}' must be provided when instantiating '{GetType()}'");
            }

            VisualParent = visualParent;
            LogicalParent = logicalParent;
        }

        internal UIElement VisualParent { get; }

        internal FrameworkElement LogicalParent { get; }

        internal override void AddOverride(UIElement value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            SetLogicalParent(value);
            SetVisualParent(value);
            AddInternal(value);

            VisualParent.InvalidateMeasure();
        }

        internal override void ClearOverride()
        {
            int count = CountInternal;
            if (count > 0)
            {
                UIElement[] uies = new UIElement[count];
                for (int i = 0; i < count; ++i)
                {
                    uies[i] = GetItemInternal(i);
                }

                for (int i = 0; i < count; ++i)
                {
                    ClearVisualParent(uies[i]);
                    ClearLogicalParent(uies[i]);
                }

                ClearInternal();

                VisualParent.InvalidateMeasure();
            }
        }

        internal override UIElement GetItemOverride(int index)
        {
            if (index < 0 || index >= CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return GetItemInternal(index);
        }

        internal override void InsertOverride(int index, UIElement value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (index < 0 || index > CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            SetLogicalParent(value);
            SetVisualParent(value);
            InsertInternal(index, value);

            VisualParent.InvalidateMeasure();
        }

        internal override void RemoveAtOverride(int index)
        {
            if (index < 0 || index >= CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            UIElement oldChild = GetItemInternal(index);
            ClearVisualParent(oldChild);
            ClearLogicalParent(oldChild);
            RemoveAtInternal(index);
            
            VisualParent.InvalidateMeasure();
        }

        internal override void SetItemOverride(int index, UIElement value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (index < 0 || index >= CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

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
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => base.CollectionChanged += value;
            remove => base.CollectionChanged -= value;
        }

        private void SetLogicalParent(UIElement child)
        {
            LogicalParent?.AddLogicalChild(child);
        }

        private void ClearLogicalParent(UIElement child)
        {
            LogicalParent?.RemoveLogicalChild(child);
        }

        private void SetVisualParent(UIElement child)
        {
            VisualParent.AddVisualChild(child);
        }

        private void ClearVisualParent(UIElement child)
        {
            VisualParent.RemoveVisualChild(child);
        }
    }
}
