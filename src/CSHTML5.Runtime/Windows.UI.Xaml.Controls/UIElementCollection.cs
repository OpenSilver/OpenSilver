
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
    /// <exclude/>
    public class UIElementCollection : ObservableCollection<UIElement>
    {
        bool _suppressCollectionChanged;


        public UIElementCollection() { }


        // We override the "ClearItems" method so that the list of removed items is passed to the "CollectionChanged" event when some code calls ".Clear()".
        protected override void ClearItems()
        {
            _suppressCollectionChanged = true; // To avoid raising "CollectionChanged" twice.
            List<UIElement> oldItems = new List<UIElement>(this);
            base.ClearItems();
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressCollectionChanged)
                base.OnCollectionChanged(e);

            _suppressCollectionChanged = false;
        }

#if WORKINPROGRESS
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }
#endif
    }
}
