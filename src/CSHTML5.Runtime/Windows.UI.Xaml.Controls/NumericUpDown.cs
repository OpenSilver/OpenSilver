
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



using System;
using System.Globalization;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.System;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Determins whether the value bar behind the value text should be visible
    /// </summary>
    public enum NumericUpDownValueBarVisibility
    {
        /// <summary>
        /// Visible
        /// </summary>
        Visible,
        /// <summary>
        /// Collapsed
        /// </summary>
        Collapsed
    }

    /// <summary>
    /// NumericUpDown control - for representing values that can be
    /// entered with keyboard,
    /// using increment/decrement buttons
    /// as well as swiping over the control.
    /// </summary>
    [TemplatePart(Name = ValueTextBoxName, Type = typeof(TextBox))]
    [TemplatePart(Name = ValueBarName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DragOverlayName, Type = typeof(UIElement))]
    [TemplatePart(Name = DecrementButtonName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = IncrementButtonName, Type = typeof(RepeatButton))]
    [TemplateVisualState(GroupName = "IncrementalButtonStates", Name = "IncrementEnabled")]
    [TemplateVisualState(GroupName = "IncrementalButtonStates", Name = "IncrementDisabled")]
    [TemplateVisualState(GroupName = "DecrementalButtonStates", Name = "DecrementEnabled")]
    [TemplateVisualState(GroupName = "DecrementalButtonStates", Name = "DecrementDisabled")]
    public class NumericUpDown : RangeBase
    {
        private const string DecrementButtonName = "PART_DecrementButton";
        private const string IncrementButtonName = "PART_IncrementButton";
        private const string DragOverlayName = "PART_DragOverlay";
        private const string ValueTextBoxName = "PART_ValueTextBox";
        private const string ValueBarName = "PART_ValueBar";
        private UIElement _dragOverlay;
        private UpDownTextBox _valueTextBox;
        private RepeatButton _decrementButton;
        private RepeatButton _incrementButton;
        private FrameworkElement _valueBar;
        private bool _isDragUpdated;
        private bool _isChangingTextWithCode;
        private bool _isChangingValueWithCode;
        private double _unusedManipulationDelta;

        private bool _isDraggingWithMouse;
#if UWP
        private MouseDevice _mouseDevice;
#endif
        private const double MinMouseDragDelta = 2;
        private double _totalDeltaX;
        private double _totalDeltaY;

#if UWP
        #region ValueFormat
        /// <summary>
        /// ValueFormat Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register(
                "ValueFormat",
                typeof(string),
                typeof(NumericUpDown),
                new PropertyMetadata("F2", OnValueFormatChanged));

        /// <summary>
        /// Gets or sets the ValueFormat property. This dependency property 
        /// indicates the format of the value string.
        /// </summary>
        public string ValueFormat
        {
            get { return (string)this.GetValue(ValueFormatProperty); }
            set { this.SetValue(ValueFormatProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ValueFormat property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnValueFormatChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (NumericUpDown)d;
            target.OnValueFormatChanged();
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ValueFormat property.
        /// </summary>
        private void OnValueFormatChanged()
        {
            this.UpdateValueText();
        }
        #endregion
#endif

        #region ValueBarVisibility
        /// <summary>
        /// ValueBarVisibility Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueBarVisibilityProperty =
            DependencyProperty.Register(
                "ValueBarVisibility",
                typeof(NumericUpDownValueBarVisibility),
                typeof(NumericUpDown),
                new PropertyMetadata(NumericUpDownValueBarVisibility.Visible, OnValueBarVisibilityChanged));

        /// <summary>
        /// Gets or sets the ValueBarVisibility property. This dependency property 
        /// indicates whether the value bar should be shown.
        /// </summary>
        public NumericUpDownValueBarVisibility ValueBarVisibility
        {
            get { return (NumericUpDownValueBarVisibility)this.GetValue(ValueBarVisibilityProperty); }
            set { this.SetValue(ValueBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ValueBarVisibility property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnValueBarVisibilityChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (NumericUpDown)d;
            target.OnValueBarVisibilityChanged();
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ValueBarVisibility property.
        /// </summary>
        private void OnValueBarVisibilityChanged()
        {
            this.UpdateValueBar();
        }
        #endregion

        #region IsReadOnly
        /// <summary>
        /// IsReadOnly Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(NumericUpDown),
                new PropertyMetadata(false, OnIsReadOnlyChanged));

        /// <summary>
        /// Gets or sets the IsReadOnly property. This dependency property 
        /// indicates whether the box should only allow to read the values by copying and pasting them.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsReadOnly property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsReadOnlyChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (NumericUpDown)d;
            target.OnIsReadOnlyChanged();
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the IsReadOnly property.
        /// </summary>
        private void OnIsReadOnlyChanged()
        {
            this.UpdateIsReadOnlyDependants();
        }
        #endregion

        #region DragSpeed
        /// <summary>
        /// DragSpeed Dependency Property
        /// </summary>
        public static readonly DependencyProperty DragSpeedProperty =
            DependencyProperty.Register(
                "DragSpeed",
                typeof(double),
                typeof(NumericUpDown),
                new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the DragSpeed property. This dependency property 
        /// indicates the speed with which the value changes when manipulated with dragging.
        /// The default value of double.NaN indicates that the value will change by (Maximum - Minimum),
        /// when the control is dragged a full screen length.
        /// </summary>
        public double DragSpeed
        {
            get { return (double)this.GetValue(DragSpeedProperty); }
            set { this.SetValue(DragSpeedProperty, value); }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericUpDown" /> class.
        /// </summary>
        public NumericUpDown()
        {
#if UWP
            this.DefaultStyleKey = typeof(NumericUpDown);
#else
            // Set default style:
            this.DefaultStyleKey = typeof(NumericUpDown);
#endif
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_dragOverlay != null)
            {
#if UWP
                Window.Current.CoreWindow.PointerReleased += this.CoreWindowOnPointerReleased;
                Window.Current.CoreWindow.VisibilityChanged += this.OnCoreWindowVisibilityChanged;
#else
#if MIGRATION
                Window.Current.MouseLeftButtonUp += this.CoreWindowOnPointerReleased;
#else
                Window.Current.PointerReleased += this.CoreWindowOnPointerReleased;
#endif
#endif
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
#if UWP
            Window.Current.CoreWindow.PointerReleased -= this.CoreWindowOnPointerReleased;
            Window.Current.CoreWindow.VisibilityChanged -= this.OnCoreWindowVisibilityChanged;
#else
#if MIGRATION
            Window.Current.MouseLeftButtonUp -= this.CoreWindowOnPointerReleased;
#else
            Window.Current.PointerReleased -= this.CoreWindowOnPointerReleased;
#endif
#endif
        }

        #region OnApplyTemplate()
        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest terms, this means the method is called just before a UI element displays in your app. Override this method to influence the default post-template logic of a class.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

#if UWP
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }
#endif

            this.GotFocus += this.OnGotFocus;
            this.LostFocus += this.OnLostFocus;
#if UWP
            this.PointerWheelChanged += this.OnPointerWheelChanged;
#endif
            _valueTextBox = this.GetTemplateChild(ValueTextBoxName) as UpDownTextBox;
            _dragOverlay = this.GetTemplateChild(DragOverlayName) as UIElement;
            _decrementButton = this.GetTemplateChild(DecrementButtonName) as RepeatButton;
            _incrementButton = this.GetTemplateChild(IncrementButtonName) as RepeatButton;
            _valueBar = this.GetTemplateChild(ValueBarName) as FrameworkElement;

            if (_valueTextBox != null)
            {
                _valueTextBox.LostFocus += this.OnValueTextBoxLostFocus;
                _valueTextBox.GotFocus += this.OnValueTextBoxGotFocus;
#if UWP
                _valueTextBox.Text = this.Value.ToString(CultureInfo.CurrentCulture);
#else
                _valueTextBox.Text = this.Value.ToString();
#endif
                _valueTextBox.TextChanged += this.OnValueTextBoxTextChanged;
                _valueTextBox.KeyDown += this.OnValueTextBoxKeyDown;
                _valueTextBox.UpPressed += (s, e) => this.DoIncrement();
                _valueTextBox.DownPressed += (s, e) => this.DoDecrement();
#if UWP
                _valueTextBox.PointerExited += this.OnValueTextBoxPointerExited;
#endif
            }

            if (_dragOverlay != null)
            {
#if UWP
                _dragOverlay.Tapped += this.OnDragOverlayTapped;
                _dragOverlay.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
#endif
#if MIGRATION
                _dragOverlay.MouseLeftButtonDown += this.OnDragOverlayPointerPressed;
                _dragOverlay.MouseLeftButtonUp += this.OnDragOverlayPointerReleased;
                _dragOverlay.LostMouseCapture += this.OnDragOverlayPointerCaptureLost;
#else
                _dragOverlay.PointerPressed += this.OnDragOverlayPointerPressed;
                _dragOverlay.PointerReleased += this.OnDragOverlayPointerReleased;
                _dragOverlay.PointerCaptureLost += this.OnDragOverlayPointerCaptureLost;
#endif
            }

            if (_decrementButton != null)
            {
                _decrementButton.Click += this.OnDecrementButtonClick;
#if UWP
                var pcc =
                    new PropertyChangeEventSource<bool>
                        (_decrementButton, "IsPressed");
                pcc.ValueChanged += this.OnDecrementButtonIsPressedChanged;
#endif
            }

            if (_incrementButton != null)
            {
                _incrementButton.Click += this.OnIncrementButtonClick;
#if UWP
                var pcc =
                    new PropertyChangeEventSource<bool>
                        (_incrementButton, "IsPressed");
                pcc.ValueChanged += this.OnIncrementButtonIsPressedChanged;
#endif
            }

            if (_valueBar != null)
            {
                _valueBar.SizeChanged += this.OnValueBarSizeChanged;

                this.UpdateValueBar();
            }

            this.UpdateIsReadOnlyDependants();
            this.SetValidIncrementDirection();
        }

#if UWP
        private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            if (!_hasFocus)
            {
                return;
            }

            var delta = pointerRoutedEventArgs.GetCurrentPoint(this).Properties.MouseWheelDelta;

            if (delta < 0)
            {
                this.Decrement();
            }
            else
            {
                this.Increment();
            }

            pointerRoutedEventArgs.Handled = true;
        }
#endif

        private bool _hasFocus;
        private const double Epsilon = .00001;

        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            _hasFocus = false;
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            _hasFocus = true;
        }

        private void OnValueTextBoxTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            this.UpdateValueFromText();
        }

#if MIGRATION
        private void OnValueTextBoxKeyDown(object sender, KeyEventArgs e)
#else
        private void OnValueTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
#endif
        {
#if MIGRATION
            if (e.Key == Key.Enter)
#else
            if (e.Key == VirtualKey.Enter)
#endif
            {
                if (this.UpdateValueFromText())
                {
                    this.UpdateValueText();
                    _valueTextBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        private bool UpdateValueFromText()
        {
            if (_isChangingTextWithCode)
            {
                return false;
            }

            double val;

#if UWP
            if (double.TryParse(_valueTextBox.Text, NumberStyles.Any, CultureInfo.CurrentUICulture, out val) ||
                Calculator.TryCalculate(_valueTextBox.Text, out val))
#else
            if (double.TryParse(_valueTextBox.Text, out val))
#endif
            {
                _isChangingValueWithCode = true;
                this.SetValueAndUpdateValidDirections(val);
                _isChangingValueWithCode = false;

                return true;
            }

            return false;
        }

        #endregion

        #region Button event handlers
#if UWP
        private void OnDecrementButtonIsPressedChanged(object decrementButton, bool isPressed)
        {
            // TODO: The thinking was to handle speed and acceleration of value changes manually on a regular Button when it is pressed.
            // Currently just using RepeatButtons
        }

        private void OnIncrementButtonIsPressedChanged(object incrementButton, bool isPressed)
        {
        }
#endif
        private void OnDecrementButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
#if UWP
            if (Window.Current.CoreWindow.IsInputEnabled)
            {
#endif
                this.DoDecrement();
#if UWP
            }
#endif
        }

        private void OnIncrementButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
#if UWP
            if (Window.Current.CoreWindow.IsInputEnabled)
            {
#endif
                this.DoIncrement();
#if UWP
            }
#endif
        }
        #endregion

        /// <summary>
        /// Decrements the value by Increment.
        /// </summary>
        /// <returns><c>true</c> if the value was decremented by exactly <c>Increment</c>; <c>false</c> if it was constrained.</returns>
        protected bool DoDecrement()
        {
#if !BRIDGE
            return this.SetValueAndUpdateValidDirections(Math.Round(value: this.Value - this.Increment, digits: 5));
#else
            return this.SetValueAndUpdateValidDirections(Math.Round(d: this.Value - this.Increment, digits: 5));
#endif
        }

        protected bool DoIncrement()
        {
#if !BRIDGE
            return this.SetValueAndUpdateValidDirections(Math.Round(value: this.Value + this.Increment, digits: 5));
#else
            return this.SetValueAndUpdateValidDirections(Math.Round(d: this.Value + this.Increment, digits: 5));
#endif
        }

        /// <summary>
        /// Gets or sets a value added or subtracted from the value property. The default values is one.
        /// </summary>
        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }
        /// <summary>
        /// The identifier for the Increment dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(NumericUpDown), new PropertyMetadata(1.0));

        private void OnValueTextBoxGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_dragOverlay != null)
            {
                _dragOverlay.IsHitTestVisible = false;
            }

            _valueTextBox.SelectAll();
        }

        private void OnValueTextBoxLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
