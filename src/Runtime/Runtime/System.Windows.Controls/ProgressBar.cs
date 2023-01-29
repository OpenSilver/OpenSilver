
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

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Automation.Peers;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Automation.Peers;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that indicates the progress of an operation.
    /// </summary>
    [TemplatePart(Name = ProgressBarIndicatorName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ProgressBarTrackName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = StateIndeterminate, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = StateDeterminate, GroupName = VisualStates.GroupCommon)]
    public partial class ProgressBar : RangeBase
    {
        private const string ProgressBarIndicatorName = "ProgressBarIndicator";
        private const string ProgressBarTrackName = "ProgressBarTrack";
        private const string StateIndeterminate = "Indeterminate";
        private const string StateDeterminate = "Determinate";

        private FrameworkElement _track;
        private FrameworkElement _indicator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        public ProgressBar()
        {
            DefaultStyleKey = typeof(ProgressBar);
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
            set { SetValue(IsIndeterminateProperty, value); }
        }

        private static void IsIndeterminatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBar)d).UpdateVisualStates();
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="ProgressBar"/> control when a new 
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (_track != null)
            {
                _track.SizeChanged -= new SizeChangedEventHandler(OnTrackSizeChanged);
            }

            _indicator = GetTemplateChild(ProgressBarIndicatorName) as FrameworkElement;
            _track = GetTemplateChild(ProgressBarTrackName) as FrameworkElement;

            if (_indicator != null && _track != null)
            {
                _track.SizeChanged += new SizeChangedEventHandler(OnTrackSizeChanged);
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

        internal override void UpdateVisualStates()
        {
            VisualStateManager.GoToState(this, IsIndeterminate ? StateIndeterminate : StateDeterminate, true);
        }

        private void OnTrackSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetProgressBarIndicatorLength();
        }

        private void SetProgressBarIndicatorLength()
        {
            if (_track != null && _indicator != null &&
                VisualTreeHelper.GetParent(_indicator) is FrameworkElement parent)
            {
                double widthOffset = _indicator.Margin.Left + _indicator.Margin.Right;
                double min = Minimum;
                double max = Maximum;
                double val = Value;

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
}
