
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

using System.Windows.Input;
using System.Windows.Automation.Provider;
using OpenSilver.Internal;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents the base class for all button controls, such as <see cref="Button"/>,
    /// <see cref="RepeatButton"/>, and <see cref="HyperlinkButton"/>.
    /// </summary>
    public class ButtonBase : ContentControl
    {
        private WeakEventListener<ButtonBase, ICommand, EventArgs> _canExecuteChangedListener;
        private bool _commandDisabled;
        private bool _isMouseCaptured;
        private bool _isSpaceKeyDown;
        private bool _isMouseLeftButtonDown;
        private Point _mousePosition;
        private bool _suspendStateChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonBase"/> class.
        /// </summary>
        public ButtonBase()
        {
            // Attach the necessary events to their virtual counterparts
            Loaded += delegate { UpdateVisualState(false); };
            IsEnabledChanged += OnIsEnabledChanged;
        }

        /// <summary>
        /// Occurs when a <see cref="Button"/> is clicked.
        /// </summary>
        public event RoutedEventHandler Click;

        /// <summary>
        /// Identifies the <see cref="ClickMode"/> dependency property.
        /// </summary> 
        public static readonly DependencyProperty ClickModeProperty =
            DependencyProperty.Register(
                nameof(ClickMode),
                typeof(ClickMode),
                typeof(ButtonBase),
                new PropertyMetadata(ClickMode.Release),
                IsValidClickMode);

        /// <summary>
        /// Gets or sets when the <see cref="Click"/> event occurs.
        /// </summary>
        /// <returns>
        /// When the <see cref="Click"/> event occurs. The default value 
        /// <see cref="ClickMode.Release"/>.
        /// </returns>
        public ClickMode ClickMode
        {
            get => (ClickMode)GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }

        private static bool IsValidClickMode(object o)
        {
            ClickMode value = (ClickMode)o;
            return value == ClickMode.Press
                || value == ClickMode.Release
                || value == ClickMode.Hover;
        }

        private static readonly DependencyPropertyKey IsFocusedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsFocused),
                typeof(bool),
                typeof(ButtonBase),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Identifies the <see cref="IsFocused"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that determines whether the button has focus.
        /// </summary>
        /// <returns>
        /// true if the control has focus; otherwise, false. The default is false.
        /// </returns>
        public bool IsFocused
        {
            get => (bool)GetValue(IsFocusedProperty);
            private set => SetValue(IsFocusedPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsMouseOverPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsMouseOver),
                typeof(bool),
                typeof(ButtonBase),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Identifies the <see cref="IsMouseOver"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty = IsMouseOverPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating whether the mouse pointer is located over this button control.
        /// </summary>
        /// <returns>
        /// true to indicate the mouse pointer is over the button control, otherwise false.
        /// The default is false.
        /// </returns>
        public bool IsMouseOver
        {
            get => (bool)GetValue(IsMouseOverProperty);
            private set => SetValue(IsMouseOverPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsPressedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsPressed),
                typeof(bool),
                typeof(ButtonBase),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsPressedChanged));

        /// <summary>
        /// Identifies the <see cref="IsPressed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether a <see cref="ButtonBase"/> is currently 
        /// in a pressed state.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ButtonBase"/> is in a pressed state; otherwise false.
        /// The default is false.
        /// </returns>
        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            protected set => SetValue(IsPressedPropertyKey, value);
        }

        private static void OnIsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ButtonBase)d).OnIsPressedChanged(e);
        }

        /// <summary>
        /// Identifier for the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(ButtonBase),
                new PropertyMetadata(null, OnCommandParameterChanged));

        /// <summary>
        /// Gets or sets the parameter to pass to the <see cref="Command"/> property.
        /// </summary>
        /// <returns>
        /// The parameter to pass to the <see cref="Command"/> property. The default is null.
        /// </returns>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ButtonBase)d).UpdateCanExecute();
        }

        /// <summary>
        /// Identifier for the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(ButtonBase),
                new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Gets or sets the command to invoke when this button is pressed.
        /// </summary>
        /// <returns>
        /// The command to invoke when this button is pressed. The default is null.
        /// </returns>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonBase b = (ButtonBase)d;
            b.OnCommandChanged((ICommand)e.NewValue);
        }

        /// <summary>
        /// Raises the <see cref="Click"/> event.
        /// </summary>
        protected virtual void OnClick()
        {
            Click?.Invoke(this, new RoutedEventArgs
            {
                OriginalSource = this
            });

            ExecuteCommand();
        }

        /// <summary>
        /// Called when the value of the <see cref="IsPressed"/> property changes.
        /// </summary>
        /// <param name="e">
        /// The data for <see cref="DependencyPropertyChangedEventArgs"/>.
        /// </param>
        protected virtual void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) => UpdateVisualState();

        /// <summary>
        /// Builds the visual tree for the <see cref="ButtonBase" /> control when 
        /// a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState(false);
        }

        /// <inheritdoc />
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            IsFocused = true;

            UpdateVisualState();
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            IsFocused = false;

            _suspendStateChanges = true;
            try
            {
                if (ClickMode != ClickMode.Hover)
                {
                    IsPressed = false;
                    ReleaseMouseCaptureInternal();
                    _isSpaceKeyDown = false;
                }
            }
            finally
            {
                _suspendStateChanges = false;
                UpdateVisualState();
            }
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            if (OnKeyDownInternal(e.Key))
            {
                e.Handled = true;
            }
        }

        /// <summary> 
        /// Handles the KeyDown event for ButtonBase.
        /// </summary> 
        /// <param name="key">
        /// The keyboard key associated with the event.
        /// </param> 
        /// <returns>True if the event was handled, false otherwise.</returns>
        /// <remarks>
        /// This method exists for the purpose of unit testing since we can't 
        /// set KeyEventArgs.Key to simulate key press events. 
        /// </remarks>
        internal bool OnKeyDownInternal(Key key)
        {
            // True if the button will handle the event, false otherwise.
            bool handled = false;

            // Key presses can be ignored when disabled or in ClickMode.Hover
            if (IsEnabled && ClickMode != ClickMode.Hover)
            {
                // Hitting the SPACE key is equivalent to pressing the mouse
                // button 
                if (key == Key.Space)
                {
                    // Ignore the SPACE key if we already have the mouse 
                    // captured or if it had been pressed previously.
                    if (!_isMouseCaptured && !_isSpaceKeyDown)
                    {
                        _isSpaceKeyDown = true;
                        IsPressed = true;
                        CaptureMouseInternal();

                        if (ClickMode == ClickMode.Press)
                        {
                            OnClick();
                        }

                        handled = true;
                    }
                }
                // The ENTER key forces a click
                else if (key == Key.Enter)
                {
                    _isSpaceKeyDown = false;
                    IsPressed = false;
                    ReleaseMouseCaptureInternal();

                    OnClick();

                    handled = true;
                }
                // Any other keys pressed are irrelevant 
                else if (_isSpaceKeyDown)
                {
                    IsPressed = false;
                    _isSpaceKeyDown = false;
                    ReleaseMouseCaptureInternal();
                }
            }

            return handled;
        }

        /// <inheritdoc />
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Handled)
            {
                return;
            }

            if (OnKeyUpInternal(e.Key))
            {
                e.Handled = true;
            }
        }

        /// <summary> 
        /// Handles the KeyUp event for ButtonBase. 
        /// </summary>
        /// <param name="key">The keyboard key associated with the event.</param> 
        /// <returns>True if the event was handled, false otherwise.</returns>
        /// <remarks>
        /// This method exists for the purpose of unit testing since we can't 
        /// set KeyEventArgs.Key to simulate key press events.
        /// </remarks>
        internal bool OnKeyUpInternal(Key key)
        {
            // True if the button will handle the event, false otherwise.
            bool handled = false;

            // Key presses can be ignored when disabled or in ClickMode.Hover
            // or if any other key than SPACE was released. 
            if (IsEnabled && ClickMode != ClickMode.Hover && key == Key.Space)
            {
                _isSpaceKeyDown = false;

                if (!_isMouseLeftButtonDown)
                {
                    // If the mouse isn't in use, raise the Click event if we're
                    // in the correct click mode
                    ReleaseMouseCaptureInternal();
                    if (IsPressed && ClickMode == ClickMode.Release)
                    {
                        OnClick();
                    }

                    IsPressed = false;
                }
                else if (_isMouseCaptured)
                {
                    // Determine if the button should still be pressed based on
                    // the position of the mouse.
                    bool isValid = IsValidMousePosition();
                    IsPressed = isValid;
                    if (!isValid)
                    {
                        ReleaseMouseCaptureInternal();
                    }
                }

                handled = true;
            }

            return handled;
        }

        /// <inheritdoc />
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            IsMouseOver = true;

            _suspendStateChanges = true;
            try
            {
                if (ClickMode == ClickMode.Hover && IsEnabled)
                {
                    IsPressed = true;
                    OnClick();
                }
            }
            finally
            {
                _suspendStateChanges = false;
                UpdateVisualState();
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            IsMouseOver = false;

            _suspendStateChanges = true;

            try
            {
                if (ClickMode == ClickMode.Hover && IsEnabled)
                {
                    IsPressed = false;
                }
            }
            finally
            {
                _suspendStateChanges = false;
                UpdateVisualState();
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.Handled)
            {
                return;
            }

            _isMouseLeftButtonDown = true;

            if (!IsEnabled || ClickMode == ClickMode.Hover)
            {
                return;
            }

            e.Handled = true;
            _suspendStateChanges = true;
            try
            {
                Focus();

                CaptureMouseInternal();
                if (_isMouseCaptured)
                {
                    IsPressed = true;
                }
            }
            finally
            {
                _suspendStateChanges = false;
                UpdateVisualState();
            }

            if (ClickMode == ClickMode.Press)
            {
                OnClick();
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (e.Handled)
            {
                return;
            }

            _isMouseLeftButtonDown = false;

            if (!IsEnabled || ClickMode == ClickMode.Hover)
            {
                return;
            }

            e.Handled = true;
            if (!_isSpaceKeyDown && IsPressed && ClickMode == ClickMode.Release)
            {
                OnClick();
            }

            if (!_isSpaceKeyDown)
            {
                ReleaseMouseCaptureInternal();
                IsPressed = false;
            }
        }

        /// <inheritdoc />
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            ReleaseMouseCaptureInternal();
            IsPressed = false;
        }

        /// <inheritdoc />
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Cache the latest mouse position.
            _mousePosition = e.GetPosition(this);

            // Determine if the button is still pressed based on the mouse's
            // current position. 
            if (_isMouseLeftButtonDown &&
                IsEnabled &&
                ClickMode != ClickMode.Hover &&
                _isMouseCaptured &&
                !_isSpaceKeyDown)
            {
                IsPressed = IsValidMousePosition();
            }
        }

        /// <summary>
        /// Fetches the value of the IsEnabled property
        /// </summary>
        /// <remarks>
        /// The reason this property is overridden is so that Button
        /// can infuse the value for CanExecute into it.
        /// </remarks>
        internal override bool IsEnabledCore => base.IsEnabledCore && CanExecute;

        private bool CanExecute
        {
            get => !_commandDisabled;
            set
            {
                if (value != CanExecute)
                {
                    _commandDisabled = !value;
                    CoerceValue(IsEnabledProperty);
                }
            }
        }

        /// <summary>
        /// This method is called when button is clicked via <see cref="IInvokeProvider"/>.
        /// </summary>
        internal void AutomationButtonBaseClick() => OnClick();

        /// <summary>
        /// Update the current visual state of the button. 
        /// </summary>
        private void UpdateVisualState() => UpdateVisualState(true);

        /// <summary> 
        /// Update the current visual state of the button.
        /// </summary> 
        /// <param name="useTransitions">
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state. 
        /// </param>
        private void UpdateVisualState(bool useTransitions)
        {
            if (!_suspendStateChanges)
            {
                UpdateVisualStates();
            }
        }

        internal override void UpdateVisualStates()
        {
            if (!IsEnabled)
            {
                GoToState(VisualStates.StateDisabled);
            }
            else if (IsPressed)
            {
                GoToState(VisualStates.StatePressed);
            }
            else if (IsMouseOver)
            {
                GoToState(VisualStates.StateMouseOver);
            }
            else
            {
                GoToState(VisualStates.StateNormal);
            }

            if (IsFocused)
            {
                GoToState(VisualStates.StateFocused);
            }
            else
            {
                GoToState(VisualStates.StateUnfocused);
            }
        }

        /// <summary> 
        /// Capture the mouse. 
        /// </summary>
        private void CaptureMouseInternal()
        {
            if (!_isMouseCaptured)
            {
                _isMouseCaptured = CaptureMouse();
            }
        }

        /// <summary>
        /// Release mouse capture if we already had it. 
        /// </summary>
        private void ReleaseMouseCaptureInternal()
        {
            ReleaseMouseCapture();
            _isMouseCaptured = false;
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _suspendStateChanges = true;

            try
            {
                if (!IsEnabled)
                {
                    IsPressed = false;
                    IsMouseOver = false;
                    _isMouseCaptured = false;
                    _isSpaceKeyDown = false;
                    _isMouseLeftButtonDown = false;
                }
            }
            finally
            {
                _suspendStateChanges = false;
                UpdateVisualState();
            }
        }

        /// <summary>
        /// Determine if the mouse is above the button based on its last known 
        /// position.
        /// </summary>
        /// <returns> 
        /// True if the mouse is considered above the button, false otherwise. 
        /// </returns>
        private bool IsValidMousePosition()
        {
            return _mousePosition.X >= 0.0 &&
                _mousePosition.X <= ActualWidth &&
                _mousePosition.Y >= 0.0 &&
                _mousePosition.Y <= ActualHeight;
        }

        private void OnCommandChanged(ICommand newCommand)
        {
            if (_canExecuteChangedListener != null)
            {
                _canExecuteChangedListener.Detach();
                _canExecuteChangedListener = null;
            }

            if (newCommand != null)
            {
                _canExecuteChangedListener = new(this, newCommand)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnCanExecuteChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.CanExecuteChanged -= listener.OnEvent,
                };

                newCommand.CanExecuteChanged += _canExecuteChangedListener.OnEvent;
            }

            UpdateCanExecute();
        }

        private void OnCanExecuteChanged(object sender, EventArgs e) => UpdateCanExecute();

        private void UpdateCanExecute()
        {
            if (Command != null)
            {
                CanExecute = Command.CanExecute(CommandParameter);
            }
            else
            {
                CanExecute = true;
            }
        }

        private void ExecuteCommand()
        {
            if (Command != null)
            {
                if (Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }
        }
    }
}
