using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace OpenSilver.Internal
{
    /// <summary>
    ///     This class iterates and callsback for
    ///     each descendent in a given subtree
    /// </summary>
    internal class DescendentsWalker<T> : DescendentsWalkerBase
    {
#region Construction

        public DescendentsWalker(TreeWalkPriority priority, VisitedCallback<T> callback) :
            this(priority, callback, default(T))
        {
            // Forwarding
        }

        public DescendentsWalker(TreeWalkPriority priority, VisitedCallback<T> callback, T data)
            : base(priority)
        {
            _callback = callback;
            _data = data;
        }

#endregion Construction

        /// <summary>
        ///     Start Iterating through the current subtree
        /// </summary>
        public void StartWalk(DependencyObject startNode)
        {
            // Don't skip starting node
            StartWalk(startNode, false);
        }

        /// <summary>
        ///     Start Iterating through the current subtree
        /// </summary>
        public virtual void StartWalk(DependencyObject startNode, bool skipStartNode)
        {
            _startNode = startNode;
            bool continueWalk = true;

            if (!skipStartNode)
            {
                if (typeof(FrameworkElement).IsInstanceOfType(_startNode))
                {
                    // Callback for the root of the subtree
                    continueWalk = _callback(_startNode, _data, _priority == TreeWalkPriority.VisualTree);
                }
            }

            if (continueWalk)
            {
                // Iterate through the children of the root
                IterateChildren(_startNode);
            }
        }

        /// <summary>
        ///     Given a DependencyObject, see if it's any of the types we know
        /// to have children.  If so, call VisitNode on each of its children.
        /// </summary>
        private void IterateChildren(DependencyObject d)
        {
            _recursionDepth++;

            if (typeof(FrameworkElement).IsInstanceOfType(d))
            {
                FrameworkElement fe = (FrameworkElement)d;
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
                UIElement v = d as UIElement;
                if (v != null)
                {
                    WalkVisualChildren(v);
                }
            }

            _recursionDepth--;
        }

        /// <summary>
        ///     Given a object of type Visual, call VisitNode on each of its
        /// Visual children.
        /// </summary>
        private void WalkVisualChildren(UIElement v)
        {
#if WPF
            v.IsVisualChildrenIterationInProgress = true;

            try
            {
                int count = v.InternalVisual2DOr3DChildrenCount;
                for (int i = 0; i < count; i++)
                {
                    DependencyObject childVisual = v.InternalGet2DOr3DVisualChild(i);
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
#else
            if (v.INTERNAL_VisualChildrenInformation != null)
            {
                foreach (DependencyObject childVisual in v.INTERNAL_VisualChildrenInformation.Select(kp => kp.Key))
                {
                    if (childVisual != null)
                    {
                        bool visitedViaVisualTree = true;
                        VisitNode(childVisual, visitedViaVisualTree);
                    }
                }
            }
#endif // WPF
        }

        /// <summary>
        ///     Given an enumerator for Logical children, call VisitNode on each
        /// of the nodes in the enumeration.
        /// </summary>
        private void WalkLogicalChildren(
            FrameworkElement feParent,
            IEnumerator logicalChildren)
        {
            feParent.IsLogicalChildrenIterationInProgress = true;

            try
            {
                if (logicalChildren != null)
                {
                    while (logicalChildren.MoveNext())
                    {
                        DependencyObject child = logicalChildren.Current as DependencyObject;
                        if (child != null)
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
        ///     FrameworkElement objects have both a visual and a logical tree.
        /// This variant walks the visual children first
        /// </summary>
        /// <remarks>
        ///     It calls the generic WalkVisualChildren, but doesn't call the
        /// generic WalkLogicalChildren because there are shortcuts we can take
        /// to be smarter than the generic logical children walk.
        /// </remarks>
        private void WalkFrameworkElementVisualThenLogicalChildren(
            FrameworkElement feParent, bool hasLogicalChildren)
        {
            WalkVisualChildren(feParent);

#if WPF
            //
            // If a popup is attached to the framework element visit each popup node.
            //
            List<Popup> registeredPopups = Popup.RegisteredPopupsField.GetValue(feParent);

            if (registeredPopups != null)
            {
                foreach (Popup p in registeredPopups)
                {
                    bool visitedViaVisualTree = false;
                    VisitNode(p, visitedViaVisualTree);
                }
            }
#endif // WPF

            feParent.IsLogicalChildrenIterationInProgress = true;

            try
            {
                // Optimized variant of WalkLogicalChildren
                if (hasLogicalChildren)
                {
                    IEnumerator logicalChildren = feParent.LogicalChildren;
                    if (logicalChildren != null)
                    {
                        while (logicalChildren.MoveNext())
                        {
                            object current = logicalChildren.Current;
                            FrameworkElement fe = current as FrameworkElement;
                            if (fe != null)
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
        ///     FrameworkElement objects have both a visual and a logical tree.
        /// This variant walks the logical children first
        /// </summary>
        /// <remarks>
        ///     It calls the generic WalkLogicalChildren, but doesn't call the
        /// generic WalkVisualChildren because there are shortcuts we can take
        /// to be smarter than the generic visual children walk.
        /// </remarks>
        private void WalkFrameworkElementLogicalThenVisualChildren(
            FrameworkElement feParent, bool hasLogicalChildren)
        {
            if (hasLogicalChildren)
            {
                WalkLogicalChildren(feParent, feParent.LogicalChildren);
            }

#if WPF
            feParent.IsVisualChildrenIterationInProgress = true;

            try
            {
                // Optimized variant of WalkVisualChildren
                int count = feParent.InternalVisualChildrenCount;

                for (int i = 0; i < count; i++)
                {
                    Visual child = feParent.InternalGetVisualChild(i);
                    if (child != null && FrameworkElement.DType.IsInstanceOfType(child))
                    {
                        // For the case that both parents are identical, this node should
                        // have already been visited when walking through logical
                        // children, hence we short-circuit here
                        if (VisualTreeHelper.GetParent(child) != ((FrameworkElement)child).Parent)
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
#else
            // Optimized variant of WalkVisualChildren
            if (feParent.INTERNAL_VisualChildrenInformation != null)
            {
                foreach (UIElement child in feParent.INTERNAL_VisualChildrenInformation.Select(kp => kp.Key))
                {
                    if (child != null && typeof(FrameworkElement).IsInstanceOfType(child))
                    {
                        // For the case that both parents are identical, this node should
                        // have already been visited when walking through logical
                        // children, hence we short-circuit here
                        if (VisualTreeHelper.GetParent(child) != ((FrameworkElement)child).Parent)
                        {
                            bool visitedViaVisualTree = true;
                            VisitNode(child, visitedViaVisualTree);
                        }
                    }
                }
            }
#endif // WPF

#if WPF
            //
            // If a popup is attached to the framework element visit each popup node.
            //
            List<Popup> registeredPopups = Popup.RegisteredPopupsField.GetValue(feParent);

            if (registeredPopups != null)
            {
                foreach (Popup p in registeredPopups)
                {
                    bool visitedViaVisualTree = false;
                    VisitNode(p, visitedViaVisualTree);
                }
            }
#endif // WPF
        }

        private void VisitNode(FrameworkElement fe, bool visitedViaVisualTree)
        {
            if (_recursionDepth <= 4096 /* ContextLayoutManager.s_LayoutRecursionLimit */)
            {
                // For the case when the collection contains the node
                // being visted, we do not need to visit it again. Also
                // this node will not be visited another time because
                // any node can be reached at most two times, once
                // via its visual parent and once via its logical parent

                int index = _nodes.IndexOf(fe);

                // If index is not -1, then fe was in the list, remove it
                if (index != -1)
                {
                    _nodes.RemoveAt(index);
                }
                else
                {
                    // A node will be visited a second time only if it has
                    // different non-null logical and visual parents.
                    // Hence that is the only case that we need to
                    // remember this node, to avoid duplicate callback for it

                    DependencyObject dependencyObjectParent = VisualTreeHelper.GetParent(fe);
                    DependencyObject logicalParent = fe.Parent;
                    if (dependencyObjectParent != null && logicalParent != null && dependencyObjectParent != logicalParent)
                    {
                        _nodes.Add(fe);
                    }

                    _VisitNode(fe, visitedViaVisualTree);
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
                if (typeof(FrameworkElement).IsInstanceOfType(d))
                {
                    VisitNode(d as FrameworkElement, visitedViaVisualTree);
                }
                else
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

        protected virtual void _VisitNode(DependencyObject d, bool visitedViaVisualTree)
        {
            // Callback for this node of the subtree
            bool continueWalk = _callback(d, _data, visitedViaVisualTree);
            if (continueWalk)
            {
                // Iterate through the children of this node
                IterateChildren(d);
            }
        }

        protected T Data
        {
            get
            {
                return _data;
            }
        }

        private VisitedCallback<T> _callback;
        private T _data;
    }

    /// <summary>
    ///     Callback for each visited node
    /// </summary>
    internal delegate bool VisitedCallback<T>(DependencyObject d, T data, bool visitedViaVisualTree);
}
