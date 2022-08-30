
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

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Specifies whether data in a table should be read primarily by row or by column.
    /// </summary>
    public enum RowOrColumnMajor
    {
        /// <summary>
        /// Data in the table should be read row by row.
        /// </summary>
        RowMajor,
        /// <summary>
        /// Data in the table should be read column by column.
        /// </summary>
        ColumnMajor,
        /// <summary>
        /// The best way to present the data is indeterminate.
        /// </summary>
        Indeterminate,
    }
}
