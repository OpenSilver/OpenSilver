
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
    /// of the <see cref="ISelectionItemProvider"/> pattern.
    /// </summary>
    public static class SelectionItemPatternIdentifiers
    {
        /// <summary>
        /// Identifies the <see cref="ISelectionItemProvider.IsSelected"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsSelectedProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.SelectionItemIsSelected,
                "SelectionItemPatternIdentifiers.IsSelectedProperty");

        /// <summary>
        /// Identifies the <see cref="ISelectionItemProvider.SelectionContainer"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty SelectionContainerProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.SelectionItemSelectionContainer,
                "SelectionItemPatternIdentifiers.SelectionContainerProperty");
    }
}
