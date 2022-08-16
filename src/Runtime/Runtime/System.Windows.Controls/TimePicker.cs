// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;
using System.Windows.Automation;
#else
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
    /// Represents a control that allows the user to select a time.
    /// </summary>
    /// <remarks>TimeInput supports only the following formatting characters:
    /// 'h', 'm', 's', 'H', 't'. All other characters are filtered out:
    /// 'd', 'f', 'F', 'g', 'K', 'M', 'y', 'z'.</remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StatePopupClosed, GroupName = VisualStates.GroupPopup)]
    [TemplateVisualState(Name = VisualStates.StatePopupOpened, GroupName = VisualStates.GroupPopup)]

    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplatePart(Name = ElementTimeUpDownName, Type = typeof(TimeUpDown))]
    [TemplatePart(Name = ElementPopupName, Type = typeof(Popup))]
    [TemplatePart(Name = ElementDropDownToggleName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementPopupPlaceHolderPartName, Type = typeof(ContentControl))]
    [StyleTypedProperty(Property = TimeUpDownStyleName, StyleTargetType = typeof(TimeUpDown))]
    [StyleTypedProperty(Property = UpDownBase.SpinnerStyleName, StyleTargetType = typeof(ButtonSpinner))]
    public class TimePicker : Picker, ITimeInput
    {
#region Name constants
        /// <summary>
        /// The name for the TimeUpDown element.
        /// </summary>
        private const string ElementTimeUpDownName = "TimeUpDown";

        /// <summary>
        /// The name for the TimeUpDownStyle element.
        /// </summary>
        private const string TimeUpDownStyleName = "TimeUpDownStyle";

        /// <summary>
        /// The name for the PopupPlaceHolder element.
        /// </summary>
        private const string ElementPopupPlaceHolderPartName = "PopupPlaceHolder";
#endregion Name constants

#region TemplateParts
        /// <summary>
        /// Gets or sets the time up down part.
        /// </summary>
        /// <value>The time up down part.</value>
        private TimeUpDown TimeUpDownPart
        {
            get { return _timeUpDownPart; }
            set
            {
                if (_timeUpDownPart != null)
                {
                    _timeUpDownPart.ValueChanged -= TimeUpDownValueChanged;
                    _timeUpDownPart.Parsing -= TimeUpDownParsing;
                    _timeUpDownPart.ParseError -= TimeUpDownParseError;
                }
                _timeUpDownPart = value;
                if (_timeUpDownPart != null)
                {
                    _timeUpDownPart.ValueChanged += TimeUpDownValueChanged;
                    _timeUpDownPart.Parsing += TimeUpDownParsing;
                    _timeUpDownPart.ParseError += TimeUpDownParseError;

                    PropagateNewValue();
                }
            }
        }

        /// <summary>
        /// BackingField for TimeUpDownPart.
        /// </summary>
        private TimeUpDown _timeUpDownPart;

        /// <summary>
        /// Gets or sets the popup place holder part.
        /// </summary>
        /// <remarks>This is the ContentControl that is used to display
        /// Popups.</remarks>
        private ContentControl PopupPlaceHolderPart
        {
            get { return _popupPlaceHolderPart; }
            set
            {
                if (_popupPlaceHolderPart != null)
                {
                    _popupPlaceHolderPart.Content = null;
                    _popupPlaceHolderPart.ContentTemplate = null;
                }

                _popupPlaceHolderPart = value;

                if (_popupPlaceHolderPart != null)
                {
                    if (ActualTimePickerPopup != null)
                    {
                        _popupPlaceHolderPart.Content = ActualTimePickerPopup;
                    }
                }
            }
        }

        /// <summary>
        /// BackingField for PopupPlaceHolderPart.
        /// </summary>
        private ContentControl _popupPlaceHolderPart;
#endregion TemplateParts

        /// <summary>
        /// Helper class that centralizes the coercion logic across all 
        /// TimeInput controls.
        /// </summary>
        private readonly TimeCoercionHelper _timeCoercionHelper;

        /// <summary>
        /// Cache of the value before we open a popup.
        /// </summary>
        private DateTime? _popupSessionValueCache;

        /// <summary>
        /// Indicates that the control has finished initialization.
        /// </summary>
        private bool _isInitialized;

#region public DateTime? Value
        /// <summary>
        /// Gets or sets the currently selected time.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Property is named Value in UpDown hierarchy.")]
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
                typeof(TimePicker),
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
            TimePicker source = (TimePicker)d;

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

            // hack: work around the class hierarchy for value coercion in NumericUpDown
            if (changingArgs.InCoercion)
            {
            }
            else if (!changingArgs.Cancel)
            {
                newValue = changingArgs.NewValue;
                RoutedPropertyChangedEventArgs<DateTime?> changedArgs = new RoutedPropertyChangedEventArgs<DateTime?>(oldValue, newValue);
                source.OnValueChanged(changedArgs);

                source.PropagateNewValue();

                TimePickerAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(source) as TimePickerAutomationPeer;
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
        /// Gets or sets the minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the minimum property is applicable for the following 
        /// features: Selecting a value through a popup, Parsing a new value from 
        /// the textbox, spinning a new value and programmatically specifying a value.</remarks>
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
                typeof(TimePicker),
                new PropertyMetadata(null, OnMinimumPropertyChanged));

        /// <summary>
        /// MinimumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            source._ignoreValueChange = true;
            source._timeCoercionHelper.ProcessMinimumChange(newValue);
            source._ignoreValueChange = false;

            source.OnMinimumChanged(oldValue, newValue);
            if (source.ActualTimePickerPopup != null)
            {
                source.ActualTimePickerPopup.Minimum = newValue;
            }
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
        /// Gets or sets the maximum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Maximum property is applicable for the following 
        /// features: Selecting a value through a popup, Parsing a new value 
        /// from the textbox, spinning a new value and programmatically specifying 
        /// a value. </remarks>
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
                typeof(TimePicker),
                new PropertyMetadata(null, OnMaximumPropertyChanged));

        /// <summary>
        /// MaximumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            source._ignoreValueChange = true;
            source._timeCoercionHelper.ProcessMaximumChange(newValue);
            source._ignoreValueChange = false;

            source.OnMaximumChanged(oldValue, newValue);
            if (source.ActualTimePickerPopup != null)
            {
                source.ActualTimePickerPopup.Maximum = newValue;
            }
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

