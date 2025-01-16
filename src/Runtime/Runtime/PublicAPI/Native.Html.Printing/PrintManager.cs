

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CSHTML5.Native.Html.Printing
{
    /// <summary>
    /// Provides methods to print.
    /// </summary>
    public static class PrintManager
    {
        static UIElement CurrentPrintArea; // By default, the print area is set to Window.Current.Content. Please refer to the Setter of the property "Window.Current.Content" to see how the default print area is set.
        internal static bool IsDefaultPrintArea = true;

        /// <summary>
        /// Make the browser Print dialog appear.
        /// </summary>
        public static void Print()
        {
            OpenSilver.Interop.ExecuteJavaScriptVoid("window.print()");
        }

        /// <summary>
        /// Define the element that will be printed when pressing Ctrl+P or when clicking the Print command in the browser menu.
        /// </summary>
        /// <param name="element">An element in in the Visual Tree.</param>
        public static void SetPrintArea(UIElement element)
        {
            if (element != null)
            {
                if (element._isLoaded)
                {
                    // Remove the class "section-to-print" from the previous print area:
                    if (CurrentPrintArea != null && CurrentPrintArea._isLoaded)
                        OpenSilver.Interop.ExecuteJavaScriptVoid(
                            $"{OpenSilver.Interop.GetVariableStringForJS(CurrentPrintArea.OuterDiv)}.classList.remove(\"section-to-print\")");

                    // Add the class "section-to-print" to the new print area: (credits: https://stackoverflow.com/questions/468881/print-div-id-printarea-div-only )
                    OpenSilver.Interop.ExecuteJavaScriptVoid(
                        $"{OpenSilver.Interop.GetVariableStringForJS(element.OuterDiv)}.classList.add(\"section-to-print\")");

                    // Remember the new print area:
                    CurrentPrintArea = element;

                    // Remember that the print is no longer the default one:
                    IsDefaultPrintArea = false;
                }
                else
                    throw new InvalidOperationException("You can only set the print area to an element that is visible on screen or that has been loaded in the Visual Tree.");
            }
            else
                throw new ArgumentNullException("element");
        }

        /// <summary>
        /// Revert to the default print area, which is the area that will be printed when pressing Ctrl+P or when clicking the Print command in the browser menu.
        /// </summary>
        public static void ResetPrintArea()
        {
            // Set the print area to be the whole window (this is the default value, also called from the "setter" of "Window.Current.Content"):
            var root = Window.Current.Content;
            if (root != null)
            {
                SetPrintArea(root);
            }
            else
            {
                //-----------------------------------------------------------------------
                // If "root" is null, it is likely because we have arrived here due to a
                // call to the Print(element) method during the constructor of MainPage.
                // In that case, the "Window.Current.Content" has not yet been set.
                // It's not a problem, because it will be set later, so we will likely
                // arrive again here after the user code of the constructor.
                //-----------------------------------------------------------------------
            }

            // Remember that the print area is now back to the default one:
            IsDefaultPrintArea = true;
        }

        /// <summary>
        /// Print a specific element.
        /// </summary>
        /// <param name="element">The element to print.</param>
        public static void Print(UIElement element)
        {
            // Remember the previous print area, if any:
            var previousPrintArea = CurrentPrintArea;

            // Check whether the element is already in the Visual Tree:
            if (element._isLoaded)
            {
                //---------------------------------------------------
                // The element is already in the Visual Tree.
                // (in this case, the "Print" method is synchronous)
                //---------------------------------------------------

                // Temporarily set the print area to the element that the user wants to print:
                SetPrintArea(element);

                // Print:
                Print();

                // Revert to the previous print area:
                RestorePreviousPrintArea(previousPrintArea);
            }
            else
            {
                //---------------------------------------------------
                // The element is not in the Visual Tree.
                // (in this case, the "Print" method is asynchronous)
                //---------------------------------------------------

                // Create and show a popup that will be used to temporarily put the element into the visual tree. This is required in order to be able to print it. We show the popup off-screen so that it is not visible:
                var temporaryPopup = new Popup();

                // Create a container for the element
                var container = new Border()
                {
                    Child = element,
                    Margin = new Thickness(0, 10000, 0, 0)
                };

                // Listen to the "LayoutUpdated" event of the container, so that we are notified when the element becomes visible:
                container.LayoutUpdated += handler;

                void handler(object s, EventArgs e)
                {
                    container.LayoutUpdated -= handler;

                    // Print the element:
                    Print(element);

                    // Revert to the previous print area:
                    RestorePreviousPrintArea(previousPrintArea);

                    // Close the temporary popup:
                    temporaryPopup.IsOpen = false;
                }

                // Put the container into the popup, and open the popup:
                temporaryPopup.Child = container;
                temporaryPopup.IsOpen = true;
            }
        }

        private static void RestorePreviousPrintArea(UIElement previousPrintArea)
        {
            // Revert to the previous print area or the default one:
            if (previousPrintArea != null)
                SetPrintArea(previousPrintArea);
            else
                ResetPrintArea();
        }
    }
}
