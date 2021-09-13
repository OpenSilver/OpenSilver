#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Contains values that specify the System.Windows.Automation.ToggleState of a UI
	//     automation element
	public enum ToggleState
	{
		//
		// Summary:
		//     The UI automation element is not selected, checked, marked, or otherwise activated.
		Off = 0,
		//
		// Summary:
		//     The UI automation element is selected, checked, marked, or otherwise activated.
		On = 1,
		//
		// Summary:
		//     The UI automation element is in an indeterminate state.
		Indeterminate = 2
	}
}
