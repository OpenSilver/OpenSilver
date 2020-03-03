#if WORKINPROGRESS

using System.Windows.Automation.Peers;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public abstract partial class UIElement : DependencyObject
	{
		//
		// Summary:
		//     When implemented in a derived class, returns class-specific System.Windows.Automation.Peers.AutomationPeer
		//     implementations for the Silverlight automation infrastructure.
		//
		// Returns:
		//     The class-specific System.Windows.Automation.Peers.AutomationPeer subclass to
		//     return.
		protected virtual AutomationPeer OnCreateAutomationPeer()
		{
			return default(AutomationPeer);
		}
	}
}
#endif