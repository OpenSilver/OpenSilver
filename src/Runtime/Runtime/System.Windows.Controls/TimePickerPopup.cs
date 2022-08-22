// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.ComponentModel;
using System.Linq;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;
using System.Windows.Automation;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation;
#endif

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
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = AllowSecondsAndDesignatorsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = AllowTimeDesignatorsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = AllowSecondsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = HoursAndMinutesOnlyStateName, GroupName = PopupModeGroupName)]

    [TemplateVisualState(Name = ContainedStateName, GroupName = ContainedByPickerGroupName)]
    [TemplateVisualState(Name = NotContainedStateName, GroupName = ContainedByPickerGroupName)]
    public abstract class TimePickerPopup : Control, ITimeInput, IUpdateVisualState
    {
#region Visual state names
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

        /// <summary>
        /// The name of the ContainedByPicker state group.
        /// </summary>
        internal const string ContainedByPickerGroupName = "ContainedByPickerStates";

        /// <summary>
        /// The name of the PopupMode state group.
        /// </summary>
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

#endregion Visual state names

        /// <summary>
        /// Helper class that centralizes the coercion logic across all 
        /// TimeInput controls.
        /// </summary>
        private readonly TimeCoercionHelper _timeCoercionHelper;

        /// <summary>
        /// Gets or sets the container for this Popup.
        /// </summary>
        /// <value>The TimePicker that contains this Popup.</value>
        internal TimePicker TimePickerParent
        {
            get { return _timePickerParent; }
            set
            {
                _timePickerParent = value;
                UpdateVisualState(true);
            }
        }

        /// <summary>
        /// BackingField of TimePickerParent.
        /// </summary>
        private TimePicker _timePickerParent;

        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality.
        /// </summary>
        private InteractionHelper _interaction;

#region public DateTime? Value
        /// <summary>
        /// Gets or sets the currently selected time.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Overwriting from base, so TypeConverter can be added.")]
        [TypeConverter(typeof(TimeTypeConverter))]
        public virtual DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(DateTime?),
                typeof(TimePickerPopup),
                new PropertyMetadata(default(DateTime?), OnValuePropertyChanged));

        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignoreValueChange;

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary>
        /// <param name="d">UpDownBase whose Value changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;

            // Ignore the change if requested
            if (source._ignoreValueChange)
            {
                // we do want to raise the event, but won't react to anything.
                source.OnValueChanged(new RoutedPropertyChangedEventArgs<DateTime?>(e.OldValue as DateTime?, e.NewValue as DateTime?));
                return;
            }

            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            // simulate pre and post events
            // The Value has already been changed when this function is called. 
            // So if user's chaning event handler check Value, it will be the changed value.
            // This is confusing, because we are simulating pre event on the platform that doesn't natively support it.
            RoutedPropertyChangingEventArgs<DateTime?> changingArgs = new RoutedPropertyChangingEventArgs<DateTime?>(e.Property, oldValue, newValue, true);
            source.OnValueChanging(changingArgs);

            // workaround the class hierarchy for value coercion
            if (changingArgs.InCoercion)
            {
            }
            else if (!changingArgs.Cancel)
            {
                newValue = changingArgs.NewValue;
                RoutedPropertyChangedEventArgs<DateTime?> changedArgs = new RoutedPropertyChangedEventArgs<DateTime?>(oldValue, newValue);
                source.OnValueChanged(changedArgs);

                TimePickerPopupAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(source) as TimePickerPopupAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseValueAutomationEvent(oldValue, newValue);
                }
            }
            else
            {
                // revert back to old value if an event handler canceled the changing event.
                source._ignoreValueChange = true;
                source.Value = oldValue;
                source._ignoreValueChange = false;
            }
        }
#endregion public T Value

