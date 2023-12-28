// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Resource = OpenSilver.Controls.Resources;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="CalendarDayButton" /> types to UI Automation.
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
            get { return (int) OwningCalendarDayButton.GetValue(Grid.ColumnProperty); }
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
        /// Gets a UI Automation provider that implements <see cref="IGridProvider" />
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
                Debug.Assert((int) OwningCalendarDayButton.GetValue(Grid.RowProperty) > 0, "Row should be greater than 0");
                return (int) OwningCalendarDayButton.GetValue(Grid.RowProperty) - 1;
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
        /// Gets the UI Automation provider that implements <see cref="ISelectionProvider" />
        /// and that acts as the container for the calling object.
        /// </summary>
        /// <value>The UI Automation provider.</value>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get { return OwningCalendarAutomationPeer; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarDayButtonAutomationPeer" />
        /// class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="CalendarDayButton" /> instance that is associated with this
        /// <see cref="CalendarDayButtonAutomationPeer" />.
        /// </param>
        public CalendarDayButtonAutomationPeer(CalendarDayButton owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control pattern implementation for this <see cref="CalendarDayButtonAutomationPeer" />.
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
            return Resource.CalendarAutomationPeer_DayButtonLocalizedControlType;
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
                var info = DateTimeHelper.GetCurrentDateFormat();

                return !button.IsBlackout ?
                    dataContext.Date.ToString(info.LongDatePattern, info) :
                    string.Format(info, Resource.CalendarAutomationPeer_BlackoutDayHelpText, dataContext.Date.ToString(info.LongDatePattern, info));
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
        void IInvokeProvider.Invoke()
        {
            if (EnsureSelection())
            {
                OwningCalendar.SelectedDates.Clear();

                if (OwningCalendarDayButton.DataContext != null)
                {
                    OwningCalendar.SelectedDates.Add((DateTime) OwningCalendarDayButton.DataContext);
                    OwningCalendar.OnDayClick((DateTime) OwningCalendarDayButton.DataContext);
                }
            }
        }

        /// <summary>
        /// Adds the current element to the collection of selected items.
        /// </summary>
        void ISelectionItemProvider.AddToSelection()
        {
            // Return if the item is already selected
            CalendarDayButton button = OwningCalendarDayButton;
            if (button.IsSelected)
            {
                return;
            }

            Calendar calendar = OwningCalendar;
            if (EnsureSelection() && button.DataContext != null)
            {
                if (calendar.SelectionMode == CalendarSelectionMode.SingleDate)
                {
                    calendar.SelectedDate = (DateTime)button.DataContext;
                }
                else
                {
                    calendar.SelectedDates.Add((DateTime)button.DataContext);
                }
            }
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
                calendar.SelectedDates.Remove((DateTime) button.DataContext);
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

                calendar.SelectedDates.Clear();
                if (button.DataContext != null)
                {
                    calendar.SelectedDates.Add((DateTime)button.DataContext);
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
                calendar != null &&
                calendar.SelectionMode != CalendarSelectionMode.None;
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
}