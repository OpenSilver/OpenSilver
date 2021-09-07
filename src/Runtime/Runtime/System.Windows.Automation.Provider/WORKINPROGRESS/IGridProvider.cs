#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	public partial interface IGridProvider
	{
		//
		// Summary:
		//     Gets the total number of columns in a grid.
		//
		// Returns:
		//     The total number of columns in a grid.
		int ColumnCount
		{
			get;
		}

		//
		// Summary:
		//     Gets the total number of rows in a grid.
		//
		// Returns:
		//     The total number of rows in a grid.
		int RowCount
		{
			get;
		}

		//
		// Summary:
		//     Retrieves the UI automation provider for the specified cell.
		//
		// Parameters:
		//   row:
		//     The ordinal number of the row that contains the cell.
		//
		//   column:
		//     The ordinal number of the column that contains the cell.
		//
		// Returns:
		//     The UI automation provider for the specified cell.
		IRawElementProviderSimple GetItem(int row, int column);
	}
}
