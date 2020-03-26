

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


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