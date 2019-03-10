
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Describes the direction that content is scaled.
    /// </summary>
    public enum StretchDirection
    {
        /// <summary>
        /// The content scales upward only when it is smaller than the parent. If the
        /// content is larger, no scaling downward is performed.
        /// </summary>
        UpOnly = 0,

        /// <summary>
        /// The content scales downward only when it is larger than the parent. If the
        /// content is smaller, no scaling upward is performed.
        /// </summary>
        DownOnly = 1,

        /// <summary>
        /// The content stretches to fit the parent according to the Stretch property.
        /// </summary>
        Both = 2,
    }
}
