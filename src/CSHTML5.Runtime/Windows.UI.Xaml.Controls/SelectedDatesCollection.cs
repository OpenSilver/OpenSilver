
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
