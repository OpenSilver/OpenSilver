
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    //
    // Summary:
    //     Exposes System.Windows.Controls.Button types to UI automation.
    public class ButtonAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Automation.Peers.ButtonAutomationPeer
        //     class.
        //
        // Parameters:
        //   owner:
        //     The element associated with this automation peer.
        public ButtonAutomationPeer(Button owner) : base(owner)
        {
        }

        void IInvokeProvider.Invoke()
        {
            if (!IsEnabled()) throw new ElementNotEnabledException();

            var buttonBase = (ButtonBase)Owner;

            if (Dispatcher.CheckAccess())
                buttonBase.AutomationButtonBaseClick();
            else
                Dispatcher.BeginInvoke(() => buttonBase.AutomationButtonBaseClick());
        }

        //
        // Summary:
        //     Gets an object that supports the specified pattern, based on the patterns supported
        //     by this System.Windows.Automation.Peers.ButtonAutomationPeer.
        //
        // Parameters:
        //   patternInterface:
        //     One of the enumeration values.
        //
        // Returns:
        //     The object that implements the pattern interface, or null if the specified pattern
        //     interface is not implemented by this peer.
        public override object GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.Invoke ? this : null;
        }

        //
        // Summary:
        //     Gets the control type for the element that is associated with this System.Windows.Automation.Peers.ButtonAutomationPeer.
        //     This method is called by System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType.
        //
        // Returns:
        //     A value of the enumeration.
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        //
        // Summary:
        //     Gets the name of the class that is associated with this UI automation peer.
        //
        // Returns:
        //     The class name.
        protected override string GetClassNameCore()
        {
            return "Button";
        }
    }
}