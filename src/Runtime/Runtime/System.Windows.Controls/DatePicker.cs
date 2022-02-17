

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Globalization;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Automation.Peers;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Xaml.Automation.Peers;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class DatePicker : INTERNAL_DateTimePickerBase
    {
        public DatePicker()
        {
            _defaultText = ""; // the text displayed when no date is selected

            // Set default style:
            this.DefaultStyleKey = typeof(DatePicker);
            this.GotFocus += OnGotFocus;
            this.LostFocus += OnLostFocus;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            SetSelectedDate();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (IsEnabled && _textBox != null)
                _textBox.Focus();
        }

        private static DateTime? ParseText(string text)
        {
            if (DateTime.TryParse(text, out var dt))
            {
                return dt;
            }

            return null;
        }

        /// <summary>
        /// Updates selected date according to current Text property value
        /// </summary>
        private void SetSelectedDate()
        {
            if (_textBox == null)
            {
                var date = SetTextBoxValue(_defaultText);
                if (!this.SelectedDate.Equals(date))
                {
                    this.SelectedDate = date;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_textBox.Text))
                {
                    this.SelectedDate = null;
                    SetWaterMarkText();
                }
                else
                {
                    if (this.SelectedDate != null)
                    {
                        string selectedDate = SetTextFromDate(this.SelectedDate);
                        if (selectedDate == _textBox.Text)
                            return;
                    }

                    var date = SetTextBoxValue(_textBox.Text);
                    if (!this.SelectedDate.Equals(date))
                    {
                        this.SelectedDate = date;
                    }
                }
            }
        }

        protected override void OnTextChanged()
        {
            SetSelectedDate();
        }

        protected override void OnEmptyText()
        {
            this.SetValueNoCallback(SelectedDateProperty, null);
        }

        protected override INTERNAL_CalendarOrClockBase GenerateCalendarOrClock()
        {
            return new Calendar();
        }

        protected override string SetTextFromDate(DateTime? newDate)
        {
            if (newDate == null)
                return null;

            DateTimeFormatInfo dtfi = GetCurrentDateFormat();

            switch (this.SelectedDateFormat)
            {
                case DatePickerFormat.Short:
                    {
                        return string.Format(CultureInfo.CurrentCulture, newDate.Value.ToString(dtfi.ShortDatePattern, dtfi));
                    }
                case DatePickerFormat.Long:
                    {
                        return string.Format(CultureInfo.CurrentCulture, newDate.Value.ToString(dtfi.LongDatePattern, dtfi));
                    }
            }

            return null;
        }

        protected override void OnSelectionChanged(DateTime? newSelectedDate)
        {
            if (newSelectedDate.HasValue && SelectedDate.HasValue)
            {
                if (SelectedDate != newSelectedDate) // safety to avoid dependency loop
                    SelectedDate = newSelectedDate;
            }
            else if (newSelectedDate.HasValue && !SelectedDate.HasValue) // if the previous value is null, but not the new one 
            {
                SelectedDate = newSelectedDate;
            }
            else // if the new value is null
            {
                if (SelectedDate != null) // safety to avoid dependency loop
                    SelectedDate = null;
            }

            IsDropDownOpen = false; // close the popup
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (_textBox != null)
            {
                _textBox.TextChanged += OnTextBoxTextChanged;
                _textBox.KeyDown += OnTextBoxKeyDown;
                _textBox.LostFocus += OnTextBoxLostFocus;
            }
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            SetSelectedDate();
        }

#if MIGRATION
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
#else
        private void OnTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
