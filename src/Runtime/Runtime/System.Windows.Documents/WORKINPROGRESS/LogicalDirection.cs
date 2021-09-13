#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	//
	// Summary:
	//     Specifies a logical direction in which to perform certain text operations, such
	//     as inserting, retrieving, or navigating through text relative to a specified
	//     position (a System.Windows.Documents.TextPointer).
	public enum LogicalDirection
	{
		//
		// Summary:
		//     Backward, or from right to left.
		Backward = 0,
		//
		// Summary:
		//     Forward, or from left to right.
		Forward = 1
	}
}
