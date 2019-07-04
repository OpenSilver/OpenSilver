
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
using CSHTML5.Internal;
using CSHTML5.Native.Html.Controls;
using DotNetForHtml5.Core;

#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that creates a pop-up window that displays information for an element in the UI.
    /// </summary>
    public class ToolTip : ContentControl
    {
        Popup _parentPopup;
        internal Point? _forceSpecifyAbsoluteCoordinates;
        internal UIElement INTERNAL_ElementToWhichThisToolTipIsAssigned;
        internal HtmlCanvasElement INTERNAL_HtmlCanvasElementToWhichThisToolTipIsAssigned;

        DispatcherTimer _timerForClosingTooltipAfter5Seconds = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 5) };

        /// <summary>
        /// Initializes a new instance of the ToolTip class.
        /// </summary>
        public ToolTip()
        {
            // Set default style:
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(ToolTip);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultToolTipStyle.GetDefaultStyle());
#endif
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the ToolTip is visible. True if the ToolTip is visible; otherwise, false. The default is false.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the IsOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ToolTip), new PropertyMetadata(false, IsOpen_Changed));

        private static void IsOpen_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolTip toolTip = (ToolTip)d;
            if (e.NewValue is bool)
            {
                bool isOpen = (bool)e.NewValue;
                if (isOpen)
                {
                    bool wasPopupAlreadyOpen = (toolTip._parentPopup != null && toolTip._parentPopup.IsOpen == true);
                    if (!wasPopupAlreadyOpen) // Note: this prevents loops due to the fact that when the popup opens, the "ToolTip.IsOpen_Changed" method is called because the tooltip is the child of the Popup so its properties are called when it is loaded into the Visual Tree.
                    {
                        // Propagate the DataContext:
                        if (toolTip.INTERNAL_ElementToWhichThisToolTipIsAssigned is FrameworkElement)
                            toolTip.DataContext = ((FrameworkElement)toolTip.INTERNAL_ElementToWhichThisToolTipIsAssigned).DataContext;

                        // Make sure the tooltip is transparent to clicks:
                        toolTip.IsHitTestVisible = false;

                        // Make sure the tooltip is Top/Left-aligned:
                        toolTip.HorizontalAlignment = HorizontalAlignment.Left;
                        toolTip.VerticalAlignment = VerticalAlignment.Top;

                        // Create the popup if not already created:
                        if (toolTip._parentPopup == null)
                        {
                            toolTip._parentPopup = new Popup()
                            {
                                Child = toolTip,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalContentAlignment = HorizontalAlignment.Left,
                                VerticalContentAlignment = VerticalAlignment.Top,
                            };

                            // Make sure that the popup is displayed in the same Window as the element to which the ToolTip is assigned. This is useful when there are multiple Windows:
                            if (toolTip.INTERNAL_ElementToWhichThisToolTipIsAssigned != null)
                                toolTip._parentPopup.INTERNAL_ParentWindow = toolTip.INTERNAL_ElementToWhichThisToolTipIsAssigned.INTERNAL_ParentWindow;
                        }

                        // Calculate the popup position:
                        Point popupAbsolutePosition;
                        if (toolTip._forceSpecifyAbsoluteCoordinates.HasValue)
                        {
                            popupAbsolutePosition = toolTip._forceSpecifyAbsoluteCoordinates.Value;
                        }
                        else
                        {
                            popupAbsolutePosition = INTERNAL_PopupsManager.CalculatePopupAbsolutePositionBasedOnElementPosition(
                                    toolTip.INTERNAL_ElementToWhichThisToolTipIsAssigned,
                                    toolTip.HorizontalOffset,
                                    toolTip.VerticalOffset);
                        }

                        // Set the popup position:
                        toolTip._parentPopup.HorizontalOffset = popupAbsolutePosition.X;
                        toolTip._parentPopup.VerticalOffset = popupAbsolutePosition.Y;

                        // Ensure that the popup stays within the screen bounds if its content is big:
                        toolTip._parentPopup.Loaded -= toolTip._parentPopup_Loaded; // We unregister the event to ensure that it is not registered twice.
                        toolTip._parentPopup.Loaded += toolTip._parentPopup_Loaded;

                        // Open the popup:
                        toolTip._parentPopup.IsOpen = true;

                        //Start the timer to close the popup after 5 seconds:
                        toolTip._timerForClosingTooltipAfter5Seconds.Tick -= toolTip._timerForClosingTooltipAfter5Seconds_Tick;
                        toolTip._timerForClosingTooltipAfter5Seconds.Tick += toolTip._timerForClosingTooltipAfter5Seconds_Tick;
                        toolTip._timerForClosingTooltipAfter5Seconds.Start();

                        // Raise the "Opened" event:
                        if (toolTip.Opened != null)
                            toolTip.Opened(toolTip, new RoutedEventArgs());
                    }
                }
                else
                {
                    toolTip._timerForClosingTooltipAfter5Seconds.Stop();

                    if (toolTip._parentPopup != null
                        && toolTip._parentPopup.IsOpen == true)
                    {
                        toolTip._parentPopup.IsOpen = false;

                        // Raise the "Closed" event:
                        if (toolTip.Closed != null)
                            toolTip.Closed(toolTip, new RoutedEventArgs());
                    }
                }
            }
        }

        void _parentPopup_Loaded(object sender, RoutedEventArgs e)
        {
            INTERNAL_PopupsManager.EnsurePopupStaysWithinScreenBounds(_parentPopup);
        }

        void _timerForClosingTooltipAfter5Seconds_Tick(object sender, object e)
        {
            _timerForClosingTooltipAfter5Seconds.Stop();
            if (this.IsOpen)
            {
                this.IsOpen = false;
            }
        }

        public void INTERNAL_OpenAtCoordinates(Point absoluteCoordinates)
        {
            _forceSpecifyAbsoluteCoordinates = absoluteCoordinates;

            this.IsOpen = true;
        }

        /*
        /// <summary>
        /// Gets or sets how a ToolTip should be positioned in relation to the placement target element.
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// Identifies the Placement dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(ToolTip), new PropertyMetadata(PlacementMode.Bottom));
        */

        /// <summary>
        /// Occurs when a ToolTip is closed and is no longer visible.
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        /// Occurs when a ToolTip becomes visible.
        /// </summary>
        public event RoutedEventHandler Opened;

