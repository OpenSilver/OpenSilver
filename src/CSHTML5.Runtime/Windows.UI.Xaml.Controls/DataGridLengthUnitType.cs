
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
    /// Defines constants that specify how elements in a System.Windows.Controls.DataGrid
    /// are sized. Note: SizeToCells and SizeToHeader are not available yet.
    /// </summary>
    public enum DataGridLengthUnitType //todo: add the SizetoCells and SizeToHeader
    {
        /// <summary>
        /// The size is based on the contents of both the cells and the column header.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// The size is a fixed value expressed in pixels.
        /// </summary>
        Pixel = 1,
        ////
        //// Summary:
        ////     The size is based on the contents of the cells.
        //SizeToCells = 2,
        ////
        //// Summary:
        ////     The size is based on the contents of the column header.
        //SizeToHeader = 3,
        //
        // Summary:
        //     The size is a weighted proportion of available space.
        Star = 4,
    }
}