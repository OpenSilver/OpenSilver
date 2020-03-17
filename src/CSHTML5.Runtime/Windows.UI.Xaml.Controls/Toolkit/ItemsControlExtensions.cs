

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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides useful extensions to ItemsControl instances.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static partial class ItemsControlExtensions
    {
        ///// <summary>
        ///// Gets the Panel that contains the containers of an ItemsControl.
        ///// </summary>
        ///// <param name="control">The ItemsControl.</param>
        ///// <returns>
        ///// The Panel that contains the containers of an ItemsControl, or null
        ///// if the Panel could not be found.
        ///// </returns>
        ///// <exception cref="T:System.ArgumentNullException">
        ///// <paramref name="control" /> is null.
        ///// </exception>
        //public static Panel GetItemsHost(this ItemsControl control)
        //{
        //    if (control == null)
        //    {
        //        throw new ArgumentNullException("control");
        //    }

        //    // Get the first live container
        //    DependencyObject container = control.ItemContainerGenerator.ContainerFromIndex(0);

        //    if (container != null)
        //    {
        //        return VisualTreeHelper.GetParent(container) as Panel;
        //    }

        //    FrameworkElement rootVisual = control.GetVisualChildren().FirstOrDefault() as FrameworkElement;
        //    if (rootVisual != null)
        //    {
        //        ItemsPresenter presenter = rootVisual.GetLogicalDescendents().OfType<ItemsPresenter>().FirstOrDefault();
        //        if (presenter != null && VisualTreeHelper.GetChildrenCount(presenter) > 0)
        //        {
        //            return VisualTreeHelper.GetChild(presenter, 0) as Panel;
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Gets the ScrollViewer that contains the containers of an
        ///// ItemsControl.
        ///// </summary>
        ///// <param name="control">The ItemsControl.</param>
        ///// <returns>
        ///// The ScrollViewer that contains the containers of an ItemsControl, or
        ///// null if a ScrollViewer could not be found.
        ///// </returns>
        ///// <exception cref="T:System.ArgumentNullException">
        ///// <paramref name="control" /> is null.
        ///// </exception>
        //public static ScrollViewer GetScrollHost(this ItemsControl control)
        //{
        //    if (control == null)
        //    {
        //        throw new ArgumentNullException("control");
        //    }

        //    Panel itemsHost = GetItemsHost(control);
        //    if (itemsHost == null)
        //    {
        //        return null;
        //    }

        //    // Walk up the visual tree from the ItemsHost to the
        //    // ItemsControl looking for a ScrollViewer that wraps
        //    // the ItemsHost.
        //    return itemsHost
        //        .GetVisualAncestors()
        //        .Where(c => c != control)
        //        .OfType<ScrollViewer>()
        //        .FirstOrDefault();
        //}

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
                throw new ArgumentNullException("control");
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
        //[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Common pattern for extensions that cast.")]
        public static IEnumerable<TContainer> GetContainers<TContainer>(this ItemsControl control)
            where TContainer : DependencyObject
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
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
            //Debug.Assert(control != null, "control should not be null!");
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
        //[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<KeyValuePair<object, DependencyObject>> GetItemsAndContainers(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
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
        //[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Returns a generic type.")]
        //[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<KeyValuePair<object, TContainer>> GetItemsAndContainers<TContainer>(this ItemsControl control)
            where TContainer : DependencyObject
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
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
            //Debug.Assert(control != null, "control should not be null!");

            int count = control.Items.Count;
            for (int i = 0; i < count; i++)
            {
                DependencyObject container = (DependencyObject)control.ItemContainerGenerator.ContainerFromIndex(i);
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
                return that.ItemsSource.CanInsert(item);
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
                return !that.ItemsSource.IsReadOnly() && that.ItemsSource is INotifyCollectionChanged;
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
                that.ItemsSource.Insert(index, item);
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
                that.ItemsSource.Add(item);
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
                that.ItemsSource.Remove(item);
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
                that.ItemsSource.RemoveAt(index);
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
                return that.ItemsSource.Count();
            }
        }
    }
}