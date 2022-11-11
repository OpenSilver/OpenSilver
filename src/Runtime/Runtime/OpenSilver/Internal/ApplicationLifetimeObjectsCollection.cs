//
// ApplicationLifetimeObjectsCollection class
//
// Contact:
//   Moonlight List (moonlight-list@lists.ximian.com)
//
// Copyright (C) 2010 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal
{
	internal sealed class ApplicationLifetimeObjectsCollection : IList
	{
		private readonly List<object> list = new List<object>();
		private bool read_only;

		public object this[int index]
		{
			get
			{
				return list[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public bool IsReadOnly
		{
			get { return read_only; }
		}

		public int Add(object value)
		{
			if (!(value is IApplicationService))
            {
				throw new NotSupportedException("Element declared as Application Service does not implement IApplicationService interface.");
            }

			if (IsReadOnly)
            {
				throw new NotSupportedException();
			}

			// we can add only before application startup (i.e. parsing time)
			list.Add(value);
			return Count - 1;
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(object value)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(object value)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public void Remove(object value)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		internal void Close()
		{
			read_only = true;
		}
	}
}
