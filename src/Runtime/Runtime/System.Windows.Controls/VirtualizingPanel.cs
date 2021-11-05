using System.Windows;
using System;
using System.Collections.Specialized;

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
		private ItemContainerGenerator generator;

		public IItemContainerGenerator ItemContainerGenerator
		{
			get
			{
				if (generator == null)
				{
					ItemsControl owner = ItemsControl.GetItemsOwner(this);
					if (owner == null)
                    {
						throw new InvalidOperationException("A VirtualizingPanel is not nested in an ItemsControl. VirtualizingPanel must be nested in ItemsControl to get and show items.");
					}
					generator = owner.ItemContainerGenerator;
					generator.ItemsChanged += OnItemsChangedInternal;
				}
				return generator;
			}
		}

		protected VirtualizingPanel()
		{
			this.ClipToBounds = true;
		}

		void OnItemsChangedInternal(object sender, ItemsChangedEventArgs args)
		{
			InvalidateMeasure();
			if (args.Action == NotifyCollectionChangedAction.Reset)
			{
				Children.Clear();
				ItemContainerGenerator.RemoveAll();
				OnClearChildren();
			}

			OnItemsChanged(sender, args);
		}

		protected void AddInternalChild(UIElement @child)
		{
			Children.Add(child);
		}

		protected void InsertInternalChild(int @index, UIElement @child)
		{
			Children.Insert(index, child);
		}

		protected void RemoveInternalChildRange(int @index, int @range)
		{
			for (int i = 0; i < range; i++)
				Children.RemoveAt(index);
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
	}
}
