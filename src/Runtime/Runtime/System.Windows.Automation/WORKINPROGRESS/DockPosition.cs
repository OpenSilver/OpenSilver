#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Contains values that specify the dock position of an object within a docking
	//     container.
	public enum DockPosition
	{
		//
		// Summary:
		//     Indicates that the UI automation element is docked along the top edge of the
		//     docking container.
		Top = 0,
		//
		// Summary:
		//     Indicates that the UI automation element is docked along the left edge of the
		//     docking container.
		Left = 1,
		//
		// Summary:
		//     Indicates that the UI automation element is docked along the bottom edge of the
		//     docking container.
		Bottom = 2,
		//
		// Summary:
		//     Indicates that the UI automation element is docked along the right edge of the
		//     docking container.
		Right = 3,
		//
		// Summary:
		//     Indicates that the UI automation element is docked along all edges of the docking
		//     container and fills all available space within the container.
		Fill = 4,
		//
		// Summary:
		//     Indicates that the UI automation element is not docked to any edge of the docking
		//     container
		None = 5
	}
}
