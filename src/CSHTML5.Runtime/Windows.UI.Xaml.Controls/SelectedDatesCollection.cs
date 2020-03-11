

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
    public sealed partial class SelectedDatesCollection : ObservableCollection<DateTime>
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

#if WORKINPROGRESS
        private void RaiseSelectionChanged(IList removedItems, IList addedItems)
#else
        private void RaiseSelectionChanged(IList<object> removedItems, IList<object> addedItems)
#endif
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
