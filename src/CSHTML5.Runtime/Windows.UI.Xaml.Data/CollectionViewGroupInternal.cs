﻿
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// The class that creates a tree results based on the operations requested by a PagedCollectionView.
    /// This is the class that can filter, sort and group (not paged, because pages are handled PagedCollectionView).
    /// </summary>
    internal partial class CollectionViewGroupInternal //: INotifyPropertyChanged
    {
        // Parent has lower priority on sort and other operations
        CollectionViewGroupInternal _parentView;

        // Allow to get the operations that must be done on the data
        INTERNAL_Operations _operations;

        // The grouping level of the view
        int _level;

        // Get if this view is a leaf
        public bool IsLeaf { get; private set; }

        // Define a sub-source for a sub-group
        public ICollection<object> Items { get; private set; }

        // Take the result of the parent and generate the result for the next operation
        internal CollectionViewGroupInternal(ICollection<object> source, CollectionViewGroupInternal parent, INTERNAL_Operations operations, int level = 0)
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
                else if (operation is PropertyGroupDescription)
                {
                    PropertyGroupDescription groupOperation = (PropertyGroupDescription)operation;

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
            CollectionViewGroupInternal newView = new CollectionViewGroupInternal(childItems, this, _operations);
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
            CollectionViewGroupInternal newView = new CollectionViewGroupInternal(childItems, this, _operations);
            _operations.Requester.AddView(newView); // create child branch
        }

        // Verify if the data that will be sorted can be compared
        bool VerifyComparable(PropertySortDescription operation)
        {
            if (operation.Comparer == null) // not null here means that the user wants to use a custom comparer
            {
                if (Items.Count != 0)
                {
                    if(CSHTML5.Interop.IsRunningInTheSimulator)
                    {
                        Type type = GetValue(Items.ElementAt(0), operation).GetType();

                        if (typeof(IComparable).IsAssignableFrom(type))
                            return true;
                    }
                    else
                    {
                        //--------------------
                        // HACK: in JavaScript, since Comparer does not work properly in JSIL, we attempt to compare dynamic values and see if it passes the try/catch.
                        //--------------------

                        // We try to do a comparaison between the same object to see if js allows comparison (fail has not been tested...)
                        try 
                        {
                            // basics types
                            dynamic value1ToCompare = GetValue(Items.ElementAt(0), operation);
                            dynamic value2ToCompare = value1ToCompare;

                            if (value1ToCompare > value2ToCompare) { }

                            return true;
                        }
                        catch
                        {
                            /*
                            // custom types (see also __comparer__ in jsil)
                            Type type = GetValue(Items.ElementAt(0), operation).GetType();

                            foreach (MethodInfo method in type.GetMethods())
                            {
                                if (method.Name == "CompareTo")
                                    return true;
                            }
                            */
                             
                            return false;
                        }
                    }
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
                    int index;

                    // we get the value of the sorting property on this item to allow comparison (if we are here, we know that data is comparable)
                    // then we get the position where we need to insert this item
                    if (CSHTML5.Interop.IsRunningInTheSimulator)
                    {
                        IComparable value = (IComparable)GetValue(item, operation);

                        index = GetPosIndex_SimulatorOnly(operation, value, 0, childItems.Count, childItems);
                    }
                    else
                    {
                        dynamic value = GetValue(item, operation);

                        index = GetPosIndex_JSOnly(operation, value, 0, childItems.Count, childItems);
                    }

                    childItems.Insert(index, item);
                }

                operation.Direction = directionBackup;

                // Pass the result of the sort to a child CollectionViewGroup in order to apply other operations (such as sort) if any:
                CollectionViewGroupInternal newView = new CollectionViewGroupInternal(childItems, this, _operations);
                _operations.Requester.AddView(newView); // create child branch after sorting operation
            }
            else
            {
                CollectionViewGroupInternal newView = new CollectionViewGroupInternal(Items, this, _operations);
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
        void GroupBy(PropertyGroupDescription operation)
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
                CollectionViewGroupInternal newView = new CollectionViewGroupInternal(child, this, _operations, _level + 1);
                _operations.Requester.AddView(newView); // create child branch (level matter in grouping views)
            }
        }

        // Get the value of the property described in operation by reflection
        object GetValue(object obj, PropertySortDescription operation)
        {
            return obj.GetType().GetProperty(operation.PropertyName).GetValue(obj, null);
        }

        // Get the value of the property described in operation by reflection
        object GetValue(object obj, PropertyGroupDescription operation)
        {
            return obj.GetType().GetProperty(operation.PropertyName).GetValue(obj, null);
        }
    }



}
