

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
using System.Windows.Markup;
using System.ComponentModel;
using System.Collections.Generic;
using DotNetForHtml5.Core;

#if MIGRATION
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
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


        PopupRoot _popupRoot;
        Border _outerBorder; // Used for positioning and alignment.
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


        private UIElement _placementTarget;
        /// <summary>
        /// Gets or Sets the UIElement that the Popup will stick to. A null value will make the Popup stay at its originally defined position.
        /// </summary>
        public UIElement PlacementTarget //todo: change this into a DependencyProperty
        {
            get { return _placementTarget; }
            set { _placementTarget = value; }
        }

        private PlacementMode _placement;
        /// <summary>
        /// Gets or sets the position of the Popup relative to the UIElement it is attached to. NOTE: The only currently supported positions are Right and Bottom.
        /// </summary>
        public PlacementMode Placement //todo: change this into a DependencyProperty
        {
            get { return _placement; }
            set { _placement = value; }
        }


        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            //--------------------------------
            // Hide the PopupRoot when the Popup is removed from the visual tree:
            //--------------------------------

            this.HidePopupRootIfVisible();
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
            DependencyProperty.Register("Child", typeof(UIElement), typeof(Popup), new PropertyMetadata(null, Child_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        private static void Child_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            if (popup._isVisible)
            {
                var newContent = (UIElement)e.NewValue;
                popup._outerBorder.Child = newContent;
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
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Popup), new PropertyMetadata(false, IsOpen_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


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
                }
                else
                {
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
                        Window.Current.INTERNAL_PositionsWatcher.AddControlToWatch(targetElement, popup.RefreshPopupPosition);
                        popup.ShowPopupRootIfNotAlreadyVisible();

                        //We calculate the position at which the popup will be:

                        //We get the position of the element to which the popup is attached:
                        Point placementTargetPosition = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(targetElement);

                        //we get the size of the element:
                        Size elementCurrentSize;
                        if (targetElement is FrameworkElement)
                        {
                            elementCurrentSize = ((FrameworkElement)targetElement).INTERNAL_GetActualWidthAndHeight();
                        }
                        else
                        {
                            elementCurrentSize = new Size();
                        }

                        //We put the popup at the calculated position:
                        popup.RefreshPopupPosition(placementTargetPosition, elementCurrentSize); //note: We might have a position bug here if parentposition is set, ie if popup is in the visual tree
                    }
                    else
                    {
                        Window.Current.INTERNAL_PositionsWatcher.RemoveControlToWatch(popup._controlToWatch);
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
                HidePopupRootIfVisible();
            }
            else if (PlacementTarget != null)
            {
                //We change the position so that the popup goes to the correct relative position:
                switch (Placement)
                {
                    case PlacementMode.Bottom:
                        if (!double.IsNaN(placementTargetSize.Height))
                        {
                            placementTargetPosition.Y += placementTargetSize.Height;
                        }
                        break;
                    //case PlacementMode.Right:
                    //    break;
                    //case PlacementMode.Mouse:
                    //    break;
                    //case PlacementMode.Left:
                    //    break;
                    //case PlacementMode.Top:
                    //    break;
                    default: //note: we currently consider Right as the default placement (only Bottom and Right are supported)
                        if (!double.IsNaN(placementTargetSize.Width))
                        {
                            placementTargetPosition.X += placementTargetSize.Width;
                        }
                        break;
                }

                _referencePosition = placementTargetPosition;
                RepositionPopup(HorizontalOffset, VerticalOffset);
            }

            // Raise the internal "PopupMoved" event, which is useful for example to hide the validation popups of TextBoxes in case the user scrolls and the TextBox is no longer visible on screen (cf. ZenDesk 628):
            if (INTERNAL_PopupMoved != null)
                INTERNAL_PopupMoved(this, new EventArgs());
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
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(Popup), new PropertyMetadata(0d, HorizontalOffset_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

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
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(Popup), new PropertyMetadata(0d, VerticalOffset_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void VerticalOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            popup.RepositionPopup(popup.HorizontalOffset, (double)e.NewValue);
        }

        private void RepositionPopup(double horizontalOffset, double verticalOffset)
        {
            if (_outerBorder != null)
            {
                _outerBorder.Margin = new Thickness(_referencePosition.X + horizontalOffset, _referencePosition.Y + verticalOffset, 0d, 0d);
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
            DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(Popup), new PropertyMetadata(HorizontalAlignment.Left, HorizontalContentAlignment_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

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
            DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(Popup), new PropertyMetadata(VerticalAlignment.Top, VerticalContentAlignment_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void VerticalContentAlignment_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            if (popup._outerBorder != null)
                popup._outerBorder.VerticalAlignment = (VerticalAlignment)e.NewValue;
        }

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
                var popupRoot = INTERNAL_PopupsManager.CreateAndAppendNewPopupRoot(parentWindow);
                _popupRoot = popupRoot;
                _popupRoot.INTERNAL_LinkedPopup = this;
                UpdatePopupParent();

                // Clear the previous content if any:
                if (_outerBorder != null)
                    _outerBorder.Child = null;

                // Calculate the position of the parent of the popup, in case that the popup is in the Visual Tree:
                _referencePosition = CalculateReferencePosition(parentWindow) ?? new Point();

                // We make it transparent to clicks only if either the popup has a false "IsHitTestVisible", or the content of the popup has a false "IsHitTestVisible":
                bool transparentToClicks = (!this.IsHitTestVisible) || (child is FrameworkElement && !((FrameworkElement)child).IsHitTestVisible);

                // Create a surrounding border to enable positioning and alignment:
                _outerBorder = new Border()
                {
                    Margin = new Thickness(_referencePosition.X + this.HorizontalOffset, _referencePosition.Y + this.VerticalOffset, 0d, 0d),
                    Child = child,
                    HorizontalAlignment = this.HorizontalContentAlignment,
                    VerticalAlignment = this.VerticalContentAlignment,
                    INTERNAL_ForceEnableAllPointerEvents = !transparentToClicks, // This is here because we set "pointerEvents='none' to the PopupRoot, so we need to re-enable pointer events in the children (unless we have calculated that the popup should be "transparentToClicks").
                };

                // Make sure that after the OuterBorder raises the Loaded event, the PopupRoot also raises the Loaded event:
                _outerBorder.Loaded += (s, e) => { popupRoot.INTERNAL_RaiseLoadedEvent(); };

                popupRoot.Content = _outerBorder;
                _isVisible = true;
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
                // Hide the PopupRoot:
                //---------------------
                var popupRoot = _popupRoot;
                popupRoot.INTERNAL_LinkedPopup = null;
                popupRoot.Content = null;
                INTERNAL_PopupsManager.RemovePopupRoot(popupRoot);
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
            Window parentWindow = (this._placementTarget != null ? this._placementTarget.INTERNAL_ParentWindow : null) ?? this.INTERNAL_ParentWindow ?? Application.Current.MainWindow;

            return parentWindow;
        }

        private Point? CalculateReferencePosition(Window parentWindow)
        {
            UIElement placementTarget = this.PlacementTarget;
            if (placementTarget != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(placementTarget))
            {
                GeneralTransform gt = placementTarget.TransformToVisual(parentWindow);
                Point p = gt.Transform(new Point(0d, 0d));
            }
            else if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                GeneralTransform gt = this.TransformToVisual(parentWindow);
                Point p = gt.Transform(new Point(0d, 0d));
                return p;
            }
            return new Point();
        }



        public event EventHandler ClosedDueToOutsideClick;

        internal void CloseFromAnOutsideClick()
        {
            if (ClosedDueToOutsideClick != null)
                ClosedDueToOutsideClick(this, new EventArgs());

            if (IsOpen)
                this.IsOpen = false;
        }


        private bool _stayOpen = true;
        public bool StayOpen
        {
            get { return _stayOpen; }
            set { _stayOpen = value; }
        }


        internal void UpdatePopupParent()
        {

            UIElement element = PlacementTarget ?? (UIElement)this.INTERNAL_VisualParent;

            if (element == null)
            {
                ParentPopup = null;
                return;
            }
            while (true)
            {
                if (element is PopupRoot)
                {
                    ParentPopup = ((PopupRoot)element).INTERNAL_LinkedPopup;
                    return;
                }

                DependencyObject obj = VisualTreeHelper.GetParent(element);
                if (obj is UIElement)
                    element = (UIElement)obj;
                else
                    break;
            }

        }

#if WORKINPROGRESS
        public event EventHandler Closed;

        public event EventHandler Opened;

        public void SetWindow(Window associatedWindow)
        {

        }
#endif
    }
}
