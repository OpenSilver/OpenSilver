#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Contains values that specify the visual state of a window for the System.Windows.Automation.Provider.IWindowProvider
	//     pattern.
	public enum WindowVisualState
	{
		//
		// Summary:
		//     Specifies that the window is normal (restored).
		Normal = 0,
		//
		// Summary:
		//     Specifies that the window is maximized.
		Maximized = 1,
		//
		// Summary:
		//     Specifies that the window is minimized.
		Minimized = 2
	}
}
