

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


using System;
using System.Collections;
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
    public partial class SelectionChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the SelectionChangedEventArgs class with the specified removed and added items.
        /// </summary>
        /// <param name="removedItems">A list of the elements that have been removed from the list.</param>
        /// <param name="addedItems">A list of the elements that have been added to the list.</param>
        public SelectionChangedEventArgs(IList removedItems, IList addedItems)
        {
            _addedItems = addedItems;
            _removedItems = removedItems;
        }

        IList _addedItems;
        IList _removedItems;

        // Returns:
        //     The loosely typed collection of items that were selected in this event.
        /// <summary>
        /// Gets a list that contains the items that were selected.
        /// </summary>
        public IList AddedItems { get { return _addedItems; } }

        // Returns:
        //     The loosely typed list of items that were unselected in this event.
        /// <summary>
        /// Gets a list that contains the items that were unselected.
        /// </summary>
        public IList RemovedItems { get { return _removedItems; } }
    }
}