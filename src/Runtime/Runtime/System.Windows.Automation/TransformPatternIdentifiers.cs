
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
    /// of the <see cref="ITransformProvider"/> pattern.
    /// </summary>
    public static class TransformPatternIdentifiers
    {
        /// <summary>
        /// Identifies the <see cref="ITransformProvider.CanMove"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty CanMoveProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.TransformCanMove,
                "TransformPatternIdentifiers.CanMoveProperty");

        /// <summary>
        /// Identifies the <see cref="ITransformProvider.CanResize"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty CanResizeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.TransformCanResize,
                "TransformPatternIdentifiers.CanResizeProperty");

        /// <summary>
        /// Identifies the <see cref="ITransformProvider.CanRotate"/> automation property.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty CanRotateProperty =
             AutomationProperty.Register(
                 AutomationIdentifierConstants.Properties.TransformCanRotate,
                 "TransformPatternIdentifiers.CanRotateProperty");
    }
}
