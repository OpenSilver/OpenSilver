// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Globalization = System.Globalization;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.Column", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.ColumnSpan", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.ContainingGrid", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.Row", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.RowSpan", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.IInvokeProvider.Invoke()", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.AddToSelection()", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.IsSelected", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.RemoveFromSelection()", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.Select()", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.SelectionContainer", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.ITableItemProvider.GetColumnHeaderItems()", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer.#System.Windows.Automation.Provider.ITableItemProvider.GetRowHeaderItems()", Justification = "WPF Compatibility")]

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes
    /// <see cref="T:System.Windows.Controls.Primitives.CalendarDayButton" />
    /// types to UI Automation.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class CalendarDayButtonAutomationPeer :
        FrameworkElementAutomationPeer,
        IGridItemProvider,
        IInvokeProvider,
        ISelectionItemProvider,
        ITableItemProvider
    {
        /// <summary>
        /// Gets the CalendarDayButton instance that is associated with this
        /// CalendarDayButtonAutomationPeer.
        /// </summary>
        private CalendarDayButton OwningCalendarDayButton
        {
            get { return Owner as CalendarDayButton; }
        }

        /// <summary>
        /// Gets the Calendar associated with the button.
        /// </summary>
        private Calendar OwningCalendar
        {
            get { return OwningCalendarDayButton.Owner; }
        }

        /// <summary>
        /// Gets the automation peer for the Calendar associated with the
        /// button.
        /// </summary>
        private IRawElementProviderSimple OwningCalendarAutomationPeer
        {
            get
            {
                Calendar calendar = OwningCalendar;
                if (calendar != null)
                {
                    AutomationPeer peer = CreatePeerForElement(calendar);
                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the ordinal number of the column that contains the cell or
        /// item.
        /// </summary>
        /// <value>
        /// A zero-based ordinal number that identifies the column that contains
        /// the cell or item.
        /// </value>
        int IGridItemProvider.Column
        {
            get { return (int)OwningCalendarDayButton.GetValue(Grid.ColumnProperty); }
        }

        /// <summary>
        /// Gets the number of columns that are spanned by a cell or item.
        /// </summary>
        /// <value>
        /// The number of columns.
        /// </value>
        int IGridItemProvider.ColumnSpan
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets a UI Automation provider that implements
        /// <see cref="T:System.Windows.Automation.Provider.IGridProvider" />
        /// and that represents the container of the cell or item.
        /// </summary>
        /// <value>
        /// The UI Automation provider.
        /// </value>
        IRawElementProviderSimple IGridItemProvider.ContainingGrid
        {
            get { return OwningCalendarAutomationPeer; }
        }

        /// <summary>
        /// Gets the ordinal number of the row that contains the cell or item.
        /// </summary>
        /// <value>
        /// A zero-based ordinal number that identifies the row that contains
        /// the cell or item.
        /// </value>
        int IGridItemProvider.Row
        {
            get
            {
                // we decrement the Row value by one since the first row is
                // composed of DayTitles
                Debug.Assert((int)OwningCalendarDayButton.GetValue(Grid.RowProperty) > 0, "Row should be greater than 0");
                return (int)OwningCalendarDayButton.GetValue(Grid.RowProperty) - 1;
            }
        }

        /// <summary>
        /// Gets the number of rows that are spanned by a cell or item.
        /// </summary>
        /// <value>The number of rows.</value>
        int IGridItemProvider.RowSpan
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets a value indicating whether an item is selected.
        /// </summary>
        /// <value>True if the element is selected; otherwise, false.</value>
        bool ISelectionItemProvider.IsSelected
        {
            get { return OwningCalendarDayButton.IsSelected; }
        }

        /// <summary>
        /// Gets the UI Automation provider that implements
        /// <see cref="T:System.Windows.Automation.Provider.ISelectionProvider" />
        /// and that acts as the container for the calling object.
        /// </summary>
        /// <value>The UI Automation provider.</value>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get { return OwningCalendarAutomationPeer; }
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer" />
        /// class.
        /// </summary>
        /// <param name="owner">
        /// The
        /// <see cref="T:System.Windows.Controls.Primitives.CalendarDayButton" />
        /// instance that is associated with this
        /// <see cref="T:System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer" />.
        /// </param>
        public CalendarDayButtonAutomationPeer(CalendarDayButton owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control pattern implementation for this
        /// <see cref="T:System.Windows.Automation.Peers.CalendarDayButtonAutomationPeer" />.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the
        /// specified pattern interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if ((patternInterface == PatternInterface.GridItem ||
                 patternInterface == PatternInterface.Invoke ||
                 patternInterface == PatternInterface.SelectionItem ||
                 patternInterface == PatternInterface.TableItem) &&
                OwningCalendar != null)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Returns the control type for the CalendarDayButton that is
        /// associated with this CalendarDayButtonAutomationPeer.  This method
        /// is called by GetAutomationControlType.
        /// </summary>
        /// <returns>A value of the AutomationControlType enumeration.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <summary>
        /// Returns the localized version of the control type for the owner type
        /// that is associated with this CalendarDayButtonAutomationPeer.
        /// </summary>
        /// <returns>The string that contains the type of control.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            //todo: replace with resource CalendarAutomationPeer_DayButtonLocalizedControlType
            return "day button";
        }

        /// <summary>
        /// Returns the string that describes the functionality of the
        /// CalendarDayButton that is associated with this
        /// CalendarDayButtonAutomationPeer.  This method is called by
        /// GetHelpText.
        /// </summary>
        /// <returns>
        /// The help text, or String.Empty if there is no help text.
        /// </returns>
        protected override string GetHelpTextCore()
        {
            CalendarDayButton button = OwningCalendarDayButton;
            if (button != null && button.DataContext != null && button.DataContext is DateTime)
            {
                DateTime dataContext = (DateTime)OwningCalendarDayButton.DataContext;
                Globalization.DateTimeFormatInfo info = DateTimeHelper.GetCurrentDateFormat();

                return !button.IsBlackout ?
                    dataContext.Date.ToString(info.LongDatePattern, info) :
                    string.Format(info, "Blackout Day - {0}", dataContext.Date.ToString(info.LongDatePattern, info));
            }

            return base.GetHelpTextCore();
        }

        /// <summary>
        /// Returns the name of the CalendarDayButton that is associated with
        /// this CalendarDayButtonAutomationPeer.  This method is called by
        /// GetClassName.
        /// </summary>
        /// <returns>
        /// The name of the owner type that is associated with this
        /// CalendarDayButtonAutomationPeer.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Returns the text label of the CalendarDayButton that is associated
        /// with this CalendarDayButtonAutomationPeer. This method is called by
        /// GetName.
        /// </summary>
        /// <returns>
        /// The text label of the element that is associated with this
        /// automation peer.
        /// </returns>
        /// <remarks>
        /// The name property can be thought of as the string of text that a
        /// user would use to explain which control is being referred to.  It is
        /// important to have a textual representation for all controls in the
        /// graphical user interface (GUI) so that you can programmatically
        /// refer to the control in a localized manner.  The value is settable
        /// on control instances through the AutomationProperties.Name attached
        /// property.
        /// </remarks>
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (string.IsNullOrEmpty(name))
            {
                AutomationPeer labeledBy = GetLabeledByCore();
                if (labeledBy != null)
                {
                    name = labeledBy.GetName();
                }

                CalendarDayButton button = OwningCalendarDayButton;
                if (string.IsNullOrEmpty(name) && button != null)
                {
                    if (button.DataContext is DateTime)
                    {
                        name = ((DateTime)button.DataContext).ToLongDateString();
                    }
                    else if (button.Content != null)
                    {
                        name = string.Format(DateTimeHelper.GetCurrentDateFormat(), button.Content.ToString());
                    }
                }
            }
            return name;
        }

        /// <summary>
        /// Sends a request to activate the control and to initiate its single,
        /// unambiguous action.
        /// </summary>
      [OpenSilver.NotImplemented]
        void IInvokeProvider.Invoke()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the current element to the collection of selected items.
        /// </summary>
        [OpenSilver.NotImplemented]
        void ISelectionItemProvider.AddToSelection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the current element from the collection of selected items.
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            // Return if the item is not already selected.
            CalendarDayButton button = OwningCalendarDayButton;
            if (!button.IsSelected)
            {
                return;
            }

            Calendar calendar = OwningCalendar;
            if (calendar != null && button.DataContext != null)
            {
                calendar.SelectedDate = null;
            }
        }

        /// <summary>
        /// Clear any existing selection and then selects the current element.
        /// </summary>
        void ISelectionItemProvider.Select()
        {
            if (EnsureSelection())
            {
                Calendar calendar = OwningCalendar;
                CalendarDayButton button = OwningCalendarDayButton;

                if (button.DataContext != null)
                {
                    calendar.SelectedDate = (DateTime)button.DataContext;
                }
            }
        }

        /// <summary>
        /// Ensure selection of the CalendarDayButton is possible.
        /// </summary>
        /// <returns>
        /// A value indicating whether selection of the CalendarDayButton is
        /// possible.
        /// </returns>
        private bool EnsureSelection()
        {
            CalendarDayButton button = OwningCalendarDayButton;
            if (!button.IsEnabled)
            {
                throw new ElementNotEnabledException();
            }

            // If the day is a blackout day or the SelectionMode is None,
            // selection is not allowed
            Calendar calendar = OwningCalendar;
            return !button.IsBlackout &&
                button.Visibility != Visibility.Collapsed &&
                calendar != null;
            //todo add selectionMode
            //&&  calendar.SelectionMode != CalendarSelectionMode.None;
        }

        /// <summary>
        /// Retrieves a collection of UI Automation providers that represent all
        /// the column headers that are associated with a table item or cell.
        /// </summary>
        /// <returns>A collection of UI Automation providers.</returns>
        IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
        {
            Calendar calendar = OwningCalendar;
            if (calendar != null && OwningCalendarAutomationPeer != null)
            {
                IRawElementProviderSimple[] headers = ((ITableProvider)CreatePeerForElement(calendar)).GetColumnHeaders();
                if (headers != null)
                {
                    int column = ((IGridItemProvider)this).Column;
                    return new IRawElementProviderSimple[] { headers[column] };
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves a collection of UI Automation providers that represent all
        /// the row headers that are associated with a table item or cell.
        /// </summary>
        /// <returns>A collection of UI Automation providers.</returns>
        IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems()
        {
            return null;
        }
    }

    internal static class DateTimeHelper
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="time">Inherited code: Requires comment 1.</param>
        /// <param name="days">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static DateTime? AddDays(DateTime time, int days)
        {
            try
            {
                return time.AddDays(days);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="time">Inherited code: Requires comment 1.</param>
        /// <param name="months">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static DateTime? AddMonths(DateTime time, int months)
        {
            try
            {
                return time.AddMonths(months);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="time">Inherited code: Requires comment 1.</param>
        /// <param name="years">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static DateTime? AddYears(DateTime time, int years)
        {
            try
            {
                return time.AddYears(years);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="dt1">Inherited code: Requires comment 1.</param>
        /// <param name="dt2">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static int CompareDays(DateTime dt1, DateTime dt2)
        {
            return DateTime.Compare(DiscardTime(dt1).Value, DiscardTime(dt2).Value);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="dt1">Inherited code: Requires comment 1.</param>
        /// <param name="dt2">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static int CompareYearMonth(DateTime dt1, DateTime dt2)
        {
            return (dt1.Year - dt2.Year) * 12 + (dt1.Month - dt2.Month);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="date">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        public static int DecadeOfDate(DateTime date)
        {
            return date.Year - (date.Year % 10);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        public static DateTime DiscardDayTime(DateTime d)
        {
            int year = d.Year;
            int month = d.Month;
            DateTime newD = new DateTime(year, month, 1, 0, 0, 0);
            return newD;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        public static DateTime? DiscardTime(DateTime? d)
        {
            if (d == null)
            {
                return null;
            }
            return d.Value.Date;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="date">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        public static int EndOfDecade(DateTime date)
        {
            return DecadeOfDate(date) + 9;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <returns>Inherited code: Requires comment 1.</returns>
        public static Globalization.DateTimeFormatInfo GetCurrentDateFormat()
        {
            if (Globalization.CultureInfo.CurrentCulture.Calendar is Globalization.DateTimeFormatInfo)
            {
                return Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
            }
            else
            {
                foreach (Globalization.Calendar cal in Globalization.CultureInfo.CurrentCulture.OptionalCalendars)
                {
                    if (cal is Globalization.DateTimeFormatInfo)
                    {
                        // if the default calendar is not Gregorian, return the
                        // first supported Globalization.DateTimeFormatInfo dtfi
                        Globalization.DateTimeFormatInfo dtfi = new Globalization.CultureInfo(Globalization.CultureInfo.CurrentCulture.Name).DateTimeFormat;
                        dtfi.Calendar = cal;
                        return dtfi;
                    }
                }

                // if there are no Globalization.DateTimeFormatInfos in the OptionalCalendars
                // list, use the invariant dtfi
                Globalization.DateTimeFormatInfo dt = new Globalization.CultureInfo(Globalization.CultureInfo.InvariantCulture.Name).DateTimeFormat;
                return dt;
            }
        }

        /// <summary>
        /// Returns if the date is included in the range.
        /// </summary>
        /// <param name="date">Inherited code: Requires comment 1.</param>
        /// <param name="range">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        public static bool InRange(DateTime date, CalendarDateRange range)
        {
            Debug.Assert(DateTime.Compare(range.Start, range.End) < 1, "The range should start before it ends!");

            if (CompareDays(date, range.Start) > -1 && CompareDays(date, range.End) < 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a localized string for the specified date using the YearMonthPattern format.
        /// </summary>
        /// <param name="date">Date to convert.</param>
        /// <returns>Localized string.</returns>
        public static string ToYearMonthPatternString(DateTime date)
        {
            string result = string.Empty;
            Globalization.DateTimeFormatInfo format = GetCurrentDateFormat();

            if (format != null)
            {
                result = date.ToString(format.YearMonthPattern, format);
            }

            return result;
        }

        /// <summary>
        /// Gets a localized string for the specified date's year.
        /// </summary>
        /// <param name="date">Date to convert.</param>
        /// <returns>Localized string.</returns>
        public static string ToYearString(DateTime date)
        {
            string result = string.Empty;
            Globalization.DateTimeFormatInfo format = GetCurrentDateFormat();

            if (format != null)
            {
                result = date.Year.ToString(format);
            }

            return result;
        }
    }
}