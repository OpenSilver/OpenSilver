

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

#if MIGRATION
using System.Windows;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Input;
using Windows.Foundation;
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
    public partial class ButtonBase : ContentControl
    {
        public ButtonBase()
        {
            _reactsToKeyboardEventsWhenFocused = true;
            UseSystemFocusVisuals = true;

            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Interval = new TimeSpan(0, 0, 5); // See comment where this variable is defined.
            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Tick += TimerToReleaseCaptureAutomaticallyIfNoMouseUpEvent_Tick;

#if MIGRATION
            base.MouseLeftButtonDown += (s, e) => { }; // cf. note below
            base.MouseLeftButtonUp += (s, e) => { }; // cf. note below
#else
            base.PointerPressed += (s, e) => { }; // Note: even though the logic for PointerPressed is located in the overridden method "OnPointerPressed" (below), we still need to register this event so that the underlying UIElement can listen to the HTML DOM "mousedown" event (cf. see the "Add" accessor of the "PointerPressed" event definition).
            base.PointerReleased += (s, e) => { }; // Note: even though the logic for PointerReleased is located in the overridden method "OnPointerPressed" (below), we still need to register this event so that the underlying UIElement can listen to the HTML DOM "mouseup" event (cf. see the "Add" accessor of the "PointerReleased" event definition).
#endif
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the <see cref="Command"/> property.
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ButtonBase), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        
        /// <summary>
        /// Gets or sets the command to invoke when this button is pressed. 
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ButtonBase), new PropertyMetadata(null, Command_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

#if WORKINPROGRESS
        //
        // Summary:
        //     Identifies the System.Windows.Controls.Primitives.ButtonBase.IsMouseOver dependency
        //     property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Primitives.ButtonBase.IsMouseOver
        //     dependency property.
        public static readonly DependencyProperty IsMouseOverProperty;
#endif

        static void Command_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonBase buttonBase = (ButtonBase)d;
            if (e.OldValue != null && e.NewValue == null)
            {
                buttonBase.Click -= ExecuteCommand;
            }
            if (e.OldValue == null && e.NewValue != null)
            {
                buttonBase.Click += ExecuteCommand;
            }
        }

        static void ExecuteCommand(object sender, RoutedEventArgs e)
        {
            ButtonBase buttonBase = (ButtonBase)sender;
            if (buttonBase.Command.CanExecute(buttonBase.CommandParameter))
            {
                buttonBase.Command.Execute(buttonBase.CommandParameter);
            }
        }

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
                Click(this, new RoutedEventArgs()
                {
                    OriginalSource = this //todo: the OriginalSource should not aways be the button itself: if the button contains inner elements, it is supposed to be the inner element on which the user has clicked.
                });
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
            eventArgs.Handled = true;

#if MIGRATION
            this.CaptureMouse();
#else
            this.CapturePointer();
#endif
            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Stop();
            _timerToReleaseCaptureAutomaticallyIfNoMouseUpEvent.Start();

            this.IsPressed = true;

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
            if (this.IsMouseCaptured) // Avoids calling the OnPointerReleased method twice for each click (cf. todo above)
#else
            if (this.IsPointerCaptured) // Avoids calling the OnPointerReleased method twice for each click (cf. todo above)
#endif
            {

#if MIGRATION
                base.OnMouseLeftButtonUp(eventArgs);
#else
                base.OnPointerReleased(eventArgs);
#endif
                eventArgs.Handled = true;

                StopPointerCapture();

                if (ClickMode == ClickMode.Release
                    //&& IsPointerOverThisControl(eventArgs) //todo: uncomment this line!!!
                    )
                    OnClick();
            }

            this.IsPressed = false;
        }

#if WORKINPROGRESS
#if MIGRATION
        //
        // Summary:
        //     Called when the value of the System.Windows.Controls.Primitives.ButtonBase.IsPressed
        //     property changes.
        //
        // Parameters:
        //   e:
        //     The data for System.Windows.DependencyPropertyChangedEventArgs.
        protected virtual void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            
        }
#endif
#endif

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

#if MIGRATION
        private bool IsPointerOverThisControl(MouseButtonEventArgs e)
#else
        private bool IsPointerOverThisControl(PointerRoutedEventArgs e)
#endif
        {
            Size actualSize = this.INTERNAL_GetActualWidthAndHeight();
            var actualWidth = actualSize.Width;
            var actualHeight = actualSize.Height;
            if (!double.IsNaN(actualWidth) && !double.IsNaN(actualHeight))
            {
#if MIGRATION
                var position = e.GetPosition(this);
#else
                var position = e.GetCurrentPoint(this).Position;
#endif
                return (position.X > 0 && position.Y > 0 && position.X < actualWidth && position.Y < actualHeight);
            }
            else
                return false;
        }

        #endregion


#if WORKINPROGRESS
        //
        // Summary:
        //     Gets a value that determines whether the button has focus.
        //
        // Returns:
        //     true if the control has focus; otherwise, false. The default is false.
        public bool IsFocused { get; }

        //
        // Summary:
        //     Gets a value indicating whether the mouse pointer is located over this button
        //     control.
        //
        // Returns:
        //     true to indicate the mouse pointer is over the button control, otherwise false.
        //     The default is false.
        public bool IsMouseOver { get; }
#endif

        /// <summary>
        /// Gets a value that indicates whether a ButtonBase is currently in a pressed state.
        /// The default is false.
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            protected set { SetValue(IsPressedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsPressed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(ButtonBase), new PropertyMetadata(false)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });



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
            DependencyProperty.Register("ClickMode", typeof(ClickMode), typeof(ButtonBase), new PropertyMetadata(ClickMode.Release)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        #region handling the behavior towards keyboard events


#if MIGRATION
        internal override void OnKeyDownWhenFocused(object sender, KeyEventArgs e)
#else
        internal override void OnKeyDownWhenFocused(object sender, KeyRoutedEventArgs e)
#endif
        {
            //We can completely remove RadioButton from this method because we can let the default browser behaviour which is completely different than Silverlight's and might be very tricky to fix.
            // For reference:
            //      Silverlight: focus on the radioButtons separately (pressing tab will go to the next element, no matter if it is a radioButton of the same group),
            //                  arrows do nothing,
            //                  space selects the current RadioButton.
            //      Browsers: focus on the RadioButtons as a whole (pressing tab will go to the next element that is not one of the radioButtons of the current group of radioButtons
            //                  arrows move the selection (and focus) on the different radioButtons of the group,
            //                  space does nothing.

            //space --> click
            //enter --> click unless checkbox and radioButton (I think those are the only exceptions, and for now we will "click" even if it is checkbox because it spares us a test and still makes sense).
#if MIGRATION
            if ((e.Key == Key.Space) || (e.Key == Key.Enter))
#else
            if ((e.Key == Windows.System.VirtualKey.Space) || (e.Key == Windows.System.VirtualKey.Enter))
#endif
            {
                OnClick();
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
    }
}
