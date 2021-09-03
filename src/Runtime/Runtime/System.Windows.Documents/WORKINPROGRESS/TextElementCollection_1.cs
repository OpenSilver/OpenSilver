using System.Collections;
using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	public partial class TextElementCollection<T> : IList, ICollection<T> where T : TextElement
	{
		internal TextElementCollection(DependencyObject owner, bool isOwnerParent) : this(owner)
		{
			
		}
	}
}
