﻿
using System.Collections;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internals.Controls
{
    internal interface IGeneratorHost
    {
        IList View { get; }

        void ClearContainerForItem(DependencyObject container, object item);
        DependencyObject GetContainerForItem(object item, DependencyObject recycledContainer);
        bool IsHostForItemContainer(DependencyObject container);
        bool IsItemItsOwnContainer(object item);
        void PrepareItemContainer(DependencyObject container, object item);
    }
}
