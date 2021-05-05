

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


using CSHTML5;
using System;
using System.Collections;
using System.Collections.Generic;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
#endif

namespace DotNetForHtml5.Core // Important: do not rename this class without updating the Simulator as well! The class is called via reflection from the Simulator.
{
    internal static class INTERNAL_PopupsManager // Important! DO NOT RENAME this class without updating the Simulator as well! // Note: this class is "internal" but still visible to the Emulator because of the "InternalsVisibleTo" flag in "Assembly.cs".
    {
        static int CurrentPopupRootIndentifier = 0;
        static Dictionary<string, object> PopupRootIdentifierToInstance = new Dictionary<string, object>();


        // Is called every time a click happens, the sender is the root element that has received the click
#if MIGRATION
        internal static void OnClickOnPopupOrWindow(object sender, MouseButtonEventArgs e)
#else
        internal static void OnClickOnPopupOrWindow(object sender, PointerRoutedEventArgs e)
#endif
        {
            // Note: If a popup has StayOpen=True, the value of "StayOpen" of its parents is ignored.
            // In other words, the parents of a popup that has StayOpen=True will always stay open
            // regardless of the value of their "StayOpen" property.

            HashSet2<Popup> listOfPopupThatMustBeClosed = new HashSet2<Popup>();
            List<PopupRoot> popupRootList = new List<PopupRoot>();

            foreach (object obj in GetAllRootUIElements())
            {
                if (obj is PopupRoot)
                {
                    PopupRoot root = (PopupRoot)obj;
                    popupRootList.Add(root);

                    if (root.INTERNAL_LinkedPopup != null)
                        listOfPopupThatMustBeClosed.Add(root.INTERNAL_LinkedPopup);
                }
            }

            // We determine which popup needs to stay open after this click
            foreach (PopupRoot popupRoot in popupRootList)
            {
                if (popupRoot.INTERNAL_LinkedPopup != null)
                {
                    // We must prevent all the parents of a popup to be closed when:
                    // - this popup is set to StayOpen
                    // - or the click happend in this popup

                    Popup popup = popupRoot.INTERNAL_LinkedPopup;

                    if (popup.StayOpen || sender == popupRoot)
                    {
                        do
                        {
                            if (!listOfPopupThatMustBeClosed.Contains(popup))
                                break;

                            listOfPopupThatMustBeClosed.Remove(popup);

                            popup = popup.ParentPopup;

                        } while (popup != null);
                    }
                }
            }

            foreach (Popup popup in listOfPopupThatMustBeClosed)
            {
                popup.CloseFromAnOutsideClick();
            }

        }

        public static PopupRoot CreateAndAppendNewPopupRoot(Window parentWindow)
        {
            // Generate a unique identifier for the PopupRoot:
            CurrentPopupRootIndentifier++;
            string uniquePopupRootIdentifier = "INTERNAL_Cshtml5_PopupRoot_" + CurrentPopupRootIndentifier.ToString();

            //--------------------------------------
            // Create a DIV for the PopupRoot in the DOM tree:
            //--------------------------------------

            CSHTML5.Interop.ExecuteJavaScriptAsync(
@"
var popupRoot = document.createElement('div');
popupRoot.setAttribute('id', $0);
popupRoot.style.position = 'absolute';
popupRoot.style.width = '100%';
popupRoot.style.height = '100%';
popupRoot.style.overflowX = 'hidden';
popupRoot.style.overflowY = 'hidden';
popupRoot.style.pointerEvents = 'none';
$1.appendChild(popupRoot);
", uniquePopupRootIdentifier, parentWindow.INTERNAL_RootDomElement);

            //--------------------------------------
            // Get the PopupRoot DIV:
            //--------------------------------------

            object popupRootDiv;

            if (Interop.IsRunningInTheSimulator)
                popupRootDiv = new INTERNAL_HtmlDomElementReference(uniquePopupRootIdentifier, null);
            else
                popupRootDiv = Interop.ExecuteJavaScriptAsync("document.getElementByIdSafe($0)", uniquePopupRootIdentifier);

            //--------------------------------------
            // Create the C# class that points to the PopupRoot DIV:
            //--------------------------------------

            var popupRoot = new PopupRoot(uniquePopupRootIdentifier, parentWindow);
            popupRoot.INTERNAL_OuterDomElement
                = popupRoot.INTERNAL_InnerDomElement
                = popupRootDiv;

            //--------------------------------------
            // Listen to clicks anywhere in the popup (this is used to close other popups that are not supposed to stay open):
            //--------------------------------------

#if MIGRATION
            popupRoot.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(INTERNAL_PopupsManager.OnClickOnPopupOrWindow), true);
#else
            popupRoot.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(INTERNAL_PopupsManager.OnClickOnPopupOrWindow), true);
#endif

