#if WORKINPROGRESS
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
	public abstract partial class CollectionViewGroup : INotifyPropertyChanged
	{
		private object _name;
		private ReadOnlyObservableCollection<object> _items;
		public object Name
		{
			get
			{
				return _name;
			}
		}

		public ReadOnlyObservableCollection<object> Items
		{
			get
			{
				return _items;
			}
		}

		public int ItemCount
		{
			get;
			private set;
		}

		protected int ProtectedItemCount
		{
			get;
			set;
		}

		protected ObservableCollection<object> ProtectedItems
		{
			get;
			private set;
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
			}

			remove
			{
			}
		}

		protected virtual event PropertyChangedEventHandler PropertyChanged;
		protected CollectionViewGroup(object @name)
		{
			_name = @name;
			_items = null;
		}

		public abstract bool IsBottomLevel
		{
			get;
		}
	}
}
#endif