

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

#if MIGRATION
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Automation.Peers;
#else
using System;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Automation.Peers;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class ProgressBar : RangeBase
    {
        private Border _trackBorder;
        private Rectangle _rectProgressIndicator;

        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(
                "IsIndeterminate", 
                typeof(bool), 
                typeof(ProgressBar), 
                new PropertyMetadata(false, IsIndeterminatePropertyChanged));

        private static void IsIndeterminatePropertyChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ProgressBar)d;
            target.OnIsIndeterminateChanged();
        }

        public ProgressBar()
        {
            this.DefaultStyleKey = typeof(ProgressBar);
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_trackBorder != null && _rectProgressIndicator != null)
            {
                UpdateProgressRectangle();
            }
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            /*
             * Builds the visual tree for the ProgressBar control when a new template is applied. 
             * (Overrides FrameworkElement.OnApplyTemplate().)
             * */
            base.OnApplyTemplate();
            //----------------------------
            // Get a reference to the UI elements defined in the control template:
            //----------------------------

            if (_trackBorder != null)
            {
                _trackBorder.SizeChanged -= new SizeChangedEventHandler(OnTrackSizeChanged);
            }

            _trackBorder = GetTemplateChild("ProgressBarTrack") as Border;

            if (_trackBorder != null)
            {
                _trackBorder.SizeChanged += new SizeChangedEventHandler(OnTrackSizeChanged);
            }

            _rectProgressIndicator = GetTemplateChild("ProgressBarIndicator") as Rectangle;

            if (_trackBorder != null && _rectProgressIndicator != null)
            {
                if (IsIndeterminate)
                {
                    OnIsIndeterminateChanged();
                }
                else
                {
                    UpdateProgressRectangle();
                }
            }
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

        private void OnTrackSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateProgressRectangle();
        }

        private void UpdateProgressRectangle()
        {
            if (_trackBorder == null || _rectProgressIndicator == null) return;

            var parent = VisualTreeHelper.GetParent(_rectProgressIndicator) as FrameworkElement;
            if (parent == null) return;

            double widthOffset = _rectProgressIndicator.Margin.Left + _rectProgressIndicator.Margin.Right + _rectProgressIndicator.StrokeThickness;
            double minimum = Minimum;
            double maximum = Maximum;

            switch (parent)
            {
                case Border border:
                    widthOffset += border.Padding.Left + border.Padding.Right;
                    break;
                case Control control:
                    widthOffset += control.Padding.Left + control.Padding.Right;
                    break;
            }

            _rectProgressIndicator.Width = (IsIndeterminate || maximum == minimum ? 1.0 : (Value - minimum) / (maximum - minimum)) * Math.Max(0.0, parent.ActualWidth - widthOffset);
            _rectProgressIndicator.ScheduleRedraw();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            UpdateProgressRectangle();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            UpdateProgressRectangle();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            UpdateProgressRectangle();
        }

        private void OnIsIndeterminateChanged()
        {
            UpdateVisualState(true);
        }

        private void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, IsIndeterminate ? "Indeterminate" : "Determinate", useTransitions);
        }
    }
}
