

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
namespace System.Windows.Controls.Primitives
#else
using System;
using Windows.Foundation;
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control that provides a scroll bar that has a sliding Thumb whose position corresponds to a value.
    /// </summary>
    public sealed partial class ScrollBar : RangeBase
    {
        const double MINIMUM_THUMB_SIZE = 16d;
        double _smallDecreaseButtonSize;
        double _smallIncreaseButtonSize;
        bool _controlWasProperlyDrawn;
        double _valueWithoutCoercion; // Note: Normally we keep the value within the Min/Max range (this is called "coercion"). However, while dragging the thumb, we store the "ValueWithoutCoercion" so that when the user drags the pointer beyond the scrollbar limits and then goes back, the "going back" movement does not modify the actual value until the pointer is again inside the scrollbar range.

        // Horizontal elements:
        Canvas _horizontalRoot;
        Thumb _horizontalThumb;
        Button _horizontalSmallDecrease;
        Button _horizontalSmallIncrease;
        Button _horizontalLargeDecrease;
        Button _horizontalLargeIncrease;

        // Vertical elements:
        Canvas _verticalRoot;
        Thumb _verticalThumb;
        Button _verticalSmallDecrease;
        Button _verticalSmallIncrease;
        Button _verticalLargeDecrease;
        Button _verticalLargeIncrease;

        /// <summary>
        /// Initializes a new instance of the ScrollBar class.
        /// </summary>
        public ScrollBar()
        {
            // Set default style:
            this.DefaultStyleKey = typeof(ScrollBar);

            // Register some events:
            this.Unloaded += ScrollBar_Unloaded;
            this.SizeChanged += ScrollBar_SizeChanged;
        }

        void ScrollBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Note: this handler is also called the first time that the element is added to the visual tree.

            // Update the position of all the elements:
            if (TryUpdateSizeAndPositionOfUIElements())
                _controlWasProperlyDrawn = true;
        }

        void ScrollBar_Unloaded(object sender, RoutedEventArgs e)
        {
            _controlWasProperlyDrawn = false;
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            //----------------------------
            // Get a reference to the UI elements defined in the control template:
            //----------------------------

            _horizontalRoot = this.GetTemplateChild("HorizontalRoot") as Canvas;
            _horizontalThumb = this.GetTemplateChild("HorizontalThumb") as Thumb;
            _horizontalSmallDecrease = this.GetTemplateChild("HorizontalSmallDecrease") as Button;
            _horizontalSmallIncrease = this.GetTemplateChild("HorizontalSmallIncrease") as Button;
            _horizontalLargeDecrease = this.GetTemplateChild("HorizontalLargeDecrease") as Button;
            _horizontalLargeIncrease = this.GetTemplateChild("HorizontalLargeIncrease") as Button;

            _verticalRoot = this.GetTemplateChild("VerticalRoot") as Canvas;
            _verticalThumb = this.GetTemplateChild("VerticalThumb") as Thumb;
            _verticalSmallDecrease = this.GetTemplateChild("VerticalSmallDecrease") as Button;
            _verticalSmallIncrease = this.GetTemplateChild("VerticalSmallIncrease") as Button;
            _verticalLargeDecrease = this.GetTemplateChild("VerticalLargeDecrease") as Button;
            _verticalLargeIncrease = this.GetTemplateChild("VerticalLargeIncrease") as Button;


            //----------------------------
            // Register the events:
            //----------------------------
            if (_horizontalThumb != null)
            {
                _horizontalThumb.DragStarted += HorizontalThumb_DragStarted;
                _horizontalThumb.DragDelta += HorizontalThumb_DragDelta;
                _horizontalThumb.DragCompleted += HorizontalThumb_DragCompleted;
            }
            if (_verticalThumb != null)
            {
                _verticalThumb.DragStarted += VerticalThumb_DragStarted;
                _verticalThumb.DragDelta += VerticalThumb_DragDelta;
                _verticalThumb.DragCompleted += VerticalThumb_DragCompleted;
            }
            if (_horizontalSmallDecrease != null)
                _horizontalSmallDecrease.Click += SmallDecrease_Click;
            if (_verticalSmallDecrease != null)
                _verticalSmallDecrease.Click += SmallDecrease_Click;
            if (_horizontalLargeDecrease != null)
                _horizontalLargeDecrease.Click += LargeDecrease_Click;
            if (_verticalLargeDecrease != null)
                _verticalLargeDecrease.Click += LargeDecrease_Click;
            if (_horizontalSmallIncrease != null)
                _horizontalSmallIncrease.Click += SmallIncrease_Click;
            if (_verticalSmallIncrease != null)
                _verticalSmallIncrease.Click += SmallIncrease_Click;
            if (_horizontalLargeIncrease != null)
                _horizontalLargeIncrease.Click += LargeIncrease_Click;
            if (_verticalLargeIncrease != null)
                _verticalLargeIncrease.Click += LargeIncrease_Click;


            //----------------------------
            // Display the horizontal or vertical root depending on the orientation:
            //----------------------------

            if (_verticalRoot != null)
            {
                _verticalRoot.Visibility = (this.Orientation == Orientation.Vertical ? Visibility.Visible : Visibility.Collapsed);
            }
            if (_horizontalRoot != null)
            {
                _horizontalRoot.Visibility = (this.Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Collapsed);
            }

            if (TryUpdateSizeAndPositionOfUIElements())
                _controlWasProperlyDrawn = true;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            // Refresh the position of the thumb:
            UpdateThumbPositionAndSize();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);

            // Refresh the position of the thumb:
            UpdateThumbPositionAndSize();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);

            // Refresh the position of the thumb:
            UpdateThumbPositionAndSize();
        }

        void LargeIncrease_Click(object sender, RoutedEventArgs e)
        {
            OnClickOnSmallOrLargeDecreaseIncreaseButtons(this.LargeChange, ScrollEventType.LargeIncrement);
        }

        void SmallIncrease_Click(object sender, RoutedEventArgs e)
        {
            OnClickOnSmallOrLargeDecreaseIncreaseButtons(this.SmallChange, ScrollEventType.SmallIncrement);
        }

        void LargeDecrease_Click(object sender, RoutedEventArgs e)
        {
            OnClickOnSmallOrLargeDecreaseIncreaseButtons(-this.LargeChange, ScrollEventType.LargeDecrement);
        }

        void SmallDecrease_Click(object sender, RoutedEventArgs e)
        {
            OnClickOnSmallOrLargeDecreaseIncreaseButtons(-this.SmallChange, ScrollEventType.SmallDecrement);
        }

        void VerticalThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            OnDragCompleted();
        }

        void HorizontalThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            OnDragCompleted();
        }

        void VerticalThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            OnDragStarted();
        }

        void HorizontalThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            OnDragStarted();
        }

        void VerticalThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ChangeValueBasedOnPointerMovement(e.VerticalChange);
        }

        void HorizontalThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ChangeValueBasedOnPointerMovement(e.HorizontalChange);
        }

        void OnClickOnSmallOrLargeDecreaseIncreaseButtons(double amountByWhichToIncreaseOrDecreaseTheValue, ScrollEventType scrollEventType)
        {
            double newValue = CoerceValue(this.Value + amountByWhichToIncreaseOrDecreaseTheValue);
            if (newValue != this.Value)
            {
                this.SetCurrentValue(RangeBase.ValueProperty, newValue); // Note: we do not use "this.Value = newValue" because it deletes any bindings that the user may have set to the scrollbar with <ScrollBar Value="{Binding...}"/>.
                if (this.Scroll != null)
                    this.Scroll(this, new ScrollEventArgs(newValue, scrollEventType));
            }
        }

        void OnDragStarted()
        {
            // Reset the "ValueWithoutCoercion" (read the note near the definition of that variable to better understand what it is used for):
            _valueWithoutCoercion = this.Value;

            // Disable the large decrease/increase buttons so that if the user moves the pointer really quickly, they do not get highlighted:
            if (_verticalLargeDecrease != null)
                _verticalLargeDecrease.IsHitTestVisible = false;
            if (_verticalLargeIncrease != null)
                _verticalLargeIncrease.IsHitTestVisible = false;
            if (_horizontalLargeDecrease != null)
                _horizontalLargeDecrease.IsHitTestVisible = false;
            if (_horizontalLargeIncrease != null)
                _horizontalLargeIncrease.IsHitTestVisible = false;
        }

        void OnDragCompleted()
        {
            // Re-enable the large decrease/increase buttons (see note in the method where they are disabled to better understand why they were disabled in the first place):
            if (_verticalLargeDecrease != null)
                _verticalLargeDecrease.IsHitTestVisible = true;
            if (_verticalLargeIncrease != null)
                _verticalLargeIncrease.IsHitTestVisible = true;
            if (_horizontalLargeDecrease != null)
                _horizontalLargeDecrease.IsHitTestVisible = true;
            if (_horizontalLargeIncrease != null)
                _horizontalLargeIncrease.IsHitTestVisible = true;

            // Call the "Scroll" event passing the "EndScroll" argument:
            if (this.Scroll != null)
                this.Scroll(this, new ScrollEventArgs(this.Value, ScrollEventType.EndScroll));
        }

        void ChangeValueBasedOnPointerMovement(double pointerMovementInPixels)
        {
            //----------------------------
            // Change the value when the thumb is dragged:
            //----------------------------

            //First, check that the control was properly drawn and get its size:
            Size totalControlSize;
            if (CheckIfControlWasRenderedProperlyAndGetCurrentControlSize(out totalControlSize))
            {
                double totalControlSizeInMainDirection = (this.Orientation == Orientation.Vertical ? totalControlSize.Height : totalControlSize.Width);
                double trackSize; // Note: the "Track" is the area where the thumb moves.
                double thumbSize;
                double scrollableSize; // Note: the "ScrollableSize" corresponds to the distance that the center of the thumb can travel.
                double minValue;
                double maxValue;
                CalculateThumbTrackAndScrollableSizes(totalControlSizeInMainDirection, out trackSize, out thumbSize, out scrollableSize, out minValue, out maxValue);
                double maxMinusMin = maxValue - minValue;

                // Calculate the amount by which we should change the "this.Value" based on the movement of the pointern, by comparing the movement of the pointer to the length of the "track" where the thumb is allowed to move into:
                double valueDelta;
                if (scrollableSize > 0 && maxMinusMin > 0)
                    valueDelta = (pointerMovementInPixels * maxMinusMin) / scrollableSize;
                else
                    valueDelta = 0;

                // Calculate what should be the new "value" of the ScrollBar, while ensuring that it does not get out of the Min/Max range:
                _valueWithoutCoercion += valueDelta; // Note: read the note near the definition of the "_valueWithoutCoercion" variable to better understand what it is used for. 
                double newValue = CoerceValue(_valueWithoutCoercion); // Note: "Coerce" means that we ensure that it stays within the Min/Max range.

                // Apply the change to the value of the ScrollBar, and update the Thumb position accordingly:
                if (newValue != this.Value)
                {
                    this.SetCurrentValue(RangeBase.ValueProperty, newValue); // Note: we do not use "this.Value = newValue" because it deletes any bindings that the user may have set to the scrollbar with <ScrollBar Value="{Binding...}"/>.

                    // Update the position of the Thumb based on the new value:
                    UpdateThumbPositionAndSize(totalControlSizeInMainDirection);

                    // Call the "Scroll" event:
                    if (this.Scroll != null)
                        this.Scroll(this, new ScrollEventArgs(newValue, ScrollEventType.ThumbTrack));
                }
            }
        }

        double CoerceValue(double value)
        {
            // Note: "Coerce" means that we ensure that the value stays within the Min/Max range.

            double minValue = this.Minimum;
            double maxValue = this.Maximum;
            if (value <= minValue)
                value = minValue;
            if (value >= maxValue)
                value = maxValue;
            return value;
        }

        bool TryUpdateSizeAndPositionOfUIElements()
        {
            //----------------------------
            // Position the elements that are inside the scrollbar:
            //----------------------------

            //First, check that the control was properly drawn and get its size:
            Size totalControlSize;
            if (this.IsLoaded && TryGetCurrentControlSize(out totalControlSize))
            {
                if (Orientation == Orientation.Vertical)
                {
                    //***************
                    // VERTICAL CASE
                    //***************

                    if (_verticalRoot != null
                        && _verticalThumb != null
                        && _verticalSmallDecrease != null
                        && _verticalSmallIncrease != null
                        && _verticalLargeDecrease != null
                        && _verticalLargeIncrease != null)
                    {
                        //--------------------------------
                        // Update the size of the UI elements:
                        //--------------------------------
                        _verticalThumb.Width
                            = _verticalSmallDecrease.Width
                            = _verticalSmallIncrease.Width
                            = _verticalLargeDecrease.Width
                            = _verticalLargeIncrease.Width
                            = totalControlSize.Width;

                        double buttonsHeight = totalControlSize.Width; // Deliberately square.

                        // We choose the size of the small decrease buttons (ie. the arrows):
                        _smallDecreaseButtonSize = buttonsHeight;
                        _smallIncreaseButtonSize = buttonsHeight;

                        // We apply that size:
                        _verticalSmallDecrease.Height = _smallDecreaseButtonSize;
                        _verticalSmallIncrease.Height = _smallIncreaseButtonSize;

                        //--------------------------------
                        // Update the position of the buttons:
                        //--------------------------------
                        Canvas.SetTop(_verticalSmallDecrease, 0d);
                        Canvas.SetTop(_verticalSmallIncrease, totalControlSize.Height - buttonsHeight);

                        //--------------------------------
                        // Update the position of the thumb and of the large increment/decrement areas:
                        //--------------------------------
                        UpdateThumbPositionAndSize(totalControlSize.Height);

                        return true;
                    }
                }
                else
                {
                    //***************
                    // HORIZONTAL CASE
                    //***************

                    if (_horizontalRoot != null
                        && _horizontalThumb != null
                        && _horizontalSmallDecrease != null
                        && _horizontalSmallIncrease != null
                        && _horizontalLargeDecrease != null
                        && _horizontalLargeIncrease != null)
                    {
                        //--------------------------------
                        // Update the size of the UI elements:
                        //--------------------------------
                        _horizontalThumb.Height
                            = _horizontalSmallDecrease.Height
                            = _horizontalSmallIncrease.Height
                            = _horizontalLargeDecrease.Height
                            = _horizontalLargeIncrease.Height
                            = totalControlSize.Height;

                        double buttonsWidth = totalControlSize.Height; // Deliberately square.

                        // We choose the size of the small decrease buttons (ie. the arrows):
                        _smallDecreaseButtonSize = buttonsWidth;
                        _smallIncreaseButtonSize = buttonsWidth;

                        // We apply that size:
                        _horizontalSmallDecrease.Width = _smallDecreaseButtonSize;
                        _horizontalSmallIncrease.Width = _smallIncreaseButtonSize;

                        //--------------------------------
                        // Update the position of the UI elements:
                        //--------------------------------
                        Canvas.SetLeft(_horizontalSmallDecrease, 0d);
                        Canvas.SetLeft(_horizontalSmallIncrease, totalControlSize.Width - buttonsWidth);

                        //--------------------------------
                        // Update the position of the thumb and of the large increment/decrement areas:
                        //--------------------------------
                        UpdateThumbPositionAndSize(totalControlSize.Width);

                        return true;
                    }
                }
            }

            return false;
        }

        bool CheckIfControlWasRenderedProperlyAndGetCurrentControlSize(out Size totalControlSize)
        {
            if (_controlWasProperlyDrawn) // Note: this is true when all the internal sizes of the control have been calculated, such as the "_smallDecreaseButtonSize" and "_smallIncreaseButtonSize" values.
            {
                return TryGetCurrentControlSize(out totalControlSize);
            }
            else
            {
                totalControlSize = new Size(0d, 0d); // Note: this is the default return value.
                return false;
            }
        }

        bool TryGetCurrentControlSize(out Size totalControlSize)
        {
            totalControlSize = new Size(0d, 0d); // Note: this is the default return value.
            if (_verticalRoot != null && _horizontalRoot != null)
            {
                // Calculate current width and height of this control:
                Size actualSize;
                if (this.Orientation == Orientation.Vertical)
                {
                    actualSize = _verticalRoot.INTERNAL_GetActualWidthAndHeight();
                }
                else
                {
                    actualSize = _horizontalRoot.INTERNAL_GetActualWidthAndHeight();
                }
                double width = !double.IsNaN(actualSize.Width) ? actualSize.Width : this.Width;
                double height = !double.IsNaN(actualSize.Height) ? actualSize.Height : this.Height;

                // If the width and height are not null, return the size:
                if (!double.IsNaN(width) && !double.IsNaN(height))
                {
                    totalControlSize = new Size(width, height);
                    return true;
                }
            }
            return false;
        }

        void CalculateThumbTrackAndScrollableSizes(double totalControlSize, out double trackSize, out double thumbSize, out double scrollableSize, out double minValue, out double maxValue)
        {
            minValue = this.Minimum;
            maxValue = this.Maximum;
            double maxMinusMin = maxValue - minValue;

            trackSize = Math.Max(0d, totalControlSize - _smallDecreaseButtonSize - _smallIncreaseButtonSize); // Note: the "Track" is the area where the thumb moves.
            thumbSize = maxMinusMin > 0 ? (trackSize * this.ViewportSize / (maxMinusMin * 2)) : trackSize; // Note: the *2 factor was added to obtain the same result as in MS XAML, where for some reason, when for example the ViewportSize is the same as the MaxMinusMin, the thumb occupies half the track length.
            thumbSize = Math.Min(trackSize, Math.Max(MINIMUM_THUMB_SIZE, thumbSize));
            scrollableSize = trackSize - thumbSize; // Note: the "ScrollableSize" corresponds to the distance that the center of the thumb can travel.
        }

        void UpdateThumbPositionAndSize()
        {
            //First, check that the control was properly drawn and get its size:
            Size totalControlSize;
            if (CheckIfControlWasRenderedProperlyAndGetCurrentControlSize(out totalControlSize))
            {
                // Refresh the position of the thumb:
                double totalControlSizeInMainDirection = (this.Orientation == Orientation.Vertical ? totalControlSize.Height : totalControlSize.Width);
                UpdateThumbPositionAndSize(totalControlSizeInMainDirection);
            }
        }

        void UpdateThumbPositionAndSize(double totalControlSize)
        {
            // Note: in the arguments of this method, and this method implementation, by the word "Size" we mean either "Width" (if we are in horizontal orientation) or "Height" (if we are in vertical orientation).

            double value = this.Value;
            double trackSize; // Note: the "Track" is the area where the thumb moves.
            double thumbSize;
            double scrollableSize; // Note: the "ScrollableSize" corresponds to the distance that the center of the thumb can travel.
            double minValue;
            double maxValue;
            CalculateThumbTrackAndScrollableSizes(totalControlSize, out trackSize, out thumbSize, out scrollableSize, out minValue, out maxValue);
            double maxMinusMin = maxValue - minValue;

            double thumbCenterPosition = _smallDecreaseButtonSize + (maxMinusMin > 0 ? (thumbSize / 2) + (scrollableSize * (value - Minimum) / maxMinusMin) : trackSize / 2); // This is the X or Y position of the thumb (respectively for horizontal or vertical orientation).
            double thumbTopLeftPosition = thumbCenterPosition - (thumbSize / 2);
            double thumbBottomRightPosition = thumbCenterPosition + (thumbSize / 2);

            if (Orientation == Orientation.Vertical)
            {
                if (_verticalThumb != null
                    && _verticalLargeDecrease != null
                    && _verticalLargeIncrease != null)
                {
                    // Position the thumb:
                    _verticalThumb.Height = thumbSize;
                    Canvas.SetTop(_verticalThumb, thumbTopLeftPosition);

                    // Position the large decrease button:
                    _verticalLargeDecrease.Height = Math.Max(1d, thumbTopLeftPosition - _smallDecreaseButtonSize); //todo: replace 1d with 0d when fixed bug with Height=0.
                    Canvas.SetTop(_verticalLargeDecrease, _smallDecreaseButtonSize);

                    // Position the large increase button:
                    _verticalLargeIncrease.Height = Math.Max(1d, totalControlSize - _smallIncreaseButtonSize - thumbBottomRightPosition); //todo: replace 1d with 0d when fixed bug with Height=0.
                    Canvas.SetTop(_verticalLargeIncrease, thumbBottomRightPosition);
                }
            }
            else
            {
                if (_horizontalThumb != null
                    && _horizontalLargeDecrease != null
                    && _horizontalLargeIncrease != null)
                {
                    // Position the thumb:
                    _horizontalThumb.Width = thumbSize;
                    Canvas.SetLeft(_horizontalThumb, thumbTopLeftPosition);

                    // Position the large decrease button:
                    _horizontalLargeDecrease.Width = Math.Max(1d, thumbTopLeftPosition - _smallDecreaseButtonSize); //todo: replace 1d with 0d when fixed bug with Width=0.
                    Canvas.SetLeft(_horizontalLargeDecrease, _smallDecreaseButtonSize);

                    // Position the large increase button:
                    _horizontalLargeIncrease.Width = Math.Max(1d, totalControlSize - _smallIncreaseButtonSize - thumbBottomRightPosition); //todo: replace 1d with 0d when fixed bug with Width=0.
                    Canvas.SetLeft(_horizontalLargeIncrease, thumbBottomRightPosition);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the ScrollBar is displayed horizontally or vertically.
        /// The default is Horizontal.  Specific control templates might change this value, which would cause the templated value to be the apparent runtime default.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ScrollBar), new PropertyMetadata(Orientation.Vertical, Orientation_Changed));

        private static void Orientation_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Attempt to refresh all the elements inside the scrollbar:
            var scrollBar = (ScrollBar)d;
            scrollBar._controlWasProperlyDrawn = scrollBar.TryUpdateSizeAndPositionOfUIElements();
        }

        /// <summary>
        /// Gets or sets the amount of the scrollable content that is currently visible. The default is 0.
        /// </summary>
        public double ViewportSize
        {
            get { return (double)GetValue(ViewportSizeProperty); }
            set { SetValue(ViewportSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the ViewportSize dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportSizeProperty =
            DependencyProperty.Register("ViewportSize", typeof(double), typeof(ScrollBar), new PropertyMetadata(0d, ViewportSize_Changed));

        private static void ViewportSize_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Attempt to refresh all the elements inside the scrollbar:
            var scrollBar = (ScrollBar)d;
            scrollBar._controlWasProperlyDrawn = scrollBar.TryUpdateSizeAndPositionOfUIElements();
        }

        /// <summary>
        /// Occurs one or more times as content scrolls in a ScrollBar when the user moves the Thumb by using the mouse.
        /// </summary>
        public event ScrollEventHandler Scroll;
    }
}
