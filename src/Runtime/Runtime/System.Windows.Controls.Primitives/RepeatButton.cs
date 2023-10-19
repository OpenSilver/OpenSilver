
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

using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Threading;

namespace System.Windows.Controls.Primitives
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
        {
            DefaultStyleKey = typeof(RepeatButton);
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

        protected override AutomationPeer OnCreateAutomationPeer()
            => new RepeatButtonAutomationPeer(this);

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

        private void OnTimeout(object sender, EventArgs e)
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs eventArgs)
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

            base.OnMouseLeftButtonDown(eventArgs);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs eventArgs)
        {
            base.OnMouseLeftButtonUp(eventArgs);

            //if (ClickMode != ClickMode.Hover)
            //{
                StopTimer();
            //}
        }

        /// <summary>
        ///     Called when this element loses mouse capture.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            StopTimer();
        }

#endregion
    }
}