#if UWP
            if (_dragOverlay != null)
            {
                _dragOverlay.IsHitTestVisible = true;
                this.UpdateValueText();
            }
#else
            //---------------------------------------------------------------
            // If the value of the TextBox is invalid or out of range
            // (note: value coercion is handled by the base RangeBase class),
            // we need to update the TextBox to reflect the current Value:
            //---------------------------------------------------------------
            double val;
            if (_valueTextBox != null && (!double.TryParse(_valueTextBox.Text, out val) || val != this.Value))
            {
                UpdateValueText();
            }
#endif
        }

#if MIGRATION
        private void OnDragOverlayPointerPressed(object sender, MouseEventArgs e)
#else
        private void OnDragOverlayPointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
#if UWP
#if MIGRATION
            _dragOverlay.CaptureMouse();
#else
            _dragOverlay.CapturePointer(e.Pointer);
#endif
#endif
            _totalDeltaX = 0;
            _totalDeltaY = 0;

#if UWP
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _isDraggingWithMouse = true;
                _mouseDevice = MouseDevice.GetForCurrentView();
                _mouseDevice.MouseMoved += this.OnMouseDragged;
                Window.Current.CoreWindow.PointerCursor = null;
            }
            else
            {
                _dragOverlay.ManipulationDelta += this.OnDragOverlayManipulationDelta;
            }
