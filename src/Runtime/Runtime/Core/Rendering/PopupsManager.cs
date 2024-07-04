
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
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CSHTML5.Internal;

//
// Important: do not rename this class without updating the Simulator as well!
// The class is called via reflection from the Simulator.
//

namespace DotNetForHtml5.Core
{
    internal static class PopupsManager
    {
        private static readonly HashSet<PopupRoot> _popupRoots = new();

        // IMPORTANT: This is called via reflection from the "Visual Tree Inspector" of the Simulator.
        // If you rename or remove it, be sure to update the Simulator accordingly!
        public static IEnumerable GetAllRootUIElements()
        {
            // Include the main window:
            yield return Window.Current;

            // And all the popups:
            foreach (PopupRoot popupRoot in _popupRoots)
            {
                yield return popupRoot;
            }
        }

        internal static PopupRoot CreateAndAppendNewPopupRoot(Popup popup, Window parentWindow)
        {
            var popupRoot = new PopupRoot(parentWindow, popup);

            _popupRoots.Add(popupRoot);

            //--------------------------------------
            // Create a DIV for the PopupRoot in the DOM tree:
            //--------------------------------------

            popupRoot.OuterDiv = INTERNAL_HtmlDomManager.CreatePopupRootDomElementAndAppendIt(popupRoot);
            popupRoot.IsConnectedToLiveTree = true;
            popupRoot.UpdateIsVisible();

            return popupRoot;
        }

        internal static void RemovePopupRoot(PopupRoot popupRoot)
        {
            _popupRoots.Remove(popupRoot);

            if (popupRoot.OuterDiv is not null)
            {
                INTERNAL_HtmlDomManager.RemoveNodeNative(popupRoot.OuterDiv);
            }

            popupRoot.OuterDiv = null;
            popupRoot.IsConnectedToLiveTree = false;
            popupRoot.ParentPopup = null;
        }
        
        internal static IEnumerable<PopupRoot> GetActivePopupRoots() => _popupRoots;
        
        /// <summary>
        /// Returns the coordinates of the UIElement, relative to the Window that contains it.
        /// </summary>
        /// <param name="element">The element of which the position will be returned.</param>
        /// <returns>The position of the element relative to the Window that contains it.</returns>
        internal static Point GetUIElementAbsolutePosition(UIElement element)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                GeneralTransform gt = element.TransformToVisual(Window.GetWindow(element));
                return gt.Transform(new Point(0d, 0d));
            }
            return new Point();
        }
    }
}
