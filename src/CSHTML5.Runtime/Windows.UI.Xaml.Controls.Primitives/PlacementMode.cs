
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
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Specifies the preferred location for positioning a ToolTip relative to a visual element.
    /// </summary>
    public enum PlacementMode
    {
        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the bottom of the target element.
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the right of the target element.
        /// </summary>
        Right = 4,

        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the mouse pointer location.
        /// </summary>
        Mouse = 7,

        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the left of the target element.
        /// </summary>
        Left = 9,

        /// <summary>
        /// Indicates that the preferred location of the tooltip is at the top of the target element.
        /// </summary>
        Top = 10,
    }
}
