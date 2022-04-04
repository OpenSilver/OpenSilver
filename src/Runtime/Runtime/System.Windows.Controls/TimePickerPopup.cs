

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Automation.Peers;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Base class for a control that can be used as the popup portion in a TimePicker.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowSecondsAndDesignatorsSelection")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplateVisualState(GroupName = "ContainedByPickerStates", Name = "Contained")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowTimeDesignatorsSelection")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowSecondsSelection")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "HoursAndMinutesOnly")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "ContainedByPickerStates", Name = "NotContained")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    public abstract class TimePickerPopup : Control, ITimeInput, IUpdateVisualState
    {
        /// <summary>Identifies the Value dependency property.</summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(DateTime?), typeof(TimePickerPopup), new PropertyMetadata((object)new DateTime?(), new PropertyChangedCallback(TimePickerPopup.OnValuePropertyChanged)));
        /// <summary>Identifies the Minimum dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(DateTime?), typeof(TimePickerPopup), new PropertyMetadata((object)null, new PropertyChangedCallback(TimePickerPopup.OnMinimumPropertyChanged)));
        /// <summary>Identifies the Maximum dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(DateTime?), typeof(TimePickerPopup), new PropertyMetadata((object)null, new PropertyChangedCallback(TimePickerPopup.OnMaximumPropertyChanged)));
        /// <summary>Identifies the Culture dependency property.</summary>
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(nameof(Culture), typeof(CultureInfo), typeof(TimePickerPopup), new PropertyMetadata((object)null, new PropertyChangedCallback(TimePickerPopup.OnCulturePropertyChanged)));
        /// <summary>
        /// Identifies the TimeGlobalizationInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeGlobalizationInfoProperty = DependencyProperty.Register(nameof(TimeGlobalizationInfo), typeof(TimeGlobalizationInfo), typeof(TimePickerPopup), new PropertyMetadata(new PropertyChangedCallback(TimePickerPopup.OnTimeGlobalizationInfoPropertyChanged)));
        /// <summary>Identifies the Format dependency property.</summary>
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(nameof(Format), typeof(ITimeFormat), typeof(TimePickerPopup), new PropertyMetadata(new PropertyChangedCallback(TimePickerPopup.OnFormatPropertyChanged)));
        /// <summary>
        /// Identifies the PopupSecondsInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupSecondsIntervalProperty = DependencyProperty.Register(nameof(PopupSecondsInterval), typeof(int), typeof(TimePickerPopup), new PropertyMetadata(new PropertyChangedCallback(TimePickerPopup.OnPopupSecondsIntervalPropertyChanged)));
        /// <summary>
        /// Identifies the PopupMinutesInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupMinutesIntervalProperty = DependencyProperty.Register(nameof(PopupMinutesInterval), typeof(int), typeof(TimePickerPopup), new PropertyMetadata(new PropertyChangedCallback(TimePickerPopup.OnPopupMinutesIntervalPropertyChanged)));
        /// <summary>
        /// Identifies the PopupTimeSelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupTimeSelectionModeProperty = DependencyProperty.Register(nameof(PopupTimeSelectionMode), typeof(PopupTimeSelectionMode), typeof(TimePickerPopup), new PropertyMetadata((object)PopupTimeSelectionMode.HoursAndMinutesOnly, new PropertyChangedCallback(TimePickerPopup.OnPopupTimeSelectionModePropertyChanged)));
        /// <summary>
        /// The name of the visual state that represents a Popup that is
        /// contained by a picker.
        /// </summary>
        internal const string ContainedStateName = "Contained";
        /// <summary>
        /// The name of the visual state that represent a Popup that is not
        /// contained by a picker.
        /// </summary>
        internal const string NotContainedStateName = "NotContained";
        /// <summary>The name of the ContainedByPicker state group.</summary>
        internal const string ContainedByPickerGroupName = "ContainedByPickerStates";
        /// <summary>The name of the PopupMode state group.</summary>
        internal const string PopupModeGroupName = "PopupModeStates";
        /// <summary>
        /// The name of the visual state that represents a PopupMode where
        /// TimeDesignators, Hours, Minutes and Seconds can be picked.
        /// </summary>
        internal const string AllowSecondsAndDesignatorsSelectionStateName = "AllowSecondsAndDesignatorsSelection";
        /// <summary>
        /// The name of the visual state that represents a PopupMode where
        /// TimeDesignators, Hours and Minutes can be picked.
        /// </summary>
        internal const string AllowTimeDesignatorsSelectionStateName = "AllowTimeDesignatorsSelection";
        /// <summary>
        /// The name of the visual state that represents a PopupMode where
        /// Hours, Minutes and Seconds can be picked.
        /// </summary>
        internal const string AllowSecondsSelectionStateName = "AllowSecondsSelection";
        /// <summary>
        /// The name of the visual state that represents a PopupMode where
        /// Hours and Minutes can be picked.
        /// </summary>
        internal const string HoursAndMinutesOnlyStateName = "HoursAndMinutesOnly";
        /// <summary>
        /// Helper class that centralizes the coercion logic across all
        /// TimeInput controls.
        /// </summary>
        private readonly TimeCoercionHelper _timeCoercionHelper;
        /// <summary>BackingField of TimePickerParent.</summary>
        private TimePicker _timePickerParent;
        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality.
        /// </summary>
        private InteractionHelper _interaction;
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignoreValueChange;
        /// <summary>BackingField for ActualTimeGlobalizationInfo.</summary>
        private TimeGlobalizationInfo _actualTimeGlobalizationInfo;
        /// <summary>BackingField for ActualFormat.</summary>
        private ITimeFormat _actualFormat;

        /// <summary>Gets or sets the container for this Popup.</summary>
        /// <value>The TimePicker that contains this Popup.</value>
        internal TimePicker TimePickerParent
        {
            get
            {
                return this._timePickerParent;
            }
            set
            {
                this._timePickerParent = value;
                this.UpdateVisualState(true);
            }
        }

        /// <summary>Gets or sets the currently selected time.</summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public virtual DateTime? Value
        {
            get
            {
                return (DateTime?)this.GetValue(TimePickerPopup.ValueProperty);
            }
            set
            {
                this.SetValue(TimePickerPopup.ValueProperty, (object)value);
            }
        }

        /// <summary>ValueProperty property changed handler.</summary>
        /// <param name="d">UpDownBase whose Value changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValuePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            if (timePickerPopup._ignoreValueChange)
            {
                timePickerPopup.OnValueChanged(new RoutedPropertyChangedEventArgs<DateTime?>(e.OldValue as DateTime?, e.NewValue as DateTime?));
            }
            else
            {
                DateTime? oldValue = (DateTime?)e.OldValue;
                DateTime? newValue1 = (DateTime?)e.NewValue;
                RoutedPropertyChangingEventArgs<DateTime?> e1 = new RoutedPropertyChangingEventArgs<DateTime?>(e.Property, oldValue, newValue1, true);
                timePickerPopup.OnValueChanging(e1);
                if (e1.InCoercion)
                    return;
                if (!e1.Cancel)
                {
                    DateTime? newValue2 = e1.NewValue;
                    RoutedPropertyChangedEventArgs<DateTime?> e2 = new RoutedPropertyChangedEventArgs<DateTime?>(oldValue, newValue2);
                    timePickerPopup.OnValueChanged(e2);
                }
                else
                {
                    timePickerPopup._ignoreValueChange = true;
                    timePickerPopup.Value = oldValue;
                    timePickerPopup._ignoreValueChange = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Minimum property will be used to prevent users
        /// from choosing values out of range in the TimePickerPopup.</remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum
        {
            get
            {
                return (DateTime?)this.GetValue(TimePickerPopup.MinimumProperty);
            }
            set
            {
                this.SetValue(TimePickerPopup.MinimumProperty, (object)value);
            }
        }

        /// <summary>MinimumProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;
            int num;
            if (timePickerPopup.TimePickerParent != null)
            {
                DateTime? minimum = timePickerPopup.TimePickerParent.Minimum;
                DateTime? nullable = newValue;
                num = (minimum.HasValue != nullable.HasValue ? 1 : (!minimum.HasValue ? 0 : (minimum.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0))) == 0 ? 1 : 0;
            }
            else
                num = 1;
            if (num == 0)
                timePickerPopup.TimePickerParent.SetValue(TimePicker.MinimumProperty, (object)newValue);
            timePickerPopup._ignoreValueChange = true;
            timePickerPopup._timeCoercionHelper.ProcessMinimumChange(newValue);
            timePickerPopup._ignoreValueChange = false;
            timePickerPopup.OnMinimumChanged(oldValue, newValue);
        }

        /// <summary>Called when the Minimum property value has changed.</summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }

        /// <summary>
        /// Gets or sets the Maximum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Maximum property will be used to prevent users
        /// from choosing values out of range in the TimePickerPopup.</remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum
        {
            get
            {
                return (DateTime?)this.GetValue(TimePickerPopup.MaximumProperty);
            }
            set
            {
                this.SetValue(TimePickerPopup.MaximumProperty, (object)value);
            }
        }

        /// <summary>MaximumProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;
            int num;
            if (timePickerPopup.TimePickerParent != null)
            {
                DateTime? maximum = timePickerPopup.TimePickerParent.Maximum;
                DateTime? nullable = newValue;
                num = (maximum.HasValue != nullable.HasValue ? 1 : (!maximum.HasValue ? 0 : (maximum.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0))) == 0 ? 1 : 0;
            }
            else
                num = 1;
            if (num == 0)
                timePickerPopup.TimePickerParent.SetValue(TimePicker.MaximumProperty, (object)newValue);
            timePickerPopup._ignoreValueChange = true;
            timePickerPopup._timeCoercionHelper.ProcessMaximumChange(newValue);
            timePickerPopup._ignoreValueChange = false;
            timePickerPopup.OnMaximumChanged(oldValue, newValue);
        }

        /// <summary>Called when the Maximum property value has changed.</summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }

        /// <summary>
        /// Gets or sets the culture that will be used by the control for
        /// time formatting.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)this.GetValue(TimePickerPopup.CultureProperty);
            }
            set
            {
                this.SetValue(TimePickerPopup.CultureProperty, (object)value);
            }
        }

        /// <summary>CultureProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Culture.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCulturePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            CultureInfo newValue = e.NewValue as CultureInfo;
            timePickerPopup.ActualTimeGlobalizationInfo.Culture = newValue;
            if (timePickerPopup.TimePickerParent != null && timePickerPopup.TimePickerParent.Culture != timePickerPopup.ActualTimeGlobalizationInfo.Culture)
                timePickerPopup.TimePickerParent.SetValue(TimePicker.CultureProperty, (object)newValue);
            timePickerPopup.OnCultureChanged(e.OldValue as CultureInfo, newValue);
        }

        /// <summary>Called when the culture changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnCultureChanged(CultureInfo oldValue, CultureInfo newValue)
        {
        }

        /// <summary>
        /// Gets the actual culture used by the control for formatting and parsing.
        /// </summary>
        /// <value>The actual culture.</value>
        public CultureInfo ActualCulture
        {
            get
            {
                return this.ActualTimeGlobalizationInfo.ActualCulture;
            }
        }

        /// <summary>
        /// Gets or sets the strategy object that determines how the control
        /// interacts with DateTime and CultureInfo.
        /// </summary>
        public TimeGlobalizationInfo TimeGlobalizationInfo
        {
            get
            {
                return (TimeGlobalizationInfo)this.GetValue(TimePickerPopup.TimeGlobalizationInfoProperty);
            }
            set
            {
                this.SetValue(TimePickerPopup.TimeGlobalizationInfoProperty, (object)value);
            }
        }

        /// <summary>
        /// TimeGlobalizationInfoProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeGlobalizationInfo.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeGlobalizationInfoPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            TimeGlobalizationInfo newValue = e.NewValue as TimeGlobalizationInfo;
            if (timePickerPopup.TimePickerParent != null && timePickerPopup.TimePickerParent.TimeGlobalizationInfo != newValue)
                timePickerPopup.TimePickerParent.SetValue(TimePicker.TimeGlobalizationInfoProperty, (object)newValue);
            if (newValue != null)
            {
                newValue.Culture = timePickerPopup.Culture;
                timePickerPopup._actualTimeGlobalizationInfo = (TimeGlobalizationInfo)null;
            }
            timePickerPopup.OnTimeGlobalizationInfoChanged(e.OldValue as TimeGlobalizationInfo, newValue);
        }

        /// <summary>Called when the time globalization info changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnTimeGlobalizationInfoChanged(
          TimeGlobalizationInfo oldValue,
          TimeGlobalizationInfo newValue)
        {
        }

        /// <summary>
        /// Gets the actual TimeGlobalizationInfo used by the control.
        /// </summary>
        /// <remarks>If TimeGlobalizationInfo is not set, will return
        /// default TimeGlobalizationInfo instance.</remarks>
        public TimeGlobalizationInfo ActualTimeGlobalizationInfo
        {
            get
            {
                TimeGlobalizationInfo globalizationInfo = this.TimeGlobalizationInfo;
                if (globalizationInfo != null)
                    return globalizationInfo;
                if (this._actualTimeGlobalizationInfo == null)
                    this._actualTimeGlobalizationInfo = new TimeGlobalizationInfo();
                return this._actualTimeGlobalizationInfo;
            }
        }

        /// <summary>
        /// Gets or sets the Format used by the control.
        /// From XAML Use either "Short", "Long" or a custom format.
        /// Custom formats can only contain "H", "h", "m", "s" or "t".
        /// For example: use 'hh:mm:ss' is used to format time as "13:45:30".
        /// </summary>
        [TypeConverter(typeof(TimeFormatConverter))]
        public ITimeFormat Format
        {
            get
            {
                return this.GetValue(TimePickerPopup.FormatProperty) as ITimeFormat;
            }
            set
            {
                this.SetValue(TimePickerPopup.FormatProperty, (object)value);
            }
        }

        /// <summary>FormatProperty property changed handler.</summary>
        /// <param name="d">TimePickerPopup that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            ITimeFormat newValue = e.NewValue as ITimeFormat;
            if (timePickerPopup.TimePickerParent != null && timePickerPopup.TimePickerParent.Format != newValue)
                timePickerPopup.TimePickerParent.SetValue(TimePicker.FormatProperty, (object)newValue);
            if (e.NewValue != null)
                timePickerPopup._actualFormat = (ITimeFormat)null;
            timePickerPopup.OnFormatChanged(e.OldValue as ITimeFormat, newValue);
        }

        /// <summary>Called when display format changed.</summary>
        /// <param name="oldValue">The old format.</param>
        /// <param name="newValue">The new format.</param>
        protected virtual void OnFormatChanged(ITimeFormat oldValue, ITimeFormat newValue)
        {
        }

        /// <summary>
        /// Gets the actual format that will be used to display Time
        /// in the TimePickerPopup. If no format is specified, ShortTimeFormat
        /// is used.
        /// </summary>
        /// <value>The actual display format.</value>
        public ITimeFormat ActualFormat
        {
            get
            {
                if (this.Format != null)
                    return this.Format;
                if (this._actualFormat == null)
                    this._actualFormat = (ITimeFormat)new ShortTimeFormat();
                return this._actualFormat;
            }
        }

        /// <summary>
        /// Gets or sets the interval of seconds that can be
        /// picked in a popup.
        /// </summary>
        public int PopupSecondsInterval
        {
            get
            {
                return (int)this.GetValue(TimePickerPopup.PopupSecondsIntervalProperty);
            }
            set
            {
                this.SetValue(TimePickerPopup.PopupSecondsIntervalProperty, (object)value);
            }
        }

        /// <summary>
        /// PopupSecondsIntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupSecondsInterval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupSecondsIntervalPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;
            if (newValue < 0 || newValue > 59)
            {
                timePickerPopup.SetValue(TimePickerPopup.PopupSecondsIntervalProperty, e.OldValue);
                throw new ArgumentOutOfRangeException(nameof(e), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid {0}", (object)newValue));
            }
            if (timePickerPopup.TimePickerParent != null && timePickerPopup.TimePickerParent.PopupSecondsInterval != newValue)
                timePickerPopup.TimePickerParent.SetValue(TimePicker.PopupSecondsIntervalProperty, (object)newValue);
            timePickerPopup.OnPopupSecondsIntervalChanged(oldValue, newValue);
        }

        /// <summary>Called when the popup seconds interval changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPopupSecondsIntervalChanged(int oldValue, int newValue)
        {
        }

        /// <summary>
        /// Gets or sets the interval of minutes that can be
        /// picked in a popup.
        /// </summary>
        public int PopupMinutesInterval
        {
            get
            {
                return (int)this.GetValue(TimePickerPopup.PopupMinutesIntervalProperty);
            }
            set
            {
                this.SetValue(TimePickerPopup.PopupMinutesIntervalProperty, (object)value);
            }
        }

        /// <summary>
        /// PopupMinutesIntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupMinutesInterval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupMinutesIntervalPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;
            if (newValue < 0 || newValue > 59)
            {
                timePickerPopup.SetValue(TimePickerPopup.PopupMinutesIntervalProperty, e.OldValue);
                throw new ArgumentOutOfRangeException(nameof(e), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid {0}", (object)newValue));
            }
            if (timePickerPopup.TimePickerParent != null && timePickerPopup.TimePickerParent.PopupMinutesInterval != newValue)
                timePickerPopup.TimePickerParent.SetValue(TimePicker.PopupMinutesIntervalProperty, (object)newValue);
            timePickerPopup.OnPopupMinutesIntervalChanged(oldValue, newValue);
        }

        /// <summary>Called when the popup minutes interval changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPopupMinutesIntervalChanged(int oldValue, int newValue)
        {
        }

        /// <summary>
        /// Gets or sets the whether the TimePickerPopup supports selecting
        /// designators and/or seconds.
        /// </summary>
        public PopupTimeSelectionMode PopupTimeSelectionMode
        {
            get
            {
                return (PopupTimeSelectionMode)this.GetValue(TimePickerPopup.PopupTimeSelectionModeProperty);
            }
            set
            {
                this.SetValue(TimePickerPopup.PopupTimeSelectionModeProperty, (object)value);
            }
        }

        /// <summary>
        /// PopupTimeSelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePickerPopup that changed its PopupTimeSelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupTimeSelectionModePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup timePickerPopup = (TimePickerPopup)d;
            PopupTimeSelectionMode newValue = (PopupTimeSelectionMode)e.NewValue;
            if (!((IEnumerable<PopupTimeSelectionMode>)timePickerPopup.GetValidPopupTimeSelectionModes()).Contains<PopupTimeSelectionMode>(newValue))
            {
                timePickerPopup.SetValue(TimePickerPopup.PopupTimeSelectionModeProperty, e.OldValue);
                throw new ArgumentOutOfRangeException(nameof(e), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid {0}", (object)newValue));
            }
            if (timePickerPopup.TimePickerParent != null && timePickerPopup.TimePickerParent.PopupTimeSelectionMode != newValue)
                timePickerPopup.TimePickerParent.SetValue(TimePicker.PopupTimeSelectionModeProperty, (object)newValue);
            timePickerPopup.OnPopupTimeSelectionModeChanged((PopupTimeSelectionMode)e.OldValue, (PopupTimeSelectionMode)e.NewValue);
            timePickerPopup.UpdateVisualState(true);
        }

        /// <summary>Called when the time selection mode is changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPopupTimeSelectionModeChanged(
          PopupTimeSelectionMode oldValue,
          PopupTimeSelectionMode newValue)
        {
        }

        /// <summary>Occurs when Value property is changing.</summary>
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;

        /// <summary>Occurs when Value property has changed.</summary>
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;

        /// <summary>Occurs when a selected item is committed.</summary>
        public event RoutedEventHandler Commit;

        /// <summary>Occurs when a selection has been canceled.</summary>
        public event RoutedEventHandler Cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.TimePickerPopup" /> class.
        /// </summary>
        protected TimePickerPopup()
        {
            this._timeCoercionHelper = new TimeCoercionHelper((ITimeInput)this);
            this._interaction = new InteractionHelper((Control)this);
        }

        /// <summary>
        /// Builds the visual tree for the TimePickerPopup control when a new
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateVisualState(false);
        }

        /// <summary>Raises the Cancel event.</summary>
        protected void DoCancel()
        {
            RoutedEventHandler cancel = this.Cancel;
            if (cancel == null)
                return;
            cancel((object)this, new RoutedEventArgs());
        }

        /// <summary>Raises the Commit event.</summary>
        protected void DoCommit()
        {
            RoutedEventHandler commit = this.Commit;
            if (commit == null)
                return;
            commit((object)this, new RoutedEventArgs());
        }

        /// <summary>
        /// Called when the TimePicker control has opened this popup.
        /// </summary>
        /// <remarks>Called before the TimePicker reacts to value changes.
        /// This is done so that the Popup can 'snap' to a specific value without
        /// changing the selected value in the TimePicker.</remarks>
        public virtual void OnOpened()
        {
        }

        /// <summary>
        /// Called when the TimePicker control has closed this popup.
        /// </summary>
        public virtual void OnClosed()
        {
        }

        /// <summary>
        /// Raises the ValueChanging event when Value property is changing.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected virtual void OnValueChanging(RoutedPropertyChangingEventArgs<DateTime?> e)
        {
            if (this._timeCoercionHelper.CoerceValue(e.OldValue, e.NewValue))
            {
                e.InCoercion = false;
                e.NewValue = this.Value;
                RoutedPropertyChangingEventHandler<DateTime?> valueChanging = this.ValueChanging;
                if (valueChanging == null)
                    return;
                valueChanging((object)this, e);
            }
            else
                e.InCoercion = true;
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            RoutedPropertyChangedEventHandler<DateTime?> valueChanged = this.ValueChanged;
            if (valueChanged == null)
                return;
            valueChanged((object)this, e);
        }

        DateTime? ITimeInput.Value
        {
            get
            {
                return this.Value;
            }
            set
            {
                this.Value = value;
            }
        }

        DateTime? ITimeInput.Minimum
        {
            get
            {
                return this.Minimum;
            }
            set
            {
                this.Minimum = value;
            }
        }

        DateTime? ITimeInput.Maximum
        {
            get
            {
                return this.Maximum;
            }
            set
            {
                this.Maximum = value;
            }
        }

        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            this.UpdateVisualState(useTransitions);
        }

        /// <summary>Update the current visual states.</summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState((Control)this, this.TimePickerParent == null ? "NotContained" : "Contained", useTransitions);
            VisualStateManager.GoToState((Control)this, this.PopupTimeSelectionMode.ToString(), useTransitions);
            this._interaction.UpdateVisualStateBase(useTransitions);
        }

        /// <summary>Gets the valid popup time selection modes.</summary>
        /// <returns>An array of PopupTimeSelectionModes that are supported by
        /// the Popup.</returns>
        internal abstract PopupTimeSelectionMode[] GetValidPopupTimeSelectionModes();
    }
}
