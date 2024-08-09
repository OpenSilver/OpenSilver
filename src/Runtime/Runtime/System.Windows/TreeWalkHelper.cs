
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

using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows
{
    /// <summary>
    /// This is a static helper class that has methods that use the DescendentsWalker to do tree walks.
    /// </summary>
    internal static class TreeWalkHelper
    {
        /// <summary>
        ///     Invalidate inheritable properties and resource
        ///     references during a tree change operation.
        /// </summary>
        internal static void InvalidateOnTreeChange(
            FrameworkElement fe,
            DependencyObject parent,
            bool isAddOperation)
        {
            DependencyObject.InvalidateInheritedProperties(fe, fe.Parent ?? fe.VisualParent);

            if (HasChildren(fe))
            {
                // The TreeChangeInfo object is used here to track
                // information that we have because we're doing a tree walk.
                var parentInfo = new TreeChangeInfo(fe, parent, isAddOperation);

                var walker = new DescendentsWalker<TreeChangeInfo>(
                    TreeWalkPriority.LogicalTree, TreeChangeDelegate, parentInfo);

                walker.StartWalk(fe, false);
            }
            else
            {
                // Degenerate case when the current node is a leaf node and has no children.

                var parentInfo = new TreeChangeInfo(fe, parent, isAddOperation);

                // Degenerate case of OnAncestorChanged for a single node
                OnAncestorChanged(fe, parentInfo);
            }
        }

        /// <summary>
        ///     Callback on visiting each node in the descendency during a tree change
        ///     Note that this is only used in an entire sub-tree undergoes a change.
        ///     If the tree change is happening on a single node with no children, this
        ///     invalidation happens inside InvalidateOnTreeChange and this method doesn't
        ///     get involved.
        /// </summary>
        private static bool OnAncestorChanged(
            DependencyObject d,
            TreeChangeInfo info,
            bool visitedViaVisualTree)
        {
            // Invalidate properties on current instance

            if (d is IInternalFrameworkElement fe)
            {
                OnAncestorChanged(fe, info);
            }

            // Continue walk down subtree
            return true;
        }

        /// <summary>
        ///     OnAncestorChanged variant when we know what type FE the
        ///     tree node is.
        /// </summary>
        private static void OnAncestorChanged(
           IInternalFrameworkElement fe,
           TreeChangeInfo info)
        {
            fe.OnAncestorChangedInternal(info);
        }

        /// <summary>
        ///     Invalidates all the properties on the nodes in the given sub-tree
        ///     that are referring to the resource[s] that are changing.
        /// </summary>
        internal static void InvalidateOnResourcesChange(
            IInternalFrameworkElement fe,
            ResourcesChangeInfo info)
        {
            Debug.Assert(fe is not null, "Node with the resources change notification must be a FrameworkElement.");

            if (HasChildren(fe))
            {
                // Spin up a DescendentsWalker only when
                // the current node has children to walk

                var walker = new DescendentsWalker<ResourcesChangeInfo>(
                    TreeWalkPriority.LogicalTree, ResourcesChangeDelegate, info);

                walker.StartWalk(fe.AsDependencyObject(), false);
            }
            else
            {
                // Degenerate case when the current node is a leaf node and has no children.

                OnResourcesChanged(fe.AsDependencyObject(), info);
            }
        }

        /// <summary>
        ///     Callback on visiting each node in the descendency
        ///     during a resources change.
        /// </summary>
        private static bool OnResourcesChangedCallback(
            DependencyObject d,
            ResourcesChangeInfo info,
            bool visitedViaVisualTree)
        {
            OnResourcesChanged(d, info);

            // Continue walk down subtree
            return true;
        }

        /// <summary>
        ///     Process a resource change for the given DependencyObject.
        ///     Return true if the DO has resource references.
        /// </summary>
        internal static void OnResourcesChanged(
            DependencyObject d,
            ResourcesChangeInfo info)
        {
            if (d is IInternalFrameworkElement fe)
            {
                fe.OnResourcesChanged(info);
            }
        }

        internal static void InvalidateOnInheritablePropertyChange(
            IInternalUIElement uie,
            InheritablePropertyChangeInfo info,
            bool skipStartNode)
        {
            if (HasChildren(uie))
            {
                var walker = new DescendentsWalker<InheritablePropertyChangeInfo>(
                    TreeWalkPriority.LogicalTree, InheritablePropertyChangeDelegate, info);

                walker.StartWalk(uie.AsDependencyObject(), skipStartNode);
            }
            else if (!skipStartNode)
            {
                // Degenerate case when the current node is a leaf node and has no children.
                // If the current node needs a notification, do so now.
                bool visitedViaVisualTree = false;
                OnInheritablePropertyChanged(uie.AsDependencyObject(), info, visitedViaVisualTree);
            }
        }

        /// <summary>
        /// Callback on visiting each node in the descendency
        /// during an inheritable property change
        /// </summary>
        private static bool OnInheritablePropertyChanged(
            DependencyObject d,
            InheritablePropertyChangeInfo info,
            bool visitedViaVisualTree)
        {
            Debug.Assert(d != null, "Must have non-null current node");

            DependencyProperty dp = info.Property;
            PropertyMetadata metadata = dp.GetMetadata(d.DependencyObjectType);
            bool inheritanceNode = IsInheritanceNode(metadata);

            if (inheritanceNode)
            {
                Storage storage = d.GetStorage(dp, metadata, false);
                BaseValueSourceInternal oldValueSource = storage?.Entry.BaseValueSourceInternal ?? BaseValueSourceInternal.Default;

                // If the oldValueSource is of lower precedence than Inheritance
                // only then do we need to Invalidate the property
                if (BaseValueSourceInternal.Inherited >= oldValueSource)
                {
                    if (visitedViaVisualTree && d is IInternalFrameworkElement fe)
                    {
                        DependencyObject logicalParent = fe.Parent;
                        if (logicalParent != null)
                        {
                            DependencyObject visualParent = fe.VisualParent;
                            if (visualParent != null && visualParent != logicalParent)
                            {
                                return false;
                            }
                        }
                    }

                    return d.SetInheritedValue(dp, metadata, info.NewValue, false);
                }
                else
                {
                    Debug.Assert(storage is not null);

                    // set the inherited value so that it is known if at some point,
                    // the value of higher precedence that is currently used is removed.
                    // we know that the value of the property is not changing, so we can
                    // skip the call to UpdateEffectiveValue(...)
                    storage.InheritedValue = info.NewValue;
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Determine if the current DependencyObject is a candidate for
        /// producing inheritable values
        /// </summary>
        internal static bool IsInheritanceNode(PropertyMetadata metadata)
        {
            return metadata is not null && metadata.Inherits;
        }

        /// <summary>
        /// Says if the current FE has visual or logical children
        /// </summary>
        internal static bool HasChildren(IInternalUIElement uie)
        {
            // See if we have logical or visual children, in which case this is a real tree invalidation.
            return uie != null && (uie.HasVisualChildren ||
                (uie is IInternalFrameworkElement fe && fe.HasLogicalChildren));
        }

        private static readonly VisitedCallback<TreeChangeInfo> TreeChangeDelegate
            = new VisitedCallback<TreeChangeInfo>(OnAncestorChanged);

        private static readonly VisitedCallback<ResourcesChangeInfo> ResourcesChangeDelegate
            = new VisitedCallback<ResourcesChangeInfo>(OnResourcesChangedCallback);

        private static readonly VisitedCallback<InheritablePropertyChangeInfo> InheritablePropertyChangeDelegate
            = new VisitedCallback<InheritablePropertyChangeInfo>(OnInheritablePropertyChanged);
    }
}
