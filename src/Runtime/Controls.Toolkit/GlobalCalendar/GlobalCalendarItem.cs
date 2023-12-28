// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Windows.Input;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents the currently displayed month or year on a
    /// <see cref="T:System.Windows.Controls.GlobalCalendar" />.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [TemplatePart(Name = GlobalCalendarItem.ElementRoot, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = GlobalCalendarItem.ElementHeaderButton, Type = typeof(Button))]
    [TemplatePart(Name = GlobalCalendarItem.ElementPreviousButton, Type = typeof(Button))]
    [TemplatePart(Name = GlobalCalendarItem.ElementNextButton, Type = typeof(Button))]
    [TemplatePart(Name = GlobalCalendarItem.ElementDayTitleTemplate, Type = typeof(DataTemplate))]
    [TemplatePart(Name = GlobalCalendarItem.ElementMonthView, Type = typeof(Grid))]
    [TemplatePart(Name = GlobalCalendarItem.ElementYearView, Type = typeof(Grid))]
    [TemplatePart(Name = GlobalCalendarItem.ElementDisabledVisual, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    public sealed partial class GlobalCalendarItem : Control
    {
        #region Template Parts
        /// <summary>
        /// The name of the Root template part.
        /// </summary>
        /// <remarks>
        /// TODO: It appears this template part is no longer used.  Verify with
        /// compat whether we can remove the attribute.
        /// </remarks>
        private const string ElementRoot = "Root";

        /// <summary>
        /// The name of the HeaderButton template part.
        /// </summary>
        private const string ElementHeaderButton = "HeaderButton";

        /// <summary>
        /// The name of the PreviousButton template part.
        /// </summary>
        private const string ElementPreviousButton = "PreviousButton";

        /// <summary>
        /// The name of the NextButton template part.
        /// </summary>
        private const string ElementNextButton = "NextButton";

        /// <summary>
        /// The name of the DayTitleTemplate template part.
        /// </summary>
        private const string ElementDayTitleTemplate = "DayTitleTemplate";

        /// <summary>
        /// The name of the MonthView template part.
        /// </summary>
        private const string ElementMonthView = "MonthView";

        /// <summary>
        /// The name of the YearView template part.
        /// </summary>
        private const string ElementYearView = "YearView";

        /// <summary>
        /// The name of the DisabledVisual template part.
        /// </summary>
        private const string ElementDisabledVisual = "DisabledVisual";

        /// <summary>
        /// The button that allows switching between month mode, year mode, and
        /// decade mode. 
        /// </summary>
        private Button _headerButton;

        /// <summary>
        /// Gets the button that allows switching between month mode, year mode,
        /// and decade mode. 
        /// </summary>
        internal Button HeaderButton
        {
            get { return _headerButton; }
            private set
            {
                if (_headerButton != null)
                {
                    _headerButton.Click -= HeaderButton_Click;
                }

                _headerButton = value;

                if (_headerButton != null)
                {
                    _headerButton.Click += HeaderButton_Click;
                    _headerButton.IsTabStop = false;
                }
            }
        }

        /// <summary>
        /// The button that displays the next page of the calendar when it is
        /// clicked.
        /// </summary>
        private Button _nextButton;

        /// <summary>
        /// Gets the button that displays the next page of the calendar when it
        /// is clicked.
        /// </summary>
        internal Button NextButton
        {
            get { return _nextButton; }
            private set
            {
                if (_nextButton != null)
                {
                    _nextButton.Click -= NextButton_Click;
                }

                _nextButton = value;

                if (_nextButton != null)
                {
                    // If the user does not provide a Content value in template,
                    // we provide a helper text that can be used in
                    // Accessibility this text is not shown on the UI, just used
                    // for Accessibility purposes
                    if (_nextButton.Content == null)
                    {
                        _nextButton.Content = Resource.Calendar_NextButtonName;
                    }

                    if (_isTopRightMostMonth)
                    {
                        _nextButton.Visibility = Visibility.Visible;
                        _nextButton.Click += NextButton_Click;
                        _nextButton.IsTabStop = false;
                    }
                }
            }
        }

        /// <summary>
        /// The button that displays the previous page of the calendar when it
        /// is clicked.
        /// </summary>
        private Button _previousButton;

        /// <summary>
        /// Gets the button that displays the previous page of the calendar when
        /// it is clicked.
        /// </summary>
        internal Button PreviousButton
        {
            get { return _previousButton; }
            private set
            {
                if (_previousButton != null)
                {
                    _previousButton.Click -= PreviousButton_Click;
                }

                _previousButton = value;

                if (_previousButton != null)
                {
                    // If the user does not provide a Content value in template,
                    // we provide a helper text that can be used in
                    // Accessibility this text is not shown on the UI, just used
                    // for Accessibility purposes
                    if (_previousButton.Content == null)
                    {
                        _previousButton.Content = Resource.Calendar_PreviousButtonName;
                    }

                    if (_isTopLeftMostMonth)
                    {
                        _previousButton.Visibility = Visibility.Visible;
                        _previousButton.Click += PreviousButton_Click;
                        _previousButton.IsTabStop = false;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private DataTemplate _dayTitleTemplate;

        /// <summary>
        /// Hosts the content when in month mode.
        /// </summary>
        private Grid _monthView;

        /// <summary>
        /// Gets the Grid that hosts the content when in month mode.
        /// </summary>
        internal Grid MonthView
        {
            get { return _monthView; }
            private set
            {
                if (_monthView != null)
                {
                    _monthView.MouseLeave -= MonthView_MouseLeave;
                }

                _monthView = value;

                if (_monthView != null)
                {
                    _monthView.MouseLeave += MonthView_MouseLeave;
                }
            }
        }

        /// <summary>
        /// Hosts the content when in year or decade mode.
        /// </summary>
        private Grid _yearView;

        /// <summary>
        /// Gets the Grid that hosts the content when in year or decade mode.
        /// </summary>
        internal Grid YearView
        {
            get { return _yearView; }
            private set
            {
                if (_yearView != null)
                {
                    _yearView.MouseLeave -= YearView_MouseLeave;
                }

                _yearView = value;

                if (_yearView != null)
                {
                    _yearView.MouseLeave += YearView_MouseLeave;
                }
            }
        }

        /// <summary>
        /// The overlay for the disabled state.
        /// </summary>
        /// <remarks>
        /// The disabled visual isn't necessary given that we also have a
        /// Disabled visual state.  It's only being left in for compatability
        /// with existing templates.
        /// </remarks>
        private FrameworkElement _disabledVisual;
        #endregion Template Parts

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal GlobalCalendar Owner { get; set; }

        /// <summary>
        /// Gets the CalendarInfo that provides globalized date operations.
        /// </summary>
        internal CalendarInfo Info
        {
            get
            {
                return Owner != null ?
                    Owner.Info :
                    GlobalCalendar.DefaultCalendarInfo;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private GlobalCalendarButton _lastCalendarButton;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private GlobalCalendarDayButton _lastCalendarDayButton;

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal GlobalCalendarDayButton CurrentButton { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private MouseButtonEventArgs _downEventArg;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private MouseButtonEventArgs _downEventArgYearView;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isMouseLeftButtonDown;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isMouseLeftButtonDownYearView;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isTopLeftMostMonth = true;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _isTopRightMostMonth = true;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private DateTime _currentMonth;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.Primitives.GlobalCalendarItem" />
        /// class.
        /// </summary>
        public GlobalCalendarItem()
        {
            DefaultStyleKey = typeof(GlobalCalendarItem);
        }

        #region Templating
        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.Primitives.GlobalCalendarItem" />
        /// when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            HeaderButton = GetTemplateChild(ElementHeaderButton) as Button;
            PreviousButton = GetTemplateChild(ElementPreviousButton) as Button;
            NextButton = GetTemplateChild(ElementNextButton) as Button;
            _dayTitleTemplate = GetTemplateChild(ElementDayTitleTemplate) as DataTemplate;
            MonthView = GetTemplateChild(ElementMonthView) as Grid;
            YearView = GetTemplateChild(ElementYearView) as Grid;
            _disabledVisual = GetTemplateChild(ElementDisabledVisual) as FrameworkElement;

            if (Owner != null)
            {
                UpdateDisabledGrid(Owner.IsEnabled);
            }

            PopulateGrids();

            if (MonthView != null && YearView != null)
            {
                if (Owner != null)
                {
                    Owner.SelectedMonth = Owner.DisplayDateInternal;
                    Owner.SelectedYear = Owner.DisplayDateInternal;

                    if (Owner.DisplayMode == CalendarMode.Year)
                    {
                        UpdateYearMode();
                    }
                    else if (Owner.DisplayMode == CalendarMode.Decade)
                    {
                        UpdateDecadeMode();
                    }

                    if (Owner.DisplayMode == CalendarMode.Month)
                    {
                        UpdateMonthMode();
                        MonthView.Visibility = Visibility.Visible;
                        YearView.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        YearView.Visibility = Visibility.Visible;
                        MonthView.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    UpdateMonthMode();
                    MonthView.Visibility = Visibility.Visible;
                    YearView.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void PopulateGrids()
        {
            if (MonthView != null)
            {
                for (int i = 0; i < GlobalCalendar.RowsPerMonth; i++)
                {
                    if (_dayTitleTemplate != null)
                    {
                        FrameworkElement cell = (FrameworkElement)_dayTitleTemplate.LoadContent();
                        cell.SetValue(Grid.RowProperty, 0);
                        cell.SetValue(Grid.ColumnProperty, i);
                        MonthView.Children.Add(cell);
                    }
                }

                for (int i = 1; i < GlobalCalendar.RowsPerMonth; i++)
                {
                    for (int j = 0; j < GlobalCalendar.ColumnsPerMonth; j++)
                    {
                        GlobalCalendarDayButton cell = new GlobalCalendarDayButton();

                        if (Owner != null)
                        {
                            cell.Owner = Owner;
                            Owner.ApplyDayButtonStyle(cell);
                        }
                        cell.SetValue(Grid.RowProperty, i);
                        cell.SetValue(Grid.ColumnProperty, j);
                        cell.CalendarDayButtonMouseDown += Cell_MouseLeftButtonDown;
                        cell.CalendarDayButtonMouseUp += Cell_MouseLeftButtonUp;
                        cell.MouseEnter += Cell_MouseEnter;
                        cell.MouseLeave += Cell_MouseLeave;
                        cell.Click += Cell_Click;
                        MonthView.Children.Add(cell);
                    }
                }
            }

            if (YearView != null)
            {
                GlobalCalendarButton month;
                int count = 0;
                for (int i = 0; i < GlobalCalendar.RowsPerYear; i++)
                {
                    for (int j = 0; j < GlobalCalendar.ColumnsPerYear; j++)
                    {
                        month = new GlobalCalendarButton();

                        if (Owner != null)
                        {
                            month.Owner = Owner;
                            if (Owner.CalendarButtonStyle != null)
                            {
                                month.Style = Owner.CalendarButtonStyle;
                            }
                        }
                        month.SetValue(Grid.RowProperty, i);
                        month.SetValue(Grid.ColumnProperty, j);
                        month.CalendarButtonMouseDown += Month_CalendarButtonMouseDown;
                        month.CalendarButtonMouseUp += Month_CalendarButtonMouseUp;
                        month.MouseEnter += Month_MouseEnter;
                        month.MouseLeave += Month_MouseLeave;
                        YearView.Children.Add(month);
                        count++;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="isEnabled">Inherited code: Requires comment 1.</param>
        internal void UpdateDisabledGrid(bool isEnabled)
        {
            if (isEnabled)
            {
                if (_disabledVisual != null)
                {
                    _disabledVisual.Visibility = Visibility.Collapsed;
                }
                VisualStates.GoToState(this, true, VisualStates.StateNormal);
            }
            else
            {
                if (_disabledVisual != null)
                {
                    _disabledVisual.Visibility = Visibility.Visible;
                }
                VisualStates.GoToState(this, true, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
        }
        #endregion Templating

        #region Month Mode
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void UpdateMonthMode()
        {
            if (Owner != null)
            {
                Debug.Assert(Owner.DisplayDate != null, "The Owner GlobalCalendar's DisplayDate should not be null!");
                _currentMonth = Owner.DisplayDateInternal;
            }
            else
            {
                _currentMonth = DateTime.Today;
            }

            if (_currentMonth != null)
            {
                SetMonthModeHeaderButton();
                SetMonthModePreviousButton(_currentMonth);
                SetMonthModeNextButton(_currentMonth);

                if (MonthView != null)
                {
                    SetDayTitles();
                    SetCalendarDayButtons(_currentMonth);
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetDayTitles()
        {
            CalendarInfo info = Info;
            for (int childIndex = 0; childIndex < GlobalCalendar.ColumnsPerMonth; childIndex++)
            {
                FrameworkElement daytitle = MonthView.Children[childIndex] as FrameworkElement;
                if (daytitle != null)
                {
                    DayOfWeek firstDay = (Owner != null) ?
                        Owner.FirstDay :
                        info.FirstDayOfWeek;

                    int index = (childIndex + (int)firstDay) % info.DaysInWeek;
                    daytitle.DataContext = info.GetShortestDayName(index);
                }
            }
        }

        /// <summary>
        /// How many days of the previous month need to be displayed.
        /// </summary>
        /// <param name="firstOfMonth">Inherited code: Requires comment.</param>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private int PreviousMonthDays(DateTime firstOfMonth)
        {
            CalendarInfo info = Info;
            DayOfWeek firstDay = (Owner != null) ?
                Owner.FirstDay :
                info.FirstDayOfWeek;
            int daysPerWeek = info.DaysInWeek;
            DayOfWeek day = info.GetDayOfWeek(firstOfMonth);

            int i = (day - firstDay + daysPerWeek) % daysPerWeek;
            return (i != 0) ?
                i :
                daysPerWeek;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="childButton">Inherited code: Requires comment 1.</param>
        /// <param name="dateToAdd">Inherited code: Requires comment 2.</param>
        private void SetButtonState(GlobalCalendarDayButton childButton, DateTime dateToAdd)
        {
            if (Owner == null)
            {
                return;
            }

            CalendarInfo info = Info;
            childButton.Opacity = 1;

            // If the day is outside the DisplayDateStart/End boundary, do
            // not show it
            if (info.CompareDays(dateToAdd, Owner.DisplayDateRangeStart) < 0 ||
                info.CompareDays(dateToAdd, Owner.DisplayDateRangeEnd) > 0)
            {
                childButton.IsEnabled = false;
                childButton.Opacity = 0;
            }
            else
            {
                // SET IF THE DAY IS SELECTABLE OR NOT
                childButton.IsBlackout = Owner.BlackoutDates.Contains(dateToAdd);
                childButton.IsEnabled = true;

                // SET IF THE DAY IS INACTIVE OR NOT: set if the day is a
                // trailing day or not
                childButton.IsInactive = info.GetMonthDifference(dateToAdd, Owner.DisplayDateInternal) != 0;

                // SET IF THE DAY IS TODAY OR NOT
                childButton.IsToday = Owner.IsTodayHighlighted && Info.Compare(dateToAdd, DateTime.Today) == 0;

                // SET IF THE DAY IS SELECTED OR NOT
                // (Since we should be comparing the Date values not DateTime
                // values, we can't use Owner.SelectedDates.Contains(dateToAdd)
                // directly)
                bool selected = false;
                foreach (DateTime item in Owner.SelectedDates)
                {
                    if (info.CompareDays(dateToAdd, item) == 0)
                    {
                        selected = true;
                        break;
                    }
                }
                childButton.IsSelected = selected;

                // SET THE FOCUS ELEMENT
                if (Owner.LastSelectedDate != null)
                {
                    if (info.CompareDays(Owner.LastSelectedDate.Value, dateToAdd) == 0)
                    {
                        if (Owner.FocusButton != null)
                        {
                            Owner.FocusButton.IsCurrent = false;
                        }
                        Owner.FocusButton = childButton;
                        if (Owner.HasFocusInternal)
                        {
                            Owner.FocusButton.IsCurrent = true;
                        }
                    }
                    else
                    {
                        childButton.IsCurrent = false;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="firstDayOfMonth">Inherited code: Requires comment 1.</param>
        private void SetCalendarDayButtons(DateTime firstDayOfMonth)
        {
            Debug.Assert(firstDayOfMonth.Day == 1, "firstDayOfMonth should be the first day of the month!");

            CalendarInfo info = Info;

            DateTime dateToAdd;
            int lastMonthToDisplay = PreviousMonthDays(firstDayOfMonth);
            if (info.GetMonthDifference(firstDayOfMonth, DateTime.MinValue) > 0)
            {
                // DisplayDate is not equal to DateTime.MinValue we can subtract
                // days from the DisplayDate
                dateToAdd = info.AddDays(firstDayOfMonth, -lastMonthToDisplay).Value;
            }
            else
            {
                dateToAdd = firstDayOfMonth;
            }

            if (Owner != null && Owner.HoverEnd != null && Owner.HoverStart != null)
            {
                Owner.HoverEndIndex = null;
                Owner.HoverStartIndex = null;
            }

            int count = GlobalCalendar.RowsPerMonth * GlobalCalendar.ColumnsPerMonth;
            for (int childIndex = GlobalCalendar.ColumnsPerMonth; childIndex < count; childIndex++)
            {
                GlobalCalendarDayButton childButton = MonthView.Children[childIndex] as GlobalCalendarDayButton;
                Debug.Assert(childButton != null, "childButton should not be null!");

                childButton.Index = childIndex;
                SetButtonState(childButton, dateToAdd);

                // Update the indexes of hoverStart and hoverEnd
                if (Owner != null && Owner.HoverEnd != null && Owner.HoverStart != null)
                {
                    if (info.CompareDays(dateToAdd, Owner.HoverEnd.Value) == 0)
                    {
                        Owner.HoverEndIndex = childIndex;
                    }

                    if (info.CompareDays(dateToAdd, Owner.HoverStart.Value) == 0)
                    {
                        Owner.HoverStartIndex = childIndex;
                    }
                }

                childButton.IsTabStop = false;
                childButton.Content = info.DayToString(dateToAdd);
                GlobalCalendarExtensions.SetDate(childButton, dateToAdd);
                Owner.ApplyDayButtonStyle(childButton);

                if (info.Compare(DateTime.MaxValue.Date, dateToAdd) > 0)
                {
                    // Since we are sure DisplayDate is not equal to
                    // DateTime.MaxValue, it is safe to use AddDays 
                    dateToAdd = info.AddDays(dateToAdd, 1).Value;
                }
                else
                {
                    // DisplayDate is equal to the DateTime.MaxValue, so there
                    // are no trailing days
                    childIndex++;
                    for (int i = childIndex; i < count; i++)
                    {
                        childButton = MonthView.Children[i] as GlobalCalendarDayButton;
                        Debug.Assert(childButton != null, "childButton should not be null!");
                        // button needs a content to occupy the necessary space
                        // for the content presenter
                        DateTime? childDay = info.AddDays(firstDayOfMonth, i - 1);
                        childButton.Content = childDay != null ? info.DayToString(childDay.Value) : null;
                        childButton.IsEnabled = false;
                        childButton.Opacity = 0;
                    }
                    return;
                }
            }

            // If the HoverStart or HoverEndInternal could not be found on the
            // DisplayMonth set the values of the HoverStartIndex or
            // HoverEndIndex to be the first or last day indexes on the current
            // month
            if (Owner != null && Owner.HoverStart != null && Owner.HoverEndInternal != null)
            {
                if (Owner.HoverEndIndex == null)
                {
                    if (info.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0)
                    {
                        Owner.HoverEndIndex = GlobalCalendar.ColumnsPerMonth * GlobalCalendar.RowsPerMonth - 1;
                    }
                    else
                    {
                        Owner.HoverEndIndex = GlobalCalendar.ColumnsPerMonth;
                    }
                }

                if (Owner.HoverStartIndex == null)
                {
                    if (info.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0)
                    {
                        Owner.HoverStartIndex = GlobalCalendar.ColumnsPerMonth;
                    }
                    else
                    {
                        Owner.HoverStartIndex = GlobalCalendar.ColumnsPerMonth * GlobalCalendar.RowsPerMonth - 1;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetMonthButtonsForYearMode()
        {
            CalendarInfo info = Info;

            int count = 0;
            foreach (object child in YearView.Children)
            {
                GlobalCalendarButton childButton = child as GlobalCalendarButton;
                Debug.Assert(childButton != null, "childButton should not be null!");

                // There should be no time component. Time is 12:00 AM
                DateTime day = info.AddMonths(info.GetFirstDayInYear(_currentMonth), count).Value;
                childButton.DataContext = day;

                childButton.Content = info.GetAbbreviatedMonthName(count);
                childButton.Visibility = Visibility.Visible;

                if (Owner != null)
                {
                    if (day.Year == _currentMonth.Year &&
                        day.Month == _currentMonth.Month &&
                        day.Day == _currentMonth.Day)
                    {
                        Owner.FocusCalendarButton = childButton;
                        childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
                    }
                    else
                    {
                        childButton.IsCalendarButtonFocused = false;
                    }

                    Debug.Assert(Owner.DisplayDateInternal != null, "The Owner GlobalCalendar's DisplayDateInternal should not be null!");
                    childButton.IsSelected = info.GetMonthDifference(day, Owner.DisplayDateInternal) == 0;

                    if (info.GetMonthDifference(day, Owner.DisplayDateRangeStart) < 0 ||
                        info.GetMonthDifference(day, Owner.DisplayDateRangeEnd) > 0)
                    {
                        childButton.IsEnabled = false;
                        childButton.Opacity = 0;
                    }
                    else
                    {
                        childButton.IsEnabled = true;
                        childButton.Opacity = 1;
                    }
                }

                childButton.IsInactive = false;
                count++;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetMonthModeHeaderButton()
        {
            if (HeaderButton != null)
            {
                DateTime date = DateTime.Today;
                if (Owner != null)
                {
                    date = Owner.DisplayDateInternal;
                    HeaderButton.IsEnabled = true;
                }
                HeaderButton.Content = Info.MonthAndYearToString(date);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="firstDayOfMonth">Inherited code: Requires comment 1.</param>
        private void SetMonthModeNextButton(DateTime firstDayOfMonth)
        {
            Debug.Assert(firstDayOfMonth.Day == 1, "firstDayOfMonth should be the first day of the month!");
            if (Owner != null && NextButton != null)
            {
                NextButton.IsEnabled = false;

                // DisplayDate is equal to DateTime.MaxValue
                CalendarInfo info = Info;
                if (info.GetMonthDifference(firstDayOfMonth, DateTime.MaxValue) != 0)
                {
                    // Since we are sure DisplayDate is not equal to
                    // DateTime.MaxValue, it is safe to use AddMonths  
                    DateTime? firstDayOfNextMonth = info.AddMonths(firstDayOfMonth, 1);
                    if (firstDayOfNextMonth != null)
                    {
                        NextButton.IsEnabled =
                            info.CompareDays(Owner.DisplayDateRangeEnd, firstDayOfNextMonth.Value) > -1;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="firstDayOfMonth">Inherited code: Requires comment 1.</param>
        private void SetMonthModePreviousButton(DateTime firstDayOfMonth)
        {
            Debug.Assert(firstDayOfMonth.Day == 1, "firstDayOfMonth should be the first day of the month!");
            if (Owner != null && PreviousButton != null)
            {
                PreviousButton.IsEnabled =
                    Info.CompareDays(Owner.DisplayDateRangeStart, firstDayOfMonth) < 0;
            }
        }
        #endregion Month Mode

        #region Year Mode
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void UpdateYearMode()
        {
            if (Owner != null)
            {
                Debug.Assert(Owner.SelectedMonth != null, "The Owner GlobalCalendar's SelectedMonth should not be null!");
                _currentMonth = (DateTime)Owner.SelectedMonth;
            }
            else
            {
                _currentMonth = DateTime.Today;
            }

            if (_currentMonth != null)
            {
                SetYearModeHeaderButton();
                SetYearModePreviousButton();
                SetYearModeNextButton();

                if (YearView != null)
                {
                    SetMonthButtonsForYearMode();
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="decade">Inherited code: Requires comment 1.</param>
        /// <param name="decadeEnd">Inherited code: Requires comment 2.</param>
        private void SetYearButtons(int decade, int decadeEnd)
        {
            int year;
            int count = -1;
            foreach (object child in YearView.Children)
            {
                GlobalCalendarButton childButton = child as GlobalCalendarButton;
                Debug.Assert(childButton != null, "childButton should not be null!");
                year = decade + count;

                if (year <= DateTime.MaxValue.Year && year >= DateTime.MinValue.Year)
                {
                    // There should be no time component. Time is 12:00 AM
                    DateTime day = new DateTime(year, 1, 1);
                    childButton.DataContext = day;
                    childButton.Content = Info.YearToString(day);
                    childButton.Visibility = Visibility.Visible;

                    if (Owner != null)
                    {
                        if (year == Owner.SelectedYear.Year)
                        {
                            Owner.FocusCalendarButton = childButton;
                            childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
                        }
                        else
                        {
                            childButton.IsCalendarButtonFocused = false;
                        }
                        childButton.IsSelected = (Owner.DisplayDate.Year == year);

                        if (year < Owner.DisplayDateRangeStart.Year || year > Owner.DisplayDateRangeEnd.Year)
                        {
                            childButton.IsEnabled = false;
                            childButton.Opacity = 0;
                        }
                        else
                        {
                            childButton.IsEnabled = true;
                            childButton.Opacity = 1;
                        }
                    }

                    // SET IF THE YEAR IS INACTIVE OR NOT: set if the year is a
                    // trailing year or not
                    childButton.IsInactive = (year < decade || year > decadeEnd);
                }
                else
                {
                    childButton.IsEnabled = false;
                    childButton.Opacity = 0;
                }

                count++;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetYearModeHeaderButton()
        {
            if (HeaderButton != null)
            {
                HeaderButton.IsEnabled = true;
                HeaderButton.Content = Info.YearToString(_currentMonth);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetYearModePreviousButton()
        {
            if (Owner != null && PreviousButton != null)
            {
                PreviousButton.IsEnabled = Owner.DisplayDateRangeStart.Year != _currentMonth.Year;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetYearModeNextButton()
        {
            if (Owner != null && NextButton != null)
            {
                NextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year != _currentMonth.Year;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="calendarButton">Inherited code: Requires comment 1.</param>
        internal void UpdateYearViewSelection(GlobalCalendarButton calendarButton)
        {
            if (Owner != null && calendarButton != null && calendarButton.DataContext is DateTime)
            {
                Owner.FocusCalendarButton.IsCalendarButtonFocused = false;
                Owner.FocusCalendarButton = calendarButton;
                calendarButton.IsCalendarButtonFocused = Owner.HasFocusInternal;

                DateTime date = (DateTime)calendarButton.DataContext;
                if (Owner.DisplayMode == CalendarMode.Year)
                {
                    Owner.SelectedMonth = date;
                }
                else
                {
                    Owner.SelectedYear = date;
                }
            }
        }
        #endregion Year Mode

        #region Decade Mode
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal void UpdateDecadeMode()
        {
            DateTime selectedYear;

            if (Owner != null)
            {
                Debug.Assert(Owner.SelectedYear != null, "The owning GlobalCalendar's selected year should not be null!");
                selectedYear = Owner.SelectedYear;
                _currentMonth = (DateTime)Owner.SelectedMonth;
            }
            else
            {
                _currentMonth = DateTime.Today;
                selectedYear = DateTime.Today;
            }

            CalendarInfo info = Info;
            if (_currentMonth != null)
            {
                int decade = info.GetDecadeStart(selectedYear);
                int decadeEnd = info.GetDecadeEnd(selectedYear);

                SetDecadeModeHeaderButton(decade, decadeEnd);
                SetDecadeModePreviousButton(decade);
                SetDecadeModeNextButton(decadeEnd);

                if (YearView != null)
                {
                    SetYearButtons(decade, decadeEnd);
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="decade">Inherited code: Requires comment 1.</param>
        /// <param name="decadeEnd">Inherited code: Requires comment 2.</param>
        private void SetDecadeModeHeaderButton(int decade, int decadeEnd)
        {
            if (HeaderButton != null)
            {
                HeaderButton.Content = Info.DecadeToString(decade, decadeEnd);
                HeaderButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="decadeEnd">Inherited code: Requires comment 1.</param>
        private void SetDecadeModeNextButton(int decadeEnd)
        {
            if (Owner != null && NextButton != null)
            {
                NextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year > decadeEnd;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="decade">Inherited code: Requires comment 1.</param>
        private void SetDecadeModePreviousButton(int decade)
        {
            if (Owner != null && PreviousButton != null)
            {
                PreviousButton.IsEnabled = decade > Owner.DisplayDateRangeStart.Year;
            }
        }
        #endregion Decade Mode

        #region Cell Mouse Events
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Owner != null)
            {
                GlobalCalendarDayButton b = sender as GlobalCalendarDayButton;
                if (_isMouseLeftButtonDown && b != null && b.IsEnabled && !b.IsBlackout)
                {
                    // Update the states of all buttons to be selected starting
                    // from HoverStart to b
                    switch (Owner.SelectionMode)
                    {
                        case CalendarSelectionMode.SingleDate:
                            {
                                DateTime selectedDate = GlobalCalendarExtensions.GetDate(b);
                                Owner.DatePickerDisplayDateFlag = true;
                                if (Owner.SelectedDates.Count == 0)
                                {
                                    Owner.SelectedDates.Add(selectedDate);
                                }
                                else
                                {
                                    Owner.SelectedDates[0] = selectedDate;
                                }
                                return;
                            }
                        case CalendarSelectionMode.SingleRange:
                        case CalendarSelectionMode.MultipleRange:
                            {
                                Debug.Assert(b.DataContext != null, "The DataContext should not be null!");
                                Owner.UnHighlightDays();
                                Owner.HoverEndIndex = b.Index;
                                Owner.HoverEnd = GlobalCalendarExtensions.GetDate(b);
                                // Update the States of the buttons
                                Owner.HighlightDays();
                                return;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDown)
            {
                GlobalCalendarDayButton b = sender as GlobalCalendarDayButton;
                // The button is in Pressed state. Change the state to normal.
                b.ReleaseMouseCapture();
                // null check is added for unit tests
                if (_downEventArg != null)
                {
                    b.SendMouseLeftButtonUp(_downEventArg);
                }
                _lastCalendarDayButton = b;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Owner != null)
            {
                if (!Owner.HasFocusInternal)
                {
                    Owner.Focus();
                }

                bool ctrl, shift;
                CalendarExtensions.GetMetaKeyState(out ctrl, out shift);
                GlobalCalendarDayButton b = sender as GlobalCalendarDayButton;

                if (b != null)
                {
                    if (b.IsEnabled && !b.IsBlackout)
                    {
                        DateTime selectedDate = GlobalCalendarExtensions.GetDate(b);
                        Debug.Assert(selectedDate != null, "selectedDate should not be null!");
                        _isMouseLeftButtonDown = true;
                        // null check is added for unit tests
                        if (e != null)
                        {
                            _downEventArg = e;
                        }

                        switch (Owner.SelectionMode)
                        {
                            case CalendarSelectionMode.None:
                                {
                                    return;
                                }
                            case CalendarSelectionMode.SingleDate:
                                {
                                    Owner.DatePickerDisplayDateFlag = true;
                                    if (Owner.SelectedDates.Count == 0)
                                    {
                                        Owner.SelectedDates.Add(selectedDate);
                                    }
                                    else
                                    {
                                        Owner.SelectedDates[0] = selectedDate;
                                    }
                                    return;
                                }
                            case CalendarSelectionMode.SingleRange:
                                {
                                    // Set the start or end of the selection
                                    // range
                                    if (shift)
                                    {
                                        Owner.UnHighlightDays();
                                        Owner.HoverEnd = selectedDate;
                                        Owner.HoverEndIndex = b.Index;
                                        Owner.HighlightDays();
                                    }
                                    else
                                    {
                                        Owner.UnHighlightDays();
                                        Owner.HoverStart = selectedDate;
                                        Owner.HoverStartIndex = b.Index;
                                    }
                                    return;
                                }
                            case CalendarSelectionMode.MultipleRange:
                                {
                                    if (shift)
                                    {
                                        if (!ctrl)
                                        {
                                            // clear the list, set the states to
                                            // default
                                            foreach (DateTime item in Owner.SelectedDates)
                                            {
                                                Owner.RemovedItems.Add(item);
                                            }
                                            Owner.SelectedDates.ClearInternal();
                                        }
                                        Owner.HoverEnd = selectedDate;
                                        Owner.HoverEndIndex = b.Index;
                                        Owner.HighlightDays();
                                    }
                                    else
                                    {
                                        if (!ctrl)
                                        {
                                            // clear the list, set the states to
                                            // default
                                            foreach (DateTime item in Owner.SelectedDates)
                                            {
                                                Owner.RemovedItems.Add(item);
                                            }
                                            Owner.SelectedDates.ClearInternal();
                                            Owner.UnHighlightDays();
                                        }
                                        Owner.HoverStart = selectedDate;
                                        Owner.HoverStartIndex = b.Index;
                                    }
                                    return;
                                }
                        }
                    }
                    else
                    {
                        // If a click occurs on a BlackOutDay we set the
                        // HoverStart to be null
                        Owner.HoverStart = null;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="b">Inherited code: Requires comment 1.</param>
        private void AddSelection(GlobalCalendarDayButton b)
        {
            if (Owner != null)
            {
                DateTime date = GlobalCalendarExtensions.GetDate(b);
                Owner.HoverEndIndex = b.Index;
                Owner.HoverEnd = date;

                if (Owner.HoverEnd != null && Owner.HoverStart != null)
                {
                    // this is selection with Mouse, we do not guarantee the
                    // range does not include BlackOutDates.  AddRange method
                    // will throw away the BlackOutDates based on the
                    // SelectionMode
                    Owner.IsMouseSelection = true;
                    Owner.SelectedDates.AddRange(Owner.HoverStart.Value, Owner.HoverEnd.Value);
                    Owner.OnDayClick(date);
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Cell_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Owner != null)
            {
                bool ctrl, shift;
                CalendarExtensions.GetMetaKeyState(out ctrl, out shift);
                GlobalCalendarDayButton b = sender as GlobalCalendarDayButton;
                if (b != null && !b.IsBlackout)
                {
                    Owner.OnDayButtonMouseUp(e);
                }
                _isMouseLeftButtonDown = false;
                if (b != null && b.DataContext is DateTime)
                {
                    if (Owner.SelectionMode == CalendarSelectionMode.None || Owner.SelectionMode == CalendarSelectionMode.SingleDate)
                    {
                        Owner.OnDayClick(GlobalCalendarExtensions.GetDate(b));
                        return;
                    }
                    if (Owner.HoverStart.HasValue)
                    {
                        switch (Owner.SelectionMode)
                        {
                            case CalendarSelectionMode.SingleRange:
                                {
                                    // Update SelectedDates
                                    foreach (DateTime item in Owner.SelectedDates)
                                    {
                                        Owner.RemovedItems.Add(item);
                                    }
                                    Owner.SelectedDates.ClearInternal();
                                    AddSelection(b);
                                    return;
                                }
                            case CalendarSelectionMode.MultipleRange:
                                {
                                    // add the selection (either single day or
                                    // SingleRange day)
                                    AddSelection(b);
                                    return;
                                }
                        }
                    }
                    else
                    {
                        // If the day is Disabled but a trailing day we should
                        // be able to switch months
                        if (b.IsInactive && b.IsBlackout)
                        {
                            Owner.OnDayClick(GlobalCalendarExtensions.GetDate(b));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                bool ctrl, shift;
                CalendarExtensions.GetMetaKeyState(out ctrl, out shift);

                if (ctrl && Owner.SelectionMode == CalendarSelectionMode.MultipleRange)
                {
                    GlobalCalendarDayButton b = sender as GlobalCalendarDayButton;
                    Debug.Assert(b != null, "The sender should be a non-null GlobalCalendarDayButton!");

                    if (b.IsSelected)
                    {
                        Owner.HoverStart = null;
                        _isMouseLeftButtonDown = false;
                        b.IsSelected = false;
                        DateTime? date = GlobalCalendarExtensions.GetDateNullable(b);
                        if (date != null)
                        {
                            Owner.SelectedDates.Remove(date.Value);
                        }
                    }
                }
            }
        }
        #endregion Cell Mouse Events

        #region Mouse Events
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                if (!Owner.HasFocusInternal)
                {
                    Owner.Focus();
                }
                Button b = sender as Button;
                DateTime d;

                if (b.IsEnabled)
                {
                    if (Owner.DisplayMode == CalendarMode.Month)
                    {
                        if (Owner.DisplayDate != null)
                        {
                            d = Owner.DisplayDateInternal;
                            Owner.SelectedMonth = Info.GetFirstDayInMonth(d);
                        }
                        Owner.DisplayMode = CalendarMode.Year;
                    }
                    else
                    {
                        Debug.Assert(Owner.DisplayMode == CalendarMode.Year, "The Owner GlobalCalendar's DisplayMode should be Year!");

                        if (Owner.SelectedMonth != null)
                        {
                            d = Owner.SelectedMonth;
                            Owner.SelectedYear = Info.GetFirstDayInMonth(d);
                        }
                        Owner.DisplayMode = CalendarMode.Decade;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                if (!Owner.HasFocusInternal)
                {
                    Owner.Focus();
                }

                Button b = sender as Button;
                if (b.IsEnabled)
                {
                    Owner.OnPreviousClick();
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                if (!Owner.HasFocusInternal)
                {
                    Owner.Focus();
                }
                Button b = sender as Button;

                if (b.IsEnabled)
                {
                    Owner.OnNextClick();
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Month_CalendarButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            GlobalCalendarButton b = sender as GlobalCalendarButton;
            Debug.Assert(b != null, "The sender should be a non-null GlobalCalendarDayButton!");

            _isMouseLeftButtonDownYearView = true;

            if (e != null)
            {
                _downEventArgYearView = e;
            }

            UpdateYearViewSelection(b);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        internal void Month_CalendarButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseLeftButtonDownYearView = false;

            if (Owner != null)
            {
                DateTime newmonth = (DateTime)((GlobalCalendarButton)sender).DataContext;

                if (Owner.DisplayMode == CalendarMode.Year)
                {
                    Owner.DisplayDate = newmonth;
                    Owner.DisplayMode = CalendarMode.Month;
                }
                else
                {
                    Debug.Assert(Owner.DisplayMode == CalendarMode.Decade, "The owning GlobalCalendar should be in decade mode!");
                    Owner.SelectedMonth = newmonth;
                    Owner.DisplayMode = CalendarMode.Year;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Month_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDownYearView)
            {
                GlobalCalendarButton b = sender as GlobalCalendarButton;
                Debug.Assert(b != null, "The sender should be a non-null GlobalCalendarDayButton!");
                UpdateYearViewSelection(b);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Month_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDownYearView)
            {
                GlobalCalendarButton b = sender as GlobalCalendarButton;
                // The button is in Pressed state. Change the state to normal.
                b.ReleaseMouseCapture();
                if (_downEventArgYearView != null)
                {
                    b.SendMouseLeftButtonUp(_downEventArgYearView);
                }
                _lastCalendarButton = b;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void MonthView_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_lastCalendarDayButton != null)
            {
                _lastCalendarDayButton.CaptureMouse();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void YearView_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_lastCalendarButton != null)
            {
                _lastCalendarButton.CaptureMouse();
            }
        }
        #endregion Mouse Events
    }
}