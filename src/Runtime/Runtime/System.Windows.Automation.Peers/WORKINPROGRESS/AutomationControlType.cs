#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
	public enum AutomationControlType
	{
		//
		// Summary:
		//     A button control.
		Button = 0,
		//
		// Summary:
		//     A calendar control, such as a date picker.
		Calendar = 1,
		//
		// Summary:
		//     A check box control.
		CheckBox = 2,
		//
		// Summary:
		//     A combo box control.
		ComboBox = 3,
		//
		// Summary:
		//     An edit control, such as a text box.
		Edit = 4,
		//
		// Summary:
		//     A hyperlink control.
		Hyperlink = 5,
		//
		// Summary:
		//     An image control.
		Image = 6,
		//
		// Summary:
		//     A list item control, which is a child item of a list control.
		ListItem = 7,
		//
		// Summary:
		//     A list control, such as a list box.
		List = 8,
		//
		// Summary:
		//     A menu control, such as a top-level menu in an application window.
		Menu = 9,
		//
		// Summary:
		//     A menu bar control, which generally contains a set of top-level menus.
		MenuBar = 10,
		//
		// Summary:
		//     A menu item control.
		MenuItem = 11,
		//
		// Summary:
		//     A progress bar control, which visually indicates the progress of a lengthy operation.
		ProgressBar = 12,
		//
		// Summary:
		//     A radio button control, which is a selection mechanism allowing exactly one selected
		//     item in a group.
		RadioButton = 13,
		//
		// Summary:
		//     A scroll bar control, such as a scroll bar in an application window.
		ScrollBar = 14,
		//
		// Summary:
		//     A slider control.
		Slider = 15,
		//
		// Summary:
		//     A spinner control.
		Spinner = 16,
		//
		// Summary:
		//     A status bar control.
		StatusBar = 17,
		//
		// Summary:
		//     A tab control.
		Tab = 18,
		//
		// Summary:
		//     A tab item control, which represents a page of a tab control.
		TabItem = 19,
		//
		// Summary:
		//     An edit control, such as a text box or rich text box.
		Text = 20,
		//
		// Summary:
		//     A toolbar, such as the control that contains a set of command buttons in an application
		//     window.
		ToolBar = 21,
		//
		// Summary:
		//     A tooltip control, an informational window that appears as a result of moving
		//     the pointer over a control or sometimes when tabbing to a control using the keyboard.
		ToolTip = 22,
		//
		// Summary:
		//     A tree control.
		Tree = 23,
		//
		// Summary:
		//     A node in a tree control.
		TreeItem = 24,
		//
		// Summary:
		//     A control that is not one of the defined control types.
		Custom = 25,
		//
		// Summary:
		//     A group control, which acts as a container for other controls.
		Group = 26,
		//
		// Summary:
		//     The control in a scrollbar that can be dragged to a different position.
		Thumb = 27,
		//
		// Summary:
		//     A data grid control.
		DataGrid = 28,
		//
		// Summary:
		//     A data item control.
		DataItem = 29,
		//
		// Summary:
		//     A document control.
		Document = 30,
		//
		// Summary:
		//     A split button, which is a button that performs a default action and can also
		//     expand to a list of other possible actions.
		SplitButton = 31,
		//
		// Summary:
		//     A window frame, which contains child objects.
		Window = 32,
		//
		// Summary:
		//     A pane control.
		Pane = 33,
		//
		// Summary:
		//     A header control, which is a container for the labels of rows and columns of
		//     information.
		Header = 34,
		//
		// Summary:
		//     A header item, which is the label for a row or column of information.
		HeaderItem = 35,
		//
		// Summary:
		//     A table.
		Table = 36,
		//
		// Summary:
		//     The caption bar on a window.
		TitleBar = 37,
		//
		// Summary:
		//     A separator, which creates a visual division in controls such as menus and toolbars.
		Separator = 38
	}
}
