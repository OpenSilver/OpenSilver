#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
	//
	// Summary:
	//     Specifies the control pattern that the System.Windows.Automation.Peers.AutomationPeer.GetPattern(System.Windows.Automation.Peers.PatternInterface)
	//     method returns.
	public enum PatternInterface
	{
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IInvokeProvider control pattern interface.
		Invoke = 0,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.ISelectionProvider control pattern interface.
		Selection = 1,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IValueProvider control pattern interface.
		Value = 2,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IRangeValueProvider control pattern interface.
		RangeValue = 3,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IScrollProvider control pattern interface.
		Scroll = 4,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IScrollItemProvider control pattern interface.
		ScrollItem = 5,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IExpandCollapseProvider control pattern
		//     interface.
		ExpandCollapse = 6,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IGridProvider control pattern interface.
		Grid = 7,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IGridItemProvider control pattern interface.
		GridItem = 8,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IMultipleViewProvider control pattern
		//     interface.
		MultipleView = 9,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IWindowProvider control pattern interface.
		Window = 10,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.ISelectionItemProvider control pattern
		//     interface.
		SelectionItem = 11,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IDockProvider control pattern interface.
		Dock = 12,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.ITableProvider control pattern interface.
		Table = 13,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.ITableItemProvider control pattern interface.
		TableItem = 14,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.IToggleProvider control pattern interface.
		Toggle = 15,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.ITransformProvider control pattern interface.
		Transform = 16,
		//
		// Summary:
		//     The System.Windows.Automation.Provider.ITextProvider control pattern interface.
		Text = 17
	}
}
