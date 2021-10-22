using System.Windows.Controls.Primitives;

#if !MIGRATION
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
	//     Represents a base class for exposing classes derived from System.Windows.Controls.Primitives.ButtonBase
	//     to UI automation.
    [OpenSilver.NotImplemented]
	public abstract partial class ButtonBaseAutomationPeer : FrameworkElementAutomationPeer
	{
		//
		// Summary:
		//     Provides initialization for base class values when called by the constructor
		//     of a derived class.
		//
		// Parameters:
		//   owner:
		//     The System.Windows.Controls.Primitives.ButtonBase to associate with this peer.
        [OpenSilver.NotImplemented]
		protected ButtonBaseAutomationPeer(ButtonBase owner): base(owner)
		{
		}

		//
		// Summary:
		//     Gets the UI Automation Name for the element associated with this System.Windows.Automation.Peers.ButtonBaseAutomationPeer.
		//     Called by System.Windows.Automation.Peers.AutomationPeer.GetName.
		//
		// Returns:
		//     The UI Automation Name for the associated element.
        [OpenSilver.NotImplemented]
		protected override string GetNameCore()
		{
			return null;
		}
	}
}
