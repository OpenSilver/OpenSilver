
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using CSHTML5;
using CSHTML5.Internal;

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
        
        internal static IEnumerable<PopupRoot> GetActivePopupRoots() => PopupRootIdentifierToInstance;
        
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
    }
}
