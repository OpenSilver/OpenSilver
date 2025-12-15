
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
using System.Windows.Controls.Primitives;
using OpenSilver.Internal;

namespace System.Windows;

/// <summary>
/// This is a static helper class that has methods that use the DescendentsWalker to do tree walks.
/// </summary>
internal static class TreeWalkHelper
{
    /// <summary>
    ///     Invalidate inheritable properties and resource
    ///     references during a tree change operation.
    /// </summary>
    internal static void InvalidateOnTreeChange<TFrameworkElement>(TFrameworkElement fe, DependencyObject parent, bool isAddOperation)
        where TFrameworkElement : DependencyObject, IInternalFrameworkElement
    {
        InvalidateInheritedProperties(fe);

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

    private static void InvalidateInheritedProperties<TFrameworkElement>(TFrameworkElement fe)
        where TFrameworkElement : DependencyObject, IInternalFrameworkElement
    {
        DependencyObject parent = fe.Parent ??
            fe.VisualParent ??
            FrameworkElement.FindMentor(fe.InheritanceContext)?.AsDependencyObject();

        DependencyObject.InvalidateInheritedProperties(fe, parent);
    }

    /// <summary>
    ///     Callback on visiting each node in the descendency during a tree change
    ///     Note that this is only used in an entire sub-tree undergoes a change.
    ///     If the tree change is happening on a single node with no children, this
    ///     invalidation happens inside InvalidateOnTreeChange and this method doesn't
    ///     get involved.
    /// </summary>
    private static bool OnAncestorChanged(DependencyObject d, TreeChangeInfo info, bool visitedViaVisualTree)
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
    private static void OnAncestorChanged<TFrameworkElement>(TFrameworkElement fe, TreeChangeInfo info)
        where TFrameworkElement : IInternalFrameworkElement
    {
        fe.OnAncestorChangedInternal(info);
    }

    /// <summary>
    ///     Invalidates all the properties on the nodes in the given sub-tree
    ///     that are referring to the resource[s] that are changing.
    /// </summary>
    internal static void InvalidateOnResourcesChange<TFrameworkElement>(TFrameworkElement fe, ResourcesChangeInfo info)
        where TFrameworkElement : DependencyObject, IInternalFrameworkElement
    {
        Debug.Assert(fe is not null, "Node with the resources change notification must be a FrameworkElement.");

        if (HasChildren(fe))
        {
            // Spin up a DescendentsWalker only when
            // the current node has children to walk

            var walker = new DescendentsWalker<ResourcesChangeInfo>(
                TreeWalkPriority.LogicalTree, ResourcesChangeDelegate, info);

            walker.StartWalk(fe, false);
        }
        else
        {
            // Degenerate case when the current node is a leaf node and has no children.

            OnResourcesChanged(fe, info);
        }
    }

    /// <summary>
    ///     Callback on visiting each node in the descendency
    ///     during a resources change.
    /// </summary>
    private static bool OnResourcesChangedCallback(DependencyObject d, ResourcesChangeInfo info, bool visitedViaVisualTree)
    {
        if (d is IInternalFrameworkElement fe)
        {
            OnResourcesChanged(fe, info);
        }

        // Continue walk down subtree
        return true;
    }

    /// <summary>
    ///     Process a resource change for the given DependencyObject.
    ///     Return true if the DO has resource references.
    /// </summary>
    internal static void OnResourcesChanged<TFrameworkElement>(TFrameworkElement fe, ResourcesChangeInfo info)
        where TFrameworkElement : IInternalFrameworkElement
    {
        fe.OnResourcesChanged(info);
    }

    internal static void InvalidateOnInheritablePropertyChange<TUIElement>(TUIElement uie, InheritablePropertyChangeInfo info, bool skipStartNode)
        where TUIElement : DependencyObject, IInternalUIElement
    {
        if (HasChildren(uie))
        {
            var walker = new DescendentsWalker<InheritablePropertyChangeInfo>(
                TreeWalkPriority.LogicalTree, InheritablePropertyChangeDelegate, info);

            walker.StartWalk(uie, skipStartNode);
        }
        else if (!skipStartNode)
        {
            // Degenerate case when the current node is a leaf node and has no children.
            // If the current node needs a notification, do so now.
            bool visitedViaVisualTree = false;
            OnInheritablePropertyChanged(uie, info, visitedViaVisualTree);
        }
    }

    private static bool IsForceInheritedProperty(DependencyProperty dp) => dp == FrameworkElement.FlowDirectionProperty;

    /// <summary>
    /// Callback on visiting each node in the descendency
    /// during an inheritable property change
    /// </summary>
    private static bool OnInheritablePropertyChanged(DependencyObject d, InheritablePropertyChangeInfo info, bool visitedViaVisualTree)
    {
        Debug.Assert(d is not null, "Must have non-null current node");

        DependencyProperty dp = info.Property;
        PropertyMetadata metadata = dp.GetMetadata(d.DependencyObjectType);
        bool inheritanceNode = IsInheritanceNode(metadata);
        bool isForceInheritedProperty = IsForceInheritedProperty(dp);

        if (inheritanceNode || isForceInheritedProperty)
        {
            Storage storage = d.GetStorage(dp.GlobalIndex);
            BaseValueSourceInternal oldValueSource = storage is not null ?
                storage.Entry.BaseValueSourceInternal :
                BaseValueSourceInternal.Default;

            // If the oldValueSource is of lower precedence than Inheritance
            // only then do we need to Invalidate the property
            if (BaseValueSourceInternal.Inherited >= oldValueSource)
            {
                if (visitedViaVisualTree && d is IInternalFrameworkElement fe)
                {
                    if (fe.Parent is DependencyObject logicalParent &&
                        fe.VisualParent is DependencyObject visualParent &&
                        visualParent != logicalParent)
                    {
                        return false;
                    }
                }

                return d.SetInheritedValue(dp, metadata, info.NewValue, false);
            }
            else
            {
                Debug.Assert(storage is not null);

                storage.InheritedValue = info.NewValue;

                if (isForceInheritedProperty)
                {
                    return DependencyObjectStore.UpdateEffectiveValue(
                        storage,
                        d,
                        dp,
                        metadata,
                        ref storage.Entry,
                        ref storage.Entry,
                        false,
                        OperationType.Inherit);
                }

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
    internal static bool HasChildren<TUIElement>(TUIElement uie)
        where TUIElement : DependencyObject, IInternalUIElement
    {
        // See if we have logical or visual children, in which case this is a real tree invalidation.
        return uie is not null &&
            (uie.HasVisualChildren ||
             Popup.RegisteredPopupsField.GetValue(uie) is not null ||
             (uie is IInternalFrameworkElement fe && fe.HasLogicalChildren));
    }

    private static readonly VisitedCallback<TreeChangeInfo> TreeChangeDelegate
        = new(OnAncestorChanged);

    private static readonly VisitedCallback<ResourcesChangeInfo> ResourcesChangeDelegate
        = new(OnResourcesChangedCallback);

    private static readonly VisitedCallback<InheritablePropertyChangeInfo> InheritablePropertyChangeDelegate
        = new(OnInheritablePropertyChanged);
}
