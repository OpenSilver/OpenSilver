#if WORKINPROGRESS
using System.Windows.Controls;
using System;
using System.Windows;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
	public partial interface IItemContainerGenerator
	{
		ItemContainerGenerator GetItemContainerGeneratorForPanel(Panel @panel);
		IDisposable StartAt(GeneratorPosition @position, GeneratorDirection @direction, bool @allowStartAtRealizedItem);
		DependencyObject GenerateNext(out bool @isNewlyRealized);
		void PrepareItemContainer(DependencyObject @container);
		void RemoveAll();
		void Remove(GeneratorPosition @position, int @count);
		GeneratorPosition GeneratorPositionFromIndex(int @itemIndex);
		int IndexFromGeneratorPosition(GeneratorPosition @position);
	}
}
#endif