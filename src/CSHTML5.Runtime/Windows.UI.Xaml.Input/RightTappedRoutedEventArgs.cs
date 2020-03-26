

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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Provides event data for the RightTapped event.
    /// </summary>
    public sealed partial class RightTappedRoutedEventArgs  // Note: normally it does not inherit from "PointerRoutedEventArgs", but this lets us reuse code.
#if MIGRATION
        : MouseEventArgs
#else
        : PointerRoutedEventArgs
#endif
    {
        ///// <summary>
        ///// Initializes a new instance of the RightTappedRoutedEventArgs class.
        ///// </summary>
        //public RightTappedRoutedEventArgs()
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