#region public Style TimeUpDownStyle
        /// <summary>
        /// Gets or sets the Style applied to the TimeUpDown portion of the TimePicker 
        /// control.
        /// </summary>
        public Style TimeUpDownStyle
        {
            get { return GetValue(TimeUpDownStyleProperty) as Style; }
            set { SetValue(TimeUpDownStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TimeUpDownStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeUpDownStyleProperty =
            DependencyProperty.Register(
                "TimeUpDownStyle",
                typeof(Style),
                typeof(TimePicker),
                new PropertyMetadata(OnTimeUpDownStylePropertyChanged));

        /// <summary>
        /// TimeUpDownStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its TimeUpDownStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeUpDownStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
#endregion public Style TimeUpDownStyle

#region public Style SpinnerStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the spinner.
        /// </summary>
        public Style SpinnerStyle
        {
            get { return GetValue(SpinnerStyleProperty) as Style; }
            set { SetValue(SpinnerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the SpinnerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty SpinnerStyleProperty =
            DependencyProperty.Register(
                "SpinnerStyle",
                typeof(Style),
                typeof(TimePicker),
                new PropertyMetadata(OnSpinnerStylePropertyChanged));

        /// <summary>
        /// SpinnerStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its SpinnerStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSpinnerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
#endregion public Style SpinnerStyle

#region public TimeParserCollection TimeParsers
        /// <summary>
        /// Gets or sets a collection of TimeParsers that are used when parsing 
        /// text to time.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Can be set from xaml.")]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Can be set from xaml.")]
        public TimeParserCollection TimeParsers
        {
            get { return GetValue(TimeParsersProperty) as TimeParserCollection; }
            set { SetValue(TimeParsersProperty, value); }
        }

        /// <summary>
        /// Identifies the TimeParsers dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeParsersProperty =
            DependencyProperty.Register(
                "TimeParsers",
                typeof(TimeParserCollection),
                typeof(TimePicker),
                new PropertyMetadata(OnTimeParsersPropertyChanged));

        /// <summary>
        /// TimeParsersProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeParsers.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeParsersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the actual TimeParsers that will be used for parsing by the control.
        /// </summary>
        /// <remarks>Includes the TimeParsers introduced in the TimeGlobalizationInfo.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Can be set from xaml.")]
        public TimeParserCollection ActualTimeParsers
        {
            get
            {
                return new TimeParserCollection(ActualTimeGlobalizationInfo.GetActualTimeParsers(TimeParsers == null ? null : TimeParsers.ToList()));
            }
        }
#endregion public TimeParserCollection TimeParsers

#region public ITimeFormat Format
        /// <summary>
        /// Gets or sets the Format used by the control. From XAML Use either 
        /// "Short", "Long" or a custom format. 
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
                typeof(TimePicker),
                new PropertyMetadata(OnFormatPropertyChanged));

        /// <summary>
        /// FormatProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            ITimeFormat newValue = e.NewValue as ITimeFormat;

            if (e.NewValue != null)
            {
                // no need for cache any more.
                source._actualFormat = null;
            }

            if (source.ActualTimePickerPopup != null)
            {
                source.ActualTimePickerPopup.Format = newValue;
            }
        }

        /// <summary>
        /// Gets the actual format that will be used to display Time in the 
        /// TimePicker. If no format is specified, ShortTimeFormat is used.
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

#region public CultureInfo Culture
        /// <summary>
        /// Gets or sets the culture that will be used by the control for 
        /// parsing and formatting.
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
                typeof(TimePicker),
                new PropertyMetadata(null, OnCulturePropertyChanged));

        /// <summary>
        /// CultureProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Culture.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            source.ActualTimeGlobalizationInfo.Culture = e.NewValue as CultureInfo;

            if (source.ActualTimePickerPopup != null)
            {
                source.ActualTimePickerPopup.Culture = e.NewValue as CultureInfo;
            }
        }

        /// <summary>
        /// Gets the actual culture used by the control for formatting and parsing.
        /// </summary>
        public CultureInfo ActualCulture
        {
            get
            {
                return ActualTimeGlobalizationInfo.ActualCulture;
            }
        }
#endregion public CultureInfo Culture

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
                typeof(TimePicker),
                new PropertyMetadata(OnTimeGlobalizationInfoPropertyChanged));

        /// <summary>
        /// TimeGlobalizationInfoProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeGlobalizationInfo.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeGlobalizationInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            TimeGlobalizationInfo newValue = e.NewValue as TimeGlobalizationInfo;
            if (newValue != null)
            {
                newValue.Culture = source.Culture;
                source._actualTimeGlobalizationInfo = null; // no need for default any more.
            }

            if (source.ActualTimePickerPopup != null)
            {
                source.ActualTimePickerPopup.TimeGlobalizationInfo = newValue;
            }
        }

        /// <summary>
        /// Gets the actual TimeGlobalization info used by the control.
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

        /// <summary>
        /// Gets the TimePickerPopup that is used by the TimePicker. This
        /// can be either a popup created from the PopupTemplate, or a popup
        /// set directly to the Popup property. When both are set, the Popup
        /// property will win.
        /// </summary>
        protected virtual TimePickerPopup ActualTimePickerPopup
        {
            get
            {
                if (Popup != null)
                {
                    return Popup;
                }
                else
                {
                    return _instantiatedPopupFromTemplate;
                }
            }
        }

