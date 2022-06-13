

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
using System.Collections.ObjectModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a collection of non-selectable dates in a <see cref="T:System.Windows.Controls.Calendar" />.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class CalendarBlackoutDatesCollection : ObservableCollection<CalendarDateRange>
    {
        /// <summary>
        /// Adds all dates before <see cref="P:System.DateTime.Today" /> to the collection.
        /// </summary>
        [OpenSilver.NotImplemented]
        public void AddDatesInPast() => Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));

        [OpenSilver.NotImplemented]
        public bool Contains(DateTime date) { return true; }

        [OpenSilver.NotImplemented]
        public bool Contains(DateTime start, DateTime end) { return true; }

        [OpenSilver.NotImplemented]
        public bool ContainsAny(CalendarDateRange range) { return true; }

        [OpenSilver.NotImplemented]
        protected override void ClearItems() { }

        [OpenSilver.NotImplemented]
        protected override void InsertItem(int index, CalendarDateRange item) { }

        [OpenSilver.NotImplemented]
        protected override void RemoveItem(int index) { }

        [OpenSilver.NotImplemented]
        protected override void SetItem(int index, CalendarDateRange item) { }
    }
}
