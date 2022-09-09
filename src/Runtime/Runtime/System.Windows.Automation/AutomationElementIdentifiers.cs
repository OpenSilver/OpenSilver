
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
using System.Windows.Automation.Peers;
#else
using Windows.UI.Xaml.Automation.Peers;
#endif

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Contains values used as automation property identifiers by UI automation providers
    /// and UI automation clients.
    /// </summary>
    public static class AutomationElementIdentifiers
    {
        /// <summary>
        /// Identifies the accelerator key automation property. The accelerator key property
        /// value is returned by the <see cref="AutomationPeer.GetAcceleratorKey"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty AcceleratorKeyProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.AcceleratorKey,
                "AutomationElementIdentifiers.AcceleratorKeyProperty");


        /// <summary>
        /// Identifies the access key automation property. The access key property value
        /// is returned by the <see cref="AutomationPeer.GetAccessKey"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty AccessKeyProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.AccessKey,
                "AutomationElementIdentifiers.AccessKeyProperty");

        /// <summary>
        /// Identifies the automation element identifier automation property. The automation
        /// element identifier value is returned by the <see cref="AutomationPeer.GetAutomationId"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty AutomationIdProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.AutomationId,
                "AutomationElementIdentifiers.AutomationIdProperty");

        /// <summary>
        /// Identifies the bounding rectangle automation property. The bounding rectangle
        /// property value is returned by the <see cref="AutomationPeer.GetBoundingRectangle"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty BoundingRectangleProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.BoundingRectangle,
                "AutomationElementIdentifiers.BoundingRectangleProperty");

        /// <summary>
        /// Identifies the class name automation property. The class name property value
        /// is returned by the <see cref="AutomationPeer.GetClassName"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ClassNameProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ClassName,
                "AutomationElementIdentifiers.ClassNameProperty");

        /// <summary>
        /// Identifies the clickable point automation property. A valid clickable point property
        /// value is returned by the <see cref="AutomationPeer.GetClickablePoint"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ClickablePointProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ClickablePoint,
                "AutomationElementIdentifiers.ClickablePointProperty");

        /// <summary>
        /// Identifies the control type automation property. The control type property value
        /// is returned by the <see cref="AutomationPeer.GetAutomationControlType"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ControlTypeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ControlType,
                "AutomationElementIdentifiers.ControlTypeProperty");

        /// <summary>
        /// Identifies the keyboard focus automation property. The keyboard focus state is
        /// returned by the <see cref="AutomationPeer.HasKeyboardFocus"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty HasKeyboardFocusProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.HasKeyboardFocus,
                "AutomationElementIdentifiers.HasKeyboardFocusProperty");

        /// <summary>
        /// Identifies the help text automation property. The help text property value is
        /// returned by the <see cref="AutomationPeer.GetHelpText"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty HelpTextProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.HelpText,
                "AutomationElementIdentifiers.HelpTextProperty");

        /// <summary>
        /// Identifies the content element determination automation property. The content
        /// element status indicates whether the element contains content that is valuable
        /// to the end user. The current status is returned by the <see cref="AutomationPeer.IsContentElement"/>
        /// method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsContentElementProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.IsContentElement,
                "AutomationElementIdentifiers.IsContentElementProperty");

        /// <summary>
        /// Identifies the control element determination automation property. The control
        /// element status indicates whether the element contains user interface components
        /// that can be manipulated. The current status is returned by the <see cref="AutomationPeer.IsControlElement"/>
        /// method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsControlElementProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.IsControlElement,
                "AutomationElementIdentifiers.IsControlElementProperty");

        /// <summary>
        /// Identifies the enabled determination automation property. The enabled status
        /// indicates whether the item referenced by the automation peer is enabled. The
        /// current status is returned by the <see cref="AutomationPeer.IsEnabled"/>
        /// method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsEnabledProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.IsEnabled,
                "AutomationElementIdentifiers.IsEnabledProperty");

        /// <summary>
        /// Identifies the keyboard-focusable determination automation property. The keyboard
        /// focusable status is returned by the <see cref="AutomationPeer.IsKeyboardFocusable"/>
        /// method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsKeyboardFocusableProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.IsKeyboardFocusable,
                "AutomationElementIdentifiers.IsKeyboardFocusableProperty");

        /// <summary>
        /// Identifies the offscreen determination automation property. The offscreen status
        /// indicates whether the item referenced by the automation peer is off the screen.
        /// The current status is returned by the <see cref="AutomationPeer.IsOffscreen"/>
        /// method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsOffscreenProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.IsOffscreen,
                "AutomationElementIdentifiers.IsOffscreenProperty");

        /// <summary>
        /// Identifies the password determination automation property. The password status
        /// indicates whether the item referenced by the automation peer contains a password.
        /// The current status is returned by the <see cref="AutomationPeer.IsPassword"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsPasswordProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.IsPassword,
                "AutomationElementIdentifiers.IsPasswordProperty");

        /// <summary>
        /// Identifies the form requirement determination automation property. The form requirement
        /// status indicates whether the element must be completed on a form. The current
        /// status is returned by the <see cref="AutomationPeer.IsRequiredForForm"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty IsRequiredForFormProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.IsRequiredForForm,
                "AutomationElementIdentifiers.IsRequiredForFormProperty");

        /// <summary>
        /// Identifies the item status automation property. The current item status is returned
        /// by the <see cref="AutomationPeer.GetItemStatus" /> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ItemStatusProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ItemStatus,
                "AutomationElementIdentifiers.ItemStatusProperty");

        /// <summary>
        /// Identifies the item type automation property. The item type value is returned
        /// by <see cref="AutomationPeer.GetItemType"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty ItemTypeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.ItemType,
                "AutomationElementIdentifiers.ItemTypeProperty");

        /// <summary>
        /// Identifies the labeled-by peer automation property. The current label peer is
        /// returned by the <see cref="AutomationPeer.GetLabeledBy"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty LabeledByProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.LabeledBy,
                "AutomationElementIdentifiers.LabeledByProperty");

        /// <summary>
        /// Identifies the localized control type automation property. The current localized
        /// control type is returned by the <see cref="AutomationPeer.GetLocalizedControlTypeCore"/>
        /// method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty LocalizedControlTypeProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.LocalizedControlType,
                "AutomationElementIdentifiers.LocalizedControlTypeProperty");

        /// <summary>
        /// Identifies the element name automation property. The current name is returned
        /// by the <see cref="AutomationPeer.GetName"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty NameProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.Name,
                "AutomationElementIdentifiers.NameProperty");

        /// <summary>
        /// Identifies the orientation automation property. The current orientation value
        /// is returned by the <see cref="AutomationPeer.GetOrientation"/> method.
        /// </summary>
        /// <returns>
        /// The automation property identifier.
        /// </returns>
        public static readonly AutomationProperty OrientationProperty =
            AutomationProperty.Register(
                AutomationIdentifierConstants.Properties.Orientation,
                "AutomationElementIdentifiers.OrientationProperty");
    }
}
