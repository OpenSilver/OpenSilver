

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class PanelDragDropTarget : DragDropTarget<Panel, UIElement>
    {
        /// <summary>
        /// Initializes a new instance of the PanelDragDropTarget class.
        /// </summary>
        public PanelDragDropTarget()
        {

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
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected override void RemoveItem(Panel itemsControl, object data)
        {
            itemsControl.Children.Remove((UIElement)data);
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

        /// <summary>
        /// Create a new TItemsControlType
        /// </summary>
        /// <returns>A new TItemsControlType</returns>
        protected override Panel INTERNAL_ReturnNewTItemsControl()
        {
            return new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
        }

        [OpenSilver.NotImplemented]
        protected override int GetItemCount(Panel itemsControl)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        protected override Panel GetItemsHost(Panel itemsControl)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        protected override object ItemFromContainer(Panel itemsControl, UIElement itemContainer)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        protected override bool CanRemove(Panel itemsControl)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        protected override bool CanAddItem(Panel itemsControl, object data)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        protected override bool IsItemContainerOfItemsControl(Panel itemsControl, DependencyObject itemContainer)
        {
            throw new NotImplementedException();
        }
    }
}
