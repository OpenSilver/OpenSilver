
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
using System.Collections.Specialized;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides a framework for <see cref="Panel"/> elements that virtualize
    /// their visual children.
    /// </summary>
    public abstract partial class VirtualizingPanel : Panel
    {
        /// <summary>
        /// Gets a value that identifies the <see cref="ItemContainerGenerator"/>
        /// for this <see cref="VirtualizingPanel"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ItemContainerGenerator"/> for this <see cref="VirtualizingPanel"/>.
        /// </returns>
        public IItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                return Generator;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingPanel"/> class.
        /// </summary>
        protected VirtualizingPanel()
        {
            this.ClipToBounds = true;
        }

        /// <summary>
        /// Adds the specified <see cref="UIElement"/> to the <see cref="Panel.Children"/>
        /// collection of a <see cref="VirtualizingPanel"/> element.
        /// </summary>
        /// <param name="child">
        /// The <see cref="UIElement"/> child to add to the collection.
        /// </param>
        protected void AddInternalChild(UIElement @child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// Adds the specified <see cref="UIElement"/> to the collection of a <see cref="VirtualizingPanel"/>
        /// element at the specified index position.
        /// </summary>
        /// <param name="index">
        /// The index position within the collection at which the child element is inserted.
        /// </param>
        /// <param name="child">
        /// The <see cref="UIElement"/> child to add to the collection.
        /// </param>
        protected void InsertInternalChild(int @index, UIElement @child)
        {
            Children.Insert(index, child);
        }

        /// <summary>
        /// Removes child elements from the <see cref="Panel.Children"/> collection.
        /// </summary>
        /// <param name="index">
        /// The beginning index position within the collection at which the first child element
        /// is removed.
        /// </param>
        /// <param name="range">
        /// The total number of child elements to remove from the collection.
        /// </param>
        protected void RemoveInternalChildRange(int @index, int @range)
        {
            for (int i = 0; i < range; i++)
                Children.RemoveAt(index);
        }

        /// <summary>
        /// Called when the <see cref="ItemsControl.Items"/> collection that is
        /// associated with the <see cref="ItemsControl"/> for this <see cref="Panel"/>
        /// changes.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="object"/> that raised the event.
        /// </param>
        /// <param name="args">
        /// Provides data for the <see cref="ItemContainerGenerator.ItemsChanged"/>
        /// event.
        /// </param>
        protected virtual void OnItemsChanged(object @sender, ItemsChangedEventArgs @args)
        {
        }

        /// <summary>
        /// Called when the collection of child elements is cleared by the base <see cref="Panel"/>
        /// class.
        /// </summary>
        protected virtual void OnClearChildren()
        {
        }

        /// <summary>
        /// When implemented in a derived class, generates the item at the specified index
        /// location and makes it visible.
        /// </summary>
        /// <param name="index">
        /// The index position of the item that is generated and made visible.
        /// </param>
        protected virtual void BringIndexIntoView(int @index)
        {
        }

        internal override void GenerateChildren()
        {
            if (!IsCustomLayoutRoot && !IsUnderCustomLayout)
            {
                base.GenerateChildren();
            }
            // Do nothing. Subclasses will use the exposed generator to generate children.
        }

        // This method returns a bool to indicate if or not the panel layout is affected by this collection change
        internal override bool OnItemsChangedInternal(object sender, ItemsChangedEventArgs args)
        {
            if (!IsCustomLayoutRoot && !IsUnderCustomLayout)
            {
                return base.OnItemsChangedInternal(sender, args);
            }

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    // Don't allow Panel's code to run for add/remove/replace/move
                    break;

                default:
                    base.OnItemsChangedInternal(sender, args);
                    break;
            }

            OnItemsChanged(sender, args);

            return true;
        }

        internal override void OnClearChildrenInternal()
        {
            OnClearChildren();
        }
    }
}
