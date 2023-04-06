// (c) Copyright Microsoft Corporation. 
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.ComponentModel;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Automation.Peers;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control that provides a scroll bar that has a sliding 
    /// <see cref="Thumb" /> whose position corresponds to a value.
    /// </summary> 
    [TemplatePart(Name = ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementHorizontalLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalSmallDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalSmallIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementVerticalLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalSmallIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalSmallDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalThumbName, Type = typeof(Thumb))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    public sealed class ScrollBar : RangeBase
    {
        private DebounceDispatcher _debounceDispatcher;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static readonly DependencyProperty DebounceProperty =
            DependencyProperty.RegisterAttached(
                nameof(Debounce),
                typeof(TimeSpan),
                typeof(ScrollBar),
                new PropertyMetadata(GetDefaultDebounce()));

        private static TimeSpan GetDefaultDebounce()
        {
            Application app = Application.Current;
            if (app != null)
            {
                return app.Host.Settings.ScrollDebounce;
            }

            return TimeSpan.Zero;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public TimeSpan Debounce
        {
            get => (TimeSpan)GetValue(DebounceProperty);
            set => SetValue(DebounceProperty, value);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static TimeSpan GetDebounce(FrameworkElement fe)
        {
            if (fe is null)
            {
                throw new ArgumentNullException(nameof(fe));
            }

            return (TimeSpan)fe.GetValue(DebounceProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void SetDebounce(FrameworkElement fe, TimeSpan debounce)
        {
            if (fe is null)
            {
                throw new ArgumentNullException(nameof(fe));
            }

            fe.SetValue(DebounceProperty, debounce);
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="ScrollBar"/> class.
        /// </summary> 
        public ScrollBar()
        {
            SizeChanged += delegate { UpdateTrackLayout(GetTrackLength()); };

            DefaultStyleKey = typeof(ScrollBar);
            IsEnabledChanged += OnIsEnabledChanged;
        }

        /// <summary> 
        /// Builds the visual tree for the <see cref="ScrollBar"/> control
        /// when a new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            // Get the parts
            ElementHorizontalTemplate = GetTemplateChild(ElementHorizontalTemplateName) as FrameworkElement;
            ElementHorizontalLargeIncrease = GetTemplateChild(ElementHorizontalLargeIncreaseName) as RepeatButton;
            ElementHorizontalLargeDecrease = GetTemplateChild(ElementHorizontalLargeDecreaseName) as RepeatButton;
            ElementHorizontalSmallIncrease = GetTemplateChild(ElementHorizontalSmallIncreaseName) as RepeatButton;
            ElementHorizontalSmallDecrease = GetTemplateChild(ElementHorizontalSmallDecreaseName) as RepeatButton;
            ElementHorizontalThumb = GetTemplateChild(ElementHorizontalThumbName) as Thumb;
            ElementVerticalTemplate = GetTemplateChild(ElementVerticalTemplateName) as FrameworkElement;
            ElementVerticalLargeIncrease = GetTemplateChild(ElementVerticalLargeIncreaseName) as RepeatButton;
            ElementVerticalLargeDecrease = GetTemplateChild(ElementVerticalLargeDecreaseName) as RepeatButton;
            ElementVerticalSmallIncrease = GetTemplateChild(ElementVerticalSmallIncreaseName) as RepeatButton;
            ElementVerticalSmallDecrease = GetTemplateChild(ElementVerticalSmallDecreaseName) as RepeatButton;
            ElementVerticalThumb = GetTemplateChild(ElementVerticalThumbName) as Thumb;

            if (ElementHorizontalThumb != null)
            {
                ElementHorizontalThumb.DragStarted += delegate (object sender, DragStartedEventArgs e) { OnThumbDragStarted(); };
                ElementHorizontalThumb.DragDelta += delegate (object sender, DragDeltaEventArgs e) { OnThumbDragDelta(e); };
                ElementHorizontalThumb.DragCompleted += delegate (object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
            }
            if (ElementHorizontalLargeDecrease != null)
            {
                ElementHorizontalLargeDecrease.Click += delegate (object sender, RoutedEventArgs e) { LargeDecrement(); };
            }
            if (ElementHorizontalLargeIncrease != null)
            {
                ElementHorizontalLargeIncrease.Click += delegate (object sender, RoutedEventArgs e) { LargeIncrement(); };
            }
            if (ElementHorizontalSmallDecrease != null)
            {
                ElementHorizontalSmallDecrease.Click += delegate (object sender, RoutedEventArgs e) { SmallDecrement(); };
            }
            if (ElementHorizontalSmallIncrease != null)
            {
                ElementHorizontalSmallIncrease.Click += delegate (object sender, RoutedEventArgs e) { SmallIncrement(); };
            }
            if (ElementVerticalThumb != null)
            {
                ElementVerticalThumb.DragStarted += delegate (object sender, DragStartedEventArgs e) { OnThumbDragStarted(); };
                ElementVerticalThumb.DragDelta += delegate (object sender, DragDeltaEventArgs e) { OnThumbDragDelta(e); };
                ElementVerticalThumb.DragCompleted += delegate (object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
            }
            if (ElementVerticalLargeDecrease != null)
            {
                ElementVerticalLargeDecrease.Click += delegate (object sender, RoutedEventArgs e) { LargeDecrement(); };
            }
            if (ElementVerticalLargeIncrease != null)
            {
                ElementVerticalLargeIncrease.Click += delegate (object sender, RoutedEventArgs e) { LargeIncrement(); };
            }
            if (ElementVerticalSmallDecrease != null)
            {
                ElementVerticalSmallDecrease.Click += delegate (object sender, RoutedEventArgs e) { SmallDecrement(); };
            }
            if (ElementVerticalSmallIncrease != null)
            {
                ElementVerticalSmallIncrease.Click += delegate (object sender, RoutedEventArgs e) { SmallIncrement(); };
            }
            // Updating states for parts where properties might have been updated through 
            // XAML before the template was loaded.
            OnOrientationChanged();
            UpdateVisualState(false);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new ScrollBarAutomationPeer(this);

        private void SmallDecrement()
        {
            double newValue = Math.Max(Value - SmallChange, Minimum);
            if (Value != newValue)
            {
                Value = newValue;
                RaiseScrollEvent(ScrollEventType.SmallDecrement);
            }
        }
        private void SmallIncrement()
        {
            double newValue = Math.Min(Value + SmallChange, Maximum);
            if (Value != newValue)
            {
                Value = newValue;
                RaiseScrollEvent(ScrollEventType.SmallIncrement);
            }
        }
        private void LargeDecrement()
        {
            double newValue = Math.Max(Value - LargeChange, Minimum);
            if (Value != newValue)
            {
                Value = newValue;
                RaiseScrollEvent(ScrollEventType.LargeDecrement);
            }
        }
        private void LargeIncrement()
        {
            double newValue = Math.Min(Value + LargeChange, Maximum);
            if (Value != newValue)
            {
                Value = newValue;
                RaiseScrollEvent(ScrollEventType.LargeIncrement);
            }
        }

        /// <summary>
        /// Called whenever the Thumb drag operation is complete
        /// </summary> 
        private void OnThumbDragCompleted()
        {
            RaiseScrollEvent(ScrollEventType.EndScroll);
        }

        /// <summary> 
        /// Called whenever the Thumb drag operation is started
        /// </summary>
        private void OnThumbDragStarted()
        {
            this._dragValue = this.Value;
        }

        /// <summary>
        /// Whenever the thumb gets dragged, we handle the event through 
        /// this function to update the current value depending upon the
        /// thumb drag delta.
        /// </summary> 
        /// <param name="e">DragEventArgs</param> 
        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            double offset = 0;

            if (Orientation == Orientation.Horizontal && ElementHorizontalThumb != null)
            {
                offset = e.HorizontalChange / (GetTrackLength() - ElementHorizontalThumb.ActualWidth) * (Maximum - Minimum);
            }
            else if (Orientation == Orientation.Vertical && ElementVerticalThumb != null)
            {
                offset = e.VerticalChange / (GetTrackLength() - ElementVerticalThumb.ActualHeight) * (Maximum - Minimum);
            }

            if (!double.IsNaN(offset) && !double.IsInfinity(offset))
            {
                _dragValue += offset;

                double newValue = Math.Min(Maximum, Math.Max(Minimum, _dragValue));

                if (newValue != Value)
                {
                    Value = newValue;
                    RaiseScrollEvent(ScrollEventType.ThumbTrack);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="ScrollBar"/> is displayed
        /// horizontally or vertically.
        /// </summary>
        /// <returns>
        /// An <see cref="Orientation"/> enumeration value that defines whether
        /// the <see cref="ScrollBar"/> is displayed horizontally or vertically.
        /// The default is <see cref="Orientation.Horizontal"/>.
        /// </returns>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(ScrollBar),
                new PropertyMetadata(Orientation.Vertical, OnOrientationPropertyChanged));

        /// <summary> 
        /// OrientationProperty property changed handler.
        /// </summary>
        /// <param name="d">ScrollBar that changed Orientation.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollBar s = d as ScrollBar;
            Debug.Assert(s != null);

            s.OnOrientationChanged();
        }

        /// <summary>
        /// Gets or sets the amount of the scrollable content that is currently visible.
        /// </summary>
        /// <returns>
        /// The amount of the scrollable content that is currently visible. The default is 0.
        /// </returns>
        public double ViewportSize
        {
            get { return (double)GetValue(ViewportSizeProperty); }
            set { SetValue(ViewportSizeProperty, value); }
        }

        /// <summary> 
        /// Identifies the <see cref="ViewportSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportSizeProperty =
            DependencyProperty.Register(
                "ViewportSize",
                typeof(double),
                typeof(ScrollBar),
                new PropertyMetadata(0.0d, OnViewportSizeChanged));

        private static void OnViewportSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollBar s = d as ScrollBar;
            Debug.Assert(s != null);

            s.UpdateTrackLayout(s.GetTrackLength());
        }

        /// <summary> 
        /// Called when the IsEnabled property changes.
        /// </summary> 
        /// <param name="e">Property changed args</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
            {
                IsMouseOver = false;
            }
            UpdateVisualState();
        }

        /// <summary> 
        /// Called when the Value property changes. 
        /// </summary>
        /// <param name="oldValue">Old value of the Value property.</param> 
        /// <param name="newValue">New value of the Value property.</param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            double trackLength = GetTrackLength();

            base.OnValueChanged(oldValue, newValue);

            UpdateTrackLayout(trackLength);
        }

        /// <summary>
        /// Called when the Maximum property changes 
        /// </summary>
        /// <param name="oldMaximum">Old value of the Maximum property.</param>
        /// <param name="newMaximum">New value of the Maximum property.</param> 
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            double trackLength = GetTrackLength();

            base.OnMaximumChanged(oldMaximum, newMaximum);
            UpdateTrackLayout(trackLength);
        }

        /// <summary> 
        /// Called when the Minimum property changes 
        /// </summary>
        /// <param name="oldMinimum">Old value of the Minimum property.</param> 
        /// <param name="newMinimum">New value of the Minimum property.</param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            double trackLength = GetTrackLength();

            base.OnMinimumChanged(oldMinimum, newMinimum);
            UpdateTrackLayout(trackLength);
        }

        /// <summary>
        /// Responds to the MouseEnter event.
        /// </summary> 
        /// <param name="e">The event data for the MouseEnter event.</param> 
#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected override void OnPointerEntered(MouseEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseEnter(e);
#else
            base.OnPointerEntered(e);
#endif
            IsMouseOver = true;
            if ((Orientation == Orientation.Horizontal && ElementHorizontalThumb != null && !ElementHorizontalThumb.IsDragging) ||
                (Orientation == Orientation.Vertical && ElementVerticalThumb != null && !ElementVerticalThumb.IsDragging))
            {
                UpdateVisualState();
            }
        }

        /// <summary>
        /// Responds to the MouseLeave event. 
        /// </summary>
        /// <param name="e">The event data for the MouseLeave event.</param>
#if MIGRATION
        protected override void OnMouseLeave(MouseEventArgs e)
#else
        protected override void OnPointerExited(MouseEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeave(e);
#else
            base.OnPointerExited(e);
#endif
            IsMouseOver = false;
            if ((Orientation == Orientation.Horizontal && ElementHorizontalThumb != null && !ElementHorizontalThumb.IsDragging) ||
                (Orientation == Orientation.Vertical && ElementVerticalThumb != null && !ElementVerticalThumb.IsDragging))
            {
                UpdateVisualState();
            }
        }

        /// <summary> 
        /// Responds to the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The event data for the MouseLeftButtonDown event.</param>
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(MouseButtonEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
#if MIGRATION
            CaptureMouse();
#else
            CapturePointer(e.Pointer);
#endif
        }

        /// <summary> 
        /// Responds to the MouseLeftButtonUp event.
        /// </summary> 
        /// <param name="e">The event data for the MouseLeftButtonUp event.</param>
#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(MouseButtonEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
#if MIGRATION
            ReleaseMouseCapture();
#else
            ReleasePointerCapture(e.Pointer);
#endif
            UpdateVisualState();
        }

        /// <summary>
        /// Occurs one or more times as content scrolls in a <see cref="ScrollBar"/>
        /// when the user moves the <see cref="Thumb"/> by using the mouse.
        /// </summary>
        public event ScrollEventHandler Scroll;

        /// <summary> 
        /// This raises the Scroll event, passing in the scrollEventType 
        /// as a parameter to let the handler know what triggered this event.
        /// </summary> 
        /// <param name="scrollEventType">ScrollEventType</param>
        internal void RaiseScrollEvent(ScrollEventType scrollEventType)
        {
            TimeSpan debounce = Debounce;
            if (debounce > TimeSpan.Zero && scrollEventType != ScrollEventType.EndScroll)
            {
                _debounceDispatcher ??= new DebounceDispatcher();

                _debounceDispatcher.Debounce(
                    debounce,
                    () => Scroll?.Invoke(this, new ScrollEventArgs(scrollEventType, Value)));
            }
            else
            {
                Scroll?.Invoke(this, new ScrollEventArgs(scrollEventType, Value));
            }
        }

        /// <summary> 
        /// Update the current visual state of the ScrollBar.
        /// </summary> 
        internal void UpdateVisualState()
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// Update the current visual state of the ScrollBar.
        /// </summary> 
        /// <param name="useTransitions">
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state. 
        /// </param> 
        internal void UpdateVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                GoToState(useTransitions, VisualStates.StateDisabled);
            }
            else if (IsMouseOver)
            {
                GoToState(useTransitions, VisualStates.StateMouseOver);
            }
            else
            {
                GoToState(useTransitions, VisualStates.StateNormal);
            }
        }

        /// <summary> 
        /// This code will run whenever Orientation changes, to change the template
        /// being used to display this control.
        /// </summary> 
        private void OnOrientationChanged()
        {
            double trackLength = GetTrackLength();

            if (ElementHorizontalTemplate != null)
            {
                ElementHorizontalTemplate.Visibility = (Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Collapsed);
            }
            if (ElementVerticalTemplate != null)
            {
                ElementVerticalTemplate.Visibility = (Orientation == Orientation.Horizontal ? Visibility.Collapsed : Visibility.Visible);
            }
            UpdateTrackLayout(trackLength);
        }

        /// <summary>
        /// This method will take the current min, max, and value to
        /// calculate and layout the current control measurements. 
        /// </summary>
        private void UpdateTrackLayout(double trackLength)
        {
            double maximum = Maximum;
            double minimum = Minimum;
            double value = Value;
            double multiplier = (value - minimum) / (maximum - minimum);

            double thumbSize = UpdateThumbSize(trackLength);

            if (Orientation == Orientation.Horizontal && ElementHorizontalLargeDecrease != null && ElementHorizontalThumb != null)
            {
                ElementHorizontalLargeDecrease.Width = Math.Max(0.0f, multiplier * (trackLength - thumbSize));
            }
            else if (Orientation == Orientation.Vertical && ElementVerticalLargeDecrease != null && ElementVerticalThumb != null)
            {
                ElementVerticalLargeDecrease.Height = Math.Max(0.0f, multiplier * (trackLength - thumbSize));
            }
        }

        /// <summary> 
        /// Based on the size of the Large Increase/Decrease RepeatButtons 
        /// and on the Thumb, we will calculate the size of the track area
        /// of the ScrollBar 
        /// </summary>
        /// <returns>The length of the track</returns>
        internal double GetTrackLength()
        {
            double length = Double.NaN;

            if (Orientation == Orientation.Horizontal)
            {
                length = this.ActualWidth;

                if (ElementHorizontalSmallDecrease != null)
                {
                    length -= ElementHorizontalSmallDecrease.ActualWidth + ElementHorizontalSmallDecrease.Margin.Left + ElementHorizontalSmallDecrease.Margin.Right;
                }
                if (ElementHorizontalSmallIncrease != null)
                {
                    length -= ElementHorizontalSmallIncrease.ActualWidth + ElementHorizontalSmallIncrease.Margin.Left + ElementHorizontalSmallIncrease.Margin.Right;
                }
            }
            else
            {
                length = this.ActualHeight;

                if (ElementVerticalSmallDecrease != null)
                {
                    length -= ElementVerticalSmallDecrease.ActualHeight + ElementVerticalSmallDecrease.Margin.Top + ElementVerticalSmallDecrease.Margin.Bottom;
                }
                if (ElementVerticalSmallIncrease != null)
                {
                    length -= ElementVerticalSmallIncrease.ActualHeight + ElementVerticalSmallIncrease.Margin.Top + ElementVerticalSmallIncrease.Margin.Bottom;
                }
            }

            return length;
        }

        /// <summary>
        /// Based on the ViewportSize, the Track's length, and the
        /// Minimum and Maximum values, we will calculate the length 
        /// of the thumb.
        /// </summary>
        /// <returns>Double value representing display unit length</returns> 
        private double ConvertViewportSizeToDisplayUnits(double trackLength)
        {
            double viewRangeValue = Maximum - Minimum;

            return trackLength * ViewportSize / (ViewportSize + viewRangeValue);
        }

        /// <summary>
        /// This will resize the Thumb, based on calculations with the ViewportSize, 
        /// the Track's length, and the Minimum and Maximum values. 
        /// </summary>
        internal double UpdateThumbSize(double trackLength)
        {
            double result = Double.NaN;
            bool hideThumb = trackLength <= 0;

            if (trackLength > 0)
            {
                if (Orientation == Orientation.Horizontal && ElementHorizontalThumb != null)
                {
                    if (Maximum - Minimum != 0)
                    {
                        result = Math.Max(ElementHorizontalThumb.MinWidth, ConvertViewportSizeToDisplayUnits(trackLength));
                    }

                    // hide the thumb if too big
                    if (Maximum - Minimum == 0 || result > ActualWidth || trackLength <= ElementHorizontalThumb.MinWidth)
                    {
                        hideThumb = true;
                    }
                    else
                    {
                        ElementHorizontalThumb.Visibility = Visibility.Visible;
                        ElementHorizontalThumb.Width = result;
                    }
                }
                else if (Orientation == Orientation.Vertical && ElementVerticalThumb != null)
                {
                    if (Maximum - Minimum != 0)
                    {
                        result = Math.Max(ElementVerticalThumb.MinHeight, ConvertViewportSizeToDisplayUnits(trackLength));
                    }

                    // hide the thumb if too big
                    if (Maximum - Minimum == 0 || result > ActualHeight || trackLength <= ElementVerticalThumb.MinHeight)
                    {
                        hideThumb = true;
                    }
                    else
                    {
                        ElementVerticalThumb.Visibility = Visibility.Visible;
                        ElementVerticalThumb.Height = result;
                    }
                }
            }

            if (hideThumb)
            {
                if (ElementHorizontalThumb != null)
                {
                    ElementHorizontalThumb.Visibility = Visibility.Collapsed;
                }

                if (ElementVerticalThumb != null)
                {
                    ElementVerticalThumb.Visibility = Visibility.Collapsed;
                }
            }

            return result;
        }

        /// <summary>
        /// Horizontal template root 
        /// </summary>
        internal FrameworkElement ElementHorizontalTemplate { get; set; }
        internal const string ElementHorizontalTemplateName = "HorizontalRoot";

        /// <summary>
        /// Large increase repeat button 
        /// </summary>
        internal RepeatButton ElementHorizontalLargeIncrease { get; set; }
        internal const string ElementHorizontalLargeIncreaseName = "HorizontalLargeIncrease";

        /// <summary>
        /// Large decrease repeat button 
        /// </summary> 
        internal RepeatButton ElementHorizontalLargeDecrease { get; set; }
        internal const string ElementHorizontalLargeDecreaseName = "HorizontalLargeDecrease";

        /// <summary>
        /// Small increase repeat button 
        /// </summary>
        internal RepeatButton ElementHorizontalSmallIncrease { get; set; }
        internal const string ElementHorizontalSmallIncreaseName = "HorizontalSmallIncrease";

        /// <summary>
        /// Small decrease repeat button 
        /// </summary>
        internal RepeatButton ElementHorizontalSmallDecrease { get; set; }
        internal const string ElementHorizontalSmallDecreaseName = "HorizontalSmallDecrease";

        /// <summary>
        /// Thumb for dragging track 
        /// </summary> 
        internal Thumb ElementHorizontalThumb { get; set; }
        internal const string ElementHorizontalThumbName = "HorizontalThumb";

        /// <summary>
        /// Vertical template root 
        /// </summary>
        internal FrameworkElement ElementVerticalTemplate { get; set; }
        internal const string ElementVerticalTemplateName = "VerticalRoot";

        /// <summary>
        /// Large increase repeat button 
        /// </summary>
        internal RepeatButton ElementVerticalLargeIncrease { get; set; }
        internal const string ElementVerticalLargeIncreaseName = "VerticalLargeIncrease";

        /// <summary>
        /// Large decrease repeat button 
        /// </summary> 
        internal RepeatButton ElementVerticalLargeDecrease { get; set; }
        internal const string ElementVerticalLargeDecreaseName = "VerticalLargeDecrease";

        /// <summary>
        /// Small increase repeat button 
        /// </summary>
        internal RepeatButton ElementVerticalSmallIncrease { get; set; }
        internal const string ElementVerticalSmallIncreaseName = "VerticalSmallIncrease";

        /// <summary>
        /// Small decrease repeat button 
        /// </summary>
        internal RepeatButton ElementVerticalSmallDecrease { get; set; }
        internal const string ElementVerticalSmallDecreaseName = "VerticalSmallDecrease";

        /// <summary>
        /// Thumb for dragging track 
        /// </summary> 
        internal Thumb ElementVerticalThumb { get; set; }
        internal const string ElementVerticalThumbName = "VerticalThumb";

        /// <summary> 
        /// Whether the mouse is currently over the control 
        /// </summary>
        internal bool IsMouseOver { get; set; }
        /// <summary>
        /// Accumulates drag offsets in case the mouse drags off the end of the track.
        /// </summary> 
        private double _dragValue;
    }
}
