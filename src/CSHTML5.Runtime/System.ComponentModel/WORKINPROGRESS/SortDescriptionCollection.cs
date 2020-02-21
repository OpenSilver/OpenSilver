#if WORKINPROGRESS
using System;
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
    }
}

#endif