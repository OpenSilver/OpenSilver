

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
using System.Windows.Controls.Primitives;

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
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplatePart(Name = "TimeUpDown", Type = typeof(TimeUpDown))]
    [StyleTypedProperty(Property = "SpinnerStyle", StyleTargetType = typeof(ButtonSpinner))]
    [TemplateVisualState(GroupName = "PopupStates", Name = "PopupClosed")]
    [TemplateVisualState(GroupName = "PopupStates", Name = "PopupOpened")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    [TemplatePart(Name = "Popup", Type = typeof(System.Windows.Controls.Primitives.Popup))]
    [TemplatePart(Name = "DropDownToggle", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PopupPlaceHolder", Type = typeof(ContentControl))]
    [StyleTypedProperty(Property = "TimeUpDownStyle", StyleTargetType = typeof(TimeUpDown))]
    public class TimePicker : Picker, ITimeInput
    {
        /// <summary>Identifies the Value dependency property.</summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(DateTime?), typeof(TimePicker), new PropertyMetadata((object)new DateTime?(), new PropertyChangedCallback(TimePicker.OnValuePropertyChanged)));
        /// <summary>Identifies the Minimum dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(DateTime?), typeof(TimePicker), new PropertyMetadata((object)null, new PropertyChangedCallback(TimePicker.OnMinimumPropertyChanged)));
        /// <summary>Identifies the Maximum dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(DateTime?), typeof(TimePicker), new PropertyMetadata((object)null, new PropertyChangedCallback(TimePicker.OnMaximumPropertyChanged)));
        /// <summary>Identifies the TimeUpDownStyle dependency property.</summary>
        public static readonly DependencyProperty TimeUpDownStyleProperty = DependencyProperty.Register(nameof(TimeUpDownStyle), typeof(Style), typeof(TimePicker), new PropertyMetadata(new PropertyChangedCallback(TimePicker.OnTimeUpDownStylePropertyChanged)));
        /// <summary>Identifies the SpinnerStyle dependency property.</summary>
        public static readonly DependencyProperty SpinnerStyleProperty = DependencyProperty.Register(nameof(SpinnerStyle), typeof(Style), typeof(TimePicker), new PropertyMetadata(new PropertyChangedCallback(TimePicker.OnSpinnerStylePropertyChanged)));
        /// <summary>Identifies the TimeParsers dependency property.</summary>
        public static readonly DependencyProperty TimeParsersProperty = DependencyProperty.Register(nameof(TimeParsers), typeof(TimeParserCollection), typeof(TimePicker), new PropertyMetadata(new PropertyChangedCallback(TimePicker.OnTimeParsersPropertyChanged)));
        /// <summary>Identifies the Format dependency property.</summary>
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(nameof(Format), typeof(ITimeFormat), typeof(TimePicker), new PropertyMetadata(new PropertyChangedCallback(TimePicker.OnFormatPropertyChanged)));
        /// <summary>Identifies the Culture dependency property.</summary>
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(nameof(Culture), typeof(CultureInfo), typeof(TimePicker), new PropertyMetadata((object)null, new PropertyChangedCallback(TimePicker.OnCulturePropertyChanged)));
        /// <summary>
        /// Identifies the TimeGlobalizationInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeGlobalizationInfoProperty = DependencyProperty.Register(nameof(TimeGlobalizationInfo), typeof(TimeGlobalizationInfo), typeof(TimePicker), new PropertyMetadata(new PropertyChangedCallback(TimePicker.OnTimeGlobalizationInfoPropertyChanged)));
        /// <summary>Identifies the Popup dependency property.</summary>
        public static readonly DependencyProperty PopupProperty = DependencyProperty.Register(nameof(Popup), typeof(TimePickerPopup), typeof(TimePicker), new PropertyMetadata(new PropertyChangedCallback(TimePicker.OnPopupPropertyChanged)));
        /// <summary>Identifies the PopupTemplate dependency property.</summary>
        public static readonly DependencyProperty PopupTemplateProperty = DependencyProperty.Register(nameof(PopupTemplate), typeof(TimePickerPopupTemplate), typeof(TimePicker), new PropertyMetadata((object)null, new PropertyChangedCallback(TimePicker.OnPopupTemplatePropertyChanged)));
        /// <summary>
        /// Identifies the PopupSecondsInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupSecondsIntervalProperty = DependencyProperty.Register(nameof(PopupSecondsInterval), typeof(int), typeof(TimePicker), new PropertyMetadata(new PropertyChangedCallback(TimePicker.OnPopupSecondsIntervalPropertyChanged)));
        /// <summary>
        /// Identifies the PopupMinutesInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupMinutesIntervalProperty = DependencyProperty.Register(nameof(PopupMinutesInterval), typeof(int), typeof(TimePicker), new PropertyMetadata(new PropertyChangedCallback(TimePicker.OnPopupMinutesIntervalPropertyChanged)));
        /// <summary>
        /// Identifies the PopupTimeSelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupTimeSelectionModeProperty = DependencyProperty.Register(nameof(PopupTimeSelectionMode), typeof(PopupTimeSelectionMode), typeof(TimePicker), new PropertyMetadata((object)PopupTimeSelectionMode.HoursAndMinutesOnly, new PropertyChangedCallback(TimePicker.OnPopupTimeSelectionModePropertyChanged)));
        /// <summary>The name for the TimeUpDown element.</summary>
        private const string ElementTimeUpDownName = "TimeUpDown";
        /// <summary>The name for the TimeUpDownStyle element.</summary>
        private const string TimeUpDownStyleName = "TimeUpDownStyle";
        /// <summary>The name for the PopupPlaceHolder element.</summary>
        private const string ElementPopupPlaceHolderPartName = "PopupPlaceHolder";
        /// <summary>BackingField for TimeUpDownPart.</summary>
        private TimeUpDown _timeUpDownPart;
        /// <summary>BackingField for PopupPlaceHolderPart.</summary>
        private ContentControl _popupPlaceHolderPart;
        /// <summary>
        /// Helper class that centralizes the coercion logic across all
        /// TimeInput controls.
        /// </summary>
        private readonly TimeCoercionHelper _timeCoercionHelper;
        /// <summary>Cache of the value before we open a popup.</summary>
        private DateTime? _popupSessionValueCache;
        /// <summary>
        /// Indicates that the control has finished initialization.
        /// </summary>
        private bool _isInitialized;
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignoreValueChange;
        /// <summary>BackingField for ActualFormat.</summary>
        private ITimeFormat _actualFormat;
        /// <summary>BackingField for ActualTimeGlobalizationInfo.</summary>
        private TimeGlobalizationInfo _actualTimeGlobalizationInfo;
        /// <summary>BackingField for InstantiatedPopupFromTemplate.</summary>
        private TimePickerPopup _instantiatedPopupFromTemplate;
        /// <summary>Determines whether PopupSeconds has been changed.</summary>
        private bool _isPopupSecondsInitialized;
        /// <summary>Determines whether PopupMinutes has been changed.</summary>
        private bool _isPopupMinutesInitialized;

        /// <summary>Gets or sets the time up down part.</summary>
        /// <value>The time up down part.</value>
        private TimeUpDown TimeUpDownPart
        {
            get
            {
                return this._timeUpDownPart;
            }
            set
            {
                if (this._timeUpDownPart != null)
                {
                    this._timeUpDownPart.ValueChanged -= new RoutedPropertyChangedEventHandler<DateTime?>(this.TimeUpDownValueChanged);
                    this._timeUpDownPart.Parsing -= new EventHandler<UpDownParsingEventArgs<DateTime?>>(this.TimeUpDownParsing);
                    this._timeUpDownPart.ParseError -= new EventHandler<UpDownParseErrorEventArgs>(this.TimeUpDownParseError);
                }
                this._timeUpDownPart = value;
                if (this._timeUpDownPart == null)
                    return;
                this._timeUpDownPart.ValueChanged += new RoutedPropertyChangedEventHandler<DateTime?>(this.TimeUpDownValueChanged);
                this._timeUpDownPart.Parsing += new EventHandler<UpDownParsingEventArgs<DateTime?>>(this.TimeUpDownParsing);
                this._timeUpDownPart.ParseError += new EventHandler<UpDownParseErrorEventArgs>(this.TimeUpDownParseError);
                this.PropagateNewValue();
            }
        }

        /// <summary>Gets or sets the popup place holder part.</summary>
        /// <remarks>This is the ContentControl that is used to display
        /// Popups.</remarks>
        private ContentControl PopupPlaceHolderPart
        {
            get
            {
                return this._popupPlaceHolderPart;
            }
            set
            {
                if (this._popupPlaceHolderPart != null)
                {
                    this._popupPlaceHolderPart.Content = (object)null;
                    this._popupPlaceHolderPart.ContentTemplate = (DataTemplate)null;
                }
                this._popupPlaceHolderPart = value;
                if (this._popupPlaceHolderPart == null || this.ActualTimePickerPopup == null)
                    return;
                this._popupPlaceHolderPart.Content = (object)this.ActualTimePickerPopup;
            }
        }

        /// <summary>Gets or sets the currently selected time.</summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public virtual DateTime? Value
        {
            get
            {
                return (DateTime?)this.GetValue(TimePicker.ValueProperty);
            }
            set
            {
                this.SetValue(TimePicker.ValueProperty, (object)value);
            }
        }

        /// <summary>ValueProperty property changed handler.</summary>
        /// <param name="d">UpDownBase whose Value changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValuePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            if (timePicker._ignoreValueChange)
            {
                timePicker.OnValueChanged(new RoutedPropertyChangedEventArgs<DateTime?>(e.OldValue as DateTime?, e.NewValue as DateTime?));
            }
            else
            {
                DateTime? oldValue = (DateTime?)e.OldValue;
                DateTime? newValue1 = (DateTime?)e.NewValue;
                RoutedPropertyChangingEventArgs<DateTime?> e1 = new RoutedPropertyChangingEventArgs<DateTime?>(e.Property, oldValue, newValue1, true);
                timePicker.OnValueChanging(e1);
                if (e1.InCoercion)
                    return;
                if (!e1.Cancel)
                {
                    DateTime? newValue2 = e1.NewValue;
                    RoutedPropertyChangedEventArgs<DateTime?> e2 = new RoutedPropertyChangedEventArgs<DateTime?>(oldValue, newValue2);
                    timePicker.OnValueChanged(e2);
                    timePicker.PropagateNewValue();
                }
                else
                {
                    timePicker._ignoreValueChange = true;
                    timePicker.Value = oldValue;
                    timePicker._ignoreValueChange = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the minimum property is applicable for the following
        /// features: Selecting a value through a popup, Parsing a new value from
        /// the textbox, spinning a new value and programmatically specifying a value.</remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum
        {
            get
            {
                return (DateTime?)this.GetValue(TimePicker.MinimumProperty);
            }
            set
            {
                this.SetValue(TimePicker.MinimumProperty, (object)value);
            }
        }

        /// <summary>MinimumProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;
            timePicker._ignoreValueChange = true;
            timePicker._timeCoercionHelper.ProcessMinimumChange(newValue);
            timePicker._ignoreValueChange = false;
            timePicker.OnMinimumChanged(oldValue, newValue);
            if (timePicker.ActualTimePickerPopup == null)
                return;
            timePicker.ActualTimePickerPopup.Minimum = newValue;
        }

        /// <summary>Called when the Minimum property value has changed.</summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }

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
            get
            {
                return (DateTime?)this.GetValue(TimePicker.MaximumProperty);
            }
            set
            {
                this.SetValue(TimePicker.MaximumProperty, (object)value);
            }
        }

        /// <summary>MaximumProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;
            timePicker._ignoreValueChange = true;
            timePicker._timeCoercionHelper.ProcessMaximumChange(newValue);
            timePicker._ignoreValueChange = false;
            timePicker.OnMaximumChanged(oldValue, newValue);
            if (timePicker.ActualTimePickerPopup == null)
                return;
            timePicker.ActualTimePickerPopup.Maximum = newValue;
        }

        /// <summary>Called when the Maximum property value has changed.</summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }

        /// <summary>
        /// Gets or sets the Style applied to the TimeUpDown portion of the TimePicker
        /// control.
        /// </summary>
        public Style TimeUpDownStyle
        {
            get
            {
                return this.GetValue(TimePicker.TimeUpDownStyleProperty) as Style;
            }
            set
            {
                this.SetValue(TimePicker.TimeUpDownStyleProperty, (object)value);
            }
        }

        /// <summary>TimeUpDownStyleProperty property changed handler.</summary>
        /// <param name="d">TimePicker that changed its TimeUpDownStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeUpDownStylePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets the Style that is applied to the spinner.
        /// </summary>
        public Style SpinnerStyle
        {
            get
            {
                return this.GetValue(TimePicker.SpinnerStyleProperty) as Style;
            }
            set
            {
                this.SetValue(TimePicker.SpinnerStyleProperty, (object)value);
            }
        }

        /// <summary>SpinnerStyleProperty property changed handler.</summary>
        /// <param name="d">TimePicker that changed its SpinnerStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSpinnerStylePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets a collection of TimeParsers that are used when parsing
        /// text to time.
        /// </summary>
        public TimeParserCollection TimeParsers
        {
            get
            {
                return this.GetValue(TimePicker.TimeParsersProperty) as TimeParserCollection;
            }
            set
            {
                this.SetValue(TimePicker.TimeParsersProperty, (object)value);
            }
        }

        /// <summary>TimeParsersProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its TimeParsers.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeParsersPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the actual TimeParsers that will be used for parsing by the control.
        /// </summary>
        /// <remarks>Includes the TimeParsers introduced in the TimeGlobalizationInfo.</remarks>
        public TimeParserCollection ActualTimeParsers
        {
            get
            {
                return new TimeParserCollection(this.ActualTimeGlobalizationInfo.GetActualTimeParsers(this.TimeParsers == null ? (IEnumerable<TimeParser>)null : (IEnumerable<TimeParser>)this.TimeParsers.ToList<TimeParser>()));
            }
        }

        /// <summary>
        /// Gets or sets the Format used by the control. From XAML Use either
        /// "Short", "Long" or a custom format.
        /// Custom formats can only contain "H", "h", "m", "s" or "t".
        /// For example: use 'hh:mm:ss' is used to format time as "13:45:30".
        /// </summary>
        [TypeConverter(typeof(TimeFormatConverter))]
        public ITimeFormat Format
        {
            get
            {
                return this.GetValue(TimePicker.FormatProperty) as ITimeFormat;
            }
            set
            {
                this.SetValue(TimePicker.FormatProperty, (object)value);
            }
        }

        /// <summary>FormatProperty property changed handler.</summary>
        /// <param name="d">TimePicker that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            ITimeFormat newValue = e.NewValue as ITimeFormat;
            if (e.NewValue != null)
                timePicker._actualFormat = (ITimeFormat)null;
            if (timePicker.ActualTimePickerPopup == null)
                return;
            timePicker.ActualTimePickerPopup.Format = newValue;
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
                if (this.Format != null)
                    return this.Format;
                if (this._actualFormat == null)
                    this._actualFormat = (ITimeFormat)new ShortTimeFormat();
                return this._actualFormat;
            }
        }

        /// <summary>
        /// Gets or sets the culture that will be used by the control for
        /// parsing and formatting.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)this.GetValue(TimePicker.CultureProperty);
            }
            set
            {
                this.SetValue(TimePicker.CultureProperty, (object)value);
            }
        }

        /// <summary>CultureProperty property changed handler.</summary>
        /// <param name="d">TimeUpDown that changed its Culture.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCulturePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            timePicker.ActualTimeGlobalizationInfo.Culture = e.NewValue as CultureInfo;
            if (timePicker.ActualTimePickerPopup == null)
                return;
            timePicker.ActualTimePickerPopup.Culture = e.NewValue as CultureInfo;
        }

        /// <summary>
        /// Gets the actual culture used by the control for formatting and parsing.
        /// </summary>
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
                return (TimeGlobalizationInfo)this.GetValue(TimePicker.TimeGlobalizationInfoProperty);
            }
            set
            {
                this.SetValue(TimePicker.TimeGlobalizationInfoProperty, (object)value);
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
            TimePicker timePicker = (TimePicker)d;
            TimeGlobalizationInfo newValue = null;
            if (e.NewValue is TimeGlobalizationInfo)
            {
                newValue = e.NewValue as TimeGlobalizationInfo;
                newValue.Culture = timePicker.Culture;
                timePicker._actualTimeGlobalizationInfo = (TimeGlobalizationInfo)null;
            }
            if (timePicker.ActualTimePickerPopup == null)
                return;
            timePicker.ActualTimePickerPopup.TimeGlobalizationInfo = newValue;
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
                TimeGlobalizationInfo globalizationInfo = this.TimeGlobalizationInfo;
                if (globalizationInfo != null)
                    return globalizationInfo;
                if (this._actualTimeGlobalizationInfo == null)
                    this._actualTimeGlobalizationInfo = new TimeGlobalizationInfo();
                return this._actualTimeGlobalizationInfo;
            }
        }

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
                return this.Popup != null ? this.Popup : this._instantiatedPopupFromTemplate;
            }
        }

        /// <summary>
        /// Gets or sets the TimePickerPopup that will be shown to the user by the
        /// TimePicker control. This property may not be styled. To style a
        /// TimePicker with a Popup, please use the PopupTemplate property.
        /// When both PopupTemplate and Popup are set, Popup will be used.
        /// </summary>
        /// <remark>This property might be null, since a template can be used.</remark>
        public TimePickerPopup Popup
        {
            get
            {
                return this.GetValue(TimePicker.PopupProperty) as TimePickerPopup;
            }
            set
            {
                this.SetValue(TimePicker.PopupProperty, (object)value);
            }
        }

        /// <summary>PopupProperty property changed handler.</summary>
        /// <param name="d">TimePicker that changed its Popup.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            if (timePicker.Style != null && timePicker.Style.Setters.Where<SetterBase>((Func<SetterBase, bool>)(setterbase => setterbase is Setter setter && setter.Property == TimePicker.PopupProperty)).Count<SetterBase>() > 0)
                throw new ArgumentException("Time Picker Popup Style Not Set");
            TimePickerPopup oldValue = e.OldValue as TimePickerPopup;
            TimePickerPopup newValue = e.NewValue as TimePickerPopup;
            if (timePicker.PopupPlaceHolderPart != null)
                timePicker.PopupPlaceHolderPart.ContentTemplate = (DataTemplate)null;
            timePicker.UnregisterPopup(oldValue);
            timePicker.RegisterPopup(newValue);
            if (newValue == null)
                timePicker.InstantiatePopupFromTemplate();
            else
                timePicker._instantiatedPopupFromTemplate = (TimePickerPopup)null;
        }

        /// <summary>
        /// Gets or sets the template used as Popup. A Popup can also be set
        /// directly on the Popup property. When both PopupTemplate and Popup
        /// are set, Popup will be used.
        /// </summary>
        public TimePickerPopupTemplate PopupTemplate
        {
            get
            {
                return this.GetValue(TimePicker.PopupTemplateProperty) as TimePickerPopupTemplate;
            }
            set
            {
                this.SetValue(TimePicker.PopupTemplateProperty, (object)value);
            }
        }

        /// <summary>PopupTemplateProperty property changed handler.</summary>
        /// <param name="d">TimePicker that changed its PopupTemplate.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupTemplatePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            ((TimePicker)d).InstantiatePopupFromTemplate();
        }

        /// <summary>
        /// Gets or sets the seconds interval between time values allowed by
        /// the TimePickerPopup.
        /// </summary>
        public int PopupSecondsInterval
        {
            get
            {
                return (int)this.GetValue(TimePicker.PopupSecondsIntervalProperty);
            }
            set
            {
                this.SetValue(TimePicker.PopupSecondsIntervalProperty, (object)value);
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
            TimePicker timePicker = (TimePicker)d;
            int newValue = (int)e.NewValue;
            if (newValue < 0 || newValue > 59)
            {
                timePicker.SetValue(TimePicker.PopupSecondsIntervalProperty, e.OldValue);
                throw new ArgumentOutOfRangeException(nameof(e), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid {0}", (object)newValue));
            }
            timePicker._isPopupSecondsInitialized = true;
            if (timePicker.ActualTimePickerPopup == null)
                return;
            timePicker.ActualTimePickerPopup.PopupSecondsInterval = newValue;
        }

        /// <summary>
        /// Gets or sets the minutes interval between time values allowed by the
        /// TimePickerPopup.
        /// </summary>
        public int PopupMinutesInterval
        {
            get
            {
                return (int)this.GetValue(TimePicker.PopupMinutesIntervalProperty);
            }
            set
            {
                this.SetValue(TimePicker.PopupMinutesIntervalProperty, (object)value);
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
            TimePicker timePicker = (TimePicker)d;
            int newValue = (int)e.NewValue;
            if (newValue < 0 || newValue > 59)
            {
                timePicker.SetValue(TimePicker.PopupMinutesIntervalProperty, e.OldValue);
                throw new ArgumentOutOfRangeException(nameof(e), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid {0}", (object)newValue));
            }
            timePicker._isPopupMinutesInitialized = true;
            if (timePicker.ActualTimePickerPopup == null)
                return;
            timePicker.ActualTimePickerPopup.PopupMinutesInterval = newValue;
        }

        /// <summary>
        /// Gets or sets the whether the TimePickerPopup supports selecting
        /// designators and/or seconds.
        /// </summary>
        public PopupTimeSelectionMode PopupTimeSelectionMode
        {
            get
            {
                return (PopupTimeSelectionMode)this.GetValue(TimePicker.PopupTimeSelectionModeProperty);
            }
            set
            {
                this.SetValue(TimePicker.PopupTimeSelectionModeProperty, (object)value);
            }
        }

        /// <summary>
        /// PopupTimeSelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupTimeSelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupTimeSelectionModePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            TimePicker timePicker = (TimePicker)d;
            PopupTimeSelectionMode newValue = (PopupTimeSelectionMode)e.NewValue;
            bool flag = newValue == PopupTimeSelectionMode.HoursAndMinutesOnly || newValue == PopupTimeSelectionMode.AllowSecondsSelection;
            if (timePicker.ActualTimePickerPopup != null)
                flag = ((IEnumerable<PopupTimeSelectionMode>)timePicker.ActualTimePickerPopup.GetValidPopupTimeSelectionModes()).Contains<PopupTimeSelectionMode>(newValue);
            if (!flag)
            {
                timePicker.SetValue(TimePicker.PopupTimeSelectionModeProperty, e.OldValue);
                throw new ArgumentOutOfRangeException(nameof(e), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid {0}", (object)newValue));
            }
            if (timePicker.ActualTimePickerPopup == null)
                return;
            timePicker.ActualTimePickerPopup.PopupTimeSelectionMode = newValue;
        }

        /// <summary>Occurs when Value property is changing.</summary>
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;

        /// <summary>Occurs when Value property has changed.</summary>
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;

        /// <summary>
        /// Occurs when a value is being parsed and allows custom parsing.
        /// </summary>
        public event EventHandler<UpDownParsingEventArgs<DateTime?>> Parsing;

        /// <summary>
        /// Occurs when there is an error in parsing user input and allows adding parsing logic.
        /// </summary>
        public event EventHandler<UpDownParseErrorEventArgs> ParseError;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.TimePicker" />
        /// class.
        /// </summary>
        public TimePicker()
        {
            this.DefaultStyleKey = (object)typeof(TimePicker);
            this._timeCoercionHelper = new TimeCoercionHelper((ITimeInput)this);
        }

        /// <summary>
        /// Builds the visual tree for the TimePicker control when a new
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.TimeUpDownPart = this.GetTemplateChild("TimeUpDown") as TimeUpDown;
            this.PopupPlaceHolderPart = this.GetTemplateChild("PopupPlaceHolder") as ContentControl;
            this._isInitialized = true;
            this.InstantiatePopupFromTemplate();
        }

        /// <summary>Gets the selected time  represented in the control.</summary>
        /// <returns>The value that is picked.</returns>
        public override object GetSelectedValue()
        {
            return (object)this.Value;
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

        /// <summary>Reacts to a change in value in TimeUpDown.</summary>
        /// <param name="sender">The TimeUpDown that changed its value.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void TimeUpDownValueChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            this.Value = e.NewValue;
        }

        /// <summary>Raised when TimeUpDown raises this event.</summary>
        /// <param name="sender">The TimeUpDown instance raising this event.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <remarks>Here to make it easier to access
        /// these events.</remarks>
        private void TimeUpDownParseError(object sender, UpDownParseErrorEventArgs e)
        {
            EventHandler<UpDownParseErrorEventArgs> parseError = this.ParseError;
            if (parseError == null)
                return;
            parseError((object)this, e);
        }

        /// <summary>Raised when TimeUpDown raises this event.</summary>
        /// <param name="sender">The TimeUpDown instance raising this event.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <remarks>Here to make it easier to access
        /// these events.</remarks>
        private void TimeUpDownParsing(object sender, UpDownParsingEventArgs<DateTime?> e)
        {
            EventHandler<UpDownParsingEventArgs<DateTime?>> parsing = this.Parsing;
            if (parsing == null)
                return;
            parsing((object)this, e);
        }

        /// <summary>
        /// Raises an DropDownOpened event when the IsDropDownOpen property
        /// changed from false to true.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDropDownOpened(RoutedPropertyChangedEventArgs<bool> e)
        {
            base.OnDropDownOpened(e);
            this._popupSessionValueCache = this.Value;
            TimePickerPopup popup = this.ActualTimePickerPopup;
            if (popup == null)
                return;
            popup.Minimum = this.Minimum;
            popup.Maximum = this.Maximum;
            popup.Value = this.Value;
            popup.Culture = this.Culture;
            popup.TimeGlobalizationInfo = this.TimeGlobalizationInfo;
            popup.PopupTimeSelectionMode = this.PopupTimeSelectionMode;
            popup.Format = this.Format;
            popup.OnOpened();
            popup.ValueChanged += new RoutedPropertyChangedEventHandler<DateTime?>(this.PopupValueChanged);
            popup.Cancel += new RoutedEventHandler(this.PopupCanceled);
            popup.Commit += new RoutedEventHandler(this.PopupCommitted);
            if (this._isPopupSecondsInitialized)
                popup.PopupSecondsInterval = this.PopupSecondsInterval;
            else
                this.PopupSecondsInterval = popup.PopupSecondsInterval;
            if (this._isPopupMinutesInitialized)
                popup.PopupMinutesInterval = this.PopupMinutesInterval;
            else
                this.PopupMinutesInterval = popup.PopupMinutesInterval;
            this.Dispatcher.BeginInvoke((Action)(() => popup.Focus()));
        }

        /// <summary>
        /// Raises an DropDownClosed event when the IsDropDownOpen property
        /// changed from true to false.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            base.OnDropDownClosed(e);
            TimePickerPopup actualTimePickerPopup = this.ActualTimePickerPopup;
            if (actualTimePickerPopup != null)
            {
                actualTimePickerPopup.OnClosed();
                actualTimePickerPopup.ValueChanged -= new RoutedPropertyChangedEventHandler<DateTime?>(this.PopupValueChanged);
                actualTimePickerPopup.Cancel -= new RoutedEventHandler(this.PopupCanceled);
                actualTimePickerPopup.Commit -= new RoutedEventHandler(this.PopupCommitted);
            }
            if (!this._isInitialized)
                return;
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (this.DropDownToggleButton == null)
                    return;
                this.DropDownToggleButton.Focus();
            }));
        }

        /// <summary>Reacts to a Value change in a popup.</summary>
        /// <param name="sender">The Popup that raised a ValueChange.</param>
        /// <param name="e">The  instance containing the event data.</param>
        private void PopupValueChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            this.Value = e.NewValue;
        }

        /// <summary>The Popup has been committed. Will close the popup.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void PopupCommitted(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        /// <summary>
        /// The Popup has been canceled. Will close the popup,
        /// and set the value back to its initial value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void PopupCanceled(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = false;
            this.Value = this._popupSessionValueCache;
        }

        /// <summary>Called when a new Popup is set.</summary>
        /// <param name="popup">The new value.</param>
        private void RegisterPopup(TimePickerPopup popup)
        {
            if (popup == null)
                return;
            if (this.PopupPlaceHolderPart != null)
                this.PopupPlaceHolderPart.Content = (object)popup;
            popup.TimePickerParent = this;
            if (!((IEnumerable<PopupTimeSelectionMode>)popup.GetValidPopupTimeSelectionModes()).Contains<PopupTimeSelectionMode>(this.PopupTimeSelectionMode))
                this.PopupTimeSelectionMode = popup.PopupTimeSelectionMode;
        }

        /// <summary>Unregisters the popup.</summary>
        /// <param name="popup">The old value.</param>
        private void UnregisterPopup(TimePickerPopup popup)
        {
            if (popup == null)
                return;
            popup.TimePickerParent = (TimePicker)null;
        }

        /// <summary>Instantiates the template.</summary>
        /// <remarks>Will only use template if Popup is not set.</remarks>
        private void InstantiatePopupFromTemplate()
        {
            if (!this._isInitialized || this.Popup != null || this.PopupTemplate == null)
                return;
            this.UnregisterPopup(this.ActualTimePickerPopup);
            if (this.PopupTemplate.LoadContent() is TimePickerPopup timePickerPopup)
            {
                this._instantiatedPopupFromTemplate = timePickerPopup;
                this.RegisterPopup(this.ActualTimePickerPopup);
            }
        }

        /// <summary>
        /// Propagates the new value to components that are part of
        /// our template.
        /// </summary>
        /// <remarks>Workaround for SL2 inability to do twoway
        /// templatebinding. Should remove in SL3.</remarks>
        private void PropagateNewValue()
        {
            if (this.ActualTimePickerPopup != null)
                this.ActualTimePickerPopup.Value = this.Value;
            if (this.TimeUpDownPart == null)
                return;
            this.TimeUpDownPart.Value = this.Value;
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
    }
}
