
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


#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Describes the shape that joins two lines or segments.
    /// </summary>
    public enum PenLineJoin
    {
        /// <summary>
        /// Line joins use regular angular vertices.
        /// </summary>
        Miter = 0,
        /// <summary>
        /// Line joins use beveled vertices.
        /// </summary>
        Bevel = 1,
        /// <summary>
        /// Line joins use rounded vertices.
        /// </summary>
        Round = 2,
    }
}