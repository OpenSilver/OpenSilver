
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace System.Windows.Data
{
    /// <summary>
    /// The class that creates a tree results based on the operations requested by a PagedCollectionView.
    /// This is the class that can filter, sort and group (not paged, because pages are handled PagedCollectionView).
    /// </summary>
    internal partial class INTERNAL_CollectionViewGroupInternal //: INotifyPropertyChanged
    {
        // Parent has lower priority on sort and other operations
        INTERNAL_CollectionViewGroupInternal _parentView;

        // Allow to get the operations that must be done on the data
        INTERNAL_Operations _operations;

        // The grouping level of the view
        int _level;

        // Get if this view is a leaf
        public bool IsLeaf { get; private set; }

        // Define a sub-source for a sub-group
        public ICollection<object> Items { get; private set; }

        // Take the result of the parent and generate the result for the next operation
        internal INTERNAL_CollectionViewGroupInternal(ICollection<object> source, INTERNAL_CollectionViewGroupInternal parent, INTERNAL_Operations operations, int level = 0)
        {
            Items = source;
            _level = level;
            _parentView = parent;
            _operations = operations;

            object operationToDo = _operations.HandleNextOperation(_level);

            ExecuteOperation(operationToDo);
        }

        // Execute the next operation requested in the operations list
        void ExecuteOperation(object operation)
        {
            if (operation != null)
            {
                IsLeaf = false;

                if (operation is FilterDescription)
                {
                    FilterDescription filterOperation = (FilterDescription)operation;

                    // both cannot be null and both cannot be not null
                    if (filterOperation.FilterUsingAnEvent != null)
                        Filter(filterOperation.FilterUsingAnEvent);
                    else
                        Filter(filterOperation.FilterUsingAPredicate);
                }
                else if (operation is PropertySortDescription)
                {
                    PropertySortDescription sortOperation = (PropertySortDescription)operation;

                    if (sortOperation.Comparer == null)
                        Sort(sortOperation);
                    else
                        CustomSort(sortOperation);

                }
                else if (operation is INTERNAL_PropertyGroupDescription)
                {
                    INTERNAL_PropertyGroupDescription groupOperation = (INTERNAL_PropertyGroupDescription)operation;

                    GroupBy(groupOperation);
                }
                else
                {
                    throw new InvalidOperationException("unknow operation type");
                }
            }
            else // if the operation to do on this view is null, that mean we have reach the end of the operation list and this view is usable as data source
            {
                IsLeaf = true;
            }
        }

        // Filter the data source with a predicate
        void Filter(Predicate<Object> predicate)
        {
            // Create a new collection with the result of the filtering:
            Collection<object> childItems = new Collection<object>();

            foreach (object obj in Items)
            {
                if (predicate(obj))
                {
                    childItems.Add(obj);
                }
            }

            // Pass the result of the filtering to a child CollectionViewGroup in order to apply other operations (such as sort) if any:
            INTERNAL_CollectionViewGroupInternal newView = new INTERNAL_CollectionViewGroupInternal(childItems, this, _operations);
            _operations.Requester.AddView(newView); // create child branch
        }


        // Filter the data source using an event (the user sets e.Accepted to true or false in the event handler):
        void Filter(FilterEventHandler logicMethod)
        {
            // Create a new collection with the result of the filtering:
            Collection<object> childItems = new Collection<object>();

            foreach (object obj in Items)
            {
                FilterEventArgs args = new FilterEventArgs(obj);

                logicMethod(this, args);

                if (args.Accepted)
                {
                    childItems.Add(obj);
                }
            }

            // Pass the result of the filtering to a child CollectionViewGroup in order to apply other operations (such as sort) if any:
            INTERNAL_CollectionViewGroupInternal newView = new INTERNAL_CollectionViewGroupInternal(childItems, this, _operations);
            _operations.Requester.AddView(newView); // create child branch
        }

        // Verify if the data that will be sorted can be compared
        bool VerifyComparable(PropertySortDescription operation)
        {
            if (operation.Comparer == null) // not null here means that the user wants to use a custom comparer
            {
                if (Items.Count != 0)
                {
                    Type type = GetValue(Items.ElementAt(0), operation).GetType();

                    if (typeof(IComparable).IsAssignableFrom(type))
                        return true;
                }   
                return false;
            }

            throw new NotImplementedException("Custom Sorting is not supported yet");
        }

        void CustomSort(PropertySortDescription operation)
        {
            throw new NotImplementedException("Custom Sorting is not supported yet");
        }

        // Sort the data source with a default comparer
        void Sort(PropertySortDescription operation)
        {
            if (VerifyComparable(operation))// if we can't compare data, we don't try to sort
            {
                ListSortDirection directionBackup = operation.Direction;

                operation.Direction = _operations.GetFixedDirection(operation);

                Collection<object> childItems = new Collection<object>();

                //todo: replace with a sort algorithm that is O(NLogN), for example by creating a List<T> and calling List<T>.Sort(comparer).

                foreach (object item in Items)
                {
                    // we get the value of the sorting property on this item to allow comparison (if we are here, we know that data is comparable)
                    // then we get the position where we need to insert this item
                    IComparable value = (IComparable)GetValue(item, operation);
                    int index = GetPosIndex_SimulatorOnly(operation, value, 0, childItems.Count, childItems);

                    childItems.Insert(index, item);
                }

                operation.Direction = directionBackup;

                // Pass the result of the sort to a child CollectionViewGroup in order to apply other operations (such as sort) if any:
                INTERNAL_CollectionViewGroupInternal newView = new INTERNAL_CollectionViewGroupInternal(childItems, this, _operations);
                _operations.Requester.AddView(newView); // create child branch after sorting operation
            }
            else
            {
                INTERNAL_CollectionViewGroupInternal newView = new INTERNAL_CollectionViewGroupInternal(Items, this, _operations);
                _operations.Requester.AddView(newView); // create child branch without sorting
            }
        }


        // Get the position where the x object must be inserted in the data source of this view (simple dichotomic sorting)
        int GetPosIndex_SimulatorOnly(PropertySortDescription operation, IComparable x, int min, int max, Collection<object> childItems)
        {
            if (min == max) // recursive end condition
                return min;
            else
            {
                int middle = ((max - min) / 2) + min;

                object childValue = GetValue(childItems[middle], operation);

                if ( (operation.Direction == ListSortDirection.Ascending ? 1 : -1) * x.CompareTo(childValue) > 0) // the ternaire operation allow to swith the comparation behaviour
                    return GetPosIndex_SimulatorOnly(operation, x, middle + 1, max, childItems); // +1 to fix the rounded int of middle
                else
                    return GetPosIndex_SimulatorOnly(operation, x, min, middle, childItems);
            }
        }

        // Get the position where the x object must be inserted in the data source of this view (simple dichotomic sorting)
        int GetPosIndex_JSOnly(PropertySortDescription operation, dynamic x, int min, int max, Collection<object> childItems)
        {
            if (min == max) // recursive end condition
                return min;
            else
            {
                int middle = ((max - min) / 2) + min;

                dynamic childValue = GetValue(childItems[middle], operation);

                if (operation.Direction == ListSortDirection.Ascending ? (x > childValue) : (x < childValue))
                    return GetPosIndex_JSOnly(operation, x, middle + 1, max, childItems); // +1 to fix the rounded int of middle
                else
                    return GetPosIndex_JSOnly(operation, x, min, middle, childItems);
            }
        }

        // Generate sub-group based on property equality
        void GroupBy(INTERNAL_PropertyGroupDescription operation)
        {
            List<Collection<object>> childGroups = new List<Collection<object>>();

            foreach(object item in Items)
            {
                bool find = false;

                foreach(Collection<object> child in childGroups)
                {
                    if (Object.Equals(GetValue(child[0], operation), GetValue(item, operation)))
                    {
                        child.Add(item);
                        find = true;
                        break;
                    }
                }

                if(!find)
                {
                    childGroups.Add(new Collection<object>());
                    childGroups[childGroups.Count - 1].Add(item);
                }
            }

            foreach(Collection<object> child in childGroups)
            {
                INTERNAL_CollectionViewGroupInternal newView = new INTERNAL_CollectionViewGroupInternal(child, this, _operations, _level + 1);
                _operations.Requester.AddView(newView); // create child branch (level matter in grouping views)
            }
        }

        // Get the value of the property described in operation by reflection
        object GetValue(object obj, PropertySortDescription operation)
        {
            return obj.GetType().GetProperty(operation.PropertyName).GetValue(obj, null);
        }

        // Get the value of the property described in operation by reflection
        object GetValue(object obj, INTERNAL_PropertyGroupDescription operation)
        {
            return obj.GetType().GetProperty(operation.PropertyName).GetValue(obj, null);
        }
    }



}
