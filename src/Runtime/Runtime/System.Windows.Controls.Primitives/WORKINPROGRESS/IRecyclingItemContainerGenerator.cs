using System;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
	public partial interface IRecyclingItemContainerGenerator : IItemContainerGenerator
	{
		void Recycle(GeneratorPosition @position, int @count);
	}
}
