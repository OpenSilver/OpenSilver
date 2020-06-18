#if WORKINPROGRESS
using System.Windows;
using System;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public abstract partial class VirtualizingPanel : Panel
	{
		private IItemContainerGenerator _itemContainerGenerator;
		public IItemContainerGenerator ItemContainerGenerator
		{
			get
			{
				return _itemContainerGenerator;
			}
		}

		protected void AddInternalChild(UIElement @child)
		{
		}

		protected void InsertInternalChild(int @index, UIElement @child)
		{
		}

		protected void RemoveInternalChildRange(int @index, int @range)
		{
		}

		protected virtual void OnItemsChanged(object @sender, ItemsChangedEventArgs @args)
		{
		}

		protected virtual void OnClearChildren()
		{
		}

		protected virtual void BringIndexIntoView(int @index)
		{
		}

		protected VirtualizingPanel()
		{
			_itemContainerGenerator = null;
		}
	}
}
#endif