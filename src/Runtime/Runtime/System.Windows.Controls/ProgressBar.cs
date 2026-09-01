
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
using OpenSilver.Compatibility;
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
        private const string ProgressBarIndicatorName = "ProgressBarIndicator", WPF_ProgressBarIndicatorName = "PART_Indicator";
        private const string ProgressBarTrackName = "ProgressBarTrack", WPF_ProgressBarTrackName = "PART_Track";
        private const string WPF_ProgressBarGlowRectName = "PART_GlowRect";

        private const string StateIndeterminate = "Indeterminate";
        private const string StateDeterminate = "Determinate";

        private FrameworkElement _track;
        private FrameworkElement _indicator;
        private FrameworkElement _glow;

        private bool _useWpfTemplate;

        static ProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBar), new PropertyMetadata(typeof(ProgressBar)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        public ProgressBar() { }

        /// <summary>
        /// Identifies the <see cref="TemplateKind"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateKindProperty =
            FrameworkOptions.TemplateKindProperty.AddOwner(typeof(ProgressBar), new PropertyMetadata(TemplateKind.Auto));

        /// <summary>
        /// Gets or sets a value that determines which control template conventions are used for this
        /// <see cref="ProgressBar"/>
        /// </summary>
        /// <returns>
        /// A <see cref="OpenSilver.Compatibility.TemplateKind"/> enumeration value that indicates how 
        /// template parts are resolved. The default is <see cref="TemplateKind.Auto"/>.
        /// </returns>
        public TemplateKind TemplateKind
        {
            get { return (TemplateKind)GetValue(TemplateKindProperty); }
            set { SetValue(TemplateKindProperty, value); }
        }

        private bool IsWpfTemplate()
        {
            return TemplateKind switch
            {
                TemplateKind.Wpf => true,
                TemplateKind.Silverlight => false,
                _ => GetTemplateChild(WPF_ProgressBarIndicatorName) is FrameworkElement,
            };
        }

        /// <summary>
        /// Identifies the <see cref="IsIndeterminate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(
                nameof(IsIndeterminate),
                typeof(bool),
                typeof(ProgressBar),
                new PropertyMetadata(BooleanBoxes.FalseBox, IsIndeterminatePropertyChanged));

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
            if (pb._useWpfTemplate)
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

            _useWpfTemplate = IsWpfTemplate();

            _indicator = GetIndicator(_useWpfTemplate);
            _track = GetTrack(_useWpfTemplate);
            _glow = GetGlow(_useWpfTemplate);

            if (_indicator != null && _track != null)
            {
                _track.SizeChanged += new SizeChangedEventHandler(OnTrackSizeChanged);
            }

            if (_useWpfTemplate && IsIndeterminate)
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

        private FrameworkElement GetIndicator(bool isWpfTemplate)
        {
            return GetTemplateChild(isWpfTemplate ? WPF_ProgressBarIndicatorName : ProgressBarIndicatorName) as FrameworkElement;
        }

        private FrameworkElement GetTrack(bool isWpfTemplate)
        {
            return GetTemplateChild(isWpfTemplate ? WPF_ProgressBarTrackName : ProgressBarTrackName) as FrameworkElement;
        }

        private FrameworkElement GetGlow(bool isWpfTemplate)
        {
            if (isWpfTemplate)
            {
                return GetTemplateChild(WPF_ProgressBarGlowRectName) as FrameworkElement;
            }

            return null;
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

            if (_useWpfTemplate)
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
            if (!_useWpfTemplate || _glow == null)
                return;

            if (IsIndeterminate && _glow.Width > 0 && _indicator.Width > 0)
            {
                double endPos = _indicator.Width + _glow.Width;
                double startPos = -1 * _glow.Width;

                TimeSpan translateTime = TimeSpan.FromSeconds((endPos - startPos) / 200.0); // travel at 200px per second
                TimeSpan pauseTime = TimeSpan.FromSeconds(1.0);

                var animation = new ThicknessAnimation
                {
                    From = new Thickness(startPos, 0, 0, 0),
                    To = new Thickness(endPos, 0, 0, 0),
                    Duration = new Duration(translateTime + pauseTime),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                _glow.BeginAnimation(MarginProperty, animation);
            }
            else
            {
                _glow.BeginAnimation(MarginProperty, null);
            }
        }

        /// <summary>
        /// Sets up the glow brush for WPF indeterminate mode.
        /// Creates a gradient from transparent to the foreground color and back.
        /// </summary>
        private void SetProgressBarGlowElementBrush()
        {
            if (_glow is not Shape shape)
                return;

            if (IsIndeterminate)
            {
                if (Foreground is SolidColorBrush scb)
                {
                    Color color = scb.Color;
                    var brush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 0)
                    };

                    brush.GradientStops.Add(new GradientStop(Colors.Transparent, 0.0));
                    brush.GradientStops.Add(new GradientStop(color, 0.4));
                    brush.GradientStops.Add(new GradientStop(color, 0.6));
                    brush.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));

                    shape.SetCurrentValue(OpacityMaskProperty, null);
                    shape.SetCurrentValue(Shape.FillProperty, brush);
                }
                else
                {
                    // This is not a solid color brush so we will need an opacity mask.
                    var mask = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 0)
                    };

                    mask.GradientStops.Add(new GradientStop(Colors.Transparent, 0.0));
                    mask.GradientStops.Add(new GradientStop(Colors.Black, 0.4));
                    mask.GradientStops.Add(new GradientStop(Colors.Black, 0.6));
                    mask.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));

                    shape.SetCurrentValue(OpacityMaskProperty, mask);
                    shape.SetCurrentValue(Shape.FillProperty, Foreground);
                }
            }
        }
    }
}
