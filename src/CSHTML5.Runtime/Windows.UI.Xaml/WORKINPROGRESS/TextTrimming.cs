
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
#if WORKINPROGRESS
    /// <summary>
    /// This property determines how text is trimmed when it overflows the edge of its
    /// container.
    /// </summary>
    public enum TextTrimming
    {
        /// <summary>
        /// Default no trimming
        /// </summary>
        None = 0,

        /// <summary>
        /// Text is trimmed at a character boundary.
        /// </summary>
        CharacterEllipsis = 1,

        /// <summary>
        /// Text is trimmed at word boundary.
        /// </summary>
        WordEllipsis = 2,

        /// <summary>
        /// Text is trimmed at a pixel level, visually clipping the excess glyphs.
        /// </summary>
        Clip = 3,
    }
#endif
}
