#if WORKINPROGRESS

using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
	public partial class PagedCollectionView
	{
		public bool CanFilter { get; }
		public bool CanGroup { get; }
		public bool CanSort { get; }
		public CultureInfo Culture { get; set; }
		public object CurrentItem { get; }
		public int CurrentPosition { get; }
		public bool IsCurrentAfterLast { get; }
		public bool IsCurrentBeforeFirst { get; }
		public bool IsEmpty { get; }
		
		public bool Contains(object item)
		{
			return default(bool);
		}

		public IDisposable DeferRefresh()
		{
			return default(IDisposable);
		}

		public bool MoveCurrentTo(object item)
		{
			return default(bool);
		}

		public bool MoveCurrentToFirst()
		{
			return default(bool);
		}

		public bool MoveCurrentToLast()
		{
			return default(bool);
		}

		public bool MoveCurrentToNext()
		{
			return default(bool);
		}

		public bool MoveCurrentToPosition(int position)
		{
			return default(bool);
		}

		public bool MoveCurrentToPrevious()
		{
			return default(bool);
		}

		public event EventHandler CurrentChanged;
		public event CurrentChangingEventHandler CurrentChanging;
		public event EventHandler<PageChangingEventArgs> PageChanging;
		public bool CanAddNew { get; }
		public bool CanCancelEdit { get; }
		public bool CanRemove { get; }
		public object CurrentAddItem { get; }
		public object CurrentEditItem { get; }
		public bool IsAddingNew { get; }
		public bool IsEditingItem { get; }
		public NewItemPlaceholderPosition NewItemPlaceholderPosition { get; set; }
		
		public object AddNew()
		{
			return default(object);
		}

		public void CancelEdit()
		{
			
		}

		public void CancelNew()
		{
			
		}

		public void CommitEdit()
		{
			
		}

		public void CommitNew()
		{
			
		}

		public void EditItem(object item)
		{
			
		}

		public void Remove(object item)
		{
			
		}

		public void RemoveAt(int idx)
		{
			
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

#endif