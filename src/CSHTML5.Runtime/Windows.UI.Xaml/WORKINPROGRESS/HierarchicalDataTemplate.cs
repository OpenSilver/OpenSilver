
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    #region Not implemented yet
    public class HierarchicalDataTemplate : DataTemplate
    {
        /// <summary>
        /// Gets or sets the binding that is used to generate content for the next sublevel in the data hierarchy.
        /// </summary>
        public Binding ItemsSource { get; set; }

        /// <summary>
        /// Gets or sets the System.Windows.DataTemplate to apply to the System.Windows.Controls.ItemsControl.ItemTemplate property 
        /// on a generated System.Windows.Controls.HeaderedItemsControl, such as a System.Windows.Controls.TreeViewItem, to indicate
        /// how to display items in the next sublevel in the data hierarchy.
        /// </summary>
        public DataTemplate ItemTemplate { set; get; }
    }
    #endregion
#endif
}
