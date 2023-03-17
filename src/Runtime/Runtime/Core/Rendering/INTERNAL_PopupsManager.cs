

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
        private static int CurrentPopupRootIndentifier = 0;
        private static readonly HashSet<PopupRoot> PopupRootIdentifierToInstance = new();

        public static PopupRoot CreateAndAppendNewPopupRoot(Popup popup, Window parentWindow)
        {
            // Generate a unique identifier for the PopupRoot:
            string uniquePopupRootIdentifier = $"INTERNAL_Cshtml5_PopupRoot_{++CurrentPopupRootIndentifier}";

            var popupRoot = new PopupRoot(uniquePopupRootIdentifier, parentWindow, popup);

            //--------------------------------------
            // Create a DIV for the PopupRoot in the DOM tree:
            //--------------------------------------

            object popupRootDiv = INTERNAL_HtmlDomManager.CreatePopupRootDomElementAndAppendIt(popupRoot);
            popupRoot.INTERNAL_OuterDomElement
                = popupRoot.INTERNAL_InnerDomElement
                = popupRootDiv;
            popupRoot.IsConnectedToLiveTree = true;
            popupRoot.INTERNAL_AttachToDomEvents();
            popupRoot.UpdateIsVisible();

            //--------------------------------------
            // Remember the PopupRoot for later use:
            //--------------------------------------

            PopupRootIdentifierToInstance.Add(popupRoot);

            return popupRoot;
        }

        public static void RemovePopupRoot(PopupRoot popupRoot)
        {
            if (!PopupRootIdentifierToInstance.Remove(popupRoot))
            {
                throw new InvalidOperationException(
                    $"No PopupRoot with identifier '{popupRoot.INTERNAL_UniqueIndentifier}' was found.");
            }

            //--------------------------------------
            // Remove from the DOM:
            //--------------------------------------

            popupRoot.INTERNAL_DetachFromDomEvents();

            string sWindow = INTERNAL_InteropImplementation.GetVariableStringForJS(popupRoot.INTERNAL_ParentWindow.INTERNAL_RootDomElement);

            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $@"var popupRoot = document.getElementById(""{popupRoot.INTERNAL_UniqueIndentifier}"");
if (popupRoot) {sWindow}.removeChild(popupRoot);");

            popupRoot.INTERNAL_OuterDomElement = popupRoot.INTERNAL_InnerDomElement = null;
            popupRoot.IsConnectedToLiveTree = false;
            popupRoot.INTERNAL_LinkedPopup = null;
        }

        public static IEnumerable GetAllRootUIElements() // IMPORTANT: This is called via reflection from the "Visual Tree Inspector" of the Simulator. If you rename or remove it, be sure to update the Simulator accordingly!
        {
            // Include the main window:
            yield return Window.Current;

            // And all the popups:
            foreach (PopupRoot popupRoot in PopupRootIdentifierToInstance)
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

        /// <summary>
        /// Returns the coordinates of the UIElement, relative to the Window that contains it.
        /// </summary>
        /// <param name="element">The element of which the position will be returned.</param>
        /// <returns>The position of the element relative to the Window that contains it.</returns>
        public static Point GetUIElementAbsolutePosition(UIElement element)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                GeneralTransform gt = element.TransformToVisual(Window.GetWindow(element));
                return gt.Transform(new Point(0d, 0d));
            }
            return new Point();
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
            if (!element.IsVisible)
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
