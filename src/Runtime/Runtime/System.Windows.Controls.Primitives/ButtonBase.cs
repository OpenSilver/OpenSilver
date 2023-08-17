﻿
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
using System.Windows.Input;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Provider;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents the base class for all button controls, such as Button, RepeatButton,
    /// and HyperlinkButton.
    /// </summary>
    public class ButtonBase : ContentControl
    {
        private WeakEventListener<ButtonBase, ICommand, EventArgs> _canExecuteChangedListener;

        public ButtonBase()
        {
            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Interval = new TimeSpan(0, 0, 5); // See comment where this variable is defined.
            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Tick += TimerToReleaseCaptureAutomaticallyIfNoMouseUpEvent_Tick;
            IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            Loaded += (o, e) => UpdateVisualStates();
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (!IsEnabled)
                {
                    IsPressed = false;
                    IsMouseOver = false;
                }
            }
            finally
            {
                UpdateVisualStates();
            }
        }

        /// <summary>
        ///     Fetches the value of the IsEnabled property
        /// </summary>
        /// <remarks>
        ///     The reason this property is overridden is so that Button
        ///     can infuse the value for CanExecute into it.
        /// </remarks>
        internal override bool IsEnabledCore
        {
            get
            {
                return base.IsEnabledCore && CanExecute;
            }
        }

        #region Command, CommandParameter Properties

        /// <summary>
        /// Gets or sets the parameter to pass to the <see cref="Command"/> property.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ButtonBase.CommandParameter"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(ButtonBase),
                new PropertyMetadata(null, OnCommandParameterChanged));

        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ButtonBase)d).UpdateCanExecute();
        }

        /// <summary>
        /// Gets or sets the command to invoke when this button is pressed. 
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(ButtonBase),
                new PropertyMetadata(null, OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonBase b = (ButtonBase)d;
            b.OnCommandChanged((ICommand)e.NewValue);
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

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            UpdateCanExecute();
        }

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

        private bool CanExecute
        {
            get { return !CommandDisabled; }
            set
            {
                if (value != CanExecute)
                {
                    CommandDisabled = !value;
                    CoerceValue(IsEnabledProperty);
                }
            }
        }

        private bool CommandDisabled;

        #endregion Command, CommandParameter Properties

        #region Click event

        // The following Timer serves as a workaround to the issue that happens if you press the
        // button, then move the pointer outside the application, and release it. In that case,
        // the "PointerReleased" event may not be called, which may lead to the pointer capture
        // never being released. Therefore we automatically release the capture after a few seconds.
        DispatcherTimer _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent = new DispatcherTimer();

        /// <summary>
        /// Occurs when a button control is clicked.
        /// </summary>
        public event RoutedEventHandler Click;

        /// <summary>
        /// Raises the Click event
        /// </summary>
        protected virtual void OnClick()
        {
            if (Click != null)
            {
                Click(this, new RoutedEventArgs 
                { 
                    OriginalSource = this 
                });
            }

            ExecuteCommand();
        }

        /// <summary>
        /// This method is called when button is clicked via <see cref="IInvokeProvider"/>.
        /// </summary>
        internal void AutomationButtonBaseClick()
        {
            OnClick();
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="ButtonBase" /> control when 
        /// a new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            UpdateVisualStates();
        }
        
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs eventArgs)
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonDown(eventArgs);
#else
            base.OnPointerPressed(eventArgs);
#endif

            if (eventArgs.Handled || ClickMode == ClickMode.Hover)
            {
                return;
            }

            eventArgs.Handled = true;

            Focus();

#if MIGRATION
            CaptureMouse();
#else
            CapturePointer();
#endif
            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Stop();
            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Start();

            IsPressed = true;

            if (ClickMode == ClickMode.Press)
                OnClick();
        }

#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs eventArgs)
#else
        protected override void OnPointerReleased(PointerRoutedEventArgs eventArgs)
#endif
        {
            //todo: investigate why we enter twice in this method for each click.

#if MIGRATION
            base.OnMouseLeftButtonUp(eventArgs);
#else
            base.OnPointerReleased(eventArgs);
#endif

            if (eventArgs.Handled || ClickMode == ClickMode.Hover)
            {
                return;
            }

#if MIGRATION
            if (this.IsMouseCaptured) // Avoids calling the OnPointerReleased method twice for each click (cf. todo above)
#else
            if (this.IsPointerCaptured) // Avoids calling the OnPointerReleased method twice for each click (cf. todo above)
#endif
            {
                eventArgs.Handled = true;

                StopPointerCapture();

                if (ClickMode == ClickMode.Release)
                {
                    OnClick();
                }
            }

            this.IsPressed = false;
        }

        void StopPointerCapture()
        {
            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Stop();
#if MIGRATION
            this.ReleaseMouseCapture();
#else
            this.ReleasePointerCapture();
#endif
        }

        void TimerToReleaseCaptureAutomaticallyIfNoMouseUpEvent_Tick(object sender, object e)
        {
            StopPointerCapture();
        }


        #endregion

        /// <summary>
        /// Gets a value that indicates whether a ButtonBase is currently in a pressed state.
        /// The default is false.
        /// </summary>
        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            protected set => SetValue(IsPressedPropertyKey, value);
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

        private static void OnIsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ButtonBase)d).OnIsPressedChanged(e);
        }

        /// <summary>
        /// Called when the value of the <see cref="ButtonBase.IsPressed"/> property changes.
        /// </summary>
        /// <param name="e">The data for <see cref="DependencyPropertyChangedEventArgs"/></param>
        protected virtual void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateVisualStates();
        }

        /// <summary>
        /// Gets or sets when the Click event occurs. The default value is ClickMode.Release.
        /// </summary>
        public ClickMode ClickMode
        {
            get { return (ClickMode)GetValue(ClickModeProperty); }
            set { SetValue(ClickModeProperty, value); }
        }

        /// <summary>
        /// Identifies the ClickMode dependency property.
        /// </summary>
        public static readonly DependencyProperty ClickModeProperty =
            DependencyProperty.Register("ClickMode", typeof(ClickMode), typeof(ButtonBase), new PropertyMetadata(ClickMode.Release));

        #region handling the behavior towards keyboard events

