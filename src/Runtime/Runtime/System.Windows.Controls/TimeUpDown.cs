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
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Automation.Peers;
using System.Windows.Automation;
#else
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Interop;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that uses a spinner and textbox to allow a user to 
    /// input time.
    /// </summary>
    /// <remarks>TimeInput supports only the following formatting characters:
    /// 'h', 'm', 's', 'H', 't'. All other characters are filtered out:
    /// 'd', 'f', 'F', 'g', 'K', 'M', 'y', 'z'.</remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = TimeHintOpenedUpStateName, GroupName = GroupTimeHint)]
    [TemplateVisualState(Name = TimeHintOpenedDownStateName, GroupName = GroupTimeHint)]
    [TemplateVisualState(Name = TimeHintClosedStateName, GroupName = GroupTimeHint)]

    [TemplateVisualState(Name = ValidTimeStateName, GroupName = GroupTimeParsingStates)]
    [TemplateVisualState(Name = InvalidTimeStateName, GroupName = GroupTimeParsingStates)]
    [TemplateVisualState(Name = EmptyTimeStateName, GroupName = GroupTimeParsingStates)]

    [TemplatePart(Name = ElementTextName, Type = typeof(TextBox))]
    [TemplatePart(Name = ElementSpinnerName, Type = typeof(Spinner))]
    [TemplatePart(Name = ElementTimeHintPopupName, Type = typeof(Popup))]
    [StyleTypedProperty(Property = SpinnerStyleName, StyleTargetType = typeof(ButtonSpinner))]
    public class TimeUpDown : UpDownBase<DateTime?>, IUpdateVisualState, ITimeInput
    {
        /// <summary>
        /// StringFormat used in the TimeHint.
        /// </summary>
        private const string TimeHintFormat = "<{0}>";

#region TemplatePart names
        /// <summary>
        /// The name for the TimeHint element.
        /// </summary>
        private const string ElementTimeHintPopupName = "TimeHintPopup";
#endregion TemplatePart names

#region Visual state names
        /// <summary>
        /// The group name "TimeHintStates".
        /// </summary>
        internal const string GroupTimeHint = "TimeHintStates";

        /// <summary>
        /// The group name "ParsingStates".
        /// </summary>
        internal const string GroupTimeParsingStates = "ParsingStates";

        /// <summary>
        /// The state name "TimeHintOpenedUp" indicates that the hint is being
        /// shown on the top of the control.
        /// </summary>
        internal const string TimeHintOpenedUpStateName = "TimeHintOpenedUp";

        /// <summary>
        /// The state name "TimeHintOpenedDown" indicates that the hint is
        /// being shown at the bottom of the control.
        /// </summary>
        internal const string TimeHintOpenedDownStateName = "TimeHintOpenedDown";

        /// <summary>
        /// The state name "TimeHintClosed" indicates that no hint is being 
        /// shown.
        /// </summary>
        internal const string TimeHintClosedStateName = "TimeHintClosed";

        /// <summary>
        /// The state name "ValidTime" that indicates that currently the textbox
        /// text parses to a valid Time.
        /// </summary>
        internal const string ValidTimeStateName = "ValidTime";

        /// <summary>
        /// The state name "InvalidTime" that indicates that currently the textbox 
        /// text does not allow parsing.
        /// </summary>
        internal const string InvalidTimeStateName = "InvalidTime";

        /// <summary>
        /// The state name "EmptyTime" that indicates that currently the textbox
        /// text would parse to a Null.
        /// </summary>
        internal const string EmptyTimeStateName = "EmptyTime";

#endregion

#region TemplateParts

        /// <summary>
        /// Gets or sets the time hint popup part.
        /// </summary>
        /// <value>The time hint popup part.</value>
        private Popup TimeHintPopupPart
        {
            get { return _timeHintPopupPart; }
            set
            {
                // the content of the TimeHint is not going to be changed
                // on the fly. We can subscribe to mouse events from here.

                if (_timeHintPopupPart != null && _timeHintPopupPart.Child != null)
                {
#if MIGRATION
                    _timeHintPopupPart.Child.MouseLeftButtonDown -= OnTimeHintMouseLeftButtonDown;
#else
                    _timeHintPopupPart.Child.PointerPressed -= OnTimeHintMouseLeftButtonDown;
#endif
                }

                _timeHintPopupPart = value;

                if (_timeHintPopupPart != null)
                {
                    if (_timeHintPopupPart.Child != null)
                    {
#if MIGRATION
                        _timeHintPopupPart.Child.MouseLeftButtonDown += OnTimeHintMouseLeftButtonDown;
#else
                        _timeHintPopupPart.Child.PointerPressed += OnTimeHintMouseLeftButtonDown;
#endif
                    }
                    // Defer opening of the Popup to avoid an exception
                    Dispatcher.BeginInvoke(() => _timeHintPopupPart.IsOpen = true);
                }
            }
        }

        /// <summary>
        /// BackingField for TimeHintPopupPart.
        /// </summary>
        private Popup _timeHintPopupPart;

#endregion TemplateParts

        /// <summary>
        /// Helper class that centralizes the coercion logic across all 
        /// TimeInput controls.
        /// </summary>
        private readonly TimeCoercionHelper _timeCoercionHelper;

        /// <summary>
        /// The text that was last parsed. Used in comparisons.
        /// </summary>
        private string _lastParsedText;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is showing a
        /// TimeHint visual.
        /// </summary>
        /// <value><c>True</c> if this instance is showing the TimeHint; otherwise, <c>false</c>.</value>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Follows picker pattern.")]
        private bool IsShowTimeHint
        {
            get { return _isShowTimeHint; }
            set
            {
                if (value)
                {
                    // TimeHint popup needs to be shown
                    if (Text != null && TimeHintPopupPart != null && TimeHintPopupPart.Child != null)
                    {
                        // will need to work with the child of the popup
                        FrameworkElement child = TimeHintPopupPart.Child as FrameworkElement;

                        if (child != null)
                        {
                            // Popup does not determine width correctly
                            child.Width = Text.ActualWidth;

                            // defaulting to no TimeHint
                            _timeHintExpandDirection = null;

                            // determine Direction
                            // prefer opening on top.
                            MatrixTransform mt;
                            try
                            {
                                // will fail if popup is not in visual tree yet
                                mt = TransformToVisual(null) as MatrixTransform;
                            }
                            catch
                            {
                                IsShowTimeHint = false;
                                return;
                            }

                            // disregard height setting on the TimeHint since it will be animated. 
                            // using desired height to determine if the TimeHint is capable
                            // of being shown on top.
                            // this is not perfect, since the storyboard might animate
                            // to a different height, but that is not the common scenario.
                            child.Height = Double.NaN;
                            child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                            if (mt != null)
                            {
                                if (mt.Matrix.OffsetY > child.DesiredSize.Height)
                                {
                                    // will use the up animation
                                    _timeHintExpandDirection = ExpandDirection.Up;
                                    // position the popup at the top of the content
                                    TimeHintPopupPart.VerticalAlignment = VerticalAlignment.Top;
                                }
                                else
                                {
                                    // need to determine if there is space below the picker
                                    Content hostContent = Application.Current.Host.Content;
                                    if (hostContent.ActualHeight - (mt.Matrix.OffsetY + ActualHeight) > child.DesiredSize.Height)
                                    {
                                        _timeHintExpandDirection = ExpandDirection.Down;
                                        TimeHintPopupPart.VerticalAlignment = VerticalAlignment.Bottom;
                                    }
                                    else
                                    {
                                        IsShowTimeHint = false;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                _isShowTimeHint = value;
                UpdateVisualState(true);
            }
        }

        /// <summary>
        /// The direction in which the TimeHint will expand.
        /// </summary>
        private ExpandDirection? _timeHintExpandDirection;

        /// <summary>
        /// BackingField for IsShowTimeHint.
        /// </summary>
        private bool _isShowTimeHint;

        /// <summary>
        /// Gets the actual minimum. If a Minimum is set, use that, otherwise
        /// use the start of the day.
        /// </summary>        
        private DateTime ActualMinimum
        {
            get
            {
                return Minimum.HasValue ?
                    (Value.HasValue ? Value.Value.Date.Add(Minimum.Value.TimeOfDay) : Minimum.Value)
                    : (Value.HasValue ? Value.Value.Date : DateTime.Now.Date);
            }
        }

        /// <summary>
        /// Gets the actual maximum. If a Maximum is set, use that, otherwise
        /// use the end of the day.
        /// </summary>
        /// <value>The actual maximum.</value>
        private DateTime ActualMaximum
        {
            get
            {
                return Maximum.HasValue
                               ? (Value.HasValue ? Value.Value.Date.Add(Maximum.Value.TimeOfDay) : Maximum.Value)
                               : (Value.HasValue
                                          ? Value.Value.Date.Add(TimeSpan.FromDays(1).Subtract(TimeSpan.FromSeconds(1)))
                                          : DateTime.Now.Date.AddDays(1).Subtract(TimeSpan.FromSeconds(1)));
            }
        }

        /// <summary>
        /// Gets or sets the currently selected time.
        /// </summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public override DateTime? Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignoreValueChange;

#region public DateTime? Minimum
        /// <summary>
        /// Gets or sets the minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the minimum property is applicable for the following 
        /// features: Parsing a new value from the textbox, spinning a new value 
        /// and programmatically specifying a value.</remarks>
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
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnMinimumPropertyChanged));

        /// <summary>
        /// MinimumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            source._ignoreValueChange = true;
            source._timeCoercionHelper.ProcessMinimumChange(newValue);
            source._ignoreValueChange = false;
            source.OnMinimumChanged(oldValue, newValue);
            source.SetValidSpinDirection();
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
        /// features: Parsing a new value from the textbox, spinning a new value 
        /// and programmatically specifying a value. </remarks>
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
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnMaximumPropertyChanged));

        /// <summary>
        /// MaximumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            source._ignoreValueChange = true;
            source._timeCoercionHelper.ProcessMaximumChange(newValue);
            source._ignoreValueChange = false;
            source.OnMaximumChanged(oldValue, newValue);
            source.SetValidSpinDirection();
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
                typeof(TimeUpDown),
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
                typeof(TimeUpDown),
                new PropertyMetadata(OnFormatPropertyChanged));

        /// <summary>
        /// FormatProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            if (e.NewValue != null)
            {
                // no need for cache any more.
                source._actualFormat = null;
            }
            source.SetTextBoxText();
        }

        /// <summary>
        /// Gets the actual format that will be used to display Time  in the 
        /// TimeUpDown. If no format is specified, ShortTimeFormat is used.
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
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnCulturePropertyChanged));

        /// <summary>
        /// CultureProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Culture.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            source.ActualTimeGlobalizationInfo.Culture = e.NewValue as CultureInfo;
            source.SetTextBoxText();
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
                typeof(TimeUpDown),
                new PropertyMetadata(OnTimeGlobalizationInfoPropertyChanged));

        /// <summary>
        /// TimeGlobalizationInfoProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeGlobalizationInfo.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeGlobalizationInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            TimeGlobalizationInfo newValue = e.NewValue as TimeGlobalizationInfo;
            if (newValue != null)
            {
                newValue.Culture = source.Culture;
                source._actualTimeGlobalizationInfo = null; // no need for default any more.
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

#region public bool IsCyclic
        /// <summary>
        /// Gets or sets a value indicating whether the TimeUpDown control will 
        /// cycle through values when trying to spin the first and last item.
        /// </summary>
        public bool IsCyclic
        {
            get { return (bool)GetValue(IsCyclicProperty); }
            set { SetValue(IsCyclicProperty, value); }
        }

        /// <summary>
        /// Identifies the IsCyclic dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCyclicProperty =
            DependencyProperty.Register(
                "IsCyclic",
                typeof(bool),
                typeof(TimeUpDown),
                new PropertyMetadata(true, OnIsCyclicPropertyChanged));

        /// <summary>
        /// IsCyclicProperty property changed handler.
        /// </summary>
        /// <param name="d">DomainUpDown instance that changed its IsCyclic value.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsCyclicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            source.SetValidSpinDirection();
        }
