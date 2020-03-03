#if WORKINPROGRESS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace System.ComponentModel
{
    public partial class SortDescriptionCollection : Collection<SortDescription>, INotifyCollectionChanged
    {
        protected event NotifyCollectionChangedEventHandler CollectionChanged;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
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
    }
}

#endif