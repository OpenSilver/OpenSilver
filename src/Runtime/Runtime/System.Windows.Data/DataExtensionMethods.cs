// Copyright (C) by Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace OpenSilver.Internal.Data
{
    internal static class DataExtensionMethods
    {
        // Search for value in the slice of the list starting at index with length count,
        // using the given comparer.  The list is assumed to be sorted w.r.t. the
        // comparer.  Return the index if found, or the bit-complement
        // of the index where it would belong.
        internal static int Search(this IList list, int index, int count, object value, IComparer comparer)
        {
            List<object> al;
#if false // no live shaping
            LiveShapingList lsList;
#endif // no live shaping

            if ((al = list as List<object>) != null)
            {
                return al.BinarySearch(index, count, value, comparer);
            }
#if false // no live shaping
            else if ((lsList = list as LiveShapingList) != null)
            {
                return lsList.Search(index, count, value);
            }
#endif // no live shaping

            // we should never get here, but the compiler doesn't know that
            Debug.Assert(false, "Unsupported list passed to Search");
            return 0;
        }

        // convenience method for search
        internal static int Search(this IList list, object value, IComparer comparer)
        {
            return list.Search(0, list.Count, value, comparer);
        }


        // Move an item from one position to another
        internal static void Move(this IList list, int oldIndex, int newIndex)
        {
            List<object> al;
#if false // no live shaping
            LiveShapingList lsList;
#endif // no live shaping

            if ((al = list as List<object>) != null)
            {
                object item = al[oldIndex];
                al.RemoveAt(oldIndex);
                al.Insert(newIndex, item);
            }
#if false // no live shaping
            else if ((lsList = list as LiveShapingList) != null)
            {
                lsList.Move(oldIndex, newIndex);
            }
#endif // no live shaping
        }


        // Sort the list, according to the comparer
        internal static void Sort(this IList list, IComparer comparer)
        {
            List<object> al;
#if false // no live shaping
            LiveShapingList lsList;
#endif // no live shaping

            if ((al = list as List<object>) != null)
            {
                ListHelpers.Sort(al, comparer);
            }
#if false // no live shaping
            else if ((lsList = list as LiveShapingList) != null)
            {
                lsList.Sort();
            }
#endif // no live shaping
        }
    }
}

namespace OpenSilver.Internal
{
    internal static class ListHelpers
    {
        internal static int BinarySearch<T>(this List<T> list, int index, int count, T item, IComparer comparer)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            IComparer<T> comparerT = comparer as IComparer<T>;
            if (comparerT == null)
            {
                comparerT = new ComparerHelper<T>(comparer);
            }

            return list.BinarySearch(index, count, item, comparerT);
        }

        internal static void Sort<T>(this List<T> list, IComparer comparer)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            IComparer<T> comparerT = comparer as IComparer<T>;
            if (comparerT == null)
            {
                comparerT = new ComparerHelper<T>(comparer);
            }

            list.Sort(comparerT);
        }
    }

    /// <summary>
    /// Helper class that allows to use a <see cref="IComparer"/> object
    /// as a <see cref="IComparer{T}"/>.
    /// </summary>
    internal class ComparerHelper<T> : IComparer<T>
    {
        private IComparer _comparer;

        internal ComparerHelper(IComparer comparer)
        {
            Debug.Assert(comparer != null);
            this._comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return this._comparer.Compare(x, y);
        }
    }
}
