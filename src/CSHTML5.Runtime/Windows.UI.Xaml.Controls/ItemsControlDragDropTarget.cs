using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
    //[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Use of Rx makes code appear more complex than it is to static analyzer.")]
    public class ItemsControlDragDropTarget : DragDropTarget<ItemsControl, UIElement> //Was abstract but it doesn't make much sense to me.
    {

        /// <summary>
        /// Adds an item to an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void AddItem(ItemsControl itemsControl, object data)
        {
            FrameworkElement dataAsFrameworkElement = data as FrameworkElement;
            if (dataAsFrameworkElement != null)
            {
                if (dataAsFrameworkElement.DataContext != null) //todo: handle the case where DataContext is null so we can add the placeholder
                    itemsControl.AddItem(dataAsFrameworkElement.DataContext);
            }
            else
            {
                itemsControl.AddItem(data);
            }
        }

        /// <summary>
        /// Retrieves the item container at a given index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to retrieve the container.
        /// </param>
        /// <returns>The item container at a given index.</returns>
        protected override UIElement ContainerFromIndex(ItemsControl itemsControl, int index)
        {
            return itemsControl.ItemContainerGenerator.ContainerFromIndex(index) as UIElement;
        }

        /// <summary>
        /// Retrieves the index of an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The index of an item container.</returns>
        protected override int? IndexFromContainer(ItemsControl itemsControl, UIElement itemContainer)
        {
            int index = itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer);
            return (index == -1) ? null : new int?(index);
        }

        /// <summary>
        /// Inserts an item into an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void InsertItem(ItemsControl itemsControl, int index, object data)
        {
            FrameworkElement dataAsFrameworkElement = data as FrameworkElement;
            if(dataAsFrameworkElement != null)
            {
                if(dataAsFrameworkElement.DataContext != null) //todo: handle the case where DataContext is null so we can add the placeholder
                    itemsControl.InsertItem(index, dataAsFrameworkElement.DataContext);
            }
            else
            {
                itemsControl.InsertItem(index, data);
            }
        }

        /// <summary>
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected override void RemoveItem(ItemsControl itemsControl, object data)
        {
            FrameworkElement dataAsFrameworkElement = data as FrameworkElement;
            if (dataAsFrameworkElement != null)
            {
                itemsControl.RemoveItem(dataAsFrameworkElement.DataContext);
            }
            else
            {
                itemsControl.RemoveItem(data);
            }
        }

        /// <summary>
        /// Removes an item from an items control by index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index of the item to be removed.</param>
        protected override void RemoveItemAtIndex(ItemsControl itemsControl, int index)
        {
            //todo: when we will handle the placeholder (_sourcePlaceholder in DragDropTarget.cs) properly, uncomment the following ?
            //not: we could afford to comment it because it was only called for the placeholder.
            //itemsControl.RemoveItemAtIndex(index);
        }

        /// <summary>
        /// Create a new TItemsControlType
        /// </summary>
        /// <returns>A new TItemsControlType</returns>
        protected override ItemsControl INTERNAL_ReturnNewTItemsControl()
        {
            return new ItemsControl();
        }

        /// <summary>
        /// Confirms that the Content property is set to an object
        /// of type Panel.
        /// </summary>
        /// <param name="oldContent">The old content value.</param>
        /// <param name="newContent">The new content value.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            // Enforce that the Content property must be a Panel.
            if (newContent != null && !(newContent is ItemsControl))
            {
                throw new ArgumentException("Content must be an ItemsControl.");
            }

            base.OnContentChanged(oldContent, newContent);
        }

        internal override int INTERNAL_GetNumberOfElementsBetweenItemsRootAndDragDropTarget()
        {
            return 3;
        }




        #region removed stuff

        //#region public Duration ScrollItemAnimationDuration
        ///// <summary>
        ///// Gets or sets the duration to use to animate an item into view.
        ///// </summary>
        //public Duration ScrollItemAnimationDuration
        //{
        //    get { return (Duration)GetValue(ScrollItemAnimationDurationProperty); }
        //    set { SetValue(ScrollItemAnimationDurationProperty, value); }
        //}

        ///// <summary>
        ///// Identifies the ScrollItemIntoViewAnimationDuration dependency property.
        ///// </summary>
        ////[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Member is needed for dependency property.")]
        //public static readonly DependencyProperty ScrollItemAnimationDurationProperty =
        //    DependencyProperty.Register(
        //        "ScrollItemAnimationDuration",
        //        typeof(Duration),
        //        typeof(ItemsControlDragDropTarget<TItemsControl, TItemContainerType>),
        //        new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.15))));
        //#endregion public Duration ScrollItemAnimationDuration

        ///// <summary>
        ///// Returns a value indicating whether an item can be added to the
        ///// items control.
        ///// </summary>
        ///// <param name="itemsControl">The items control.</param>
        ///// <param name="data">The data to be added.</param>
        ///// <returns>A value indicating whether an item can be added to the
        ///// items control.</returns>
        //protected override bool CanAddItem(TItemsControl itemsControl, object data)
        //{
        //    return itemsControl.CanAddItem(data);
        //}

        ///// <summary>
        ///// Retrieves the number of items in an items control.
        ///// </summary>
        ///// <param name="itemsControl">The items control.</param>
        ///// <returns>The number of items in the items control.</returns>
        //protected override int GetItemCount(TItemsControl itemsControl)
        //{
        //    return itemsControl.GetItemCount();
        //}

        ///// <summary>
        ///// Retrieves the items host for a given items control.
        ///// </summary>
        ///// <param name="itemsControl">The items control.</param>
        ///// <returns>The items host for a given items control.</returns>
        //protected override Panel GetItemsHost(TItemsControl itemsControl)
        //{
        //    return itemsControl.GetItemsHost();
        //}

        ///// <summary>
        ///// Returns a value indicating whether an item can be removed from the
        ///// items control.
        ///// </summary>
        ///// <param name="itemsControl">The items control.</param>
        ///// <returns>A value indicating whether an item can be removed from the
        ///// items control.</returns>
        //protected override bool CanRemove(TItemsControl itemsControl)
        //{
        //    return itemsControl.CanRemoveItem();
        //}

        ///// <summary>
        ///// Returns a value indicating whether a container belongs to an items 
        ///// control.
        ///// </summary>
        ///// <param name="itemsControl">The items control.</param>
        ///// <param name="itemContainer">The item container.</param>
        ///// <returns>A value indicating whether a container belongs to an items 
        ///// control.</returns>
        //protected override bool IsItemContainerOfItemsControl(TItemsControl itemsControl, DependencyObject itemContainer)
        //{
        //    return itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1;
        //}

        ///// <summary>
        ///// Gets the item from an item container.
        ///// </summary>
        ///// <param name="itemsControl">The items control.</param>
        ///// <param name="itemContainer">The item container.</param>
        ///// <returns>The data contained by the item container.</returns>
        //protected override object ItemFromContainer(TItemsControl itemsControl, TItemContainerType itemContainer)
        //{
        //    return itemsControl.ItemContainerGenerator.ItemFromContainer(itemContainer);
        //}

        ///// <summary>
        ///// Returns a value indicating whether a given items control
        ///// can scroll.
        ///// </summary>
        ///// <param name="itemsControl">The items control.</param>
        ///// <returns>The value indicating whether the given items control
        ///// can scroll.</returns>
        //protected override bool CanScroll(TItemsControl itemsControl)
        //{
        //    return itemsControl.GetScrollHost() != null;
        //}

        ///// <summary>
        ///// Scrolls a given item container into the view.
        ///// </summary>
        ///// <param name="itemsControl">The items control that contains
        ///// the item container.</param>
        ///// <param name="itemContainer">The item container to scroll into
        ///// view.</param>
        //protected override void ScrollIntoView(TItemsControl itemsControl, TItemContainerType itemContainer)
        //{
        //    ScrollViewer scrollHost = itemsControl.GetScrollHost();
        //    if (scrollHost != null)
        //    {
        //        scrollHost.ScrollIntoView(itemContainer, 10, 10, ScrollItemAnimationDuration);
        //    }
        //}

        #endregion
    }
}