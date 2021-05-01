using System;
using System.Collections.Generic;

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
    ///     This is a base class to the DescendentsWalker. It is factored out so that 
    ///     FrameworkContextData can store and retrieve it from context local storage 
    ///     in a type agnostic manner.
    /// </summary>
    internal class DescendentsWalkerBase
    {
#region Construction

        protected DescendentsWalkerBase(TreeWalkPriority priority)
        {
            _startNode = null;
            _priority = priority;
            _recursionDepth = 0;
#if WPF
            _nodes = new FrugalStructList<DependencyObject>();
#else
            _nodes = new List<DependencyObject>();
#endif // WPF
        }

#endregion Construction

        internal bool WasVisited(DependencyObject d)
        {
            DependencyObject ancestor = d;

            while ((ancestor != _startNode) && (ancestor != null))
            {
                DependencyObject logicalParent = null;

                FrameworkElement fe = ancestor as FrameworkElement;
                if (fe != null)
                {
                    logicalParent = fe.Parent;
                    // FrameworkElement
                    DependencyObject dependencyObjectParent = VisualTreeHelper.GetParent(fe);
                    if (dependencyObjectParent != null && logicalParent != null && dependencyObjectParent != logicalParent)
                    {
                        return _nodes.Contains(ancestor);
                    }

                    // Follow visual tree if not null otherwise we follow logical tree
                    if (dependencyObjectParent != null)
                    {
                        ancestor = dependencyObjectParent;
                        continue;
                    }
                }
                ancestor = logicalParent;
            }
            return (ancestor != null);
        }


        internal DependencyObject _startNode;
        internal TreeWalkPriority _priority;

#if WPF
        internal FrugalStructList<DependencyObject> _nodes;
#else
        internal List<DependencyObject> _nodes;
#endif // WPF
        internal int _recursionDepth;
    }

    /// <summary>
    ///     Enum specifying whether visual tree needs
    ///     to be travesed first or the logical tree
    /// </summary>
    internal enum TreeWalkPriority
    {
        /// <summary>
        ///     Traverse Logical Tree first
        /// </summary>
        LogicalTree,

        /// <summary>
        ///     Traverse Visual Tree first
        /// </summary>
        VisualTree
    }
}
