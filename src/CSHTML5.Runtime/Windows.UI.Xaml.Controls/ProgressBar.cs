﻿
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



#if MIGRATION
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
#if false
    public partial class ProgressBar : RangeBase
    {
        //public bool IsIndeterminateProperty;
        Canvas _rootCanvas;
        Rectangle _rectangleBehind;
        Rectangle _rectangleInFront;

        //public bool IsIndeterminate
        //{
        //    get { return (bool)GetValue(IsIndeterminateProperty); }
        //    set { SetValue(IsIndeterminateProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsIndeterminate.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsIndeterminateProperty =
        //    DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ProgressBar), new PropertyMetadata(false));


        public ProgressBar()
        {
            // Set default style:
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultProgressBarStyle.GetDefaultStyle());
            //this.IsIndeterminate = false;
            this.SizeChanged += ProgressBar_SizeChanged;
        }
        void ProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_rectangleBehind != null && _rootCanvas != null && _rectangleInFront != null)
            {
                _rectangleBehind.Width = _rootCanvas.ActualWidth;
                _rectangleBehind.Height = _rootCanvas.ActualHeight;
                _rectangleInFront.Height = _rootCanvas.ActualHeight;
                _rectangleBehind.ScheduleRedraw();
                UpdateRectangleInFront();
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
            _rootCanvas = this.GetTemplateChild("RootCanvas") as Canvas;
            _rectangleBehind = this.GetTemplateChild("RectangleBehind") as Rectangle;
            _rectangleInFront = this.GetTemplateChild("RectangleInFront") as Rectangle;

            if (_rectangleBehind != null && _rectangleInFront != null)
            {
                _rectangleBehind.Height = _rootCanvas.ActualHeight;
                _rectangleBehind.Width = _rootCanvas.ActualWidth;
                _rectangleInFront.Height = _rootCanvas.ActualHeight;
                UpdateRectangleInFront();
            }


        }

        void UpdateRectangleInFront()
        {
            if (_rootCanvas != null && _rectangleInFront != null && !double.IsNaN(_rootCanvas.ActualWidth))
            {
                if (this.Minimum >= this.Maximum)
                {
                    _rectangleInFront.Width = _rootCanvas.ActualWidth;
                }
                else
                {
                    _rectangleInFront.Width = (this.Value - this.Minimum) / (this.Maximum - this.Minimum) * _rootCanvas.ActualWidth;
                }
                _rectangleInFront.ScheduleRedraw();
            }
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            //The progress bar updates its appearance when the Maximum property changes.
            UpdateRectangleInFront();
        }
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            //The progress bar updates its appearance when the Minimum property changes.
            UpdateRectangleInFront();
        }
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            //The progress bar updates its appearance when the Value property changes.
            UpdateRectangleInFront();
        }
    }
#endif
}
