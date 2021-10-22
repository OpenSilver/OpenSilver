#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support access by a UI automation client to
	//     individual child controls of containers that implement System.Windows.Automation.Provider.IGridProvider.
	public partial interface IGridItemProvider
	{
		//
		// Summary:
		//     Gets the ordinal number of the column that contains the cell or item.
		//
		// Returns:
		//     A zero-based ordinal number that identifies the column that contains the cell
		//     or item.
		int Column
		{
			get;
		}

		//
		// Summary:
		//     Gets the number of columns that are spanned by a cell or item.
		//
		// Returns:
		//     The number of columns.
		int ColumnSpan
		{
			get;
		}

		//
		// Summary:
		//     Gets a UI automation provider that implements System.Windows.Automation.Provider.IGridProvider
		//     and that represents the container of the cell or item.
		//
		// Returns:
		//     A UI automation provider that implements the System.Windows.Automation.Peers.PatternInterface.Grid
		//     control pattern and that represents the cell or item container.
		IRawElementProviderSimple ContainingGrid
		{
			get;
		}

		//
		// Summary:
		//     Gets the ordinal number of the row that contains the cell or item.
		//
		// Returns:
		//     A zero-based ordinal number that identifies the row that contains the cell or
		//     item.
		int Row
		{
			get;
		}

		//
		// Summary:
		//     Gets the number of rows spanned by a cell or item.
		//
		// Returns:
		//     The number of rows.
		int RowSpan
		{
			get;
		}
	}
}
