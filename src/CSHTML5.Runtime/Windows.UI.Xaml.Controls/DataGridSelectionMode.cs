
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
    /// Defines constants that specify whether single or multiple item selections
    /// are supported by a System.Windows.Controls.DataGrid control.
    /// </summary>
    public enum DataGridSelectionMode
    {
        /// <summary>
        /// Only one item in the System.Windows.Controls.DataGrid can be selected at
        /// a time.
        /// </summary>
        Single = 0,

        /// <summary>
        /// Multiple items in the System.Windows.Controls.DataGrid can be selected at
        /// the same time.
        /// </summary>
        Extended = 1,
    }
}