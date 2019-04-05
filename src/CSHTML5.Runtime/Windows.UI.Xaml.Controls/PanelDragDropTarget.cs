
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public class PanelDragDropTarget : DragDropTarget<Panel, UIElement>
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
    }
}