#region public TimePickerPopup Popup
        /// <summary>
        /// Gets or sets the TimePickerPopup that will be shown to the user by the 
        /// TimePicker control. This property may not be styled. To style a 
        /// TimePicker with a Popup, please use the PopupTemplate property.
        /// When both PopupTemplate and Popup are set, Popup will be used.
        /// </summary>
        /// <remark>This property might be null, since a template can be used.</remark>
        public TimePickerPopup Popup
        {
            get { return GetValue(PopupProperty) as TimePickerPopup; }
            set { SetValue(PopupProperty, value); }
        }

        /// <summary>
        /// Identifies the Popup dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.Register(
                "Popup",
                typeof(TimePickerPopup),
                typeof(TimePicker),
                new PropertyMetadata(OnPopupPropertyChanged));

        /// <summary>
        /// PopupProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its Popup.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;

            // a style may never set the popup property.
            if (source.Style != null)
            {
                if (source.Style.Setters.Where(setterbase =>
                                                   {
                                                       Setter setter = setterbase as Setter;
                                                       return (setter != null && setter.Property == PopupProperty);
                                                   })
                                         .Count() > 0)
                {
                    throw new ArgumentException("Cannot set the PopupProperty in a style. Please use PopupTemplate.");
                }
            }

            TimePickerPopup oldValue = e.OldValue as TimePickerPopup;
            TimePickerPopup newValue = e.NewValue as TimePickerPopup;

            if (source.PopupPlaceHolderPart != null)
            {
                source.PopupPlaceHolderPart.ContentTemplate = null;
            }

            // release references to oldPopup 
            source.UnregisterPopup(oldValue);

            // register newPopup
            source.RegisterPopup(newValue);

            if (newValue == null)
            {
                // possibly re-use a template.
                // will register as well
                source.InstantiatePopupFromTemplate();
            }
            else
            {
                // clear out any instantiated popup from a template.
                source._instantiatedPopupFromTemplate = null;
            }
        }
