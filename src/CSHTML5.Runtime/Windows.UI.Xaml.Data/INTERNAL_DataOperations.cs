
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal partial class INTERNAL_Operations
    {
        public INTERNAL_Operations(PagedCollectionView collectionViewer)
        {
            _hasFilteringBeenDone = false;
            _sortOperationIndex = -1;
            _collectionViewer = collectionViewer;
        }

        // if the filtering is done
        bool _hasFilteringBeenDone;

        // index in sorting operations, allows to know where we are among the list of sorting operations, and determine which one is next
        int _sortOperationIndex;

        // return the true direction to use to fix the sorting cascade problem, which is that when there are multiple sort operations, all their directions are inversed, for unknown reason, probably due to the currently used sort algorithm.
        public ListSortDirection GetFixedDirection(PropertySortDescription operation)
        {
            int index = _collectionViewer.SortDescriptions.IndexOf(operation);

            if (index % 2 == 0 && _collectionViewer.SortDescriptions.Count > 1)
                return operation.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending; // invert

            return operation.Direction; // normal
        }

        PagedCollectionView _collectionViewer;

        // get the origin / first parent of all the viewGroups
        public PagedCollectionView Requester { get { return _collectionViewer; } }

        // check if the filtering has already been applied
        public bool HasFilteringBeenDone()
        {
            return _hasFilteringBeenDone;
        }

        // get the next operation to do, and mark it as handled
        public object HandleNextOperation(int level = 0)
        {
            if (_hasFilteringBeenDone)
            {
                if (_sortOperationIndex >= _collectionViewer.SortDescriptions.Count - 1)
                {
                    if (level < _collectionViewer.GroupDescriptions.Count)// make a grouping with that level
                    {
                        return _collectionViewer.GroupDescriptions[level];
                    }
                    else
                    {
                        return null; //no more operation
                    }
                }
                else // do the next sorting
                {
                    _sortOperationIndex++;
                    return _collectionViewer.SortDescriptions[_sortOperationIndex];
                }
            }
            else // do the filtering
            {
                _hasFilteringBeenDone = true;
                if (_collectionViewer._filter != null) // make sure there is really a filtering operation
                    return _collectionViewer._filter;
                else
                    return HandleNextOperation();
            }
        }
    }

    internal partial class FilterDescription
    {
        // Note: the user has two options for filtering: either by passing a predicate,
        // or by registering an event and setting e.Accepted to true or false in the
        // event handler.

        public FilterDescription(FilterEventHandler _filterUsingAndEvent) { FilterUsingAnEvent = _filterUsingAndEvent; }

        public FilterDescription(Predicate<Object> _filterUsingAPredicate) { FilterUsingAPredicate = _filterUsingAPredicate; }

        public FilterEventHandler FilterUsingAnEvent { get; private set; }

        public Predicate<Object> FilterUsingAPredicate { get; private set; }
    }

    /// <summary>
    /// Description of grouping based on a property value.
    /// </summary>
#if WORKINPROGRESS
    public partial class PropertyGroupDescription : GroupDescription
#else
    public partial class PropertyGroupDescription
#endif
    {
#if WORKINPROGRESS
        public string PropertyName { get; set; }
#else
        public string PropertyName { get; private set; }
#endif

        public PropertyGroupDescription()
        {

        }

        public PropertyGroupDescription(string propertyName)
        {
            PropertyName = propertyName;
        }
    }

    /// <summary>
    /// Defines a property and direction to sort a list by.
    /// </summary>
    public partial class PropertySortDescription
    {
        public string PropertyName { get; private set; }
        public ListSortDirection Direction { get; internal set; }
        public IComparer Comparer { get; set; } // Specifies a comparer for make a custom sorting //todo: comparers in JSIL do not work properly, so the method for doing custom sort is not implemented (the code is commented in CollectionGroupViewInternal).

        public PropertySortDescription(string propertyName, ListSortDirection direction, IComparer comparer = null)
        {
            PropertyName = propertyName;
            Direction = direction;
            Comparer = comparer;
        }
    }

    //todo: currently not supported:
    // in PropertySortDescription and PropertyGroupDescription the modifications are not auto updated to the pagedView
    // (but the modification of the lists in pagedView are auto updated)

}
