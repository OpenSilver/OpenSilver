

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
using System.ComponentModel;
using System.Windows.Markup;
using CSHTML5.Internal;
using DotNetForHtml5.Core;
using OpenSilver.Internal.Controls;
using System.Collections;
using System.Diagnostics;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Media;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Displays content on top of existing content, within the bounds of the application window.
    /// </summary>
    [ContentProperty("Child")]
    public partial class Popup : FrameworkElement
    {
        // Note for proper placement of the popup:
        //      - The HorizontalOffset and VerticalOffset define the placement of the Popup relative to the reference point.
        //      - The reference point is determined by the Placement and placement target. If the PlacementTarget property is not set, the placement target is the popup's parent. If the popup does not have a parent, then it is the top-left corner of the window (In wpf, it is the top-left corner of the screen but we're in a browser so we cannot do that).
        // Therefore, in order to correctly place the Popup, Horizontal and VerticalOffset should only be user-defined, and the only coordinates that should be internally set are those of the reference point.

        static int _currentZIndex = 0; //This int is to be able to put newly created popups in front of the former ones, as well as allowing to click on a Modal ChildWindow to put it in front of the others.
        PopupRoot _popupRoot;

        // Note: we use a ContentPresenter because we need a container that does not force its child
        // to be a logical child (since Popup.Child is already a logical child of the Popup).
        NonLogicalContainer _outerBorder; // Used for positioning and alignment.

        bool _isVisible;
        Point _referencePosition = new Point(); // This is the (X,Y) position of the reference point defined in the "Note for proper placement of the popup" above.



        ControlToWatch _controlToWatch; //Note: this is set when the popup is attached to an UIElement, so that we can remove it from the timer for refreshing the position of the popup when needed.

        internal event EventHandler INTERNAL_PopupMoved;


        internal Popup ParentPopup { get; private set; }


        internal PopupRoot PopupRoot
        {
            get
            {
                return _popupRoot;
            }
        }

        /// <summary>
        /// Occurs when the System.Windows.Controls.Primitives.Popup.IsOpen property changes to true.
        /// </summary>
        public event EventHandler Opened;
        void OnOpened()
        {
            if(Opened != null)
            {
                Opened(this, new EventArgs());
            }
        }

        /// <summary>
        /// Occurs when the System.Windows.Controls.Primitives.Popup.IsOpen property changes to false.
        /// </summary>
        public event EventHandler Closed;
        void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets or Sets the UIElement that the Popup will stick to. A null value will make the Popup stay at its originally defined position.
        /// </summary>
        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the PlacementTarget dependency property
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register(
                nameof(PlacementTarget), 
                typeof(UIElement), 
                typeof(Popup), 
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the position of the Popup relative to the UIElement it is attached to. NOTE: The only currently supported positions are Right and Bottom.
        /// </summary>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the Placement dependency property
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(
                nameof(Placement), 
                typeof(PlacementMode), 
                typeof(Popup),
                new FrameworkPropertyMetadata(PlacementMode.Right, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage)]
        public bool INTERNAL_AllowDisableClickTransparency = true;

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            //--------------------------------
            // Hide the PopupRoot when the Popup is removed from the visual tree:
            //--------------------------------

            this.HidePopupRootIfVisible();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new PopupRootAutomationPeer(this);

        /// <summary>
        /// Returns enumerator to logical children.
        /// </summary>
        internal override IEnumerator LogicalChildren
        {
            get
            {
                object content = Child;

                if (content == null)
                {
                    return EmptyEnumerator.Instance;
                }

                return new PopupModelTreeEnumerator(this, content);
            }
        }

        private class PopupModelTreeEnumerator : ModelTreeEnumerator
        {
            internal PopupModelTreeEnumerator(Popup popup, object child)
                : base(child)
            {
                Debug.Assert(popup != null, "popup should be non-null.");
                Debug.Assert(child != null, "child should be non-null.");

                _popup = popup;
            }

            protected override bool IsUnchanged
            {
                get
                {
                    return Object.ReferenceEquals(Content, _popup.Child);
                }
            }

            private Popup _popup;
        }

        #region Dependency Properties

        //-----------------------
        // CHILD
        //-----------------------

        /// <summary>
        ///  Gets or sets the content to be hosted in the popup.
        /// </summary>
        public UIElement Child
        {
            get { return (UIElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the Child dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register(
                nameof(Child), 
                typeof(UIElement), 
                typeof(Popup), 
                new PropertyMetadata(null, Child_Changed)
                { 
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet 
                });

        private static void Child_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            var newContent = (UIElement)e.NewValue;
            var oldContent = (UIElement)e.OldValue;
            if (oldContent != null)
            {
                popup.RemoveLogicalChild(oldContent);
            }
            if (newContent != null)
            {
                popup.AddLogicalChild(newContent);
                if (popup.CustomLayout && newContent is FrameworkElement feContent)
                {
                    feContent.CustomLayout = true;
                }
            }

            if (popup._isVisible)
            {
                popup._outerBorder.Content = newContent;
            }
            else
            {
                //-----------------------------------------------------
                // There is no need to update the content here because
                // it will automatically be updated when the popup
                // becomes visible (cf. method
                // "ShowPopupRootIfNotAlreadyVisible").
                //-----------------------------------------------------
            }
        }


        //-----------------------
        // ISOPEN
        //-----------------------

        /// <summary>
        /// Gets or sets whether the popup is currently displayed on the screen.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the IsOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen), 
                typeof(bool), 
                typeof(Popup), 
                new PropertyMetadata(false, IsOpen_Changed)
                { 
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet 
                });

        private static void IsOpen_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            bool isOpen = (bool)e.NewValue;
            if (popup.PlacementTarget == null)
            {
                //------------------------
                // The Popup is not in the Visual Tree.
                //------------------------

                // Show the popup if it was not already visible, or hide it if it was visible:
                if (isOpen)
                {
                    // Show the popup:
                    popup.ShowPopupRootIfNotAlreadyVisible();
                    popup.OnOpened();
                }
                else
                {
                    popup.OnClosed();
                    // Hide the popup:
                    popup.HidePopupRootIfVisible();
                }
            }
            else
            {
                //------------------------
                // The Popup is in the Visual Tree.
                //------------------------

                UIElement targetElement = popup.PlacementTarget;
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(targetElement))
                {
                    //the popup needs to be placed to a position relative to the popup.PlacementTarget element
                    if (isOpen)
                    {
                        popup.OnOpened();

                        popup._controlToWatch = Window.Current.INTERNAL_PositionsWatcher.AddControlToWatch(targetElement, popup.RefreshPopupPosition);
                        popup.ShowPopupRootIfNotAlreadyVisible();
                    }
                    else
                    {
                        popup.OnClosed();
                        Window.Current.INTERNAL_PositionsWatcher.RemoveControlToWatch(popup._controlToWatch);
                        popup._controlToWatch = null;
                        popup.HidePopupRootIfVisible();
                    }
                }
            }
        }

        private void RefreshPopupPosition(Point placementTargetPosition, Size placementTargetSize)
        {
            //we check if there is a placementTarget, if yes, if it is no longer in the visual tree, we remove it from the PositionsWatcher:
            if (PlacementTarget != null && !INTERNAL_VisualTreeManager.IsElementInVisualTree(PlacementTarget))
            {
                Window.Current.INTERNAL_PositionsWatcher.RemoveControlToWatch(_controlToWatch);
                _controlToWatch = null;
                HidePopupRootIfVisible();
            }
            else if (PlacementTarget != null)
            {
                _referencePosition = GetOffsetToPlacementTarget(placementTargetPosition, placementTargetSize);
                RepositionPopup(HorizontalOffset, VerticalOffset);

                if (StaysWithinScreenBounds)
                {
                    INTERNAL_PopupsManager.EnsurePopupStaysWithinScreenBounds(this);
                }
            }

            // Raise the internal "PopupMoved" event, which is useful for example to hide the validation popups of TextBoxes in case the user scrolls and the TextBox is no longer visible on screen (cf. ZenDesk 628):
            if (INTERNAL_PopupMoved != null)
                INTERNAL_PopupMoved(this, new EventArgs());
        }

        private Point GetOffsetToPlacementTarget(Point targetPosition, Size targetSize)
        {
            switch (Placement)
            {
                case PlacementMode.Bottom:
                    targetPosition.Y += targetSize.Height;
                    break;
                case PlacementMode.Mouse: // Not implemented
                case PlacementMode.Left: // Not implemented
                case PlacementMode.Top: // Not implemented
                case PlacementMode.Right:
                default:
                    targetPosition.X += targetSize.Width;
                    break;
            }

            return targetPosition;
        }

        //-----------------------
        // HORIZONTALOFFSET
        //-----------------------

        /// <summary>
        /// Gets or sets the distance between the left side of the application window and the left side of the popup.
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                nameof(HorizontalOffset), 
                typeof(double), 
                typeof(Popup),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange, HorizontalOffset_Changed)
                { 
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet 
                });

        private static void HorizontalOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            popup.RepositionPopup((double)e.NewValue, popup.VerticalOffset); //todo: the first parameter might need to be changed to a popup._relativePosition which takes into consideration the Placement, PlacementTarget and whether the Popup is in the Visual Tree.
        }

        //-----------------------
        // VERTICALOFFSET
        //-----------------------

        /// <summary>
        /// Gets or sets the distance between the top of the application window and the top of the popup.
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the VerticalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                nameof(VerticalOffset), 
                typeof(double), 
                typeof(Popup),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange, VerticalOffset_Changed)
                { 
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet 
                });

        private static void VerticalOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            popup.RepositionPopup(popup.HorizontalOffset, (double)e.NewValue);
        }

        private void RepositionPopup(double horizontalOffset, double verticalOffset)
        {
            if (_outerBorder != null)
            {
                _outerBorder.Margin = new Thickness(_referencePosition.X + horizontalOffset + _positionFixing.X, _referencePosition.Y + verticalOffset + _positionFixing.Y, 0d, 0d);
            }
        }

        private Point _positionFixing = new Point();
        /// <summary>
        /// Use this for temporary changes in position of the Popup (for example, when scrolling)
        /// </summary>
        internal Point PositionFixing
        {
            get { return _positionFixing; }
            set
            {
                _positionFixing = value;
                RepositionPopup(HorizontalOffset, VerticalOffset);
            }
        }

        //-----------------------
        // HORIZONTALCONTENTALIGNMENT (This is specific to CSHTML5 and is very useful for having full-screen popups such as ChildWindows)
        //-----------------------

        /// <summary>
        /// Gets or sets the horizontal alignment of the control's content.
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalContentAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalContentAlignment), 
                typeof(HorizontalAlignment), 
                typeof(Popup), 
                new PropertyMetadata(HorizontalAlignment.Left, HorizontalContentAlignment_Changed)
                { 
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet 
                });

        private static void HorizontalContentAlignment_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            if (popup._outerBorder != null)
                popup._outerBorder.HorizontalAlignment = (HorizontalAlignment)e.NewValue;
        }

        //-----------------------
        // VERTICALCONTENTALIGNMENT (This is specific to CSHTML5 and is very useful for having full-screen popups such as ChildWindows)
        //-----------------------

        /// <summary>
        /// Gets or sets the vertical alignment of the control's content.
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalContentAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalContentAlignment), 
                typeof(VerticalAlignment), 
                typeof(Popup),
                new FrameworkPropertyMetadata(VerticalAlignment.Top, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, VerticalContentAlignment_Changed)
                { 
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet 
                });

        private static void VerticalContentAlignment_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            if (popup._outerBorder != null)
                popup._outerBorder.VerticalAlignment = (VerticalAlignment)e.NewValue;
        }

        /// <summary>
        /// Get or sets a boolean stating whether the popup should stay within the screen boundaries or not.
        /// </summary>
        public bool StaysWithinScreenBounds
        {
            get { return (bool)GetValue(StaysWithinScreenBoundsProperty); }
            set { SetValue(StaysWithinScreenBoundsProperty, value); }
        }

        /// <summary>
        /// Identifies the StaysWithinScreenBounds dependency property.
        /// </summary>
        public static readonly DependencyProperty StaysWithinScreenBoundsProperty =
            DependencyProperty.Register(
                nameof(StaysWithinScreenBounds), 
                typeof(bool), 
                typeof(Popup), 
                new PropertyMetadata(false));