#endregion public bool IsCyclic

#region public object TimeHintContent
        /// <summary>
        /// Gets the text used to guide the user when entering time.
        /// </summary>
        public object TimeHintContent
        {
            get { return GetValue(TimeHintContentProperty) as object; }
            private set
            {
                _allowHintContentChange = true;
                SetValue(TimeHintContentProperty, value);
                _allowHintContentChange = false;
            }
        }

        /// <summary>
        /// Identifies the TimeHintContent dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeHintContentProperty =
            DependencyProperty.Register(
                "TimeHintContent",
                typeof(object),
                typeof(TimeUpDown),
                new PropertyMetadata(String.Empty, OnTimeHintContentPropertyChanged));

        /// <summary>
        /// TimeHintContentProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeHintContent.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeHintContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            if (!source._allowHintContentChange)
            {
                // set old value
                source.TimeHintContent = e.OldValue;

                throw new InvalidOperationException("Cannot set read-only property TimeHintContent.");
            }
        }

        /// <summary>
        /// Represents the formatted DateTime that is used in the TimeHint hint.
        /// </summary>
        private string _timeHintDate;

        /// <summary>
        /// Indicates whether the control should not proceed with selecting all
        /// text.
        /// </summary>
        private bool _isIgnoreSelectionOfAllText;

        /// <summary>
        /// BackingField for AllowHintContentChange.
        /// </summary>
        private bool _allowHintContentChange;

