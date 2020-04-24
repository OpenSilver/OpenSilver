#if WORKINPROGRESS
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public partial class DependencyObjectCollection<T> : DependencyObject, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged
	{
		private T _item;
		private int _count;
		private bool _isReadOnly;
		private bool _isFixedSize;
		private bool _isReadOnly1;
		private object _item2;
		private int _count3;
		private bool _isSynchronized;
		private object _syncRoot;
		public T this[int index]
		{
			get
			{
				return _item;
			}

			set
			{
				_item = value;
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return _isFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return _isReadOnly1;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return _item2;
			}

			set
			{
				_item2 = value;
			}
		}

		int ICollection.Count
		{
			get
			{
				return _count3;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return _isSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return _syncRoot;
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public DependencyObjectCollection()
		{
			_item = default(T);
			_count = 0;
			_isReadOnly = false;
			_isFixedSize = false;
			_isReadOnly1 = false;
			_item2 = null;
			_count3 = 0;
			_isSynchronized = false;
			_syncRoot = null;
		}

		public int IndexOf(T @item)
		{
			return 0;
		}

		public void Insert(int @index, T @item)
		{
		}

		public void RemoveAt(int @index)
		{
		}

		public void Add(T @item)
		{
		}

		public void Clear()
		{
		}

		public bool Contains(T @item)
		{
			return false;
		}

		public void CopyTo(T[] @array, int @arrayIndex)
		{
		}

		public bool Remove(T @item)
		{
			return false;
		}

		int IList.Add(object @value)
		{
			return 0;
		}

		void IList.Clear()
		{
		}

		bool IList.Contains(object @value)
		{
			return false;
		}

		int IList.IndexOf(object @value)
		{
			return 0;
		}

		void IList.Insert(int @index, object @value)
		{
		}

		void IList.Remove(object @value)
		{
		}

		void IList.RemoveAt(int @index)
		{
		}

		void ICollection.CopyTo(Array @array, int @index)
		{
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new List<T>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
#endif