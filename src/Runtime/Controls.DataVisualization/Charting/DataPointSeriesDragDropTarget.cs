// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
#if SILVERLIGHT
using SW = Microsoft.Windows;
#else
using SW = System.Windows;
#endif

namespace System.Windows.Controls.DataVisualization.Charting
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
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Use of Rx makes code appear more complex than it is to static analyzer.")]
    public class DataPointSeriesDragDropTarget : DragDropTarget<DataPointSeries, DataPoint>
    {
        /// <summary>
        /// Gets the chart object.
        /// </summary>
        protected Chart Chart
        {
            get { return this.Content as Chart; }
        }

        /// <summary>
        /// Initializes a new instance of the DataPointSeriesDragDropTarget.
        /// </summary>
        public DataPointSeriesDragDropTarget()
        {
            this.DefaultStyleKey = typeof(DataPointSeriesDragDropTarget);
        }

        /// <summary>
        /// Ensures that the content property is set to a Chart object.
        /// </summary>
        /// <param name="oldContent">The old value.</param>
        /// <param name="newContent">The new value.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent != null && !(newContent is Chart))
            {
                throw new ArgumentException(OpenSilver.Controls.DataVisualization.OpenSilver.Controls.DataVisualization.Properties.Resources.DataPointSeriesDropTarget_set_Content_ContentMustBeAChart);
            }

            base.OnContentChanged(oldContent, newContent);
        }

        /// <summary>
        /// Returns a value indicating whether an item can be added to the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be added.</param>
        /// <returns>A value indicating whether an item can be added to the
        /// items control.</returns>
        protected override bool CanAddItem(DataPointSeries itemsControl, object data)
        {
            if (itemsControl.ItemsSource != null)
            {
                return itemsControl.ItemsSource.CanInsert(data);
            }
            return false;
        }

        /// <summary>
        /// Gets the number of items in an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The number of items in the items control.</returns>
        protected override int GetItemCount(DataPointSeries itemsControl)
        {
            return itemsControl.ItemsSource.Count();
        }

        /// <summary>
        /// Retrieves the item container at a given index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to retrieve the container.
        /// </param>
        /// <returns>The item container at a given index.</returns>
        protected override DataPoint ContainerFromIndex(DataPointSeries itemsControl, int index)
        {
            return null;
        }

        /// <summary>
        /// Retrieves the index of an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The index of an item container.</returns>
        protected override int? IndexFromContainer(DataPointSeries itemsControl, DataPoint itemContainer)
        {
            return null;
        }

        /// <summary>
        /// Retrieves the items host for a given items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The items host for a given items control.</returns>
        protected override Panel GetItemsHost(DataPointSeries itemsControl)
        {
            return VisualTreeExtensions.GetVisualDescendants(itemsControl).OfType<Panel>().FirstOrDefault();
        }

        /// <summary>
        /// Returns a value indicating whether an item can be removed from the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>A value indicating whether an item can be removed from the
        /// items control.</returns>
        protected override bool CanRemove(DataPointSeries itemsControl)
        {
            return itemsControl.ItemsSource != null && itemsControl.ItemsSource is INotifyCollectionChanged;
        }

        /// <summary>
        /// Adds an item to an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void AddItem(DataPointSeries itemsControl, object data)
        {
            if (itemsControl.ItemsSource != null)
            {
                itemsControl.ItemsSource.Add(data);
            }
        }

        /// <summary>
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected override void RemoveItem(DataPointSeries itemsControl, object data)
        {
            if (itemsControl.ItemsSource != null)
            {
                itemsControl.ItemsSource.Remove(data);
            }
        }

        /// <summary>
        /// Inserts an item into an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="data">The data to be inserted.</param>
        protected override void InsertItem(DataPointSeries itemsControl, int index, object data)
        {
            if (itemsControl.ItemsSource != null)
            {
                itemsControl.ItemsSource.Insert(index, data);
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
        protected override bool IsItemContainerOfItemsControl(DataPointSeries itemsControl, DependencyObject itemContainer)
        {
            return itemContainer is DataPoint;
        }

        /// <summary>
        /// Gets the item from an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The data contained by the item container.</returns>
        protected override object ItemFromContainer(DataPointSeries itemsControl, DataPoint itemContainer)
        {
            return itemContainer.DataContext;
        }

        /// <summary>
        /// Detects whether an item is being dragged over a legend item and
        /// selects the owner series as the drop target.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        /// <returns>The drop target.</returns>
        protected override DataPointSeries GetDropTarget(SW.DragEventArgs args)
        {
            DependencyObject originalSource = (DependencyObject)args.OriginalSource;
            LegendItem legendItem = originalSource.GetVisualAncestorsAndSelf().OfType<LegendItem>().FirstOrDefault();
            if (legendItem != null)
            {
                DataPointSeries dataPointSeries = legendItem.Owner as DataPointSeries;
                if (dataPointSeries != null)
                {
                    return dataPointSeries;
                }
            }

            DataPointSeries seriesAncestor = GetItemsControlAncestor(originalSource);
            if (seriesAncestor != null)
            {
                return seriesAncestor;
            }

            if (Chart != null)
            {
                return Chart.Series.OfType<DataPointSeries>().LastOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Retrieves the data point series that contains a dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The data point series ancestor of a given object.</returns>
        protected override DataPointSeries GetItemsControlAncestor(DependencyObject dependencyObject)
        {
            return dependencyObject.GetVisualAncestorsAndSelf().OfType<DataPointSeries>().Where(series => Chart.Series.Contains(series)).FirstOrDefault();
        }
    }
}