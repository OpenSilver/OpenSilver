
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal;

/// <summary>
/// This class iterates and callsback for each descendent in a given subtree
/// </summary>
internal struct DescendentsWalker<T>
{
    public DescendentsWalker(TreeWalkPriority priority, VisitedCallback<T> callback, T data)
    {
        _callback = callback;
        _data = data;
        _startNode = null;
        _priority = priority;
        _recursionDepth = 0;
        _nodes = new HashSet<DependencyObject>();
    }

    /// <summary>
    /// Start Iterating through the current subtree
    /// </summary>
    public void StartWalk(DependencyObject startNode, bool skipStartNode)
    {
        _startNode = startNode;
        bool continueWalk = true;

        if (!skipStartNode)
        {
            // Callback for the root of the subtree
            continueWalk = _callback(_startNode, _data, _priority == TreeWalkPriority.VisualTree);
        }

        if (continueWalk)
        {
            // Iterate through the children of the root
            IterateChildren(_startNode);
        }
    }

    /// <summary>
    /// Given a DependencyObject, see if it's any of the types we know
    /// to have children.  If so, call VisitNode on each of its children.
    /// </summary>
    private void IterateChildren(DependencyObject d)
    {
        _recursionDepth++;

        if (d is IInternalFrameworkElement fe)
        {
            bool hasLogicalChildren = fe.HasLogicalChildren;

            // FrameworkElement have both a visual and a logical tree.
            //  Sometimes we want to hit Visual first, sometimes Logical.

            if (_priority == TreeWalkPriority.VisualTree)
            {
                WalkFrameworkElementVisualThenLogicalChildren(fe, hasLogicalChildren);
            }
            else if (_priority == TreeWalkPriority.LogicalTree)
            {
                WalkFrameworkElementLogicalThenVisualChildren(fe, hasLogicalChildren);
            }
            else
            {
                Debug.Assert(false, "Tree walk priority should be Visual first or Logical first - but this instance of DescendentsWalker has an invalid priority setting that's neither of the two.");
            }
        }
        else
        {
            // Not a FrameworkElement.  See if it's a UIElement 
            // and if so walk the UIElement collection
            if (d is IInternalUIElement v)
            {
                WalkVisualChildren(v);
            }
        }

        _recursionDepth--;
    }

