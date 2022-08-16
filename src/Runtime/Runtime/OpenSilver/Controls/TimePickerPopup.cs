

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

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Controls
{
    /// <summary>
    /// Base class for a control that can be used as the popup portion in a TimePicker.
    /// </summary>
    [OpenSilver.NotImplemented]
    public abstract class TimePickerPopup : Control, IUpdateVisualState
    {
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            throw new NotImplementedException();
        }

        #region public DateTime? Value
        /// <summary>
        /// Gets or sets the currently selected time.
        /// </summary>
        [OpenSilver.NotImplemented]
        public virtual DateTime? Value
        {
            get => (DateTime?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(DateTime?),
                typeof(TimePickerPopup),
                new PropertyMetadata(default(DateTime?), OnValuePropertyChanged));

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary>
        /// <param name="d">UpDownBase whose Value changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion public DateTime? Value

        #region public DateTime? Minimum
        /// <summary>
        /// Gets or sets the Minimum time considered valid by the control.
        /// </summary>
        /// <remarks>
        /// Setting the Minimum property will be used to prevent users
        /// from choosing values out of range in the TimePickerPopup.
        /// </remarks>
        [OpenSilver.NotImplemented]
        public DateTime? Minimum
        {
            get => (DateTime?)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(DateTime?),
                typeof(TimePickerPopup),
                new PropertyMetadata(null, OnMinimumPropertyChanged));

        /// <summary>
        /// MinimumProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// TimeUpDown that changed its Minimum.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            source.OnMinimumChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the Minimum property value has changed.
        /// </summary>
        /// <param name="oldValue">
        /// Old value of the Minimum property.
        /// </param>
        /// <param name="newValue">
        /// New value of the Minimum property.
        /// </param>
        [OpenSilver.NotImplemented]
        protected virtual void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
        #endregion public DateTime? Minimum

        #region public DateTime? Maximum
        /// <summary>
        /// Gets or sets the Maximum time considered valid by the control.
        /// </summary>
        /// <remarks>
        /// Setting the Maximum property will be used to prevent users
        /// from choosing values out of range in the TimePickerPopup.
        /// </remarks>
        [OpenSilver.NotImplemented]
        public DateTime? Maximum
        {
            get => (DateTime?)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(DateTime?),
                typeof(TimePickerPopup),
                new PropertyMetadata(null, OnMaximumPropertyChanged));

        /// <summary>
        /// MaximumProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// TimeUpDown that changed its Maximum.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            source.OnMaximumChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the Maximum property value has changed.
        /// </summary>
        /// <param name="oldValue">
        /// Old value of the Maximum property.
        /// </param>
        /// <param name="newValue">
        /// New value of the Maximum property.
        /// </param>
        [OpenSilver.NotImplemented]
        protected virtual void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
        #endregion public DateTime? Maximum

        #region public ITimeFormat Format
        /// <summary>
        /// Gets or sets the Format used by the control.
        /// From XAML Use either "Short", "Long" or a custom format.
        /// Custom formats can only contain "H", "h", "m", "s" or "t".
        /// For example: use 'hh:mm:ss' is used to format time as "13:45:30".
        /// </summary>
        [OpenSilver.NotImplemented]
        public ITimeFormat Format
        {
            get { return GetValue(FormatProperty) as ITimeFormat; }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// Identifies the Format dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(
                "Format",
                typeof(ITimeFormat),
                typeof(TimePickerPopup),
                new PropertyMetadata(OnFormatPropertyChanged));

        /// <summary>
        /// FormatProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePickerPopup that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;
            ITimeFormat value = e.NewValue as ITimeFormat;

            source.OnFormatChanged(e.OldValue as ITimeFormat, value);
        }

        /// <summary>
        /// Called when display format changed.
        /// </summary>
        /// <param name="oldValue">The old format.</param>
        /// <param name="newValue">The new format.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnFormatChanged(ITimeFormat oldValue, ITimeFormat newValue)
        {
        }
        #endregion public ITimeFormat Format

        #region public int PopupSecondsInterval
        /// <summary>
        /// Gets or sets the interval of seconds that can be
        /// picked in a popup.
        /// </summary>
        [OpenSilver.NotImplemented]
        public int PopupSecondsInterval
        {
            get => (int)GetValue(PopupSecondsIntervalProperty);
            set => SetValue(PopupSecondsIntervalProperty, value);
        }

        /// <summary>
        /// Identifies the PopupSecondsInterval dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupSecondsIntervalProperty =
            DependencyProperty.Register(
                "PopupSecondsInterval",
                typeof(int),
                typeof(TimePickerPopup),
                new PropertyMetadata(OnPopupSecondsIntervalPropertyChanged));

        /// <summary>
        /// PopupSecondsIntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupSecondsInterval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupSecondsIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;

            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;

            source.OnPopupSecondsIntervalChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the popup seconds interval changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnPopupSecondsIntervalChanged(int oldValue, int newValue)
        {
        }
        #endregion public int PopupSecondsInterval


        #region public int PopupMinutesInterval
        /// <summary>
        /// Gets or sets the interval of minutes that can be picked in a popup.
        /// </summary>
        [OpenSilver.NotImplemented]
        public int PopupMinutesInterval
        {
            get => (int)GetValue(PopupMinutesIntervalProperty);
            set => SetValue(PopupMinutesIntervalProperty, value);
        }

        /// <summary>
        /// Identifies the PopupMinutesInterval dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupMinutesIntervalProperty =
            DependencyProperty.Register(
                "PopupMinutesInterval",
                typeof(int),
                typeof(TimePickerPopup),
                new PropertyMetadata(OnPopupMinutesIntervalPropertyChanged));

        /// <summary>
        /// PopupMinutesIntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// TimePicker that changed its PopupMinutesInterval.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private static void OnPopupMinutesIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;

            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;

            source.OnPopupMinutesIntervalChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the popup minutes interval changed.
        /// </summary>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        [OpenSilver.NotImplemented]
        protected virtual void OnPopupMinutesIntervalChanged(int oldValue, int newValue)
        {
        }
        #endregion public int PopupMinutesInterval

        #region public PopupTimeSelectionMode PopupTimeSelectionMode
        /// <summary>
        /// Gets or sets the whether the TimePickerPopup supports selecting designators and/or seconds.
        /// </summary>
        [OpenSilver.NotImplemented]
        public PopupTimeSelectionMode PopupTimeSelectionMode
        {
            get => (PopupTimeSelectionMode)GetValue(PopupTimeSelectionModeProperty);
            set => SetValue(PopupTimeSelectionModeProperty, value);
        }

        /// <summary>
        /// Identifies the PopupTimeSelectionMode dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupTimeSelectionModeProperty =
            DependencyProperty.Register(
                "PopupTimeSelectionMode",
                typeof(PopupTimeSelectionMode),
                typeof(TimePickerPopup),
                new PropertyMetadata(PopupTimeSelectionMode.HoursAndMinutesOnly, OnPopupTimeSelectionModePropertyChanged));

        /// <summary>
        /// PopupTimeSelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// TimePickerPopup that changed its PopupTimeSelectionMode.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private static void OnPopupTimeSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;

            PopupTimeSelectionMode value = (PopupTimeSelectionMode)e.NewValue;

            source.OnPopupTimeSelectionModeChanged((PopupTimeSelectionMode)e.OldValue, (PopupTimeSelectionMode)e.NewValue);
        }

        /// <summary>
        /// Called when the time selection mode is changed.
        /// </summary>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        [OpenSilver.NotImplemented]
        protected virtual void OnPopupTimeSelectionModeChanged(PopupTimeSelectionMode oldValue, PopupTimeSelectionMode newValue)
        {
        }
        #endregion public PopupTimeSelectionMode PopupTimeSelectionMode
    }
}
