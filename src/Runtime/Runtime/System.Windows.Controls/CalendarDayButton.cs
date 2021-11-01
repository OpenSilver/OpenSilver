// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a day on a <see cref="T:System.Windows.Controls.Calendar" />.
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
    [TemplateVisualState(Name = CalendarDayButton.StateRegularDay, GroupName = CalendarDayButton.GroupDay)]
    [TemplateVisualState(Name = CalendarDayButton.StateToday, GroupName = CalendarDayButton.GroupDay)]
    [TemplateVisualState(Name = CalendarDayButton.StateNormalDay, GroupName = CalendarDayButton.GroupBlackout)]
    [TemplateVisualState(Name = CalendarDayButton.StateBlackoutDay, GroupName = CalendarDayButton.GroupBlackout)]
    public sealed class CalendarDayButton : Button
    {
        #region Visual States
        /// <summary>
        /// Identifies the Today state.
        /// </summary>
        internal const string StateToday = "Today";

        /// <summary>
        /// Identifies the RegularDay state.
        /// </summary>
        internal const string StateRegularDay = "RegularDay";

        /// <summary>
        /// Name of the Day state group.
        /// </summary>
        internal const string GroupDay = "DayStates";

        /// <summary>
        /// Identifies the BlackoutDay state.
        /// </summary>
        internal const string StateBlackoutDay = "BlackoutDay";

        /// <summary>
        /// Identifies the NormalDay state.
        /// </summary>
        internal const string StateNormalDay = "NormalDay";

        /// <summary>
        /// Name of the BlackoutDay state group.
        /// </summary>
        internal const string GroupBlackout = "BlackoutDayStates";
        #endregion Visual States

        /// <summary>
        /// Default content for the CalendarDayButton.
        /// </summary>
        private const int DefaultContent = 1;

        /// <summary>
        /// Gets or sets the Calendar associated with this button.
        /// </summary>
        internal Calendar Owner { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal int Index { get; set; }

        /// <summary>
        /// A value indicating whether the button should ignore the MouseOver
        /// visual state.
        /// </summary>
        // REMOVE_RTM: This field should be removed after Jolt Bug#20180 is fixed.
        private bool _ignoringMouseOverState;

        #region internal bool IsBlackout
        /// <summary>
        /// A value indicating whether this is a blackout date.
        /// </summary>
        private bool _isBlackout;

        /// <summary>
        /// Gets or sets a value indicating whether this is a blackout date.
        /// </summary>
        internal bool IsBlackout
        {
            get { return _isBlackout; }
            set
            {
                _isBlackout = value;
                ChangeVisualState(true);
            }
        }
        #endregion internal bool IsBlackout

        #region internal bool IsToday
        /// <summary>
        /// A value indicating whether this button represents today.
        /// </summary>
        private bool _isToday;

        /// <summary>
        /// Gets or sets a value indicating whether this button represents
        /// today.
        /// </summary>
        internal bool IsToday
        {
            get { return _isToday; }
            set
            {
                _isToday = value;
                ChangeVisualState(true);
            }
        }
        #endregion internal bool IsToday

        #region internal bool IsCurrent
        /// <summary>
        /// A value indicating whether the button is the focused element on the
        /// Calendar control.
        /// </summary>
        private bool _isCurrent;

        /// <summary>
        /// Gets or sets a value indicating whether the button is the focused
        /// element on the Calendar control.
        /// </summary>
        internal bool IsCurrent
        {
            get { return _isCurrent; }
            set
            {
                _isCurrent = value;
                ChangeVisualState(true);
            }
        }
        #endregion internal bool IsCurrent

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
#if MIGRATION
        public event MouseButtonEventHandler CalendarDayButtonMouseDown;
#else
        public event PointerEventHandler CalendarDayButtonMouseDown;
#endif

        /// <summary>
        /// Occurs when the left mouse button is released (or the tip of the
        /// stylus is removed from the tablet PC) while the mouse (or the
        /// stylus) is over a UIElement (or while a UIElement holds mouse
        /// capture).
        /// </summary>
#if MIGRATION
        public event MouseButtonEventHandler CalendarDayButtonMouseUp;
#else
        public event PointerEventHandler CalendarDayButtonMouseUp;
#endif

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.Primitives.CalendarDayButton" />
        /// class.
        /// </summary>
        public CalendarDayButton()
            : base()
        {
            DefaultStyleKey = typeof(CalendarDayButton);
            IsTabStop = false;
            Loaded += OnLoad;

            Content = DefaultContent.ToString(CultureInfo.CurrentCulture);
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
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.Primitives.CalendarDayButton" />
        /// when a new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            ChangeVisualState(false);
        }

        /// <summary>
        /// Returns a CalendarDayButtonAutomationPeer for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>
        /// CalendarDayButtonAutomationPeer for the Button object.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates a new CalendarDayButtonAutomationPeer instance
        /// if one has not been created for the CalendarButton; otherwise, it
        /// returns the CalendarDayButtonAutomationPeer previously created.
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
            return new CalendarDayButtonAutomationPeer(this);
        }

        /// <summary>
        /// Provides class handling for the MouseLeftButtonDown event that
        /// occurs when the left mouse button is pressed while the mouse pointer
        /// is over this control.
        /// </summary>
        /// <param name="e">The event data. </param>
        /// <exception cref="System.ArgumentNullException">
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
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            MouseButtonEventHandler handler = CalendarDayButtonMouseDown;
            if (null != handler)
            {
                handler(this, e);
            }
        }
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            PointerEventHandler handler = CalendarDayButtonMouseDown;
            if (null != handler)
            {
                handler(this, e);
            }
        }