#endif
        {
            if (!e.Handled)
            {
                e.Handled = ProcessDatePickerKey(e);
            }
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetValueNoCallback(TextProperty, _textBox.Text);
        }

        #region dependency property Selection for DatePicker

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set
            {
                SetValue(SelectedDateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
            "SelectedDate",
            typeof(DateTime?),
            typeof(DatePicker),
            new PropertyMetadata(OnSelectedDateChanged)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">TemporalPicker that changed its SelectedDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = d as DatePicker;
            if (datePicker != null)
            {
                var newDate = e.NewValue as DateTime?;
                datePicker.INTERNAL_SelectedDate = newDate;

                if (newDate == null)
                    datePicker.SetWaterMarkText();
                if (datePicker.SelectedDate.HasValue)
                {
                    datePicker.Text = datePicker.DateTimeToString(datePicker.SelectedDate.Value);
                }
            }
        }


        private string DateTimeToString(DateTime d)
        {
            DateTimeFormatInfo currentDateFormat = DateTimeHelper.GetCurrentDateFormat();
            switch (SelectedDateFormat)
            {
                case DatePickerFormat.Short:
                    return string.Format(CultureInfo.CurrentCulture, d.ToString(currentDateFormat.ShortDatePattern, currentDateFormat));
                case DatePickerFormat.Long:
                    return string.Format(CultureInfo.CurrentCulture, d.ToString(currentDateFormat.LongDatePattern, currentDateFormat));
                default:
                    return null;
            }
        }

        public static readonly DependencyProperty SelectedDateFormatProperty = DependencyProperty.Register(
            "SelectedDateFormat",
            typeof(DatePickerFormat),
            typeof(DatePicker),
            new PropertyMetadata(DatePickerFormat.Short));

        public DatePickerFormat SelectedDateFormat
        {
            get => (DatePickerFormat)GetValue(SelectedDateFormatProperty);
            set => SetValue(SelectedDateFormatProperty, value);
        }

        #endregion

        protected override void SetWaterMarkText()
        {
            var textBox = _textBox as DatePickerTextBox;
            if (textBox == null)
                return;

            DateTimeFormatInfo dtfi = GetCurrentDateFormat();
            switch (this.SelectedDateFormat)
            {
                case DatePickerFormat.Long:
                    {
                        textBox.Watermark = string.Format(CultureInfo.CurrentCulture, "<{0}>", dtfi.LongDatePattern.ToString());
                        break;
                    }
                case DatePickerFormat.Short:
                    {
                        textBox.Watermark = string.Format(CultureInfo.CurrentCulture, "<{0}>", dtfi.ShortDatePattern.ToString());
                        break;
                    }
            }
        }

        private DateTimeFormatInfo GetCurrentDateFormat()
        {
#if NETSTANDARD
            if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat;
            }
            else
            {
                foreach (global::System.Globalization.Calendar cal in CultureInfo.CurrentCulture.OptionalCalendars)
                {
                    if (cal is GregorianCalendar)
                    {
                        //if the default calendar is not Gregorian, return the first supported GregorianCalendar dtfi
                        DateTimeFormatInfo dtfi = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
                        dtfi.Calendar = cal;
                        return dtfi;
                    }
                }

                //if there are no GregorianCalendars in the OptionalCalendars list, use the invariant dtfi
                DateTimeFormatInfo dt = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
                dt.Calendar = new GregorianCalendar();
                return dt;
            }
#elif BRIDGE
            return CultureInfo.CurrentCulture.DateTimeFormat;
#endif
        }

#if MIGRATION
        private bool ProcessDatePickerKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    SetSelectedDate();
                    return true;

                case Key.Down:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        HandlePopUp();
                    }
                    return true;
            }

            return false;
        }
#else
        private bool ProcessDatePickerKey(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    SetSelectedDate();
                    return true;

                case VirtualKey.Down:
                    if ((global::System.Windows.Input.Keyboard.Modifiers & VirtualKeyModifiers.Control) == VirtualKeyModifiers.Control)
                    {
                        HandlePopUp();
                    }
                    return true;
            }

            return false;
        }
#endif


        private void HandlePopUp()
        {
            if (this.IsDropDownOpen)
            {
                this.Focus();
                this.IsDropDownOpen = false;
#if MIGRATION
                _calendarOrClock.ReleaseMouseCapture();
#else
                _calendarOrClock.ReleasePointerCapture();
#endif
            }
            else
            {
#if MIGRATION
                _calendarOrClock.CaptureMouse();
#else
                _calendarOrClock.CapturePointer();
#endif
                ProcessTextBox();
            }
        }

        private void ProcessTextBox()
        {
            SetSelectedDate();
            this.IsDropDownOpen = true;
        }

        private DateTime? SetTextBoxValue(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                SetValue(TextProperty, text);
                return this.SelectedDate;
            }
            else
            {
                DateTime? date = ParseText(text);
                if (date != null)
                {
                    SetValue(TextProperty, DateTimeToString(date.Value));
                    return date;
                }
                else
                {
                    if (this.SelectedDate != null)
                    {
                        string newText = SetTextFromDate(this.SelectedDate);
                        SetValue(TextProperty, newText);
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
    }
}
