
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

namespace System.Windows.Automation.Provider
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI automation client to 
    /// controls that act as containers for a collection of child elements. 
    /// </summary>
    public interface IGridProvider
    {
        /// <summary>
        /// Gets the total number of columns in a grid.
        /// </summary>
        /// <returns>
        /// The total number of columns in a grid.
        /// </returns>
        int ColumnCount { get; }

        /// <summary>
        /// Retrieves the UI automation provider for the specified cell.
        /// </summary>
        /// <returns>
        /// The UI automation provider for the specified cell.
        /// </returns>
        /// <param name="row">
        /// The ordinal number of the row that contains the cell.
        /// </param>
        /// <param name="column">
        /// The ordinal number of the column that contains the cell.
        /// </param>
        IRawElementProviderSimple GetItem(int row, int column);

        /// <summary>
        /// Gets the total number of rows in a grid.
        /// </summary>
        /// <returns>
        /// The total number of rows in a grid.
        /// </returns>
        int RowCount { get; }
    }
}
