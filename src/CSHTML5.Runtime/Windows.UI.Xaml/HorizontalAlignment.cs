
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
    /// Describes how a child element is vertically positioned or stretched within
    /// a parent's layout slot.
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// The element is aligned to the top of the parent's layout slot.
        /// </summary>
        Left = 0,

        /// <summary>
        /// The element is aligned to the center of the parent's layout slot.
        /// </summary>
        Center = 1,

        /// <summary>
        /// The element is aligned to the right of the parent's layout slot.
        /// </summary>
        Right = 2,

        /// <summary>
        /// The element is stretched to fill the entire layout slot of the parent element.
        /// </summary>
        Stretch = 3,
    }
}