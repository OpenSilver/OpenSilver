
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
    public static class TablePatternIdentifiers
    {
        /// <summary>
        /// Identifies the automation property that is accessed by the <see cref="ITableProvider.GetRowHeaders"/>
        /// method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty RowHeadersProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.TableRowHeaders,
                "TablePatternIdentifiers.RowHeadersProperty");

        /// <summary>
        /// Identifies the automation property that is accessed by the <see cref="ITableProvider.GetColumnHeaders"/> 
        /// method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ColumnHeadersProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.TableColumnHeaders,
                "TablePatternIdentifiers.ColumnHeadersProperty");

        /// <summary>
        /// Identifies the <see cref="ITableProvider.RowOrColumnMajor"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty RowOrColumnMajorProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.TableRowOrColumnMajor,
                "TablePatternIdentifiers.RowOrColumnMajorProperty");
    }
}
