
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
        /// Initializes a new instance of the SelectionChangedEventArgs class.
        /// </summary>
        public SelectionChangedEventArgs() : this(new List<object>(), new List<object>())
        {

        }
        /// <summary>
        /// Initializes a new instance of the SelectionChangedEventArgs class with the specified removed and added items.
        /// </summary>
        /// <param name="removedItems">A list of the elements that have been removed from the list.</param>
        /// <param name="addedItems">A list of the elements that have been added to the list.</param>
#if WORKINPROGRESS
        public SelectionChangedEventArgs(IList removedItems, IList addedItems)
#else
        public SelectionChangedEventArgs(IList<object> removedItems, IList<object> addedItems)
#endif
        {
            _addedItems = addedItems;
            _removedItems = removedItems;
        }

#if WORKINPROGRESS
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
#else
        IList<object> _addedItems;
        IList<object> _removedItems;

        // Returns:
        //     The loosely typed collection of items that were selected in this event.
        /// <summary>
        /// Gets a list that contains the items that were selected.
        /// </summary>
        public IList<object> AddedItems { get { return _addedItems; } }

        // Returns:
        //     The loosely typed list of items that were unselected in this event.
        /// <summary>
        /// Gets a list that contains the items that were unselected.
        /// </summary>
        public IList<object> RemovedItems { get { return _removedItems; } }
#endif
    }
}