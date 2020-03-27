#if WORKINPROGRESS
#if MIGRATION

namespace System.Windows.Controls
{
	//
	// Summary:
	//     Specifies the types of errors displayed in an System.Windows.Controls.ValidationSummary
	//     control.
	[Flags]
	public enum ValidationSummaryFilters
	{
		//
		// Summary:
		//     No errors are displayed.
		None = 0,
		//
		// Summary:
		//     Only object level errors are displayed.
		ObjectErrors = 1,
		//
		// Summary:
		//     Only property level errors are displayed.
		PropertyErrors = 2,
		//
		// Summary:
		//     All errors are displayed.
		All = 3
	}
}
#endif
#endif