#endif
        }

#if UWP
        private void CoreWindowOnPointerReleased(CoreWindow sender, PointerEventArgs args)
#else
#if MIGRATION
        private void CoreWindowOnPointerReleased(object sender, MouseEventArgs args)
#else
        private void CoreWindowOnPointerReleased(object sender, PointerRoutedEventArgs args)
#endif
#endif
        {
            if (_isDragUpdated)
            {
                args.Handled = true;
                this.ResumeValueTextBoxTabStopAsync();
            }
        }

#if UWP
        private void SuspendValueTextBoxTabStop()
        {
            if (_valueTextBox != null)
            {
                _valueTextBox.IsTabStop = false;
            }
        }
#endif

        private async void ResumeValueTextBoxTabStopAsync()
        {
            // We need to wait for just a bit to allow manipulation events to complete.
            // It's a bit hacky, but it's the simplest solution.
            await Task.Delay(100);

#if UWP
            if (_valueTextBox != null)
            {
                _valueTextBox.IsTabStop = true;
            }
#endif
        }

#if MIGRATION
        private void OnDragOverlayPointerReleased(object sender, MouseEventArgs args)
#else
        private void OnDragOverlayPointerReleased(object sender, PointerRoutedEventArgs args)
#endif
        {
            this.EndDragging(args);
        }