#endregion public TimePickerPopup Popup

#region public TimePickerPopupTemplate PopupTemplate
        /// <summary>
        /// Gets or sets the template used as Popup. A Popup can also be set
        /// directly on the Popup property. When both PopupTemplate and Popup
        /// are set, Popup will be used. 
        /// </summary>
        public TimePickerPopupTemplate PopupTemplate
        {
            get { return GetValue(PopupTemplateProperty) as TimePickerPopupTemplate; }
            set { SetValue(PopupTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the PopupTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupTemplateProperty =
            DependencyProperty.Register(
                "PopupTemplate",
                typeof(TimePickerPopupTemplate),
                typeof(TimePicker),
                new PropertyMetadata(null, OnPopupTemplatePropertyChanged));

        /// <summary>
        /// PopupTemplateProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupTemplate.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            source.InstantiatePopupFromTemplate();
        }

        /// <summary>
        /// BackingField for InstantiatedPopupFromTemplate.
        /// </summary>
        private TimePickerPopup _instantiatedPopupFromTemplate;
#endregion public TimePickerPopupTemplate PopupTemplate

#region public int PopupSecondsInterval
        /// <summary>
        /// Gets or sets the seconds interval between time values allowed by 
        /// the TimePickerPopup.
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
                typeof(TimePicker),
                new PropertyMetadata(OnPopupSecondsIntervalPropertyChanged));

        /// <summary>
        /// PopupSecondsIntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupSecondsInterval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupSecondsIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            int value = (int)e.NewValue;

            if (value < 0 || value > 59)
            {
                // revert to old value
                source.SetValue(PopupSecondsIntervalProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid PopupSecondsInterval '{0}'.The interval can be set to 0 (no interval) to and including 59.",
                    value);

                throw new ArgumentOutOfRangeException("e", message);
            }

            source._isPopupSecondsInitialized = true;

            if (source.ActualTimePickerPopup != null)
            {
                source.ActualTimePickerPopup.PopupSecondsInterval = value;
            }
        }

        /// <summary>
        /// Determines whether PopupSeconds has been changed.
        /// </summary>
        private bool _isPopupSecondsInitialized;
