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
    [OpenSilver.NotImplemented]
	public abstract partial class VirtualizingPanel : Panel
	{
		private IItemContainerGenerator _itemContainerGenerator;
        [OpenSilver.NotImplemented]
		public IItemContainerGenerator ItemContainerGenerator
		{
			get
			{
				return _itemContainerGenerator;
			}
		}

        [OpenSilver.NotImplemented]
		protected void AddInternalChild(UIElement @child)
		{
		}

        [OpenSilver.NotImplemented]
		protected void InsertInternalChild(int @index, UIElement @child)
		{
		}

        [OpenSilver.NotImplemented]
		protected void RemoveInternalChildRange(int @index, int @range)
		{
		}

        [OpenSilver.NotImplemented]
		protected virtual void OnItemsChanged(object @sender, ItemsChangedEventArgs @args)
		{
		}

        [OpenSilver.NotImplemented]
		protected virtual void OnClearChildren()
		{
		}

        [OpenSilver.NotImplemented]
		protected virtual void BringIndexIntoView(int @index)
		{
		}

        [OpenSilver.NotImplemented]
		protected VirtualizingPanel()
		{
			_itemContainerGenerator = null;
		}
	}
}
