

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
using CSHTML5.Internal;
using CSHTML5.Native.Html.Controls;
using DotNetForHtml5.Core;
using System.Diagnostics;
using System.Windows.Media;
#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;
#else
using Windows.Foundation;
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
    [TemplateVisualState(Name = "Closed", GroupName = "OpenStates")]
    [TemplateVisualState(Name = "Open", GroupName = "OpenStates")]
    public partial class ToolTip : ContentControl
    {
        #region Constants 

        private const double TOOLTIP_tolerance = 2.0;

        #endregion Constants 

        #region HorizontalOffset Property 

        /// <summary>
        /// Determines a horizontal offset in pixels from the left side of 
        /// the mouse bounding rectangle to the left side of the ToolTip.
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary> 
        /// Identifies the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset",
                typeof(double),
                typeof(ToolTip),
                new PropertyMetadata(new PropertyChangedCallback(OnHorizontalOffsetPropertyChanged)));

        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // HorizontalOffset dependency property should be defined on a ToolTip 
            ToolTip toolTip = (ToolTip)d;

            double newOffset = (double)e.NewValue;
            // Working around temporary limitations in Silverlight:
            // perform inequality test
            // 

            if (newOffset != (double)e.OldValue)
            {
                toolTip.OnOffsetChanged(newOffset, 0);
            }
        }

        #endregion HorizontalOffset Property

        #region PlacementTarget Property
        /// <summary>
        /// Determines a horizontal offset in pixels from the left side of 
        /// the mouse bounding rectangle to the left side of the ToolTip.
        /// </summary>
        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        /// <summary> 
        /// Identifies the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register(
                "PlacementTarget",
                typeof(UIElement),
                typeof(ToolTip),
                new PropertyMetadata(new PropertyChangedCallback(OnPlacementTargetPropertyChanged)));

        private static void OnPlacementTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion PlacementTarget Property

        #region Placement Property
        /// <summary>
        /// Determines a horizontal offset in pixels from the left side of 
        /// the mouse bounding rectangle to the left side of the ToolTip.
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary> 
        /// Identifies the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(
                "Placement",
                typeof(PlacementMode),
                typeof(ToolTip),
                new PropertyMetadata(PlacementMode.Mouse, new PropertyChangedCallback(OnPlacementPropertyChanged)));

        private static void OnPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion PlacementTarget Property

        #region IsOpen Property

        /// <summary> 
        /// Gets a value that determines whether tooltip is displayed or not. 
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary> 
        /// Identifies the IsOpen dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                "IsOpen",
                typeof(bool),
                typeof(ToolTip),
                new PropertyMetadata(new PropertyChangedCallback(OnIsOpenPropertyChanged)));

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolTip toolTip = (ToolTip)d;
            toolTip.OnIsOpenChanged((bool)e.NewValue);
        }

        #endregion IsOpen Property 

        #region VerticalOffset Property

        /// <summary>
        /// Determines a vertical offset in pixels from the bottom of the
        /// mouse bounding rectangle to the top of the ToolTip. 
        /// </summary> 
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalOffset dependency property. 
        /// </summary> 
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                "VerticalOffset",
                typeof(double),
                typeof(ToolTip),
                new PropertyMetadata(new PropertyChangedCallback(OnVerticalOffsetPropertyChanged)));

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // VerticalOffset dependency property should be defined on a ToolTip
            ToolTip toolTip = (ToolTip)d;

            double newOffset = (double)e.NewValue;
            if (newOffset != (double)e.OldValue)
            {
                toolTip.OnOffsetChanged(0, newOffset);
            }
        }

        #endregion VerticalOffset Property 

        #region Events

        /// <summary>
        /// Occurs when a ToolTip is closed and is no longer visible. 
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        /// Occurs when a ToolTip becomes visible.
        /// </summary> 
        public event RoutedEventHandler Opened;

        #endregion Events 

        #region Data

        FrameworkElement tooltipParent;
        Popup _parentPopup;
        internal Popup ParentPopup
        {
            get { return _parentPopup; }
        }

        internal PlacementMode? PlacementOverride
        {
            get; set;
        }

        internal UIElement PlacementTargetOverride
        {
            get; set;
        }

        // If ToolTipService attatches a tooltip to an object we need
        // to proxy the DataContext from that object. 'TooltipParent' is
        // where we store the object the tooltip is attached to.
        internal FrameworkElement TooltipParent
        {
            get { return tooltipParent; }
            set
            {
                tooltipParent = value;
                if (_parentPopup != null)
                {
                    _parentPopup.DataContext = ((tooltipParent == null) ? null : tooltipParent.DataContext);
                }
            }
        }

        #endregion Data

        /// <summary> 
        /// Creates a default ToolTip element
        /// </summary>
        public ToolTip()
        {
            DefaultStyleKey = typeof(ToolTip);
        }

        #region Protected Methods

        /// <summary>
        /// Apply a template to the ToolTip, invoked from ApplyTemplate
        /// </summary> 
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            UpdateVisualState(false);
        }

        #endregion Protected Methods 

        #region Private Methods

        private void HookupParentPopup()
        {
            Debug.Assert(this._parentPopup == null, "this._parentPopup should be null, we want to set visual tree once");

            this._parentPopup = new Popup();
            this._parentPopup.DataContext = TooltipParent == null ? null : TooltipParent.DataContext;
            this._parentPopup.Opened += delegate
            {
                var h = Opened;
                if (h != null)
                    h(this, new RoutedEventArgs { OriginalSource = this });
                this.LayoutUpdated += OnLayoutUpdated;
            };
            this._parentPopup.Closed += delegate
            {
                var h = Closed;
                if (h != null)
                    h(this, new RoutedEventArgs { OriginalSource = this });
                this.LayoutUpdated -= OnLayoutUpdated;
            };
            this.IsTabStop = false;

            this._parentPopup.Child = this;

            // Working around temporary limitations in Silverlight:
            // set IsHitTestVisible on both the popup and the child 
            // 
            this._parentPopup.IsHitTestVisible = false;
            this.IsHitTestVisible = false;

            //
        }

        private void OnIsOpenChanged(bool isOpen)
        {
            if (isOpen)
            {
                if (_parentPopup == null)
                    HookupParentPopup();

                this._parentPopup.IsOpen = true;
                PerformPlacement(HorizontalOffset, VerticalOffset);
            }
            else
            {
                this._parentPopup.IsOpen = false;
            }
            UpdateVisualState();
        }

        private void OnOffsetChanged(double horizontalOffset, double verticalOffset)
        {
            if (this._parentPopup == null)
            {
                return;
            }

            if (IsOpen)
            {
                // update the current ToolTip position if needed 
                PerformPlacement(horizontalOffset, verticalOffset);
            }
        }

        private void OnLayoutUpdated(object sender, EventArgs args)
        {
            if (this._parentPopup != null)
            {
                PerformPlacement(this.HorizontalOffset, this.VerticalOffset);
            }
        }

        private void PerformClipping(Size size)
        {
            Point mouse = ToolTipService.MousePosition;
            RectangleGeometry rectangle = new RectangleGeometry();
            rectangle.Rect = new Rect(mouse.X, mouse.Y, size.Width, size.Height);
            if (Content is UIElement)
                ((UIElement)Content).Clip = rectangle;
        }

        private void PerformPlacement(double horizontalOffset, double verticalOffset)
        {
            if (!IsOpen)
                return;

            var bounds = new Point(0, 0);
            var point = new Point(0, 0);
            var target_bounds = new Rect(0, 0, 0, 0);

            var mode = PlacementOverride.HasValue ? PlacementOverride.Value : Placement;
            var target = (FrameworkElement)(PlacementTargetOverride ?? PlacementTarget);
            var root = Application.Current.Host.Content;
            if (root == null)
                return;

            bounds = new Point(root.ActualWidth, root.ActualHeight);

            if (mode == PlacementMode.Mouse)
            {
                point = ToolTipService.MousePosition;
            }
            else
            {
                try
                {
                    if (target != null)
                    {
                        target_bounds = new Rect(0, 0, target.ActualWidth, target.ActualHeight);
                        target_bounds = target.TransformToVisual(null).TransformBounds(target_bounds);
                        point = new Point(target_bounds.Left, target_bounds.Top);
                    }
                }
                catch
                {
                    Console.WriteLine("MOONLIGHT WARNING: Could not transform the tooltip point");
                    //_parentPopup.HorizontalOffset = horizontalOffset;
                    //_parentPopup.VerticalOffset = verticalOffset;
                    return;
                }
            }

            /* FIXME this should probably be a binding */
            if (target != null)
            {
                FlowDirection = target.FlowDirection;
            }

            /* 
             * FIXME this is especially ugly based on the way we're
             * handling FlowDirection across an unparented popup
             */
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                point.X = point.X + target_bounds.Width - ActualWidth;
                if (mode == PlacementMode.Left)
                {
                    mode = PlacementMode.Right;
                }
                else if (mode == PlacementMode.Right)
                {
                    mode = PlacementMode.Left;
                }
            }

            switch (mode)
            {
                case PlacementMode.Top:
                    point.Y = target_bounds.Top - ActualHeight;
                    break;
                case PlacementMode.Bottom:
                    point.Y = target_bounds.Bottom;
                    break;
                case PlacementMode.Left:
                    point.X = target_bounds.Left - ActualWidth;
                    break;
                case PlacementMode.Right:
                    point.X = target_bounds.Right;
                    break;
                case PlacementMode.Mouse:
                    point.Y += new TextBox().FontSize; // FIXME: Just a guess, it's about right.
                    break;
                default:
                    throw new NotSupportedException(string.Format("PlacementMode '{0}' is not supported", Placement));
            }

            /* FIXME offsets need work still */
            /*
                point.Y += verticalOffset;
                point.X += horizontalOffset;
            */


            if ((point.Y + ActualHeight) > bounds.Y)
            {
                if (mode == PlacementMode.Bottom)
                    point.Y = target_bounds.Top - ActualHeight;
                else
                    point.Y = bounds.Y - ActualHeight;
            }
            else if (point.Y < 0)
            {
                if (mode == PlacementMode.Top)
                    point.Y = target_bounds.Bottom;
                else
                    point.Y = 0;
            }

            if ((point.X + ActualWidth) > bounds.X)
            {
                if (mode == PlacementMode.Right)
                    point.X = target_bounds.Left - ActualWidth;
                else
                    point.X = bounds.X - ActualWidth;
            }
            else if (point.X < 0)
            {
                if (mode == PlacementMode.Left)
                    point.X = target_bounds.Right;
                else
                    point.X = 0;
            }

            this._parentPopup.VerticalOffset = point.Y;
            this._parentPopup.HorizontalOffset = point.X;

            // if right/bottom doesn't fit into the plug-in bounds, clip the ToolTip
            /*
                double dX = (point.X + ActualWidth) - bounds.X;
                double dY = (point.Y + ActualHeight) - bounds.Y;
                if ((dX >= TOOLTIP_tolerance) || (dY >= TOOLTIP_tolerance))
                {
                    PerformClipping(new Size(Math.Max (0, ActualWidth - dX), Math.Max (0, ActualHeight - dY)));
                } else {
                    PerformClipping (new Size (ActualWidth, ActualHeight));
                }
            */
        }

        #endregion Private Methods

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return base.OnCreateAutomationPeer();
        }

        void UpdateVisualState()
        {
            UpdateVisualState(true);
        }

        void UpdateVisualState(bool useTransitions)
        {
            if (IsOpen)
            {
                VisualStateManager.GoToState(this, "Open", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Closed", useTransitions);
            }
        }
    }
}
