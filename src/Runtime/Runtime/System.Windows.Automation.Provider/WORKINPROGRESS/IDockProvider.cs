#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support access by a UI automation client to
	//     controls that expose their dock properties in a docking container.
	public interface IDockProvider
	{
		//
		// Summary:
		//     Gets the current System.Windows.Automation.DockPosition of the control in a docking
		//     container.
		//
		// Returns:
		//     The System.Windows.Automation.DockPosition of the control, relative to the boundaries
		//     of the docking container and to other elements in the container.
		DockPosition DockPosition { get; }

		//
		// Summary:
		//     Docks the control in a docking container.
		//
		// Parameters:
		//   dockPosition:
		//     The dock position, relative to the boundaries of the docking container and to
		//     other elements in the container.
		void SetDockPosition(DockPosition dockPosition);
	}
}