#if MIGRATION
        private void OnDragOverlayPointerCaptureLost(object sender, MouseEventArgs args)
#else
        private void OnDragOverlayPointerCaptureLost(object sender, PointerRoutedEventArgs args)
#endif
        {
            this.EndDragging(args);
        }

#if MIGRATION
        private void EndDragging(MouseEventArgs args)
#else
        private void EndDragging(PointerRoutedEventArgs args)
#endif
        {
#if UWP
            if (_isDraggingWithMouse)
            {
                _isDraggingWithMouse = false;
                _mouseDevice.MouseMoved -= this.OnMouseDragged;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeAll, 1);
                _mouseDevice = null;
            }
            else if (_dragOverlay != null)
            {
                _dragOverlay.ManipulationDelta -= this.OnDragOverlayManipulationDelta;
            }
#endif
            if (_isDragUpdated)
            {
                if (args != null)
                {
                    args.Handled = true;
                }

                this.ResumeValueTextBoxTabStopAsync();
            }
        }

#if UWP
        private void OnCoreWindowVisibilityChanged(CoreWindow sender, VisibilityChangedEventArgs args)
        {
            // There are cases where pointer isn't getting released - this should hopefully end dragging too.
            if (!args.Visible)
            {
#pragma warning disable 4014
                this.EndDragging(null);
#pragma warning restore 4014
            }
        }
