
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


#if MIGRATION
namespace System.Collections.ObjectModel
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// A class that holds a selected item.
    /// </summary>
    public sealed class Selection
    {
        /// <summary>
        /// Initializes an instance of the Selection class.
        /// </summary>
        /// <param name="index">The index of the selected item within the 
        /// source collection.</param>
        /// <param name="item">The selected item.</param>
        public Selection(int? index, object item)
        {
            this.Index = index;
            this.Item = item;
        }

        /// <summary>
        /// Initializes an instance of the Selection class.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public Selection(object item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the index of the selection within the source collection.
        /// </summary>
        public int? Index { get; internal set; }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public object Item { get; internal set; }
    }
}