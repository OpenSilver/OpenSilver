#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	//
	// Summary:
	//     Specifies whether data in a table should be read primarily by row or by column.
	public enum RowOrColumnMajor
	{
		//
		// Summary:
		//     Data in the table should be read row by row.
		RowMajor = 0,
		//
		// Summary:
		//     Data in the table should be read column by column.
		ColumnMajor = 1,
		//
		// Summary:
		//     The best way to present the data is indeterminate.
		Indeterminate = 2
	}
}
