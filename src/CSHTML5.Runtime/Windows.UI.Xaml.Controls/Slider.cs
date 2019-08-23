
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace System.Windows.Controls
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;

namespace Windows.UI.Xaml.Controls
#endif
{ 
public class Slider : RangeBase
    {
        //todo: Clean up this class (even functionality-wise): It was quickly made by copy-pasting the code from ScrollBar.cs and making minor changes.
        //      Changes to do:  - change _horizontal/_verticalRoot into Grids so we can position stuff in it more easily OR keep them as Canvas but programatically position their children depending on their Horizontal/VerticalAlignment. This is to allow templating because as it is, what the user can do is VERY limited.
        //                      - remove overly-complicated stuff (for example, the thumb-size is independent of the size of the container, unless the template designed by the user makes it dependant on it so remove anything that calculates the thumb size).
        //                      - remove useless stuff: I didn't take enough time to understand half of what is hapenning in this class so there are probably useless things that were left.
        //                      - put back areas for changing the value by clicking on the Slider outside of the Thumb (Note: it changes the value by LargeChange that comes from RangeBase)

        const double MINIMUM_THUMB_SIZE = 9d;
        bool _controlWasProperlyDrawn;
        double _valueWithoutCoercion; // Note: Normally we keep the value within the Min/Max range (this is called "coercion"). However, while dragging the thumb, we store the "ValueWithoutCoercion" so that when the user drags the pointer beyond the Slider limits and then goes back, the "going back" movement does not modify the actual value until the pointer is again inside the Slider range.

        // Horizontal elements:
        Canvas _horizontalRoot;
        Thumb _horizontalThumb;
        Rectangle _horizontalRail;

        // Vertical elements:
        Canvas _verticalRoot;
        Thumb _verticalThumb;
        Rectangle _verticalRail;

        /// <summary>
        /// Initializes a new instance of the Slider class.
        /// </summary>
        public Slider()
        {
            // Set default style:
            this.DefaultStyleKey = typeof(Slider);

            // Register some events:
            this.Unloaded += Slider_Unloaded;
            this.SizeChanged += Slider_SizeChanged;
        }

        void Slider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Note: this handler is also called the first time that the element is added to the visual tree.

            // Update the position of all the elements:
            if (TryUpdateSizeAndPositionOfUIElements())
                _controlWasProperlyDrawn = true;
        }

        void Slider_Unloaded(object sender, RoutedEventArgs e)
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
            _horizontalRail = this.GetTemplateChild("HorizontalRail") as Rectangle;

            _verticalRoot = this.GetTemplateChild("VerticalRoot") as Canvas;
            _verticalThumb = this.GetTemplateChild("VerticalThumb") as Thumb;
            _verticalRail = this.GetTemplateChild("VerticalRail") as Rectangle;

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

        void OnDragStarted()
        {
            // Reset the "ValueWithoutCoercion" (read the note near the definition of that variable to better understand what it is used for):
            _valueWithoutCoercion = this.Value;
        }

        void OnDragCompleted()
        {
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

                // Calculate what should be the new "value" of the Slider, while ensuring that it does not get out of the Min/Max range:
                _valueWithoutCoercion += valueDelta; // Note: read the note near the definition of the "_valueWithoutCoercion" variable to better understand what it is used for. 
                double newValue = CoerceValue(_valueWithoutCoercion); // Note: "Coerce" means that we ensure that it stays within the Min/Max range.

                // Apply the change to the value of the Slider, and update the Thumb position accordingly:
                if (newValue != this.Value)
                {
                    this.SetLocalValue(RangeBase.ValueProperty, newValue); // Note: we do not use "this.Value = newValue" because it deletes any bindings that the user may have set to the Slider with <Slider Value="{Binding...}"/>.

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
            // Position the elements that are inside the Slider:
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
                        && _verticalRail != null)
                    {
                        //--------------------------------
                        // Update the size of the UI elements:
                        //--------------------------------
                        _verticalThumb.Width = totalControlSize.Width;

                        double buttonsHeight = totalControlSize.Width; // Deliberately square.

                        //--------------------------------
                        // Update the position of the thumb and of the large increment/decrement areas:
                        //--------------------------------
                        UpdateThumbPositionAndSize(totalControlSize.Height);
                        Canvas.SetLeft(_verticalRail, (totalControlSize.Width - _verticalRail.ActualWidth) / 2);
                        Canvas.SetLeft(_verticalThumb, (totalControlSize.Width - _verticalThumb.ActualWidth) / 2);

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
                        && _horizontalRail != null)
                    {
                        //--------------------------------
                        // Update the size of the UI elements:
                        //--------------------------------
                        _horizontalThumb.Height = totalControlSize.Height;

                        double buttonsWidth = totalControlSize.Height; // Deliberately square.

                        //--------------------------------
                        // Update the position of the thumb and of the large increment/decrement areas:
                        //--------------------------------
                        UpdateThumbPositionAndSize(totalControlSize.Width);
                        Canvas.SetTop(_horizontalRail, (totalControlSize.Height - _horizontalRail.ActualHeight) / 2);
                        Canvas.SetTop(_horizontalThumb, (totalControlSize.Height - _horizontalThumb.ActualHeight) / 2);
                        return true;
                    }
                }
            }

            return false;
        }

        bool CheckIfControlWasRenderedProperlyAndGetCurrentControlSize(out Size totalControlSize)
        {
            if (_controlWasProperlyDrawn) // Note: this is true when all the internal sizes of the control have been calculated
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

            trackSize = Math.Max(0d, totalControlSize); // Note: the "Track" is the area where the thumb moves.
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

            double thumbCenterPosition = (maxMinusMin > 0 ? (thumbSize / 2) + (scrollableSize * (value - Minimum) / maxMinusMin) : trackSize / 2); // This is the X or Y position of the thumb (respectively for horizontal or vertical orientation).
            double thumbTopLeftPosition = thumbCenterPosition - (thumbSize / 2);
            double thumbBottomRightPosition = thumbCenterPosition + (thumbSize / 2);

            if (Orientation == Orientation.Vertical)
            {
                if (_verticalThumb != null)
                {
                    // Position the thumb:
                    _verticalThumb.Height = thumbSize;
                    Canvas.SetTop(_verticalThumb, thumbTopLeftPosition);
                }
            }
            else
            {
                if (_horizontalThumb != null)
                {
                    // Position the thumb:
                    _horizontalThumb.Width = thumbSize;
                    Canvas.SetLeft(_horizontalThumb, thumbTopLeftPosition);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the Slider is displayed horizontally or vertically.
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
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Slider), new PropertyMetadata(Orientation.Vertical, Orientation_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void Orientation_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Attempt to refresh all the elements inside the Slider:
            var slider = (Slider)d;
            slider._controlWasProperlyDrawn = slider.TryUpdateSizeAndPositionOfUIElements();
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
            DependencyProperty.Register("ViewportSize", typeof(double), typeof(Slider), new PropertyMetadata(0d, ViewportSize_Changed) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        private static void ViewportSize_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Attempt to refresh all the elements inside the Slider:
            var slider = (Slider)d;
            slider._controlWasProperlyDrawn = slider.TryUpdateSizeAndPositionOfUIElements();
        }

        /// <summary>
        /// Occurs one or more times as content scrolls in a Slider when the user moves the Thumb by using the mouse.
        /// </summary>
        public event ScrollEventHandler Scroll;
    }
}
