// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Globalization = System.Globalization;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes
    /// <see cref="T:System.Windows.Controls.Primitives.GlobalCalendarDayButton" />
    /// types to UI Automation.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public partial class GlobalCalendarDayButtonAutomationPeer :
        FrameworkElementAutomationPeer,
        IGridItemProvider,
        IInvokeProvider,
        ISelectionItemProvider,
        ITableItemProvider
    {
        /// <summary>
        /// Gets the GlobalCalendarDayButton instance that is associated with this
        /// GlobalCalendarDayButtonAutomationPeer.
        /// </summary>
        private GlobalCalendarDayButton OwningCalendarDayButton
        {
            get { return Owner as GlobalCalendarDayButton; }
        }

        /// <summary>
        /// Gets the GlobalCalendar associated with the button.
        /// </summary>
        private GlobalCalendar OwningCalendar
        {
            get { return OwningCalendarDayButton.Owner; }
        }

        /// <summary>
        /// Gets the automation peer for the GlobalCalendar associated with the
        /// button.
        /// </summary>
        private IRawElementProviderSimple OwningCalendarAutomationPeer
        {
            get
            {
                GlobalCalendar calendar = OwningCalendar;
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
        /// <see cref="T:System.Windows.Automation.Peers.GlobalCalendarDayButtonAutomationPeer" />
        /// class.
        /// </summary>
        /// <param name="owner">
        /// The
        /// <see cref="T:System.Windows.Controls.Primitives.GlobalCalendarDayButton" />
        /// instance that is associated with this
        /// <see cref="T:System.Windows.Automation.Peers.GlobalCalendarDayButtonAutomationPeer" />.
        /// </param>
        public GlobalCalendarDayButtonAutomationPeer(GlobalCalendarDayButton owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control pattern implementation for this
        /// <see cref="T:System.Windows.Automation.Peers.GlobalCalendarDayButtonAutomationPeer" />.
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
        /// Returns the control type for the GlobalCalendarDayButton that is
        /// associated with this GlobalCalendarDayButtonAutomationPeer.  This method
        /// is called by GetAutomationControlType.
        /// </summary>
        /// <returns>A value of the AutomationControlType enumeration.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <summary>
        /// Returns the localized version of the control type for the owner type
        /// that is associated with this GlobalCalendarDayButtonAutomationPeer.
        /// </summary>
        /// <returns>The string that contains the type of control.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return Resource.CalendarAutomationPeer_DayButtonLocalizedControlType;
        }

        /// <summary>
        /// Returns the string that describes the functionality of the
        /// GlobalCalendarDayButton that is associated with this
        /// GlobalCalendarDayButtonAutomationPeer.  This method is called by
        /// GetHelpText.
        /// </summary>
        /// <returns>
        /// The help text, or String.Empty if there is no help text.
        /// </returns>
        protected override string GetHelpTextCore()
        {
            GlobalCalendarDayButton button = OwningCalendarDayButton;
            if (button != null && button.DataContext is DateTime)
            {
                DateTime date = GlobalCalendarExtensions.GetDate(OwningCalendarDayButton);
                string text = OwningCalendar.Info.DateToLongString(date);

                return !button.IsBlackout ?
                    text :
                    string.Format(
                        OwningCalendar.Info.DateFormatInfo,
                        Resource.CalendarAutomationPeer_BlackoutDayHelpText,
                        text);
            }

            return base.GetHelpTextCore();
        }

        /// <summary>
        /// Returns the name of the GlobalCalendarDayButton that is associated with
        /// this GlobalCalendarDayButtonAutomationPeer.  This method is called by
        /// GetClassName.
        /// </summary>
        /// <returns>
        /// The name of the owner type that is associated with this
        /// GlobalCalendarDayButtonAutomationPeer.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Returns the text label of the GlobalCalendarDayButton that is associated
        /// with this GlobalCalendarDayButtonAutomationPeer. This method is called by
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

                GlobalCalendarDayButton button = OwningCalendarDayButton;
                if (string.IsNullOrEmpty(name) && button != null && button.Content != null)
                {
                    name = string.Format(
                        OwningCalendar.Info.DateFormatInfo,
                        button.Content.ToString());
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

                DateTime? date = GlobalCalendarExtensions.GetDateNullable(OwningCalendarDayButton);
                if (date != null)
                {
                    OwningCalendar.SelectedDates.Add(date.Value);
                    OwningCalendar.OnDayClick(date.Value);
                }
            }
        }

        /// <summary>
        /// Adds the current element to the collection of selected items.
        /// </summary>
        void ISelectionItemProvider.AddToSelection()
        {
            // Return if the item is already selected
            GlobalCalendarDayButton button = OwningCalendarDayButton;
            if (button.IsSelected)
            {
                return;
            }

            GlobalCalendar calendar = OwningCalendar;
            DateTime? date = GlobalCalendarExtensions.GetDateNullable(button);
            if (EnsureSelection() && date != null)
            {
                if (calendar.SelectionMode == CalendarSelectionMode.SingleDate)
                {
                    calendar.SelectedDate = date.Value;
                }
                else
                {
                    calendar.SelectedDates.Add(date.Value);
                }
            }
        }

        /// <summary>
        /// Removes the current element from the collection of selected items.
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            // Return if the item is not already selected.
            GlobalCalendarDayButton button = OwningCalendarDayButton;
            if (!button.IsSelected)
            {
                return;
            }

            GlobalCalendar calendar = OwningCalendar;
            DateTime? date = GlobalCalendarExtensions.GetDateNullable(button);
            if (calendar != null && date != null)
            {
                calendar.SelectedDates.Remove(date.Value);
            }
        }

        /// <summary>
        /// Clear any existing selection and then selects the current element.
        /// </summary>
        void ISelectionItemProvider.Select()
        {
            if (EnsureSelection())
            {
                GlobalCalendar calendar = OwningCalendar;
                GlobalCalendarDayButton button = OwningCalendarDayButton;

                calendar.SelectedDates.Clear();
                DateTime? date = GlobalCalendarExtensions.GetDateNullable(button);
                if (date != null)
                {
                    calendar.SelectedDates.Add(date.Value);
                }
            }
        }

        /// <summary>
        /// Ensure selection of the GlobalCalendarDayButton is possible.
        /// </summary>
        /// <returns>
        /// A value indicating whether selection of the GlobalCalendarDayButton is
        /// possible.
        /// </returns>
        private bool EnsureSelection()
        {
            GlobalCalendarDayButton button = OwningCalendarDayButton;
            if (!button.IsEnabled)
            {
                throw new ElementNotEnabledException();
            }

            // If the day is a blackout day or the SelectionMode is None,
            // selection is not allowed
            GlobalCalendar calendar = OwningCalendar;
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
            GlobalCalendar calendar = OwningCalendar;
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