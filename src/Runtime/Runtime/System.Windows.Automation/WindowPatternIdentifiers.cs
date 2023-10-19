
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
    /// of the <see cref="IWindowProvider"/> pattern.
    /// </summary>
    public static class WindowPatternIdentifiers
    {
        /// <summary>
        /// Identifies the <see cref="IWindowProvider.Maximizable"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty CanMaximizeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.WindowCanMaximize,
                "WindowPatternIdentifiers.CanMaximizeProperty");

        /// <summary>
        /// Identifies the <see cref="IWindowProvider.Minimizable"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty CanMinimizeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.WindowCanMinimize,
                "WindowPatternIdentifiers.CanMinimizeProperty");

        /// <summary>
        /// Identifies the <see cref="IWindowProvider.IsModal"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsModalProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.WindowIsModal,
                "WindowPatternIdentifiers.IsModalProperty");

        /// <summary>
        /// Identifies the <see cref="IWindowProvider.VisualState"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty WindowVisualStateProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.WindowWindowVisualState,
                "WindowPatternIdentifiers.WindowVisualStateProperty");

        /// <summary>
        /// Identifies the <see cref="IWindowProvider.InteractionState"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty WindowInteractionStateProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.WindowWindowInteractionState,
                "WindowPatternIdentifiers.WindowInteractionStateProperty");

        /// <summary>
        /// Identifies the <see cref="IWindowProvider.IsTopmost"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsTopmostProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.WindowIsTopmost,
                "WindowPatternIdentifiers.IsTopmostProperty");
    }
}
