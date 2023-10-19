
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
    /// controls that act as containers for a collection of child elements. The children 
    /// of this element must implement <see cref="ITableItemProvider" /> and be organized 
    /// in a two-dimensional logical coordinate system that can be traversed (a UI automation 
    /// client can move to adjacent controls, which are headers or cells of the table) by 
    /// using the keyboard.
    /// </summary>
    public interface ITableProvider : IGridProvider
    {
        /// <summary>
        /// Returns a collection of UI Automation providers that represents all the column 
        /// headers in a table.
        /// </summary>
        /// <returns>
        /// An array of UI automation providers.
        /// </returns>
        IRawElementProviderSimple[] GetColumnHeaders();

        /// <summary>
        /// Returns a collection of UI Automation providers that represents all row headers 
        /// in the table.
        /// </summary>
        /// <returns>
        /// An array of UI automation providers.
        /// </returns>
        IRawElementProviderSimple[] GetRowHeaders();

        /// <summary>
        /// Gets the primary direction of traversal for the table.
        /// </summary>
        /// <returns>
        /// The primary direction of traversal, as a value of the enumeration.
        /// </returns>
        RowOrColumnMajor RowOrColumnMajor { get; }
    }
}
