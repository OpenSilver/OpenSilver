using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if WORKINPROGRESS
	//
	// Summary:
	//     Represents a composition related to text input which includes the composition
	//     text itself.
	public sealed partial class TextComposition
	{
		//
		// Summary:
		//     Gets the composition text for this text composition.
		//
		// Returns:
		//     The composition text for this text composition.
		public string CompositionText { get; }
	}
#endif
}
