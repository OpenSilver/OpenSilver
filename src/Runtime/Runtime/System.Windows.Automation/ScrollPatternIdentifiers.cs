
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
    /// of the <see cref="IScrollProvider"/> pattern, as well as the <see cref="NoScroll"/> 
    /// constant.
    /// </summary>
    public static class ScrollPatternIdentifiers
    {
        /// <summary>
        /// Specifies that scrolling should not be performed.
        /// </summary>
        public const double NoScroll = -1.0;

        /// <summary>
        /// Identifies the <see cref="IScrollProvider.HorizontalScrollPercent"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty HorizontalScrollPercentProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ScrollHorizontalScrollPercent,
                "ScrollPatternIdentifiers.HorizontalScrollPercentProperty");

        /// <summary>
        /// Identifies the <see cref="IScrollProvider.HorizontalViewSize"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty HorizontalViewSizeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ScrollHorizontalViewSize,
                "ScrollPatternIdentifiers.HorizontalViewSizeProperty");

        /// <summary>
        /// Identifies the <see cref="IScrollProvider.VerticalScrollPercent"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty VerticalScrollPercentProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ScrollVerticalScrollPercent,
                "ScrollPatternIdentifiers.VerticalScrollPercentProperty");

        /// <summary>
        /// Identifies the <see cref="IScrollProvider.VerticalViewSize"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty VerticalViewSizeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ScrollVerticalViewSize,
                "ScrollPatternIdentifiers.VerticalViewSizeProperty");

        /// <summary>
        /// Identifies the <see cref="IScrollProvider.HorizontallyScrollable"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty HorizontallyScrollableProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ScrollHorizontallyScrollable,
                "ScrollPatternIdentifiers.HorizontallyScrollableProperty");

        /// <summary>
        /// Identifies the <see cref="IScrollProvider.VerticallyScrollable"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty VerticallyScrollableProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ScrollVerticallyScrollable,
                "ScrollPatternIdentifiers.VerticallyScrollableProperty");
    }
}