#endif

#if UWP
        private void OnMouseDragged(MouseDevice sender, MouseEventArgs args)
        {
            var dx = args.MouseDelta.X;
            var dy = args.MouseDelta.Y;

            if (dx > 200 || dx < -200 || dy > 200 || dy < -200)
            {
                return;
            }

            _totalDeltaX += dx;
            _totalDeltaY += dy;

            if (_totalDeltaX > MinMouseDragDelta ||
                _totalDeltaX < -MinMouseDragDelta ||
                _totalDeltaY > MinMouseDragDelta ||
                _totalDeltaY < -MinMouseDragDelta)
            {
                this.UpdateByDragging(_totalDeltaX, _totalDeltaY);
                _totalDeltaX = 0;
                _totalDeltaY = 0;
            }
        }

        private void OnDragOverlayManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs manipulationDeltaRoutedEventArgs)
        {
            var dx = manipulationDeltaRoutedEventArgs.Delta.Translation.X;
            var dy = manipulationDeltaRoutedEventArgs.Delta.Translation.Y;

            if (this.UpdateByDragging(dx, dy))
                return;

            manipulationDeltaRoutedEventArgs.Handled = true;
        }

        private bool UpdateByDragging(double dx, double dy)
        {
            if (!this.IsEnabled ||
                this.IsReadOnly ||
                // ReSharper disable CompareOfFloatsByEqualityOperator
                dx == 0 && dy == 0)
            // ReSharper restore CompareOfFloatsByEqualityOperator
            {
                return false;
            }

            double delta;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                delta = dx;
            }
            else
            {
                delta = -dy;
            }

            this.ApplyManipulationDelta(delta);

#if UWP
            this.SuspendValueTextBoxTabStop();
#endif

            _isDragUpdated = true;

            return true;
        }