#if MIGRATION
        protected override void OnKeyDown(KeyEventArgs e)
#else
        protected override void OnKeyDown(KeyRoutedEventArgs e)
#endif
        {
            base.OnKeyDown(e);

#if MIGRATION
            if ((e.Key == Key.Space) || (e.Key == Key.Enter))
#else
            if ((e.Key == Windows.System.VirtualKey.Space) || (e.Key == Windows.System.VirtualKey.Enter))
#endif
            {
                OnClick();

                e.Handled = true;
            }
        }

        #endregion

#if OLD_IMPLEMENTATION_OF_THE_CLICK_BASED_ON_HTML_CLICK_EVENT

        INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs> _clickEventManager;
        protected bool _forceClickEventToBeTheLastEventRaised = false; // Derived classes can set this flag to True in their constructor. Examples are the RadioButton and CheckBox (cf. ToggleButton class), for which we want the Click event to happen after the Change event.

        /// <summary>
        /// Occurs when a button control is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add
            {
                if (_clickEventManager == null)
                    _clickEventManager = new INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs>(() => this.INTERNAL_OuterDomElement, "click", ProcessOnClick);
                _clickEventManager.Add(value);
            }
            remove
            {
                if (_clickEventManager != null)
                    _clickEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the Click event
        /// </summary>
        void ProcessOnClick(object jsEventArg)
        {
            var eventArgs = new RoutedEventArgs()
            {
                OriginalSource = this,
            };
            if (_forceClickEventToBeTheLastEventRaised)
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        OnClick(eventArgs);
                    });
            }
            else
            {
                OnClick(eventArgs);
            }
        }

        /// <summary>
        /// Raises the Click event
        /// </summary>
        protected virtual void OnClick(RoutedEventArgs eventArgs)
        {
            //note: there is no "Handled" property in RoutedEventArgs
            foreach (RoutedEventHandler handler in _clickEventManager.Handlers.ToList<RoutedEventHandler>())
            {
                handler(this, eventArgs);
            }
        }

        internal override void INTERNAL_AttachToDomEvents()
        {
            base.INTERNAL_AttachToDomEvents();
            if (_clickEventManager != null)
                _clickEventManager.AttachToDomEvents();
        }

        internal override void INTERNAL_DetachFromDomEvents()
        {
            base.INTERNAL_DetachFromDomEvents();
            if (_clickEventManager != null)
                _clickEventManager.DetachFromDomEvents();
        }

#endif

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
        public bool IsMouseOver
        {
            get => (bool)GetValue(IsMouseOverProperty);
            internal set => SetValue(IsMouseOverPropertyKey, value);
        }

#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs eventArgs)
#else
        protected override void OnPointerEntered(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            base.OnMouseEnter(eventArgs);
#else
            base.OnPointerEntered(eventArgs);
#endif
            IsMouseOver = true;

            try
            {
                if (this.ClickMode != ClickMode.Hover || !this.IsEnabled) return;

                this.IsPressed = true;
                this.OnClick();
            }
            finally
            {
                this.UpdateVisualStates();
            }
        }

#if MIGRATION
        protected override void OnMouseLeave(MouseEventArgs eventArgs)
#else
        protected override void OnPointerExited(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            base.OnMouseLeave(eventArgs);
#else
            base.OnPointerExited(eventArgs);
#endif

            IsMouseOver = false;

            try
            {
                if (this.ClickMode != ClickMode.Hover || !this.IsEnabled) return;

                this.IsPressed = false;
            }
            finally
            {
                this.UpdateVisualStates();
            }
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
        public bool IsFocused
        {
            get => (bool)GetValue(IsFocusedProperty);
            private set => SetValue(IsFocusedPropertyKey, value);
        }

        /// <inheritdoc />
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.IsFocused = true;
            this.UpdateVisualStates();
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.IsFocused = false;

            try
            {
                if (this.ClickMode == ClickMode.Hover) return;

                this.IsPressed = false;
            }
            finally
            {
                this.UpdateVisualStates();
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
    }
}
