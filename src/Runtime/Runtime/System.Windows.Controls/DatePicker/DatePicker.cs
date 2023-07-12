// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
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
using ModifierKeys = Windows.System.VirtualKeyModifiers;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that allows the user to select a date.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = DatePicker.ElementRoot, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DatePicker.ElementTextBox, Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = DatePicker.ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = DatePicker.ElementPopup, Type = typeof(Popup))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]
    [StyleTypedProperty(Property = nameof(CalendarStyle), StyleTargetType = typeof(Calendar))]
    public partial class DatePicker : Control
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private const string ElementRoot = "Root";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private const string ElementTextBox = "TextBox";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private const string ElementButton = "Button";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private const string ElementPopup = "Popup";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private Calendar _calendar;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private string _defaultText;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private Button _dropDownButton;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private Canvas _outsideCanvas;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private Canvas _outsidePopupCanvas;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private Popup _popUp;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private FrameworkElement _root;

        /// <summary>
        /// The value of the SelectedDate just before the Calendar Popup is opened.
        /// This is used for cancelling the date selection with ESC key.
        /// </summary>
        private DateTime? _onOpenSelectedDate;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _settingSelectedDate;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private DatePickerTextBox _textBox;

        /// <summary>
        /// Occurs when the drop-down <see cref="Calendar" /> is closed.
        /// </summary>
        public event RoutedEventHandler CalendarClosed;

        /// <summary>
        /// Occurs when the drop-down <see cref="Calendar" /> is opened.
        /// </summary>
        public event RoutedEventHandler CalendarOpened;

        /// <summary>
        /// Occurs when <see cref="Text" /> is assigned a value that cannot 
        /// be interpreted as a date.
        /// </summary>
        public event EventHandler<DatePickerDateValidationErrorEventArgs> DateValidationError;

        /// <summary>
        /// Occurs when the <see cref="SelectedDate" /> property is changed.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDateChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatePicker" /> class.
        /// </summary>
        public DatePicker()
        {
            InitializeCalendar();
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            this.FirstDayOfWeek = DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek;
            this._defaultText = string.Empty;
            this.DisplayDate = DateTime.Today;
            this.GotFocus += new RoutedEventHandler(DatePicker_GotFocus);
            this.LostFocus += new RoutedEventHandler(DatePicker_LostFocus);
            this.BlackoutDates = this._calendar.BlackoutDates;
            DefaultStyleKey = typeof(DatePicker);
        }

        /// <summary>
        /// Gets a collection of dates that are marked as not selectable.
        /// </summary>
        /// <value>
        /// A collection of dates that cannot be selected. The default value is
        /// an empty collection.
        /// </value>
        public CalendarBlackoutDatesCollection BlackoutDates { get; private set; }

        #region CalendarStyle
        /// <summary>
        /// Gets or sets the style that is used when rendering the calendar.
        /// </summary>
        /// <value>
        /// The style that is used when rendering the calendar.
        /// </value>
        public Style CalendarStyle
        {
            get { return (Style) GetValue(CalendarStyleProperty); }
            set { SetValue(CalendarStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CalendarStyle" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="CalendarStyle" /> dependency property.
        /// </value>
        public static readonly DependencyProperty CalendarStyleProperty =
            DependencyProperty.Register(
            nameof(CalendarStyle),
            typeof(Style),
            typeof(DatePicker),
            new PropertyMetadata(OnCalendarStyleChanged));

        /// <summary>
        /// CalendarStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its CalendarStyle.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnCalendarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Style newStyle = e.NewValue as Style;
            if (newStyle != null)
            {
                DatePicker dp = d as DatePicker;

                if (dp != null)
                {
                    Style oldStyle = e.OldValue as Style;

                    // Set the style for the calendar if it has not already been
                    // set

                    if (dp._calendar != null)
                    {
                        // REMOVE_RTM: Remove null check when Silverlight allows us to re-apply styles
                        // Apply the newCalendarDayStyle if the DayButton was
                        // using the oldCalendarDayStyle before
                        if (dp._calendar.Style == null || dp._calendar.Style == oldStyle)
                        {
                            dp._calendar.Style = newStyle;
                        }
                    }
                }
            }
        }
        #endregion CalendarStyle

        #region DisplayDate
        /// <summary>
        /// Gets or sets the date to display.
        /// </summary>
        /// <value>
        /// The date to display. The default <see cref="DateTime.Today" />.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified date is not in the range defined by <see cref="DisplayDateStart" />
        /// and <see cref="DisplayDateEnd" />.
        /// </exception>
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
            typeof(DatePicker),
            new PropertyMetadata(OnDisplayDateChanged));

        /// <summary>
        /// DisplayDateProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its DisplayDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");

            if (DateTimeHelper.CompareYearMonth(dp._calendar.DisplayDate, (DateTime) e.NewValue) != 0)
            {
                dp._calendar.DisplayDate = dp.DisplayDate;

                if (DateTime.Compare(dp._calendar.DisplayDate, dp.DisplayDate) != 0)
                {
                    dp.DisplayDate = dp._calendar.DisplayDate;
                }
            }
        }
        #endregion DisplayDate

        #region DisplayDateEnd
        /// <summary>
        /// Gets or sets the last date to be displayed.
        /// </summary>
        /// <value>The last date to display.</value>
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
            typeof(DatePicker),
            new PropertyMetadata(OnDisplayDateEndChanged));

        /// <summary>
        /// DisplayDateEndProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its DisplayDateEnd.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");

            dp._calendar.DisplayDateEnd = dp.DisplayDateEnd;

            if (dp._calendar.DisplayDateEnd.HasValue && dp.DisplayDateEnd.HasValue && DateTime.Compare(dp._calendar.DisplayDateEnd.Value, dp.DisplayDateEnd.Value) != 0)
            {
                dp.DisplayDateEnd = dp._calendar.DisplayDateEnd;
            }
        }
        #endregion DisplayDateEnd

        #region DisplayDateStart
        /// <summary>
        /// Gets or sets the first date to be displayed.
        /// </summary>
        /// <value>The first date to display.</value>
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
            typeof(DatePicker),
            new PropertyMetadata(OnDisplayDateStartChanged));

        /// <summary>
        /// DisplayDateStartProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// DatePicker that changed its DisplayDateStart.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.
        /// </param>
        private static void OnDisplayDateStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");

            dp._calendar.DisplayDateStart = dp.DisplayDateStart;

            if (dp._calendar.DisplayDateStart.HasValue && dp.DisplayDateStart.HasValue && DateTime.Compare(dp._calendar.DisplayDateStart.Value, dp.DisplayDateStart.Value) != 0)
            {
                dp.DisplayDateStart = dp._calendar.DisplayDateStart;
            }
        }
        #endregion DisplayDateStart

        #region FirstDayOfWeek
        /// <summary>
        /// Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        /// <value>
        /// A <see cref="DayOfWeek" /> representing the beginning of the week.
        /// The default is <see cref="DayOfWeek.Sunday" />.
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
            typeof(DatePicker),
            new PropertyMetadata(OnFirstDayOfWeekChanged));

        /// <summary>
        /// FirstDayOfWeekProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its FirstDayOfWeek.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");

            dp._calendar.FirstDayOfWeek = dp.FirstDayOfWeek;
        }
        #endregion FirstDayOfWeek

        #region IsDropDownOpen
        /// <summary>
        /// Gets or sets a value indicating whether the drop-down <see cref="Calendar" /> 
        /// is open or closed.
        /// </summary>
        /// <value>
        /// True if the <see cref="Calendar" /> is open; otherwise, false. The default is false.
        /// </value>
        public bool IsDropDownOpen
        {
            get { return (bool) GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsDropDownOpen" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="IsDropDownOpen" /> dependency property.
        /// </value>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
            nameof(IsDropDownOpen),
            typeof(bool),
            typeof(DatePicker),
            new PropertyMetadata(false, OnIsDropDownOpenChanged));

        /// <summary>
        /// IsDropDownOpenProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its IsDropDownOpen.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");
            bool newValue = (bool) e.NewValue;
            bool oldValue = (bool) e.OldValue;

            if (dp._popUp != null && dp._popUp.Child != null)
            {
                if (newValue != oldValue)
                {
                    if (dp._calendar.DisplayMode != CalendarMode.Month)
                    {
                        dp._calendar.DisplayMode = CalendarMode.Month;
                    }

                    if (newValue)
                    {
                        dp.OpenDropDown();
                    }
                    else
                    {
                        Debug.Assert(!newValue, "The drop down should be closed!");
                        dp._popUp.IsOpen = false;
                        dp.OnCalendarClosed(new RoutedEventArgs());
                    }
                }
            }
        }
        #endregion IsDropDownOpen

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateDisabledVisual();
        }

        #region IsTodayHighlighted
        /// <summary>
        /// Gets or sets a value indicating whether the current date will be
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
            typeof(DatePicker),
            new PropertyMetadata(true, OnIsTodayHighlightedChanged));

        /// <summary>
        /// IsTodayHighlightedProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// DatePicker that changed its IsTodayHighlighted.
        /// </param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsTodayHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");

            dp._calendar.IsTodayHighlighted = dp.IsTodayHighlighted;
        }
        #endregion IsTodayHighlighted

        #region SelectedDate
        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        /// <value>
        /// The date currently selected. The default is null.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified date is not in the range defined by <see cref="DisplayDateStart" /> 
        /// and <see cref="DisplayDateEnd" />, or the specified date is in the <see cref="BlackoutDates" />
        /// collection.
        /// </exception>
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
            typeof(DatePicker),
            new PropertyMetadata(OnSelectedDateChanged));

        /// <summary>
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its SelectedDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTime? addedDate;
            DateTime? removedDate;
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");

            addedDate = (DateTime?) e.NewValue;
            removedDate = (DateTime?) e.OldValue;

            if (addedDate != dp._calendar.SelectedDate)
            {
                dp._calendar.SelectedDate = addedDate;
            }

            if (dp.SelectedDate != null)
            {
                DateTime day = (DateTime) dp.SelectedDate;

                // When the SelectedDateProperty change is done from
                // OnTextPropertyChanged method, two-way binding breaks if
                // BeginInvoke is not used:
                dp.Dispatcher.BeginInvoke(delegate
                {
                    dp._settingSelectedDate = true;
                    dp.Text = dp.DateTimeToString(day);
                    dp._settingSelectedDate = false;
                    dp.OnDateSelected(addedDate, removedDate);
                });

                // When DatePickerDisplayDateFlag is TRUE, the SelectedDate
                // change is coming from the Calendar UI itself, so, we
                // shouldn't change the DisplayDate since it will automatically
                // be changed by the Calendar
                if ((day.Month != dp.DisplayDate.Month || day.Year != dp.DisplayDate.Year) && !dp._calendar.DatePickerDisplayDateFlag)
                {
                    dp.DisplayDate = day;
                }
                dp._calendar.DatePickerDisplayDateFlag = false;
            }
            else
            {
                dp._settingSelectedDate = true;
                dp.SetWaterMarkText();
                dp._settingSelectedDate = false;
                dp.OnDateSelected(addedDate, removedDate);
            }
        }
        #endregion SelectedDate

        #region SelectedDateFormat
        /// <summary>
        /// Gets or sets the format that is used to display the selected date.
        /// </summary>
        /// <value>
        /// The format that is used to display the selected date. The default is
        /// <see cref="DatePickerFormat.Short" />.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An specified format is not valid.
        /// </exception>
        public DatePickerFormat SelectedDateFormat
        {
            get { return (DatePickerFormat) GetValue(SelectedDateFormatProperty); }
            set { SetValue(SelectedDateFormatProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedDateFormat" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="SelectedDateFormat" /> dependency property.
        /// </value>
        public static readonly DependencyProperty SelectedDateFormatProperty =
            DependencyProperty.Register(
            nameof(SelectedDateFormat),
            typeof(DatePickerFormat),
            typeof(DatePicker),
            new PropertyMetadata(DatePickerFormat.Short, OnSelectedDateFormatChanged));

        /// <summary>
        /// SelectedDateFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// DatePicker that changed its SelectedDateFormat.
        /// </param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");

            if (IsValidSelectedDateFormat((DatePickerFormat) e.NewValue))
            {
                if (dp._textBox != null)
                {
                    // Update DatePickerTextBox.Text
                    if (string.IsNullOrEmpty(dp._textBox.Text))
                    {
                        dp.SetWaterMarkText();
                    }
                    else
                    {
                        DateTime? date = dp.ParseText(dp._textBox.Text);

                        if (date != null)
                        {
                            string s = dp.DateTimeToString((DateTime) date);
                            dp.Text = s;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(d), "DatePickerFormat value is not valid.");
            }
        }
        #endregion SelectedDateFormat

        #region SelectionBackground
        /// <summary>
        /// Gets or sets the background used for selected dates.
        /// </summary>
        /// <value>
        /// The background used for selected dates.
        /// </value>
        public Brush SelectionBackground
        {
            get { return (Brush) GetValue(SelectionBackgroundProperty); }
            set { SetValue(SelectionBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectionBackground" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="SelectionBackground" /> dependency property.
        /// </value>
        public static readonly DependencyProperty SelectionBackgroundProperty =
            DependencyProperty.Register(
            nameof(SelectionBackground),
            typeof(Brush),
            typeof(DatePicker),
            null);
        #endregion SelectionBackground

        #region Text
        /// <summary>
        /// Gets or sets the text that is displayed by the <see cref="DatePicker" />.
        /// </summary>
        /// <value>
        /// The text displayed by the <see cref="DatePicker" />.
        /// </value>
        /// <exception cref="FormatException">
        /// The text entered cannot be parsed to a valid date, and the exception
        /// is not suppressed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The text entered parses to a date that is not selectable.
        /// </exception>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Text" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="Text" /> dependency property.
        /// </value>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(DatePicker),
            new PropertyMetadata(string.Empty, OnTextChanged));

        /// <summary>
        /// TextProperty property changed handler.
        /// </summary>
        /// <param name="d">DatePicker that changed its Text.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker dp = d as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");

            if (!dp.IsHandlerSuspended(DatePicker.TextProperty))
            {
                string newValue = e.NewValue as string;
                if (newValue != null)
                {
                    if (dp._textBox != null)
                    {
                        dp._textBox.Text = newValue;
                    }
                    else
                    {
                        dp._defaultText = newValue;
                    }
                    if (!dp._settingSelectedDate)
                    {
                        dp.SetSelectedDate();
                    }
                }
                else
                {
                    if (!dp._settingSelectedDate)
                    {
                        dp.SetValueNoCallback(DatePicker.SelectedDateProperty, null);
                    }
                }
            }
            else
            {
                dp.SetWaterMarkText();
            }
        }
        #endregion Text

        /// <summary>
        /// Builds the visual tree for the <see cref="DatePicker" /> control when a
        /// new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (_popUp != null)
            {
                _popUp.Child = null;
            }

            _popUp = GetTemplateChild(ElementPopup) as Popup;

            if (_popUp != null)
            {
                if (this._outsideCanvas == null)
                {
                    _outsideCanvas = new Canvas();
                    _outsidePopupCanvas = new Canvas();
                    _outsidePopupCanvas.Background = new SolidColorBrush(Colors.Transparent);
                    _outsideCanvas.Children.Add(this._outsidePopupCanvas);
                    _outsideCanvas.Children.Add(this._calendar);
#if MIGRATION
                    _outsidePopupCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(OutsidePopupCanvas_MouseLeftButtonDown);
#else
                    _outsidePopupCanvas.PointerPressed += new MouseButtonEventHandler(OutsidePopupCanvas_MouseLeftButtonDown);
#endif
                }

                _popUp.Child = this._outsideCanvas;
                _root = GetTemplateChild(ElementRoot) as FrameworkElement;

                if (this.IsDropDownOpen)
                {
                    OpenDropDown();
                }
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Click -= new RoutedEventHandler(DropDownButton_Click);
            }

            _dropDownButton = GetTemplateChild(ElementButton) as Button;
            if (_dropDownButton != null)
            {
                _dropDownButton.Click += new RoutedEventHandler(DropDownButton_Click);
                _dropDownButton.IsTabStop = false;

                // If the user does not provide a Content value in template, we
                // provide a helper text that can be used in Accessibility this
                // text is not shown on the UI, just used for Accessibility
                // purposes
                if (_dropDownButton.Content == null)
                {
                    _dropDownButton.Content = "Show Calendar";
                }
            }

            if (_textBox != null)
            {
                _textBox.KeyDown -= new KeyEventHandler(TextBox_KeyDown);
                _textBox.TextChanged -= new TextChangedEventHandler(TextBox_TextChanged);
                _textBox.GotFocus -= new RoutedEventHandler(TextBox_GotFocus);
            }

            _textBox = GetTemplateChild(ElementTextBox) as DatePickerTextBox;

            UpdateDisabledVisual();
            if (this.SelectedDate == null)
            {
                SetWaterMarkText();
            }

            if (_textBox != null)
            {
                _textBox.KeyDown += new KeyEventHandler(TextBox_KeyDown);
                _textBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
                _textBox.GotFocus += new RoutedEventHandler(TextBox_GotFocus);

                if (this.SelectedDate == null)
                {
                    if (!string.IsNullOrEmpty(this._defaultText))
                    {
                        _textBox.Text = this._defaultText;
                        SetSelectedDate();
                    }
                }
                else
                {
                    _textBox.Text = this.DateTimeToString((DateTime) this.SelectedDate);
                }
            }
        }

        /// <summary>
        /// Provides a text representation of the selected date.
        /// </summary>
        /// <returns>
        /// A text representation of the selected date, or an empty string if
        /// SelectedDate is a null reference.
        /// </returns>
        public override string ToString()
        {
            if (this.SelectedDate != null)
            {
                return this.SelectedDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat());
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns a <see cref="DatePickerAutomationPeer" /> for use by the 
        /// Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="DatePickerAutomationPeer" /> for the 
        /// <see cref="DatePicker" /> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DatePickerAutomationPeer(this);
        }

        /// <summary>
        /// Raises the <see cref="DateValidationError" /> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DatePickerDateValidationErrorEventArgs" /> that 
        /// contains the event data.
        /// </param>
        protected virtual void OnDateValidationError(DatePickerDateValidationErrorEventArgs e)
        {
            EventHandler<DatePickerDateValidationErrorEventArgs> handler = this.DateValidationError;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_DayButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            this.IsDropDownOpen = false;
#if MIGRATION
            _calendar.ReleaseMouseCapture();
#else
            _calendar.ReleasePointerCapture(e.Pointer);
#endif
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            if (e.AddedDate != this.DisplayDate)
            {
                SetValue(DisplayDateProperty, (DateTime) e.AddedDate);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_KeyDown(object sender, KeyEventArgs e)
        {
            Calendar c = sender as Calendar;
            Debug.Assert(c != null, "The Calendar should not be null!");

            if (!e.Handled && (e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.Escape) && c.DisplayMode == CalendarMode.Month)
            {
                this.Focus();
                this.IsDropDownOpen = false;

                if (e.Key == Key.Escape)
                {
                    this.SelectedDate = this._onOpenSelectedDate;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Default mouse wheel handler for the DatePicker control.
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
            if (!e.Handled && this.SelectedDate.HasValue)
            {
                DateTime selectedDate = this.SelectedDate.Value;
                DateTime? newDate = DateTimeHelper.AddDays(selectedDate, e.Delta > 0 ? -1 : 1);
                if (newDate.HasValue && Calendar.IsValidDateSelection(this._calendar, newDate.Value))
                {
                    this.SelectedDate = newDate;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Assert(e.AddedItems.Count < 2, "There should be less than 2 AddedItems!");

            if (e.AddedItems.Count > 0 && this.SelectedDate.HasValue && DateTime.Compare((DateTime) e.AddedItems[0], this.SelectedDate.Value) != 0)
            {
                this.SelectedDate = (DateTime?) e.AddedItems[0];
            }
            else
            {
                if (e.AddedItems.Count == 0)
                {
                    this.SelectedDate = null;
                    return;
                }

                if (!this.SelectedDate.HasValue)
                {
                    if (e.AddedItems.Count > 0)
                    {
                        this.SelectedDate = (DateTime?) e.AddedItems[0];
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void Calendar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPopUpPosition();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private string DateTimeToString(DateTime d)
        {
            DateTimeFormatInfo dtfi = DateTimeHelper.GetCurrentDateFormat();

            switch (this.SelectedDateFormat)
            {
                case DatePickerFormat.Short:
                    return string.Format(CultureInfo.CurrentCulture, d.ToString(dtfi.ShortDatePattern, dtfi));
                case DatePickerFormat.Long:
                    return string.Format(CultureInfo.CurrentCulture, d.ToString(dtfi.LongDatePattern, dtfi));
            }
            return null;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            DatePicker dp = sender as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");
            Debug.Assert(e.OriginalSource != null, "The OriginalSource should not be null!");

            if (dp.IsDropDownOpen && dp._dropDownButton != null && !(e.OriginalSource.Equals(dp._textBox)) && !(e.OriginalSource.Equals(dp._dropDownButton)) && !(e.OriginalSource.Equals(dp._calendar)))
            {
                dp.IsDropDownOpen = false;
            }
            SetSelectedDate();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void DatePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            DatePicker dp = sender as DatePicker;
            Debug.Assert(dp != null, "The DatePicker should not be null!");
            if (this.IsEnabled && this._textBox != null)
            {
                this._textBox.Focus();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="d">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private static DateTime DiscardDayTime(DateTime d)
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
        private static DateTime? DiscardTime(DateTime? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                DateTime discarded = (DateTime) d;
                int year = discarded.Year;
                int month = discarded.Month;
                int day = discarded.Day;
                DateTime newD = new DateTime(year, month, day, 0, 0, 0);
                return newD;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            HandlePopUp();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void HandlePopUp()
        {
            if (this.IsDropDownOpen)
            {
                this.Focus();
                this.IsDropDownOpen = false;
#if MIGRATION
                _calendar.ReleaseMouseCapture();
#else
                _calendar.ReleasePointerCapture();
#endif
            }
            else
            {
                Debug.Assert(!this.IsDropDownOpen, "The drop down should be closed!");
#if MIGRATION
                _calendar.CaptureMouse();
#else
                _calendar.CapturePointer();
#endif
                ProcessTextBox();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void InitializeCalendar()
        {
            _calendar = new Calendar();
            _calendar.DayButtonMouseUp += new MouseButtonEventHandler(Calendar_DayButtonMouseUp);
            _calendar.DisplayDateChanged += new EventHandler<CalendarDateChangedEventArgs>(Calendar_DisplayDateChanged);
            _calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(Calendar_SelectedDatesChanged);
#if MIGRATION
            _calendar.MouseLeftButtonDown += new MouseButtonEventHandler(Calendar_MouseLeftButtonDown);
#else
            _calendar.PointerPressed += new MouseButtonEventHandler(Calendar_MouseLeftButtonDown);
#endif
            _calendar.KeyDown += new KeyEventHandler(Calendar_KeyDown);
            _calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            _calendar.SizeChanged += new SizeChangedEventHandler(Calendar_SizeChanged);
            _calendar.IsTabStop = true;
        }

        /// <summary>
        /// Sets the matrix to its inverse.
        /// </summary>
        /// <param name="matrix">Matrix to be inverted.</param>
        /// <returns>
        /// True if the Matrix is invertible, false otherwise.
        /// </returns>
        private static bool InvertMatrix(ref Matrix matrix)
        {
            double determinant = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;

            if (determinant == 0.0)
            {
                return false;
            }

            Matrix matCopy = matrix;
            matrix.M11 = matCopy.M22 / determinant;
            matrix.M12 = -1 * matCopy.M12 / determinant;
            matrix.M21 = -1 * matCopy.M21 / determinant;
            matrix.M22 = matCopy.M11 / determinant;
            matrix.OffsetX = (matCopy.OffsetY * matCopy.M21 - matCopy.OffsetX * matCopy.M22) / determinant;
            matrix.OffsetY = (matCopy.OffsetX * matCopy.M12 - matCopy.OffsetY * matCopy.M11) / determinant;

            return true;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="value">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private static bool IsValidSelectedDateFormat(DatePickerFormat value)
        {
            DatePickerFormat format = (DatePickerFormat) value;
            return format == DatePickerFormat.Long
                || format == DatePickerFormat.Short;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        private void OnCalendarClosed(RoutedEventArgs e)
        {
            RoutedEventHandler handler = this.CalendarClosed;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        private void OnCalendarOpened(RoutedEventArgs e)
        {
            RoutedEventHandler handler = this.CalendarOpened;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="addedDate">The added date.</param>
        /// <param name="removedDate">The removed date.</param>
        private void OnDateSelected(DateTime? addedDate, DateTime? removedDate)
        {
            EventHandler<SelectionChangedEventArgs> handler = this.SelectedDateChanged;
            if (null != handler)
            {
                Collection<DateTime> addedItems = new Collection<DateTime>();
                Collection<DateTime> removedItems = new Collection<DateTime>();

                if (addedDate.HasValue)
                {
                    addedItems.Add(addedDate.Value);
                }

                if (removedDate.HasValue)
                {
                    removedItems.Add(removedDate.Value);
                }

                handler(this, new SelectionChangedEventArgs(removedItems, addedItems));
            }

            DatePickerAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as DatePickerAutomationPeer;
            if (peer != null)
            {
                string oldValue = removedDate.HasValue ? removedDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat()) : string.Empty;
                string newValue = addedDate.HasValue ? addedDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat()) : string.Empty;
                peer.RaiseValuePropertyChangedEvent(oldValue, newValue);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void OpenDropDown()
        {
            this._calendar.Focus();
            OpenPopUp();
            this._calendar.ResetStates();
            this.OnCalendarOpened(new RoutedEventArgs());
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void OpenPopUp()
        {
            FrameworkElement page = (Application.Current != null) ?
                       Application.Current.RootVisual as FrameworkElement :
                       null;

            if (page != null)
            {
                this._onOpenSelectedDate = this.SelectedDate;
                this._popUp.IsOpen = true;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void OutsidePopupCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        /// <summary>
        /// Input text is parsed in the correct format and changed into a
        /// DateTime object.  If the text can not be parsed TextParseError Event
        /// is thrown.
        /// </summary>
        /// <param name="text">Inherited code: Requires comment.</param>
        /// <returns>
        /// IT SHOULD RETURN NULL IF THE STRING IS NOT VALID, RETURN THE
        /// DATETIME VALUE IF IT IS VALID.
        /// </returns>
        private DateTime? ParseText(string text)
        {
            DateTime newSelectedDate;

            // TryParse is not used in order to be able to pass the exception to
            // the TextParseError event
            try
            {
                newSelectedDate = DateTime.Parse(text, DateTimeHelper.GetCurrentDateFormat());

                if (Calendar.IsValidDateSelection(this._calendar, newSelectedDate))
                {
                    return newSelectedDate;
                }
                else
                {
                    DatePickerDateValidationErrorEventArgs dateValidationError = new DatePickerDateValidationErrorEventArgs(new ArgumentOutOfRangeException(nameof(text), "SelectedDate value is not valid."), text);
                    OnDateValidationError(dateValidationError);

                    if (dateValidationError.ThrowException)
                    {
                        throw dateValidationError.Exception;
                    }
                }
            }
            catch (FormatException ex)
            {
                DatePickerDateValidationErrorEventArgs textParseError = new DatePickerDateValidationErrorEventArgs(ex, text);
                OnDateValidationError(textParseError);

                if (textParseError.ThrowException)
                {
                    throw textParseError.Exception;
                }
            }
            return null;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private bool ProcessDatePickerKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        SetSelectedDate();
                        return true;
                    }
                case Key.Down:
                    {
                        // REMOVE_RTM: Ctrl+Down may be changed into Alt+Down once the related Jolt bug is fixed 
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        {
                            HandlePopUp();
                        }
                        return true;
                    }
            }
            return false;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void ProcessTextBox()
        {
            SetSelectedDate();
            this.IsDropDownOpen = true;
            _calendar.Focus();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetPopUpPosition()
        {
            if (this._calendar != null && Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                double pageHeight = Application.Current.Host.Content.ActualHeight;
                double pageWidth = Application.Current.Host.Content.ActualWidth;
                double calendarHeight = this._calendar.ActualHeight;
                double actualHeight = this.ActualHeight;

                if (this._root != null)
                {
                    GeneralTransform gt = this._root.TransformToVisual(null);

                    if (gt != null)
                    {
                        Point point00 = new Point(0, 0);
                        Point point10 = new Point(1, 0);
                        Point point01 = new Point(0, 1);
                        Point transform00 = gt.Transform(point00);
                        Point transform10 = gt.Transform(point10);
                        Point transform01 = gt.Transform(point01);

                        double dpX = transform00.X;
                        double dpY = transform00.Y;

                        double calendarX = dpX;
                        double calendarY = dpY + actualHeight;

                        // if the page height is less then the total height of
                        // the PopUp + DatePicker or if we can fit the PopUp
                        // inside the page, we want to place the PopUp to the
                        // bottom
                        if (pageHeight < calendarY + calendarHeight)
                        {
                            calendarY = dpY - calendarHeight;
                        }
                        this._popUp.HorizontalOffset = 0;
                        this._popUp.VerticalOffset = 0;
                        this._outsidePopupCanvas.Width = pageWidth;
                        this._outsidePopupCanvas.Height = pageHeight;
                        this._calendar.HorizontalAlignment = HorizontalAlignment.Left;
                        this._calendar.VerticalAlignment = VerticalAlignment.Top;
                        Canvas.SetLeft(this._calendar, calendarX - dpX);
                        Canvas.SetTop(this._calendar, calendarY - dpY);

                        // Transform the invisible canvas to plugin coordinate
                        // space origin
                        Matrix transformToRootMatrix = Matrix.Identity;
                        transformToRootMatrix.M11 = transform10.X - transform00.X;
                        transformToRootMatrix.M12 = transform10.Y - transform00.Y;
                        transformToRootMatrix.M21 = transform01.X - transform00.X;
                        transformToRootMatrix.M22 = transform01.Y - transform00.Y;
                        transformToRootMatrix.OffsetX = transform00.X;
                        transformToRootMatrix.OffsetY = transform00.Y;
                        MatrixTransform mt = new MatrixTransform();
                        InvertMatrix(ref transformToRootMatrix);
                        mt.Matrix = transformToRootMatrix;
                        this._outsidePopupCanvas.RenderTransform = mt;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetSelectedDate()
        {
            if (this._textBox != null)
            {
                if (!string.IsNullOrEmpty(this._textBox.Text))
                {
                    string s = this._textBox.Text;

                    if (this.SelectedDate != null)
                    {
                        // If the string value of the SelectedDate and the
                        // TextBox string value are equal, we do not parse the
                        // string again if we do an extra parse, we lose data in
                        // M/d/yy format.
                        // ex: SelectedDate = DateTime(1008,12,19) but when
                        // "12/19/08" is parsed it is interpreted as
                        // DateTime(2008,12,19)
                        string selectedDate = DateTimeToString(this.SelectedDate.Value);
                        if (selectedDate == s)
                        {
                            return;
                        }
                    }
                    DateTime? d = SetTextBoxValue(s);
                    if (!this.SelectedDate.Equals(d))
                    {
                        this.SelectedDate = d;
                    }
                }
                else
                {
                    if (this.SelectedDate != null)
                    {
                        this.SelectedDate = null;
                    }
                }
            }
            else
            {
                DateTime? d = SetTextBoxValue(_defaultText);
                if (!this.SelectedDate.Equals(d))
                {
                    this.SelectedDate = d;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="s">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private DateTime? SetTextBoxValue(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SetValue(TextProperty, s);
                return this.SelectedDate;
            }
            else
            {
                DateTime? d = ParseText(s);
                if (d != null)
                {
                    SetValue(TextProperty, s);
                    return d;
                }
                else
                {
                    // If parse error: TextBox should have the latest valid
                    // selecteddate value:
                    if (this.SelectedDate != null)
                    {
                        string newtext = this.DateTimeToString((DateTime) this.SelectedDate);
                        SetValue(TextProperty, newtext);
                        return this.SelectedDate;
                    }
                    else
                    {
                        SetWaterMarkText();
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void SetWaterMarkText()
        {
            if (this._textBox != null)
            {
                DateTimeFormatInfo dtfi = DateTimeHelper.GetCurrentDateFormat();
                this.Text = string.Empty;
                this._defaultText = string.Empty;

                switch (this.SelectedDateFormat)
                {
                    case DatePickerFormat.Long:
                        {
                            this._textBox.Watermark = string.Format(CultureInfo.CurrentCulture, "<{0}>", dtfi.LongDatePattern.ToString());
                            break;
                        }
                    case DatePickerFormat.Short:
                        {
                            this._textBox.Watermark = string.Format(CultureInfo.CurrentCulture, "<{0}>", dtfi.ShortDatePattern.ToString());
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = ProcessDatePickerKey(e);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._textBox != null)
            {
                this.SetValueNoCallback(DatePicker.TextProperty, this._textBox.Text);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void UpdateDisabledVisual()
        {
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, true, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, true, VisualStates.StateNormal);
            }
        }
    }
}
