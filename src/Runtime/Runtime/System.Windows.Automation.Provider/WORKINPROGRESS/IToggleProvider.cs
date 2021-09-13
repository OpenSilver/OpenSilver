#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support UI automation client access to controls
	//     that can cycle through a set of states and maintain a particular state.
	public partial interface IToggleProvider
	{
		//
		// Summary:
		//     Gets the toggle state of the control.
		//
		// Returns:
		//     The toggle state of the control, as a value of the enumeration.
		ToggleState ToggleState
		{
			get;
		}

		//
		// Summary:
		//     Cycles through the toggle states of a control.
		void Toggle();
	}
}
