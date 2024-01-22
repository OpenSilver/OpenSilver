
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
    /// Represents a control that raises its <see cref="ButtonBase.Click"/> event repeatedly 
    /// from the time it is pressed until it is released.
    /// </summary>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    public class RepeatButton : ButtonBase
    {
        private DispatcherTimer _timer;

        static RepeatButton()
        {
            ClickModeProperty.OverrideMetadata(typeof(RepeatButton), new PropertyMetadata(ClickMode.Press));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatButton"/> class.
        /// </summary>
        public RepeatButton()
        {
            DefaultStyleKey = typeof(RepeatButton);
        }

        /// <summary>
        /// Identifies the <see cref="Delay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register(
                nameof(Delay),
                typeof(int),
                typeof(RepeatButton),
                new PropertyMetadata(500),
                IsDelayValid);

        /// <summary>
        /// Gets or sets the time, in milliseconds, the <see cref="RepeatButton"/> waits when 
        /// it is pressed before it starts repeating the click action.
        /// </summary>
        /// <returns>
        /// The time, in milliseconds, the <see cref="RepeatButton"/>
        /// waits when it is pressed before it starts repeating the click action. The default
        /// is 500.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Delay"/> is set to a value less than 0.
        /// </exception>
        public int Delay
        {
            get => (int)GetValue(DelayProperty);
            set => SetValueInternal(DelayProperty, value);
        }

        private static bool IsDelayValid(object value) => (int)value >= 0;

        /// <summary>
        /// Identifies the <see cref="Interval"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                nameof(Interval),
                typeof(int),
                typeof(RepeatButton),
                new PropertyMetadata(33),
                IsIntervalValid);

        /// <summary>
        /// Gets or sets the time, in milliseconds, between repetitions of the click action,
        /// as soon as repeating starts.
        /// </summary>
        /// <returns>
        /// The time, in milliseconds, between repetitions of the click action, as soon as
        /// repeating starts. The default is 33.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Interval"/> is set to a value less than 0.
        /// </exception>
        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValueInternal(IntervalProperty, value);
        }

        private static bool IsIntervalValid(object value) => (int)value > 0;

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
            => new RepeatButtonAutomationPeer(this);

        /// <inheritdoc />
        protected override void OnClick()
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                if (FrameworkElementAutomationPeer.CreatePeerForElement(this) is AutomationPeer peer)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
                }
            }

            base.OnClick();
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsPressed && ClickMode != ClickMode.Hover)
            {
                StartTimer();
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (ClickMode != ClickMode.Hover)
            {
                StopTimer();
            }
        }

        /// <inheritdoc />
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            StopTimer();
        }

        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Tick += OnTimeout;
            }
            else if (_timer.IsEnabled)
            {
                return;
            }

            _timer.Interval = TimeSpan.FromMilliseconds(Delay);
            _timer.Start();
        }

        private void StopTimer() => _timer?.Stop();

        private void OnTimeout(object sender, EventArgs e)
        {
            TimeSpan interval = TimeSpan.FromMilliseconds(Interval);
            if (_timer.Interval != interval)
            {
                _timer.Interval = interval;
            }

            if (IsPressed)
            {
                OnClick();
            }
        }
    }
}
