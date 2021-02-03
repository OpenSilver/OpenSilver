#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	/// <summary>
	/// An abstract class that provides a base for all block-level content elements.
	/// </summary>
	public abstract partial class Block : TextElement
	{
	}
}
#endif