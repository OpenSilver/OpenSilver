// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="Calendar" /> types to UI automation.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class CalendarAutomationPeer :
        FrameworkElementAutomationPeer,
        IGridProvider,
        IMultipleViewProvider,
        ISelectionProvider,
        ITableProvider
    {
        /// <summary>
        /// Gets the Calendar associated with the CalendarAutomationPeer.
        /// </summary>
        private Calendar OwningCalendar
        {
            get { return Owner as Calendar; }
        }

        /// <summary>
        /// Gets the current top-level Grid of with the Calendar.
        /// </summary>
        private Grid OwningGrid
        {
            get
            {
                Calendar calendar = OwningCalendar;
                if (calendar.MonthControl != null)
                {
                    if (calendar.DisplayMode == CalendarMode.Month)
                    {
                        return calendar.MonthControl.MonthView;
                    }
                    else
                    {
                        return calendar.MonthControl.YearView;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the total number of columns in a grid.
        /// </summary>
        /// <value>
        /// The total number of columns in a grid.
        /// </value>
        int IGridProvider.ColumnCount
        {
            get
            {
                Grid owner = OwningGrid;
                if (owner != null)
                {
                    return owner.ColumnDefinitions.Count;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the total number of rows in a grid.
        /// </summary>
        /// <value>
        /// The total number of rows in a grid.
        /// </value>
        int IGridProvider.RowCount
        {
            get
            {
                Grid owner = OwningGrid;
                if (owner != null)
                {
                    if (OwningCalendar.DisplayMode == CalendarMode.Month)
                    {
                        // The Month DisplayMode also includes a row of headers
                        // which we ignore
                        return owner.RowDefinitions.Count - 1;
                    }
                    else
                    {
                        return owner.RowDefinitions.Count;
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the current control-specific view.
        /// </summary>
        /// <value>
        /// The value for the current view of the UI automation element.
        /// </value>
        int IMultipleViewProvider.CurrentView
        {
            get { return (int)OwningCalendar.DisplayMode; }
        }

        /// <summary>
        /// Gets a value indicating whether the UI automation provider allows
        /// more than one child element to be selected at the same time.
        /// </summary>
        /// <value>
        /// True if multiple selection is allowed; otherwise, false.
        /// </value>
        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                Calendar owner = OwningCalendar;
                return owner.SelectionMode == CalendarSelectionMode.SingleRange ||
                     owner.SelectionMode == CalendarSelectionMode.MultipleRange;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the UI automation provider
        /// requires at least one child element to be selected.
        /// </summary>
        /// <value>
        /// True if selection is required; otherwise, false.
        /// </value>
        bool ISelectionProvider.IsSelectionRequired
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the primary direction of traversal for the table.
        /// </summary>
        /// <value>
        /// The primary direction of traversal.
        /// </value>
        RowOrColumnMajor ITableProvider.RowOrColumnMajor
        {
            get { return RowOrColumnMajor.RowMajor; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="Calendar" /> instance to associate with the <see cref="CalendarAutomationPeer" />.
        /// </param>
        public CalendarAutomationPeer(Calendar owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control pattern for this <see cref="CalendarAutomationPeer" />.
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
            bool imlpementsInterface =
                patternInterface == PatternInterface.Grid ||
                patternInterface == PatternInterface.Table ||
                patternInterface == PatternInterface.MultipleView ||
                patternInterface == PatternInterface.Selection;
            if (imlpementsInterface && OwningGrid != null)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Returns the control type for the Calendar that is associated with
        /// this CalendarAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>A value of the AutomationControlType enumeration.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Calendar;
        }

        /// <summary>
        /// Returns the name of the UIElement that is associated with this
        /// FrameworkElementAutomationPeer.  This method is called by
        /// GetClassName.
        /// </summary>
        /// <returns>
        /// The name of the owner type that is associated with this
        /// CalendarAutomationPeer. 
        /// </returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Returns the text label of the Calendar that is associated with this
        /// CalendarAutomationPeer.  This method is called by GetName.
        /// </summary>
        /// <returns>
        /// The text label of the element that is associated with this
        /// automation peer.
        /// </returns>
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

                if (string.IsNullOrEmpty(name))
                {
                    name = OwningCalendar.ToString();
                }
            }
            return name;
        }

        /// <summary>
        /// Retrieves the UI automation provider for the specified cell.
        /// </summary>
        /// <param name="row">
        /// The ordinal number of the row.
        /// </param>
        /// <param name="column">
        /// The ordinal number of the column.
        /// </param>
        /// <returns>
        /// The UI automation provider for the specified cell.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Grid coordinates are zero-based. The upper-left cell (or upper-right
        /// cell, depending on locale) has coordinates (0,0).
        /// </para>
        /// <para>
        /// If a cell is empty, a UI Automation provider must still be returned
        /// in order to support the ContainingGrid property for that cell.  This
        /// is possible when the layout of child elements in the grid is similar
        /// to a ragged array.
        /// </para>
        /// <para>
        /// Hidden rows and columns can be loaded in the tree, depending on the
        /// provider implementation.  Therefore, they will be reflected in the
        /// RowCount and ColumnCount properties.  If the hidden rows and columns
        /// have not yet been loaded, they should not be counted.
        /// </para>
        /// </remarks>
        IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
        {
            if (OwningCalendar.DisplayMode == CalendarMode.Month)
            {
                // In Month DisplayMode, since first row is DayTitles, we
                // increment the row number by 1
                row++;
            }

            if (OwningGrid != null && row >= 0 && row < OwningGrid.RowDefinitions.Count && column >= 0 && column < OwningGrid.ColumnDefinitions.Count)
            {
                foreach (UIElement child in OwningGrid.Children)
                {
                    int childRow = (int) child.GetValue(Grid.RowProperty);
                    int childColumn = (int) child.GetValue(Grid.ColumnProperty);
                    if (childRow == row && childColumn == column)
                    {
                        AutomationPeer peer = CreatePeerForElement(child);
                        if (peer != null)
                        {
                            return ProviderFromPeer(peer);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves a collection of control-specific view identifiers.
        /// </summary>
        /// <returns>
        /// A collection of values that identifies the views that are available
        /// for a UI automation element.
        /// </returns>
        /// <remarks>
        /// The collection of view identifiers must be identical across
        /// instances.  View identifier values can be passed to GetViewName.
        /// </remarks>
        int[] IMultipleViewProvider.GetSupportedViews()
        {
            int[] supportedViews = new int[3];

            supportedViews[0] = (int) CalendarMode.Month;
            supportedViews[1] = (int) CalendarMode.Year;
            supportedViews[2] = (int) CalendarMode.Decade;

            return supportedViews;
        }

        /// <summary>
        /// Retrieves the name of a control-specific view.
        /// </summary>
        /// <param name="viewId">The view identifier.</param>
        /// <returns>A localized name for the view.</returns>
        /// <remarks>
        /// View identifiers can be retrieved by using GetSupportedViews.  The
        /// collection of view identifiers must be identical across instances.
        /// View names must be suitable for use in text-to-speech, Braille, and
        /// other accessible applications.
        /// </remarks>
        string IMultipleViewProvider.GetViewName(int viewId)
        {
            switch (viewId)
            {
                case 0:
                    return "Month";
                case 1:
                    return "Year";    
                case 2:
                    return "Decade";
            }

            // REMOVE_RTM: update when Jolt 23302 is fixed
            // throw new ArgumentOutOfRangeException("viewId", System.Windows.Controls.Properties.Resources.Calendar_OnDisplayModePropertyChanged_InvalidValue);
            return string.Empty;
        }

        /// <summary>
        /// Sets the current control-specific view.
        /// </summary>
        /// <param name="viewId">A view identifier.</param>
        /// <remarks>
        /// View identifiers can be retrieved by using GetSupportedViews.
        /// </remarks>
        void IMultipleViewProvider.SetCurrentView(int viewId)
        {
            OwningCalendar.DisplayMode = (CalendarMode) viewId;
        }

        /// <summary>
        /// Retrieves a UI automation provider for each child element that is
        /// selected.
        /// </summary>
        /// <returns>
        /// A collection of UI automation providers.
        /// </returns>
        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>();

            if (OwningGrid != null)
            {
                if (OwningCalendar.DisplayMode == CalendarMode.Month && OwningCalendar.SelectedDates != null && OwningCalendar.SelectedDates.Count != 0)
                {
                    // return selected DayButtons
                    CalendarDayButton dayButton;

                    foreach (UIElement child in OwningGrid.Children)
                    {
                        int childRow = (int) child.GetValue(Grid.RowProperty);

                        if (childRow != 0)
                        {
                            dayButton = child as CalendarDayButton;

                            if (dayButton != null && dayButton.IsSelected)
                            {
                                AutomationPeer peer = CreatePeerForElement(dayButton);

                                if (peer != null)
                                {
                                    providers.Add(ProviderFromPeer(peer));
                                }
                            }
                        }
                    }
                }
                else
                {
                    // return the CalendarButton which has focus
                    CalendarButton calendarButton;

                    foreach (UIElement child in OwningGrid.Children)
                    {
                        calendarButton = child as CalendarButton;

                        if (calendarButton != null && calendarButton.IsCalendarButtonFocused)
                        {
                            AutomationPeer peer = CreatePeerForElement(calendarButton);

                            if (peer != null)
                            {
                                providers.Add(ProviderFromPeer(peer));
                            }
                            break;
                        }
                    }
                }

                if (providers.Count > 0)
                {
                    return providers.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Raise selection AutomationEvents when the Calendar's SelectedDates
        /// collection changes.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal void RaiseSelectionEvents(SelectionChangedEventArgs e)
        {
            Calendar calendar = OwningCalendar;
            int selectedDates = calendar.SelectedDates.Count;

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) && selectedDates == 1)
            {
                RaiseDayButtonSelectionEvent(
                    calendar,
                    (DateTime) e.AddedItems[0],
                    AutomationEvents.SelectionItemPatternOnElementSelected);
            }
            else
            {
                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
                {
                    foreach (DateTime date in e.AddedItems)
                    {
                        RaiseDayButtonSelectionEvent(
                            calendar,
                            date,
                            AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                    }
                }

                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                {
                    foreach (DateTime date in e.RemovedItems)
                    {
                        RaiseDayButtonSelectionEvent(
                            calendar,
                            date,
                            AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                    }
                }
            }
        }

        /// <summary>
        /// Raise an automation peer event for the selection of a day button.
        /// </summary>
        /// <param name="calendar">
        /// The Calendar associated with this automation peer.
        /// </param>
        /// <param name="date">The selected date.</param>
        /// <param name="eventToRaise">The selection event to raise.</param>
        private static void RaiseDayButtonSelectionEvent(Calendar calendar, DateTime date, AutomationEvents eventToRaise)
        {
            CalendarDayButton button = calendar.FindDayButtonFromDay(date);
            if (button != null)
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(button);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(eventToRaise);
                }
            }
        }

        /// <summary>
        /// Gets a collection of UI automation providers that represents all the
        /// column headers in a table.
        /// </summary>
        /// <returns>A collection of UI automation providers.</returns>
        IRawElementProviderSimple[] ITableProvider.GetColumnHeaders()
        {
            if (OwningCalendar.DisplayMode == CalendarMode.Month)
            {
                List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>();
                foreach (UIElement child in OwningGrid.Children)
                {
                    int childRow = (int) child.GetValue(Grid.RowProperty);
                    if (childRow == 0)
                    {
                        AutomationPeer peer = CreatePeerForElement(child);
                        if (peer != null)
                        {
                            providers.Add(ProviderFromPeer(peer));
                        }
                    }
                }

                if (providers.Count > 0)
                {
                    return providers.ToArray();
                }
            }

            return null;
        }
        
        /// <summary>
        /// Retrieves a collection of UI automation providers that represents
        /// all row headers in the table.
        /// </summary>
        /// <returns>A collection of UI automation providers.</returns>
        IRawElementProviderSimple[] ITableProvider.GetRowHeaders()
        {
            // If WeekNumber functionality is supported by Calendar in the
            // future, this method should return WeekNumbers.
            return null;
        }
    }
}