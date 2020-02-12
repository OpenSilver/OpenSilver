
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



using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
#else
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Provides event data for the Tapped event.
    /// </summary>
    public sealed partial class TappedRoutedEventArgs // Note: normally it does not inherit from "PointerRoutedEventArgs", but this lets us reuse code.
#if MIGRATION
        : MouseEventArgs
#else
        : PointerRoutedEventArgs
#endif
    {
        ///// <summary>
        ///// Initializes a new instance of the TappedRoutedEventArgs class.
        ///// </summary>
        //public TappedRoutedEventArgs()
        //{

        //}

        //
        // Summary:
        //     Gets the PointerDeviceType for the pointer device that initiated the associated
        //     input event.
        //
        // Returns:
        //     The PointerDeviceType for this event occurrence.
        //public PointerDeviceType PointerDeviceType { get; }
    }
}
