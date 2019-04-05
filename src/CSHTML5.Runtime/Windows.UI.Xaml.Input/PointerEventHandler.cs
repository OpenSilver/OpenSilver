
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
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if MIGRATION

    /// <summary>
    /// Represents the method that handles the System.Windows.UIElement.MouseLeftButtonDown
    /// and System.Windows.UIElement.MouseLeftButtonUp events.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    /// <summary>
    /// Represents the method that will handle mouse related routed events that do not
    /// specifically involve mouse buttons; for example, System.Windows.UIElement.MouseMove.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

#else

    /// <summary>
    /// Represents the method that will handle pointer message events such as PointerPressed.
    /// </summary>
    /// <param name="sender">The object that fired the event</param>
    /// <param name="e">The infos on the event</param>
    public delegate void PointerEventHandler(object sender, PointerRoutedEventArgs e);

#endif
}