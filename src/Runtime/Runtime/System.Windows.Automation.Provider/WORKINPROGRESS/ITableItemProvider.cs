#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support UI automation client access to child
	//     controls of containers that implement System.Windows.Automation.Provider.ITableProvider.
	public partial interface ITableItemProvider : IGridItemProvider
	{
		//
		// Summary:
		//     Retrieves an array of UI automation providers representing all the column headers
		//     associated with a table item or cell.
		//
		// Returns:
		//     An array of UI automation providers.
		IRawElementProviderSimple[] GetColumnHeaderItems();
		//
		// Summary:
		//     Retrieves an array of UI automation providers representing all the row headers
		//     associated with a table item or cell.
		//
		// Returns:
		//     An array of UI automation providers.
		IRawElementProviderSimple[] GetRowHeaderItems();
	}
}
