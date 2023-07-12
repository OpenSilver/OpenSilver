// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using MouseButtonEventHandler = Windows.UI.Xaml.Input.PointerEventHandler;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Key = Windows.System.VirtualKey;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that enables a user to select a date by using a
    /// visual calendar display.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A Calendar control can be used on its own, or as a drop-down part of a
    /// DatePicker control. For more information, see DatePicker.  A Calendar
    /// displays either the days of a month, the months of a year, or the years
    /// of a decade, depending on the value of the DisplayMode property.  When
    /// displaying the days of a month, the user can select a date, a range of
    /// dates, or multiple ranges of dates.  The kinds of selections that are
    /// allowed are controlled by the SelectionMode property.
    /// </para>
    /// <para>
    /// The range of dates displayed is governed by the DisplayDateStart and
    /// DisplayDateEnd properties.  If DisplayMode is Year or Decade, only
    /// months or years that contain displayable dates will be displayed.
    /// Setting the displayable range to a range that does not include the
    /// current DisplayDate will throw an ArgumentOutOfRangeException.
    /// </para>
    /// <para>
    /// The BlackoutDates property can be used to specify dates that cannot be
    /// selected. These dates will be displayed as dimmed and disabled.
    /// </para>
    /// <para>
    /// By default, Today is highlighted.  This can be disabled by setting
    /// IsTodayHighlighted to false.
    /// </para>
    /// <para>
    /// The Calendar control provides basic navigation using either the mouse or
    /// keyboard. The following table summarizes keyboard navigation.
    /// 
    ///     Key Combination     DisplayMode     Action
    ///     ARROW               Any             Change focused date, unselect
    ///                                         all selected dates, and select
    ///                                         new focused date.
    ///                                         
    ///     SHIFT+ARROW         Any             If SelectionMode is not set to
    ///                                         SingleDate or None begin
    ///                                         selecting a range of dates.
    ///                                         
    ///     CTRL+UP ARROW       Any             Switch to the next larger
    ///                                         DisplayMode.  If DisplayMode is
    ///                                         already Decade, no action.
    ///                                         
    ///     CTRL+DOWN ARROW     Any             Switch to the next smaller
    ///                                         DisplayMode.  If DisplayMode is
    ///                                         already Month, no action.
    ///                                         
    ///     SPACEBAR            Month           Select focused date.
    ///     
    ///     SPACEBAR            Year or Decade  Switch DisplayMode to the Month
    ///                                         or Year represented by focused
    ///                                         item.
    /// </para>
    /// <para>
    /// XAML Usage for Classes Derived from Calendar
    /// If you define a class that derives from Calendar, the class can be used
    /// as an object element in XAML, and all of the inherited properties and
    /// events that show a XAML usage in the reference for the Calendar members
    /// can have the same XAML usage for the derived class. However, the object
    /// element itself must have a different prefix mapping than the controls:
    /// mapping shown in the usages, because the derived class comes from an
    /// assembly and namespace that you create and define.  You must define your
    /// own prefix mapping to an XML namespace to use the class as an object
    /// element in XAML.
    /// </para>
    /// </remarks>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = Calendar.ElementRoot, Type = typeof(Panel))]
    [TemplatePart(Name = Calendar.ElementMonth, Type = typeof(CalendarItem))]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]
    [StyleTypedProperty(Property = nameof(CalendarButtonStyle), StyleTargetType = typeof(CalendarButton))]
    [StyleTypedProperty(Property = nameof(CalendarDayButtonStyle), StyleTargetType = typeof(CalendarDayButton))]
    [StyleTypedProperty(Property = nameof(CalendarItemStyle), StyleTargetType = typeof(CalendarItem))]
    public partial class Calendar : Control
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const int RowsPerMonth = 7;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const int ColumnsPerMonth = 7;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const int RowsPerYear = 3;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const int ColumnsPerYear = 4;

        #region Template Parts
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private const string ElementRoot = "Root";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private const string ElementMonth = "CalendarItem";

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal Panel Root { get; set; }
        #endregion Template Parts

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        internal CalendarItem MonthControl
        {
            get
            {
                if (Root != null && Root.Children.Count > 0)
                {
                    return Root.Children[0] as CalendarItem;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal CalendarDayButton FocusButton { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal CalendarButton FocusCalendarButton { get; set; }

        #region CalendarButtonStyle
        /// <summary>
        /// Gets or sets the <see cref="Style" /> associated with the control's 
        /// internal <see cref="CalendarButton" /> object.
        /// </summary>
        /// <value>
        /// The current style of the <see cref="CalendarButton" /> object.
        /// </value>
        public Style CalendarButtonStyle
        {
            get { return (Style) GetValue(CalendarButtonStyleProperty); }
            set { SetValue(CalendarButtonStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CalendarButtonStyle" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="CalendarButtonStyle" /> dependency property.
        /// </value>
        public static readonly DependencyProperty CalendarButtonStyleProperty =
            DependencyProperty.Register(
            nameof(CalendarButtonStyle),
            typeof(Style),
            typeof(Calendar),
            new PropertyMetadata(OnCalendarButtonStyleChanged));

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private static void OnCalendarButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Style newStyle = e.NewValue as Style;
            Style oldStyle = e.OldValue as Style;
            Calendar c = d as Calendar;

            if (newStyle != null && c != null)
            {
                CalendarItem monthControl = c.MonthControl;

                if (monthControl != null && monthControl.YearView != null)
                {
                    foreach (UIElement child in monthControl.YearView.Children)
                    {
                        CalendarButton calendarButton = child as CalendarButton;

                        if (calendarButton != null)
                        {
                            EnsureCalendarButtonStyle(calendarButton, oldStyle, newStyle);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="calendarButton">Inherited code: Requires comment 1.</param>
        /// <param name="oldCalendarButtonStyle">Inherited code: Requires comment 2.</param>
        /// <param name="newCalendarButtonStyle">Inherited code: Requires comment 3.</param>
        private static void EnsureCalendarButtonStyle(CalendarButton calendarButton, Style oldCalendarButtonStyle, Style newCalendarButtonStyle)
        {
            Debug.Assert(calendarButton != null, "The calendarButton should not be null!");

            if (newCalendarButtonStyle != null)
            {
                // REMOVE_RTM: Remove null check when Silverlight allows us to re-apply styles
                // Apply the newCalendarButtonStyle if the CalendarButton was
                // using the oldCalendarButtonStyle before
                if (calendarButton != null && (calendarButton.Style == null || calendarButton.Style == oldCalendarButtonStyle))
                {
                    calendarButton.Style = newCalendarButtonStyle;
                }
            }
        }
        #endregion CalendarButtonStyle

        #region CalendarDayButtonStyle
        /// <summary>
        /// Gets or sets the <see cref="Style" /> associated with the control's 
        /// internal <see cref="CalendarDayButton" /> object.
        /// </summary>
        /// <value>
        /// The current style of the <see cref="CalendarDayButton" /> object.
        /// </value>
        public Style CalendarDayButtonStyle
        {
            get { return (Style) GetValue(CalendarDayButtonStyleProperty); }
            set { SetValue(CalendarDayButtonStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CalendarDayButtonStyle" /> dependency property.
        /// </summary>
        /// <remarks>
        /// The identifier for the <see cref="CalendarDayButtonStyle" /> dependency property.
        /// </remarks>
        public static readonly DependencyProperty CalendarDayButtonStyleProperty =
            DependencyProperty.Register(
            nameof(CalendarDayButtonStyle),
            typeof(Style),
            typeof(Calendar),
            new PropertyMetadata(OnCalendarDayButtonStyleChanged));

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private static void OnCalendarDayButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Style newStyle = e.NewValue as Style;
            Style oldStyle = e.OldValue as Style;
            Calendar c = d as Calendar;

            if (newStyle != null && c != null)
            {
                CalendarItem monthControl = c.MonthControl;

                if (monthControl != null && monthControl.MonthView != null)
                {
                    foreach (UIElement child in monthControl.MonthView.Children)
                    {
                        CalendarDayButton dayButton = child as CalendarDayButton;

                        if (dayButton != null)
                        {
                            EnsureDayButtonStyle(dayButton, oldStyle, newStyle);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="dayButton">Inherited code: Requires comment 1.</param>
        /// <param name="oldDayButtonStyle">Inherited code: Requires comment 2.</param>
        /// <param name="newDayButtonStyle">Inherited code: Requires comment 3.</param>
        private static void EnsureDayButtonStyle(CalendarDayButton dayButton, Style oldDayButtonStyle, Style newDayButtonStyle)
        {
            Debug.Assert(dayButton != null, "The dayButton should not be null!");

            if (newDayButtonStyle != null)
            {
                // REMOVE_RTM: Remove null check when Silverlight allows us to re-apply styles
                // Apply the newDayButtonStyle if the DayButton was using the
                // oldDayButtonStyle before
                if (dayButton != null && (dayButton.Style == null || dayButton.Style == oldDayButtonStyle))
                {
                    dayButton.Style = newDayButtonStyle;
                }
            }
        }
        #endregion CalendarDayButtonStyle

        #region CalendarItemStyle
        /// <summary>
        /// Gets or sets the <see cref="Style" /> associated with the control's 
        /// internal <see cref="CalendarItem" /> object.
        /// </summary>
        /// <value>
        /// The current style of the <see cref="CalendarItem" /> object.
        /// </value>
        public Style CalendarItemStyle
        {
            get { return (Style) GetValue(CalendarItemStyleProperty); }
            set { SetValue(CalendarItemStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CalendarItemStyle" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="CalendarItemStyle" /> dependency property.
        /// </value>
        public static readonly DependencyProperty CalendarItemStyleProperty =
            DependencyProperty.Register(
            nameof(CalendarItemStyle),
            typeof(Style),
            typeof(Calendar),
            new PropertyMetadata(OnCalendarItemStyleChanged));

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private static void OnCalendarItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Style newStyle = e.NewValue as Style;
            Style oldStyle = e.OldValue as Style;
            Calendar c = d as Calendar;

            if (newStyle != null && c != null)
            {
                CalendarItem monthControl = c.MonthControl;

                if (monthControl != null)
                {
                    EnsureMonthStyle(monthControl, oldStyle, newStyle);
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="month">Inherited code: Requires comment 1 .</param>
        /// <param name="oldMonthStyle">Inherited code: Requires comment 2 .</param>
        /// <param name="newMonthStyle">Inherited code: Requires comment 3.</param>
        private static void EnsureMonthStyle(CalendarItem month, Style oldMonthStyle, Style newMonthStyle)
        {
            Debug.Assert(month != null, "month should not be null!");

            if (newMonthStyle != null)
            {
                // REMOVE_RTM: Remove null check when Silverlight allows us to re-apply styles
                // Apply the newMonthStyle if the Month was using the
                // oldMonthStyle before
                if (month != null && (month.Style == null || month.Style == oldMonthStyle))
                {
                    month.Style = newMonthStyle;
                }
            }
        }
        #endregion CalendarItemStyle

        #region IsTodayHighlighted
        /// <summary>
        /// Gets or sets a value indicating whether the current date is
        /// highlighted.
        /// </summary>
        /// <value>
        /// True if the current date is highlighted; otherwise, false. The
        /// default is true.
        /// </value>
        public bool IsTodayHighlighted
        {
            get { return (bool) GetValue(IsTodayHighlightedProperty); }
            set { SetValue(IsTodayHighlightedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsTodayHighlighted" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="IsTodayHighlighted" /> dependency property.
        /// </value>
        public static readonly DependencyProperty IsTodayHighlightedProperty =
            DependencyProperty.Register(
            nameof(IsTodayHighlighted),
            typeof(bool),
            typeof(Calendar),
            new PropertyMetadata(true, OnIsTodayHighlightedChanged));

        /// <summary>
        /// IsTodayHighlightedProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// Calendar that changed its IsTodayHighlighted.
        /// </param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsTodayHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null, "c should not be null!");

            if (c.DisplayDate != null)
            {
                int i = DateTimeHelper.CompareYearMonth(c.DisplayDateInternal, DateTime.Today);

                if (i > -2 && i < 2)
                {
                    c.UpdateMonths();
                }
            }
        }
        #endregion IsTodayHighlighted

        #region DisplayMode
        /// <summary>
        /// Gets or sets a value indicating whether the calendar is displayed in
        /// months, years, or decades.
        /// </summary>
        /// <value>
        /// A value indicating what length of time the <see cref="Calendar" /> 
        /// should display.
        /// </value>
        public CalendarMode DisplayMode
        {
            get { return (CalendarMode) GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DisplayMode" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="DisplayMode" /> dependency property.
        /// </value>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
            nameof(DisplayMode),
            typeof(CalendarMode),
            typeof(Calendar),
            new PropertyMetadata(OnDisplayModePropertyChanged));

        /// <summary>
        /// DisplayModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayMode.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null, "c should not be null!");
            CalendarMode mode = (CalendarMode) e.NewValue;
            CalendarMode oldMode = (CalendarMode) e.OldValue;
            CalendarItem monthControl = c.MonthControl;

            if (!c.IsHandlerSuspended(Calendar.DisplayModeProperty))
            {
                if (IsValidDisplayMode(mode))
                {
                    if (monthControl != null)
                    {
                        switch (oldMode)
                        {
                            case CalendarMode.Month:
                                {
                                    c.SelectedYear = c.DisplayDateInternal;
                                    c.SelectedMonth = c.DisplayDateInternal;
                                    break;
                                }
                            case CalendarMode.Year:
                                {
                                    c.DisplayDate = c.SelectedMonth;
                                    c.SelectedYear = c.SelectedMonth;
                                    break;
                                }
                            case CalendarMode.Decade:
                                {
                                    c.DisplayDate = c.SelectedYear;
                                    c.SelectedMonth = c.SelectedYear;
                                    break;
                                }
                        }

                        switch (mode)
                        {
                            case CalendarMode.Month:
                                {
                                    c.OnMonthClick();
                                    break;
                                }
                            case CalendarMode.Year:
                            case CalendarMode.Decade:
                                {
                                    c.OnHeaderClick();
                                    break;
                                }
                        }
                    }
                    c.OnDisplayModeChanged(new CalendarModeChangedEventArgs((CalendarMode)e.OldValue, mode));
                }
                else
                {
                    c.SetValueNoCallback(Calendar.DisplayModeProperty, e.OldValue);
                    throw new ArgumentOutOfRangeException(nameof(d), "DisplayMode value is not valid.");
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="mode">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private static bool IsValidDisplayMode(CalendarMode mode)
        {
            return mode == CalendarMode.Month
                || mode == CalendarMode.Year
                || mode == CalendarMode.Decade;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="args">Inherited code: Requires comment 1.</param>
        private void OnDisplayModeChanged(CalendarModeChangedEventArgs args)
        {
            EventHandler<CalendarModeChangedEventArgs> handler = DisplayModeChanged;

            if (null != handler)
            {
                handler(this, args);
            }
        }
        #endregion DisplayMode

        #region FirstDayOfWeek
        /// <summary>
        /// Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        /// <value>
        /// A <see cref="DayOfWeek" /> representing the beginning of
        /// the week. The default is <see cref="DayOfWeek.Sunday" />.
        /// </value>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek) GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FirstDayOfWeek" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="FirstDayOfWeek" /> dependency property.
        /// </value>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(
            nameof(FirstDayOfWeek),
            typeof(DayOfWeek),
            typeof(Calendar),
            new PropertyMetadata(DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek, OnFirstDayOfWeekChanged));

        /// <summary>
        /// FirstDayOfWeekProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its FirstDayOfWeek.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null, "c should not be null!");

            if (IsValidFirstDayOfWeek(e.NewValue))
            {
                c.UpdateMonths();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(d), "FirstDayOfWeek value is not valid.");
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="value">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private static bool IsValidFirstDayOfWeek(object value)
        {
            DayOfWeek day = (DayOfWeek) value;

            return day == DayOfWeek.Sunday
                || day == DayOfWeek.Monday
                || day == DayOfWeek.Tuesday
                || day == DayOfWeek.Wednesday
                || day == DayOfWeek.Thursday
                || day == DayOfWeek.Friday
                || day == DayOfWeek.Saturday;
        }
        #endregion FirstDayOfWeek

        #region SelectionMode
        /// <summary>
        /// Gets or sets a value that indicates what kind of selections are
        /// allowed.
        /// </summary>
        /// <value>
        /// A value that indicates the current selection mode. The default is
        /// <see cref="CalendarSelectionMode.SingleDate" />.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property determines whether the Calendar allows no selection,
        /// selection of a single date, or selection of multiple dates.  The
        /// selection mode is specified with the CalendarSelectionMode
        /// enumeration.
        /// </para>
        /// <para>
        /// When this property is changed, all selected dates will be cleared.
        /// </para>
        /// </remarks>
        public CalendarSelectionMode SelectionMode
        {
            get { return (CalendarSelectionMode) GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectionMode" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="SelectionMode" /> dependency property.
        /// </value>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
            nameof(SelectionMode),
            typeof(CalendarSelectionMode),
            typeof(Calendar),
            new PropertyMetadata(OnSelectionModeChanged));

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null, "c should not be null!");

            if (IsValidSelectionMode(e.NewValue))
            {
                c.SetValueNoCallback(Calendar.SelectedDateProperty, null);
                c.SelectedDates.Clear();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(d), "SelectionMode value is not valid.");
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="value">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private static bool IsValidSelectionMode(object value)
        {
            CalendarSelectionMode mode = (CalendarSelectionMode) value;

            return mode == CalendarSelectionMode.SingleDate
                || mode == CalendarSelectionMode.SingleRange
                || mode == CalendarSelectionMode.MultipleRange
                || mode == CalendarSelectionMode.None;
        }
        #endregion SelectionMode

        #region SelectedDate
        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        /// <value>The date currently selected. The default is null.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given date is outside the range specified by <see cref="DisplayDateStart" />
        /// and <see cref="DisplayDateEnd" />
        /// -or- The given date is in the <see cref="BlackoutDates" /> collection.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If set to anything other than null when <see cref="SelectionMode" /> is 
        /// set to <see cref="CalendarSelectionMode.None" />.
        /// </exception>
        /// <remarks>
        /// Use this property when SelectionMode is set to SingleDate.  In other
        /// modes, this property will always be the first date in SelectedDates.
        /// </remarks>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime? SelectedDate
        {
            get { return (DateTime?) GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedDate" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="SelectedDate" /> dependency property.
        /// </value>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
            nameof(SelectedDate),
            typeof(DateTime?),
            typeof(Calendar),
            new PropertyMetadata(OnSelectedDateChanged));

        /// <summary>
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its SelectedDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null, "c should not be null!");

            if (!c.IsHandlerSuspended(Calendar.SelectedDateProperty))
            {
                if (c.SelectionMode != CalendarSelectionMode.None)
                {
                    DateTime? addedDate;

                    addedDate = (DateTime?) e.NewValue;

                    if (IsValidDateSelection(c, addedDate))
                    {
                        if (addedDate == null)
                        {
                            c.SelectedDates.Clear();
                        }
                        else
                        {
                            if (addedDate.HasValue && !(c.SelectedDates.Count > 0 && c.SelectedDates[0] == addedDate.Value))
                            {
                                foreach (DateTime item in c.SelectedDates)
                                {
                                    c.RemovedItems.Add(item);
                                }
                                c.SelectedDates.ClearInternal();
                                // the value is added as a range so that the
                                // SelectedDatesChanged event can be thrown with
                                // all the removed items
                                c.SelectedDates.AddRange(addedDate.Value, addedDate.Value);
                            }
                        }

                        // We update the LastSelectedDate for only the Single
                        // mode.  For the other modes it automatically gets
                        // updated when the HoverEnd is updated.
                        if (c.SelectionMode == CalendarSelectionMode.SingleDate)
                        {
                            c.LastSelectedDate = addedDate;
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(d), "SelectedDate value is not valid.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The SelectedDate property cannot be set when the selection mode is None.");
                }
            }
        }
        #endregion SelectedDate

        #region public SelectedDatesCollection SelectedDates
        /// <summary>
        /// Gets a collection of selected dates.
        /// </summary>
        /// <value>
        /// A <see cref="SelectedDatesCollection" /> object that contains the 
        /// currently selected dates. The default is an empty collection.
        /// </value>
        /// <remarks>
        /// Dates can be added to the collection either individually or in a
        /// range using the AddRange method.  Depending on the value of the
        /// SelectionMode property, adding a date or range to the collection may
        /// cause it to be cleared.  The following table lists how
        /// CalendarSelectionMode affects the SelectedDates property.
        /// 
        ///     CalendarSelectionMode   Description
        ///     None                    No selections are allowed.  SelectedDate
        ///                             cannot be set and no values can be added
        ///                             to SelectedDates.
        ///                             
        ///     SingleDate              Only a single date can be selected,
        ///                             either by setting SelectedDate or the
        ///                             first value in SelectedDates.  AddRange
        ///                             cannot be used.
        ///                             
        ///     SingleRange             A single range of dates can be selected.
        ///                             Setting SelectedDate, adding a date
        ///                             individually to SelectedDates, or using
        ///                             AddRange will clear all previous values
        ///                             from SelectedDates.
        ///     MultipleRange           Multiple non-contiguous ranges of dates
        ///                             can be selected. Adding a date
        ///                             individually to SelectedDates or using
        ///                             AddRange will not clear SelectedDates.
        ///                             Setting SelectedDate will still clear
        ///                             SelectedDates, but additional dates or
        ///                             range can then be added.  Adding a range
        ///                             that includes some dates that are
        ///                             already selected or overlaps with
        ///                             another range results in the union of
        ///                             the ranges and does not cause an
        ///                             exception.
        /// </remarks>
        public SelectedDatesCollection SelectedDates { get; private set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private static bool IsSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != e.RemovedItems.Count)
            {
                return true;
            }
            foreach (DateTime addedDate in e.AddedItems)
            {
                if (!e.RemovedItems.Contains(addedDate))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
        {
            if (IsSelectionChanged(e))
            {
                EventHandler<SelectionChangedEventArgs> handler = SelectedDatesChanged;

                if (null != handler)
                {
                    handler(this, e);
                }

                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                {
                    CalendarAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as CalendarAutomationPeer;
                    if (peer != null)
                    {
                        peer.RaiseSelectionEvents(e);
                    }
                }
            }
        }
        #endregion public SelectedDatesCollection SelectedDates

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal Collection<DateTime> RemovedItems { get; set; }

        #region internal DateTime? LastSelectedDate
        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal DateTime? LastSelectedDateInternal { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal DateTime? LastSelectedDate
        {
            get { return LastSelectedDateInternal; }
            set
            {
                LastSelectedDateInternal = value;

                if (SelectionMode == CalendarSelectionMode.None)
                {
                    if (FocusButton != null)
                    {
                        FocusButton.IsCurrent = false;
                    }
                    FocusButton = FindDayButtonFromDay(LastSelectedDate.Value);
                    if (FocusButton != null)
                    {
                        FocusButton.IsCurrent = HasFocusInternal;
                    }
                }
            }
        }
        #endregion internal DateTime? LastSelectedDate

        #region internal DateTime SelectedMonth
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private DateTime _selectedMonth;

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal DateTime SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                int monthDifferenceStart = DateTimeHelper.CompareYearMonth(value, DisplayDateRangeStart);
                int monthDifferenceEnd = DateTimeHelper.CompareYearMonth(value, DisplayDateRangeEnd);

                if (monthDifferenceStart >= 0 && monthDifferenceEnd <= 0)
                {
                    _selectedMonth = DateTimeHelper.DiscardDayTime(value);
                }
                else
                {
                    if (monthDifferenceStart < 0)
                    {
                        _selectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
                    }
                    else
                    {
                        Debug.Assert(monthDifferenceEnd > 0, "monthDifferenceEnd should be greater than 0!");
                        _selectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
                    }
                }
            }
        }
        #endregion internal DateTime SelectedMonth

        #region internal DateTime SelectedYear
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private DateTime _selectedYear;

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal DateTime SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                if (value.Year < DisplayDateRangeStart.Year)
                {
                    _selectedYear = DisplayDateRangeStart;
                }
                else
                {
                    if (value.Year > DisplayDateRangeEnd.Year)
                    {
                        _selectedYear = DisplayDateRangeEnd;
                    }
                    else
                    {
                        _selectedYear = value;
                    }
                }
            }
        }
        #endregion internal DateTime SelectedYear

        /// <summary>
        /// Gets a collection of dates that are marked as not selectable.
        /// </summary>
        /// <value>
        /// A collection of dates that cannot be selected. The default value is
        /// an empty collection.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Adding a date to this collection when it is already selected or
        /// adding a date outside the range specified by DisplayDateStart and
        /// DisplayDateEnd.
        /// </exception>
        /// <remarks>
        /// <para>
        /// Dates in this collection will appear as disabled on the calendar.
        /// </para>
        /// <para>
        /// To make all past dates not selectable, you can use the
        /// AddDatesInPast method provided by the collection returned by this
        /// property.
        /// </para>
        /// </remarks>
        public CalendarBlackoutDatesCollection BlackoutDates { get; private set; }

        #region DisplayDate
        /// <summary>
        /// Gets or sets the date to display.
        /// </summary>
        /// <value>The date to display.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given date is not in the range specified by <see cref="DisplayDateStart" />
        /// and <see cref="DisplayDateEnd" />.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This property allows the developer to specify a date to display.  If
        /// this property is a null reference (Nothing in Visual Basic),
        /// SelectedDate is displayed.  If SelectedDate is also a null reference
        /// (Nothing in Visual Basic), Today is displayed.  The default is
        /// Today.
        /// </para>
        /// <para>
        /// To set this property in XAML, use a date specified in the format
        /// yyyy/mm/dd.  The mm and dd components must always consist of two
        /// characters, with a leading zero if necessary.  For instance, the
        /// month of May should be specified as 05.
        /// </para>
        /// </remarks>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime DisplayDate
        {
            get { return (DateTime) GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DisplayDate" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="DisplayDate" /> dependency property.
        /// </value>
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register(
            nameof(DisplayDate),
            typeof(DateTime),
            typeof(Calendar),
            new PropertyMetadata(DateTime.Today, OnDisplayDateChanged));

        /// <summary>
        /// DisplayDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateDisplayDate((Calendar)d, (DateTime)e.NewValue, (DateTime)e.OldValue);
        }

        /// <summary>
        /// Updates the display date.
        /// </summary>
        /// <param name="c">Calendar instance.</param>
        /// <param name="addedDate">Added date.</param>
        /// <param name="removedDate">Removed date.</param>
        private static void UpdateDisplayDate(Calendar c, DateTime addedDate, DateTime removedDate)
        {
            Debug.Assert(c != null, "c should not be null!");

            // If DisplayDate < DisplayDateStart, DisplayDate = DisplayDateStart
            if (DateTime.Compare(addedDate, c.DisplayDateRangeStart) < 0)
            {
                c.DisplayDate = c.DisplayDateRangeStart;
                return;
            }

            // If DisplayDate > DisplayDateEnd, DisplayDate = DisplayDateEnd
            if (DateTime.Compare(addedDate, c.DisplayDateRangeEnd) > 0)
            {
                c.DisplayDate = c.DisplayDateRangeEnd;
                return;
            }

            c.DisplayDateInternal = DateTimeHelper.DiscardDayTime(addedDate);
            c.UpdateMonths();
            c.OnDisplayDate(new CalendarDateChangedEventArgs(removedDate, addedDate));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        private void OnDisplayDate(CalendarDateChangedEventArgs e)
        {
            EventHandler<CalendarDateChangedEventArgs> handler = DisplayDateChanged;
            if (null != handler)
            {
                handler(this, e);
            }
        }
        #endregion DisplayDate

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        internal DateTime DisplayDateInternal { get; private set; }

        #region DisplayDateStart
        /// <summary>
        /// Gets or sets the first date to be displayed.
        /// </summary>
        /// <value>The first date to display.</value>
        /// <remarks>
        /// To set this property in XAML, use a date specified in the format
        /// yyyy/mm/dd.  The mm and dd components must always consist of two
        /// characters, with a leading zero if necessary.  For instance, the
        /// month of May should be specified as 05.
        /// </remarks>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime? DisplayDateStart
        {
            get { return (DateTime?) GetValue(DisplayDateStartProperty); }
            set { SetValue(DisplayDateStartProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DisplayDateStart" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="DisplayDateStart" /> dependency property.
        /// </value>
        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register(
            nameof(DisplayDateStart),
            typeof(DateTime?),
            typeof(Calendar),
            new PropertyMetadata(OnDisplayDateStartChanged));

        /// <summary>
        /// DisplayDateStartProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDateStart.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null, "c should not be null!");

            if (!c.IsHandlerSuspended(Calendar.DisplayDateStartProperty))
            {
                DateTime? newValue = e.NewValue as DateTime?;

                if (newValue.HasValue)
                {
                    // DisplayDateStart coerces to the value of the
                    // SelectedDateMin if SelectedDateMin < DisplayDateStart
                    DateTime? selectedDateMin = SelectedDateMin(c);

                    if (selectedDateMin.HasValue && DateTime.Compare(selectedDateMin.Value, newValue.Value) < 0)
                    {
                        c.DisplayDateStart = selectedDateMin.Value;
                        return;
                    }

                    // if DisplayDateStart > DisplayDateEnd,
                    // DisplayDateEnd = DisplayDateStart
                    if (DateTime.Compare(newValue.Value, c.DisplayDateRangeEnd) > 0)
                    {
                        c.DisplayDateEnd = c.DisplayDateStart;
                    }

                    // If DisplayDate < DisplayDateStart,
                    // DisplayDate = DisplayDateStart
                    if (DateTimeHelper.CompareYearMonth(newValue.Value, c.DisplayDateInternal) > 0)
                    {
                        c.DisplayDate = newValue.Value;
                    }
                }
                c.UpdateMonths();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="cal">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private static DateTime? SelectedDateMin(Calendar cal)
        {
            DateTime selectedDateMin;

            if (cal.SelectedDates.Count > 0)
            {
                selectedDateMin = cal.SelectedDates[0];
                Debug.Assert(DateTime.Compare(cal.SelectedDate.Value, selectedDateMin) == 0, "The SelectedDate should be the minimum selected date!");
            }
            else
            {
                return null;
            }

            foreach (DateTime selectedDate in cal.SelectedDates)
            {
                if (DateTime.Compare(selectedDate, selectedDateMin) < 0)
                {
                    selectedDateMin = selectedDate;
                }
            }
            return selectedDateMin;
        }
        #endregion DisplayDateStart

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        internal DateTime DisplayDateRangeStart
        {
            get { return DisplayDateStart.GetValueOrDefault(DateTime.MinValue); }
        }

        #region DisplayDateEnd
        /// <summary>
        /// Gets or sets the last date to be displayed.
        /// </summary>
        /// <value>The last date to display.</value>
        /// <remarks>
        /// To set this property in XAML, use a date specified in the format
        /// yyyy/mm/dd.  The mm and dd components must always consist of two
        /// characters, with a leading zero if necessary.  For instance, the
        /// month of May should be specified as 05.
        /// </remarks>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime? DisplayDateEnd
        {
            get { return (DateTime?) GetValue(DisplayDateEndProperty); }
            set { SetValue(DisplayDateEndProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DisplayDateEnd" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="DisplayDateEnd" /> dependency property.
        /// </value>
        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(
            nameof(DisplayDateEnd),
            typeof(DateTime?),
            typeof(Calendar),
            new PropertyMetadata(OnDisplayDateEndChanged));

        /// <summary>
        /// DisplayDateEndProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDateEnd.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null, "c should not be null!");

            if (!c.IsHandlerSuspended(Calendar.DisplayDateEndProperty))
            {
                DateTime? newValue = e.NewValue as DateTime?;

                if (newValue.HasValue)
                {
                    // DisplayDateEnd coerces to the value of the
                    // SelectedDateMax if SelectedDateMax > DisplayDateEnd
                    DateTime? selectedDateMax = SelectedDateMax(c);

                    if (selectedDateMax.HasValue && DateTime.Compare(selectedDateMax.Value, newValue.Value) > 0)
                    {
                        c.DisplayDateEnd = selectedDateMax.Value;
                        return;
                    }

                    // if DisplayDateEnd < DisplayDateStart,
                    // DisplayDateEnd = DisplayDateStart
                    if (DateTime.Compare(newValue.Value, c.DisplayDateRangeStart) < 0)
                    {
                        c.DisplayDateEnd = c.DisplayDateStart;
                        return;
                    }

                    // If DisplayDate > DisplayDateEnd,
                    // DisplayDate = DisplayDateEnd
                    if (DateTimeHelper.CompareYearMonth(newValue.Value, c.DisplayDateInternal) < 0)
                    {
                        c.DisplayDate = newValue.Value;
                    }
                }
                c.UpdateMonths();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="cal">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private static DateTime? SelectedDateMax(Calendar cal)
        {
            DateTime selectedDateMax;

            if (cal.SelectedDates.Count > 0)
            {
                selectedDateMax = cal.SelectedDates[0];
                Debug.Assert(DateTime.Compare(cal.SelectedDate.Value, selectedDateMax) == 0, "The SelectedDate should be the maximum SelectedDate!");
            }
            else
            {
                return null;
            }

            foreach (DateTime selectedDate in cal.SelectedDates)
            {
                if (DateTime.Compare(selectedDate, selectedDateMax) > 0)
                {
                    selectedDateMax = selectedDate;
                }
            }
            return selectedDateMax;
        }
        #endregion DisplayDateEnd

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        internal DateTime DisplayDateRangeEnd
        {
            get { return DisplayDateEnd.GetValueOrDefault(DateTime.MaxValue); }
        }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal DateTime? HoverStart { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal int? HoverStartIndex { get; set; }

        #region internal DateTime? HoverEnd
        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal DateTime? HoverEndInternal { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal DateTime? HoverEnd
        {
            get { return HoverEndInternal; }
            set
            {
                HoverEndInternal = value;
                LastSelectedDate = value;
            }
        }
        #endregion internal DateTime? HoverEnd

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal int? HoverEndIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        internal bool HasFocusInternal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        internal bool IsMouseSelection { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isShiftPressed;

        /// <summary>
        /// Gets or sets a value indicating whether DatePicker should change its 
        /// DisplayDate because of a SelectedDate change on its Calendar.
        /// </summary>
        internal bool DatePickerDisplayDateFlag { get; set; }

        /// <summary>
        /// Occurs when the collection returned by the
        /// <see cref="SelectedDates" /> property is changed.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDatesChanged;

        /// <summary>
        /// Occurs when the <see cref="DisplayDate" /> property is changed.
        /// </summary>
        /// <remarks>
        /// This event occurs after DisplayDate is assigned its new value.
        /// </remarks>
        public event EventHandler<CalendarDateChangedEventArgs> DisplayDateChanged;

        /// <summary>
        /// Occurs when the <see cref="DisplayMode" /> property is changed.
        /// </summary>
        public event EventHandler<CalendarModeChangedEventArgs> DisplayModeChanged;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal event MouseButtonEventHandler DayButtonMouseUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="Calendar" /> class.
        /// </summary>
        public Calendar()
        {
            DefaultStyleKey = typeof(Calendar);
            UpdateDisplayDate(this, this.DisplayDate, DateTime.MinValue);
            GotFocus += new RoutedEventHandler(Calendar_GotFocus);
            LostFocus += new RoutedEventHandler(Calendar_LostFocus);
            IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
#if MIGRATION
            MouseLeftButtonUp += new MouseButtonEventHandler(Calendar_MouseLeftButtonUp);
#else
            PointerReleased += new MouseButtonEventHandler(Calendar_MouseLeftButtonUp);
#endif
            BlackoutDates = new CalendarBlackoutDatesCollection(this);
            SelectedDates = new SelectedDatesCollection(this);
            RemovedItems = new Collection<DateTime>();
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="Calendar" /> when a new
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            Root = GetTemplateChild(ElementRoot) as Panel;

            SelectedMonth = DisplayDate;
            SelectedYear = DisplayDate;

            if (Root != null)
            {
                CalendarItem month = GetTemplateChild(ElementMonth) as CalendarItem;

                if (month != null)
                {
                    month.Owner = this;

                    if (CalendarItemStyle != null)
                    {
                        month.Style = CalendarItemStyle;
                    }
                    // REMOVE_RTM:If MultiCalendar is supported, while creating other months the _DisplayDate property should be changed
                }
            }

            SizeChanged += new SizeChangedEventHandler(Calendar_SizeChanged);
            KeyDown += new KeyEventHandler(Calendar_KeyDown);
            KeyUp += new KeyEventHandler(Calendar_KeyUp);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.Assert(sender is Calendar, "The sender should be a Calendar!");

            RectangleGeometry rg = new RectangleGeometry();
            rg.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

            if (Root != null)
            {
                Root.Clip = rg;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void ResetStates()
        {
            CalendarDayButton d;
            CalendarItem monthControl = MonthControl;
            int count = RowsPerMonth * ColumnsPerMonth;
            if (monthControl != null)
            {
                if (monthControl.MonthView != null)
                {
                    for (int childIndex = ColumnsPerMonth; childIndex < count; childIndex++)
                    {
                        d = monthControl.MonthView.Children[childIndex] as CalendarDayButton;
                        d.IgnoreMouseOverState();
                    }
                }
            }
        }

        /// <summary>
        /// Provides a text representation of the selected date.
        /// </summary>
        /// <returns>
        /// A text representation of the selected date, or an empty string if
        /// <see cref="SelectedDate" /> is null.
        /// </returns>
        public override string ToString()
        {
            if (SelectedDate != null)
            {
                return SelectedDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat());
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns a <see cref="CalendarAutomationPeer" /> for use by the Silverlight 
        /// automation infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="CalendarAutomationPeer" /> for the <see cref="Calendar" /> 
        /// object.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates a new CalendarAutomationPeer instance if one has
        /// not been created for the control instance; otherwise, it returns the
        /// CalendarAutomationPeer previously created.
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
            return new CalendarAutomationPeer(this);
        }

        /// <summary>
        ///  Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(e.NewValue is bool, "NewValue should be a boolean!");
            bool isEnabled = (bool) e.NewValue;

            if (MonthControl != null)
            {
                MonthControl.UpdateDisabledGrid(isEnabled);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="day">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        internal CalendarDayButton FindDayButtonFromDay(DateTime day)
        {
            CalendarDayButton b;
            DateTime? d;
            CalendarItem monthControl = MonthControl;

            // REMOVE_RTM: should be updated if we support MultiCalendar
            int count = RowsPerMonth * ColumnsPerMonth;
            if (monthControl != null)
            {
                if (monthControl.MonthView != null)
                {
                    for (int childIndex = ColumnsPerMonth; childIndex < count; childIndex++)
                    {
                        b = monthControl.MonthView.Children[childIndex] as CalendarDayButton;
                        d = b.DataContext as DateTime?;

                        if (d.HasValue)
                        {
                            if (DateTimeHelper.CompareDays(d.Value, day) == 0)
                            {
                                return b;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void UpdateMonths()
        {
            CalendarItem monthControl = MonthControl;
            if (monthControl != null)
            {
                switch (DisplayMode)
                {
                    case CalendarMode.Month:
                        {
                            monthControl.UpdateMonthMode();
                            break;
                        }
                    case CalendarMode.Year:
                        {
                            monthControl.UpdateYearMode();
                            break;
                        }
                    case CalendarMode.Decade:
                        {
                            monthControl.UpdateDecadeMode();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="selectedMonth">Inherited code: Requires comment 1.</param>
        private void OnSelectedMonthChanged(DateTime? selectedMonth)
        {
            if (selectedMonth.HasValue)
            {
                Debug.Assert(DisplayMode == CalendarMode.Year, "DisplayMode should be Year!");
                SelectedMonth = selectedMonth.Value;
                UpdateMonths();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="selectedYear">Inherited code: Requires comment 1.</param>
        private void OnSelectedYearChanged(DateTime? selectedYear)
        {
            if (selectedYear.HasValue)
            {
                Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade!");
                SelectedYear = selectedYear.Value;
                UpdateMonths();
            }
        }

        #region Selection
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="cal">Inherited code: Requires comment 1.</param>
        /// <param name="value">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        internal static bool IsValidDateSelection(Calendar cal, object value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                if (cal.BlackoutDates.Contains((DateTime) value))
                {
                    return false;
                }
                else
                {
                    if (DateTime.Compare((DateTime) value, cal.DisplayDateRangeStart) < 0)
                    {
                        cal.SetValueNoCallback(Calendar.DisplayDateStartProperty, value);
                    }
                    else if (DateTime.Compare((DateTime) value, cal.DisplayDateRangeEnd) > 0)
                    {
                        cal.SetValueNoCallback(Calendar.DisplayDateEndProperty, value);
                    }
                    return true;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="cal">Inherited code: Requires comment 1.</param>
        /// <param name="value">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        private static bool IsValidKeyboardSelection(Calendar cal, object value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                if (cal.BlackoutDates.Contains((DateTime) value))
                {
                    return false;
                }
                else
                {
                    return (DateTime.Compare((DateTime) value, cal.DisplayDateRangeStart) >= 0 && DateTime.Compare((DateTime) value, cal.DisplayDateRangeEnd) <= 0);
                }
            }
        }

        /// <summary>
        /// This method adds the days that were selected by Keyboard to the
        /// SelectedDays Collection.
        /// </summary>
        private void AddSelection()
        {
            if (HoverEnd != null && HoverStart != null)
            {
                foreach (DateTime item in SelectedDates)
                {
                    RemovedItems.Add(item);
                }

                SelectedDates.ClearInternal();
                // In keyboard selection, we are sure that the collection does
                // not include any blackout days
                SelectedDates.AddRange(HoverStart.Value, HoverEnd.Value);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="shift">Inherited code: Requires comment 1.</param>
        /// <param name="lastSelectedDate">Inherited code: Requires comment 2.</param>
        /// <param name="index">Inherited code: Requires comment 3.</param>
        private void ProcessSelection(bool shift, DateTime? lastSelectedDate, int? index)
        {
            if (SelectionMode == CalendarSelectionMode.None && lastSelectedDate != null)
            {
                OnDayClick(lastSelectedDate.Value);
                return;
            }
            if (lastSelectedDate != null && IsValidKeyboardSelection(this, lastSelectedDate.Value))
            {
                if (SelectionMode == CalendarSelectionMode.SingleRange || SelectionMode == CalendarSelectionMode.MultipleRange)
                {
                    foreach (DateTime item in SelectedDates)
                    {
                        RemovedItems.Add(item);
                    }
                    SelectedDates.ClearInternal();
                    if (shift)
                    {
                        CalendarDayButton b;
                        _isShiftPressed = true;
                        if (HoverStart == null)
                        {
                            if (LastSelectedDate != null)
                            {
                                HoverStart = LastSelectedDate;
                            }
                            else
                            {
                                if (DateTimeHelper.CompareYearMonth(DisplayDateInternal, DateTime.Today) == 0)
                                {
                                    HoverStart = DateTime.Today;
                                }
                                else
                                {
                                    HoverStart = DisplayDateInternal;
                                }
                            }

                            b = FindDayButtonFromDay(HoverStart.Value);
                            if (b != null)
                            {
                                HoverStartIndex = b.Index;
                            }
                        }
                        // the index of the SelectedDate is always the last
                        // selectedDate's index
                        UnHighlightDays();
                        // If we hit a BlackOutDay with keyboard we do not
                        // update the HoverEnd
                        CalendarDateRange range;

                        if (DateTime.Compare(HoverStart.Value, lastSelectedDate.Value) < 0)
                        {
                            range = new CalendarDateRange(HoverStart.Value, lastSelectedDate.Value);
                        }
                        else
                        {
                            range = new CalendarDateRange(lastSelectedDate.Value, HoverStart.Value);
                        }

                        if (!BlackoutDates.ContainsAny(range))
                        {
                            HoverEnd = lastSelectedDate;

                            if (index.HasValue)
                            {
                                HoverEndIndex += index;
                            }
                            else
                            {
                                // For Home, End, PageUp and PageDown Keys there
                                // is no easy way to predict the index value
                                b = FindDayButtonFromDay(HoverEndInternal.Value);

                                if (b != null)
                                {
                                    HoverEndIndex = b.Index;
                                }
                            }
                        }

                        OnDayClick(HoverEnd.Value);
                        HighlightDays();
                    }
                    else
                    {
                        HoverStart = lastSelectedDate;
                        HoverEnd = lastSelectedDate;
                        AddSelection();
                        OnDayClick(lastSelectedDate.Value);
                    }
                }
                else
                {
                    // ON CLEAR 
                    LastSelectedDate = lastSelectedDate.Value;
                    if (SelectedDates.Count > 0)
                    {
                        SelectedDates[0] = lastSelectedDate.Value;
                    }
                    else
                    {
                        SelectedDates.Add(lastSelectedDate.Value);
                    }
                    OnDayClick(lastSelectedDate.Value);
                }
            }
        }

        /// <summary>
        /// This method highlights the days in MultiSelection mode without
        /// adding them to the SelectedDates collection.
        /// </summary>
        internal void HighlightDays()
        {
            if (HoverEnd != null && HoverStart != null)
            {
                int startIndex, endIndex, i;
                CalendarDayButton b;
                DateTime? d;
                CalendarItem monthControl = MonthControl;

                // This assumes a contiguous set of dates:
                if (HoverEndIndex != null && HoverStartIndex != null)
                {
                    SortHoverIndexes(out startIndex, out endIndex);

                    for (i = startIndex; i <= endIndex; i++)
                    {
                        b = monthControl.MonthView.Children[i] as CalendarDayButton;
                        b.IsSelected = true;
                        d = b.DataContext as DateTime?;

                        if (d.HasValue && DateTimeHelper.CompareDays(HoverEnd.Value, d.Value) == 0)
                        {
                            if (FocusButton != null)
                            {
                                FocusButton.IsCurrent = false;
                            }
                            b.IsCurrent = HasFocusInternal;
                            FocusButton = b;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method un-highlights the days that were hovered over but not
        /// added to the SelectedDates collection or un-highlighted the
        /// previously selected days in SingleRange Mode.
        /// </summary>
        internal void UnHighlightDays()
        {
            if (HoverEnd != null && HoverStart != null)
            {
                CalendarItem monthControl = MonthControl;
                CalendarDayButton b;
                DateTime? d;

                if (HoverEndIndex != null && HoverStartIndex != null)
                {
                    int startIndex, endIndex, i;
                    SortHoverIndexes(out startIndex, out endIndex);

                    if (SelectionMode == CalendarSelectionMode.MultipleRange)
                    {
                        for (i = startIndex; i <= endIndex; i++)
                        {
                            b = monthControl.MonthView.Children[i] as CalendarDayButton;
                            d = b.DataContext as DateTime?;

                            if (d.HasValue)
                            {
                                if (!SelectedDates.Contains(d.Value))
                                {
                                    b.IsSelected = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // It is SingleRange
                        for (i = startIndex; i <= endIndex; i++)
                        {
                            (monthControl.MonthView.Children[i] as CalendarDayButton).IsSelected = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="startIndex">Inherited code: Requires comment 1.</param>
        /// <param name="endIndex">Inherited code: Requires comment 2.</param>
        internal void SortHoverIndexes(out int startIndex, out int endIndex)
        {
            // not comparing indexes since the two days may not be on the same
            // month
            // REMOVE_RTM: this assumes that the two indexes are on the same month
            // should be updated if MultiCalendar support is added

            if (DateTimeHelper.CompareDays(HoverEnd.Value, HoverStart.Value) > 0)
            {
                startIndex = HoverStartIndex.Value;
                endIndex = HoverEndIndex.Value;
            }
            else
            {
                startIndex = HoverEndIndex.Value;
                endIndex = HoverStartIndex.Value;
            }
        }
        #endregion Selection

        #region Mouse Events
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!HasFocusInternal)
            {
                Focus();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void OnHeaderClick()
        {
            Debug.Assert(DisplayMode == CalendarMode.Year || DisplayMode == CalendarMode.Decade, "The DisplayMode should be Year or Decade");
            CalendarItem monthControl = MonthControl;
            if (monthControl != null && monthControl.MonthView != null && monthControl.YearView != null)
            {
                monthControl.MonthView.Visibility = Visibility.Collapsed;
                monthControl.YearView.Visibility = Visibility.Visible;
                UpdateMonths();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void OnNextClick()
        {
            if (DisplayMode == CalendarMode.Month && DisplayDate != null)
            {
                DateTime? d = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(DisplayDate), 1);
                if (d.HasValue)
                {
                    if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0)
                    {
                        LastSelectedDate = d.Value;
                    }
                    DisplayDate = d.Value;
                }
            }
            else
            {
                if (DisplayMode == CalendarMode.Year)
                {
                    DateTime? d = DateTimeHelper.AddYears(new DateTime(SelectedMonth.Year, 1, 1), 1);

                    if (d.HasValue)
                    {
                        SelectedMonth = d.Value;
                    }
                    else
                    {
                        SelectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
                    }
                }
                else
                {
                    Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade");

                    DateTime? d = DateTimeHelper.AddYears(new DateTime(SelectedYear.Year, 1, 1), 10);

                    if (d.HasValue)
                    {
                        int decade = Math.Max(1, DateTimeHelper.DecadeOfDate(d.Value));
                        SelectedYear = new DateTime(decade, 1, 1);
                    }
                    else
                    {
                        SelectedYear = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
                    }
                }
                UpdateMonths();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void OnPreviousClick()
        {
            if (DisplayMode == CalendarMode.Month && DisplayDate != null)
            {
                DateTime? d = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(DisplayDate), -1);
                if (d.HasValue)
                {
                    if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0)
                    {
                        LastSelectedDate = d.Value;
                    }
                    DisplayDate = d.Value;
                }
            }
            else
            {
                if (DisplayMode == CalendarMode.Year)
                {
                    DateTime? d = DateTimeHelper.AddYears(new DateTime(SelectedMonth.Year, 1, 1), -1);

                    if (d.HasValue)
                    {
                        SelectedMonth = d.Value;
                    }
                    else
                    {
                        SelectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
                    }
                }
                else
                {
                    Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade!");

                    DateTime? d = DateTimeHelper.AddYears(new DateTime(SelectedYear.Year, 1, 1), -10);

                    if (d.HasValue)
                    {
                        int decade = Math.Max(1, DateTimeHelper.DecadeOfDate(d.Value));
                        SelectedYear = new DateTime(decade, 1, 1);
                    }
                    else
                    {
                        SelectedYear = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
                    }
                }
                UpdateMonths();
            }
        }

        /// <summary>
        /// If the day is a trailing day, Update the DisplayDate.
        /// </summary>
        /// <param name="selectedDate">Inherited code: Requires comment.</param>
        internal void OnDayClick(DateTime selectedDate)
        {
            Debug.Assert(DisplayMode == CalendarMode.Month, "DisplayMode should be Month!");
            int i = DateTimeHelper.CompareYearMonth(selectedDate, DisplayDateInternal);

            if (SelectionMode == CalendarSelectionMode.None)
            {
                LastSelectedDate = selectedDate;
            }

            if (i > 0)
            {
                OnNextClick();
            }
            else if (i < 0)
            {
                OnPreviousClick();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void OnMonthClick()
        {
            CalendarItem monthControl = MonthControl;
            if (monthControl != null && monthControl.YearView != null && monthControl.MonthView != null)
            {
                monthControl.YearView.Visibility = Visibility.Collapsed;
                monthControl.MonthView.Visibility = Visibility.Visible;

                if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, DisplayDate) != 0)
                {
                    LastSelectedDate = DisplayDate;
                }

                UpdateMonths();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void OnDayButtonMouseUp(MouseButtonEventArgs e)
        {
            MouseButtonEventHandler handler = DayButtonMouseUp;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Default mouse wheel handler for the calendar control.
        /// </summary>
        /// <param name="e">Mouse wheel event args.</param>
#if MIGRATION

        protected override void OnMouseWheel(MouseWheelEventArgs e)
#else
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseWheel(e);
#else
            base.OnPointerWheelChanged(e);
#endif
            if (!e.Handled)
            {
                bool ctrl, shift;
                CalendarExtensions.GetMetaKeyState(out ctrl, out shift);

                if (!ctrl)
                {
                    if (e.Delta > 0)
                    {
                        ProcessPageUpKey(false);
                    }
                    else
                    {
                        ProcessPageDownKey(false);
                    }
                }
                else
                {
                    if (e.Delta > 0)
                    {
                        ProcessDownKey(ctrl, shift);
                    }
                    else
                    {
                        ProcessUpKey(ctrl, shift);
                    }
                }
                e.Handled = true;
            }
        }
#endregion Mouse Events

        #region Key Events
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Calendar_KeyDown(object sender, KeyEventArgs e)
        {
            Calendar c = sender as Calendar;
            Debug.Assert(c != null, "c should not be null!");

            if (!e.Handled && c.IsEnabled)
            {
                e.Handled = ProcessCalendarKey(e);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        internal bool ProcessCalendarKey(KeyEventArgs e)
        {
            if (DisplayMode == CalendarMode.Month)
            {
                if (LastSelectedDate.HasValue && DisplayDateInternal != null)
                {
                    // If a blackout day is inactive, when clicked on it, the
                    // previous inactive day which is not a blackout day can get
                    // the focus.  In this case we should allow keyboard
                    // functions on that inactive day
                    if (DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, DisplayDateInternal) != 0 && FocusButton != null && !FocusButton.IsInactive)
                    {
                        return true;
                    }
                }
            }

            // Some keys (e.g. Left/Right) need to be translated in RightToLeft mode
            Key invariantKey = InteractionHelper.GetLogicalKey(FlowDirection, e.Key);
            
            bool ctrl, shift;
            CalendarExtensions.GetMetaKeyState(out ctrl, out shift);

            switch (invariantKey)
            {
                case Key.Up:
                    {
                        ProcessUpKey(ctrl, shift);
                        return true;
                    }
                case Key.Down:
                    {
                        ProcessDownKey(ctrl, shift);
                        return true;
                    }
                case Key.Left:
                    {
                        ProcessLeftKey(shift);
                        return true;
                    }
                case Key.Right:
                    {
                        ProcessRightKey(shift);
                        return true;
                    }
                case Key.PageDown:
                    {
                        ProcessPageDownKey(shift);
                        return true;
                    }
                case Key.PageUp:
                    {
                        ProcessPageUpKey(shift);
                        return true;
                    }
                case Key.Home:
                    {
                        ProcessHomeKey(shift);
                        return true;
                    }
                case Key.End:
                    {
                        ProcessEndKey(shift);
                        return true;
                    }
                case Key.Enter:
                case Key.Space:
                    {
                        return ProcessEnterKey();
                    }
            }
            return false;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="ctrl">Inherited code: Requires comment 1.</param>
        /// <param name="shift">Inherited code: Requires comment 2.</param>
        internal void ProcessUpKey(bool ctrl, bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        if (ctrl)
                        {
                            SelectedMonth = DisplayDateInternal;
                            DisplayMode = CalendarMode.Year;
                        }
                        else
                        {
                            DateTime? selectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), -ColumnsPerMonth);
                            ProcessSelection(shift, selectedDate, -ColumnsPerMonth);
                        }
                        break;
                    }
                case CalendarMode.Year:
                    {
                        if (ctrl)
                        {
                            SelectedYear = SelectedMonth;
                            DisplayMode = CalendarMode.Decade;
                        }
                        else
                        {
                            DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, -ColumnsPerYear);
                            OnSelectedMonthChanged(selectedMonth);
                        }
                        break;
                    }
                case CalendarMode.Decade:
                    {
                        if (!ctrl)
                        {
                            DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -ColumnsPerYear);
                            OnSelectedYearChanged(selectedYear);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="ctrl">Inherited code: Requires comment 1.</param>
        /// <param name="shift">Inherited code: Requires comment 2.</param>
        internal void ProcessDownKey(bool ctrl, bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        if (!ctrl || shift)
                        {
                            DateTime? selectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), ColumnsPerMonth);
                            ProcessSelection(shift, selectedDate, ColumnsPerMonth);
                        }
                        break;
                    }
                case CalendarMode.Year:
                    {
                        if (ctrl)
                        {
                            DisplayDate = SelectedMonth;
                            DisplayMode = CalendarMode.Month;
                        }
                        else
                        {
                            DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, ColumnsPerYear);
                            OnSelectedMonthChanged(selectedMonth);
                        }
                        break;
                    }
                case CalendarMode.Decade:
                    {
                        if (ctrl)
                        {
                            SelectedMonth = SelectedYear;
                            DisplayMode = CalendarMode.Year;
                        }
                        else
                        {
                            DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, ColumnsPerYear);
                            OnSelectedYearChanged(selectedYear);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="shift">Inherited code: Requires comment 1.</param>
        internal void ProcessLeftKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime? selectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), -1);
                        ProcessSelection(shift, selectedDate, -1);
                        break;
                    }
                case CalendarMode.Year:
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, -1);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }
                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -1);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="shift">Inherited code: Requires comment 1.</param>
        internal void ProcessRightKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime? selectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), 1);
                        ProcessSelection(shift, selectedDate, 1);
                        break;
                    }
                case CalendarMode.Year:
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, 1);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }
                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, 1);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private bool ProcessEnterKey()
        {
            switch (DisplayMode)
            {
                case CalendarMode.Year:
                    {
                        DisplayDate = SelectedMonth;
                        DisplayMode = CalendarMode.Month;
                        return true;
                    }
                case CalendarMode.Decade:
                    {
                        SelectedMonth = SelectedYear;
                        DisplayMode = CalendarMode.Year;
                        return true;
                    }
            }
            return false;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="shift">Inherited code: Requires comment 1.</param>
        internal void ProcessHomeKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        // REMOVE_RTM: Not all types of calendars start with Day1. If Non-Gregorian is supported check this:
                        DateTime? selectedDate = new DateTime(DisplayDateInternal.Year, DisplayDateInternal.Month, 1);
                        ProcessSelection(shift, selectedDate, null);
                        break;
                    }
                case CalendarMode.Year:
                    {
                        DateTime selectedMonth = new DateTime(_selectedMonth.Year, 1, 1);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }
                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = new DateTime(DateTimeHelper.DecadeOfDate(SelectedYear), 1, 1);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="shift">Inherited code: Requires comment 1.</param>
        internal void ProcessEndKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        if (DisplayDate != null)
                        {
                            DateTime? selectedDate = new DateTime(DisplayDateInternal.Year, DisplayDateInternal.Month, 1);

                            if (DateTimeHelper.CompareYearMonth(DateTime.MaxValue, selectedDate.Value) > 0)
                            {
                                // since DisplayDate is not equal to
                                // DateTime.MaxValue we are sure selectedDate is\
                                // not null
                                selectedDate = DateTimeHelper.AddMonths(selectedDate.Value, 1).Value;
                                selectedDate = DateTimeHelper.AddDays(selectedDate.Value, -1).Value;
                            }
                            else
                            {
                                selectedDate = DateTime.MaxValue;
                            }
                            ProcessSelection(shift, selectedDate, null);
                        }
                        break;
                    }
                case CalendarMode.Year:
                    {
                        DateTime selectedMonth = new DateTime(_selectedMonth.Year, 12, 1);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }
                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = new DateTime(DateTimeHelper.EndOfDecade(SelectedYear), 1, 1);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="shift">Inherited code: Requires comment 1.</param>
        internal void ProcessPageDownKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime? selectedDate = DateTimeHelper.AddMonths(LastSelectedDate.GetValueOrDefault(DateTime.Today), 1);
                        ProcessSelection(shift, selectedDate, null);
                        break;
                    }
                case CalendarMode.Year:
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddYears(_selectedMonth, 1);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }
                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, 10);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="shift">Inherited code: Requires comment 1.</param>
        internal void ProcessPageUpKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime? selectedDate = DateTimeHelper.AddMonths(LastSelectedDate.GetValueOrDefault(DateTime.Today), -1);
                        ProcessSelection(shift, selectedDate, null);
                        break;
                    }
                case CalendarMode.Year:
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddYears(_selectedMonth, -1);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }
                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -10);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Handled && e.Key == Key.Shift)
            {
                ProcessShiftKeyUp();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void ProcessShiftKeyUp()
        {
            if (_isShiftPressed && (SelectionMode == CalendarSelectionMode.SingleRange || SelectionMode == CalendarSelectionMode.MultipleRange))
            {
                AddSelection();
                _isShiftPressed = false;
            }
        }
        #endregion Key Events

        #region Focus Events
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_GotFocus(object sender, RoutedEventArgs e)
        {
            Calendar c = sender as Calendar;
            Debug.Assert(c != null, "c should not be null!");
            HasFocusInternal = true;

            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime focusDate;
                        if (LastSelectedDate.HasValue && DateTimeHelper.CompareYearMonth(DisplayDateInternal, LastSelectedDate.Value) == 0)
                        {
                            focusDate = LastSelectedDate.Value;
                        }
                        else
                        {
                            focusDate = DisplayDate;
                            LastSelectedDate = DisplayDate;
                        }
                        Debug.Assert(focusDate != null, "focusDate should not be null!");
                        FocusButton = FindDayButtonFromDay(focusDate);

                        if (FocusButton != null)
                        {
                            FocusButton.IsCurrent = true;
                        }
                        break;
                    }
                case CalendarMode.Year:
                case CalendarMode.Decade:
                    {
                        if (this.FocusCalendarButton != null)
                        {
                            FocusCalendarButton.IsCalendarButtonFocused = true;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment1 .</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_LostFocus(object sender, RoutedEventArgs e)
        {
            Calendar c = sender as Calendar;
            Debug.Assert(c != null, "c should not be null!");
            HasFocusInternal = false;

            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        if (FocusButton != null)
                        {
                            FocusButton.IsCurrent = false;
                        }
                        break;
                    }
                case CalendarMode.Year:
                case CalendarMode.Decade:
                    {
                        if (FocusCalendarButton != null)
                        {
                            FocusCalendarButton.IsCalendarButtonFocused = false;
                        }
                        break;
                    }
            }
        }
        #endregion Focus Events
    }
}
