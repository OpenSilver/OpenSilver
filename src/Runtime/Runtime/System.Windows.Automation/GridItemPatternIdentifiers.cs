
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

using System.Windows.Automation.Provider;

namespace System.Windows.Automation
{
    /// <summary>
    /// Contains values used as automation property identifiers specifically for properties
    /// of the <see cref="IGridItemProvider"/> pattern.
    /// </summary>
    public static class GridItemPatternIdentifiers
    {
        /// <summary>
        /// Identifies the <see cref="IGridItemProvider.Column"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ColumnProperty =
             AutomationProperty.Register(
                 AutomationIdentifierConstants.Properties.GridItemColumn,
                 "GridItemPatternIdentifiers.ColumnProperty");

        /// <summary>
        /// Identifies the <see cref="IGridItemProvider.ColumnSpan"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ColumnSpanProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.GridItemColumnSpan,
                "GridItemPatternIdentifiers.ColumnSpanProperty");

        /// <summary>
        /// Identifies the <see cref="IGridItemProvider.ContainingGrid"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ContainingGridProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.GridItemContainingGrid,
                "GridItemPatternIdentifiers.ContainingGridProperty");

        /// <summary>
        /// Identifies the <see cref="IGridItemProvider.Row"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty RowProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.GridItemRow,
                "GridItemPatternIdentifiers.RowProperty");

        /// <summary>
        /// Identifies the <see cref="IGridItemProvider.RowSpan"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty RowSpanProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.GridItemRowSpan,
                "GridItemPatternIdentifiers.RowSpanProperty");
    }
}
