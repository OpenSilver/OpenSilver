

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


#if WORKINPROGRESS

using System.Collections;

namespace System.ComponentModel
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    //
    // Summary:
    //     Represents a collection of System.ComponentModel.SortDescription instances.
    public class SortDescriptionCollection : Collection<SortDescription>, INotifyCollectionChanged
    {
        //
        // Summary:
        //     Occurs when a System.ComponentModel.SortDescription is added or removed.
        protected event NotifyCollectionChangedEventHandler CollectionChanged;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add => CollectionChanged += value;
            remove => CollectionChanged -= value;
        }

        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.SortDescriptionCollection
        //     class.
        public SortDescriptionCollection()
        {

        }

        /// <summary>
        /// Immutable, read-only SortDescriptionCollection
        /// </summary>
        private class EmptySortDescriptionCollection : SortDescriptionCollection, IList
        {
            /// <summary>
            /// called by base class Collection&lt;T&gt; when the list is being cleared;
            /// raises a CollectionChanged event to any listeners
            /// </summary>
            protected override void ClearItems()
            {
                throw new NotSupportedException();
            }
 
            /// <summary>
            /// called by base class Collection&lt;T&gt; when an item is removed from list;
            /// raises a CollectionChanged event to any listeners
            /// </summary>
            protected override void RemoveItem(int index)
            {
                throw new NotSupportedException();
            }
 
            /// <summary>
            /// called by base class Collection&lt;T&gt; when an item is added to list;
            /// raises a CollectionChanged event to any listeners
            /// </summary>
            protected override void InsertItem(int index, SortDescription item)
            {
                throw new NotSupportedException();
            }
 
            /// <summary>
            /// called by base class Collection&lt;T&gt; when an item is set in list;
            /// raises a CollectionChanged event to any listeners
            /// </summary>
            protected override void SetItem(int index, SortDescription item)
            {
                throw new NotSupportedException();
            }
 
            // explicit implementation to override the IsReadOnly and IsFixedSize properties
 
            bool IList.IsFixedSize => true;

            bool IList.IsReadOnly => true;
        }
        
        /// <summary>
        /// returns an empty and non-modifiable SortDescriptionCollection
        /// </summary>
        public static readonly SortDescriptionCollection Empty = new EmptySortDescriptionCollection();

        //
        // Summary:
        //     Removes all System.ComponentModel.SortDescription instances from the collection.
        protected override void ClearItems()
        {

        }
        //
        // Summary:
        //     Inserts a System.ComponentModel.SortDescription into the collection at the specified
        //     index.
        //
        // Parameters:
        //   index:
        //     The zero-based index where the System.ComponentModel.SortDescription is inserted.
        //
        //   item:
        //     The System.ComponentModel.SortDescription to insert.
        protected override void InsertItem(int index, SortDescription item)
        {

        }
        //
        // Summary:
        //     Removes the System.ComponentModel.SortDescription at the specified index in the
        //     collection.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the System.ComponentModel.SortDescription to remove.
        protected override void RemoveItem(int index)
        {

        }
        //
        // Summary:
        //     Replaces the System.ComponentModel.SortDescription at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the System.ComponentModel.SortDescription to replace.
        //
        //   item:
        //     The new value for the System.ComponentModel.SortDescription at the specified
        //     index.
        protected override void SetItem(int index, SortDescription item)
        {

        }
    }
}

#endif