#endregion

        private void ShowPopupRootIfNotAlreadyVisible()
        {
            if (!_isVisible)
            {
                //---------------------
                // Show the PopupRoot:
                //---------------------

                var child = this.Child;

                // Get the window that is supposed to contain the popup:
                Window parentWindow = GetParentWindowOfPopup();

                // Create the popup root:
                _popupRoot = INTERNAL_PopupsManager.CreateAndAppendNewPopupRoot(this, parentWindow);

                // Set CustomLayout of the popup root:
                if (CustomLayout)
                {
                    // Setting Visibility to Collapse as a fix to the issue where Popup shows briefly at 0,0
                    // Will set to Visible where ShowPopupRootIfNotAlreadyVisible is called
                    _popupRoot.CustomLayout = true;
                    if (Child is FrameworkElement childFe)
                    {
                        childFe.CustomLayout = true;
                    }
                }

                UpdatePopupParent();

                // Clear the previous content if any:
                if (_outerBorder != null)
                    _outerBorder.Content = null;

                // Calculate the position of the parent of the popup, in case that the popup is in the Visual Tree:
                _referencePosition = CalculateReferencePosition();

                // Create a surrounding border to enable positioning and alignment:
                _outerBorder = new NonLogicalContainer()
                {
                    Margin = new Thickness(_referencePosition.X + this.HorizontalOffset, _referencePosition.Y + this.VerticalOffset, 0d, 0d),
                    Content = child,
                    HorizontalAlignment = this.HorizontalContentAlignment,
                    VerticalAlignment = this.VerticalContentAlignment,
                };
                Binding b = new Binding("Width") { Source = this };
                _outerBorder.SetBinding(Border.WidthProperty, b);
                Binding b2 = new Binding("Height") { Source = this };
                _outerBorder.SetBinding(Border.HeightProperty, b2);

                Binding b3 = new Binding("MaxHeight") { Source = this };
                _outerBorder.SetBinding(Border.MaxHeightProperty, b3);
                // Make sure that after the OuterBorder raises the Loaded event, the PopupRoot also raises the Loaded event:
                _outerBorder.Loaded += (s, e) =>
                {
                    _popupRoot?.RaiseLoadedEvent();
                    _popupRoot?.InvalidateMeasure();
                };

                _popupRoot.Content = _outerBorder;
                _isVisible = true;
                // Show the popup in front of any potential previously displayed popup:
                PutPopupInFront();
            }
            else
            {
                // The popup is already visible.
            }
        }

        private void HidePopupRootIfVisible()
        {
            if (_isVisible)
            {
                //---------------------
                // If the popup being closed is the one with the highest zIndex, we decrement it to reduce the chances of reaching the maximum value:
                //---------------------
                int closingPopupZIndex = Canvas.GetZIndex(_popupRoot);
                if (closingPopupZIndex == _currentZIndex)
                {
                    --_currentZIndex;
                }

                //---------------------
                // Hide the PopupRoot:
                //---------------------
                var popupRoot = _popupRoot;
                popupRoot.Content = null;
                INTERNAL_PopupsManager.RemovePopupRoot(popupRoot);
                popupRoot.INTERNAL_LinkedPopup = null;
                _popupRoot = null;
                _isVisible = false;
            }
            else
            {
                // The popup is already hidden.
            }
        }

        private Window GetParentWindowOfPopup()
        {
            // If the popup has a placement target, and the latter is in the visual tree, we get the window from there. Otherwise, if the popup itself is inthe visual tree, "Popup.INTERNAL_ParentWindow" should be populated. Otherwise, we use the default window (MainWindow) to display the popup.
            Window parentWindow = (this.PlacementTarget != null ? this.PlacementTarget.INTERNAL_ParentWindow : null) ?? this.INTERNAL_ParentWindow ?? Application.Current.MainWindow;

            return parentWindow;
        }

        private Point CalculateReferencePosition()
        {
            UIElement placementTarget = PlacementTarget;
            if (placementTarget != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(placementTarget))
            {
                Point p = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(placementTarget);
                Size s = placementTarget.GetBoundingClientSize();
                return GetOffsetToPlacementTarget(p, s);
            }
            else if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                GeneralTransform gt = TransformToVisual(null);
                return gt.Transform(new Point(0d, 0d));
            }

            return new Point();
        }

        public event EventHandler ClosedDueToOutsideClick;

        internal void CloseFromAnOutsideClick()
        {
            ClosedDueToOutsideClick?.Invoke(this, EventArgs.Empty);

            if (IsOpen)
                this.IsOpen = false;
        }

        internal event EventHandler<OutsideClickEventArgs> OutsideClick;

        internal void OnOutsideClick(OutsideClickEventArgs args) => OutsideClick?.Invoke(this, args);

        public bool StayOpen { get; set; } = true;

        internal void UpdatePopupParent()
        {
            UIElement element = PlacementTarget ?? (UIElement)VisualTreeHelper.GetParent(this);

            if (element == null)
            {
                ParentPopup = null;
                return;
            }

            while (true)
            {
                if (!(VisualTreeHelper.GetParent(element) is UIElement parent))
                {
                    break;
                }

                element = parent;
            }

            if (element is FrameworkElement fe && fe.Parent is Popup popup)
            {
                ParentPopup = popup;
            }
        }

        internal void PutPopupInFront()
        {
            if (Canvas.MaxZIndex > _currentZIndex)
            {
                _currentZIndex = Canvas.MaxZIndex;
            }

            bool needsZIndexChange = _currentZIndex == 0 ? true : (Canvas.GetZIndex(_popupRoot) != _currentZIndex);
            if (needsZIndexChange)
            {
                Canvas.SetZIndex(_popupRoot, ++_currentZIndex);
            }
        }
        [OpenSilver.NotImplemented]
        public void SetWindow(Window associatedWindow)
        {

        }
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement targetElement = this.PlacementTarget;

            if (targetElement != null)
            {
                Point placementTargetPosition = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(targetElement);

                //we get the size of the element:
                Size elementCurrentSize = targetElement.GetBoundingClientSize();

                //We put the popup at the calculated position:
                RefreshPopupPosition(placementTargetPosition, elementCurrentSize);
            }

            if (_popupRoot != null)
            {
                _popupRoot.InvalidateMeasure();
                _popupRoot.InvalidateArrange();
            }
            return finalSize;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void EnsurePopupStaysWithinScreenBounds(double forcedWidth = double.NaN, double forcedHeight = double.NaN)
        {
            INTERNAL_PopupsManager.EnsurePopupStaysWithinScreenBounds(this, forcedWidth, forcedHeight);
        }
    }

    internal sealed class OutsideClickEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }

    internal sealed class NonLogicalContainer : ContentPresenter
    {
        public NonLogicalContainer()
        {
            ContentTemplate = UIElementContentTemplate;
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (VisualTreeHelper.GetChildrenCount(this) > 0
                && VisualTreeHelper.GetChild(this, 0) is UIElement child)
            {
                child.UpdateIsVisible();
            }
        }

        internal override FrameworkElement TemplateChild
        {
            get => _templateChild;
            set
            {
                if (_templateChild != value)
                {
                    INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_templateChild, this);
                    _templateChild = value;
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_templateChild, this, 0);
                }
            }
        }

        private FrameworkElement _templateChild;
    }
}
