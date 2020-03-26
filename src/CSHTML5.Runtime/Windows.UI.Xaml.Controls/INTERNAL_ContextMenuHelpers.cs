

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


using CSHTML5.Internal;
using CSHTML5.Native.Html.Controls;
using CSHTML5.Native.Html.Input;
using System;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal static class INTERNAL_ContextMenuHelpers
    {
        public static void RegisterContextMenu(FrameworkElement frameworkElement, ContextMenu contextMenu)
        {
            if (contextMenu != null)
            {
                // Remember the FrameworkElement:
                contextMenu.INTERNAL_ElementToWhichThisMenuIsAssigned = frameworkElement;

                // Register the right-click event:
#if MIGRATION
                frameworkElement.MouseRightButtonUp -= FrameworkElement_MouseRightButtonUp; // Note: we unregister the event first, in order not to register twice.
                frameworkElement.MouseRightButtonUp += FrameworkElement_MouseRightButtonUp;
#else
                frameworkElement.RightTapped -= FrameworkElement_RightTapped; // Note: we unregister the event first, in order not to register twice.
                frameworkElement.RightTapped += FrameworkElement_RightTapped;
#endif
            }
            else
            {
                UnregisterContextMenu(frameworkElement);
            }
        }

        public static void RegisterContextMenu(HtmlCanvasElement htmlCanvasElement, ContextMenu contextMenu)
        {
            if (contextMenu != null)
            {
                // Remember the HtmlCanvasElement:
                contextMenu.INTERNAL_HtmlCanvasElementToWhichThisMenuIsAssigned = htmlCanvasElement;

                // Register the right-click event:
                htmlCanvasElement.RightTapped -= HtmlCanvasElement_RightTapped; // Note: we unregister the event first, in order not to register twice.
                htmlCanvasElement.RightTapped += HtmlCanvasElement_RightTapped;
            }
            else
            {
                UnregisterContextMenu(htmlCanvasElement);
            }
        }

        static void htmlCanvasElement_RightTapped(object sender, CSHTML5.Native.Html.Input.HtmlCanvasPointerRoutedEventArgs e)
        {
            
        }

        public static void UnregisterContextMenu(FrameworkElement frameworkElement)
        {
            // Unregister all events:
#if MIGRATION
            frameworkElement.MouseRightButtonUp += FrameworkElement_MouseRightButtonUp;
#else
            frameworkElement.RightTapped -= FrameworkElement_RightTapped;
#endif
        }

        public static void UnregisterContextMenu(HtmlCanvasElement htmlCanvasElement)
        {
            // Unregister all events:
            htmlCanvasElement.RightTapped -= HtmlCanvasElement_RightTapped;
        }

#if MIGRATION
        static void FrameworkElement_MouseRightButtonUp(object sender, Input.MouseButtonEventArgs e)
#else
        static void FrameworkElement_RightTapped(object sender, Input.RightTappedRoutedEventArgs e)
#endif
        {
            // Show the ContextMenu where the pointer is located:
            var frameworkElement = (FrameworkElement)sender;
            var contextMenu = frameworkElement.ContextMenu;
            if (contextMenu != null)
            {
                if (contextMenu.IsOpen == false)
                {
#if MIGRATION
                    Point pointerPosition = e.GetPosition(null);
#else
                    Point pointerPosition = e.GetCurrentPoint(null).Position;
#endif
                    frameworkElement.INTERNAL_RaiseContextMenuOpeningEvent(pointerPosition.X, pointerPosition.Y);
                    contextMenu.INTERNAL_OpenAtCoordinates(pointerPosition);
                }
            }
            else
            {
                UnregisterContextMenu(frameworkElement);
            }
        }

        static void HtmlCanvasElement_RightTapped(object sender, HtmlCanvasPointerRoutedEventArgs e)
        {
            // Show the ContextMenu where the pointer is located:
            var htmlCanvasElement = (HtmlCanvasElement)sender;
            var contextMenu = htmlCanvasElement.ContextMenu;
            if (contextMenu != null)
            {
                if (contextMenu.IsOpen == false)
                {
#if MIGRATION
                    Point pointerPosition = e.GetPosition(null);
#else
                    Point pointerPosition = e.GetCurrentPoint(null).Position;
#endif
                    htmlCanvasElement.INTERNAL_RaiseContextMenuOpeningEvent(pointerPosition.X, pointerPosition.Y);
                    contextMenu.INTERNAL_OpenAtCoordinates(pointerPosition);
                }
            }
            else
            {
                UnregisterContextMenu(htmlCanvasElement);
            }
        }
    }
}
