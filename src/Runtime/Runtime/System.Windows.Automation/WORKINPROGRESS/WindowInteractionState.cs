#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Defines values that specify the current state of the window for purposes of user
	//     or programmatic interaction.
	public enum WindowInteractionState
	{
		//
		// Summary:
		//     The window is running. This does not guarantee that the window is responding
		//     or ready for user interaction.
		Running = 0,
		//
		// Summary:
		//     The window is closing.
		Closing = 1,
		//
		// Summary:
		//     The window is ready for user interaction.
		ReadyForUserInteraction = 2,
		//
		// Summary:
		//     The window is blocked by a modal window.
		BlockedByModalWindow = 3,
		//
		// Summary:
		//     The window is not responding.
		NotResponding = 4
	}
}
