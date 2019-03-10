
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
    /// Describes the shape at the end of a line or segment.
    /// </summary>
    public enum PenLineCap
    {
        /// <summary>
        /// A cap that does not extend past the last point of the line. Comparable to
        /// no line cap.
        /// </summary>
        Flat = 0,
        /// <summary>
        /// A rectangle that has a height equal to the line thickness and a length equal
        /// to half the line thickness.
        /// </summary>
        Square = 1,
        /// <summary>
        /// A semicircle that has a diameter equal to the line thickness.
        /// </summary>
        Round = 2,
#if WORKINPROGRESS
        /// <summary>
        ///     Triangle - Triangle line cap.
        /// </summary>
        Triangle = 3,
#endif
    }
}