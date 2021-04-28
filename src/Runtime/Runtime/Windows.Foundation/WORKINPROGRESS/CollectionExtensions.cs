#if WORKINPROGRESS

#if !MIGRATION
using System;
#endif

#if MIGRATION
using System.Collections;
using System.Collections.Generic;

namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
	public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        public static void AddRange(this IList collection, IEnumerable items)
        {
            foreach (object item in items)
            {
                collection.Add(item);
            }
        }
    }
}
#endif