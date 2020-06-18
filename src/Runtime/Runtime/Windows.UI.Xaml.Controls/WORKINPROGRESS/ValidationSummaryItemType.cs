#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	//
	// Summary:
	//     Specifies whether a validation error came from object-level or property-level
	//     validation.
	public enum ValidationSummaryItemType
	{
		//
		// Summary:
		//     The error came from object-level validation.
		ObjectError = 1,
		//
		// Summary:
		//     The error came from property-level validation.
		PropertyError = 2
	}
}
#endif