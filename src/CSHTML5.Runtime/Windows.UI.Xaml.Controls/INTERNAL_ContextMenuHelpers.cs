
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
