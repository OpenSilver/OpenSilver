using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Windows.Controls
{
    [OpenSilver.NotImplemented]
    public class ObservableObjectCollection : ObservableCollection<object>, ICollection<object>, IEnumerable<object>, IEnumerable
    {
        [OpenSilver.NotImplemented]
        public ObservableObjectCollection() { }
   
        [OpenSilver.NotImplemented]
        public ObservableObjectCollection(IEnumerable collection) { }

        [OpenSilver.NotImplemented]
        public bool IsReadOnly { get; }

        [OpenSilver.NotImplemented]
        protected override void ClearItems() { }

        [OpenSilver.NotImplemented]
        protected override void InsertItem(int index, object item) { }

        [OpenSilver.NotImplemented]
        protected override void RemoveItem(int index) { }

        [OpenSilver.NotImplemented]
        protected override void SetItem(int index, object item) { }
    }
}