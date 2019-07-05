
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

#if MIGRATION
using System.Windows.Input;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Input;
#endif
    
#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control that raises its Click event repeatedly from the time it is pressed until it is released.
    /// </summary>
    public class RepeatButton : ButtonBase
    {
        private DispatcherTimer _timer;

        static RepeatButton()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(RepeatButton), new FrameworkPropertyMetadata(typeof(RepeatButton)));
            ClickModeProperty.OverrideMetadata(typeof(RepeatButton), new PropertyMetadata(ClickMode.Press));
        }

        /// <summary>
        /// Initializes a new instance of the RepeatButton class.
        /// </summary>
        public RepeatButton()
            : base()
        {
            // Set default style:
            this.DefaultStyleKey = typeof(RepeatButton);

#if UNCOMMENT_WHEN_CLICKMODE_HOVER_WILL_BE_IMPLEMENTED

#if MIGRATION
            base.MouseEnter += (s, e) => { }; // cf. note below
            base.MouseLeave += (s, e) => { }; // cf. note below
#else
            base.PointerEntered += (s, e) => { }; // Note: even though the logic for PointerEntered is located in the overridden method "OnPointerEntered" (below), we still need to register this event so that the underlying UIElement can listen to the HTML DOM event (cf. see the "Add" accessor of the "PointerEntered" event definition).
            base.PointerExited += (s, e) => { }; // Note: even though the logic for PointerExited is located in the overridden method "OnPointerExited" (below), we still need to register this event so that the underlying UIElement can listen to the HTML DOM event (cf. see the "Add" accessor of the "PointerExited" event definition).
#endif

#endif
        }

#region Dependencies and Events

        /// <summary>
        /// Gets or sets the time, in milliseconds, the RepeatButton
        /// waits when it is pressed before it starts repeating the click action.
        /// The default is 250.
        /// </summary>
        public static readonly DependencyProperty DelayProperty
            = DependencyProperty.Register("Delay",
            typeof(int),
            typeof(RepeatButton),
            new PropertyMetadata(250));

        /// <summary>
        /// Gets or sets the time, in milliseconds, the RepeatButton
        /// waits when it is pressed before it starts repeating the click action.
        /// The default is 250.
        /// </summary>
        public int Delay
        {
            get
            {
                return (int)GetValue(DelayProperty);
            }
            set
            {
                SetValue(DelayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the time, in milliseconds, between repetitions of the click
        /// action, as soon as repeating starts. The default is 250.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register("Interval",
            typeof(int),
            typeof(RepeatButton),
            new PropertyMetadata(250));

        /// <summary>
        /// Gets or sets the time, in milliseconds, between repetitions of the click
        /// action, as soon as repeating starts. The default is 250.
        /// </summary>
        public int Interval
        {
            get
            {
                return (int)GetValue(IntervalProperty);
            }
            set
            {
                SetValue(IntervalProperty, value);
            }
        }

#endregion Dependencies and Events

#region Private helpers

        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Tick += OnTimeout;
            }
            else if (_timer.IsEnabled)
                return;

            _timer.Interval = TimeSpan.FromMilliseconds(Delay);
            _timer.Start();
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

#if MIGRATION
        private void OnTimeout(object sender, EventArgs e)
#else
        private void OnTimeout(object sender, object e)
#endif
        {
            TimeSpan interval = TimeSpan.FromMilliseconds(Interval);
            if (_timer.Interval != interval)
                _timer.Interval = interval;

            if (IsPressed)
            {
                OnClick();
            }
        }

#endregion Private helpers

#region Override methods

        /// <summary>
        /// Raises InvokedAutomationEvent and call the base method to raise the Click event
        /// </summary>
        /// <ExternalAPI/>
        protected override void OnClick()
        {
            base.OnClick();
        }

#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs eventArgs)
#else
        protected override void OnPointerPressed(Input.PointerRoutedEventArgs eventArgs)
#endif
        {
            //if (IsPressed && ClickMode != ClickMode.Hover)
            //{
            StartTimer();
            //}

            //--------------------------------------------------------------------------
            // Note: it is important that the code below ("base.OnPointerPressed")is executed AFTER the code above, otherwise
            // it will not work properly. In fact, you can reproduce the issue in case of inverted order by creating two
            // NumericUpDown controls, and using the following code-behind:
            // private void Numeric2_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
            // {
            //     if (Numeric1.Value + Numeric2.Value > 1)
            //     {
            //         Numeric2.Value = 0;
            //         MessageBox.Show("Exceeded");
            //     }
            // }
            // You will then notice that there is an infinite number of message boxes.
            //--------------------------------------------------------------------------

#if MIGRATION
            base.OnMouseLeftButtonDown(eventArgs);
#else
            base.OnPointerPressed(eventArgs);
#endif
        }

#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs eventArgs)
#else
        protected override void OnPointerReleased(Input.PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonUp(eventArgs);
#else
            base.OnPointerReleased(eventArgs);
#endif

            //if (ClickMode != ClickMode.Hover)
            //{
                StopTimer();
            //}
        }

        /// <summary>
        ///     Called when this element loses mouse capture.
        /// </summary>
        /// <param name="e"></param>
#if MIGRATION
        protected override void OnLostMouseCapture(MouseEventArgs e)
#else
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnLostMouseCapture(e);
#else
            base.OnPointerCaptureLost(e);
#endif
            StopTimer();
        }


#region Not supported

        /*
        /// <summary>
        ///     An event reporting the mouse entered this element.
        /// </summary>
        /// <param name="e">Event arguments</param>
#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
#endif
        {
            base.OnPointerEntered(e);
            if (HandleIsMouseOverChanged())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        ///     An event reporting the mouse left this element.
        /// </summary>
        /// <param name="e">Event arguments</param>
#if MIGRATION
        protected override void OnMouseLeave(MouseEventArgs e)
#else
        protected override void OnPointerExited(PointerRoutedEventArgs e)
#endif
        {
            base.OnPointerExited(e);
            if (HandleIsMouseOverChanged())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        ///     An event reporting that the IsMouseOver property changed.
        /// </summary>
        private bool HandleIsMouseOverChanged()
        {
            if (ClickMode == ClickMode.Hover)
            {
                if (IsMouseOver)
                {
                    StartTimer();
                }
                else
                {
                    StopTimer();
                }

                return true;
            }

            return false;
        }
         */

        ///// <summary>
        ///// This is the method that responds to the KeyDown event.
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);
        //    if ((e.Key == Key.Space) && (ClickMode != ClickMode.Hover))
        //    {
        //        StartTimer();
        //    }
        //}

        ///// <summary>
        ///// This is the method that responds to the KeyUp event.
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    if ((e.Key == Key.Space) && (ClickMode != ClickMode.Hover))
        //    {
        //        StopTimer();
        //    }
        //    base.OnKeyUp(e);
        //}

#endregion

#endregion
    }
}