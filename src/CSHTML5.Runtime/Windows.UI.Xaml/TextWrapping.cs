
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
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Specifies whether text wraps when it reaches the edge of its container.
    /// </summary>
    public enum TextWrapping
    {
        /// <summary>
        /// No line wrapping is performed.
        /// </summary>
        NoWrap = 1,
             
        /// <summary>
        /// Line breaking occurs if a line of text overflows beyond the available width
        /// of its container. Line breaking occurs even if the standard line-breaking
        /// algorithm cannot determine any line break opportunity, such as when a line
        /// of text includes a long word that is constrained by a fixed-width container
        /// without scrolling.
        /// </summary>
        Wrap = 2,
    }
}