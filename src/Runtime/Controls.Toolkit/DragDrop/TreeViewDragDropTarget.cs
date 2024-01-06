// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using SW = Microsoft.Windows;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// A drag drop target for the TreeView control.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public class TreeViewDragDropTarget : ItemsControlDragDropTarget<ItemsControl, TreeViewItem>
    {
        private readonly DragDropHelper _dragHelper;

        #region public int ExpandNodeDelay
        /// <summary>
        /// Gets or sets the delay before expanding a node that is being 
        /// hovered over during a drag operation.
        /// </summary>
        public int ExpandNodeDelay
        {
            get { return (int)GetValue(ExpandNodeDelayProperty); }
            set { SetValue(ExpandNodeDelayProperty, value); }
        }

        /// <summary>
        /// Throws an exception if the content is not a TreeView.
        /// </summary>
        /// <param name="oldContent">The old content value.</param>
        /// <param name="newContent">The new content value.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent != null && !(newContent is TreeView))
            {
                throw new ArgumentException(Resource.TreeViewDragDropTarget_set_Content_ContentMustBeATreeView);
            }

            base.OnContentChanged(oldContent, newContent);
        }

        /// <summary>
        /// Identifies the ExpandNodeDelay dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandNodeDelayProperty =
            DependencyProperty.Register(
                nameof(ExpandNodeDelay),
                typeof(int),
                typeof(TreeViewDragDropTarget),
                new PropertyMetadata(1500));

        #endregion public int ExpandNodeDelay

        /// <summary>
        /// Initializes a new instance of the TreeViewDragDropTarget class.
        /// </summary>
        public TreeViewDragDropTarget()
        {
            this.DefaultStyleKey = typeof(TreeViewDragDropTarget);

            _dragHelper = new DragDropHelper(this);
        }

        /// <summary>
        /// Retrieves the tree view item hovered over in a drag event.
        /// </summary>
        /// <param name="args">Information about a drag event.</param>
        /// <returns>The tree view item hovered over in a drag event.</returns>
        private TreeViewItem GetTreeViewItem(SW.DragEventArgs args)
        {
            DependencyObject originalSource = (DependencyObject)args.OriginalSource;
            ItemsControl itemsControl = GetDropTarget(args);
            if (itemsControl == null)
            {
                return null;
            }

            TreeViewItem item = GetItemContainerAncestor(itemsControl, originalSource);

            return item;
        }

        /// <summary>
        /// Returns the items control ancestor of a dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object to retrieve the
        /// element for.</param>
        /// <returns>The items control ancestor of the dependency object.
        /// </returns>
        protected override ItemsControl GetItemsControlAncestor(DependencyObject dependencyObject)
        {
            TreeViewItem item = dependencyObject as TreeViewItem;
            if (item == null)
            {
                // if element is within TreeViewItem, jump up to TreeViewItem that contains it.
                item = dependencyObject.GetVisualAncestors().OfType<TreeViewItem>().FirstOrDefault();

                // if not inside of a TreeViewItem it must be inside of a TreeView
                if (item == null)
                {
                    return dependencyObject.GetVisualAncestors().OfType<TreeView>().FirstOrDefault();
                }
            }

            // grab the TreeViewItem the element is inside of
            TreeViewItem ancestor = item.GetVisualAncestors().OfType<TreeViewItem>().FirstOrDefault();
            if (ancestor != null)
            {
                return ancestor;
            }
            return item.GetVisualAncestors().OfType<TreeView>().FirstOrDefault();
        }

        /// <summary>
        /// Prevents Move, Copy, or Link actions if an item is dragged into its
        /// descendent.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        protected override void OnDragEnter(SW.DragEventArgs args)
        {
            base.OnDragEnter(args);

            _dragHelper.OnDragEnter(args);
        }

        /// <summary>
        /// Prevents Move, Copy, or Link actions if an item is dragged into its
        /// descendent.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        protected override void OnDragLeave(SW.DragEventArgs args)
        {
            base.OnDragLeave(args);

            _dragHelper.OnDragLeave(args);
        }

        /// <summary>
        /// This method is called whenever a target event is raised.
        /// </summary>
        /// <param name="args">Information about the drag target event.</param>
        protected override void OnDragEvent(Microsoft.Windows.DragEventArgs args)
        {
            SetEffects(args);
            base.OnDragEvent(args);
        }

        /// <summary>
        /// Prevents Move, Copy, or Link actions if an item is dragged into its
        /// descendent.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        protected override void OnDrop(SW.DragEventArgs args)
        {
            SetEffects(args);
            if (!args.Handled)
            {
                base.OnDrop(args);
            }

            _dragHelper.OnDrop(args);
        }

        /// <summary>
        /// Prevents Move, Copy, or Link actions if an item is dragged into its
        /// descendent.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        private void SetEffects(SW.DragEventArgs args)
        {
            ItemsControl itemsControl = GetDropTarget(args);
            // If a tree view is dragged into a descendent or would be dropped 
            // into itself, set effects to None.
            if (itemsControl == null || IsTreeViewItemDraggedInDescendent(args) || IsTreeViewItemDraggedDirectlyAboveOrBelowSelf(args))
            {
                SW.DragDropEffects effects = args.AllowedEffects & (~(SW.DragDropEffects.Copy | SW.DragDropEffects.Link | SW.DragDropEffects.Move));
                args.Effects = effects;
                if (args.Effects != args.AllowedEffects)
                {
                    args.Handled = true;
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether a TreeViewItem being dragged 
        /// directly above or below itself.  If there are no indices in the
        /// selection the answer is result is always true.
        /// </summary>
        /// <param name="args">Information about a drag event.</param>
        /// <returns>A value indicating whether the TreeViewItem being dragged 
        /// directly above or below itself.</returns>
        protected bool IsTreeViewItemDraggedDirectlyAboveOrBelowSelf(SW.DragEventArgs args)
        {
            ItemsControl dropTarget = GetDropTarget(args);
            ItemDragEventArgs itemDragEventArgs = args.Data.GetData() as ItemDragEventArgs;
            if (itemDragEventArgs != null && itemDragEventArgs.DragSource != null && itemDragEventArgs.DragSource == dropTarget)
            {
                SelectionCollection selectionCollection = GetSelectionCollection(itemDragEventArgs.Data);
                int index = GetDropTargetInsertionIndex(dropTarget, args);
                return selectionCollection.Any(selection => !selection.Index.HasValue || selection.Index.Value == index);
            }
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether an item is being dragged into its 
        /// own descendent.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        /// <returns>A value indicating whether an item is being dragged into 
        /// its own descendent.  If there is no index information the answer
        /// is always assumed to be true.</returns>
        protected bool IsTreeViewItemDraggedInDescendent(SW.DragEventArgs args)
        {
            ItemDragEventArgs itemDragEventArgs = args.Data.GetData() as ItemDragEventArgs;
            if (itemDragEventArgs != null)
            {
                ItemsControl itemsControl = GetDropTarget(args);
                if (itemsControl == null)
                {
                    return false;
                }

                ItemsControl dragSource = itemDragEventArgs.DragSource as ItemsControl;
                // If the items control that contains the item being dragged over has the drag source as a descendent
                if (dragSource != null && itemsControl.GetVisualAncestors().Contains(dragSource))
                {
                    SelectionCollection selectionCollection = GetSelectionCollection(itemDragEventArgs.Data);
                    IEnumerable<DependencyObject> visualAncestorsAndSelf = itemsControl.GetVisualAncestorsAndSelf().ToList();

                    return selectionCollection.Any(selection => !selection.Index.HasValue || visualAncestorsAndSelf.Any(ancestor => dragSource.ItemContainerGenerator.IndexFromContainer(ancestor) == selection.Index.Value));
                }
            }
            // Can only tell if dragging into descendent if drag operation 
            // originated from a DragDropTarget object, and the data has
            // information about the index of the dragged item within its 
            // DragSource.
            return false;
        }

        /// <summary>
        /// Retrieves the drop target of a drag event.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        /// <returns>The drop target of a drag event.</returns>
        protected override ItemsControl GetDropTarget(SW.DragEventArgs args)
        {
            DependencyObject originalSource = (DependencyObject)args.OriginalSource;
            ItemsControl dropTarget = GetItemsControlAncestor(originalSource);
            if (dropTarget != null)
            {
                TreeViewItem targetItemContainer = GetItemContainerAncestor(dropTarget, (DependencyObject)args.OriginalSource);
                Orientation? orientation = GetOrientation(dropTarget);

                if (orientation != null && targetItemContainer != null)
                {
                    Rect treeViewItemRect = GetTreeViewItemRectExcludingChildren(targetItemContainer);
                    Point relativePoint = args.GetPosition(targetItemContainer);
                    double thirdWidth = treeViewItemRect.Width / 3.0;
                    double thirdHeight = treeViewItemRect.Height / 3.0;

                    // If dragging into center third of item then the drop target
                    // is the tree view item being hovered over.
                    if
                        ((orientation == Orientation.Horizontal
                            && relativePoint.X > thirdWidth && relativePoint.X < (treeViewItemRect.Width - thirdWidth))
                        || (orientation == Orientation.Vertical
                            && relativePoint.Y > thirdHeight && relativePoint.Y < (treeViewItemRect.Height - thirdHeight)))
                    {
                        return targetItemContainer;
                    }
                }
            }

            return base.GetDropTarget(args);
        }

        /// <summary>
        /// Retrieves the location and dimensions of a TreeViewItem excluding
        /// its children.
        /// </summary>
        /// <param name="treeViewItem">The tree view item.</param>
        /// <returns>The location and dimensions of the TreeViewItem excluding
        /// its children.</returns>
        protected virtual Rect GetTreeViewItemRectExcludingChildren(TreeViewItem treeViewItem)
        {
            if (treeViewItem.IsExpanded)
            {
                FrameworkElement rootVisual = treeViewItem.GetVisualChildren().FirstOrDefault() as FrameworkElement;
                if (rootVisual != null)
                {
                    FrameworkElement header =
                        rootVisual
                            .GetLogicalDescendents()
                            .Where(element => element.Name == "Header").FirstOrDefault();
                    if (header != null)
                    {
                        Rect rectangle = new Rect(0, 0, 0, 0);
                        GeneralTransform generalTransform = header.SafeTransformToVisual(treeViewItem);
                        Point origin;
                        if (generalTransform != null && generalTransform.TryTransform(new Point(0, 0), out origin))
                        {
                            rectangle = new Rect(origin, header.GetSize());
                        }
                        return rectangle;
                    }
                }
            }

            return new Rect(new Point(), treeViewItem.GetSize());
        }

        /// <summary>
        /// Returns a geometry for the insertion indicator.
        /// </summary>
        /// <param name="dropTarget">The drop target.</param>
        /// <param name="insertionIndex">The insertion index within the drop 
        /// target.</param>
        /// <param name="dragEventArgs">Information about the drag event.
        /// </param>
        /// <returns>The geometry for the insertion indicator.</returns>
        protected override Geometry GetInsertionIndicatorGeometry(ItemsControl dropTarget, int insertionIndex, SW.DragEventArgs dragEventArgs)
        {
            ItemsControl itemsControlAncestor = GetItemsControlAncestor((DependencyObject)dragEventArgs.OriginalSource);
            TreeViewItem treeViewItem = dropTarget as TreeViewItem;
            // If the parent items control is the drop target then we're 
            // appending the item into the child collection of the currently 
            // hovered tree view item.
            if (treeViewItem != null && itemsControlAncestor != dropTarget)
            {
                GeneralTransform generalTransform = dropTarget.SafeTransformToVisual(this);
                if (generalTransform != null)
                {
                    Rect dropTargetRect = GetTreeViewItemRectExcludingChildren(treeViewItem);
                    Point origin;
                    if (generalTransform.TryTransform(new Point(0, 0), out origin))
                    {
                        Point dropTargetPositionRelativeToDragDropTarget = origin;
                        dropTargetPositionRelativeToDragDropTarget.X += dropTargetRect.Left;
                        dropTargetPositionRelativeToDragDropTarget.Y += dropTargetRect.Top;
                        return new RectangleGeometry { Rect = new Rect(dropTargetPositionRelativeToDragDropTarget, new Size(dropTargetRect.Width, dropTargetRect.Height)) };
                    }
                }
            }

            return base.GetInsertionIndicatorGeometry(dropTarget, insertionIndex, dragEventArgs);
        }

        /// <summary>
        /// Gets the insertion index within a drop target given information 
        /// about a drag event.
        /// </summary>
        /// <param name="dropTarget">The drop target.</param>
        /// <param name="args">Information about a drag event.</param>
        /// <returns>The insertion index within the drop target.</returns>
        protected override int GetDropTargetInsertionIndexOverride(ItemsControl dropTarget, SW.DragEventArgs args)
        {
            ItemsControl itemsControl = GetItemsControlAncestor((DependencyObject)args.OriginalSource);

            // If the parent items control is the drop target then we're 
            // appending the item into the child collection of the currently 
            // hovered tree view item.
            if (itemsControl != dropTarget)
            {
                return dropTarget.GetItemCount();
            }

            return base.GetDropTargetInsertionIndexOverride(dropTarget, args);
        }

        /// <summary>
        /// Returns a value indicating whether a given items control
        /// can scroll.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The value indicating whether the given items control
        /// can scroll.</returns>
        protected override bool CanScroll(ItemsControl itemsControl)
        {
            TreeView content = this.Content as TreeView;
            if (content != null)
            {
                return content.GetScrollHost() != null;
            }
            return false;
        }

        /// <summary>
        /// Scrolls a given item container into the view.
        /// </summary>
        /// <param name="itemsControl">The items control that contains
        /// the item container.</param>
        /// <param name="itemContainer">The item container to scroll into
        /// view.</param>
        protected override void ScrollIntoView(ItemsControl itemsControl, TreeViewItem itemContainer)
        {
            TreeView content = this.Content as TreeView;
            if (content != null)
            {
                ScrollViewer scrollViewer = content.GetScrollHost();
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollIntoView(itemContainer, 10, 10, ScrollItemAnimationDuration);
                }
            }
        }

        private sealed class DragDropHelper
        {
            private readonly TreeViewDragDropTarget _owner;
            private readonly DispatcherTimer _timer;

            private TreeViewItem _treeViewItem;

            public DragDropHelper(TreeViewDragDropTarget owner)
            {
                _owner = owner;
                _timer = new DispatcherTimer();
                _timer.Tick += new EventHandler(OnTimerTick);
            }

            public void OnDragEnter(SW.DragEventArgs dragEventArgs)
            {
                _timer.Stop();

                if (SW.DragDropEffects.Scroll == (dragEventArgs.Effects & SW.DragDropEffects.Scroll))
                {
                    _treeViewItem = _owner.GetTreeViewItem(dragEventArgs);
                    if (_treeViewItem != null)
                    {
                        _timer.Interval = TimeSpan.FromMilliseconds(_owner.ExpandNodeDelay);
                        _timer.Start();
                    }
                }
            }

            public void OnDragLeave(SW.DragEventArgs dragEventArgs)
            {
                if (_owner.GetTreeViewItem(dragEventArgs) == _treeViewItem)
                {
                    _timer.Stop();
                }
            }

            public void OnDrop(SW.DragEventArgs dragEventArgs)
            {
                if (_owner.GetTreeViewItem(dragEventArgs) == _treeViewItem)
                {
                    _timer.Stop();
                }
            }

            private void OnTimerTick(object sender, EventArgs e)
            {
                _timer.Stop();

                if (_treeViewItem is TreeViewItem treeViewItem)
                {
                    treeViewItem.IsExpanded = true;
                }
            }
        }
    }
}