

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

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Primitives;

#if MIGRATION
using System.Windows.Shapes;
#else
using System;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// A control that enabled drag and drop operations on an TItemsControl.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    /// <typeparam name="TItemsControl">The type of the control that contains 
    /// the items that can be dragged.</typeparam>
    /// <typeparam name="TItemContainerType">The type of the item container.</typeparam>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Use of Rx makes code appear more complex than it is to static analyzer.")]
    public abstract class ItemsControlDragDropTarget<TItemsControl, TItemContainerType> : DragDropTarget<TItemsControl, TItemContainerType>
        where TItemsControl : ItemsControl
        where TItemContainerType : FrameworkElement
    {
#region public Duration ScrollItemAnimationDuration
        /// <summary>
        /// Gets or sets the duration to use to animate an item into view.
        /// </summary>
        public Duration ScrollItemAnimationDuration
        {
            get { return (Duration)GetValue(ScrollItemAnimationDurationProperty); }
            set { SetValue(ScrollItemAnimationDurationProperty, value); }
        }

        /// <summary>
        /// Identifies the ScrollItemIntoViewAnimationDuration dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Member is needed for dependency property.")]
        public static readonly DependencyProperty ScrollItemAnimationDurationProperty =
            DependencyProperty.Register(
                "ScrollItemAnimationDuration",
                typeof(Duration),
                typeof(ItemsControlDragDropTarget<TItemsControl, TItemContainerType>),
                new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.15))));
#endregion public Duration ScrollItemAnimationDuration

        /// <summary>
        /// Returns a value indicating whether an item can be added to the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be added.</param>
        /// <returns>A value indicating whether an item can be added to the
        /// items control.</returns>
        protected override bool CanAddItem(TItemsControl itemsControl, object data)
        {
            return itemsControl.CanAddItem(data);
        }

        /// <summary>
        /// Retrieves the number of items in an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The number of items in the items control.</returns>
        protected override int GetItemCount(TItemsControl itemsControl)
        {
            return itemsControl.GetItemCount();
        }

        /// <summary>
        /// Retrieves the item container at a given index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to retrieve the container.
        /// </param>
        /// <returns>The item container at a given index.</returns>
        protected override TItemContainerType ContainerFromIndex(TItemsControl itemsControl, int index)
        {
            return itemsControl.ItemContainerGenerator.ContainerFromIndex(index) as TItemContainerType;
        }

        /// <summary>
        /// Retrieves the index of an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The index of an item container.</returns>
        protected override int? IndexFromContainer(TItemsControl itemsControl, TItemContainerType itemContainer)
        {
            int index = itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer);
            return (index == -1) ? null : new int?(index);
        }

        /// <summary>
        /// Retrieves the items host for a given items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The items host for a given items control.</returns>
        protected override Panel GetItemsHost(TItemsControl itemsControl)
        {
            return itemsControl.GetItemsHost();
        }

        /// <summary>
        /// Returns a value indicating whether an item can be removed from the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>A value indicating whether an item can be removed from the
        /// items control.</returns>
        protected override bool CanRemove(TItemsControl itemsControl)
        {
            return itemsControl.CanRemoveItem();
        }

        /// <summary>
        /// Adds an item to an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void AddItem(TItemsControl itemsControl, object data)
        {
            itemsControl.AddItem(data);
        }

        /// <summary>
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected override void RemoveItem(TItemsControl itemsControl, object data)
        {
            itemsControl.RemoveItem(data);
        }

        /// <summary>
        /// Removes an item from an items control by index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index of the item to be removed.</param>
        protected override void RemoveItemAtIndex(TItemsControl itemsControl, int index)
        {
            itemsControl.RemoveItemAtIndex(index);
        }

        /// <summary>
        /// Inserts an item into an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void InsertItem(TItemsControl itemsControl, int index, object data)
        {
            itemsControl.InsertItem(index, data);
        }

        /// <summary>
        /// Returns a value indicating whether a container belongs to an items 
        /// control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>A value indicating whether a container belongs to an items 
        /// control.</returns>
        protected override bool IsItemContainerOfItemsControl(TItemsControl itemsControl, DependencyObject itemContainer)
        {
            return itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1;
        }

        /// <summary>
        /// Gets the item from an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The data contained by the item container.</returns>
        protected override object ItemFromContainer(TItemsControl itemsControl, TItemContainerType itemContainer)
        {
            return itemsControl.ItemContainerGenerator.ItemFromContainer(itemContainer);
        }

        /// <summary>
        /// Returns a value indicating whether a given items control
        /// can scroll.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The value indicating whether the given items control
        /// can scroll.</returns>
        protected override bool CanScroll(TItemsControl itemsControl)
        {
            return itemsControl.GetScrollHost() != null;
        }

        /// <summary>
        /// Scrolls a given item container into the view.
        /// </summary>
        /// <param name="itemsControl">The items control that contains
        /// the item container.</param>
        /// <param name="itemContainer">The item container to scroll into
        /// view.</param>
        protected override void ScrollIntoView(TItemsControl itemsControl, TItemContainerType itemContainer)
        {
            ScrollViewer scrollHost = itemsControl.GetScrollHost();
            if (scrollHost != null)
            {
                scrollHost.ScrollIntoView(itemContainer, 10, 10, ScrollItemAnimationDuration);
            }
        }
    }
}
