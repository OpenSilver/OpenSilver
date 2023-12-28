// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides useful extensions to ItemsControl instances.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static class ItemsControlExtensions
    {
        /// <summary>
        /// Gets the Panel that contains the containers of an ItemsControl.
        /// </summary>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>
        /// The Panel that contains the containers of an ItemsControl, or null
        /// if the Panel could not be found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static Panel GetItemsHost(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            // Get the first live container
            DependencyObject container = control.ItemContainerGenerator.ContainerFromIndex(0);

            if (container != null)
            {
                return VisualTreeHelper.GetParent(container) as Panel;
            }

            FrameworkElement rootVisual = control.GetVisualChildren().FirstOrDefault() as FrameworkElement;
            if (rootVisual != null)
            {
                ItemsPresenter presenter = rootVisual.GetLogicalDescendents().OfType<ItemsPresenter>().FirstOrDefault();
                if (presenter != null && VisualTreeHelper.GetChildrenCount(presenter) > 0)
                {
                    return VisualTreeHelper.GetChild(presenter, 0) as Panel;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the ScrollViewer that contains the containers of an
        /// ItemsControl.
        /// </summary>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>
        /// The ScrollViewer that contains the containers of an ItemsControl, or
        /// null if a ScrollViewer could not be found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static ScrollViewer GetScrollHost(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Panel itemsHost = GetItemsHost(control);
            if (itemsHost == null)
            {
                return null;
            }

            // Walk up the visual tree from the ItemsHost to the
            // ItemsControl looking for a ScrollViewer that wraps
            // the ItemsHost.
            return itemsHost
                .GetVisualAncestors()
                .Where(c => c != control)
                .OfType<ScrollViewer>()
                .FirstOrDefault();
        }

        #region GetContainers
        /// <summary>
        /// Get the item containers of an ItemsControl.
        /// </summary>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetContainers(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            return GetContainersIterator<DependencyObject>(control);
        }

        /// <summary>
        /// Get the item containers of an ItemsControl.
        /// </summary>
        /// <typeparam name="TContainer">
        /// The type of the item containers.
        /// </typeparam>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static IEnumerable<TContainer> GetContainers<TContainer>(this ItemsControl control)
            where TContainer : DependencyObject
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            return GetContainersIterator<TContainer>(control);
        }

        /// <summary>
        /// Get the item containers of an ItemsControl.
        /// </summary>
        /// <typeparam name="TContainer">
        /// The type of the item containers.
        /// </typeparam>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The item containers of an ItemsControl.</returns>
        private static IEnumerable<TContainer> GetContainersIterator<TContainer>(ItemsControl control)
            where TContainer : DependencyObject
        {
            Debug.Assert(control != null, "control should not be null!");
            return control.GetItemsAndContainers<TContainer>().Select(p => p.Value);
        }
        #endregion GetContainers

        #region GetItemsAndContainers
        /// <summary>
        /// Get the items and item containers of an ItemsControl.
        /// </summary>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The items and item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static IEnumerable<KeyValuePair<object, DependencyObject>> GetItemsAndContainers(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            return GetItemsAndContainersIterator<DependencyObject>(control);
        }

        /// <summary>
        /// Get the items and item containers of an ItemsControl.
        /// </summary>
        /// <typeparam name="TContainer">
        /// The type of the item containers.
        /// </typeparam>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The items and item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static IEnumerable<KeyValuePair<object, TContainer>> GetItemsAndContainers<TContainer>(this ItemsControl control)
            where TContainer : DependencyObject
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            return GetItemsAndContainersIterator<TContainer>(control);
        }

        /// <summary>
        /// Get the items and item containers of an ItemsControl.
        /// </summary>
        /// <typeparam name="TContainer">
        /// The type of the item containers.
        /// </typeparam>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The items and item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        private static IEnumerable<KeyValuePair<object, TContainer>> GetItemsAndContainersIterator<TContainer>(ItemsControl control)
            where TContainer : DependencyObject
        {
            Debug.Assert(control != null, "control should not be null!");

            int count = control.Items.Count;
            for (int i = 0; i < count; i++)
            {
                DependencyObject container = control.ItemContainerGenerator.ContainerFromIndex(i);
                if (container == null)
                {
                    continue;
                }

                yield return new KeyValuePair<object, TContainer>(
                    control.Items[i],
                    container as TContainer);
            }
        }
        #endregion GetItemsAndContainers

        /// <summary>
        /// Returns a value indicating whether an item can be added to an
        /// ItemsControl.
        /// </summary>
        /// <param name="that">The ItemsControl instance.</param>
        /// <param name="item">The item to be added.</param>
        /// <returns>A value Indicating whether an item can be added to an
        /// ItemsControl.</returns>
        internal static bool CanAddItem(this ItemsControl that, object item)
        {
            if (that.ItemsSource == null)
            {
                return true;
            }
            else
            {
                return CollectionHelper.CanInsert(that.ItemsSource, item);
            }
        }

        /// <summary>
        /// Returns a value indicating whether an item can be removed from the
        /// ItemsControl.
        /// </summary>
        /// <param name="that">The items control.</param>
        /// <returns>A value indicating whether an item can be removed from the
        /// ItemsControl.</returns>
        internal static bool CanRemoveItem(this ItemsControl that)
        {
            if (that.ItemsSource != null)
            {
                return !CollectionHelper.IsReadOnly(that.ItemsSource) && that.ItemsSource is INotifyCollectionChanged;
            }
            return true;
        }

        /// <summary>
        /// Inserts an item into an ItemsControl.
        /// </summary>
        /// <param name="that">The ItemsControl instance.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to be inserted.</param>
        internal static void InsertItem(this ItemsControl that, int index, object item)
        {
            if (that.ItemsSource == null)
            {
                that.Items.Insert(index, item);
            }
            else
            {
                CollectionHelper.Insert(that.ItemsSource, index, item);
            }
        }

        /// <summary>
        /// Adds an item to an ItemsControl.
        /// </summary>
        /// <param name="that">The ItemsControl instance.</param>
        /// <param name="item">The item to be inserted.</param>
        internal static void AddItem(this ItemsControl that, object item)
        {
            if (that.ItemsSource == null)
            {
                that.InsertItem(that.Items.Count, item);
            }
            else
            {
                CollectionHelper.Add(that.ItemsSource, item);
            }
        }

        /// <summary>
        /// Removes an item from an ItemsControl.
        /// </summary>
        /// <param name="that">The ItemsControl instance.</param>
        /// <param name="item">The item to be removed.</param>
        internal static void RemoveItem(this ItemsControl that, object item)
        {
            if (that.ItemsSource == null)
            {
                that.Items.Remove(item);
            }
            else
            {
                CollectionHelper.Remove(that.ItemsSource, item);
            }
        }

        /// <summary>
        /// Removes an item from an ItemsControl.
        /// </summary>
        /// <param name="that">The ItemsControl instance.</param>
        /// <param name="index">The index of the item to be removed.</param>
        internal static void RemoveItemAtIndex(this ItemsControl that, int index)
        {
            if (that.ItemsSource == null)
            {
                that.Items.RemoveAt(index);
            }
            else
            {
                CollectionHelper.RemoveAt(that.ItemsSource, index);
            }
        }

        /// <summary>
        /// Gets the number of items in an ItemsControl.
        /// </summary>
        /// <param name="that">The ItemsControl instance.</param>
        /// <returns>The number of items in the ItemsControl.</returns>
        internal static int GetItemCount(this ItemsControl that)
        {
            if (that.ItemsSource == null)
            {
                return that.Items.Count;
            }
            else
            {
                return CollectionHelper.Count(that.ItemsSource);
            }
        }
    }
}