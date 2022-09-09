
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
using System.Windows.Automation.Provider;
#else
using Windows.UI.Xaml.Automation.Provider;
#endif

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Contains values used as automation property identifiers specifically for properties
    /// of the <see cref="ITableProvider"/> pattern.
    /// </summary>
    public static class TableItemPatternIdentifiers
    {
        /// <summary>
        /// Identifies the automation property that retrieves all the row headers associated
        /// with a table item or cell.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty RowHeaderItemsProperty =
             AutomationProperty.Register(
                 AutomationIdentifierConstants.Properties.TableItemRowHeaderItems,
                 "TableItemPatternIdentifiers.RowHeaderItemsProperty");

        /// <summary>
        /// Identifies the automation property that retrieves all the column headers associated
        /// with a table item or cell.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ColumnHeaderItemsProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.TableItemColumnHeaderItems,
                "TableItemPatternIdentifiers.ColumnHeaderItemsProperty");
    }
}
