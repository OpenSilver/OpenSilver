
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public sealed class SelectedDatesCollection : ObservableCollection<DateTime>
    {
        private Collection<object> _addedItems;
        private Collection<object> _removedItems;

        private INTERNAL_CalendarOrClockBase _owner;

        /// <summary>
        /// Initializes a new instance of the CalendarSelectedDatesCollection class.
        /// </summary>
        /// <param name="owner"></param>
        public SelectedDatesCollection(INTERNAL_CalendarOrClockBase owner)
        {
            this._owner = owner;
            this._addedItems = new Collection<object>();
            this._removedItems = new Collection<object>();
        }

        private void RaiseSelectionChanged(IList<object> removedItems, IList<object> addedItems)
        {
            this._owner.OnSelectedDatesCollectionChanged(new SelectionChangedEventArgs(removedItems, addedItems));
        }

        /// <summary>
        /// Special method for using only one date
        /// </summary>
        /// <param name="item"></param>
        public void SetDate(DateTime item)
        {
            _addedItems.Clear();
            _removedItems.Clear();

            _addedItems.Add(item);

            RaiseSelectionChanged(_removedItems, _addedItems);
        }

        //todo : Add / Remove / Set ...
    }
}
