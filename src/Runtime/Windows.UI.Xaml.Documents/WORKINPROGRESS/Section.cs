#if WORKINPROGRESS

using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	[ContentProperty("Blocks")]
	public sealed partial class Section : Block
	{
		public BlockCollection Blocks { get; private set; }
	}
}

#endif