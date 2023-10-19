
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using SW = Microsoft.Windows;

namespace System.Windows.Controls
{
    public class TreeViewDragDropTarget : ItemsControlDragDropTarget<ItemsControl, TreeViewItem>
    {
        /// <summary>
        /// Throws an exception if the content is not a TreeView.
        /// </summary>
        /// <param name="oldContent">The old content value.</param>
        /// <param name="newContent">The new content value.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent != null && !(newContent is TreeView))
            {
                throw new ArgumentException("The content property must of type TreeView.");
            }

            base.OnContentChanged(oldContent, newContent);
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
                        GeneralTransform generalTransform;
                        try
                        {
                            generalTransform = header.TransformToVisual(treeViewItem);
                        }
                        catch (ArgumentException)
                        {
                            generalTransform = null;
                        }
                        Point origin;
                        if (generalTransform != null && generalTransform.TryTransform(new Point(0, 0), out origin))
                        {
                            rectangle = new Rect(origin, new Size(header.ActualWidth, header.ActualHeight));
                        }
                        return rectangle;
                    }
                }
            }

            return new Rect(new Point(), new Size(treeViewItem.ActualWidth, treeViewItem.ActualHeight));
        }

        /// <inheritdoc/>
        internal override TreeViewItem INTERNAL_GetDeepestItemContainer(ItemsControl itemsControl,
            List<object> elementsFromDeepestToRoot)
        {
            TreeViewItem deepestItemContainer = null;
            foreach (object element in elementsFromDeepestToRoot)
            {
                if (element is ToggleButton)
                {
                    // Do not start drag & drop when trying to expand a TreeViewItem with the ToggleButton
                    return null;
                }

                if (deepestItemContainer == null && element is TreeViewItem deepestTreeViewItem)
                {
                    deepestItemContainer = deepestTreeViewItem;
                }
            }
            // Not possible to use the base method because items deeper than on the first level
            // fail the test of whether they have an index on the TreeView (similar to a contains test).
            // Instead, these deeper items belong to their TreeViewItem parents, instead to the TreeView.
            // So the contains test is skipped and the deepest item is just returned.
            return deepestItemContainer;
        }
    }
}
