
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    }
}
