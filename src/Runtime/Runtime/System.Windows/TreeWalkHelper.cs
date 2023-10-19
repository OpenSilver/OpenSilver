using System;
using System.Diagnostics;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows
{
    /// <summary>
    ///     This is a static helper class that has methods
    ///     that use the DescendentsWalker to do tree walks.
    /// </summary>
    internal static class TreeWalkHelper
    {
        #region InheritablePropertyChange

        internal static void InvalidateOnInheritablePropertyChange(IInternalFrameworkElement fe, InheritablePropertyChangeInfo info, bool skipStartNode)
        {
            if (HasChildren(fe))
            {
                DescendentsWalker<InheritablePropertyChangeInfo> walker = new DescendentsWalker<InheritablePropertyChangeInfo>(
                    TreeWalkPriority.LogicalTree, InheritablePropertyChangeDelegate, info);

                walker.StartWalk(fe.AsDependencyObject(), skipStartNode);
            }
            else if (!skipStartNode)
            {
                // Degenerate case when the current node is a leaf node and has no children.
                // If the current node needs a notification, do so now.
                bool visitedViaVisualTree = false;
                OnInheritablePropertyChanged(fe.AsDependencyObject(), info, visitedViaVisualTree);
            }
        }

        /// <summary>
        ///     Callback on visiting each node in the descendency
        ///     during an inheritable property change
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
                BaseValueSourceInternal oldValueSource = BaseValueSourceInternal.Default;
                if (INTERNAL_PropertyStore.TryGetInheritedPropertyStorage(d, dp, metadata, false, out INTERNAL_PropertyStorage storage))
                {
                    oldValueSource = storage.Entry.BaseValueSourceInternal;
                }

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
                    if (storage == null)
                    {
                        // get the storage if we didn't to it ealier.
                        INTERNAL_PropertyStore.TryGetInheritedPropertyStorage(d,
                            dp,
                            metadata,
                            true,
                            out storage);
                    }

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
        ///     Determine if the current DependencyObject is a candidate for
        ///     producing inheritable values
        /// </summary>
        internal static bool IsInheritanceNode(PropertyMetadata metadata)
        {
            return metadata is not null && metadata.Inherits;
        }

        #endregion InheritablePropertyChange

        #region PrivateMethods

        /// <summary>
        ///     Says if the current FE has visual or logical children
        /// </summary>
        internal static bool HasChildren(IInternalFrameworkElement fe)
        {
            // See if we have logical or visual children, in which case this is a real tree invalidation.
            return fe != null && (fe.HasLogicalChildren || fe.HasVisualChildren);
        }

        #endregion PrivateMethods

        #region StaticData

        private static VisitedCallback<InheritablePropertyChangeInfo> InheritablePropertyChangeDelegate
            = new VisitedCallback<InheritablePropertyChangeInfo>(OnInheritablePropertyChanged);

        #endregion StaticData
    }
}
