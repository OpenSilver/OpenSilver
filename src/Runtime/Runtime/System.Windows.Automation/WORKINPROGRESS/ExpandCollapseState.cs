#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Contains values that specify the System.Windows.Automation.Provider.IExpandCollapseProvider.ExpandCollapseState
	//     automation property value of a UI automation element.
	public enum ExpandCollapseState
	{
		//
		// Summary:
		//     No child nodes, controls, or content of the UI automation element are displayed.
		Collapsed = 0,
		//
		// Summary:
		//     All child nodes, controls or content of the UI automation element are displayed.
		Expanded = 1,
		//
		// Summary:
		//     Some, but not all, child nodes, controls, or content of the UI automation element
		//     are displayed.
		PartiallyExpanded = 2,
		//
		// Summary:
		//     The UI automation element has no child nodes, controls, or content to display.
		LeafNode = 3
	}
}
