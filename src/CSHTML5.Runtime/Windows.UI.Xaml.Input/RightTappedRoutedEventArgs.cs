
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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
    public sealed class RightTappedRoutedEventArgs  // Note: normally it does not inherit from "PointerRoutedEventArgs", but this lets us reuse code.
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
