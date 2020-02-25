﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
	//
	// Summary:
	//     Represents a collection of System.Windows.Window instances.
	public sealed partial class WindowCollection : ICollection, IEnumerable
	{
		//
		// Summary:
		//     Gets the number of windows in the collection.
		//
		// Returns:
		//     The number of windows in the collection.
		public int Count { get; }
		//
		// Summary:
		//     Gets a value that indicates whether access to the collection is synchronized
		//     (thread safe).
		//
		// Returns:
		//     Always returns false.
		public bool IsSynchronized { get; }
		//
		// Summary:
		//     Gets an object that can be used to synchronize access to the System.Windows.WindowCollection.
		//
		// Returns:
		//     An object that can be used to synchronize access to the System.Windows.WindowCollection.
		public object SyncRoot { get; }

		public void CopyTo(Array array, int arrayIndex)
		{
			
		}

		//
		// Summary:
		//     Returns an enumerator that iterates through the collection.
		//
		// Returns:
		//     An enumerator for the collection.
		public IEnumerator GetEnumerator()
		{
			return default(IEnumerator);
		}
	}
#endif
}