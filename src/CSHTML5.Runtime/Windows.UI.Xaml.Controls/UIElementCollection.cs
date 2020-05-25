

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
    public partial class UIElementCollection : ObservableCollection<UIElement>
    {
        bool _suppressCollectionChanged;


        public UIElementCollection() { }


        // We override the "ClearItems" method so that the list of removed items is passed to the "CollectionChanged" event when some code calls ".Clear()".
        protected override void ClearItems()
        {
            _suppressCollectionChanged = true; // To avoid raising "CollectionChanged" twice.
            List<UIElement> oldItems = new List<UIElement>(this);
            try
            {
                base.ClearItems();
            }
            finally
            {
                _suppressCollectionChanged = false;
            }
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressCollectionChanged)
                base.OnCollectionChanged(e);
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
