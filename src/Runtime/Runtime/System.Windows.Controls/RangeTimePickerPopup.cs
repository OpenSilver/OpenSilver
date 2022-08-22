// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a time picker popup that allows choosing time through 3 
    /// sliders: Hours, Minutes and seconds.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = ContainedStateName, GroupName = ContainedByPickerGroupName)]
    [TemplateVisualState(Name = NotContainedStateName, GroupName = ContainedByPickerGroupName)]

    [TemplateVisualState(Name = AllowSecondsAndDesignatorsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = AllowTimeDesignatorsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = AllowSecondsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = HoursAndMinutesOnlyStateName, GroupName = PopupModeGroupName)]

    [TemplatePart(Name = CommitButtonPartName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = CancelButtonPartName, Type = typeof(ButtonBase))]

    [TemplatePart(Name = HoursContainerPartName, Type = typeof(Panel))]
    [TemplatePart(Name = MinutesContainerPartName, Type = typeof(Panel))]
    [TemplatePart(Name = SecondsContainerPartName, Type = typeof(Panel))]
    [TemplatePart(Name = HoursSliderPartName, Type = typeof(RangeBase))]
    [TemplatePart(Name = MinutesSliderPartName, Type = typeof(RangeBase))]
    [TemplatePart(Name = SecondsSliderPartName, Type = typeof(RangeBase))]
    [StyleTypedProperty(Property = "SliderStyle", StyleTargetType = typeof(RangeBase))]
    [StyleTypedProperty(Property = "TimeButtonStyle", StyleTargetType = typeof(Button))]
    public class RangeTimePickerPopup : TimePickerPopup
    {
#region TemplatePartNames
        /// <summary>
        /// The HoursSliderPartName.
        /// </summary>
        private const string HoursSliderPartName = "HoursSlider";

        /// <summary>
        /// The MinutesSliderPartName.
        /// </summary>
        private const string MinutesSliderPartName = "MinutesSlider";

        /// <summary>
        /// The SecondsSliderPartName.
        /// </summary>
        private const string SecondsSliderPartName = "SecondsSlider";

        /// <summary>
        /// The HoursLabelsPartName.
        /// </summary>
        private const string HoursContainerPartName = "HoursPanel";

        /// <summary>
        /// The MinutesLabelsPartName.
        /// </summary>
        private const string MinutesContainerPartName = "MinutesPanel";

        /// <summary>
        /// The SecondsLabelsPartName.
        /// </summary>
        private const string SecondsContainerPartName = "SecondsPanel";

        /// <summary>
        /// The name of the CommitButton TemplatePart.
        /// </summary>
        private const string CommitButtonPartName = "Commit";

        /// <summary>
        /// The name of the CancelButton TemplatePart.
        /// </summary>
        private const string CancelButtonPartName = "Cancel";
#endregion

#region TemplateParts

        /// <summary>
        /// Gets or sets the seconds slider Part.
        /// </summary>
        private RangeBase SecondsSlider
        {
            get { return _secondsSlider; }
            set
            {
                if (_secondsSlider != null)
                {
                    _secondsSlider.ValueChanged -= SecondsChanged;
                    _secondsSlider.SizeChanged -= SliderSizeChange;
                }

                _secondsSlider = value;

                if (_secondsSlider != null)
                {
                    _secondsSlider.ValueChanged += SecondsChanged;
                    _secondsSlider.SizeChanged += SliderSizeChange;
                }
            }
        }

        /// <summary>
        /// BackingField for the SecondsSlider.
        /// </summary>
        private RangeBase _secondsSlider;

        /// <summary>
        /// Gets or sets the minutes slider.
        /// </summary>
        private RangeBase MinutesSlider
        {
            get { return _minutesSlider; }
            set
            {
                if (_minutesSlider != null)
                {
                    _minutesSlider.ValueChanged -= MinutesChanged;
                    _minutesSlider.SizeChanged -= SliderSizeChange;
                }

                _minutesSlider = value;

                if (_minutesSlider != null)
                {
                    _minutesSlider.ValueChanged += MinutesChanged;
                    _minutesSlider.SizeChanged += SliderSizeChange;
                }
            }
        }

        /// <summary>
        /// BackingField for the MinutesSlider.
        /// </summary>
        private RangeBase _minutesSlider;

        /// <summary>
        /// Gets or sets the HoursSlider.
        /// </summary>
        private RangeBase HoursSlider
        {
            get { return _hoursSlider; }
            set
            {
                if (_hoursSlider != null)
                {
                    _hoursSlider.ValueChanged -= HoursChanged;
                    _hoursSlider.SizeChanged -= SliderSizeChange;
                }

                _hoursSlider = value;

                if (_hoursSlider != null)
                {
                    _hoursSlider.ValueChanged += HoursChanged;
                    _hoursSlider.SizeChanged += SliderSizeChange;
                }
            }
        }

        /// <summary>
        /// BackingField for the HoursSlider.
        /// </summary>
        private RangeBase _hoursSlider;

        /// <summary>
        /// Gets or sets the hours labels.
        /// </summary>
        /// <value>The hours labels.</value>
        private Panel HoursContainer { get; set; }

        /// <summary>
        /// Gets or sets the minute labels.
        /// </summary>
        /// <value>The minute labels.</value>
        private Panel MinutesContainer { get; set; }

        /// <summary>
        /// Gets or sets the second labels.
        /// </summary>
        /// <value>The second labels.</value>
        private Panel SecondsContainer { get; set; }

        /// <summary>
        /// Gets or sets the cancel button part.
        /// </summary>
        /// <value>The cancel button part.</value>
        private ButtonBase CancelButtonPart
        {
            get { return _cancelButtonPart; }
            set
            {
                if (_cancelButtonPart != null)
                {
                    _cancelButtonPart.Click -= OnCancel;
                }
                _cancelButtonPart = value;
                if (_cancelButtonPart != null)
                {
                    _cancelButtonPart.Click += OnCancel;
                }
            }
        }

        /// <summary>
        /// BackingField for CancelButtonPart.
        /// </summary>
        private ButtonBase _cancelButtonPart;

        /// <summary>
        /// Gets or sets the commit button part.
        /// </summary>
        private ButtonBase CommitButtonPart
        {
            get { return _commitButtonPart; }
            set
            {
                if (_commitButtonPart != null)
                {
                    _commitButtonPart.Click -= OnCommit;
                }
                _commitButtonPart = value;
                if (_commitButtonPart != null)
                {
                    _commitButtonPart.Click += OnCommit;
                }
            }
        }

        /// <summary>
        /// BackingField for CommitButtonPart.
        /// </summary>
        private ButtonBase _commitButtonPart;
#endregion

        /// <summary>
        /// Gets a value indicating whether this instance is currently open.
        /// </summary>
        /// <value><c>True</c> if this instance is currently open; 
        /// otherwise, <c>false</c>.</value>
        private bool IsCurrentlyOpen
        {
            get
            {
                if (TimePickerParent == null)
                {
                    return true;
                }
                else
                {
                    return _isOpenedByContainer;
                }
            }
        }

        /// <summary>
        /// Is set when opened or closed by a container.
        /// </summary>
        private bool _isOpenedByContainer;

        /// <summary>
        /// Determines whether the control should ignore the changes in its 
        /// sliders.
        /// </summary>
        private bool _ignoreSliderChange;

#region public Style SliderStyle
        /// <summary>
        /// Gets or sets the Style applied to the sliders in the 
        /// RangeTimePickerPopup control.
        /// </summary>
        public Style SliderStyle
        {
            get { return GetValue(SliderStyleProperty) as Style; }
            set { SetValue(SliderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the SliderStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty SliderStyleProperty =
            DependencyProperty.Register(
                "SliderStyle",
                typeof(Style),
                typeof(RangeTimePickerPopup),
                new PropertyMetadata(null, OnSliderStylePropertyChanged));

        /// <summary>
        /// SliderStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">RangeTimePickerPopup that changed its SliderStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSliderStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
#endregion public Style SliderStyle

#region public Style TimeButtonStyle
        /// <summary>
        /// Gets or sets the Style applied to the buttons that represent
        /// hours, minutes and seconds.
        /// </summary>
        public Style TimeButtonStyle
        {
            get { return GetValue(TimeButtonStyleProperty) as Style; }
            set { SetValue(TimeButtonStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TimeButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeButtonStyleProperty =
            DependencyProperty.Register(
                "TimeButtonStyle",
                typeof(Style),
                typeof(RangeTimePickerPopup),
                new PropertyMetadata(null, OnTimeButtonStylePropertyChanged));

        /// <summary>
        /// TimeButtonStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">RangeTimePickerPopup that changed its TimeButtonStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
#endregion public Style TimeButtonStyle

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTimePickerPopup"/> class.
        /// </summary>
        public RangeTimePickerPopup()
        {
            DefaultStyleKey = typeof(RangeTimePickerPopup);
        }

        /// <summary>
        /// Builds the visual tree for the RangeTimePickerPopup control when a 
        /// new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            HoursSlider = GetTemplateChild(HoursSliderPartName) as RangeBase;
            MinutesSlider = GetTemplateChild(MinutesSliderPartName) as RangeBase;
            SecondsSlider = GetTemplateChild(SecondsSliderPartName) as RangeBase;

            HoursContainer = GetTemplateChild(HoursContainerPartName) as Panel;
            MinutesContainer = GetTemplateChild(MinutesContainerPartName) as Panel;
            SecondsContainer = GetTemplateChild(SecondsContainerPartName) as Panel;

            CommitButtonPart = GetTemplateChild(CommitButtonPartName) as ButtonBase;
            CancelButtonPart = GetTemplateChild(CancelButtonPartName) as ButtonBase;

            GenerateLabels();
        }

#region Reactive
        /// <summary>
        /// Called when a slider changes size.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void SliderSizeChange(object sender, SizeChangedEventArgs e)
        {
            LayoutLabels();
        }

        /// <summary>
        /// Reacts to a change in the Seconds Slider.
        /// </summary>
        /// <param name="sender">The Slider that changed its value.</param>
        /// <param name="e">The instance containing the event data.</param>
#if MIGRATION
        private void SecondsChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
#else
        private void SecondsChanged(object sender, RangeBaseValueChangedEventArgs e)
#endif
        {
            if (_ignoreSliderChange)
            {
                return;
            }

            DateTime sliderValue = GetValueFromSliders();

            // restore the old value (snapping behavior)
            _ignoreSliderChange = true;
            SecondsSlider.Value = e.OldValue;
            _ignoreSliderChange = false;

            // setting value to the snapped time will set sliders accordingly.
            Value = GetCoercedValue(new DateTime(
                                        sliderValue.Year,
                                        sliderValue.Month,
                                        sliderValue.Day,
                                        sliderValue.Hour,
                                        sliderValue.Minute,
                                        GetSnappedValue(e.NewValue, 0, 59, Math.Max(1, PopupSecondsInterval))));
        }

        /// <summary>
        /// Reacts to a change in the Minutes Slider.
        /// </summary>
        /// <param name="sender">The Slider that changed its value.</param>
        /// <param name="e">The instance containing the event data.</param>        
#if MIGRATION
        private void MinutesChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
#else
        private void MinutesChanged(object sender, RangeBaseValueChangedEventArgs e)
#endif
        {
            if (_ignoreSliderChange)
            {
                return;
            }

            DateTime sliderValue = GetValueFromSliders();

            // restore the old value (snapping behavior)
            _ignoreSliderChange = true;
            MinutesSlider.Value = e.OldValue;
            _ignoreSliderChange = false;

            // setting value to the snapped time will set sliders accordingly.
            Value = GetCoercedValue(new DateTime(
                                        sliderValue.Year,
                                        sliderValue.Month,
                                        sliderValue.Day,
                                        sliderValue.Hour,
                                        GetSnappedValue(e.NewValue, 0, 59, Math.Max(1, PopupMinutesInterval)),
                                        sliderValue.Second));
        }

        /// <summary>
        /// Reacts to a change in the Hours Slider.
        /// </summary>
        /// <param name="sender">The Slider that changed its value.</param>
        /// <param name="e">The instance containing the event data.</param>
#if MIGRATION
        private void HoursChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
#else
        private void HoursChanged(object sender, RangeBaseValueChangedEventArgs e)
#endif
        {
            if (_ignoreSliderChange)
            {
                return;
            }

            DateTime sliderValue = GetValueFromSliders();

            // restore the old value (snapping behavior)
            _ignoreSliderChange = true;
            HoursSlider.Value = e.OldValue;
            _ignoreSliderChange = false;

            // setting value to the snapped time will set sliders accordingly.
            Value = GetCoercedValue(new DateTime(
                                        sliderValue.Year,
                                        sliderValue.Month,
                                        sliderValue.Day,
                                        GetSnappedValue(e.NewValue, 0, 23, 1),
                                        sliderValue.Minute,
                                        sliderValue.Second));
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            base.OnValueChanged(e);

            if (!IsCurrentlyOpen)
            {
                return;
            }

            if (e.NewValue != null)
            {
                SetSlidersToValue(e.NewValue.Value);
                SetEnabledStatusOnLabels();
            }
        }

        /// <summary>
        /// Called when the TimePicker control has opened this popup.
        /// </summary>
        /// <remarks>Called before the TimePicker reacts to value changes.
        /// This is done so that the Popup can 'snap' to a specific value without
        /// changing the selected value in the TimePicker.</remarks>
        public override void OnOpened()
        {
            base.OnOpened();

            _isOpenedByContainer = true;

            GenerateLabels();

            if (Value.HasValue)
            {
                SetSlidersToValue(Value.Value);
            }
        }

        /// <summary>
        /// Called when the TimePicker control has closed this popup.
        /// </summary>
        public override void OnClosed()
        {
            base.OnClosed();

            _isOpenedByContainer = false;
        }

        /// <summary>
        /// Called by the commit button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCommit(object sender, RoutedEventArgs e)
        {
            DoCommit();
        }

        /// <summary>
        /// Called by the cancel button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance 
        /// containing the event data.</param>
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DoCancel();
        }
#endregion

#region Proactive
        /// <summary>
        /// Calculates the value based on the sliders.
        /// </summary>
        /// <returns>The DateTime as represented by the current values in the 
        /// Sliders.</returns>
        private DateTime GetValueFromSliders()
        {
            DateTime returnTime = Value.HasValue ? Value.Value.Date : DateTime.Today.Date;

            if (HoursSlider == null || MinutesSlider == null || SecondsSlider == null)
            {
                return returnTime;
            }

            return new DateTime(
                returnTime.Year,
                returnTime.Month,
                returnTime.Day,
                (int)HoursSlider.Value,
                (int)MinutesSlider.Value,
                (int)SecondsSlider.Value);
        }

        /// <summary>
        /// Sets the sliders to value.
        /// </summary>
        /// <param name="value">The DateTime that needs to be reflected by
        /// the three sliders.</param>
        private void SetSlidersToValue(DateTime value)
        {
            _ignoreSliderChange = true;
            try
            {
                if (HoursSlider != null)
                {
                    HoursSlider.Value = value.Hour;
                }
                if (MinutesSlider != null)
                {
                    MinutesSlider.Value = value.Minute;
                }
                if (SecondsSlider != null)
                {
                    SecondsSlider.Value = value.Second;
                }
            }
            finally
            {
                _ignoreSliderChange = false;
            }
        }
#endregion

#region label layout
        /// <summary>
        /// Generates the labels.
        /// </summary>
        private void GenerateLabels()
        {
            Action<UIElement> removeHandler = element =>
                                                  {
                                                      Button b = element as Button;
                                                      if (b != null)
                                                      {
                                                          b.Click -= OnLabelClicked;
                                                      }
                                                  };

            // create the hour labels. These are not based on interval.
            if (HoursContainer != null)
            {
                HoursContainer.Children.ForEach(removeHandler);
                HoursContainer.Children.Clear();
                EnumerableExtensions.Range(22, 0, 2)
                    .ForEach(hour => HoursContainer.Children.Add(
                        CreateLabelElement(
                        ActualTimeGlobalizationInfo.FormatTime(
                                DateTime.MinValue.AddHours(hour),
                                ActualFormat,
                                'h',
                                'H',
                                't',
                                ' '),
                                TimeSpan.FromHours(hour))));
            }

            // create the hour labels. These are not based on interval.
            if (MinutesContainer != null)
            {
                MinutesContainer.Children.ForEach(removeHandler);
                MinutesContainer.Children.Clear();
                EnumerableExtensions.Range(55, 0, 5)
                    .ForEach(minute => MinutesContainer.Children.Add(
                        CreateLabelElement(
                            ActualTimeGlobalizationInfo.FormatTime(
                            DateTime.MinValue.AddMinutes(minute),
                            new CustomTimeFormat("mm")),
                             TimeSpan.FromMinutes(minute))));
            }

            // create the hour labels. These are not based on interval.
            if (SecondsContainer != null)
            {
                SecondsContainer.Children.ForEach(removeHandler);
                SecondsContainer.Children.Clear();
                EnumerableExtensions.Range(55, 0, 5)
                    .ForEach(second => SecondsContainer.Children.Add(
                        CreateLabelElement(
                            ActualTimeGlobalizationInfo.FormatTime(
                            DateTime.MinValue.AddSeconds(second),
                            new CustomTimeFormat("ss")),
                            TimeSpan.FromSeconds(second))));
            }

            UpdateLayout();
            LayoutLabels();
            SetEnabledStatusOnLabels();
        }

        /// <summary>
        /// Sets the enabled status on the labels.
        /// </summary>
        private void SetEnabledStatusOnLabels()
        {
            Func<DateTime, bool> CalculateEnabledStatus = time =>
                                                              {
                                                                  if (Minimum.HasValue && time.TimeOfDay < Minimum.Value.TimeOfDay)
                                                                  {
                                                                      return false;
                                                                  }
                                                                  if (Maximum.HasValue && time.TimeOfDay > Maximum.Value.TimeOfDay)
                                                                  {
                                                                      return false;
                                                                  }
                                                                  return true;
                                                              };

            if (HoursSlider == null || MinutesSlider == null || SecondsSlider == null)
            {
                // if there is no slider, there wouldn't be labels.
                return;
            }

            if (HoursContainer != null)
            {
                foreach (Button button in HoursContainer.Children)
                {
                    TimeSpan ts = (TimeSpan)button.Tag;
                    DateTime dt = new DateTime(
                        1900,
                        1,
                        1,
                        ts.Hours,
                        (int)MinutesSlider.Value,
                        (int)SecondsSlider.Value);

                    button.IsEnabled = CalculateEnabledStatus(dt);
                }
            }
            if (MinutesContainer != null)
            {
                foreach (Button button in MinutesContainer.Children)
                {
                    TimeSpan ts = (TimeSpan)button.Tag;
                    DateTime dt = new DateTime(
                        1900,
                        1,
                        1,
                        (int)HoursSlider.Value,
                        ts.Minutes,
                        (int)SecondsSlider.Value);

                    button.IsEnabled = CalculateEnabledStatus(dt);
                }
            }
            if (SecondsContainer != null)
            {
                foreach (Button button in SecondsContainer.Children)
                {
                    TimeSpan ts = (TimeSpan)button.Tag;
                    DateTime dt = new DateTime(
                        1900,
                        1,
                        1,
                        (int)HoursSlider.Value,
                        (int)MinutesSlider.Value,
                        ts.Seconds);

                    button.IsEnabled = CalculateEnabledStatus(dt);
                }
            }
        }

        /// <summary>
        /// Arranges the time labels on the RangeTimePickerPopup control.
        /// </summary>
        /// <remarks>Uses a canvas to layout labels vertically.</remarks>
        protected virtual void LayoutLabels()
        {
            // some magic numbers going on to layout the labels in such a way
            // that it looks good to human eyes.

            // the positioning here is done with a canvas which doesn't
            // report back a correct size. In the future, this should be
            // refactored into a custom panel or a better solution.

            Action<double, Panel, Func<TimeSpan, double>> PositionItems = (height, canvas, calc) =>
            {
                int count = canvas.Children.Count;
                double heightPerItem = height / count;
                double offset = heightPerItem / 3;

                for (int i = 0; i < count; i++)
                {
                    FrameworkElement item = canvas.Children[i] as FrameworkElement;
                    if (item != null && item.Tag is TimeSpan)
                    {
                        TimeSpan ts = (TimeSpan)item.Tag;
                        double percentage = calc(ts);
                        item.Margin = new Thickness(0, height * percentage - offset * percentage - offset, 0, 0);
                    }
                }
            };

            if (HoursSlider != null && HoursContainer != null && HoursContainer.Children.Count > 0)
            {
                PositionItems(HoursSlider.ActualHeight, HoursContainer, span => (23 - span.Hours) / 23.0);
            }
            if (MinutesSlider != null && MinutesContainer != null && MinutesContainer.Children.Count > 0)
            {
                PositionItems(MinutesSlider.ActualHeight, MinutesContainer, span => (59 - span.Minutes) / 59.0);
            }
            if (SecondsSlider != null && SecondsContainer != null && SecondsContainer.Children.Count > 0)
            {
                PositionItems(SecondsSlider.ActualHeight, SecondsContainer, span => (59 - span.Seconds) / 59.0);
            }
        }

        /// <summary>
        /// Called when a label is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLabelClicked(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            TimeSpan ts = (TimeSpan)b.Tag;
            DateTime dt = GetValueFromSliders();

            if (HoursContainer != null && HoursContainer.Children.Contains(b))
            {
                Value = new DateTime(dt.Year, dt.Month, dt.Day, ts.Hours, dt.Minute, dt.Second);
            }
            else if (MinutesContainer != null && MinutesContainer.Children.Contains(b))
            {
                Value = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, ts.Minutes, dt.Second);
            }
            else if (SecondsContainer != null && SecondsContainer.Children.Contains(b))
            {
                Value = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, ts.Seconds);
            }
        }
#endregion

#region Helpers
        /// <summary>
        /// Gets the value after Minimum and Maximum coercion.
        /// </summary>
        /// <param name="time">The input.</param>
        /// <returns>Time between Minimum and Maximum.</returns>
        /// <remarks>Done to avoid coercion in containing controls.</remarks>
        private DateTime GetCoercedValue(DateTime time)
        {
            if (Minimum.HasValue && time.TimeOfDay < Minimum.Value.TimeOfDay)
            {
                return Minimum.Value;
            }
            else if (Maximum.HasValue && time.TimeOfDay > Maximum.Value.TimeOfDay)
            {
                return Maximum.Value;
            }
            else
            {
                return time;
            }
        }

        /// <summary>
        /// Gets the coerced value, using interval and a minimum and maximum.
        /// </summary>
        /// <param name="value">The value that will be snapped.</param>
        /// <param name="minimum">The minimum the value may be.</param>
        /// <param name="maximum">The maximum the value may be.</param>
        /// <param name="interval">The interval to which the value gets snapped.</param>
        /// <returns>A value that is within range and snapped to an interval.</returns>
        private static int GetSnappedValue(double value, int minimum, int maximum, int interval)
        {
            if (value < minimum)
            {
                return minimum;
            }
            if (value > maximum)
            {
                return maximum;
            }

            // rounded
            int tick = (int)value / interval;

            int start = tick * interval;
            double halfpoint = start + interval / 2;

            return value > halfpoint ? Math.Min(start + interval, maximum) : Math.Max(start, minimum);
        }

        /// <summary>
        /// Creates the element for a label.
        /// </summary>
        /// <param name="text">The text that is set as content.</param>
        /// <param name="timespan">The TimeSpan that is represented by the Button.</param>
        /// <returns>A Button representing the label.</returns>
        private Button CreateLabelElement(string text, TimeSpan timespan)
        {
            Button c = new Button();
            c.SetBinding(
                StyleProperty,
                new Binding()
                {
                    Path = new PropertyPath("TimeButtonStyle"),
                    Source = this
                });
            c.VerticalAlignment = VerticalAlignment.Top;
            // the easiest way to pass a value. Since we manage this element
            // there is no interference possible.
            c.Tag = timespan;
            c.Content = text;
            c.Click += OnLabelClicked;
            return c;
        }
#endregion

        /// <summary>
        /// Gets the valid popup time selection modes.
        /// </summary>
        /// <returns>
        /// An array of PopupTimeSelectionModes that are supported by
        /// the Popup.
        /// </returns>
        internal override PopupTimeSelectionMode[] GetValidPopupTimeSelectionModes()
        {
            return new[]
                       {
                           PopupTimeSelectionMode.HoursAndMinutesOnly,
                           PopupTimeSelectionMode.AllowSecondsSelection
                       };
        }

        /// <summary>
        /// Creates the automation peer.
        /// </summary>
        /// <returns>The RangeTimePickerPopupAutomationPeer for this instance.</returns>
        protected override TimePickerPopupAutomationPeer CreateAutomationPeer()
        {
            return new RangeTimePickerPopupAutomationPeer(this);
        }
    }
}