            //--------------------------------------
            // Remember the PopupRoot for later use:
            //--------------------------------------

            PopupRootIdentifierToInstance.Add(uniquePopupRootIdentifier, popupRoot);

            return popupRoot;
        }

        public static void RemovePopupRoot(PopupRoot popupRoot)
        {
            string uniquePopupRootIdentifier = popupRoot.INTERNAL_UniqueIndentifier;
            if (PopupRootIdentifierToInstance.ContainsKey(uniquePopupRootIdentifier))
            {
                Window parentWindow = popupRoot.INTERNAL_ParentWindow;

                //--------------------------------------
                // Stop listening to clicks anywhere in the popup (this was used to close other popups that are not supposed to stay open):
                //--------------------------------------

#if MIGRATION
                popupRoot.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(INTERNAL_PopupsManager.OnClickOnPopupOrWindow));
#else
                popupRoot.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(INTERNAL_PopupsManager.OnClickOnPopupOrWindow));
#endif

                //--------------------------------------
                // Remove from the DOM:
                //--------------------------------------

                CSHTML5.Interop.ExecuteJavaScriptAsync(
@"
var popupRoot = document.getElementByIdSafe($0);
$1.removeChild(popupRoot);
", uniquePopupRootIdentifier, parentWindow.INTERNAL_RootDomElement);

                //--------------------------------------
                // Remove from the list of popups:
                //--------------------------------------

                PopupRootIdentifierToInstance.Remove(uniquePopupRootIdentifier);
            }
            else
                throw new Exception("No PopupRoot with the following identifier was found: " + uniquePopupRootIdentifier + ". Please contact support.");
        }

        public static IEnumerable GetAllRootUIElements() // IMPORTANT: This is called via reflection from the "Visual Tree Inspector" of the Simulator. If you rename or remove it, be sure to update the Simulator accordingly!
        {
            // Include the main window:
            yield return Window.Current;

            // And all the popups:
            foreach (var popupRoot in
#if BRIDGE
                INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(PopupRootIdentifierToInstance)
#else
                PopupRootIdentifierToInstance.Values
#endif
                )
            {
                yield return popupRoot;
            }
        }

        public static Point CalculatePopupAbsolutePositionBasedOnElementPosition(UIElement parentElement, double horizontalOffset, double verticalOffset)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(parentElement))
            {
                // Determine the absolute position of the parent element:
                GeneralTransform gt = parentElement.TransformToVisual(parentElement.INTERNAL_ParentWindow);
                Point parentElementAbsolutePosition = gt.Transform(new Point(0d, 0d));

                // If the parent size can be determined, displace the tooltip by the parent Height:
                if (parentElement is FrameworkElement
                    && !double.IsNaN(((FrameworkElement)parentElement).ActualHeight))
                {
                    verticalOffset += ((FrameworkElement)parentElement).ActualHeight;
                }

                return new Point(parentElementAbsolutePosition.X + horizontalOffset, parentElementAbsolutePosition.Y + verticalOffset);
            }
            else
            {
                return new Point();
            }
        }

        public static Point GetUIElementAbsolutePosition(UIElement element)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                GeneralTransform gt = element.TransformToVisual(null);

                // Note: by passing "null" to "TransformToVisual", we tell the framework to
                // get the coordinates relative to the Window root.

                return gt.Transform(new Point(0d, 0d));
            }
            return new Point();
        }

        public static void EnsurePopupStaysWithinScreenBounds(Popup popup)
        {
            if (popup.IsOpen
                && popup.PopupRoot != null
                && popup.PopupRoot.Content is FrameworkElement)
            {
                // Determine the size of the popup:
                FrameworkElement content = (FrameworkElement)popup.PopupRoot.Content;
                double popupActualWidth = content.ActualWidth;
                double popupActualHeight = content.ActualHeight;
                if (!double.IsNaN(popupActualWidth)
                    && !double.IsNaN(popupActualHeight)
                    && popupActualWidth > 0
                    && popupActualHeight > 0)
                {
                    Point popupPosition = new Point(0, 0);
                    if (popup.INTERNAL_VisualParent != null)
                    {
                        popupPosition = popup.TransformToVisual(Application.Current.RootVisual).Transform(popupPosition);
                    }

                    // Determine the size of the window:
                    Rect windowBounds = Window.Current.Bounds;
                    double popupX = popup.HorizontalOffset + popupPosition.X;
                    double popupY = popup.VerticalOffset + popupPosition.Y;

                    // Calculate the area of the popup that is outside the screen bounds:
                    // Note: when adding widthOfLeftOverflow and heightOfTopOverflow, I guessed the X and Y of windowBounds were 0 because of the way widthOfRightOverflow and heightOfBottomOverflow was calculated.
                    double widthOfRightOverflow = (popupX + popupActualWidth) - windowBounds.Width;
                    double widthOfLeftOverflow = -popupX; // Note: this would be -(popupX - windowBounds.X)
                    double heightOfBottomOverflow = (popupY + popupActualHeight) - windowBounds.Height;
                    double heightOfTopOverflow = -popupY; // Note: this would be -(popupY - windowBounds.Y)
                    double totalWidthOverflow = widthOfRightOverflow + widthOfLeftOverflow;
                    double totalHeightOverflow = heightOfBottomOverflow + heightOfTopOverflow;

                    //Arbitrary decision here: If the popup is too big to fit on screen, we align it with the left and the top of the screen (depending on whether it is too wide, too high, or both), and generally give priority to fixing left and top overflow.
                    Point positionFixing = new Point();
                    // Adjust the position of the popup to remain on-screen:
                    if(totalWidthOverflow > 0 || widthOfLeftOverflow > 0)
                    {
                        //align to the left:
                        positionFixing.X = widthOfLeftOverflow;
                    }
                    else if (widthOfRightOverflow > 0)
                    {
                        positionFixing.X = - widthOfRightOverflow;
                    }
                    if(totalHeightOverflow > 0 || heightOfTopOverflow > 0)
                    {
                        positionFixing.Y = heightOfTopOverflow;
                    }
                    else if (heightOfBottomOverflow > 0)
                    {
                        positionFixing.Y = - heightOfBottomOverflow; //todo: same as for HorizontalOffset.
                    }

                    popup.PositionFixing = positionFixing;
                }
            }
        }

        /// <summary>
        /// Determines whether the parent of a popup is visible on screen, and if it is the foremost element (ie. not covered by another element). This is useful for example to hide the TextBox validation tooltips when they become hidden after scrolling (cf. ZenDesk 628).
        /// </summary>
        /// <param name="popup">The popup.</param>
        /// <returns>Returns True if the parent of the popup is visible on screen and it is the foremost element, False otherwise.</returns>
        public static bool IsPopupParentVisibleOnScreen(Popup popup)
        {
            if (popup.PlacementTarget != null)
            {
                if (popup.PlacementTarget is FrameworkElement)
                {
                    var element = (FrameworkElement)popup.PlacementTarget;

                    return IsElementVisibleOnScreen(element);
                }
                else
                    throw new InvalidOperationException("The Popup.PlacementTarget property must contain a FrameworkElement");
            }
            else
                throw new InvalidOperationException("The Popup.PlacementTarget property must be set.");
        }

        /// <summary>
        /// Determines whether an element is visible on screen, and if it is the foremost element (ie. it is not covered by another element). This is useful for example to hide the TextBox validation tooltips when they become hidden after scrolling (cf. ZenDesk 628).
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <returns>Returns True if the element is visible on screen and it is the foremost element, False otherwise.</returns>
        internal static bool IsElementVisibleOnScreen(FrameworkElement element)
        {
            // First, verify that the element is in the visual tree:
            if (!element._isLoaded)
                return false;

            // Then, check whether the element is visible in the DOM tree, regardless of elements that may cover it. This can be false if at least one of the parents has "Visibility=Collapsed".
            if (!INTERNAL_VisibilityChangedNotifier.IsElementVisible(element))
                return false;

            // Verify that the size of the element can be read:
            Size actualSize = element.INTERNAL_GetActualWidthAndHeight();
            if (double.IsNaN(actualSize.Width) || double.IsNaN(actualSize.Height))
                return false;

            // Get the bounds of the element:
            Point topLeftCoordinates = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(element);
            Rect elementBounds = new Rect(topLeftCoordinates.X, topLeftCoordinates.Y, actualSize.Width, actualSize.Height);

            // Get the bounds of the window:
            Rect windowBounds = Window.Current.Bounds;

            // Verify that the element is at least partially inside the window:
            if (elementBounds.X + elementBounds.Width < 0d
                || elementBounds.Y + elementBounds.Height < 0d
                || elementBounds.X > windowBounds.Width
                || elementBounds.Y > windowBounds.Height)
                return false;

            // Get the coordinates of the center point:
            Point centerPosition = new Point(elementBounds.X + elementBounds.Width / 2, elementBounds.Y + elementBounds.Height / 2);

            // Do a "HitTest" to verify that there is no element overlapping it:
            var elementAtCenterPosition = VisualTreeHelper.FindElementInHostCoordinates(centerPosition);

            if (elementAtCenterPosition == element)
                return true;
            else
                return false;
        }
    }
}
