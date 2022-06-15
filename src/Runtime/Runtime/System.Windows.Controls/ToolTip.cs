

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

#if MIGRATION
using System.Windows.Controls.Primitives;
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
    [TemplateVisualState(Name = VisualStates.StateToolTipOpen, GroupName = VisualStates.GroupToolTip)]
    [TemplateVisualState(Name = VisualStates.StateToolTipClosed, GroupName = VisualStates.GroupToolTip)]
    public partial class ToolTip : ContentControl
    {
        private Popup _parentPopup;
        private FrameworkElement _owner;

        /// <summary>
        /// Initializes a new instance of the ToolTip class.
        /// </summary>
        public ToolTip()
        {
            // Set default style:
            this.DefaultStyleKey = typeof(ToolTip);
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
            bool isOpen = (bool)e.NewValue;
            if (isOpen)
            {
                bool wasPopupAlreadyOpen = (toolTip._parentPopup != null && toolTip._parentPopup.IsOpen == true);
                // Note: this prevents loops due to the fact that when the popup opens, the "ToolTip.IsOpen_Changed" method is called
                // because the tooltip is the child of the Popup so its properties are called when it is loaded into the Visual Tree.
                if (wasPopupAlreadyOpen) 
                {
                    return;
                }

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

                    toolTip._parentPopup.DataContext = toolTip._owner?.DataContext;

                    toolTip._parentPopup.Loaded += new RoutedEventHandler(ParentPopupLoaded);
                }

                Point position = GetMousePosition();
                position.X = Math.Max(position.X + toolTip.HorizontalOffset, 0.0) + 10.0;
                position.Y = Math.Max(position.Y + toolTip.VerticalOffset, 0.0) + 10.0;

                toolTip._parentPopup.HorizontalOffset = position.X;
                toolTip._parentPopup.VerticalOffset = position.Y;

                toolTip._parentPopup.IsOpen = true;

                VisualStateManager.GoToState(toolTip, VisualStates.StateToolTipOpen, true);

                if (toolTip.Opened != null)
                {
                    toolTip.Opened(toolTip, new RoutedEventArgs
                    {
                        OriginalSource = toolTip
                    });
                }
            }
            else
            {
                if (toolTip._parentPopup != null && toolTip._parentPopup.IsOpen == true)
                {
                    VisualStateManager.GoToState(toolTip, VisualStates.StateToolTipClosed, true);
                    toolTip._parentPopup.IsOpen = false;

                    // Raise the "Closed" event:
                    if (toolTip.Closed != null)
                        toolTip.Closed(toolTip, new RoutedEventArgs
                        {
                            OriginalSource = toolTip
                        });
                }
            }
        }

        internal void SetOwner(UIElement owner)
        {
            if (_owner != null)
            {
                _owner.DataContextChanged -= new DependencyPropertyChangedEventHandler(OnOwnerDataContextChanged);
            }

            _owner = owner as FrameworkElement;

            if (_owner != null)
            {
                _owner.DataContextChanged += new DependencyPropertyChangedEventHandler(OnOwnerDataContextChanged);
            }

            if (_parentPopup != null)
            {
                _parentPopup.DataContext = _owner?.DataContext;
            }
        }

        private void OnOwnerDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_parentPopup != null)
            {
                _parentPopup.DataContext = e.NewValue;
            }
        }

        private static Point GetMousePosition()
        {
            return Point.Parse(
                Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("_opensilver.mousePositionX.toString() + \",\" + _opensilver.mousePositionY.toString()"))
            );
        }

        private static void ParentPopupLoaded(object sender, RoutedEventArgs e)
        {
            INTERNAL_PopupsManager.EnsurePopupStaysWithinScreenBounds((Popup)sender);
        }

        /// <summary>
        /// Occurs when a ToolTip is closed and is no longer visible.
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        /// Occurs when a ToolTip becomes visible.
        /// </summary>
        public event RoutedEventHandler Opened;

#region Not supported yet
        /// <summary>Gets or sets the visual element or control that the tool tip should be positioned in relation to when opened by the <see cref="T:System.Windows.Controls.ToolTipService" />.</summary>
        /// <returns>The visual element or control that the tool tip should be positioned in relation to when opened by the <see cref="T:System.Windows.Controls.ToolTipService" />. The default is null.</returns>
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register("PlacementTarget", typeof(UIElement), typeof(ToolTip), null);

        /// <summary>Gets or sets how the <see cref="T:System.Windows.Controls.ToolTip" /> should be positioned in relation to the <see cref="P:System.Windows.Controls.ToolTip.PlacementTarget" />.</summary>
        /// <returns>One of the <see cref="T:System.Windows.Controls.Primitives.PlacementMode" /> values. The default is <see cref="F:System.Windows.Controls.Primitives.PlacementMode.Mouse" />. </returns>
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(ToolTip), new PropertyMetadata(PlacementMode.Mouse));
#endregion

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
