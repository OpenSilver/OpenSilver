
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines constants that specify scrolling behavior for ScrollViewer and other
    /// parts involved in scrolling scenarios.
    /// </summary>
    public enum ScrollMode
    {
        /// <summary>
        /// Scrolling is disabled.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Scrolling is enabled.
        /// </summary>
        Enabled = 1,
        /// <summary>
        /// Scrolling is enabled but behavior uses a "rails" manipulation mode.
        /// </summary>
        Auto = 2,
    }
}