#if WORKINPROGRESS
#region Not supported yet
        /// <summary>Gets or sets the visual element or control that the tool tip should be positioned in relation to when opened by the <see cref="T:System.Windows.Controls.ToolTipService" />.</summary>
        /// <returns>The visual element or control that the tool tip should be positioned in relation to when opened by the <see cref="T:System.Windows.Controls.ToolTipService" />. The default is null.</returns>
        public UIElement PlacementTarget
        {
            get
            {
                return (UIElement)this.GetValue(ToolTip.PlacementTargetProperty);
            }
            set
            {
                this.SetValue(ToolTip.PlacementTargetProperty, (DependencyObject)value);
            }
        }

        /// <summary>
        /// Identifies the PlacementTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register("PlacementTarget", typeof(UIElement), typeof(ToolTip), null);

        /// <summary>Gets or sets how the <see cref="T:System.Windows.Controls.ToolTip" /> should be positioned in relation to the <see cref="P:System.Windows.Controls.ToolTip.PlacementTarget" />.</summary>
        /// <returns>One of the <see cref="T:System.Windows.Controls.Primitives.PlacementMode" /> values. The default is <see cref="F:System.Windows.Controls.Primitives.PlacementMode.Mouse" />. </returns>
        public PlacementMode Placement
        {
            get
            {
                return (PlacementMode)this.GetValue(ToolTip.PlacementProperty);
            }
            set
            {
                this.SetValue(ToolTip.PlacementProperty, (Enum)value);
            }
        }

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.ToolTip.Placement" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.ToolTip.Placement" />dependency property.</returns>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(ToolTip), null);
#endregion
#endif

        //-----------------------
        // HORIZONTALOFFSET
        //-----------------------

        /// <summary>
        /// Gets or sets the horizontal distance between the target origin and the pop-up alignment point. The default is 0.
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }
        /// <summary>
        /// Identifies the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ToolTip), new PropertyMetadata(0d));


        //-----------------------
        // VERTICALOFFSET
        //-----------------------

        /// <summary>
        /// Gets or sets the vertical distance between the target origin and the pop-up alignment point. The default is 0.
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }
        /// <summary>
        /// Identifies the VerticalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ToolTip), new PropertyMetadata(0d));

    }
}
