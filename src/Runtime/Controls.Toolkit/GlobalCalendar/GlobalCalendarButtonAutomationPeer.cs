// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes
    /// <see cref="T:System.Windows.Controls.Primitives.GlobalCalendarButton" /> types
    /// to UI automation.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public partial class GlobalCalendarButtonAutomationPeer :
        FrameworkElementAutomationPeer,
        IGridItemProvider,
        IInvokeProvider,
        ISelectionItemProvider
    {
        /// <summary>
        /// Gets the GlobalCalendarButton to associate with this AutomationPeer.
        /// </summary>
        private GlobalCalendarButton OwningCalendarButton
        {
            get { return Owner as GlobalCalendarButton; }
        }

        /// <summary>
        /// Gets the GlobalCalendar associated with the button.
        /// </summary>
        private GlobalCalendar OwningCalendar
        {
            get { return OwningCalendarButton.Owner; }
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
            get { return (int)OwningCalendarButton.GetValue(Grid.ColumnProperty); }
        }

        /// <summary>
        /// Gets the number of columns that are spanned by a cell or item.
        /// </summary>
        /// <value>
        /// The number of columns spanned.
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
            get { return (int)OwningCalendarButton.GetValue(Grid.RowProperty); }
        }

        /// <summary>
        /// Gets the number of rows that are spanned by a cell or item.
        /// </summary>
        /// <value>
        /// The number of rows that are spanned.
        /// </value>
        int IGridItemProvider.RowSpan
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets a value indicating whether an item is selected.
        /// </summary>
        /// <value>
        /// True if the element is selected; otherwise, false.
        /// </value>
        bool ISelectionItemProvider.IsSelected
        {
            get { return OwningCalendarButton.IsCalendarButtonFocused; }
        }

        /// <summary>
        /// Gets the UI Automation provider that implements
        /// <see cref="T:System.Windows.Automation.Provider.ISelectionProvider" />
        /// and that acts as the container for the calling object.
        /// </summary>
        /// <value>
        /// The provider that supports
        /// <see cref="T:System.Windows.Automation.Provider.ISelectionProvider" />.
        /// </value>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get { return OwningCalendarAutomationPeer; }
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Automation.Peers.GlobalCalendarButtonAutomationPeer" />
        /// class.
        /// </summary>
        /// <param name="owner">
        /// The
        /// <see cref="T:System.Windows.Controls.Primitives.GlobalCalendarButton" />
        /// to associate with this
        /// <see cref="T:System.Windows.Automation.Peers.AutomationPeer" />.
        /// </param>
        public GlobalCalendarButtonAutomationPeer(GlobalCalendarButton owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control pattern for this
        /// <see cref="T:System.Windows.Automation.Peers.GlobalCalendarButtonAutomationPeer" />.
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
            if (patternInterface == PatternInterface.GridItem ||
                patternInterface == PatternInterface.Invoke ||
                patternInterface == PatternInterface.SelectionItem)
            {
                GlobalCalendar calendar = OwningCalendar;
                if (calendar != null && calendar.MonthControl != null)
                {
                    return this;
                }
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Returns the control type for the GlobalCalendarButton that is associated
        /// with this GlobalCalendarButtonAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>A value of the AutomationControlType enumeration.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <summary>
        /// Returns the localized version of the control type for the owner type
        /// that is associated with this GlobalCalendarButtonAutomationPeer.
        /// </summary>
        /// <returns>The string that contains the type of control.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return Resource.CalendarAutomationPeer_CalendarButtonLocalizedControlType;
        }

        /// <summary>
        /// Returns the string that describes the functionality of the
        /// GlobalCalendarButton that is associated with this
        /// GlobalCalendarButtonAutomationPeer.  This method is called by GetHelpText.
        /// </summary>
        /// <returns>
        /// The help text, or String.Empty if there is no help text.
        /// </returns>
        protected override string GetHelpTextCore()
        {
            GlobalCalendarButton button = OwningCalendarButton;
            if (button != null && button.DataContext is DateTime)
            {
                DateTime date = (DateTime)OwningCalendarButton.DataContext;
                return OwningCalendar.Info.DateToLongString(date);
            }

            return base.GetHelpTextCore();
        }

        /// <summary>
        /// Returns the name of the GlobalCalendarButton that is associated with this
        /// GlobalCalendarButtonAutomationPeer.  This method is called by
        /// GetClassName.
        /// </summary>
        /// <returns>
        /// The name of the owner type that is associated with this
        /// GlobalCalendarButtonAutomationPeer.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Returns the text label of the GlobalCalendarButton that is associated with
        /// this GlobalCalendarButtonAutomationPeer. This method is called by GetName.
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

                if (string.IsNullOrEmpty(name) && OwningCalendarButton.Content != null)
                {
                    name = string.Format(
                        OwningCalendar.Info.DateFormatInfo,
                        OwningCalendarButton.Content.ToString());
                }
            }
            return name;
        }

        /// <summary>
        /// Sends a request to activate the control and initiate its single,
        /// unambiguous action.
        /// </summary>
        void IInvokeProvider.Invoke()
        {
            GlobalCalendar calendar = OwningCalendar;
            if (calendar != null && OwningCalendarButton.IsEnabled)
            {
                calendar.MonthControl.UpdateYearViewSelection(OwningCalendarButton);
                calendar.MonthControl.Month_CalendarButtonMouseUp(OwningCalendarButton, null);
            }
            else
            {
                throw new ElementNotEnabledException();
            }
        }

        /// <summary>
        /// Adds the current element to the collection of selected items.
        /// </summary>
        void ISelectionItemProvider.AddToSelection()
        {
            // REMOVE_RTM: update when Jolt 23302 is fixed
            // This operation is not possible for GlobalCalendarButton.  Since we
            // cannot currently throw exceptions in the UIAutomation classes, we
            // return silently.
            return;
        }

        /// <summary>
        /// Removes the current element from the collection of selected items.
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            // REMOVE_RTM: update when Jolt 23302 is fixed
            // This operation is not possible for GlobalCalendarButton.  Since we
            // cannot currently throw exceptions in the UIAutomation classes, we
            // return silently.
            return;
        }

        /// <summary>
        /// Clears any existing selection and then selects the current element.
        /// </summary>
        void ISelectionItemProvider.Select()
        {
            GlobalCalendarButton button = OwningCalendarButton;
            if (!button.IsEnabled)
            {
                throw new ElementNotEnabledException();
            }

            GlobalCalendar calendar = OwningCalendar;
            if (calendar != null &&
                button.Visibility != Visibility.Collapsed &&
                !button.IsCalendarButtonFocused)
            {
                foreach (GlobalCalendarButton child in calendar.MonthControl.YearView.Children)
                {
                    if (child.IsCalendarButtonFocused)
                    {
                        child.IsCalendarButtonFocused = false;
                        break;
                    }
                }

                button.IsCalendarButtonFocused = true;
            }
        }
    }
}