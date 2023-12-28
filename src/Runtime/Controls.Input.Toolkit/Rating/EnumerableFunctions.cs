// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Windows.Controls
{
    /// <summary>
    /// This class contains general purpose functions to manipulate the generic
    /// IEnumerable type.
    /// </summary>
    internal static class EnumerableFunctions
    {
        /////// <summary>
        /////// Applies a function to an accumulated value and an item in the 
        /////// sequence and yields the result as the accumulated value.
        /////// </summary>
        /////// <typeparam name="T">The type of the sequence.</typeparam>
        /////// <param name="that">The sequence to scan.</param>
        /////// <param name="func">The function applied to the accumulator and the
        /////// current item.</param>
        /////// <returns>A sequence of computed values.</returns>
        ////public static IEnumerable<T> Scan<T>(this IEnumerable<T> that, Func<T, T, T> func)
        ////{
        ////    IEnumerator<T> enumerator = that.GetEnumerator();
        ////    if (!enumerator.MoveNext())
        ////    {
        ////        yield break;
        ////    }

        ////    T acc = enumerator.Current;
        ////    yield return acc;

        ////    while (enumerator.MoveNext())
        ////    {
        ////        acc = func(acc, enumerator.Current);
        ////        yield return acc;
        ////    }
        ////}

        /// <summary>
        /// Applies a function to an accumulated value and an item in the 
        /// sequence and yields the result as the accumulated value.
        /// </summary>
        /// <typeparam name="T">The type of the input sequence.</typeparam>
        /// <typeparam name="R">The type of the initial value.</typeparam>
        /// <param name="that">The sequence to scan.</param>
        /// <param name="func">The function applied to the accumulator and the
        /// current item.</param>
        /// <param name="initialValue">The initial value in the output sequence.
        /// </param>
        /// <returns>A sequence of computed values.</returns>
        public static IEnumerable<R> Scan<T, R>(this IEnumerable<T> that, Func<R, T, R> func, R initialValue)
        {
            R acc = initialValue;
            yield return acc;

            IEnumerator<T> enumerator = that.GetEnumerator();
            while (enumerator.MoveNext())
            {
                acc = func(acc, enumerator.Current);
                yield return acc;
            }
        }

        /// <summary>
        /// Accepts two sequences and applies a function to the corresponding 
        /// values in the two sequences.
        /// </summary>
        /// <typeparam name="T0">The type of the first sequence.</typeparam>
        /// <typeparam name="T1">The type of the second sequence.</typeparam>
        /// <typeparam name="R">The return type of the function.</typeparam>
        /// <param name="enumerable0">The first sequence.</param>
        /// <param name="enumerable1">The second sequence.</param>
        /// <param name="func">The function to apply to the corresponding values
        /// from the two sequences.</param>
        /// <returns>A sequence of transformed values from both sequences.</returns>
        public static IEnumerable<R> Zip<T0, T1, R>(IEnumerable<T0> enumerable0, IEnumerable<T1> enumerable1, Func<T0, T1, R> func)
        {
            IEnumerator<T0> enumerator0 = enumerable0.GetEnumerator();
            IEnumerator<T1> enumerator1 = enumerable1.GetEnumerator();
            while (enumerator0.MoveNext() && enumerator1.MoveNext())
            {
                yield return func(enumerator0.Current, enumerator1.Current);
            }
        }

        /// <summary>
        /// Returns the index of an item in a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the sequence.</typeparam>
        /// <param name="that">The sequence.</param>
        /// <param name="item">The item in the sequence.</param>
        /// <returns>The index of an item in a sequence.</returns>
        public static int? IndexOf<T>(this IEnumerable<T> that, T item)
        {
            IEnumerator<T> enumerator = that.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                if (object.ReferenceEquals(enumerator.Current, item))
                {
                    return index;
                }
                index++;
            }
            return null;
        }

        /// <summary>
        /// Returns a stream of weighted values based on a percentage.
        /// </summary>
        /// <param name="values">A sequence of values.</param>
        /// <param name="percent">The percentage of values.</param>
        /// <returns>A sequence of percentages.</returns>
        public static IEnumerable<double> GetWeightedValues(this IEnumerable<double> values, double percent)
        {
            double total = values.Sum();
            if (total == 0)
            {
                return values.Select(_ => 0.0);
            }

            return
                EnumerableFunctions
                    .Zip(
                        values.Scan((acc, current) => acc + current, 0.0),
                        values,
                        (acc, current) => Tuple.Create(acc, current))
                    .Select(tuple => Tuple.Create(tuple.Item1 / total, tuple.Item2 / total))
                    .Select(tuple =>
                    {
                        double accumulated = tuple.Item1;
                        double current = tuple.Item2;

                        if (percent > accumulated && accumulated + current > percent)
                        {
                            return (percent - accumulated) * total;
                        }
                        else if (percent <= accumulated)
                        {
                            return 0.0;
                        }
                        else
                        {
                            return 1.0;
                        }
                    });
        }
    }
}
