
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
    /// Specifies whether text is centered, left-aligned, or right-aligned.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// Text is centered within the container.
        /// </summary>
        Center = 0,
        
        /// <summary>
        /// Text is aligned to the left edge of the container.
        /// </summary>
        Left = 1,
        
        /// <summary>
        /// Text is aligned to the right edge of the container.
        /// </summary>
        Right = 2,
        
        /// <summary>
        /// Text is justified within the container.
        /// </summary>
        Justify = 3,
    }
}