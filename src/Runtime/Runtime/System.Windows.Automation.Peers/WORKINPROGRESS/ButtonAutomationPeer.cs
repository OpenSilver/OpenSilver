using System;
using System.Windows.Controls;

#if MIGRATION
using System.Windows.Automation.Provider;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
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
    [OpenSilver.NotImplemented]
	public partial class ButtonAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Automation.Peers.ButtonAutomationPeer
		//     class.
		//
		// Parameters:
		//   owner:
		//     The element associated with this automation peer.
        [OpenSilver.NotImplemented]
		public ButtonAutomationPeer(Button owner) : base(owner)
		{
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
        [OpenSilver.NotImplemented]
		public override object GetPattern(PatternInterface patternInterface)
		{
			return null;
		}

		//
		// Summary:
		//     Gets the control type for the element that is associated with this System.Windows.Automation.Peers.ButtonAutomationPeer.
		//     This method is called by System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType.
		//
		// Returns:
		//     A value of the enumeration.
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
		protected override string GetClassNameCore()
		{
			return null;
		}

		void IInvokeProvider.Invoke()
		{
			throw new NotImplementedException();
		}
	}
}
