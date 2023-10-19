
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
    /// of the <see cref="IValueProvider"/> pattern.
    /// </summary>
    public static class ValuePatternIdentifiers
    {
        /// <summary>
        /// Identifies the <see cref="IValueProvider.Value"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ValueProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ValueValue,
                "ValuePatternIdentifiers.ValueProperty");

        /// <summary>
        /// Identifies the <see cref="IValueProvider.IsReadOnly"/> property.
        /// </summary>
        public static readonly AutomationProperty IsReadOnlyProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ValueIsReadOnly,
                "ValuePatternIdentifiers.IsReadOnlyProperty");
    }
}
