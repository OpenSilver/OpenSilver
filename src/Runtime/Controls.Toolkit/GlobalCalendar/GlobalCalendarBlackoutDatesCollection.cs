// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a collection of non-selectable dates in a
    /// <see cref="T:System.Windows.Controls.GlobalCalendar" />.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class GlobalCalendarBlackoutDatesCollection : ObservableCollection<CalendarDateRange>
    {
        /// <summary>
        /// The GlobalCalendar whose dates this object represents.
        /// </summary>
        private GlobalCalendar _owner;

        /// <summary>
        /// The dispatcher thread.
        /// </summary>
        private Thread _dispatcherThread;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.GlobalCalendarBlackoutDatesCollection" />
        /// class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="T:System.Windows.Controls.GlobalCalendar" /> whose dates
        /// this object represents.
        /// </param>
        public GlobalCalendarBlackoutDatesCollection(GlobalCalendar owner)
        {
            // TODO: This assert should be replaced with an exception.
            Debug.Assert(owner != null, "owner should not be null!");

            _owner = owner;
            _dispatcherThread = Thread.CurrentThread;
        }

        /// <summary>
        /// Adds all dates before <see cref="P:System.DateTime.Today" /> to the
        /// collection.
        /// </summary>
        public void AddDatesInPast()
        {
            Add(new CalendarDateRange(
                DateTime.MinValue,
                _owner.Info.AddDays(DateTime.Today, -1).Value));
        }

        /// <summary>
        /// Returns a value that represents whether this collection contains the
        /// specified date.
        /// </summary>
        /// <param name="date">The date to search for.</param>
        /// <returns>
        /// True if the collection contains the specified date; otherwise,
        /// false.
        /// </returns>
        public bool Contains(DateTime date)
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                if (InRange(this[i], date))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a value that represents whether this collection contains the
        /// specified range of dates.
        /// </summary>
        /// <param name="start">The start of the date range.</param>
        /// <param name="end">The end of the date range.</param>
        /// <returns>
        /// True if all dates in the range are contained in the collection;
        /// otherwise, false.
        /// </returns>
        public bool Contains(DateTime start, DateTime end)
        {
            DateTime rangeStart;
            DateTime rangeEnd;

            if (_owner.Info.Compare(end, start) > -1)
            {
                rangeStart = start.Date;
                rangeEnd = end.Date;
            }
            else
            {
                rangeStart = end.Date;
                rangeEnd = start.Date;
            }

            int count = Count;
            for (int i = 0; i < count; i++)
            {
                CalendarDateRange range = this[i];
                if (_owner.Info.Compare(range.Start, rangeStart) == 0 &&
                    _owner.Info.Compare(range.End, rangeEnd) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a value that represents whether this collection contains any
        /// date in the specified range.
        /// </summary>
        /// <param name="range">The range of dates to search for.</param>
        /// <returns>
        /// True if any date in the range is contained in the collection;
        /// otherwise, false.
        /// </returns>
        public bool ContainsAny(CalendarDateRange range)
        {
            return this.Any(r => GlobalCalendarExtensions.ContainsAny(r, range, _owner.Info));
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <remarks>
        /// This implementation raises the CollectionChanged event.
        /// </remarks>
        protected override void ClearItems()
        {
            EnsureValidThread();

            base.ClearItems();
            _owner.UpdateMonths();
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="item">The object to insert.</param>
        /// <remarks>
        /// This implementation raises the CollectionChanged event.
        /// </remarks>
        protected override void InsertItem(int index, CalendarDateRange item)
        {
            EnsureValidThread();

            if (!IsValid(item))
            {
                throw new ArgumentOutOfRangeException(Resource.Calendar_UnSelectableDates);
            }

            base.InsertItem(index, item);
            _owner.UpdateMonths();
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
        /// <remarks>
        /// This implementation raises the CollectionChanged event.
        /// </remarks>
        protected override void RemoveItem(int index)
        {
            EnsureValidThread();

            base.RemoveItem(index);
            _owner.UpdateMonths();
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to replace.
        /// </param>
        /// <param name="item">
        /// The new value for the element at the specified index.
        /// </param>
        /// <remarks>
        /// This implementation raises the CollectionChanged event.
        /// </remarks>
        protected override void SetItem(int index, CalendarDateRange item)
        {
            EnsureValidThread();

            if (!IsValid(item))
            {
                throw new ArgumentOutOfRangeException(Resource.Calendar_UnSelectableDates);
            }

            base.SetItem(index, item);
            _owner.UpdateMonths();
        }

        /// <summary>
        /// Returns a value indicating whether a date is included in the range.
        /// </summary>
        /// <param name="range">The range to check.</param>
        /// <param name="day">The date to check.</param>
        /// <returns>
        /// A value indicating whether a date is included in the range.
        /// </returns>
        private bool InRange(CalendarDateRange range, DateTime day)
        {
            Debug.Assert(range != null, "range should not be null!");
            Debug.Assert(_owner.Info.Compare(range.Start, range.End) < 1, "The range should start before it ends!");

            return _owner.Info.Compare(day, range.Start) > -1 &&
                _owner.Info.Compare(day, range.End) < 1;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="item">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private bool IsValid(CalendarDateRange item)
        {
            foreach (DateTime day in _owner.SelectedDates)
            {
                if (InRange(item, day))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void EnsureValidThread()
        {
            if (Thread.CurrentThread != _dispatcherThread)
            {
                throw new NotSupportedException(Resource.CalendarCollection_MultiThreadedCollectionChangeNotSupported);
            }
        }
    }
}