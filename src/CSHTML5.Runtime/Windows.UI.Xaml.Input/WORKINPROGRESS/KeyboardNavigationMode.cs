
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


using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if WORKINPROGRESS
    // Summary:
    //     Specifies the tabbing behavior across tab stops for a Silverlight tabbing
    //     sequence within a container.
    public enum KeyboardNavigationMode
    {
        // Summary:
        //     Tab indexes are considered on the local subtree only inside this container.
        Local = 0,
        //
        // Summary:
        //     Focus returns to the first or the last keyboard navigation stop inside of
        //     a container when the first or last keyboard navigation stop is reached.
        Cycle = 1,
        //
        // Summary:
        //     The container and all of its child elements as a whole receive focus only
        //     once.
        Once = 2,
    }
#endif
}