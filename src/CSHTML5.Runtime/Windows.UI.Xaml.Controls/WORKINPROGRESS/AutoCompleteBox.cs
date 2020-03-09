#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public partial class AutoCompleteBox
	{
		
		//
		// Summary:
		//     Gets or sets a value that indicates whether the first possible match found during
		//     the filtering process will be displayed automatically in the text box.
		//
		// Returns:
		//     true if the first possible match found will be displayed automatically in the
		//     text box; otherwise, false. The default is false.
		public bool IsTextCompletionEnabled { get; set; }
	}
}

#endif