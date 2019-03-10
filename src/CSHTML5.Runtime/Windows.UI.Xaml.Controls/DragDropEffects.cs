
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
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    /// <summary>
    /// Specifies the effects of a drag-and-drop operation.
    /// </summary>
    [Flags]
    public enum DragDropEffects
    {
#if unsupported
        /// <summary>
        /// Scrolling is about to start or is currently occurring in the drop target.
        /// </summary>
        Scroll = -2147483648,

        /// <summary>
        /// The data is copied, removed from the drag source, and scrolled in the drop
        /// target.
        /// </summary>
        All = -2147483645,
#endif
        /// <summary>
        /// The drop target does not accept the data.
        /// </summary>
        None = 0,
#if unsupported
        /// <summary>
        /// The data is copied to the drop target.
        /// </summary>
        Copy = 1,

        /// <summary>
        /// The data from the drag source is moved to the drop target.
        /// </summary>
        Move = 2,

        /// <summary>
        /// The data from the drag source is linked to the drop target.
        /// </summary>
        Link = 4,
#endif
    }
}