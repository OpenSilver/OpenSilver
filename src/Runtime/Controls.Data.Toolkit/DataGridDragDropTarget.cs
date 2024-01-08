// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Automation.Peers;

namespace System.Windows.Controls
{
    /// <summary>
    /// A control that enabled drag and drop operations on an Chart.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [TemplatePart(Name = DragContainerName, Type = typeof(Canvas))]
    [TemplatePart(Name = DragDecoratorName, Type = typeof(DragDecorator))]
    [TemplatePart(Name = InsertionIndicatorName, Type = typeof(Path))]
    [TemplatePart(Name = InsertionIndicatorContainerName, Type = typeof(Canvas))]
    [TemplatePart(Name = DragPopupName, Type = typeof(Popup))]
    public class DataGridDragDropTarget : DragDropTarget<DataGrid, DataGridRow>
    {
        /// <summary>
        /// Gets the ListBox that is the drag drop target.
        /// </summary>
        protected DataGrid DataGrid
        {
            get { return Content as DataGrid; }
        }

        /// <summary>
        /// Initializes a new instance of the DataGridDragDropTarget class.
        /// </summary>
        public DataGridDragDropTarget()
        {
            this.DefaultStyleKey = typeof(DataGridDragDropTarget);
        }

        /// <summary>
        /// Returns a value indicating whether an item can be added to the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be added.</param>
        /// <returns>A value indicating whether an item can be added to the
        /// items control.</returns>
        protected override bool CanAddItem(DataGrid itemsControl, object data)
        {
            if (itemsControl.ItemsSource != null)
            {
                return CollectionHelper.CanInsert(itemsControl.ItemsSource, data);
            }
            return false;
        }

        /// <summary>
        /// Gets the number of items in an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The number of items in the items control.</returns>
        protected override int GetItemCount(DataGrid itemsControl)
        {
            return CollectionHelper.Count(itemsControl.ItemsSource);
        }

        /// <summary>
        /// Retrieves the item container at a given index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to retrieve the container.
        /// </param>
        /// <returns>The item container at a given index.</returns>
        protected override DataGridRow ContainerFromIndex(DataGrid itemsControl, int index)
        {
            return null;
        }

        /// <summary>
        /// Retrieves the index of an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The index of an item container.</returns>
        protected override int? IndexFromContainer(DataGrid itemsControl, DataGridRow itemContainer)
        {
            return null;
        }

        /// <summary>
        /// Retrieves the items host for a given items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The items host for a given items control.</returns>
        protected override Panel GetItemsHost(DataGrid itemsControl)
        {
            return itemsControl.GetVisualDescendants().OfType<Panel>().FirstOrDefault();
        }

        /// <summary>
        /// Returns a value indicating whether an item can be removed from the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>A value indicating whether an item can be removed from the
        /// items control.</returns>
        protected override bool CanRemove(DataGrid itemsControl)
        {
            return itemsControl.ItemsSource != null && itemsControl.ItemsSource is INotifyCollectionChanged;
        }

        /// <summary>
        /// Adds an item to an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void AddItem(DataGrid itemsControl, object data)
        {
            if (itemsControl.ItemsSource != null)
            {
                CollectionHelper.Add(itemsControl.ItemsSource, data);
            }
        }

        /// <summary>
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected override void RemoveItem(DataGrid itemsControl, object data)
        {
            if (itemsControl.ItemsSource != null)
            {
                CollectionHelper.Remove(itemsControl.ItemsSource, data);
            }
        }

        /// <summary>
        /// Inserts an item into an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void InsertItem(DataGrid itemsControl, int index, object data)
        {
            if (itemsControl.ItemsSource != null)
            {
                CollectionHelper.Insert(itemsControl.ItemsSource, index, data);
            }
        }

        /// <summary>
        /// Returns a value indicating whether a container belongs to an items 
        /// control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>A value indicating whether a container belongs to an items 
        /// control.</returns>
        protected override bool IsItemContainerOfItemsControl(DataGrid itemsControl, DependencyObject itemContainer)
        {
            return itemContainer is DataGridRow;
        }

        /// <summary>
        /// Gets the item from an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The data contained by the item container.</returns>
        protected override object ItemFromContainer(DataGrid itemsControl, DataGridRow itemContainer)
        {
            return itemContainer.DataContext;
        }

        /// <summary>
        /// Adds all selected items when drag operation begins.
        /// </summary>
        /// <param name="eventArgs">Information about the event.</param>
        protected override void OnItemDragStarting(ItemDragEventArgs eventArgs)
        {
            SelectionCollection selectionCollection = new SelectionCollection(this.DataGrid.SelectedItems.OfType<object>());
            eventArgs.Data = selectionCollection;

            CardPanel cardPanel = new CardPanel();
            foreach (Selection selection in selectionCollection)
            {
                object item = selection.Item;

                DataGridRow row = this.DataGrid
                    .GetVisualDescendants()
                    .OfType<DataGridRow>()
                    .Where(dataGridRow => dataGridRow.DataContext == item)
                    .FirstOrDefault();

                if (row != null)
                {
                    cardPanel.Children.Add(new Image
                    {
                        //Source = new WriteableBitmap(row, new TranslateTransform())
                    });
                }
            }

            eventArgs.DragDecoratorContent = cardPanel;

            eventArgs.Handled = true;
            base.OnItemDragStarting(eventArgs);
        }

        /// <summary>
        /// Ensures the content of control is a DataGrid.
        /// </summary>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent != null && !(newContent is DataGrid))
            {
                throw new ArgumentException(Properties.Resources.DataGridDragDropTarget_OnContentChanged_ContentMustBeADataGrid);
            }
            base.OnContentChanged(oldContent, newContent);
        }

        /// <summary>
        /// Returns a value indicating whether a given items control
        /// can scroll.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The value indicating whether the given items control
        /// can scroll.</returns>
        protected override bool CanScroll(DataGrid itemsControl)
        {
            return true;
        }

        /// <summary>
        /// Scrolls a DataGridRow into view.
        /// </summary>
        /// <param name="itemsControl">The DataGrid containing the row.</param>
        /// <param name="itemContainer">The DataGridRow object.</param>
        protected override void ScrollIntoView(DataGrid itemsControl, DataGridRow itemContainer)
        {
            IScrollProvider scrollInfo = (IScrollProvider)DataGridAutomationPeer.CreatePeerForElement(itemsControl);

            double horizontalScrollPercentBeforeScroll = scrollInfo.HorizontalScrollPercent;
            double verticalScrollPercentBeforeScroll = scrollInfo.VerticalScrollPercent;

            itemsControl.ScrollIntoView(itemContainer.DataContext, null);

            double horizontalScrollPercentAfterScroll = scrollInfo.HorizontalScrollPercent;
            double verticalScrollPercentAfterScroll = scrollInfo.VerticalScrollPercent;

            double horizontalDiff = horizontalScrollPercentAfterScroll - horizontalScrollPercentBeforeScroll;
            double verticalDiff = verticalScrollPercentAfterScroll - verticalScrollPercentBeforeScroll;

            scrollInfo.SetScrollPercent(scrollInfo.HorizontalScrollPercent + (horizontalDiff / 2.0), scrollInfo.VerticalScrollPercent + (verticalDiff / 2.0));
        }
    }
}