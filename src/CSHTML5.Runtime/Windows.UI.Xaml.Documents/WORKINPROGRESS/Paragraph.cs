#if WORKINPROGRESS

using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	//
	// Summary:
	//     Provides a block-level content element that is used to group content into a paragraph.
	[ContentProperty("Inlines")]
	public sealed class Paragraph : Block
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Documents.Paragraph class.
		public Paragraph()
		{
			
		}

		//
		// Summary:
		//     Gets an System.Windows.Documents.InlineCollection containing the top-level System.Windows.Documents.Inline
		//     elements that include the contents of the System.Windows.Documents.Paragraph.
		//
		// Returns:
		//     An System.Windows.Documents.InlineCollection containing the System.Windows.Documents.Inline
		//     elements that include the contents of the System.Windows.Documents.Paragraph.
		public InlineCollection Inlines { get; }
	}
}

#endif