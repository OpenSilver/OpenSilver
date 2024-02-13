
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
using System.Windows.Media;
using OpenSilver.Internal;

namespace System.Windows
{
    /// <summary>
    /// This is a static helper class that has methods
    /// that use the DescendentsWalker to do tree walks.
    /// </summary>
    internal static class TreeWalkHelper
    {
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
                    if (visitedViaVisualTree && typeof(IInternalFrameworkElement).IsInstanceOfType(d))
                    {
                        DependencyObject logicalParent = ((IInternalFrameworkElement)d).Parent;
                        if (logicalParent != null)
                        {
                            DependencyObject visualParent = VisualTreeHelper.GetParent(d);
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

        private static readonly VisitedCallback<InheritablePropertyChangeInfo> InheritablePropertyChangeDelegate
            = new VisitedCallback<InheritablePropertyChangeInfo>(OnInheritablePropertyChanged);
    }
}
