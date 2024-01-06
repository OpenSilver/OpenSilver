// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// A control that enables drag and drop operations on ListBox.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [TemplatePart(Name = DragContainerName, Type = typeof(Canvas))]
    [TemplatePart(Name = DragDecoratorName, Type = typeof(DragDecorator))]
    [TemplatePart(Name = InsertionIndicatorName, Type = typeof(Path))]
    [TemplatePart(Name = InsertionIndicatorContainerName, Type = typeof(Canvas))]
    [TemplatePart(Name = DragPopupName, Type = typeof(Popup))]
    public class ListBoxDragDropTarget : ItemsControlDragDropTarget<ListBox, ListBoxItem>
    {
        /// <summary>
        /// Gets the ListBox that is the drag drop target.
        /// </summary>
        protected ListBox ListBox
        {
            get { return Content as ListBox; }
        }

        /// <summary>
        /// Initializes a new instance of the ListBoxDragDropTarget class.
        /// </summary>
        public ListBoxDragDropTarget()
        {
            this.DefaultStyleKey = typeof(ListBoxDragDropTarget);
        }

        /// <summary>
        /// Adds all selected items when drag operation begins.
        /// </summary>
        /// <param name="eventArgs">Information about the event.</param>
        protected override void OnItemDragStarting(ItemDragEventArgs eventArgs)
        {
            SelectionCollection selectionCollection = new SelectionCollection();

            // If panel is virtualized there is no way of knowing the precise
            // index of each selected item.
            Panel itemsHost = this.ListBox.GetItemsHost();
            if (itemsHost is VirtualizingPanel)
            {
                foreach (object item in this.ListBox.SelectedItems)
                {
                    selectionCollection.Add(new Selection(item));
                }

                // Adding the item dragged even if it isn't selected
                SelectionCollection defaultSelectionCollection = SelectionCollection.ToSelectionCollection(eventArgs.Data);

                if (defaultSelectionCollection.Count == 1 && !selectionCollection.Any(selection => object.Equals(selection.Item, defaultSelectionCollection[0].Item)))
                {
                    selectionCollection.Add(defaultSelectionCollection[0]);
                }
            }
            else
            {
                for (int cnt = 0; cnt < this.ListBox.Items.Count; cnt++)
                {
                    ListBoxItem listBoxItem = this.ListBox.ItemContainerGenerator.ContainerFromIndex(cnt) as ListBoxItem;
                    if (listBoxItem.IsSelected)
                    {
                        selectionCollection.Add(new Selection(cnt, this.ListBox.Items[cnt]));
                    }
                }

                // Adding the item dragged even if it isn't selected
                SelectionCollection defaultSelectionCollection = GetSelectionCollection(eventArgs.Data);
                if (defaultSelectionCollection.Count == 1)
                {
                    if (selectionCollection.All(selection => selection.Index != defaultSelectionCollection[0].Index))
                    {
                        selectionCollection.Add(defaultSelectionCollection[0]);
                    }
                }
            }

            eventArgs.Data = selectionCollection;

            CardPanel cardPanel = new CardPanel();
            IEnumerable<UIElement> itemContainers =
                selectionCollection.SelectedItems
                    .Select(item => this.ListBox.ItemContainerGenerator.ContainerFromItem(item))
                    .Where(item => item != null)
                    .OfType<UIElement>();

            foreach (ListBoxItem row in itemContainers)
            {
                cardPanel.Children.Add(new Image
                {
                    // Source = new WriteableBitmap(row, new TranslateTransform())
                });
            }

            eventArgs.DragDecoratorContent = cardPanel;

            eventArgs.Handled = true;
            base.OnItemDragStarting(eventArgs);
        }

        /// <summary>
        /// Ensures the content of control is a ListBox.
        /// </summary>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent != null && !(newContent is ListBox))
            {
                throw new ArgumentException(Resource.ListBoxDragDropTarget_OnContentChanged_ContentMustBeAListBox);
            }
            base.OnContentChanged(oldContent, newContent);
        }
    }
}