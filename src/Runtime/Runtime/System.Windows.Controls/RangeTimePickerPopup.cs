
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

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

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
    [StyleTypedProperty(Property = "TimeButtonStyle", StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = "SliderStyle", StyleTargetType = typeof(RangeBase))]
    [TemplatePart(Name = "SecondsSlider", Type = typeof(RangeBase))]
    [TemplateVisualState(GroupName = "ContainedByPickerStates", Name = "NotContained")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowSecondsAndDesignatorsSelection")]
    [TemplatePart(Name = "SecondsPanel", Type = typeof(Panel))]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplateVisualState(GroupName = "ContainedByPickerStates", Name = "Contained")]
    [TemplatePart(Name = "HoursSlider", Type = typeof(RangeBase))]
    [TemplatePart(Name = "MinutesSlider", Type = typeof(RangeBase))]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowTimeDesignatorsSelection")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowSecondsSelection")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "HoursAndMinutesOnly")]
    [TemplatePart(Name = "Commit", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "Cancel", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "HoursPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "MinutesPanel", Type = typeof(Panel))]
    public class RangeTimePickerPopup : TimePickerPopup
    {
        /// <summary>Identifies the SliderStyle dependency property.</summary>
        public static readonly DependencyProperty SliderStyleProperty = DependencyProperty.Register(nameof(SliderStyle), typeof(Style), typeof(RangeTimePickerPopup), new PropertyMetadata((object)null, new PropertyChangedCallback(RangeTimePickerPopup.OnSliderStylePropertyChanged)));
        /// <summary>Identifies the TimeButtonStyle dependency property.</summary>
        public static readonly DependencyProperty TimeButtonStyleProperty = DependencyProperty.Register(nameof(TimeButtonStyle), typeof(Style), typeof(RangeTimePickerPopup), new PropertyMetadata((object)null, new PropertyChangedCallback(RangeTimePickerPopup.OnTimeButtonStylePropertyChanged)));
        /// <summary>The HoursSliderPartName.</summary>
        private const string HoursSliderPartName = "HoursSlider";
        /// <summary>The MinutesSliderPartName.</summary>
        private const string MinutesSliderPartName = "MinutesSlider";
        /// <summary>The SecondsSliderPartName.</summary>
        private const string SecondsSliderPartName = "SecondsSlider";
        /// <summary>The HoursLabelsPartName.</summary>
        private const string HoursContainerPartName = "HoursPanel";
        /// <summary>The MinutesLabelsPartName.</summary>
        private const string MinutesContainerPartName = "MinutesPanel";
        /// <summary>The SecondsLabelsPartName.</summary>
        private const string SecondsContainerPartName = "SecondsPanel";
        /// <summary>The name of the CommitButton TemplatePart.</summary>
        private const string CommitButtonPartName = "Commit";
        /// <summary>The name of the CancelButton TemplatePart.</summary>
        private const string CancelButtonPartName = "Cancel";
        /// <summary>BackingField for the SecondsSlider.</summary>
        private RangeBase _secondsSlider;
        /// <summary>BackingField for the MinutesSlider.</summary>
        private RangeBase _minutesSlider;
        /// <summary>BackingField for the HoursSlider.</summary>
        private RangeBase _hoursSlider;
        /// <summary>BackingField for CancelButtonPart.</summary>
        private ButtonBase _cancelButtonPart;
        /// <summary>BackingField for CommitButtonPart.</summary>
        private ButtonBase _commitButtonPart;
        /// <summary>Is set when opened or closed by a container.</summary>
        private bool _isOpenedByContainer;
        /// <summary>
        /// Determines whether the control should ignore the changes in its
        /// sliders.
        /// </summary>
        private bool _ignoreSliderChange;

        /// <summary>Gets or sets the seconds slider Part.</summary>
        private RangeBase SecondsSlider
        {
            get
            {
                return this._secondsSlider;
            }
            set
            {
                if (this._secondsSlider != null)
                {
                    this._secondsSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.SecondsChanged);
                    this._secondsSlider.SizeChanged -= new SizeChangedEventHandler(this.SliderSizeChange);
                }
                this._secondsSlider = value;
                if (this._secondsSlider == null)
                    return;
                this._secondsSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.SecondsChanged);
                this._secondsSlider.SizeChanged += new SizeChangedEventHandler(this.SliderSizeChange);
            }
        }

        /// <summary>Gets or sets the minutes slider.</summary>
        private RangeBase MinutesSlider
        {
            get
            {
                return this._minutesSlider;
            }
            set
            {
                if (this._minutesSlider != null)
                {
                    this._minutesSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.MinutesChanged);
                    this._minutesSlider.SizeChanged -= new SizeChangedEventHandler(this.SliderSizeChange);
                }
                this._minutesSlider = value;
                if (this._minutesSlider == null)
                    return;
                this._minutesSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.MinutesChanged);
                this._minutesSlider.SizeChanged += new SizeChangedEventHandler(this.SliderSizeChange);
            }
        }

        /// <summary>Gets or sets the HoursSlider.</summary>
        private RangeBase HoursSlider
        {
            get
            {
                return this._hoursSlider;
            }
            set
            {
                if (this._hoursSlider != null)
                {
                    this._hoursSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.HoursChanged);
                    this._hoursSlider.SizeChanged -= new SizeChangedEventHandler(this.SliderSizeChange);
                }
                this._hoursSlider = value;
                if (this._hoursSlider == null)
                    return;
                this._hoursSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HoursChanged);
                this._hoursSlider.SizeChanged += new SizeChangedEventHandler(this.SliderSizeChange);
            }
        }

        /// <summary>Gets or sets the hours labels.</summary>
        /// <value>The hours labels.</value>
        private Panel HoursContainer { get; set; }

        /// <summary>Gets or sets the minute labels.</summary>
        /// <value>The minute labels.</value>
        private Panel MinutesContainer { get; set; }

        /// <summary>Gets or sets the second labels.</summary>
        /// <value>The second labels.</value>
        private Panel SecondsContainer { get; set; }

        /// <summary>Gets or sets the cancel button part.</summary>
        /// <value>The cancel button part.</value>
        private ButtonBase CancelButtonPart
        {
            get
            {
                return this._cancelButtonPart;
            }
            set
            {
                if (this._cancelButtonPart != null)
                    this._cancelButtonPart.Click -= new RoutedEventHandler(this.OnCancel);
                this._cancelButtonPart = value;
                if (this._cancelButtonPart == null)
                    return;
                this._cancelButtonPart.Click += new RoutedEventHandler(this.OnCancel);
            }
        }

        /// <summary>Gets or sets the commit button part.</summary>
        private ButtonBase CommitButtonPart
        {
            get
            {
                return this._commitButtonPart;
            }
            set
            {
                if (this._commitButtonPart != null)
                    this._commitButtonPart.Click -= new RoutedEventHandler(this.OnCommit);
                this._commitButtonPart = value;
                if (this._commitButtonPart == null)
                    return;
                this._commitButtonPart.Click += new RoutedEventHandler(this.OnCommit);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is currently open.
        /// </summary>
        /// <value><c>True</c> if this instance is currently open;
        /// otherwise, <c>false</c>.</value>
        private bool IsCurrentlyOpen
        {
            get
            {
                return this.TimePickerParent == null || this._isOpenedByContainer;
            }
        }

        /// <summary>
        /// Gets or sets the Style applied to the sliders in the
        /// RangeTimePickerPopup control.
        /// </summary>
        public Style SliderStyle
        {
            get
            {
                return this.GetValue(RangeTimePickerPopup.SliderStyleProperty) as Style;
            }
            set
            {
                this.SetValue(RangeTimePickerPopup.SliderStyleProperty, (object)value);
            }
        }

        /// <summary>SliderStyleProperty property changed handler.</summary>
        /// <param name="d">RangeTimePickerPopup that changed its SliderStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSliderStylePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets the Style applied to the buttons that represent
        /// hours, minutes and seconds.
        /// </summary>
        public Style TimeButtonStyle
        {
            get
            {
                return this.GetValue(RangeTimePickerPopup.TimeButtonStyleProperty) as Style;
            }
            set
            {
                this.SetValue(RangeTimePickerPopup.TimeButtonStyleProperty, (object)value);
            }
        }

        /// <summary>TimeButtonStyleProperty property changed handler.</summary>
        /// <param name="d">RangeTimePickerPopup that changed its TimeButtonStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeButtonStylePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.RangeTimePickerPopup" /> class.
        /// </summary>
        public RangeTimePickerPopup()
        {
            this.DefaultStyleKey = (object)typeof(RangeTimePickerPopup);
        }

        /// <summary>
        /// Builds the visual tree for the RangeTimePickerPopup control when a
        /// new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.HoursSlider = this.GetTemplateChild("HoursSlider") as RangeBase;
            this.MinutesSlider = this.GetTemplateChild("MinutesSlider") as RangeBase;
            this.SecondsSlider = this.GetTemplateChild("SecondsSlider") as RangeBase;
            this.HoursContainer = this.GetTemplateChild("HoursPanel") as Panel;
            this.MinutesContainer = this.GetTemplateChild("MinutesPanel") as Panel;
            this.SecondsContainer = this.GetTemplateChild("SecondsPanel") as Panel;
            this.CommitButtonPart = this.GetTemplateChild("Commit") as ButtonBase;
            this.CancelButtonPart = this.GetTemplateChild("Cancel") as ButtonBase;
            this.GenerateLabels();
        }

        /// <summary>Called when a slider changes size.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.SizeChangedEventArgs" />
        /// instance containing the event data.</param>
        private void SliderSizeChange(object sender, SizeChangedEventArgs e)
        {
            this.LayoutLabels();
        }

        /// <summary>Reacts to a change in the Seconds Slider.</summary>
        /// <param name="sender">The Slider that changed its value.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void SecondsChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this._ignoreSliderChange)
                return;
            DateTime valueFromSliders = this.GetValueFromSliders();
            this._ignoreSliderChange = true;
            this.SecondsSlider.Value = e.OldValue;
            this._ignoreSliderChange = false;
            this.Value = new DateTime?(this.GetCoercedValue(new DateTime(valueFromSliders.Year, valueFromSliders.Month, valueFromSliders.Day, valueFromSliders.Hour, valueFromSliders.Minute, RangeTimePickerPopup.GetSnappedValue(e.NewValue, 0, 59, Math.Max(1, this.PopupSecondsInterval)))));
        }

        /// <summary>Reacts to a change in the Minutes Slider.</summary>
        /// <param name="sender">The Slider that changed its value.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void MinutesChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this._ignoreSliderChange)
                return;
            DateTime valueFromSliders = this.GetValueFromSliders();
            this._ignoreSliderChange = true;
            this.MinutesSlider.Value = e.OldValue;
            this._ignoreSliderChange = false;
            this.Value = new DateTime?(this.GetCoercedValue(new DateTime(valueFromSliders.Year, valueFromSliders.Month, valueFromSliders.Day, valueFromSliders.Hour, RangeTimePickerPopup.GetSnappedValue(e.NewValue, 0, 59, Math.Max(1, this.PopupMinutesInterval)), valueFromSliders.Second)));
        }

        /// <summary>Reacts to a change in the Hours Slider.</summary>
        /// <param name="sender">The Slider that changed its value.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void HoursChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this._ignoreSliderChange)
                return;
            DateTime valueFromSliders = this.GetValueFromSliders();
            this._ignoreSliderChange = true;
            this.HoursSlider.Value = e.OldValue;
            this._ignoreSliderChange = false;
            this.Value = new DateTime?(this.GetCoercedValue(new DateTime(valueFromSliders.Year, valueFromSliders.Month, valueFromSliders.Day, RangeTimePickerPopup.GetSnappedValue(e.NewValue, 0, 23, 1), valueFromSliders.Minute, valueFromSliders.Second)));
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            base.OnValueChanged(e);
            if (!this.IsCurrentlyOpen)
                return;
            DateTime? newValue = e.NewValue;
            if (!newValue.HasValue)
                return;
            newValue = e.NewValue;
            this.SetSlidersToValue(newValue.Value);
            this.SetEnabledStatusOnLabels();
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
            this._isOpenedByContainer = true;
            this.GenerateLabels();
            if (!this.Value.HasValue)
                return;
            this.SetSlidersToValue(this.Value.Value);
        }

        /// <summary>
        /// Called when the TimePicker control has closed this popup.
        /// </summary>
        public override void OnClosed()
        {
            base.OnClosed();
            this._isOpenedByContainer = false;
        }

        /// <summary>Called by the commit button.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void OnCommit(object sender, RoutedEventArgs e)
        {
            this.DoCommit();
        }

        /// <summary>Called by the cancel button.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> instance
        /// containing the event data.</param>
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.DoCancel();
        }

        /// <summary>Calculates the value based on the sliders.</summary>
        /// <returns>The DateTime as represented by the current values in the
        /// Sliders.</returns>
        private DateTime GetValueFromSliders()
        {
            DateTime dateTime = this.Value.HasValue ? this.Value.Value.Date : DateTime.Today.Date;
            return this.HoursSlider == null || this.MinutesSlider == null || this.SecondsSlider == null ? dateTime : new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, (int)this.HoursSlider.Value, (int)this.MinutesSlider.Value, (int)this.SecondsSlider.Value);
        }

        /// <summary>Sets the sliders to value.</summary>
        /// <param name="value">The DateTime that needs to be reflected by
        /// the three sliders.</param>
        private void SetSlidersToValue(DateTime value)
        {
            this._ignoreSliderChange = true;
            try
            {
                if (this.HoursSlider != null)
                    this.HoursSlider.Value = (double)value.Hour;
                if (this.MinutesSlider != null)
                    this.MinutesSlider.Value = (double)value.Minute;
                if (this.SecondsSlider == null)
                    return;
                this.SecondsSlider.Value = (double)value.Second;
            }
            finally
            {
                this._ignoreSliderChange = false;
            }
        }

        /// <summary>Generates the labels.</summary>
        private void GenerateLabels()
        {
            Action<UIElement> action = (Action<UIElement>)(element =>
            {
                if (!(element is Button button))
                    return;
                button.Click -= new RoutedEventHandler(this.OnLabelClicked);
            });
            if (this.HoursContainer != null)
            {
                this.HoursContainer.Children.ForEach<UIElement>(action);
                this.HoursContainer.Children.Clear();
                EnumerableExtensions.Range(22, 0, 2).ForEach<int>((Action<int>)(hour => this.HoursContainer.Children.Add((UIElement)this.CreateLabelElement(this.ActualTimeGlobalizationInfo.FormatTime(new DateTime?(DateTime.MinValue.AddHours((double)hour)), this.ActualFormat, 'h', 'H', 't', ' '), TimeSpan.FromHours((double)hour)))));
            }
            if (this.MinutesContainer != null)
            {
                this.MinutesContainer.Children.ForEach<UIElement>(action);
                this.MinutesContainer.Children.Clear();
                EnumerableExtensions.Range(55, 0, 5).ForEach<int>((Action<int>)(minute => this.MinutesContainer.Children.Add((UIElement)this.CreateLabelElement(this.ActualTimeGlobalizationInfo.FormatTime(new DateTime?(DateTime.MinValue.AddMinutes((double)minute)), (ITimeFormat)new CustomTimeFormat("mm")), TimeSpan.FromMinutes((double)minute)))));
            }
            if (this.SecondsContainer != null)
            {
                this.SecondsContainer.Children.ForEach<UIElement>(action);
                this.SecondsContainer.Children.Clear();
                EnumerableExtensions.Range(55, 0, 5).ForEach<int>((Action<int>)(second => this.SecondsContainer.Children.Add((UIElement)this.CreateLabelElement(this.ActualTimeGlobalizationInfo.FormatTime(new DateTime?(DateTime.MinValue.AddSeconds((double)second)), (ITimeFormat)new CustomTimeFormat("ss")), TimeSpan.FromSeconds((double)second)))));
            }
            this.UpdateLayout();
            this.LayoutLabels();
            this.SetEnabledStatusOnLabels();
        }

        /// <summary>Sets the enabled status on the labels.</summary>
        private void SetEnabledStatusOnLabels()
        {
            Func<DateTime, bool> func = (Func<DateTime, bool>)(time =>
            {
                DateTime dateTime;
                int num1;
                if (this.Minimum.HasValue)
                {
                    TimeSpan timeOfDay1 = time.TimeOfDay;
                    dateTime = this.Minimum.Value;
                    TimeSpan timeOfDay2 = dateTime.TimeOfDay;
                    num1 = !(timeOfDay1 < timeOfDay2) ? 1 : 0;
                }
                else
                    num1 = 1;
                if (num1 == 0)
                    return false;
                DateTime? maximum = this.Maximum;
                int num2;
                if (maximum.HasValue)
                {
                    TimeSpan timeOfDay1 = time.TimeOfDay;
                    maximum = this.Maximum;
                    dateTime = maximum.Value;
                    TimeSpan timeOfDay2 = dateTime.TimeOfDay;
                    num2 = !(timeOfDay1 > timeOfDay2) ? 1 : 0;
                }
                else
                    num2 = 1;
                return num2 != 0;
            });
            if (this.HoursSlider == null || this.MinutesSlider == null || this.SecondsSlider == null)
                return;
            DateTime dateTime1;
            if (this.HoursContainer != null)
            {
                foreach (Button child in (PresentationFrameworkCollection<UIElement>)this.HoursContainer.Children)
                {
                    dateTime1 = new DateTime(1900, 1, 1, ((TimeSpan)child.Tag).Hours, (int)this.MinutesSlider.Value, (int)this.SecondsSlider.Value);
                    child.IsEnabled = func(dateTime1);
                }
            }
            if (this.MinutesContainer != null)
            {
                foreach (Button child in (PresentationFrameworkCollection<UIElement>)this.MinutesContainer.Children)
                {
                    dateTime1 = new DateTime(1900, 1, 1, (int)this.HoursSlider.Value, ((TimeSpan)child.Tag).Minutes, (int)this.SecondsSlider.Value);
                    child.IsEnabled = func(dateTime1);
                }
            }
            if (this.SecondsContainer == null)
                return;
            foreach (Button child in (PresentationFrameworkCollection<UIElement>)this.SecondsContainer.Children)
            {
                dateTime1 = new DateTime(1900, 1, 1, (int)this.HoursSlider.Value, (int)this.MinutesSlider.Value, ((TimeSpan)child.Tag).Seconds);
                child.IsEnabled = func(dateTime1);
            }
        }

        /// <summary>
        /// Arranges the time labels on the RangeTimePickerPopup control.
        /// </summary>
        /// <remarks>Uses a canvas to layout labels vertically.</remarks>
        protected virtual void LayoutLabels()
        {
            Action<double, Panel, Func<TimeSpan, double>> action = (Action<double, Panel, Func<TimeSpan, double>>)((height, canvas, calc) =>
            {
                int count = canvas.Children.Count;
                double num1 = height / (double)count / 3.0;
                for (int index = 0; index < count; ++index)
                {
                    if (canvas.Children[index] is FrameworkElement child && child.Tag is TimeSpan)
                    {
                        TimeSpan tag = (TimeSpan)child.Tag;
                        double num2 = calc(tag);
                        child.Margin = new Thickness(0.0, height * num2 - num1 * num2 - num1, 0.0, 0.0);
                    }
                }
            });
            if (this.HoursSlider != null && this.HoursContainer != null && this.HoursContainer.Children.Count > 0)
                action(this.HoursSlider.ActualHeight, this.HoursContainer, (Func<TimeSpan, double>)(span => (double)(23 - span.Hours) / 23.0));
            if (this.MinutesSlider != null && this.MinutesContainer != null && this.MinutesContainer.Children.Count > 0)
                action(this.MinutesSlider.ActualHeight, this.MinutesContainer, (Func<TimeSpan, double>)(span => (double)(59 - span.Minutes) / 59.0));
            if (this.SecondsSlider == null || this.SecondsContainer == null || this.SecondsContainer.Children.Count <= 0)
                return;
            action(this.SecondsSlider.ActualHeight, this.SecondsContainer, (Func<TimeSpan, double>)(span => (double)(59 - span.Seconds) / 59.0));
        }

        /// <summary>Called when a label is clicked.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void OnLabelClicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TimeSpan tag = (TimeSpan)button.Tag;
            DateTime valueFromSliders = this.GetValueFromSliders();
            if (this.HoursContainer != null && this.HoursContainer.Children.Contains((UIElement)button))
                this.Value = new DateTime?(new DateTime(valueFromSliders.Year, valueFromSliders.Month, valueFromSliders.Day, tag.Hours, valueFromSliders.Minute, valueFromSliders.Second));
            else if (this.MinutesContainer != null && this.MinutesContainer.Children.Contains((UIElement)button))
            {
                this.Value = new DateTime?(new DateTime(valueFromSliders.Year, valueFromSliders.Month, valueFromSliders.Day, valueFromSliders.Hour, tag.Minutes, valueFromSliders.Second));
            }
            else
            {
                if (this.SecondsContainer == null || !this.SecondsContainer.Children.Contains((UIElement)button))
                    return;
                this.Value = new DateTime?(new DateTime(valueFromSliders.Year, valueFromSliders.Month, valueFromSliders.Day, valueFromSliders.Hour, valueFromSliders.Minute, tag.Seconds));
            }
        }

        /// <summary>Gets the value after Minimum and Maximum coercion.</summary>
        /// <param name="time">The input.</param>
        /// <returns>Time between Minimum and Maximum.</returns>
        /// <remarks>Done to avoid coercion in containing controls.</remarks>
        private DateTime GetCoercedValue(DateTime time)
        {
            DateTime dateTime;
            int num1;
            if (this.Minimum.HasValue)
            {
                TimeSpan timeOfDay1 = time.TimeOfDay;
                dateTime = this.Minimum.Value;
                TimeSpan timeOfDay2 = dateTime.TimeOfDay;
                num1 = !(timeOfDay1 < timeOfDay2) ? 1 : 0;
            }
            else
                num1 = 1;
            if (num1 == 0)
                return this.Minimum.Value;
            DateTime? maximum = this.Maximum;
            int num2;
            if (maximum.HasValue)
            {
                TimeSpan timeOfDay1 = time.TimeOfDay;
                maximum = this.Maximum;
                dateTime = maximum.Value;
                TimeSpan timeOfDay2 = dateTime.TimeOfDay;
                num2 = !(timeOfDay1 > timeOfDay2) ? 1 : 0;
            }
            else
                num2 = 1;
            if (num2 != 0)
                return time;
            maximum = this.Maximum;
            return maximum.Value;
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
            if (value < (double)minimum)
                return minimum;
            if (value > (double)maximum)
                return maximum;
            int val1 = (int)value / interval * interval;
            double num = (double)(val1 + interval / 2);
            return value > num ? Math.Min(val1 + interval, maximum) : Math.Max(val1, minimum);
        }

        /// <summary>Creates the element for a label.</summary>
        /// <param name="text">The text that is set as content.</param>
        /// <param name="timespan">The TimeSpan that is represented by the Button.</param>
        /// <returns>A Button representing the label.</returns>
        private Button CreateLabelElement(string text, TimeSpan timespan)
        {
            Button button = new Button();
            button.SetBinding(FrameworkElement.StyleProperty, new Binding()
            {
                Path = new PropertyPath("TimeButtonStyle", new object[0]),
                Source = (object)this
            });
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Tag = (object)timespan;
            button.Content = (object)text;
            button.Click += new RoutedEventHandler(this.OnLabelClicked);
            return button;
        }

        /// <summary>Gets the valid popup time selection modes.</summary>
        /// <returns>
        /// An array of PopupTimeSelectionModes that are supported by
        /// the Popup.
        /// </returns>
        internal override PopupTimeSelectionMode[] GetValidPopupTimeSelectionModes()
        {
            return new PopupTimeSelectionMode[2]
            {
        PopupTimeSelectionMode.HoursAndMinutesOnly,
        PopupTimeSelectionMode.AllowSecondsSelection
            };
        }
    }
}
