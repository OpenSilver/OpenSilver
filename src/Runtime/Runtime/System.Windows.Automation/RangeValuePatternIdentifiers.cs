
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
    /// of the <see cref="IRangeValueProvider"/> pattern.
    /// </summary>
    public static class RangeValuePatternIdentifiers
    {
        /// <summary>
        /// Identifies the <see cref="IRangeValueProvider.Value"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ValueProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.RangeValueValue,
                "RangeValuePatternIdentifiers.ValueProperty");

        /// <summary>
        /// Identifies the <see cref="IRangeValueProvider.IsReadOnly"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsReadOnlyProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.RangeValueIsReadOnly,
                "RangeValuePatternIdentifiers.IsReadOnlyProperty");

        /// <summary>
        /// Identifies the <see cref="IRangeValueProvider.Minimum"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty MinimumProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.RangeValueMinimum,
                "RangeValuePatternIdentifiers.MinimumProperty");

        /// <summary>
        /// Identifies the <see cref="IRangeValueProvider.Maximum"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty MaximumProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.RangeValueMaximum,
                "RangeValuePatternIdentifiers.MaximumProperty");

        /// <summary>
        /// Identifies the <see cref="IRangeValueProvider.LargeChange"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty LargeChangeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.RangeValueLargeChange,
                "RangeValuePatternIdentifiers.LargeChangeProperty");

        /// <summary>
        /// Identifies the <see cref="IRangeValueProvider.SmallChange"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty SmallChangeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.RangeValueSmallChange,
                "RangeValuePatternIdentifiers.SmallChangeProperty");
    }
}