    /// <summary>
    /// Given a object of type Visual, call VisitNode on each of its
    /// Visual children.
    /// </summary>
    private void WalkVisualChildren(IInternalUIElement v)
    {
        v.IsVisualChildrenIterationInProgress = true;

        try
        {
            int count = v.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                DependencyObject childVisual = v.GetVisualChild(i);
                if (childVisual != null)
                {
                    bool visitedViaVisualTree = true;
                    VisitNode(childVisual, visitedViaVisualTree);
                }
            }
        }
        finally
        {
            v.IsVisualChildrenIterationInProgress = false;
        }
    }

    /// <summary>
    /// Given an enumerator for Logical children, call VisitNode on each
    /// of the nodes in the enumeration.
    /// </summary>
    private void WalkLogicalChildren(
        IInternalFrameworkElement feParent,
        IEnumerator logicalChildren)
    {
        feParent.IsLogicalChildrenIterationInProgress = true;

        try
        {
            if (logicalChildren != null)
            {
                while (logicalChildren.MoveNext())
                {
                    if (logicalChildren.Current is DependencyObject child)
                    {
                        bool visitedViaVisualTree = false;
                        VisitNode(child, visitedViaVisualTree);
                    }
                }
            }
        }
        finally
        {
            feParent.IsLogicalChildrenIterationInProgress = false;
        }
    }

    /// <summary>
    /// FrameworkElement objects have both a visual and a logical tree.
    /// This variant walks the visual children first
    /// </summary>
    /// <remarks>
    /// It calls the generic WalkVisualChildren, but doesn't call the
    /// generic WalkLogicalChildren because there are shortcuts we can take
    /// to be smarter than the generic logical children walk.
    /// </remarks>
    private void WalkFrameworkElementVisualThenLogicalChildren(
        IInternalFrameworkElement feParent, bool hasLogicalChildren)
    {
        WalkVisualChildren(feParent);

        feParent.IsLogicalChildrenIterationInProgress = true;

        try
        {
            // Optimized variant of WalkLogicalChildren
            if (hasLogicalChildren)
            {
                var logicalChildren = feParent.LogicalChildren;
                if (logicalChildren != null)
                {
                    while (logicalChildren.MoveNext())
                    {
                        if (logicalChildren.Current is IInternalFrameworkElement fe)
                        {
                            // For the case that both parents are identical, this node should
                            // have already been visited when walking through visual
                            // children, hence we short-circuit here
                            if (VisualTreeHelper.GetParent(fe) != fe.Parent)
                            {
                                bool visitedViaVisualTree = false;
                                VisitNode(fe, visitedViaVisualTree);
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            feParent.IsLogicalChildrenIterationInProgress = false;
        }
    }

    /// <summary>
    /// FrameworkElement objects have both a visual and a logical tree.
    /// This variant walks the logical children first
    /// </summary>
    /// <remarks>
    /// It calls the generic WalkLogicalChildren, but doesn't call the
    /// generic WalkVisualChildren because there are shortcuts we can take
    /// to be smarter than the generic visual children walk.
    /// </remarks>
    private void WalkFrameworkElementLogicalThenVisualChildren(
        IInternalFrameworkElement feParent, bool hasLogicalChildren)
    {
        if (hasLogicalChildren)
        {
            WalkLogicalChildren(feParent, feParent.LogicalChildren);
        }

        feParent.IsVisualChildrenIterationInProgress = true;

        try
        {
            // Optimized variant of WalkVisualChildren
            int count = feParent.VisualChildrenCount;

            for (int i = 0; i < count; i++)
            {
                var child = feParent.GetVisualChild(i);
                if (child is IInternalUIElement)
                {
                    // For the case that both parents are identical, this node should
                    // have already been visited when walking through logical
                    // children, hence we short-circuit here
                    if (child is not IInternalFrameworkElement feChild ||
                        VisualTreeHelper.GetParent(child) != feChild.Parent)
                    {
                        bool visitedViaVisualTree = true;
                        VisitNode(child, visitedViaVisualTree);
                    }
                }
            }
        }
        finally
        {
            feParent.IsVisualChildrenIterationInProgress = false;
        }
    }

    private void VisitNode(IInternalUIElement uie, bool visitedViaVisualTree)
    {
        if (_recursionDepth <= 4096 /* ContextLayoutManager.s_LayoutRecursionLimit */)
        {
            // For the case when the collection contains the node
            // being visted, we do not need to visit it again. Also
            // this node will not be visited another time because
            // any node can be reached at most two times, once
            // via its visual parent and once via its logical parent

            if (_nodes.Remove(uie.AsDependencyObject()))
            {
                return;
            }

            if (uie is IInternalFrameworkElement fe)
            {
                DependencyObject visualParent = VisualTreeHelper.GetParent(fe);
                DependencyObject logicalParent = fe.Parent;
                if (visualParent != null && logicalParent != null && visualParent != logicalParent)
                {
                    _nodes.Add(fe.AsDependencyObject());
                }
            }

            DependencyObject d = uie.AsDependencyObject();

            // Callback for this node of the subtree
            if (_callback(d, _data, visitedViaVisualTree))
            {
                // Iterate through the children of this node
                IterateChildren(d);
            }
        }
        else
        {
            // We suspect a loop here because the recursion
            // depth has exceeded the MAX_TREE_DEPTH expected
            throw new InvalidOperationException("Logical tree depth exceeded while traversing the tree. This could indicate a cycle in the tree.");
        }
    }

    private void VisitNode(DependencyObject d, bool visitedViaVisualTree)
    {
        if (_recursionDepth <= 4096 /* ContextLayoutManager.s_LayoutRecursionLimit */)
        {
            if (d is IInternalUIElement uie)
            {
                VisitNode(uie, visitedViaVisualTree);
            }
        }
        else
        {
            // We suspect a loop here because the recursion
            // depth has exceeded the MAX_TREE_DEPTH expected
            throw new InvalidOperationException("Logical tree depth exceeded while traversing the tree. This could indicate a cycle in the tree.");
        }
    }

    private DependencyObject _startNode;
    private int _recursionDepth;
    private readonly TreeWalkPriority _priority;
    private readonly HashSet<DependencyObject> _nodes;
    private readonly VisitedCallback<T> _callback;
    private readonly T _data;
}

/// <summary>
/// Callback for each visited node
/// </summary>
internal delegate bool VisitedCallback<T>(DependencyObject d, T data, bool visitedViaVisualTree);

/// <summary>
/// Enum specifying whether visual tree needs
/// to be travesed first or the logical tree
/// </summary>
internal enum TreeWalkPriority
{
    /// <summary>
    /// Traverse Logical Tree first
    /// </summary>
    LogicalTree,

    /// <summary>
    /// Traverse Visual Tree first
    /// </summary>
    VisualTree
}
