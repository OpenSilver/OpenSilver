// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace System.Windows.Interactivity
{
    using System.Collections.Generic;

#if MIGRATION
    using System.Windows;
    using System.Windows.Media;
#else
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Media;
#endif

    public static class DependencyObjectHelper
    {
        /// <summary>
        /// This method will use the VisualTreeHelper.GetParent method to do a depth first walk up 
        /// the visual tree and return all ancestors of the specified object, including the object itself.
        /// </summary>
        /// <param name="dependencyObject">The object in the visual tree to find ancestors of.</param>
        /// <returns>Returns itself an all ancestors in the visual tree.</returns>
        public static IEnumerable<DependencyObject> GetSelfAndAncestors(this DependencyObject dependencyObject)
        {
            // Walk up the visual tree looking for the element.
            while (dependencyObject != null)
            {
                yield return dependencyObject;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
        }
    }
}
