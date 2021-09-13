#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Contains values used as automation property identifiers specifically for properties
	//     of the System.Windows.Automation.Provider.ITableProvider pattern.
    [OpenSilver.NotImplemented]
	public static class TableItemPatternIdentifiers
	{
		//
		// Summary:
		//     Identifies the automation property that retrieves all the column headers associated
		//     with a table item or cell.
		//
		// Returns:
		//     The automation property identifier.
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty ColumnHeaderItemsProperty;
		//
		// Summary:
		//     Identifies the automation property that retrieves all the row headers associated
		//     with a table item or cell.
		//
		// Returns:
		//     The automation property identifier.
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty RowHeaderItemsProperty;
	}
}
