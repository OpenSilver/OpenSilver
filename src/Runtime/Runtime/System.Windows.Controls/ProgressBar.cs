
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

using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Automation.Peers;
using System.Windows.Shapes;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that indicates the progress of an operation.
    /// </summary>
    [TemplatePart(Name = ProgressBarIndicatorName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ProgressBarTrackName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = StateIndeterminate, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = StateDeterminate, GroupName = VisualStates.GroupCommon)]
    public class ProgressBar : RangeBase
    {
        // Silverlight part names
        private const string ProgressBarIndicatorName = "ProgressBarIndicator";
        private const string ProgressBarTrackName = "ProgressBarTrack";

        // WPF part names
        private const string WpfTrackName = "PART_Track";
        private const string WpfIndicatorName = "PART_Indicator";
        private const string WpfGlowRectName = "PART_GlowRect";

        private const string StateIndeterminate = "Indeterminate";
        private const string StateDeterminate = "Determinate";

        private FrameworkElement _track;
        private FrameworkElement _indicator;
        private FrameworkElement _glow;

        static ProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBar), new PropertyMetadata(typeof(ProgressBar)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        public ProgressBar() { }

        /// <summary>
        /// Identifies the <see cref="UseWpfBehavior"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseWpfBehaviorProperty =
            DependencyProperty.Register(nameof(UseWpfBehavior), typeof(bool), typeof(ProgressBar), new PropertyMetadata(false));

        /// <summary>
        /// Defines whether the ProgressBar should use WPF-style template parts (PART_Track, PART_Indicator,
        /// PART_GlowRect) and indeterminate animation, or the Silverlight-style parts and behavior.
        /// </summary>
        public bool UseWpfBehavior
        {
            get { return (bool)GetValue(UseWpfBehaviorProperty); }
            set { SetValue(UseWpfBehaviorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsIndeterminate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(
                nameof(IsIndeterminate),
                typeof(bool),
                typeof(ProgressBar),
                new PropertyMetadata(false, IsIndeterminatePropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether the progress bar reports generic
        /// progress with a repeating pattern or reports progress based on the <see cref="RangeBase.Value"/>
        /// property.
        /// </summary>
        /// <returns>
        /// true if the progress bar reports generic progress with a repeating pattern; false
        /// if the progress bar reports progress based on the <see cref="RangeBase.Value"/>
        /// property. The default is false.
        /// </returns>
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValueInternal(IsIndeterminateProperty, value); }
        }

        private static void IsIndeterminatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = (ProgressBar)d;
            if (pb.UseWpfBehavior)
            {
                pb.SetProgressBarIndicatorLength();
                pb.SetProgressBarGlowElementBrush();
            }
            pb.UpdateVisualStates();
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="ProgressBar"/> control when a new 
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_track != null)
            {
                _track.SizeChanged -= new SizeChangedEventHandler(OnTrackSizeChanged);
            }

            if (UseWpfBehavior)
            {
                _indicator = GetTemplateChild(WpfIndicatorName) as FrameworkElement;
                _track = GetTemplateChild(WpfTrackName) as FrameworkElement;
                _glow = GetTemplateChild(WpfGlowRectName) as FrameworkElement;
            }
            else
            {
                _indicator = GetTemplateChild(ProgressBarIndicatorName) as FrameworkElement;
                _track = GetTemplateChild(ProgressBarTrackName) as FrameworkElement;
                _glow = null;
            }

            if (_indicator != null && _track != null)
            {
                _track.SizeChanged += new SizeChangedEventHandler(OnTrackSizeChanged);
            }

            if (UseWpfBehavior && IsIndeterminate)
            {
                SetProgressBarGlowElementBrush();
            }

            UpdateVisualStates();
        }

        /// <summary>
        /// Returns a <see cref="ProgressBarAutomationPeer"/> for use by the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="ProgressBarAutomationPeer"/> for the <see cref="ProgressBar"/> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new ProgressBarAutomationPeer(this);

        /// <summary>
        /// Called when value of the <see cref="RangeBase.Maximum"/> property changes.
        /// </summary>
        /// <param name="oldMaximum">
        /// The previous value.
        /// </param>
        /// <param name="newMaximum">
        /// The new value.
        /// </param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            SetProgressBarIndicatorLength();
        }

        /// <summary>
        /// Called when value of the <see cref="RangeBase.Minimum"/> property changes.
        /// </summary>
        /// <param name="oldMinimum">
        /// The previous value.
        /// </param>
        /// <param name="newMinimum">
        /// The new value.
        /// </param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            SetProgressBarIndicatorLength();
        }

        /// <summary>
        /// Called when value of the <see cref="RangeBase.Value"/> property changes.
        /// </summary>
        /// <param name="oldValue">
        /// The previous value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            SetProgressBarIndicatorLength();
        }

        internal override void UpdateVisualStates(bool useTransitions)
        {
            VisualStateManager.GoToState(this, IsIndeterminate ? StateIndeterminate : StateDeterminate, useTransitions);
        }

        private void OnTrackSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetProgressBarIndicatorLength();
        }

        private void SetProgressBarIndicatorLength()
        {
            if (_track == null || _indicator == null)
                return;

            double min = Minimum;
            double max = Maximum;
            double val = Value;

            if (UseWpfBehavior)
            {
                double percent = IsIndeterminate || max <= min ? 1.0 : (val - min) / (max - min);
                _indicator.Width = percent * _track.ActualWidth;
                UpdateGlowAnimation();
            }
            else
            {
                if (VisualTreeHelper.GetParent(_indicator) is FrameworkElement parent)
                {
                    double widthOffset = _indicator.Margin.Left + _indicator.Margin.Right;

                    switch (parent)
                    {
                        case Border border:
                            widthOffset += border.Padding.Left + border.Padding.Right;
                            break;
                        case Control control:
                            widthOffset += control.Padding.Left + control.Padding.Right;
                            break;
                    }

                    double percent = IsIndeterminate || max == min ? 1.0 : (val - min) / (max - min);
                    double parentWidth = Math.Max(0, parent.ActualWidth - widthOffset);
                    _indicator.Width = percent * parentWidth;
                }
            }
        }

        /// <summary>
        /// Sets up or updates the glow animation for WPF indeterminate mode.
        /// The glow rectangle slides from left to right across the indicator, repeating forever.
        /// </summary>
        private void UpdateGlowAnimation()
        {
            if (!UseWpfBehavior || _glow == null)
                return;

            if (IsIndeterminate && _glow.Width > 0 && _indicator.Width > 0)
            {
                double endPos = _indicator.Width + _glow.Width;
                double startPos = -1 * _glow.Width;
                double speed = 200.0; // pixels per second

                TimeSpan translateTime = TimeSpan.FromSeconds((endPos - startPos) / speed);
                TimeSpan pauseTime = TimeSpan.FromSeconds(1.0);

                var animation = new ThicknessAnimation
                {
                    From = new Thickness(startPos, 0, 0, 0),
                    To = new Thickness(endPos, 0, 0, 0),
                    Duration = new Duration(translateTime + pauseTime),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                _glow.BeginAnimation(FrameworkElement.MarginProperty, animation);
            }
            else
            {
                _glow.BeginAnimation(FrameworkElement.MarginProperty, null);
            }
        }

        /// <summary>
        /// Sets up the glow brush for WPF indeterminate mode.
        /// Creates a gradient from transparent to the foreground color and back.
        /// </summary>
        private void SetProgressBarGlowElementBrush()
        {
            if (_glow == null)
                return;

            if (IsIndeterminate && Foreground is SolidColorBrush scb)
            {
                Color color = scb.Color;
                var brush = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)
                };
                brush.GradientStops.Add(new GradientStop { Color = Colors.Transparent, Offset = 0.0 });
                brush.GradientStops.Add(new GradientStop { Color = color, Offset = 0.4 });
                brush.GradientStops.Add(new GradientStop { Color = color, Offset = 0.6 });
                brush.GradientStops.Add(new GradientStop { Color = Colors.Transparent, Offset = 1.0 });

                if (_glow is Shape shape)
                {
                    shape.Fill = brush;
                }
            }
        }
    }
}
