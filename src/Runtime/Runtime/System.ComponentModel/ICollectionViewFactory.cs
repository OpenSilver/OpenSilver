// Copyright (C) 2003 by Microsoft Corporation.  All rights reserved.

namespace System.ComponentModel
{
    /// <summary>
    /// Defines a method that enables a collection to provide a custom view for specialized
    /// sorting, filtering, grouping, and currency.
    /// </summary>
    public partial interface ICollectionViewFactory
    {
        /// <summary>
        /// Returns a custom view for specialized sorting, filtering, grouping, and currency.
        /// </summary>
        /// <returns>
        /// A custom view for specialized sorting, filtering, grouping, and currency.
        /// </returns>
        ICollectionView CreateView();
    }
}
