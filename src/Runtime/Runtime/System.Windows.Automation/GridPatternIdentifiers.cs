
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
    /// of the <see cref="IGridProvider"/> pattern.
    /// </summary>
    public static class GridPatternIdentifiers
    {
        /// <summary>
        /// Identifies the <see cref="IGridProvider.ColumnCount"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ColumnCountProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.GridColumnCount,
                "GridPatternIdentifiers.ColumnCountProperty");

        /// <summary>
        /// Identifies the <see cref="IGridProvider.RowCount"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty RowCountProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.GridRowCount,
                "GridPatternIdentifiers.RowCountProperty");
    }
}
