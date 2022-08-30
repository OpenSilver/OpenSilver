
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
    /// of the <see cref="ISelectionProvider"/> pattern.
    /// </summary>
    public static class SelectionPatternIdentifiers
    {
        /// <summary>
        /// Identifies the property that gets the selected items in a container.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty SelectionProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.SelectionSelection,
                "SelectionPatternIdentifiers.SelectionProperty");

        /// <summary>
        /// Identifies the <see cref="ISelectionProvider.CanSelectMultiple"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty CanSelectMultipleProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.SelectionCanSelectMultiple,
                "SelectionPatternIdentifiers.CanSelectMultipleProperty");

        /// <summary>
        /// Identifies the <see cref="ISelectionProvider.IsSelectionRequired"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsSelectionRequiredProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.SelectionIsSelectionRequired,
                "SelectionPatternIdentifiers.IsSelectionRequiredProperty");
    }
}
