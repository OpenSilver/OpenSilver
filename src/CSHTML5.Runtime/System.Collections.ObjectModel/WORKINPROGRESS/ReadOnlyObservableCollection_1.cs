#if WORKINPROGRESS

using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Collections.ObjectModel
{

    public class ReadOnlyObservableCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Collections.ObjectModel.ReadOnlyObservableCollection`1
        //     class that serves as a wrapper for the specified System.Collections.ObjectModel.ObservableCollection`1.
        //
        // Parameters:
        //   list:
        //     The collection to wrap.
        public ReadOnlyObservableCollection(ObservableCollection<T> list) : base(list)
        {

        }

        //
        // Summary:
        //     Occurs when an item is added or removed.
        protected event NotifyCollectionChangedEventHandler CollectionChanged;
        //
        // Summary:
        //     Occurs when a property value changes.
        protected event PropertyChangedEventHandler PropertyChanged;

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

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
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
        //     Raises the System.Collections.ObjectModel.ReadOnlyObservableCollection`1.CollectionChanged
        //     event.
        //
        // Parameters:
        //   args:
        //     The event data.
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {

        }
        //
        // Summary:
        //     Raises the System.Collections.ObjectModel.ReadOnlyObservableCollection`1.PropertyChanged
        //     event.
        //
        // Parameters:
        //   args:
        //     The event data.
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {

        }
    }
}

#endif