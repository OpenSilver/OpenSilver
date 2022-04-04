using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Exposes an ObservableCollection of T and
    /// a SelectedItem property for binding purposes.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <QualityBand>Preview</QualityBand>
    public class ItemSelectionHelper<T> : INotifyPropertyChanged
    {
        /// <summary>Name used for the SelectedItem property.</summary>
        internal const string SelectedItemName = "SelectedItem";
        /// <summary>Name used for the Items property.</summary>
        internal const string ItemsName = "Items";
        /// <summary>BackingField for the selected item.</summary>
        private T _selectedItem;
        /// <summary>BackingField for Items.</summary>
        private ObservableCollection<T> _items;

        /// <summary>Gets or sets the selected item.</summary>
        /// <value>The selected item.</value>
        public T SelectedItem
        {
            get
            {
                return this._selectedItem;
            }
            set
            {
                this._selectedItem = value;
                if (this.PropertyChanged == null)
                    return;
                this.PropertyChanged((object)this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }

        /// <summary>Gets or sets the items.</summary>
        /// <value>The items.</value>
        public ObservableCollection<T> Items
        {
            get
            {
                return this._items;
            }
            set
            {
                this._items = value;
                if (this.PropertyChanged == null)
                    return;
                this.PropertyChanged((object)this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
