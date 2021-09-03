#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
	//
	// Summary:
	//     Specifies the event that is raised by the element through the associated System.Windows.Automation.Peers.AutomationPeer.
	public enum AutomationEvents
	{
		//
		// Summary:
		//     The event that is raised when a tooltip is opened.
		ToolTipOpened = 0,
		//
		// Summary:
		//     The event that is raised when a tooltip is closed.
		ToolTipClosed = 1,
		//
		// Summary:
		//     The event that is raised when a menu is opened. .
		MenuOpened = 2,
		//
		// Summary:
		//     The event that is raised when a menu is closed.
		MenuClosed = 3,
		//
		// Summary:
		//     The event that is raised when the focus has changed. See System.Windows.Automation.Peers.AutomationPeer.SetFocus.
		AutomationFocusChanged = 4,
		//
		// Summary:
		//     The event that is raised when a control is activated. See System.Windows.Automation.Provider.IInvokeProvider.Invoke.
		InvokePatternOnInvoked = 5,
		//
		// Summary:
		//     The event that is raised when an item is added to a collection of selected items.
		//     See System.Windows.Automation.Provider.ISelectionItemProvider.AddToSelection.
		SelectionItemPatternOnElementAddedToSelection = 6,
		//
		// Summary:
		//     The event that is raised when an item is removed from a collection of selected
		//     items. See System.Windows.Automation.Provider.ISelectionItemProvider.RemoveFromSelection.
		SelectionItemPatternOnElementRemovedFromSelection = 7,
		//
		// Summary:
		//     The event that is raised when a single item is selected (which clears any previous
		//     selection). See System.Windows.Automation.Provider.ISelectionItemProvider.Select.
		SelectionItemPatternOnElementSelected = 8,
		//
		// Summary:
		//     The event that is raised when a selection in a container has changed significantly.
		SelectionPatternOnInvalidated = 9,
		//
		// Summary:
		//     The event that is raised when the text selection is modified.
		TextPatternOnTextSelectionChanged = 10,
		//
		// Summary:
		//     The event that is raised when textual content is modified.
		TextPatternOnTextChanged = 11,
		//
		// Summary:
		//     The event that is raised when content is loaded asynchronously.
		AsyncContentLoaded = 12,
		//
		// Summary:
		//     The event that is raised when a property has changed.
		PropertyChanged = 13,
		//
		// Summary:
		//     The event that is raised when the UI Automation tree structure is changed.
		StructureChanged = 14
	}
}
