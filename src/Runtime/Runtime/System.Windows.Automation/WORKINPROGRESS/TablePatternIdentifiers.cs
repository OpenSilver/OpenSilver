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
	public static class TablePatternIdentifiers
	{
		//
		// Summary:
		//     Identifies the automation property that is accessed by the System.Windows.Automation.Provider.ITableProvider.GetColumnHeaders
		//     method.
		//
		// Returns:
		//     The automation property identifier.
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty ColumnHeadersProperty;
		//
		// Summary:
		//     Identifies the automation property that is accessed by the System.Windows.Automation.Provider.ITableProvider.GetRowHeaders
		//     method.
		//
		// Returns:
		//     The automation property identifier.
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty RowHeadersProperty;
		//
		// Summary:
		//     Identifies the System.Windows.Automation.Provider.ITableProvider.RowOrColumnMajor
		//     automation property.
		//
		// Returns:
		//     The automation property identifier.
        [OpenSilver.NotImplemented]
		public static readonly AutomationProperty RowOrColumnMajorProperty;
	}
}