#region public DateTime? Minimum
        /// <summary>
        /// Gets or sets the Minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Minimum property will be used to prevent users 
        /// from choosing values out of range in the TimePickerPopup.</remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum
        {
            get { return (DateTime?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(DateTime?),
                typeof(TimePickerPopup),
                new PropertyMetadata(null, OnMinimumPropertyChanged));

        /// <summary>
        /// MinimumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            // potentially flow back to parent.
            if (source.TimePickerParent != null && source.TimePickerParent.Minimum != newValue)
            {
                source.TimePickerParent.SetValue(TimePicker.MinimumProperty, newValue);
            }
            source._ignoreValueChange = true;
            source._timeCoercionHelper.ProcessMinimumChange(newValue);
            source._ignoreValueChange = false;
            source.OnMinimumChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the Minimum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
#endregion public DateTime? Minimum

#region public DateTime? Maximum
        /// <summary>
        /// Gets or sets the Maximum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Maximum property will be used to prevent users 
        /// from choosing values out of range in the TimePickerPopup.</remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum
        {
            get { return (DateTime?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(DateTime?),
                typeof(TimePickerPopup),
                new PropertyMetadata(null, OnMaximumPropertyChanged));

        /// <summary>
        /// MaximumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            // potentially flow back to parent.
            if (source.TimePickerParent != null && source.TimePickerParent.Maximum != newValue)
            {
                source.TimePickerParent.SetValue(TimePicker.MaximumProperty, newValue);
            }
            source._ignoreValueChange = true;
            source._timeCoercionHelper.ProcessMaximumChange(newValue);
            source._ignoreValueChange = false;
            source.OnMaximumChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the Maximum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
#endregion public DateTime? Maximum

#region public CultureInfo Culture
        /// <summary>
        /// Gets or sets the culture that will be used by the control for 
        /// time formatting.
        /// </summary>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Identifies the Culture dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register(
                "Culture",
                typeof(CultureInfo),
                typeof(TimePickerPopup),
                new PropertyMetadata(null, OnCulturePropertyChanged));

        /// <summary>
        /// CultureProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Culture.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;
            CultureInfo newValue = e.NewValue as CultureInfo;
            source.ActualTimeGlobalizationInfo.Culture = newValue;

            // potentially flow back to parent.
            if (source.TimePickerParent != null && source.TimePickerParent.Culture != source.ActualTimeGlobalizationInfo.Culture)
            {
                source.TimePickerParent.SetValue(TimePicker.CultureProperty, newValue);
            }

            source.OnCultureChanged(e.OldValue as CultureInfo, newValue);
        }

        /// <summary>
        /// Called when the culture changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnCultureChanged(CultureInfo oldValue, CultureInfo newValue)
        {
        }
#endregion public CultureInfo Culture

        /// <summary>
        /// Gets the actual culture used by the control for formatting and parsing.
        /// </summary>
        /// <value>The actual culture.</value>
        public CultureInfo ActualCulture
        {
            get
            {
                return ActualTimeGlobalizationInfo.ActualCulture;
            }
        }

#region public TimeGlobalizationInfo TimeGlobalizationInfo
        /// <summary>
        /// Gets or sets the strategy object that determines how the control 
        /// interacts with DateTime and CultureInfo.
        /// </summary>
        public TimeGlobalizationInfo TimeGlobalizationInfo
        {
            get { return (TimeGlobalizationInfo)GetValue(TimeGlobalizationInfoProperty); }
            set { SetValue(TimeGlobalizationInfoProperty, value); }
        }

        /// <summary>
        /// Identifies the TimeGlobalizationInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeGlobalizationInfoProperty =
            DependencyProperty.Register(
                "TimeGlobalizationInfo",
                typeof(TimeGlobalizationInfo),
                typeof(TimePickerPopup),
                new PropertyMetadata(OnTimeGlobalizationInfoPropertyChanged));

        /// <summary>
        /// TimeGlobalizationInfoProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeGlobalizationInfo.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeGlobalizationInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;
            TimeGlobalizationInfo newValue = e.NewValue as TimeGlobalizationInfo;

            // potentially flow back to parent.
            if (source.TimePickerParent != null && source.TimePickerParent.TimeGlobalizationInfo != newValue)
            {
                source.TimePickerParent.SetValue(TimePicker.TimeGlobalizationInfoProperty, newValue);
            }

            if (newValue != null)
            {
                newValue.Culture = source.Culture;
                source._actualTimeGlobalizationInfo = null; // no need for default any more.
            }

            source.OnTimeGlobalizationInfoChanged(e.OldValue as TimeGlobalizationInfo, newValue);
        }

        /// <summary>
        /// Called when the time globalization info changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnTimeGlobalizationInfoChanged(TimeGlobalizationInfo oldValue, TimeGlobalizationInfo newValue)
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
                TimeGlobalizationInfo info = TimeGlobalizationInfo;

                if (info == null)
                {
                    if (_actualTimeGlobalizationInfo == null)
                    {
                        // set the default strategy object
                        _actualTimeGlobalizationInfo = new TimeGlobalizationInfo();
                    }
                    return _actualTimeGlobalizationInfo;
                }
                else
                {
                    return info;
                }
            }
        }

        /// <summary>
        /// BackingField for ActualTimeGlobalizationInfo.
        /// </summary>
        private TimeGlobalizationInfo _actualTimeGlobalizationInfo;
#endregion public TimeGlobalizationInfo TimeGlobalizationInfo

