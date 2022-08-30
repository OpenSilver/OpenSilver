
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
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
    /// <summary>
    /// Exposes methods and properties to support UI automation client access to 
    /// child controls of containers that implement <see cref="ITableProvider" />.
    /// </summary>
    public interface ITableItemProvider : IGridItemProvider
    {
        /// <summary>
        /// Retrieves an array of UI automation providers representing all the column 
        /// headers associated with a table item or cell.
        /// </summary>
        /// <returns>
        /// An array of UI automation providers.
        /// </returns>
        IRawElementProviderSimple[] GetColumnHeaderItems();

        /// <summary>
        /// Retrieves an array of UI automation providers representing all the row 
        /// headers associated with a table item or cell.
        /// </summary>
        /// <returns>
        /// An array of UI automation providers.
        /// </returns>
        IRawElementProviderSimple[] GetRowHeaderItems();
    }
}
