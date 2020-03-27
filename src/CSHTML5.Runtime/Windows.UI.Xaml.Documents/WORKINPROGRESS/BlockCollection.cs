#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	//
	// Summary:
	//     Represents a collection of System.Windows.Documents.Block elements.
	public sealed class BlockCollection : TextElementCollection<Block>
	{
		internal BlockCollection(DependencyObject owner, bool isOwnerParent)
			: base(owner, isOwnerParent)
		{
		}
	}
}

#endif