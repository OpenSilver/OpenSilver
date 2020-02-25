#if WORKINPROGRESS

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
        //     Gets an empty and non-modifiable System.ComponentModel.SortDescriptionCollection.
        public static readonly SortDescriptionCollection Empty;

        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.SortDescriptionCollection
        //     class.
        public SortDescriptionCollection()
        {

        }

        //
        // Summary:
        //     Occurs when a System.ComponentModel.SortDescription is added or removed.
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