#region public ITimeFormat Format
        /// <summary>
        /// Gets or sets the Format used by the control. 
        /// From XAML Use either "Short", "Long" or a custom format. 
        /// Custom formats can only contain "H", "h", "m", "s" or "t". 
        /// For example: use 'hh:mm:ss' is used to format time as "13:45:30".
        /// </summary>
        [TypeConverter(typeof(TimeFormatConverter))]
        public ITimeFormat Format
        {
            get { return GetValue(FormatProperty) as ITimeFormat; }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// Identifies the Format dependency property.
        /// </summary>
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

            // potentially flow back to parent.
            if (source.TimePickerParent != null && source.TimePickerParent.Format != value)
            {
                source.TimePickerParent.SetValue(TimePicker.FormatProperty, value);
            }

            if (e.NewValue != null)
            {
                // no need for cache any more.
                source._actualFormat = null;
            }

            source.OnFormatChanged(e.OldValue as ITimeFormat, value);
        }

        /// <summary>
        /// Called when display format changed.
        /// </summary>
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
                if (Format == null)
                {
                    if (_actualFormat == null)
                    {
                        _actualFormat = new ShortTimeFormat();
                    }
                    return _actualFormat;
                }
                else
                {
                    return Format;
                }
            }
        }

        /// <summary>
        /// BackingField for ActualFormat.
        /// </summary>
        private ITimeFormat _actualFormat;
#endregion public ITimeFormat Format

#region public int PopupSecondsInterval
        /// <summary>
        /// Gets or sets the interval of seconds that can be
        /// picked in a popup.
        /// </summary>
        public int PopupSecondsInterval
        {
            get { return (int)GetValue(PopupSecondsIntervalProperty); }
            set { SetValue(PopupSecondsIntervalProperty, value); }
        }

        /// <summary>
        /// Identifies the PopupSecondsInterval dependency property.
        /// </summary>
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

            if (newValue < 0 || newValue > 59)
            {
                // revert to old value
                source.SetValue(PopupSecondsIntervalProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid PopupSecondsInterval '{0}'.The interval can be set to 0 (no interval) to and including 59.",
                    newValue);

                throw new ArgumentOutOfRangeException("e", message);
            }

            // potentially flow back to parent.
            if (source.TimePickerParent != null && source.TimePickerParent.PopupSecondsInterval != newValue)
            {
                source.TimePickerParent.SetValue(TimePicker.PopupSecondsIntervalProperty, newValue);
            }
            source.OnPopupSecondsIntervalChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the popup seconds interval changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPopupSecondsIntervalChanged(int oldValue, int newValue)
        {
        }
#endregion public int PopupSecondsInterval

#region public int PopupMinutesInterval
        /// <summary>
        /// Gets or sets the interval of minutes that can be
        /// picked in a popup.
        /// </summary>
        public int PopupMinutesInterval
        {
            get { return (int)GetValue(PopupMinutesIntervalProperty); }
            set { SetValue(PopupMinutesIntervalProperty, value); }
        }

        /// <summary>
        /// Identifies the PopupMinutesInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupMinutesIntervalProperty =
            DependencyProperty.Register(
                "PopupMinutesInterval",
                typeof(int),
                typeof(TimePickerPopup),
                new PropertyMetadata(OnPopupMinutesIntervalPropertyChanged));

        /// <summary>
        /// PopupMinutesIntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupMinutesInterval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupMinutesIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;

            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;

            if (newValue < 0 || newValue > 59)
            {
                // revert to old value
                source.SetValue(PopupMinutesIntervalProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid PopupMinutesInterval '{0}'. The interval can be set to 0 (no interval) to and including 59.",
                    newValue);

                throw new ArgumentOutOfRangeException("e", message);
            }

            // potentially flow back to parent.
            if (source.TimePickerParent != null && source.TimePickerParent.PopupMinutesInterval != newValue)
            {
                source.TimePickerParent.SetValue(TimePicker.PopupMinutesIntervalProperty, newValue);
            }

            source.OnPopupMinutesIntervalChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the popup minutes interval changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPopupMinutesIntervalChanged(int oldValue, int newValue)
        {
        }
#endregion public int PopupMinutesInterval

