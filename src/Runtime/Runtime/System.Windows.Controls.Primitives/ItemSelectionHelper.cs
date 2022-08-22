// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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
        /// <summary>
        /// Name used for the SelectedItem property.
        /// </summary>
        internal const string SelectedItemName = "SelectedItem";

        /// <summary>
        /// Name used for the Items property.
        /// </summary>
        internal const string ItemsName = "Items";

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(SelectedItemName));
                }
            }
        }

        /// <summary>
        /// BackingField for the selected item.
        /// </summary>
        private T _selectedItem;

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Allowing the collection to be set from Xaml.")]
        public ObservableCollection<T> Items
        {
            get { return _items; }
            set
            {
                _items = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(ItemsName));
                }
            }
        }

        /// <summary>
        /// BackingField for Items.
        /// </summary>
        private ObservableCollection<T> _items;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
