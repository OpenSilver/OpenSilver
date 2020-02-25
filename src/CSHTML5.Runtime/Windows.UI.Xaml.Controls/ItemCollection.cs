
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



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Holds the list of items that represent the content of an ItemsControl.
    /// </summary>
    /// <exclude/>
    public sealed partial class ItemCollection : ObservableCollection<object>
    {
        bool _suppressCollectionChanged;

        /// <summary>
        /// Initializes a new instance of the ItemCollection class with an empty collection.
        /// </summary>
        public ItemCollection() :base() { }

        /// <summary>
        /// Initializes a new instance of the ItemCollection class containing the elements of the list entered in the parameters.
        /// </summary>
        /// <param name="list">The list containing the initial elements.</param>
        public ItemCollection(List<object> list) : base(list) { }
        /// <summary>
        /// Initializes a new instance of the ItemCollection class containing the elements of the IEnumerable entered in the parameters.
        /// </summary>
        /// <param name="enumerable">The IEnumerable containing the initial elements.</param>
        public ItemCollection(IEnumerable<object> enumerable) : base(enumerable) { }
       
        // We override the "ClearItems" method so that the list of removed items is passed to the "CollectionChanged" event when some code calls ".Clear()".
        protected override void ClearItems()
        {
            _suppressCollectionChanged = true; // To avoid raising "CollectionChanged" twice.
            List<object> oldItems = new List<object>(this);
            base.ClearItems();
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressCollectionChanged)
                base.OnCollectionChanged(e);

            _suppressCollectionChanged = false;
        }
    }
}
