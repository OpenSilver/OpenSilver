

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
            ((ToolTip)d).UpdatePopup((bool)e.NewValue);
        }

        private void UpdatePopup(bool isOpen)
        {
            if (isOpen)
            {
                bool wasPopupAlreadyOpen = (_parentPopup != null && _parentPopup.IsOpen == true);
                // Note: this prevents loops due to the fact that when the popup opens, the "IsOpen_Changed" method is called
                // because the tooltip is the child of the Popup so its properties are called when it is loaded into the Visual Tree.
                if (wasPopupAlreadyOpen)
                {
                    return;
                }

                // Make sure the tooltip is transparent to clicks:
                IsHitTestVisible = false;

                // Make sure the tooltip is Top/Left-aligned:
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;

                // Create the popup if not already created:
                if (_parentPopup == null)
                {
                    _parentPopup = new Popup()
                    {
                        Child = this,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        VerticalContentAlignment = VerticalAlignment.Top,
                    };

                    _parentPopup.DataContext = _owner?.DataContext;

                    _parentPopup.Loaded += new RoutedEventHandler(ParentPopupLoaded);
                }

                PlacementMode placement = EffectivePlacement;
                if (placement == PlacementMode.Mouse)
                {
                    Point position = GetMousePosition();
                    position.X = Math.Max(position.X + HorizontalOffset, 0.0) + 10.0;
                    position.Y = Math.Max(position.Y + VerticalOffset, 0.0) + 10.0;

                    _parentPopup.HorizontalOffset = position.X;
                    _parentPopup.VerticalOffset = position.Y;
                }
                else
                {
                    _parentPopup.HorizontalOffset = HorizontalOffset;
                    _parentPopup.VerticalOffset = VerticalOffset;
                    _parentPopup.Placement = placement;
                    _parentPopup.PlacementTarget = EffectivePlacementTarget;                    
                }

                _parentPopup.IsOpen = true;

                VisualStateManager.GoToState(this, VisualStates.StateToolTipOpen, true);

                if (Opened != null)
                {
                    Opened(this, new RoutedEventArgs { OriginalSource = this });
                }
            }
            else
            {
                if (_parentPopup != null && _parentPopup.IsOpen == true)
                {
                    VisualStateManager.GoToState(this, VisualStates.StateToolTipClosed, true);
                    _parentPopup.IsOpen = false;

                    if (Closed != null)
                    {
                        Closed(this, new RoutedEventArgs { OriginalSource = this });
                    }
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
                OpenSilver.Interop.ExecuteJavaScriptString(
                    "_opensilver.mousePositionX.toString() + \",\" + _opensilver.mousePositionY.toString()"));
        }

        private void ParentPopupLoaded(object sender, RoutedEventArgs e)
        {
            if (EffectivePlacement == PlacementMode.Mouse)
            {
                INTERNAL_PopupsManager.EnsurePopupStaysWithinScreenBounds((Popup)sender);
            }
        }

        /// <summary>
        /// Occurs when a ToolTip is closed and is no longer visible.
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        /// Occurs when a ToolTip becomes visible.
        /// </summary>
        public event RoutedEventHandler Opened;

        /// <summary>
        /// Identifies the <see cref="PlacementTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register(
                nameof(PlacementTarget),
                typeof(UIElement),
                typeof(ToolTip),
                null);

        /// <summary>
        /// Gets or sets the visual element or control that the tool tip should be 
        /// positioned in relation to when opened by the <see cref="ToolTipService" />.
        /// </summary>
        /// <returns>
        /// The visual element or control that the tool tip should be positioned in 
        /// relation to when opened by the <see cref="ToolTipService" />.
        /// The default is null.
        /// </returns>
        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        private UIElement EffectivePlacementTarget
        {
            get
            {
                if (_owner != null)
                {
                    return ToolTipService.GetPlacementTarget(_owner) ?? _owner;
                }

                return PlacementTarget;
            }
        }

        /// <summary>
        /// Identifies the <see cref="Placement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(
                nameof(Placement),
                typeof(PlacementMode),
                typeof(ToolTip),
                new PropertyMetadata(PlacementMode.Mouse));

        /// <summary>
        /// Gets or sets how the <see cref="ToolTip" /> should be positioned
        /// in relation to the <see cref="PlacementTarget" />.
        /// </summary>
        /// <returns>
        /// One of the <see cref="PlacementMode" /> values.
        /// The default is <see cref="PlacementMode.Mouse" />.
        /// </returns>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        private PlacementMode EffectivePlacement
        {
            get
            {
                if (_owner != null && !_owner.HasDefaultValue(ToolTipService.PlacementProperty))
                {
                    return ToolTipService.GetPlacement(_owner);
                }

                return Placement;
            }
        }

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
