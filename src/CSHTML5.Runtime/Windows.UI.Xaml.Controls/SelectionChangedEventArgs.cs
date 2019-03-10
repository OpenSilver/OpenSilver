
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    //[MarshalingBehavior(MarshalingType.Agile)]
    //[Threading(ThreadingModel.Both)]
    //[Version(100794368)]
    //[WebHostHidden]
    /// <summary>
    /// Provides data for the SelectionChanged event.
    /// </summary>
    public class SelectionChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the SelectionChangedEventArgs class.
        /// </summary>
        public SelectionChangedEventArgs() { }
        /// <summary>
        /// Initializes a new instance of the SelectionChangedEventArgs class with the specified removed and added items.
        /// </summary>
        /// <param name="removedItems">A list of the elements that have been removed from the list.</param>
        /// <param name="addedItems">A list of the elements that have been added to the list.</param>
        public SelectionChangedEventArgs(IList<object> removedItems, IList<object> addedItems)
        {
            _addedItems = addedItems;
            _removedItems = removedItems;
        }

        IList<object> _addedItems;
        // Returns:
        //     The loosely typed collection of items that were selected in this event.
        /// <summary>
        /// Gets a list that contains the items that were selected.
        /// </summary>
        public IList<object> AddedItems { get { return _addedItems; } }

        IList<object> _removedItems;
        // Returns:
        //     The loosely typed list of items that were unselected in this event.
        /// <summary>
        /// Gets a list that contains the items that were unselected.
        /// </summary>
        public IList<object> RemovedItems { get { return _removedItems; } }
    }
}