#endif
        private void ApplyManipulationDelta(double delta)
        {
            if (Math.Sign(delta) == Math.Sign(_unusedManipulationDelta))
                _unusedManipulationDelta += delta;
            else
                _unusedManipulationDelta = delta;

            if (_unusedManipulationDelta <= 0 && this.Value == this.Minimum)
            {
                _unusedManipulationDelta = 0;
                return;
            }

            if (_unusedManipulationDelta >= 0 && this.Value == this.Maximum)
            {
                _unusedManipulationDelta = 0;
                return;
            }

            double smallerScreenDimension;

            if (Window.Current != null)
            {
                smallerScreenDimension = Math.Min(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
            }
            else
            {
                smallerScreenDimension = 768;
            }

            var speed = this.DragSpeed;

            if (double.IsNaN(speed) ||
                double.IsInfinity(speed))
            {
                speed = this.Maximum - this.Minimum;
            }

            if (double.IsNaN(speed) ||
                double.IsInfinity(speed))
            {
                speed = double.MaxValue;
            }

            var screenAdjustedDelta = speed * _unusedManipulationDelta / smallerScreenDimension;
            this.SetValueAndUpdateValidDirections(this.Value + screenAdjustedDelta);
            _unusedManipulationDelta = 0;
        }

#if UWP
        private void OnDragOverlayTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            if (this.IsEnabled &&
                _valueTextBox != null &&
                _valueTextBox.IsTabStop)
            {
                _valueTextBox.Focus(FocusState.Programmatic);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.IBeam, 0);
            }
        }
        private void OnValueTextBoxPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (Window.Current.CoreWindow.PointerCursor.Type == CoreCursorType.IBeam)
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            }
        }
#endif

        /// <summary>
        /// Fires the ValueChanged routed event.
        /// </summary>
        /// <param name="oldValue">Old value of the Value property.</param>
        /// <param name="newValue">New value of the Value property.</param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            this.UpdateValueBar();

            if (!_isChangingValueWithCode)
            {
                this.UpdateValueText();
            }
        }

        private void UpdateValueBar()
        {
            if (_valueBar == null)
                return;

            var effectiveValueBarVisibility = this.ValueBarVisibility;

            if (effectiveValueBarVisibility == NumericUpDownValueBarVisibility.Collapsed)
            {
                _valueBar.Visibility = Visibility.Collapsed;

                return;
            }

#if UWP
            _valueBar.Clip =
                new RectangleGeometry
                {
                    Rect = new Rect
                    {
                        X = 0,
                        Y = 0,
                        Height = _valueBar.ActualHeight,
                        Width = _valueBar.ActualWidth * (this.Value - this.Minimum) / (this.Maximum - this.Minimum)
                    }
                };
#endif

            //_valueBar.Width =
            //    _valueTextBox.ActualWidth * (Value - Minimum) / (Maximum - Minimum);
        }

        private void OnValueBarSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            this.UpdateValueBar();
        }

        private void UpdateValueText()
        {
            if (_valueTextBox != null)
            {
                _isChangingTextWithCode = true;
#if UWP
                _valueTextBox.Text = this.Value.ToString(this.ValueFormat);
#else
                _valueTextBox.Text = this.Value.ToString();
#endif
                _isChangingTextWithCode = false;
            }
        }

        private void UpdateIsReadOnlyDependants()
        {
            if (_decrementButton != null)
            {
                _decrementButton.Visibility = this.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            }

            if (_incrementButton != null)
            {
                _incrementButton.Visibility = this.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private bool SetValueAndUpdateValidDirections(double value)
        {
            // Range coercion is handled by base class.
            var oldValue = this.Value;
            this.Value = value;
            this.SetValidIncrementDirection();

            return Math.Abs(this.Value - oldValue) > Epsilon;
        }

        private void SetValidIncrementDirection()
        {
            VisualStateManager.GoToState(this, this.Value < this.Maximum ? "IncrementEnabled" : "IncrementDisabled", true);
            VisualStateManager.GoToState(this, this.Value > this.Minimum ? "DecrementEnabled" : "DecrementDisabled", true);
        }
    }
}