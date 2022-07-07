// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    using ItemAndContainer = KeyValuePair<object, TreeViewItem>;

    /// <summary>
    /// Provides useful extensions to TreeView and TreeViewItem instances.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static class TreeViewExtensions
    {
        #region Get Parents
        /// <summary>
        /// Get the parent ItemsControl of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The parent ItemsControl of an element, or null if not found.
        /// </returns>
        internal static ItemsControl GetParentItemsControl(DependencyObject element)
        {
            Debug.Assert(element != null, "element should not be null!");

            // Try using GetItemsOwner first
            ItemsControl owner = ItemsControl.GetItemsOwner(element);
            if (owner != null)
            {
                return owner;
            }

            // Walk up the visual tree
            return element
                .GetVisualAncestors()
                .OfType<ItemsControl>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the ancestor ItemsControls of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The ancestor ItemsControls of an element.
        /// </returns>
        internal static IEnumerable<ItemsControl> GetAncestorItemsControls(DependencyObject element)
        {
            // Walk up the containing ItemsControls
            for (ItemsControl control = GetParentItemsControl(element);
                control != null;
                control = GetParentItemsControl(control))
            {
                yield return control;
            }
        }

        /// <summary>
        /// Get the parent TreeViewItem of a TreeViewItem.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// The parent TreeViewItem if found, otherwise null.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static TreeViewItem GetParentTreeViewItem(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            // Walk up the ancestral ItemsControls until we get to a
            // TreeViewItem
            return GetAncestorItemsControls(item)
                .OfType<TreeViewItem>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the parent TreeView of a TreeViewItem.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>The parent TreeView if found, otherwise null.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static TreeView GetParentTreeView(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            // Walk up the ancestral ItemsControls until we get to a TreeView
            return GetAncestorItemsControls(item)
                .OfType<TreeView>()
                .FirstOrDefault();
        }
        #endregion Get Parents

        #region Get Containers and Items
        /// <summary>
        /// Get the TreeViewItem containers of a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <returns>The TreeViewItem containers of a TreeView.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeView.")]
        public static IEnumerable<TreeViewItem> GetContainers(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            return ItemsControlExtensions.GetContainers<TreeViewItem>(view);
        }

        /// <summary>
        /// Get the child TreeViewItem containers of a TreeViewItem.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// The child TreeViewItem containers of a TreeViewItem.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static IEnumerable<TreeViewItem> GetContainers(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return ItemsControlExtensions.GetContainers<TreeViewItem>(item);
        }

        /// <summary>
        /// Get the items and TreeViewItem containers of a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <returns>
        /// The items and TreeViewItem containers of a TreeView.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeView.")]
        public static IEnumerable<ItemAndContainer> GetItemsAndContainers(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            return ItemsControlExtensions.GetItemsAndContainers<TreeViewItem>(view);
        }

        /// <summary>
        /// Get the items and TreeViewItem containers of a TreeViewItem.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// The items and TreeViewItem containers of a TreeViewItem.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static IEnumerable<ItemAndContainer> GetItemsAndContainers(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return ItemsControlExtensions.GetItemsAndContainers<TreeViewItem>(item);
        }
        #endregion Get Containers and Items

        #region Get Descendants and Siblings
        /// <summary>
        /// Get the TreeViewItem containers of a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <returns>The TreeViewItem containers of a TreeView.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeView.")]
        public static IEnumerable<TreeViewItem> GetDescendantContainers(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            return GetDescendantItemsAndContainersIterator(view).Select(p => p.Value);
        }

        /// <summary>
        /// Get the descendant TreeViewItem containers of a TreeViewItem.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// The descendant TreeViewItem containers of a TreeViewItem.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static IEnumerable<TreeViewItem> GetDescendantContainers(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return GetDescendantItemsAndContainersIterator(item).Select(p => p.Value);
        }

        /// <summary>
        /// Get the descendant items and TreeViewItem containers of a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <returns>
        /// The descendant items and TreeViewItem containers of a TreeView.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeView.")]
        public static IEnumerable<ItemAndContainer> GetDescendantItemsAndContainers(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            return GetDescendantItemsAndContainersIterator(view);
        }

        /// <summary>
        /// Get the descendant items and TreeViewItem containers of a
        /// TreeViewItem.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// The descendant items and TreeViewItem containers of a TreeViewItem.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<ItemAndContainer> GetDescendantItemsAndContainers(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return GetDescendantItemsAndContainersIterator(item);
        }

        /// <summary>
        /// Get the items and TreeViewItem containers of a TreeView or
        /// TreeViewItem.
        /// </summary>
        /// <param name="control">The TreeView or TreeViewItem.</param>
        /// <returns>
        /// The items and TreeViewItem containers of a TreeView or TreeViewItem.
        /// </returns>
        private static IEnumerable<ItemAndContainer> GetDescendantItemsAndContainersIterator(ItemsControl control)
        {
            Debug.Assert(control != null, "control should not be null!");

            // Recurse breadth first
            Queue<ItemAndContainer> remaining = new Queue<ItemAndContainer>(control.GetItemsAndContainers<TreeViewItem>());
            while (remaining.Count > 0)
            {
                ItemAndContainer current = remaining.Dequeue();
                yield return current;

                foreach (ItemAndContainer child in GetItemsAndContainers(current.Value))
                {
                    remaining.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// Get the sibling items and containers of the item.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>The sibling items and containers of the item.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<ItemAndContainer> GetSiblingItemsAndContainers(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            ItemsControl parent = GetParentItemsControl(item);
            if (parent == null)
            {
                return Enumerable.Empty<ItemAndContainer>();
            }

            return parent
                .GetItemsAndContainers<TreeViewItem>()
                .Where(p => p.Value != item);
        }
        #endregion Get Descendants and Siblings

        #region Get Container(s) from Item
        /// <summary>
        /// Get the TreeViewItems already created that are used to represent the
        /// given item.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <param name="item">The item being represented.</param>
        /// <returns>
        /// A sequence of TreeViewItems that represent the given item, or an
        /// empty sequence if none were found.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static IEnumerable<TreeViewItem> GetContainersFromItem(this TreeView view, object item)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            return view
                .GetDescendantItemsAndContainers()
                .Where(p => object.Equals(p.Key, item))
                .Select(p => p.Value);
        }

        /// <summary>
        /// Get the TreeViewItem already created that is used to represent the
        /// given item.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <param name="item">The item being represented.</param>
        /// <returns>
        /// The TreeViewItems that represents the given item, or null if no
        /// container was found.
        /// </returns>
        /// <remarks>
        /// If multiple TreeViewItems represent the same item, the first item
        /// found via a breadth-first search will be used.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static TreeViewItem GetContainerFromItem(this TreeView view, object item)
        {
            return GetContainersFromItem(view, item).FirstOrDefault();
        }
        #endregion Get Container(s) from Item

        #region Get Path
        /// <summary>
        /// Gets a path of items and TreeViewItem containers from the
        /// TreeViewItem to the root of the TreeView.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// A path of items and TreeViewItem containers from the TreeViewItem to
        /// the root of the TreeView.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<ItemAndContainer> GetPath(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return GetPathIterator(item).Reverse();
        }

        /// <summary>
        /// Gets a path of items and TreeViewItem containers from the
        /// TreeViewItem to the root of the TreeView.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// A path of items and TreeViewItem containers from the TreeViewItem to
        /// the root of the TreeView.
        /// </returns>
        private static IEnumerable<ItemAndContainer> GetPathIterator(TreeViewItem item)
        {
            Debug.Assert(item != null, "item should not be null!");

            TreeViewItem container = item;
            for (ItemsControl parent = GetParentItemsControl(container);
                parent != null && container != null;
                container = parent as TreeViewItem,
                parent = GetParentItemsControl(parent))
            {
                object value = parent.ItemContainerGenerator.ItemFromContainer(container);
                yield return new ItemAndContainer(value, container);
            }
        }
        #endregion Get Path

        #region TreeViewItem Attributes
        /// <summary>
        /// Get the item wrapped by this container.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// The item wrapped by the container, or null if not found.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The implementation is specific to TreeViewItem.")]
        public static object GetItem(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            ItemsControl parent = GetParentItemsControl(item);
            if (parent == null)
            {
                return null;
            }

            return parent.ItemContainerGenerator.ItemFromContainer(item);
        }

        /// <summary>
        /// Get the item of the parent container for a specified
        /// <paramref name="item" />.
        /// </summary>
        /// <param name="view">
        /// The TreeView containing the <paramref name="item" />.
        /// </param>
        /// <param name="item">The child item.</param>
        /// <returns>
        /// The item of the parent container for the specified
        /// <paramref name="item" />, or null if not found.
        /// </returns>
        public static object GetParentItem(this TreeView view, object item)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            TreeViewItem container = view.GetContainerFromItem(item);
            if (container == null)
            {
                return null;
            }

            TreeViewItem parent = container.GetParentTreeViewItem();
            if (parent == null)
            {
                return null;
            }

            return parent.GetItem();
        }

        /// <summary>
        /// Gets a value indicating whether the TreeViewItem is a root of its
        /// TreeView.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// A value indicating whether the TreeViewItem is a root of its
        /// TreeView.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static bool GetIsRoot(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return GetParentItemsControl(item) is TreeView;
        }

        /// <summary>
        /// Gets a value indicating whether the TreeViewItem is a leaf in its
        /// TreeView.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// A value indicating whether the TreeViewItem is a leaf in its
        /// TreeView.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        public static bool GetIsLeaf(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return !item.HasItems;
        }

        /// <summary>
        /// Gets the depth of a TreeViewItem in its TreeView (using a zero-based
        /// index).
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <returns>
        /// The depth of a TreeViewItem in its TreeView (using a zero-based
        /// index).
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="item" /> is not in a TreeView.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static int GetDepth(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            int depth = 0;

            // Walk up the containing ItemsControls until we get to a TreeView
            for (ItemsControl control = GetParentItemsControl(item);
                control != null;
                control = GetParentItemsControl(control), depth++)
            {
                if (control is TreeView)
                {
                    return depth;
                }
            }

            throw new ArgumentException("The item is not in a TreeView.", "item");
        }
        #endregion TreeViewItem Attributes

        #region Selection
        /// <summary>
        /// Get the selected TreeViewItem in a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <returns>
        /// The selected TreeViewItem, or null if no selected item found.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static TreeViewItem GetSelectedContainer(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            return view
                .GetDescendantContainers()
                .Where(t => t.IsSelected)
                .FirstOrDefault();
        }

        /// <summary>
        /// Sets the selected TreeViewItem of a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <param name="item">The TreeViewItem to select.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static void SetSelectedContainer(this TreeView view, TreeViewItem item)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (item != null)
            {
                item.IsSelected = true;
            }
            else
            {
                view.ClearSelection();
            }
        }

        /// <summary>
        /// Clear the selection of the TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static void ClearSelection(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            TreeViewItem item = view.GetSelectedContainer();
            if (item != null)
            {
                item.IsSelected = false;
            }
        }

        /// <summary>
        /// Select an item in the TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <param name="item">The item to select.</param>
        /// <returns>
        /// A value indicating whether the item was successfully set as the
        /// TreeView's SelectedItem.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static bool SelectItem(this TreeView view, object item)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            TreeViewItem container = view.GetContainerFromItem(item);
            bool found = container != null;
            if (found)
            {
                container.IsSelected = true;
            }

            return found;
        }

        /// <summary>
        /// Gets the path to the TreeView's selected item.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <returns>The path to the TreeView's selected item.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<ItemAndContainer> GetSelectedPath(this TreeView view)
        {
            TreeViewItem item = view.GetSelectedContainer();
            return item != null ?
                item.GetPath() :
                Enumerable.Empty<ItemAndContainer>();
        }
        #endregion Selection

        #region Expand and Collapse
        /// <summary>
        /// Recursively expand or collapse the TreeViewItem and all of its
        /// descendants.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <param name="expand">
        /// A value indicating whether to expand or collapse.
        /// </param>
        /// <param name="depth">
        /// The number of levels that have already been collapsed or expanded.
        /// This is used in conjunction with the optional maximumDepth to only
        /// expand a specified number of layers.
        /// </param>
        /// <param name="maximumDepth">
        /// An optional depth that defines the number of layers to expand or
        /// collapse.
        /// </param>
        private static void ExpandOrCollapseAll(TreeViewItem item, bool expand, int depth, int? maximumDepth)
        {
            Debug.Assert(item != null, "item should not be null!");
            Debug.Assert(depth >= 0, "depth should not be negative!");
            Debug.Assert(maximumDepth == null || maximumDepth > 0, "maximumDepth should be null or positive!");

            // Stop recursing if we've surpassed our maximum depth
            if (maximumDepth != null && depth >= maximumDepth.Value)
            {
                return;
            }

            // Expand items before recursing into children
            if (expand)
            {
                bool justExpanded = !item.IsExpanded;
                item.IsExpanded = true;

                // If the item was just expanded, we'll need to wait for the
                // visual tree to update before its children to be created
                if (justExpanded && item.HasItems)
                {
                    // Invoke ourselves recursively using the Dispatcher queue
                    int capturedDepth = depth;
                    item.InvokeOnLayoutUpdated(() => ExpandOrCollapseAll(item, expand, capturedDepth, maximumDepth));
                    return;
                }
            }

            // Recursive expand or collapse child items
            foreach (TreeViewItem child in item.GetContainers())
            {
                ExpandOrCollapseAll(child, expand, depth + 1, maximumDepth);
            }

            // Collapse items after recursing through children
            if (!expand)
            {
                item.IsExpanded = false;
            }
        }

        /// <summary>
        /// Expand or collapse all of the descendants of the TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <param name="expand">
        /// A value indicating whether to expand or collapse.
        /// </param>
        /// <param name="maximumDepth">
        /// An optional depth that defines the number of layers to expand or
        /// collapse.
        /// </param>
        private static void ExpandOrCollapseAll(TreeView view, bool expand, int? maximumDepth)
        {
            Debug.Assert(view != null, "view should not be null!");
            Debug.Assert(maximumDepth == null || maximumDepth > 0, "maximumDepth should be null or positive!");

            foreach (TreeViewItem item in view.GetContainers())
            {
                ExpandOrCollapseAll(item, expand, 0, maximumDepth);
            }
        }

        /// <summary>
        /// Expand all of the items in a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static void ExpandAll(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            ExpandOrCollapseAll(view, true, null);
        }

        /// <summary>
        /// Collapse all of the items in a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static void CollapseAll(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            ExpandOrCollapseAll(view, false, null);
        }

        /// <summary>
        /// Expand a specified number of layers in a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <param name="depth">The number of layers to expand.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static void ExpandToDepth(this TreeView view, int depth)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (depth > 0)
            {
                ExpandOrCollapseAll(view, true, depth);
            }
        }

        /// <summary>
        /// Expand a path from the given item to the root of it's TreeView.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <param name="collapseSiblings">
        /// A value indicating whether to collapse siblings while expanding the
        /// path.  This will result in only the path from the item to the root
        /// being expanded.
        /// </param>
        private static void ExpandPathToRoot(TreeViewItem item, bool collapseSiblings)
        {
            Debug.Assert(item != null, "item should not be null!");

            // Walk up the TreeView keeping references to both the item and its
            // parent as we go
            TreeViewItem container = item;
            for (ItemsControl parent = GetParentItemsControl(container);
                parent != null && container != null;
                container = parent as TreeViewItem,
                parent = GetParentItemsControl(parent))
            {
                // Collapse all of the siblings of the container if requested
                if (collapseSiblings)
                {
                    foreach (TreeViewItem sibling in parent.GetContainers())
                    {
                        if (sibling != container)
                        {
                            ExpandOrCollapseAll(sibling, false, 0, null);
                            sibling.IsExpanded = false;
                        }
                    }
                }

                // Expand all but the first item (which stays as we found it)
                if (container != item)
                {
                    container.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// Expand a path from the TreeViewItem to the root of the TreeView.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="item" /> is null.
        /// </exception>
        public static void ExpandPath(this TreeViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            ExpandPathToRoot(item, false);
        }

        /// <summary>
        /// Expand the path from the SelectedItem to the root of the TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static void ExpandSelectedPath(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            TreeViewItem selected = view.GetSelectedContainer();
            if (selected != null)
            {
                ExpandPathToRoot(selected, false);
            }
        }

        /// <summary>
        /// Collapse all TreeViewItems except those along the path from the
        /// TreeView's SelectedItem to the root of the TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        public static void CollapseAllButSelectedPath(this TreeView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            TreeViewItem selected = view.GetSelectedContainer();
            if (selected == null)
            {
                view.CollapseAll();
            }
            else
            {
                ExpandPathToRoot(selected, true);
            }
        }

        /// <summary>
        /// Expand the given path of items starting from the TreeView's root.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <param name="items">
        /// The sequence of items corresponding to the path to expand.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="items" /> is null.
        /// </exception>
        public static void ExpandPath(this TreeView view, params object[] items)
        {
            ExpandPath(view, null, items);
        }

        /// <summary>
        /// Expand the given path of items starting from the TreeView's root.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items provided in <paramref name="items" />.
        /// </typeparam>
        /// <param name="view">The TreeView.</param>
        /// <param name="items">
        /// The sequence of items corresponding to the path to expand.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="items" /> is null.
        /// </exception>
        public static void ExpandPath<T>(this TreeView view, IEnumerable<T> items)
        {
            ExpandPath<T>(view, null, items);
        }

        /// <summary>
        /// Expand the given path of items starting from the TreeView's root.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items provided in <paramref name="items" />.
        /// </typeparam>
        /// <param name="view">The TreeView.</param>
        /// <param name="comparisonSelector">
        /// A function that takes a TreeViewItem's item and returns a value to
        /// compare against elements of the given <paramref name="items" />.
        /// The item itself will be used if 
        /// <paramref name="comparisonSelector" /> is null.
        /// </param>
        /// <param name="items">
        /// The sequence of items corresponding to the path to expand.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="items" /> is null.
        /// </exception>
        public static void ExpandPath<T>(this TreeView view, Func<object, T> comparisonSelector, params T[] items)
        {
            ExpandPath<T>(view, comparisonSelector, (IEnumerable<T>)items);
        }

        /// <summary>
        /// Expand the given path of items starting from the TreeView's root.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items provided in <paramref name="items" />.
        /// </typeparam>
        /// <param name="view">The TreeView.</param>
        /// <param name="comparisonSelector">
        /// A function that takes a TreeViewItem's item and returns a value to
        /// compare against elements of the given <paramref name="items" />.
        /// The item itself will be used if 
        /// <paramref name="comparisonSelector" /> is null.
        /// </param>
        /// <param name="items">
        /// The sequence of items corresponding to the path to expand.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="items" /> is null.
        /// </exception>
        public static void ExpandPath<T>(this TreeView view, Func<object, T> comparisonSelector, IEnumerable<T> items)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            else if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            // Use the item itself if no comparisonSelector is provided
            if (comparisonSelector == null)
            {
                comparisonSelector = o => (T)o;
            }

            ItemsControl parent = view;
            IEnumerator<T> enumerator = items.GetEnumerator();

            // The resume action is used to recurse using the Dispatcher queue
            // and the closure that captures the index and parent.
            Action resume = null;
            resume = () =>
            {
                while (enumerator.MoveNext())
                {
                    // Look up the container for the current item
                    T item = enumerator.Current;
                    TreeViewItem container =
                        parent
                        .GetItemsAndContainers<TreeViewItem>()
                        .Where(p => object.Equals(item, comparisonSelector(p.Key)))
                        .Select(p => p.Value)
                        .FirstOrDefault();

                    if (container != null)
                    {
                        // Move down to the next level
                        parent = container;

                        // Expand the item and wait so its children can enter
                        // the visual tree
                        if (!container.IsExpanded)
                        {
                            if (container.HasItems)
                            {
                                container.InvokeOnLayoutUpdated(resume);
                            }
                            container.IsExpanded = true;
                            break;
                        }
                    }
                }
            };

            resume();
        }
        #endregion Expand and Collapse

        #region IsChecked
        #region public attached bool? IsChecked
        /// <summary>
        /// Gets the value of the IsChecked attached property for a specified
        /// TreeViewItem.
        /// </summary>
        /// <param name="element">
        /// The TreeViewItem from which the property value is read.
        /// </param>
        /// <returns>
        /// The IsChecked property value for the TreeViewItem.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static bool? GetIsChecked(this TreeViewItem element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(IsCheckedProperty) as bool?;
        }

        /// <summary>
        /// Sets the value of the IsChecked attached property to a specified
        /// TreeViewItem.
        /// </summary>
        /// <param name="element">
        /// The TreeViewItem to which the attached property is written.
        /// </param>
        /// <param name="value">The needed IsChecked value.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Implementation is specific to TreeViewItem.")]
        public static void SetIsChecked(this TreeViewItem element, bool? value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// Identifies the IsChecked dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached(
                "IsChecked",
                typeof(bool?),
                typeof(TreeViewExtensions),
                new PropertyMetadata(false, OnIsCheckedPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether recursive calls to
        /// OnIsCheckedPropertyChanged should ignore their notifications or
        /// process them accordingly.
        /// </summary>
        private static bool CancelIsCheckedChangedBubbling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether recursive calls to
        /// OnIsCheckedPropertyChanged should update their children when their
        /// IsChecked value has changed.
        /// </summary>
        private static bool CancelIsCheckedChangedChildNotifications { get; set; }

        /// <summary>
        /// IsCheckedProperty property changed handler.
        /// </summary>
        /// <param name="d">The TreeViewItem that changed IsChecked.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (CancelIsCheckedChangedBubbling)
            {
                return;
            }

            bool? newValue = e.NewValue as bool?;

            TreeViewItem item = d as TreeViewItem;
            if (item != null)
            {
                // Update the CheckBox to match
                CheckBox checkBox = GetAssociatedCheckBox(item);
                if (checkBox != null)
                {
                    checkBox.IsChecked = newValue;
                }

                // Force all children to match
                if (!CancelIsCheckedChangedChildNotifications)
                {
                    try
                    {
                        CancelIsCheckedChangedBubbling = true;

                        foreach (TreeViewItem child in item.GetDescendantContainers())
                        {
                            SetIsChecked(child, newValue);
                            TreeViewItemCheckBox childCheckBox = GetAssociatedCheckBox(child);

                            // Note that indeterminate values don't propogate down
                            if (childCheckBox != null && (childCheckBox.IsChecked != true || newValue != null))
                            {
                                childCheckBox.IsChecked = newValue;
                            }
                        }
                    }
                    finally
                    {
                        CancelIsCheckedChangedBubbling = false;
                    }
                }

                // Update our immediate parent TreeViewItem to match (which will
                // cause a cascade of changes up the tree)
                TreeViewItem parent = item.GetParentTreeViewItem();
                if (parent != null)
                {
                    // Check to see whether all, some, or none of the parent's child
                    // TreeViewItems are still selected.
                    bool all = true;
                    bool any = false;
                    foreach (TreeViewItem sibling in parent.GetContainers())
                    {
                        // Partial checks count for any
                        bool? isChecked = GetIsChecked(sibling);
                        all &= isChecked == true;
                        any |= isChecked != false;

                        // Break out early if we can definitively ascertain a
                        // partial check
                        if (!all && any)
                        {
                            break;
                        }
                    }

                    // Set the parent TreeViewItem's IsChecked property (which will
                    // recurse up the tree via these change notifications)
                    bool? value = false;
                    if (all)
                    {
                        value = true;
                    }
                    else if (any)
                    {
                        value = null;
                    }

                    // Update the parent and its associated CheckBox
                    try
                    {
                        CancelIsCheckedChangedChildNotifications = true;

                        parent.SetIsChecked(value);
                        TreeViewItemCheckBox parentCheckBox = GetAssociatedCheckBox(parent);
                        if (parentCheckBox != null)
                        {
                            parentCheckBox.IsChecked = value;
                        }
                    }
                    finally
                    {
                        CancelIsCheckedChangedChildNotifications = false;
                    }
                }
            }
        }
        #endregion public attached bool? IsChecked

        #region internal attached TreeViewItemCheckBox AssociatedCheckBox
        /// <summary>
        /// Gets the value of the AssociatedCheckBox attached property for a
        /// specified TreeViewItem.
        /// </summary>
        /// <param name="element">
        /// The TreeViewItem from which the property value is read.
        /// </param>
        /// <returns>
        /// The AssociatedCheckBox property value for the TreeViewItem.
        /// </returns>
        internal static TreeViewItemCheckBox GetAssociatedCheckBox(TreeViewItem element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(AssociatedCheckBoxProperty) as TreeViewItemCheckBox;
        }

        /// <summary>
        /// Sets the value of the AssociatedCheckBox attached property to a
        /// specified TreeViewItem.
        /// </summary>
        /// <param name="element">
        /// The TreeViewItem to which the attached property is written.
        /// </param>
        /// <param name="value">The needed AssociatedCheckBox value.</param>
        internal static void SetAssociatedCheckBox(TreeViewItem element, TreeViewItemCheckBox value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(AssociatedCheckBoxProperty, value);
        }

        /// <summary>
        /// Identifies the AssociatedCheckBox dependency property.
        /// </summary>
        internal static readonly DependencyProperty AssociatedCheckBoxProperty =
            DependencyProperty.RegisterAttached(
                "AssociatedCheckBox",
                typeof(TreeViewItemCheckBox),
                typeof(TreeViewExtensions),
                new PropertyMetadata(null, OnAssociatedCheckBoxPropertyChanged));

        /// <summary>
        /// AssociatedCheckBoxProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// The TreeViewItem that changed its AssociatedCheckBox.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnAssociatedCheckBoxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem source = d as TreeViewItem;
            TreeViewItemCheckBox value = e.NewValue as TreeViewItemCheckBox;

            // Initialize the CheckBox with the current TreeViewItem.IsChecked
            // property (also checking its parent in case this TreeViewItem was
            // just added to the visual tree)
            if (value != null)
            {
                bool? isChecked = source.GetIsChecked();

                // Setting a parent's IsChecked to True also checks all of its
                // descendants.  We need to make sure that any added items
                // reflect this behavior.
                TreeViewItem parent = source.GetParentTreeViewItem();
                if (parent != null && parent.GetIsChecked() == true)
                {
                    isChecked = true;
                }

                try
                {
                    CancelIsCheckedChangedBubbling = true;
                    value.IsChecked = isChecked;
                }
                finally
                {
                    CancelIsCheckedChangedBubbling = false;
                }
            }
        }
        #endregion internal attached TreeViewItemCheckBox AssociatedCheckBox

        /// <summary>
        /// Get the sequence of items and containers with their IsChecked
        /// property set to True.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <returns>
        /// The sequence of items and containers with their IsChecked property
        /// set to True.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view"/> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<ItemAndContainer> GetCheckedItemsAndContainers(this TreeView view)
        {
            return GetCheckedItemsAndContainers(view, false);
        }

        /// <summary>
        /// Get the sequence of items and containers with their IsChecked
        /// property set to True.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        /// <param name="includeIndeterminate">
        /// A value indicating whether to include TreeViewItems with an
        /// indeterminate IsChecked value.
        /// </param>
        /// <returns>
        /// The sequence of items and containers with their IsChecked property
        /// set to True or also set to null if indeterminate values are
        /// included.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="view"/> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<ItemAndContainer> GetCheckedItemsAndContainers(this TreeView view, bool includeIndeterminate)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            // Change the filter based on whether we're including indeterminates
            Func<ItemAndContainer, bool> predicate = null;
            if (includeIndeterminate)
            {
                predicate = p => p.Value.GetIsChecked() != false;
            }
            else
            {
                predicate = p => p.Value.GetIsChecked() == true;
            }

            return view
                .GetDescendantItemsAndContainers()
                .Where(predicate);
        }
        #endregion IsChecked
    }
}