#endif

        /// <summary>
        /// Provides handling for the MouseLeftButtonUp event that occurs when
        /// the left mouse button is released while the mouse pointer is over
        /// this control. 
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <exception cref="System.ArgumentNullException">
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
#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            MouseButtonEventHandler handler = CalendarDayButtonMouseUp;
            if (null != handler)
            {
                handler(this, e);
            }
        }
#else
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            PointerEventHandler handler = CalendarDayButtonMouseUp;
            if (null != handler)
            {
                handler(this, e);
            }
        }
#endif

        /// <summary>
        /// We need to simulate the MouseLeftButtonUp event for the
        /// CalendarDayButton that stays in Pressed state after MouseCapture is
        /// released since there is no actual MouseLeftButtonUp event for the
        /// release.
        /// </summary>
        /// <param name="e">Event arguments.</param>
#if MIGRATION
        internal void SendMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = false;
            base.OnMouseLeftButtonUp(e);
        }
#else
        internal void SendMouseLeftButtonUp(PointerRoutedEventArgs e)
        {
            e.Handled = false;
            base.OnPointerReleased(e);
        }
#endif

        /// <summary>
        /// Ensure the button is not in the MouseOver state.
        /// </summary>
        /// <remarks>
        /// If a button is in the MouseOver state when a Popup is closed (as is
        /// the case when you select a date in the DatePicker control), it will
        /// continue to think it's in the mouse over state even when the Popup
        /// opens again and it's not.  This method is used to forcibly clear the
        /// state by changing the CommonStates state group.
        /// </remarks>
        internal void IgnoreMouseOverState()
        {
            // TODO: Investigate whether this needs to be done by changing the
            // state everytime we change any state, or if it can be done once
            // to properly reset the control.

            _ignoringMouseOverState = false;

            // If the button thinks it's in the MouseOver state (which can
            // happen when a Popup is closed before the button can change state)
            // we will override the state so it shows up as normal.
            if (IsMouseOver)
            {
                _ignoringMouseOverState = true;
                ChangeVisualState(true);
            }
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
            if (_ignoringMouseOverState)
            {
                if (IsPressed)
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StatePressed);
                }
                if (IsEnabled)
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
                }
                else
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled);
                }
            }

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

            // Update the DayStates group
            if (IsToday)
            {
                VisualStates.GoToState(this, useTransitions, StateToday, StateRegularDay);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, StateRegularDay);
            }

            // Update the BlackoutDayStates group
            if (IsBlackout)
            {
                VisualStates.GoToState(this, useTransitions, StateBlackoutDay, StateNormalDay);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, StateNormalDay);
            }

            // Update the CalendarButtonFocusStates group (IsCurrent means the
            // button is the focused element on the Calendar control).
            if (IsCurrent && IsEnabled)
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