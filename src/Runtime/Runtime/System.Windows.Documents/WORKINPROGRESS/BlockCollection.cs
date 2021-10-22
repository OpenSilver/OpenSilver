#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	/// <summary>
	/// Represents a collection of <see cref="Block"/> elements.
	/// </summary>
    [OpenSilver.NotImplemented]
	public sealed class BlockCollection : TextElementCollection<Block>
	{
		internal BlockCollection(DependencyObject owner, bool isOwnerParent) : base(owner)
		{
		}
	}
}
