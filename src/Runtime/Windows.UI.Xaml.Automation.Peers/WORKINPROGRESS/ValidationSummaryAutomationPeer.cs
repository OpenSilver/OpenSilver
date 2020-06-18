#if WORKINPROGRESS
#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
	//
	// Summary:
	//     Exposes System.Windows.Controls.ValidationSummary types to UI automation.
	public partial class ValidationSummaryAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Automation.Peers.ValidationSummaryAutomationPeer
		//     class.
		//
		// Parameters:
		//   owner:
		//     The System.Windows.Controls.ValidationSummary to associate with this System.Windows.Automation.Peers.ValidationSummaryAutomationPeer.
		public ValidationSummaryAutomationPeer(ValidationSummary owner): base(owner)
		{
		}

		//
		// Summary:
		//     Gets an object that supports the requested pattern, based on the patterns supported
		//     by this System.Windows.Automation.Peers.ValidationSummaryAutomationPeer.
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
			return null;
		}

		//
		// Summary:
		//     Gets the System.Windows.Automation.Peers.AutomationControlType for the element
		//     associated with this System.Windows.Automation.Peers.ValidationSummaryAutomationPeer.
		//     Called by System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType.
		//
		// Returns:
		//     A value of the enumeration.
		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Custom;
		}

		//
		// Summary:
		//     Gets the name of the class associated with this System.Windows.Automation.Peers.ValidationSummaryAutomationPeer.
		//     Called by System.Windows.Automation.Peers.AutomationPeer.GetClassName.
		//
		// Returns:
		//     The class name.
		protected override string GetClassNameCore()
		{
			return null;
		}

		//
		// Summary:
		//     Gets the UI Automation Name of the System.Windows.Controls.ValidationSummary
		//     that is associated with this System.Windows.Automation.Peers.ValidationSummaryAutomationPeer.
		//     Called by System.Windows.Automation.Peers.AutomationPeer.GetName.
		//
		// Returns:
		//     The UI Automation Name
		protected override string GetNameCore()
		{
			return null;
		}

		void IInvokeProvider.Invoke()
		{
			throw new NotImplementedException();
		}
	}
}
#endif
#endif