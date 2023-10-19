// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Automation.Peers;
using System.Windows.Input;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents a button on a <see cref="Calendar" />.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnselected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateSelected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateCalendarButtonUnfocused, GroupName = VisualStates.GroupCalendarButtonFocus)]
    [TemplateVisualState(Name = VisualStates.StateCalendarButtonFocused, GroupName = VisualStates.GroupCalendarButtonFocus)]
    [TemplateVisualState(Name = VisualStates.StateInactive, GroupName = VisualStates.GroupActive)]
    [TemplateVisualState(Name = VisualStates.StateActive, GroupName = VisualStates.GroupActive)]
    public sealed class CalendarButton : Button
    {
        /// <summary>
        /// Gets or sets the Calendar associated with this button.
        /// </summary>
        internal Calendar Owner { get; set; }

        #region internal bool IsCalendarButtonFocused
        /// <summary>
        /// A value indicating whether the button is focused.
        /// </summary>
        private bool _isCalendarButtonFocused;

        /// <summary>
        /// Gets or sets a value indicating whether the button is focused.
        /// </summary>
        internal bool IsCalendarButtonFocused
        {
            get { return _isCalendarButtonFocused; }
            set
            {
                _isCalendarButtonFocused = value;
                ChangeVisualState(true);
            }
        }
        #endregion internal bool IsCalendarButtonFocused

        #region internal bool IsInactive
        /// <summary>
        /// A value indicating whether the button is inactive.
        /// </summary>
        private bool _isInactive;

        /// <summary>
        /// Gets or sets a value indicating whether the button is inactive.
        /// </summary>
        internal bool IsInactive
        {
            get { return _isInactive; }
            set
            {
                _isInactive = value;
                ChangeVisualState(true);
            }
        }
        #endregion internal bool IsInactive

        #region internal bool IsSelected
        /// <summary>
        /// A value indicating whether the button is selected.
        /// </summary>
        private bool _isSelected;

        /// <summary>
        /// Gets or sets a value indicating whether the button is selected.
        /// </summary>
        internal bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                ChangeVisualState(true);
            }
        }
        #endregion internal bool IsSelected
        
        /// <summary>
        /// Occurs when the left mouse button is pressed (or when the tip of the
        /// stylus touches the tablet PC) while the mouse pointer is over a
        /// UIElement.
        /// </summary>
        public event MouseButtonEventHandler CalendarButtonMouseDown;

        /// <summary>
        /// Occurs when the left mouse button is released (or the tip of the
        /// stylus is removed from the tablet PC) while the mouse (or the
        /// stylus) is over a UIElement (or while a UIElement holds mouse
        /// capture).
        /// </summary>
        public event MouseButtonEventHandler CalendarButtonMouseUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarButton" /> class.
        /// </summary>
        public CalendarButton()
            : base()
        {
            DefaultStyleKey = typeof(CalendarButton);
            IsTabStop = false;
            Loaded += OnLoad;

            Content = DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[0];
        }

        /// <summary>
        /// Handle the Loaded event.
        /// </summary>
        /// <param name="sender">The CalendarButton.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ChangeVisualState(false);
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="CalendarButton" /> when a new 
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ChangeVisualState(false);
        }

        /// <summary>
        /// Returns a CalendarButtonAutomationPeer for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>
        /// CalendarButtonAutomationPeer for the Button object.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates a new CalendarButtonAutomationPeer instance if
        /// one has not been created for the CalendarButton; otherwise, it
        /// returns the CalendarButtonAutomationPeer previously created.
        /// </para>
        /// <para>
        /// Classes that participate in the Silverlight automation
        /// infrastructure must implement this method to return a class-specific
        /// derived class of AutomationPeer that reports information for
        /// automation behavior.
        /// </para>
        /// </remarks>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CalendarButtonAutomationPeer(this);
        }

        /// <summary>
        /// Provides class handling for the MouseLeftButtonDown event that
        /// occurs when the left mouse button is pressed while the mouse pointer
        /// is over this control.
        /// </summary>
        /// <param name="e">The event data. </param>
        /// <exception cref="ArgumentNullException">
        /// e is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <remarks>
        /// This method marks the MouseLeftButtonDown event as handled by
        /// setting the MouseButtonEventArgs.Handled property of the event data
        /// to true when the button is enabled and its ClickMode is not set to
        /// Hover.  Since this method marks the MouseLeftButtonDown event as
        /// handled in some situations, you should use the Click event instead
        /// to detect a button click.
        /// </remarks>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            MouseButtonEventHandler handler = CalendarButtonMouseDown;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseLeftButtonUp event that occurs when
        /// the left mouse button is released while the mouse pointer is over
        /// this control. 
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <exception cref="ArgumentNullException">
        /// e is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <remarks>
        /// This method marks the MouseLeftButtonUp event as handled by setting
        /// the MouseButtonEventArgs.Handled property of the event data to true
        /// when the button is enabled and its ClickMode is not set to Hover.
        /// Since this method marks the MouseLeftButtonUp event as handled in
        /// some situations, you should use the Click event instead to detect a
        /// button click.
        /// </remarks>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            MouseButtonEventHandler handler = CalendarButtonMouseUp;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// We need to simulate the MouseLeftButtonUp event for the
        /// CalendarButton that stays in Pressed state after MouseCapture is
        /// released since there is no actual MouseLeftButtonUp event for the
        /// release.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal void SendMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = false;
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Change to the correct visual state for the button.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal void ChangeVisualState(bool useTransitions)
        {
            // Update the SelectionStates group
            if (IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateSelected, VisualStates.StateUnselected);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnselected);
            }

            // Update the ActiveStates group
            if (IsInactive)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateInactive);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateActive, VisualStates.StateInactive);
            }

            // Update the FocusStates group
            if (IsCalendarButtonFocused && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateCalendarButtonFocused, VisualStates.StateCalendarButtonUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateCalendarButtonUnfocused);
            }
        }
    }
}