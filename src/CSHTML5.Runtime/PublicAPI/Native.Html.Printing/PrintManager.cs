﻿
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
#if WORKINPROGRESS
using System.Windows.Threading;
#endif
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using System.Windows;
#endif

namespace CSHTML5.Native.Html.Printing
{
    /// <summary>
    /// Provides methods to print.
    /// </summary>
    public static class PrintManager
    {
        static FrameworkElement CurrentPrintArea; // By default, the print area is set to Window.Current.Content. Please refer to the Setter of the property "Window.Current.Content" to see how the default print area is set.
        internal static bool IsDefaultPrintArea = true;

        /// <summary>
        /// Make the browser Print dialog appear.
        /// </summary>
        public static void Print()
        {
            if (!Interop.IsRunningInTheSimulator)
            {
                Interop.ExecuteJavaScript("window.print()");
            }
            else
            {
                MessageBox.Show("The application requested to show the print dialog. This feature is not implemented in the Simulator. Please run the application in the browser instead.");
            }
        }

        /// <summary>
        /// Define the element that will be printed when pressing Ctrl+P or when clicking the Print command in the browser menu.
        /// </summary>
        /// <param name="element">An element in in the Visual Tree.</param>
        public static void SetPrintArea(FrameworkElement element)
        {
            if (element != null)
            {
                if (element._isLoaded)
                {
                    if (!Interop.IsRunningInTheSimulator)
                    {
                        // Remove the class "section-to-print" from the previous print area:
                        if (CurrentPrintArea != null && CurrentPrintArea._isLoaded)
                            Interop.ExecuteJavaScript(@"$0.classList.remove(""section-to-print"")", CurrentPrintArea.INTERNAL_OuterDomElement);

                        // Add the class "section-to-print" to the new print area: (credits: https://stackoverflow.com/questions/468881/print-div-id-printarea-div-only )
                        Interop.ExecuteJavaScript(@"$0.classList.add(""section-to-print"")", element.INTERNAL_OuterDomElement);

                        // Remember the new print area:
                        CurrentPrintArea = element;

                        // Remember that the print is no longer the default one:
                        IsDefaultPrintArea = false;
                    }
                    else
                        MessageBox.Show("The application requested to set the print area. This feature is not implemented in the Simulator. Please run the application in the browser instead.");
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
            if (!Interop.IsRunningInTheSimulator)
            {
                // Set the print area to be the whole window (this is the default value, also called from the "setter" of "Window.Current.Content"):
                var root = Window.Current.Content;
                if (root != null)
                {
                    SetPrintArea((FrameworkElement)root);
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
            else
                MessageBox.Show("The application requested to reset the print area. This feature is not implemented in the Simulator. Please run the application in the browser instead.");
        }

        /// <summary>
        /// Print a specific element.
        /// </summary>
        /// <param name="element">The element to print.</param>
        public static void Print(FrameworkElement element)
        {
            if (!Interop.IsRunningInTheSimulator)
            {
                // Remember the previous print area, if any:
                var previousPrintArea = CurrentPrintArea;

                // Check whether the element is aready in the Visual Tree:
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
                    var temporaryPopup = new Popup() { VerticalOffset = 10000 };
                    temporaryPopup.IsOpen = true;

                    // Create a container for the element
                    var container = new Border()
                    {
                        Child = element
                    };

                    // Listen to the "Loaded" event of the container, so that we are notified when the element becomes visible:
                    container.Loaded += (s2, e2) =>
                    {
                        // Print the element:
                        PrintManager.Print(element);

                        // Revert to the previous print area:
                        RestorePreviousPrintArea(previousPrintArea);

                        // Close the temporary popup:
#if MIGRATION
                        Dispatcher
#else
                        CoreDispatcher
#endif
                        .INTERNAL_GetCurrentDispatcher().BeginInvoke(() =>
                        {
                            temporaryPopup.IsOpen = false;
                        });
                    };

                    // Put the container into the popup, and open the popup:
                    temporaryPopup.Child = container;
                    temporaryPopup.IsOpen = true;
                }
            }
            else
                MessageBox.Show("The application requested to print. This feature is not implemented in the Simulator. Please run the application in the browser instead.");
        }

        private static void RestorePreviousPrintArea(FrameworkElement previousPrintArea)
        {
            // Revert to the previous print area or the default one:
            if (previousPrintArea != null)
                SetPrintArea(previousPrintArea);
            else
                ResetPrintArea();
        }
    }
}
