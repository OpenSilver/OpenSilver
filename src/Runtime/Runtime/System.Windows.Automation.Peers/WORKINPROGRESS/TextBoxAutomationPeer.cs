using System;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
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
	//     Exposes System.Windows.Controls.TextBox types to UI automation.
    [OpenSilver.NotImplemented]
	public partial class TextBoxAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Automation.Peers.TextBoxAutomationPeer
		//     class.
		//
		// Parameters:
		//   owner:
		//     The System.Windows.Controls.TextBox that is associated with this System.Windows.Automation.Peers.TextBoxAutomationPeer.
        [OpenSilver.NotImplemented]
		public TextBoxAutomationPeer(TextBox owner): base(owner)
		{
		}

		bool IValueProvider.IsReadOnly => throw new NotImplementedException();
		string IValueProvider.Value => throw new NotImplementedException();
		//
		// Summary:
		//     Gets an object that supports the requested pattern, based on the patterns supported
		//     by this System.Windows.Automation.Peers.TextBoxAutomationPeer.
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
		//     Gets the control type for the control that is associated with this System.Windows.Automation.Peers.TextBoxAutomationPeer.
		//     This method is called by System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType.
		//
		// Returns:
		//     A value of the enumeration.
        [OpenSilver.NotImplemented]
		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Text;
		}

		//
		// Summary:
		//     Gets the name of the class that is associated with this System.Windows.Automation.Peers.TextBoxAutomationPeer.
		//     This method is called by System.Windows.Automation.Peers.AutomationPeer.GetClassName.
		//
		// Returns:
		//     The class name.
        [OpenSilver.NotImplemented]
		protected override string GetClassNameCore()
		{
			return null;
		}

		//
		// Summary:
		//     Gets the UI Automation Name value from the element that is associated with this
		//     System.Windows.Automation.Peers.TextBoxAutomationPeer. Called by System.Windows.Automation.Peers.AutomationPeer.GetName.
		//
		// Returns:
		//     The UI Automation Name of the element that is associated with this automation
		//     peer.
        [OpenSilver.NotImplemented]
		protected override string GetNameCore()
		{
			return null;
		}

		void IValueProvider.SetValue(string value)
		{
			throw new NotImplementedException();
		}
	}
}
