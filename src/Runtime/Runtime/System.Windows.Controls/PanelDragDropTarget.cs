// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// A control that enables drag and drop operations on a Panel.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public class PanelDragDropTarget : DragDropTarget<Panel, UIElement>
    {
        /// <summary>
        /// Initializes a new instance of the PanelDragDropTarget class.
        /// </summary>
        public PanelDragDropTarget()
        {

        }

        /// <summary>
        /// Removes data from an ItemsControl.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to remove an item.</param>
        protected override void RemoveItemAtIndex(Panel itemsControl, int index)
        {
            itemsControl.Children.RemoveAt(index);
        }

        /// <summary>
        /// Adds an item to an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void AddItem(Panel itemsControl, object data)
        {
            itemsControl.Children.Add((UIElement)data);
        }

        /// <summary>
        /// Returns a value indicating whether an item can be added to the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be added.</param>
        /// <returns>A value indicating whether an item can be added to the
        /// items control.</returns>
        protected override bool CanAddItem(Panel itemsControl, object data)
        {
            return data is UIElement;
        }

        /// <summary>
        /// Returns a value indicating whether an item can be removed from the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>A value indicating whether an item can be removed from the
        /// items control.</returns>
        protected override bool CanRemove(Panel itemsControl)
        {
            return true;
        }

        /// <summary>
        /// Retrieves the item container at a given index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to retrieve the container.
        /// </param>
        /// <returns>The item container at a given index.</returns>
        protected override UIElement ContainerFromIndex(Panel itemsControl, int index)
        {
            if (itemsControl.Children.Count > index)
            {
                return itemsControl.Children[index];
            }
            return null;
        }

        /// <summary>
        /// Gets the number of items in an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The number of items in the items control.</returns>
        protected override int GetItemCount(Panel itemsControl)
        {
            return itemsControl.Children.Count;
        }

        /// <summary>
        /// Retrieves the items host for a given items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The items host for a given items control.</returns>
        protected override Panel GetItemsHost(Panel itemsControl)
        {
            // The items host is the panel itself.  This is not
            // the case for ItemsControls which is why we have
            // this overload.
            return itemsControl;
        }

        /// <summary>
        /// Retrieves the index of an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The index of an item container.</returns>
        protected override int? IndexFromContainer(Panel itemsControl, UIElement itemContainer)
        {
            var index = itemsControl.Children.IndexOf(itemContainer);
            return (index != -1) ? new int?(index) : null;
        }

        /// <summary>
        /// Inserts an item into an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void InsertItem(Panel itemsControl, int index, object data)
        {
            itemsControl.Children.Insert(index, (UIElement)data);
        }

        /// <summary>
        /// Returns a value indicating whether a container belongs to an items 
        /// control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>A value indicating whether a container belongs to an items 
        /// control.</returns>
        protected override bool IsItemContainerOfItemsControl(Panel itemsControl, DependencyObject itemContainer)
        {
            // In a panel the item container and the item are the same
            // so in order to determine if a given element is the 
            // item container we must simply test to see if it
            // is in the Panel's children collection.
            UIElement element = itemContainer as UIElement;
            if (element != null)
            {
                return itemsControl.Children.Contains(element);
            }
            return false;
        }

        /// <summary>
        /// Gets the item from an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The data contained by the item container.</returns>
        protected override object ItemFromContainer(Panel itemsControl, UIElement itemContainer)
        {
            // In a panel the item is always the item container.
            // This distinction only exists in the ItemsControl
            // where there is an item (some data) and an 
            // item container (a UI element that displays it on
            // screen).
            return itemContainer;
        }

        /// <summary>
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected override void RemoveItem(Panel itemsControl, object data)
        {
            itemsControl.Children.Remove((UIElement)data);
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
            if (newContent != null && !(newContent is Panel))
            {
                throw new ArgumentException("Content must be a Panel.");
            }

            base.OnContentChanged(oldContent, newContent);
        }
    }
}
