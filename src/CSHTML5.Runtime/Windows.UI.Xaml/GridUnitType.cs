
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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Describes the kind of value that a Windows.UI.Xaml.GridLength
    /// object is holding.
    /// </summary>
    public enum GridUnitType
    {
        /// <summary>
        /// [SECURITY CRITICAL] The size is determined by the size properties of the
        /// content object.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// [SECURITY CRITICAL] The value is expressed in pixels.
        /// </summary>
        Pixel = 1,
        /// <summary>
        /// [SECURITY CRITICAL] The value is expressed as a weighted proportion of available
        /// </summary>
        Star = 2,
    }
}