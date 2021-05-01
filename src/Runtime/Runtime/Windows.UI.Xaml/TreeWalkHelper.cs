using System;
using System.Diagnostics;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;

#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    ///     This is a static helper class that has methods
    ///     that use the DescendentsWalker to do tree walks.
    /// </summary>
    internal static class TreeWalkHelper
    {
        #region InheritablePropertyChange

        internal static void InvalidateOnInheritablePropertyChange(FrameworkElement fe, InheritablePropertyChangeInfo info)
        {
            if (fe.HasLogicalChildren || (fe.INTERNAL_VisualChildrenInformation != null &&
                                          fe.INTERNAL_VisualChildrenInformation.Count > 0))
            {
                DescendentsWalker<InheritablePropertyChangeInfo> walker = new DescendentsWalker<InheritablePropertyChangeInfo>(
                    TreeWalkPriority.LogicalTree, InheritablePropertyChangeDelegate, info);

                walker.StartWalk(fe, true);
            }

#if false
            if (rootFE != null)
            {
                if (rootFE.HasLogicalChildren)
                {
                    rootFE.IsLogicalChildrenIterationInProgress = true;
                    try
                    {
                        IEnumerator logicalChildren = rootFE.LogicalChildren;
                        if (logicalChildren != null)
                        {
                            while (logicalChildren.MoveNext())
                            {
                                DependencyObject child = logicalChildren.Current as DependencyObject;
                                if (child != null)
                                {
                                    child.SetInheritedValue(info.Property, info.NewValue, true);
                                }
                            }
                        }
                    }
                    finally
                    {
                        rootFE.IsLogicalChildrenIterationInProgress = false;
                    }
                }
            }

            if (rootUIE != null)
            {
                if (rootUIE.INTERNAL_VisualChildrenInformation != null)
                {
                    foreach (UIElement child in rootUIE.INTERNAL_VisualChildrenInformation.Keys)
                    {
                        child.SetInheritedValue(info.Property, info.NewValue, true);
                    }
                }
            }
#endif // false
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
            bool inheritanceNode = IsInheritanceNode(d, dp);

            if (inheritanceNode)
            {
                BaseValueSourceInternal oldValueSource = BaseValueSourceInternal.Default;
                INTERNAL_PropertyStorage storage;
                if (INTERNAL_PropertyStore.TryGetInheritedPropertyStorage(d, dp, false, out storage))
                {
                    oldValueSource = storage.BaseValueSourceInternal;
                }

                // If the oldValueSource is of lower precedence than Inheritance
                // only then do we need to Invalidate the property
                if (BaseValueSourceInternal.Inherited >= oldValueSource)
                {
                    if (visitedViaVisualTree && typeof(FrameworkElement).IsInstanceOfType(d))
                    {
                        DependencyObject logicalParent = ((FrameworkElement)d).Parent;
                        if (logicalParent != null)
                        {
                            DependencyObject visualParent = VisualTreeHelper.GetParent(d);
                            if (visualParent != null && visualParent != logicalParent)
                            {
                                return false;
                            }
                        }
                    }

                    return d.SetInheritedValue(dp, info.NewValue, false);
                }
                else
                {
                    if (storage == null)
                    {
                        // get the storage if we didn't to it ealier.
                        INTERNAL_PropertyStore.TryGetInheritedPropertyStorage(d, dp, true, out storage);
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
        private static bool IsInheritanceNode(
            DependencyObject d,
            DependencyProperty dp)
        {
            PropertyMetadata metadata = dp.GetMetadata(d.GetType());
            if (metadata != null)
            {
                if (metadata.Inherits)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion InheritablePropertyChange

        #region StaticData

        private static VisitedCallback<InheritablePropertyChangeInfo> InheritablePropertyChangeDelegate
            = new VisitedCallback<InheritablePropertyChangeInfo>(OnInheritablePropertyChanged);

        #endregion StaticData
    }
}