#region public PopupTimeSelectionMode PopupTimeSelectionMode
        /// <summary>
        /// Gets or sets the whether the TimePickerPopup supports selecting 
        /// designators and/or seconds.
        /// </summary>
        public PopupTimeSelectionMode PopupTimeSelectionMode
        {
            get { return (PopupTimeSelectionMode)GetValue(PopupTimeSelectionModeProperty); }
            set { SetValue(PopupTimeSelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the PopupTimeSelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupTimeSelectionModeProperty =
            DependencyProperty.Register(
                "PopupTimeSelectionMode",
                typeof(PopupTimeSelectionMode),
                typeof(TimePickerPopup),
                new PropertyMetadata(PopupTimeSelectionMode.HoursAndMinutesOnly, OnPopupTimeSelectionModePropertyChanged));

        /// <summary>
        /// PopupTimeSelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePickerPopup that changed its PopupTimeSelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupTimeSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePickerPopup source = (TimePickerPopup)d;

            PopupTimeSelectionMode value = (PopupTimeSelectionMode)e.NewValue;

            if (!source.GetValidPopupTimeSelectionModes().Contains(value))
            {
                // revert to old value
                source.SetValue(PopupTimeSelectionModeProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid PopupTimeSelectionMode for this popup, value '{0}'.",
                    value);

                throw new ArgumentOutOfRangeException("e", message);
            }

            // potentially flow back to parent.
            if (source.TimePickerParent != null && source.TimePickerParent.PopupTimeSelectionMode != value)
            {
                source.TimePickerParent.SetValue(TimePicker.PopupTimeSelectionModeProperty, value);
            }

            source.OnPopupTimeSelectionModeChanged((PopupTimeSelectionMode)e.OldValue, (PopupTimeSelectionMode)e.NewValue);
            source.UpdateVisualState(true);
        }

        /// <summary>
        /// Called when the time selection mode is changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPopupTimeSelectionModeChanged(PopupTimeSelectionMode oldValue, PopupTimeSelectionMode newValue)
        {
        }
#endregion public PopupTimeSelectionMode PopupTimeSelectionMode

#region events
        /// <summary>
        /// Occurs when Value property is changing.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;

        /// <summary>
        /// Occurs when Value property has changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;

        /// <summary>
        /// Occurs when a selected item is committed.
        /// </summary>
        public event RoutedEventHandler Commit;

        /// <summary>
        /// Occurs when a selection has been canceled.
        /// </summary>
        public event RoutedEventHandler Cancel;
#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePickerPopup"/> class.
        /// </summary>
        protected TimePickerPopup()
        {
            _timeCoercionHelper = new TimeCoercionHelper(this);
            _interaction = new InteractionHelper(this);
        }

        /// <summary>
        /// Builds the visual tree for the TimePickerPopup control when a new 
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            UpdateVisualState(false);
        }

        /// <summary>
        /// Raises the Cancel event.
        /// </summary>
        protected void DoCancel()
        {
            RoutedEventHandler handler = Cancel;
            if (handler != null)
            {
                handler(this, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Raises the Commit event.
        /// </summary>
        protected void DoCommit()
        {
            RoutedEventHandler handler = Commit;
            if (handler != null)
            {
                handler(this, new RoutedEventArgs());
            }
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
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Forced by UpDown.")]
        protected virtual void OnValueChanging(RoutedPropertyChangingEventArgs<DateTime?> e)
        {
            // change is from value itself.
            bool success = _timeCoercionHelper.CoerceValue(e.OldValue, e.NewValue);

            if (success)
            {
                e.InCoercion = false;
                e.NewValue = Value;
                RoutedPropertyChangingEventHandler<DateTime?> handler = ValueChanging;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            else
            {
                e.InCoercion = true;
            }
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Forced by UpDown.")]
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            RoutedPropertyChangedEventHandler<DateTime?> handler = ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

#region ITimeInput Members
        /// <summary>
        /// Gets or sets the Value property.
        /// </summary>
        /// <value></value>
        DateTime? ITimeInput.Value
        {
            get { return Value; }
            set { Value = value; }
        }

        /// <summary>
        /// Gets or sets the minimum time.
        /// </summary>
        /// <value>The minimum time.</value>
        DateTime? ITimeInput.Minimum
        {
            get { return Minimum; }
            set { Minimum = value; }
        }

        /// <summary>
        /// Gets or sets the maximum time.
        /// </summary>
        /// <value>The maximum time.</value>
        DateTime? ITimeInput.Maximum
        {
            get { return Maximum; }
            set { Maximum = value; }
        }
#endregion

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Update the current visual states.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // Contained state
            VisualStateManager.GoToState(this, TimePickerParent == null ? NotContainedStateName : ContainedStateName, useTransitions);

            // Popup mode, visual states directly match enum names.
            VisualStateManager.GoToState(this, PopupTimeSelectionMode.ToString(), useTransitions);

            // Handle the Common and Focused states
            _interaction.UpdateVisualStateBase(useTransitions);
        }

        /// <summary>
        /// Gets the valid popup time selection modes.
        /// </summary>
        /// <returns>An array of PopupTimeSelectionModes that are supported by
        /// the Popup.</returns>
        internal abstract PopupTimeSelectionMode[] GetValidPopupTimeSelectionModes();

#region automation
        /// <summary>
        /// Returns an AutomationPeer for use by the Silverlight 
        /// automation infrastructure.
        /// </summary>
        /// <returns>An AutomationPeer for the Popup object.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return CreateAutomationPeer();
        }

        /// <summary>
        /// Creates the automation peer.
        /// </summary>
        /// <returns>An AutomationPeer for this instance.</returns>
        protected abstract TimePickerPopupAutomationPeer CreateAutomationPeer();
#endregion
    }
}