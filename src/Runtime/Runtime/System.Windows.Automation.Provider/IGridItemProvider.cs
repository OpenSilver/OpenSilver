
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

using System.Windows.Automation.Peers;

namespace System.Windows.Automation.Provider
{
    /// <summary>
    /// Exposes methods and properties to support access by a UI automation client to 
    /// individual child controls of containers that implement <see cref="IGridProvider" />.
    /// </summary>
    public interface IGridItemProvider
    {
        /// <summary>
        /// Gets the ordinal number of the column that contains the cell or item.
        /// </summary>
        /// <returns>
        /// A zero-based ordinal number that identifies the column that contains the cell or item.
        /// </returns>
        int Column { get; }

        /// <summary>
        /// Gets the number of columns that are spanned by a cell or item.
        /// </summary>
        /// <returns>
        /// The number of columns.
        /// </returns>
        int ColumnSpan { get; }

        /// <summary>
        /// Gets a UI automation provider that implements <see cref="IGridProvider" /> and 
        /// that represents the container of the cell or item.
        /// </summary>
        /// <returns>
        /// A UI automation provider that implements the <see cref="PatternInterface.Grid" /> 
        /// control pattern and that represents the cell or item container.
        /// </returns>
        IRawElementProviderSimple ContainingGrid { get; }

        /// <summary>
        /// Gets the ordinal number of the row that contains the cell or item.
        /// </summary>
        /// <returns>
        /// A zero-based ordinal number that identifies the row that contains the cell or item.
        /// </returns>
        int Row { get; }

        /// <summary>
        /// Gets the number of rows spanned by a cell or item.
        /// </summary>
        /// <returns>
        /// The number of rows.
        /// </returns>
        int RowSpan { get; }
    }
}
