
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
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace OpenSilver.Controls
{
    public sealed class SelectedDatesCollection : ObservableCollection<DateTime>
    {
        private readonly Collection<object> _addedItems;
        private readonly Collection<object> _removedItems;

        private readonly INTERNAL_CalendarOrClockBase _owner;

        /// <summary>
        /// Initializes a new instance of the CalendarSelectedDatesCollection class.
        /// </summary>
        /// <param name="owner"></param>
        public SelectedDatesCollection(INTERNAL_CalendarOrClockBase owner)
        {
            _owner = owner;
            _addedItems = new Collection<object>();
            _removedItems = new Collection<object>();
        }

        private void RaiseSelectionChanged(IList removedItems, IList addedItems)
        {
            _owner.OnSelectedDatesCollectionChanged(new SelectionChangedEventArgs(removedItems, addedItems));
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
    }
}
