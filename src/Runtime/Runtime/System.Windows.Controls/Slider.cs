// (c) Copyright Microsoft Corporation. 
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that lets the user select from a range of values by moving
    /// a <see cref="Thumb"/> control along a track.
    /// </summary>
    [TemplatePart(Name = ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementHorizontalLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementHorizontalThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementVerticalLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementVerticalThumbName, Type = typeof(Thumb))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    public class Slider : RangeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Slider"/> class.
        /// </summary> 
        public Slider()
        {
            SizeChanged += delegate { UpdateTrackLayout(); };

            DefaultStyleKey = typeof(Slider);
            IsEnabledChanged += OnIsEnabledChanged;
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="Slider"/> control when a
        /// new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the parts
            ElementHorizontalTemplate = GetTemplateChild(ElementHorizontalTemplateName) as FrameworkElement;
            ElementHorizontalLargeIncrease = GetTemplateChild(ElementHorizontalLargeIncreaseName) as RepeatButton;
            ElementHorizontalLargeDecrease = GetTemplateChild(ElementHorizontalLargeDecreaseName) as RepeatButton;
            ElementHorizontalThumb = GetTemplateChild(ElementHorizontalThumbName) as Thumb;
            ElementVerticalTemplate = GetTemplateChild(ElementVerticalTemplateName) as FrameworkElement;
            ElementVerticalLargeIncrease = GetTemplateChild(ElementVerticalLargeIncreaseName) as RepeatButton;
            ElementVerticalLargeDecrease = GetTemplateChild(ElementVerticalLargeDecreaseName) as RepeatButton;
            ElementVerticalThumb = GetTemplateChild(ElementVerticalThumbName) as Thumb;

            if (ElementHorizontalThumb != null)
            {
                ElementHorizontalThumb.DragStarted += delegate (object sender, DragStartedEventArgs e) { this.Focus(); OnThumbDragStarted(); };
                ElementHorizontalThumb.DragDelta += delegate (object sender, DragDeltaEventArgs e) { OnThumbDragDelta(e); };
            }
            if (ElementHorizontalLargeDecrease != null)
            {
                ElementHorizontalLargeDecrease.Click += delegate { this.Focus(); Value -= LargeChange; };
            }
            if (ElementHorizontalLargeIncrease != null)
            {
                ElementHorizontalLargeIncrease.Click += delegate { this.Focus(); Value += LargeChange; };
            }

            if (ElementVerticalThumb != null)
            {
                ElementVerticalThumb.DragStarted += delegate (object sender, DragStartedEventArgs e) { this.Focus(); OnThumbDragStarted(); };
                ElementVerticalThumb.DragDelta += delegate (object sender, DragDeltaEventArgs e) { OnThumbDragDelta(e); };
            }
            if (ElementVerticalLargeDecrease != null)
            {
                ElementVerticalLargeDecrease.Click += delegate { this.Focus(); Value -= LargeChange; };
            }
            if (ElementVerticalLargeIncrease != null)
            {
                ElementVerticalLargeIncrease.Click += delegate { this.Focus(); Value += LargeChange; };
            }
            // Updating states for parts where properties might have been updated through 
            // XAML before the template was loaded. 
            OnOrientationChanged();
            ChangeVisualState(false);
        }

        /// <summary>
        /// Returns a <see cref="SliderAutomationPeer"/> object for use by the automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="SliderAutomationPeer"/> object for use by the slider control.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new SliderAutomationPeer(this);

        /// <summary>
        /// Gets or sets the orientation of a <see cref="Slider"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="Orientation"/> values. The default is <see cref="Orientation.Horizontal"/>.
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
                typeof(Slider),
                new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        /// <summary> 
        /// OrientationProperty property changed handler. 
        /// </summary>
        /// <param name="d">Slider that changed Orientation.</param> 
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider s = d as Slider;
            Debug.Assert(s != null);

            s.OnOrientationChanged();
        }

        /// <summary>
        /// Gets a value indicating whether the slider control has focus.
        /// </summary>
        /// <returns>
        /// true if the slider control has focus; otherwise, false. The default is false.
        /// </returns>
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            internal set { SetValue(IsFocusedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsFocusedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsFocused),
                typeof(bool),
                typeof(Slider),
                new PropertyMetadata(OnIsFocusedPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="IsFocused"/> dependency property.
        /// </summary> 
        public static readonly DependencyProperty IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;

        /// <summary>
        /// IsFocusedProperty property changed handler. 
        /// </summary> 
        /// <param name="d">Slider that changed IsFocused.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider s = d as Slider;
            Debug.Assert(s != null);

            s.OnIsFocusChanged(e);
        }

        /// <summary> 
        /// Called when the IsFocused property changes.
        /// </summary>
        /// <param name="e"> 
        /// The data for DependencyPropertyChangedEventArgs.
        /// </param>
        internal virtual void OnIsFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }
        
        /// <summary>
        /// Gets or sets a value that indicates the direction of increasing value.
        /// </summary>
        /// <returns>
        /// true if the direction of increasing value is to the left for a horizontal slider
        /// or down for a vertical slider; otherwise, false. The default is false.
        /// </returns>
        public bool IsDirectionReversed
        {
            get { return (bool)GetValue(IsDirectionReversedProperty); }
            set { SetValue(IsDirectionReversedProperty, value); }
        }

        /// <summary> 
        /// Identifies the <see cref="IsDirectionReversed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDirectionReversedProperty =
            DependencyProperty.Register(
                nameof(IsDirectionReversed),
                typeof(bool),
                typeof(Slider),
                new PropertyMetadata(OnIsDirectionReversedChanged));

        /// <summary> 
        /// IsDirectionReversedProperty property changed handler.
        /// </summary> 
        /// <param name="d">Slider that changed IsDirectionReversed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsDirectionReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slider s = d as Slider;
            Debug.Assert(s != null);

            s.UpdateTrackLayout();

        }

        /// <summary> 
        /// Called when the IsEnabled property changes.
        /// </summary> 
        /// <param name="sender">Source of the event </param>
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
        /// Updates the current position of the <see cref="Slider"/> when the <see cref="RangeBase.Value"/>
        /// property changes.
        /// </summary>
        /// <param name="oldValue">
        /// The old <see cref="RangeBase.Value"/> of the <see cref="Slider"/>.
        /// </param>
        /// <param name="newValue">
        /// The new <see cref="RangeBase.Value"/> of the <see cref="Slider"/>.
        /// </param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            UpdateTrackLayout();
        }
        
        /// <summary>
        /// Called when the <see cref="RangeBase.Minimum"/> property changes.
        /// </summary>
        /// <param name="oldMinimum">
        /// Old value of the <see cref="RangeBase.Minimum"/> property.
        /// </param>
        /// <param name="newMinimum">
        /// New value of the <see cref="RangeBase.Minimum"/> property.
        /// </param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            UpdateTrackLayout();
        }

        /// <summary>
        /// Called when the <see cref="RangeBase.Maximum"/> property changes.
        /// </summary>
        /// <param name="oldMaximum">
        /// Old value of the <see cref="RangeBase.Maximum"/> property.
        /// </param>
        /// <param name="newMaximum">
        /// New value of the <see cref="RangeBase.Maximum"/> property.
        /// </param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            UpdateTrackLayout();
        }

        /// <summary>
        /// Provides class handling <see cref="UIElement.MouseEnter"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            IsMouseOver = true;
            if ((Orientation == Orientation.Horizontal && ElementHorizontalThumb != null && !ElementHorizontalThumb.IsDragging) ||
                (Orientation == Orientation.Vertical && ElementVerticalThumb != null && !ElementVerticalThumb.IsDragging))
            {
                UpdateVisualState();
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="UIElement.MouseLeave"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseOver = false;
            if ((Orientation == Orientation.Horizontal && ElementHorizontalThumb != null && !ElementHorizontalThumb.IsDragging) ||
                (Orientation == Orientation.Vertical && ElementVerticalThumb != null && !ElementVerticalThumb.IsDragging))
            {
                UpdateVisualState();
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="UIElement.MouseLeftButtonDown"/> event.
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            Focus();
            CaptureMouse();
        }

        /// <summary>
        /// Responds to the MouseLeftButtonUp event. 
        /// </summary>
        /// <param name="e">The event data for the MouseLeftButtonUp event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            ReleaseMouseCapture();
            UpdateVisualState();
        }

        /// <summary>
        /// Provides class handling for the <see cref="UIElement.KeyDown"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="KeyEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            if (!IsEnabled)
            {
                return;
            }

            if (e.Key == Key.Left || e.Key == Key.Down)
            {
                if (IsDirectionReversed)
                {
                    Value += SmallChange;
                }
                else
                {
                    Value -= SmallChange;
                }
            }
            else if (e.Key == Key.Right || e.Key == Key.Up)
            {
                if (IsDirectionReversed)
                {
                    Value -= SmallChange;
                }
                else
                {
                    Value += SmallChange;
                }
            }
            else if (e.Key == Key.Home)
            {
                Value = Minimum;
            }
            else if (e.Key == Key.End)
            {
                Value = Maximum;
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="UIElement.GotFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="RoutedEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            IsFocused = true;
        }

        /// <summary>
        /// Provides class handling for the <see cref="UIElement.LostFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="RoutedEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            IsFocused = false;
        }

        /// <summary>
        /// Update the current visual state of the slider. 
        /// </summary> 
        internal void UpdateVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the Slider.
        /// </summary> 
        /// <param name="useTransitions"> 
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state. 
        /// </param>
        internal void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (IsMouseOver)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            if (IsFocused && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }
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
                offset = e.HorizontalChange / (ActualWidth - ElementHorizontalThumb.ActualWidth) * (Maximum - Minimum);
            }
            else if (Orientation == Orientation.Vertical && ElementVerticalThumb != null)
            {
                offset = -e.VerticalChange / (ActualHeight - ElementVerticalThumb.ActualHeight) * (Maximum - Minimum);
            }

            if (!double.IsNaN(offset) && !double.IsInfinity(offset))
            {
                _dragValue += IsDirectionReversed ? -offset : offset;

                double newValue = Math.Min(Maximum, Math.Max(Minimum, _dragValue));

                if (newValue != Value)
                {
                    Value = newValue;
                }
            }
        }

        /// <summary>
        /// This code will run whenever Orientation changes, to change the template 
        /// being used to display this control.
        /// </summary>
        internal virtual void OnOrientationChanged()
        {
            if (ElementHorizontalTemplate != null)
            {
                ElementHorizontalTemplate.Visibility = (Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Collapsed);
            }
            if (ElementVerticalTemplate != null)
            {
                ElementVerticalTemplate.Visibility = (Orientation == Orientation.Horizontal ? Visibility.Collapsed : Visibility.Visible);
            }
            UpdateTrackLayout();
        }

        /// <summary>
        /// This method will take the current min, max, and value to
        /// calculate and layout the current control measurements. 
        /// </summary>
        internal virtual void UpdateTrackLayout()
        {
            double maximum = Maximum;
            double minimum = Minimum;
            double value = Value;
            double multiplier = 1 - (maximum - value) / (maximum - minimum);

            Grid templateGrid = (Orientation == Orientation.Horizontal) ? (ElementHorizontalTemplate as Grid) : (ElementVerticalTemplate as Grid);
            if (templateGrid != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (templateGrid.ColumnDefinitions != null && templateGrid.ColumnDefinitions.Count == 3)
                    {
                        templateGrid.ColumnDefinitions[0].Width = new GridLength(1, IsDirectionReversed ? GridUnitType.Star : GridUnitType.Auto);
                        templateGrid.ColumnDefinitions[2].Width = new GridLength(1, IsDirectionReversed ? GridUnitType.Auto : GridUnitType.Star);
                        if (ElementHorizontalLargeDecrease != null)
                        {
                            ElementHorizontalLargeDecrease.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 2 : 0);
                        }
                        if (ElementHorizontalLargeIncrease != null)
                        {
                            ElementHorizontalLargeIncrease.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 0 : 2);
                        }
                    }

                    if (ElementHorizontalLargeDecrease != null && ElementHorizontalThumb != null)
                    {
                        ElementHorizontalLargeDecrease.Width = Math.Max(0, multiplier * (ActualWidth - ElementHorizontalThumb.ActualWidth));
                    }
                }
                else
                {
                    if (templateGrid.RowDefinitions != null && templateGrid.RowDefinitions.Count == 3)
                    {
                        templateGrid.RowDefinitions[0].Height = new GridLength(1, IsDirectionReversed ? GridUnitType.Auto : GridUnitType.Star);
                        templateGrid.RowDefinitions[2].Height = new GridLength(1, IsDirectionReversed ? GridUnitType.Star : GridUnitType.Auto);
                        if (ElementVerticalLargeDecrease != null)
                        {
                            ElementVerticalLargeDecrease.SetValue(Grid.RowProperty, IsDirectionReversed ? 0 : 2);
                        }
                        if (ElementVerticalLargeIncrease != null)
                        {
                            ElementVerticalLargeIncrease.SetValue(Grid.RowProperty, IsDirectionReversed ? 2 : 0);
                        }
                    }

                    if (ElementVerticalLargeDecrease != null && ElementVerticalThumb != null)
                    {
                        ElementVerticalLargeDecrease.Height = multiplier * (ActualHeight - ElementVerticalThumb.ActualHeight);
                    }
                }
            }
        }

        /// <summary> 
        /// Horizontal template root
        /// </summary>
        internal virtual FrameworkElement ElementHorizontalTemplate { get; set; }
        internal const string ElementHorizontalTemplateName = "HorizontalTemplate";

        /// <summary> 
        /// Large increase repeat button 
        /// </summary>
        internal virtual RepeatButton ElementHorizontalLargeIncrease { get; set; }
        internal const string ElementHorizontalLargeIncreaseName = "HorizontalTrackLargeChangeIncreaseRepeatButton";

        /// <summary> 
        /// Large decrease repeat button
        /// </summary>
        internal virtual RepeatButton ElementHorizontalLargeDecrease { get; set; }
        internal const string ElementHorizontalLargeDecreaseName = "HorizontalTrackLargeChangeDecreaseRepeatButton";

        /// <summary> 
        /// Thumb for dragging track
        /// </summary>
        internal virtual Thumb ElementHorizontalThumb { get; set; }
        internal const string ElementHorizontalThumbName = "HorizontalThumb";

        /// <summary> 
        /// Vertical template root 
        /// </summary>
        internal virtual FrameworkElement ElementVerticalTemplate { get; set; }
        internal const string ElementVerticalTemplateName = "VerticalTemplate";

        /// <summary> 
        /// Large increase repeat button
        /// </summary>
        internal virtual RepeatButton ElementVerticalLargeIncrease { get; set; }
        internal const string ElementVerticalLargeIncreaseName = "VerticalTrackLargeChangeIncreaseRepeatButton";

        /// <summary> 
        /// Large decrease repeat button
        /// </summary>
        internal virtual RepeatButton ElementVerticalLargeDecrease { get; set; }
        internal const string ElementVerticalLargeDecreaseName = "VerticalTrackLargeChangeDecreaseRepeatButton";

        /// <summary> 
        /// Thumb for dragging track 
        /// </summary>
        internal virtual Thumb ElementVerticalThumb { get; set; }
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