#endregion public int PopupSecondsInterval

#region public int PopupMinutesInterval
        /// <summary>
        /// Gets or sets the minutes interval between time values allowed by the 
        /// TimePickerPopup.
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
                typeof(TimePicker),
                new PropertyMetadata(OnPopupMinutesIntervalPropertyChanged));

        /// <summary>
        /// PopupMinutesIntervalProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupMinutesInterval.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupMinutesIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            int value = (int)e.NewValue;

            if (value < 0 || value > 59)
            {
                // revert to old value
                source.SetValue(PopupMinutesIntervalProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid PopupMinutesInterval '{0}'. The interval can be set to 0 (no interval) to and including 59.",
                    value);

                throw new ArgumentOutOfRangeException("e", message);
            }

            source._isPopupMinutesInitialized = true;

            if (source.ActualTimePickerPopup != null)
            {
                source.ActualTimePickerPopup.PopupMinutesInterval = value;
            }
        }

        /// <summary>
        /// Determines whether PopupMinutes has been changed.
        /// </summary>
        private bool _isPopupMinutesInitialized;
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
                typeof(TimePicker),
                new PropertyMetadata(PopupTimeSelectionMode.HoursAndMinutesOnly, OnPopupTimeSelectionModePropertyChanged));

        /// <summary>
        /// PopupTimeSelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupTimeSelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupTimeSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker source = (TimePicker)d;
            PopupTimeSelectionMode value = (PopupTimeSelectionMode)e.NewValue;

            // todo: the enum values that are commented out will be included after mix.
            bool valid = (// value == PopupTimeSelectionMode.AllowTimeDesignatorsSelection ||
                         value == PopupTimeSelectionMode.HoursAndMinutesOnly ||
                         // value == PopupTimeSelectionMode.AllowSecondsAndDesignatorsSelection ||
                         value == PopupTimeSelectionMode.AllowSecondsSelection);

            // the possibilities are limited to the popup that is used.
            // if there is a popup, question it
            if (source.ActualTimePickerPopup != null)
            {
                valid = source.ActualTimePickerPopup.GetValidPopupTimeSelectionModes().Contains(value);
            }

            if (!valid)
            {
                // revert to old value
                source.SetValue(PopupTimeSelectionModeProperty, e.OldValue);

                // todo: move to resource
                string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Invalid PopupTimeSelectionMode for this popup, value '{0}'.",
                        value);

                throw new ArgumentOutOfRangeException("e", message);
            }

            if (source.ActualTimePickerPopup != null)
            {
                source.ActualTimePickerPopup.PopupTimeSelectionMode = value;
            }
        }
#endregion public PopupTimeSelectionMode PopupTimeSelectionMode

#region public events
        /// <summary>
        /// Occurs when Value property is changing.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;

        /// <summary>
        /// Occurs when Value property has changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;

        /// <summary>
        /// Occurs when a value is being parsed and allows custom parsing.
        /// </summary>
        public event EventHandler<UpDownParsingEventArgs<DateTime?>> Parsing;

        /// <summary>
        /// Occurs when there is an error in parsing user input and allows adding parsing logic.
        /// </summary>
        public event EventHandler<UpDownParseErrorEventArgs> ParseError;
