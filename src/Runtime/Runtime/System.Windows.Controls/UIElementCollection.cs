

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
using System.Collections.Generic;
using System.Collections.Specialized;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <exclude/>
    public class UIElementCollection : PresentationFrameworkCollection<UIElement>
    {
        internal UIElementCollection(UIElement visualParent, FrameworkElement logicalParent) : base(true)
        {
            if (visualParent == null)
            {
                throw new ArgumentNullException(string.Format("'{0}' must be provided when instantiating '{1}'", "visualParent", this.GetType()));
            }

            this.VisualParent = visualParent;
            this.LogicalParent = logicalParent;
        }

        internal UIElement VisualParent { get; }

        internal FrameworkElement LogicalParent { get; }

        internal override void AddOverride(UIElement value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.SetLogicalParent(value);
            this.AddInternal(value);
        }

        internal override void ClearOverride()
        {
            int count = this.CountInternal;
            UIElement[] uies = new UIElement[count];
            for (int i = 0; i < count; ++i)
            {
                uies[i] = this.GetItemInternal(i);
            }

            for (int i = 0; i < count; ++i)
            {
                this.ClearLogicalParent(uies[i]);
            }

            this.ClearInternal();
        }

        internal override UIElement GetItemOverride(int index)
        {
            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this.GetItemInternal(index);
        }

        internal override void InsertOverride(int index, UIElement value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (index < 0 || index > this.CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.SetLogicalParent(value);
            this.InsertInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            UIElement oldChild = this.GetItemInternal(index);
            this.ClearLogicalParent(oldChild);
            this.RemoveAtInternal(index);
        }

        internal override void SetItemOverride(int index, UIElement value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (index < 0 || index >= this.CountInternal)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            UIElement oldChild = this.GetItemInternal(index);
            if (oldChild != value)
            {
                this.ClearLogicalParent(oldChild);

                this.SetItemInternal(index, value);

                this.SetLogicalParent(value);
            }
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                base.CollectionChanged += value;
            }
            remove
            {
                base.CollectionChanged -= value;
            }
        }

        internal void AddRange(IEnumerable<UIElement> children)
        {
            this.CheckReentrancy();

            if (children == null)
            {
                throw new ArgumentNullException(nameof(children));
            }

            foreach (UIElement child in children)
            {
                if (child != null)
                {
                    this.SetLogicalParent(child);
                    this.AddInternal(child);
                }
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void RemoveRange(IEnumerable<UIElement> children)
        {
            this.CheckReentrancy();

            if (children == null)
            {
                throw new ArgumentNullException(nameof(children));
            }

            foreach (UIElement child in children)
            {
                if (child != null)
                {
                    this.ClearLogicalParent(child);
                    this.RemoveInternal(child);
                }
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void SetLogicalParent(UIElement child)
        {
            if (this.LogicalParent != null)
            {
                this.LogicalParent.AddLogicalChild(child);
            }
        }

        private void ClearLogicalParent(UIElement child)
        {
            if (this.LogicalParent != null)
            {
                this.LogicalParent.RemoveLogicalChild(child);
            }
        }
    }
}