#endregion public object TimeHintContent

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeUpDown"/> class.
        /// </summary>
        public TimeUpDown()
        {
            DefaultStyleKey = typeof(TimeUpDown);

            _timeCoercionHelper = new TimeCoercionHelper(this);
        }

        /// <summary>
        /// Builds the visual tree for the TimeUpDown control when a new
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            if (Text != null)
            {
                Text.SelectionChanged -= SelectionChanged;
                Text.TextChanged -= InputChanged;
                Text.LostFocus -= OnTextLostFocus;
            }
            base.OnApplyTemplate();
            if (Text != null)
            {
                Text.SelectionChanged += SelectionChanged;
                Text.TextChanged += InputChanged;
                Text.LostFocus += OnTextLostFocus;
            }

            TimeHintPopupPart = GetTemplateChild(ElementTimeHintPopupName) as Popup;

            SetValidSpinDirection();
        }

        /// <summary>
        /// Provides handling for the ValueChanging event.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanging(RoutedPropertyChangingEventArgs<DateTime?> e)
        {
            if (_ignoreValueChange)
            {
                e.InCoercion = true;
                OnValueChanged(new RoutedPropertyChangedEventArgs<DateTime?>(e.OldValue, e.NewValue));
                return;
            }

            // change is from value itself.
            bool success = _timeCoercionHelper.CoerceValue(e.OldValue, e.NewValue);

            if (success)
            {
                e.InCoercion = false;
                e.NewValue = Value;
                base.OnValueChanging(e);
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
        protected override void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            base.OnValueChanged(e);

            // preselect all text, so that when we tab into the control, everything is selected
            if (FocusManager.GetFocusedElement() != Text)
            {
                _isIgnoreSelectionOfAllText = false;
                SelectAllText();
                _isIgnoreSelectionOfAllText = true;
            }

            SetValidSpinDirection();

            TimeUpDownAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as TimeUpDownAutomationPeer;
            if (peer != null)
            {
                // automation peer cannot handle nulls.
                string oldFormatted = ActualTimeGlobalizationInfo.FormatTime(e.OldValue, ActualFormat);
                string newFormatted = ActualTimeGlobalizationInfo.FormatTime(e.NewValue, ActualFormat);
                peer.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldFormatted, newFormatted);
            }
        }

        /// <summary>
        /// Called by ApplyValue to parse user input.
        /// </summary>
        /// <param name="text">User input.</param>
        /// <returns>Value parsed from user input.</returns>
        protected override DateTime? ParseValue(string text)
        {
            DateTime? parsed = ActualTimeGlobalizationInfo.ParseTime(text, ActualFormat, TimeParsers);
            if (parsed.HasValue && Value.HasValue)
            {
                // use the datepart of Value, and the timepart of parsed.
                return Value.Value.Date.Add(parsed.Value.TimeOfDay);
            }

            return parsed;
        }

        /// <summary>
        /// Renders the value property into the textbox text.
        /// </summary>
        /// <returns>Formatted Value.</returns>
        protected internal override string FormatValue()
        {
            _lastParsedText = ActualTimeGlobalizationInfo.FormatTime(Value, ActualFormat);
            return _lastParsedText;
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected override void OnIncrement()
        {
            if (Text != null)
            {
                int caretPosition = Text.SelectionStart;

                TimeSpan ts = ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(Text.Text, caretPosition, ActualFormat);

                // new value could be on a new day.
                DateTime newValue = ActualTimeGlobalizationInfo.OnIncrement(Value.Value, ts);

                if (newValue > ActualMaximum)
                {
                    if (IsCyclic)
                    {
                        // if there is no maximum set, we can skip over it
                        // otherwise set the time to the minimum
                        if (Maximum.HasValue)
                        {
                            newValue = ActualMinimum;
                        }
                    }
                    else
                    {
                        newValue = ActualMaximum;
                    }
                }

                Value = newValue;

                // check if old caretPosition is still valid for this TimeSpan
                if (ts == ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(Text.Text, caretPosition, ActualFormat))
                {
                    Text.SelectionStart = caretPosition;
                }
                else
                {
                    int newCaretPosition = ActualTimeGlobalizationInfo.GetTextPositionForTimeUnit(Text.Text, ts, ActualFormat);
                    Text.SelectionStart = newCaretPosition > -1 ? newCaretPosition : caretPosition;
                }
            }
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected override void OnDecrement()
        {
            if (Text != null)
            {
                int caretPosition = Text.SelectionStart;

                TimeSpan ts = ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(Text.Text, caretPosition, ActualFormat);

                // new value could be on a new day.
                DateTime newValue = ActualTimeGlobalizationInfo.OnDecrement(Value.Value, ts);

                if (newValue < ActualMinimum)
                {
                    if (IsCyclic)
                    {
                        // if there is no maximum set, we can skip over it
                        // otherwise set the time to the minimum
                        if (Minimum.HasValue)
                        {
                            newValue = ActualMaximum;
                        }
                    }
                    else
                    {
                        newValue = ActualMinimum;
                    }
                }

                Value = newValue;

                // check if old caretPosition is still valid for this TimeSpan
                if (ts == ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(Text.Text, caretPosition, ActualFormat))
                {
                    Text.SelectionStart = caretPosition;
                }
                else
                {
                    int newCaretPosition = ActualTimeGlobalizationInfo.GetTextPositionForTimeUnit(Text.Text, ts, ActualFormat);
                    Text.SelectionStart = newCaretPosition > -1 ? newCaretPosition : caretPosition;
                }
            }
        }

        /// <summary>
        /// Sets the valid spin direction based on the position of the caret,
        /// the value and the minimum and maximum.
        /// </summary>
        private void SetValidSpinDirection()
        {
            ValidSpinDirections spinDirections = ValidSpinDirections.None;

            if (Text != null && FocusManager.GetFocusedElement() == Text && !string.IsNullOrEmpty(Text.Text))
            {
                DateTime? parsed = null;
                try
                {
                    parsed = ParseValue(Text.Text);
                }
                catch (ArgumentException)
                {
                    // swallow
                }

                if (!parsed.HasValue)
                {
                    // are not able to parse current input to something nice.
                    spinDirections = ValidSpinDirections.None;
                }
                else
                {
                    if (IsCyclic)
                    {
                        spinDirections = ValidSpinDirections.Decrease | ValidSpinDirections.Increase;
                    }
                    else
                    {
                        TimeSpan span = ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(
                            Text.Text,
                            Text.SelectionStart,
                            ActualFormat);

                        if (ActualTimeGlobalizationInfo.OnIncrement(parsed.Value, span) <= ActualMaximum)
                        {
                            spinDirections = spinDirections | ValidSpinDirections.Increase;
                        }

                        if (ActualTimeGlobalizationInfo.OnDecrement(parsed.Value, span) >= ActualMinimum)
                        {
                            spinDirections = spinDirections | ValidSpinDirections.Decrease;
                        }
                    }
                }
            }
            if (Spinner != null)
            {
                Spinner.ValidSpinDirection = spinDirections;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event from TextBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> 
        /// instance containing the event data.</param>
        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetValidSpinDirection();
        }

        /// <summary>
        /// Handles the TextChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void InputChanged(object sender, TextChangedEventArgs e)
        {
            DetermineHint();
        }

        /// <summary>
        /// Determines the value of the hint property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Follows the pattern where parsing may fail.")]
        private void DetermineHint()
        {
            // when in a designer, ignore this logic.
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            // possibly called while textbox does not have focus. Remove the TimeHint.
            if (FocusManager.GetFocusedElement() != Text)
            {
                IsShowTimeHint = false;
                return;
            }

            // might show a different date as a guide. However, check if that is within range.
            Func<DateTime, DateTime> CoerceDate = time =>
                                                      {
                                                          if (time.TimeOfDay > ActualMaximum.TimeOfDay)
                                                          {
                                                              return ActualMaximum;
                                                          }
                                                          if (time.TimeOfDay < ActualMinimum.TimeOfDay)
                                                          {
                                                              return ActualMinimum;
                                                          }
                                                          return time;
                                                      };
            DateTime fallbackTime = CoerceDate(DateTime.Now);

            // prepare custom parsing hook
            UpDownParsingEventArgs<DateTime?> parsingArgs = new UpDownParsingEventArgs<DateTime?>(Text.Text);

            DateTime? parsed;
            if (!ActualTimeGlobalizationInfo.TryParseTime(Text.Text, ActualFormat, TimeParsers, out parsed))
            {
                // unable to parse, default to showing DateTime.Now as guide.

                // allow custom parsing, with an empty Value.
                try
                {
                    OnParsing(parsingArgs);

                    if (parsingArgs.Handled && parsingArgs.Value.HasValue)
                    {
                        // will only use custom parsing if parsed to non-null.
                        fallbackTime = CoerceDate(parsingArgs.Value.Value);
                    }
                }
                catch (Exception)
                {
                    // swallow any exception during custom parsing.
                }

                IsShowTimeHint = true;
                VisualStateManager.GoToState(this, InvalidTimeStateName, true);

                // the date that will be shown in the TimeHint.
                _timeHintDate = ActualTimeGlobalizationInfo.FormatTime(fallbackTime, ActualFormat);
                // show the date, correctly formatted and watermarked
                TimeHintContent = string.Format(CultureInfo.InvariantCulture, TimeHintFormat, _timeHintDate);
            }
            else
            {
                // allow custom parsing using our pre parsed value.
                parsingArgs.Value = parsed;
                try
                {
                    OnParsing(parsingArgs);

                    // if parsing was successful, use that value
                    if (parsingArgs.Handled)
                    {
                        parsed = parsingArgs.Value;
                    }
                }
                catch (Exception)
                {
                    // swallow any exception during custom parsing.
                }

                if (parsed.HasValue)
                {
                    // we were able to parse, does the value fall within range?
                    if ((parsed.Value.TimeOfDay > ActualMaximum.TimeOfDay) ||
                        (parsed.Value.TimeOfDay < ActualMinimum.TimeOfDay))
                    {
                        // value does not fall within range.
                        IsShowTimeHint = true;
                        VisualStateManager.GoToState(this, InvalidTimeStateName, true);

                        // there is a better fallback time available though
                        fallbackTime = CoerceDate(parsed.Value);

                        // the date that will be shown in the TimeHint.
                        _timeHintDate = ActualTimeGlobalizationInfo.FormatTime(fallbackTime, ActualFormat);
                        // show the date, correctly formatted and watermarked
                        TimeHintContent = string.Format(CultureInfo.InvariantCulture, TimeHintFormat, _timeHintDate);
                    }
                    else
                    {
                        // value falls within range
                        // the date that will be shown in the TimeHint.
                        _timeHintDate = ActualTimeGlobalizationInfo.FormatTime(parsed.Value, ActualFormat);

                        // show TimeHint if the formatted date does not exactly match input.
                        IsShowTimeHint = !Text.Text.Equals(_timeHintDate, StringComparison.OrdinalIgnoreCase);
                        VisualStateManager.GoToState(this, ValidTimeStateName, true);
                        TimeHintContent = _timeHintDate;
                    }
                }
                else
                {
                    // parsed to a null
                    IsShowTimeHint = true;

                    // the date that will be shown in the TimeHint.
                    _timeHintDate = ActualTimeGlobalizationInfo.FormatTime(fallbackTime, ActualFormat);

                    VisualStateManager.GoToState(this, EmptyTimeStateName, true);
                    TimeHintContent = string.Format(CultureInfo.InvariantCulture, TimeHintFormat, _timeHintDate);
                }
            }
        }

        /// <summary>
        /// Handles the Left Mouse Button Down event of the TimeHint.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> 
        /// instance containing the event data.</param>
        private void OnTimeHintMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsShowTimeHint = false;

            // will commit value in TimeHint, if possible, using the exact string
            // that was used in the TimeHint.
            ApplyValue(_timeHintDate);
        }

        /// <summary>
        /// Provides handling for the KeyDown event.
        /// </summary>
        /// <param name="e">Key event args.</param>
        /// <remarks>
        /// Only support up and down arrow keys.
        /// </remarks>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsEditable == false)
            {
                e.Handled = true;
                return;
            }

            if (Text.Text != _lastParsedText)
            {
#if MIGRATION
                if (e.Key == Key.Up || e.Key == Key.Down)
#else
                if (e.Key == VirtualKey.Up || e.Key == VirtualKey.Down)
#endif
                {
                    int caretPosition = Text.SelectionStart;
                    ApplyValue(Text.Text);
                    e.Handled = true;
                    if (caretPosition >= 0 && caretPosition < Text.Text.Length)
                    {
                        // there are situations where the caretposition
                        // is not correct. However, this is the 99% case.
                        Text.SelectionStart = caretPosition;
#if MIGRATION
                        if (e.Key == Key.Up)
#else
                        if (e.Key == VirtualKey.Up)
#endif
                        {
                            OnIncrement();
                        }
                        else
                        {
                            OnDecrement();
                        }
                    }
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Provides handling for the GotFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowGotFocus(e))
            {
                DetermineHint();
                SetValidSpinDirection();
                Interaction.OnGotFocusBase();
                base.OnGotFocus(e);
            }
        }

        /// <summary>
        /// Provides handling for the LostFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowLostFocus(e))
            {
                IsShowTimeHint = false;
                SetValidSpinDirection();
                Interaction.OnLostFocusBase();
                base.OnLostFocus(e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseEnter event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected override void OnPointerEntered(MouseEventArgs e)
#endif
        {
            if (Interaction.AllowMouseEnter(e))
            {
                Interaction.OnMouseEnterBase();
#if MIGRATION
                base.OnMouseEnter(e);
#else
                base.OnPointerEntered(e);
#endif
            }
        }

        /// <summary>
        /// Provides handling for the MouseLeave event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected internal override void OnMouseLeave(MouseEventArgs e)
#else
        protected internal override void OnPointerExited(MouseEventArgs e)
#endif
        {
            if (Interaction.AllowMouseLeave(e))
            {
                Interaction.OnMouseLeaveBase();
#if MIGRATION
                base.OnMouseLeave(e);
#else
                base.OnPointerExited(e);
#endif
            }
        }

        /// <summary>
        /// Provides handling for the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(MouseButtonEventArgs e)
#endif
        {
            if (Interaction.AllowMouseLeftButtonDown(e))
            {
                Interaction.OnMouseLeftButtonDownBase();
#if MIGRATION
                base.OnMouseLeave(e);
#else
                base.OnPointerPressed(e);
#endif
            }
        }

        /// <summary>
        /// Called before the MouseLeftButtonUp event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(MouseButtonEventArgs e)
#endif
        {
            if (Interaction.AllowMouseLeftButtonUp(e))
            {
                Interaction.OnMouseLeftButtonUpBase();
#if MIGRATION
                base.OnMouseLeftButtonUp(e);
#else
                base.OnPointerReleased(e);
#endif
            }
        }

#region TextSelection and tabbing behavior
        /// <summary>
        /// Selects all text.
        /// </summary>
        protected override void SelectAllText()
        {
            // TimeUpDown only selects all text when coming in from a tab.
            if (!_isIgnoreSelectionOfAllText)
            {
                base.SelectAllText();
            }
        }

        /// <summary>
        /// Event handler for Text template part's LostFocus event.
        /// We use this event to compare current TextBox.Text with cached previous
        /// value to decide whether user has typed in a new value.
        /// </summary>
        /// <param name="sender">The Text template part.</param>
        /// <param name="e">Event args.</param>
        private void OnTextLostFocus(object sender, RoutedEventArgs e)
        {
            _isIgnoreSelectionOfAllText = false;
            SelectAllText();
            _isIgnoreSelectionOfAllText = true;
        }
#endregion TextSelection and tabbing behavior

        /// <summary>
        /// Update current visual state.
        /// </summary>
        /// <param name="useTransitions">True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.</param>
        internal override void UpdateVisualState(bool useTransitions)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (_timeHintExpandDirection == null || !IsShowTimeHint)
                {
                    VisualStateManager.GoToState(this, TimeHintClosedStateName, useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(this, _timeHintExpandDirection == ExpandDirection.Up ? TimeHintOpenedUpStateName : TimeHintOpenedDownStateName, useTransitions);
                }
            }

            // handle common states
            base.UpdateVisualState(useTransitions);
        }

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
        /// When implemented in a derived class, returns class-specific 
        /// <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations 
        /// for the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// The class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> 
        /// subclass to return.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TimeUpDownAutomationPeer(this);
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