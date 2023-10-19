/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows
{
	//
	// Summary:
	//     Represents a collection of System.Windows.Window instances.
    [OpenSilver.NotImplemented]
	public sealed partial class WindowCollection : ICollection, IEnumerable
	{
		//
		// Summary:
		//     Gets the number of windows in the collection.
		//
		// Returns:
		//     The number of windows in the collection.
        [OpenSilver.NotImplemented]
		public int Count { get; }
		//
		// Summary:
		//     Gets a value that indicates whether access to the collection is synchronized
		//     (thread safe).
		//
		// Returns:
		//     Always returns false.
        [OpenSilver.NotImplemented]
		public bool IsSynchronized { get; }
		//
		// Summary:
		//     Gets an object that can be used to synchronize access to the System.Windows.WindowCollection.
		//
		// Returns:
		//     An object that can be used to synchronize access to the System.Windows.WindowCollection.
        [OpenSilver.NotImplemented]
		public object SyncRoot { get; }

        [OpenSilver.NotImplemented]
		public void CopyTo(Array array, int arrayIndex)
		{
			
		}

		internal WindowCollection()
		{
			Count = 0;
			IsSynchronized = false;
			SyncRoot = null;
		}

		//
		// Summary:
		//     Returns an enumerator that iterates through the collection.
		//
		// Returns:
		//     An enumerator for the collection.
        [OpenSilver.NotImplemented]
		public IEnumerator GetEnumerator()
		{
			return default(IEnumerator);
		}

		void ICollection.CopyTo(Array @array, int @index)
		{
		}
	}
}