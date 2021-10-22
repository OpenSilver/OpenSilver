#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
	public enum AutomationOrientation
	{
		//
		// Summary:
		//     The control does not have an orientation.
		None = 0,
		//
		// Summary:
		//     The control is presented horizontally.
		Horizontal = 1,
		//
		// Summary:
		//     The control is presented horizontally.
		Vertical = 2
	}
}
