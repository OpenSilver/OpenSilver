// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
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
    public partial class DatePicker : Control
    {
        /// <summary>
        /// Exposes test hooks to unit tests with internal access.
        /// </summary>
        private InternalTestHook _testHook;

        /// <summary>
        /// Gets a test hook for unit tests with internal access.
        /// </summary>
        internal InternalTestHook TestHook
        {
            get
            {
                if (_testHook == null)
                {
                    _testHook = new InternalTestHook(this);
                }
                return _testHook;
            }
        }

        /// <summary>
        /// Expose test hooks for internal and private members of the
        /// DatePicker.
        /// </summary>
        internal class InternalTestHook
        {
            /// <summary>
            /// Reference to the outer 'parent' date picker.
            /// </summary>
            private DatePicker _datePicker;

            /// <summary>
            /// Initializes a new instance of the InternalTestHook class.
            /// </summary>
            /// <param name="datePicker">The parent DatePicker.</param>
            internal InternalTestHook(DatePicker datePicker)
            {
                _datePicker = datePicker;
            }

            /// <summary>
            /// Gets the DatePicker's DropDown.
            /// </summary>
            internal Popup DropDown
            {
                get { return _datePicker._popUp; }
            }

            /// <summary>
            /// Gets the DatePicker's DropDownButton.
            /// </summary>
            internal Button DropDownButton
            {
                get { return _datePicker._dropDownButton; }
            }

            /// <summary>
            /// Gets or sets the DatePicker's DropDownCalendar.
            /// </summary>
            internal Calendar DropDownCalendar
            {
                get { return _datePicker._calendar; }
                set { _datePicker._calendar = value; }
            }

            /// <summary>
            /// Gets the DatePicker's Watermarked TextBox.
            /// </summary>
            internal DatePickerTextBox DatePickerWatermarkedTextBox
            {
                get { return _datePicker._textBox; }
            }

            /// <summary>
            /// Convert a DateTime to a String.
            /// </summary>
            /// <param name="d">The DateTime.</param>
            /// <returns>A String representation of the DateTime.</returns>
            internal string DateTimeToString(DateTime d)
            {
                return _datePicker.DateTimeToString(d);
            }

            /// <summary>
            /// Click the DatePicker's DropDown Button.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void DropDownButton_Click(object sender, RoutedEventArgs e)
            {
                _datePicker.DropDownButton_Click(sender, e);
            }

            /// <summary>
            /// Set the selected date.
            /// </summary>
            internal void SetSelectedDate()
            {
                _datePicker.SetSelectedDate();
            }
        }
    }
}