#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePicker"/> 
        /// class.
        /// </summary>
        public TimePicker()
        {
            DefaultStyleKey = typeof(TimePicker);
            _timeCoercionHelper = new TimeCoercionHelper(this);
        }

        /// <summary>
        /// Builds the visual tree for the TimePicker control when a new 
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            TimeUpDownPart = GetTemplateChild(ElementTimeUpDownName) as TimeUpDown;

            PopupPlaceHolderPart = GetTemplateChild(ElementPopupPlaceHolderPartName) as ContentControl;

            _isInitialized = true;

            // after inititialization has occured, possibly use template.
            InstantiatePopupFromTemplate();
        }

        /// <summary>
        /// Gets the selected time  represented in the control.
        /// </summary>
        /// <returns>The value that is picked.</returns>
        public override object GetSelectedValue()
        {
            return Value;
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

#region TimeUpDown handling
        /// <summary>
        /// Reacts to a change in value in TimeUpDown.
        /// </summary>
        /// <param name="sender">The TimeUpDown that changed its value.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void TimeUpDownValueChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            Value = e.NewValue;
        }

        /// <summary>
        /// Raised when TimeUpDown raises this event.
        /// </summary>
        /// <param name="sender">The TimeUpDown instance raising this event.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <remarks>Here to make it easier to access
        /// these events.</remarks>
        private void TimeUpDownParseError(object sender, UpDownParseErrorEventArgs e)
        {
            EventHandler<UpDownParseErrorEventArgs> handler = ParseError;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raised when TimeUpDown raises this event.
        /// </summary>
        /// <param name="sender">The TimeUpDown instance raising this event.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <remarks>Here to make it easier to access
        /// these events.</remarks>
        private void TimeUpDownParsing(object sender, UpDownParsingEventArgs<DateTime?> e)
        {
            EventHandler<UpDownParsingEventArgs<DateTime?>> handler = Parsing;
            if (handler != null)
            {
                handler(this, e);
            }
        }
#endregion TimeUpDown handling

#region Popup handling
        /// <summary>
        /// Raises an DropDownOpened event when the IsDropDownOpen property
        /// changed from false to true.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDropDownOpened(RoutedPropertyChangedEventArgs<bool> e)
        {
            base.OnDropDownOpened(e);

            _popupSessionValueCache = Value;

            TimePickerPopup popup = ActualTimePickerPopup;
            if (popup != null)
            {
                // initialize with current values
                popup.Minimum = Minimum;
                popup.Maximum = Maximum;
                popup.Value = Value;

                popup.Culture = Culture;
                popup.TimeGlobalizationInfo = TimeGlobalizationInfo;
                popup.PopupTimeSelectionMode = PopupTimeSelectionMode;
                popup.Format = Format;

                // be mindful that this call could change the value of the picker
                // because some popups 'snap' to closest value in their available
                // options. Since we have not yet subscribed to the valuechanged
                // event, we are able to ignore that event.
                popup.OnOpened();

                popup.ValueChanged += PopupValueChanged;
                popup.Cancel += PopupCanceled;
                popup.Commit += PopupCommitted;

                // updatelayout is called by popup.OnOpened so we know styles and 
                // templates have been applied.
                // this is necessary for values on Popup that we might want to use.

                // potentially overwrite our values with defaults from the popup
                // only do this if our own values have not been set.
                // SL does not support DependencyProperty.UnsetValue to enough
                // extent to know if the current value is the default or not.
                if (_isPopupSecondsInitialized)
                {
                    // push our value to Popup.
                    popup.PopupSecondsInterval = PopupSecondsInterval;
                }
                else
                {
                    // not initialized explicitly, use default from Popup.
                    PopupSecondsInterval = popup.PopupSecondsInterval;
                }

                if (_isPopupMinutesInitialized)
                {
                    // push our value to Popup.
                    popup.PopupMinutesInterval = PopupMinutesInterval;
                }
                else
                {
                    // not initialized explicitly, use default from Popup.
                    PopupMinutesInterval = popup.PopupMinutesInterval;
                }

                // set focus to the Popup.
                Dispatcher.BeginInvoke(() => popup.Focus());
            }
        }

        /// <summary>
        /// Raises an DropDownClosed event when the IsDropDownOpen property
        /// changed from true to false.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            base.OnDropDownClosed(e);

            TimePickerPopup popup = ActualTimePickerPopup as TimePickerPopup;
            if (popup != null)
            {
                popup.OnClosed();
                popup.ValueChanged -= PopupValueChanged;
                popup.Cancel -= PopupCanceled;
                popup.Commit -= PopupCommitted;
            }

            // only set focus to the Button if this close was initiated
            // after initialization.
            if (_isInitialized)
            {
                Dispatcher.BeginInvoke(() =>
                                           {
                                               if (DropDownToggleButton != null)
                                               {
                                                   DropDownToggleButton.Focus();
                                               }
                                           });
            }
        }

        /// <summary>
        /// Reacts to a Value change in a popup.
        /// </summary>
        /// <param name="sender">The Popup that raised a ValueChange.</param>
        /// <param name="e">The  instance containing the event data.</param>
        private void PopupValueChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            Value = e.NewValue;
        }

        /// <summary>
        /// The Popup has been committed. Will close the popup.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PopupCommitted(object sender, RoutedEventArgs e)
        {
            // no work needs to be done.
            // usage for this control will update value
            // during the picking of a time.

            IsDropDownOpen = false;
        }

        /// <summary>
        /// The Popup has been canceled. Will close the popup,
        /// and set the value back to its initial value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PopupCanceled(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
            // push back the 
            Value = _popupSessionValueCache;
        }

        /// <summary>
        /// Called when a new Popup is set.
        /// </summary>
        /// <param name="popup">The new value.</param>
        private void RegisterPopup(TimePickerPopup popup)
        {
            if (popup != null)
            {
                if (PopupPlaceHolderPart != null)
                {
                    PopupPlaceHolderPart.Content = popup;
                }

                popup.TimePickerParent = this;

                if (!popup.GetValidPopupTimeSelectionModes().Contains(PopupTimeSelectionMode))
                {
                    // silently coerce
                    PopupTimeSelectionMode = popup.PopupTimeSelectionMode;
                }
            }
        }

        /// <summary>
        /// Unregisters the popup.
        /// </summary>
        /// <param name="popup">The old value.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Symmetry with RegisterPopup.")]
        private void UnregisterPopup(TimePickerPopup popup)
        {
            if (popup != null)
            {
                popup.TimePickerParent = null;
            }
        }

        /// <summary>
        /// Instantiates the template.
        /// </summary>
        /// <remarks>Will only use template if Popup is not set.</remarks>
        private void InstantiatePopupFromTemplate()
        {
            // template will only be used when no Popup is set.

            if (_isInitialized && Popup == null && PopupTemplate != null)
            {
                // unregister oldPopup
                UnregisterPopup(ActualTimePickerPopup);

                // instantiate template
                TimePickerPopup popup = PopupTemplate.LoadContent() as TimePickerPopup;

                if (popup != null)
                {
                    _instantiatedPopupFromTemplate = popup;

                    // register
                    RegisterPopup(ActualTimePickerPopup);
                }
            }
        }
#endregion Popup handling

        /// <summary>
        /// Propagates the new value to components that are part of
        /// our template.
        /// </summary>
        /// <remarks>Workaround for SL2 inability to do twoway 
        /// templatebinding. Should remove in SL3.</remarks>
        private void PropagateNewValue()
        {
            if (ActualTimePickerPopup != null)
            {
                ActualTimePickerPopup.Value = Value;
            }

            if (TimeUpDownPart != null)
            {
                TimeUpDownPart.Value = Value;
            }
        }

        /// <summary>
        /// Returns a PickerAutomationPeer for use by the Silverlight 
        /// automation infrastructure.
        /// </summary>
        /// <returns>A PickerAutomationPeer for the Picker object.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TimePickerAutomationPeer(this);
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
    }
}