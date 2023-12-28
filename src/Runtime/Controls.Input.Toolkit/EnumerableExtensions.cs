// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;

namespace System.Windows.Controls
{
    /// <summary>
    /// Extension methods for Enumerable.
    /// </summary>
    internal static partial class EnumerableExtensions
    {
        /// <summary>
        /// Creates a sequence from an integer, to an integer, skipping 
        /// a defined amount of integers.
        /// </summary>
        /// <param name="from">The value of the first integer in the sequence.</param>
        /// <param name="to">The inclusive end of the sequence.</param>
        /// <param name="by">The amount of integers to skip.</param>
        /// <returns>A sequence of integers.</returns>
        public static IEnumerable<int> Range(int from, int to, int by)
        {
            if (by <= 0)
            {
                // todo: move to resource
                throw new ArgumentOutOfRangeException("by", "Parameter by is expected to be larger than 0");
            }

            if (from < to)
            {
                for (int i = from; i <= to; i += by)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = from; i >= to; i -= by)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Applies an action to each element in an IEnumerable.
        /// </summary>
        /// <typeparam name="T">The Type of the elements.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="action">The action applied to all the elements.</param>
        /// <remarks>See a discussion about the merits on this function here:
        /// http://blogs.msdn.com/ericlippert/archive/2009/05/18/foreach-vs-foreach.aspx.</remarks>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                return;
            }

            foreach (T i in source)
            {
                action(i);
            }
        }
    }
}