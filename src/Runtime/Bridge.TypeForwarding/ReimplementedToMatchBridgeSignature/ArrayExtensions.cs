using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System
{
    public static class ArrayExtensions
    {
        public static bool Contains<T>(this T[] array, T item)
        {
            return System.Linq.Enumerable.Contains<T>(array, item);
        }

        public static bool Every<T>(this T[] array, Func<T, int, T[], bool> callback)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (!callback(array[i], i, array))
                    return false;
            }
            return true;
        }

        public static bool Every<T>(this T[] array, Func<T, bool> callback)
        {
            return System.Linq.Enumerable.All<T>(array, callback);
        }

        public static T[] Filter<T>(this T[] array, Func<T, int, T[], bool> callback)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (callback(array[i], i, array))
                    result.Add(array[i]);
            }
            return result.ToArray();
        }

        public static T[] Filter<T>(this T[] array, Func<T, bool> callback)
        {
            return System.Linq.Enumerable.ToArray<T>(System.Linq.Enumerable.Where<T>(array, callback));
        }

        public static TResult[] Map<TSource, TResult>(this TSource[] array, Func<TSource, int, TSource[], TResult> callback)
        {
            List<TResult> result = new List<TResult>();
            for (int i = 0; i < array.Length; i++)
            {
                result.Add(callback(array[i], i, array));
            }
            return result.ToArray();
        }

        public static TResult[] Map<TSource, TResult>(this TSource[] array, Func<TSource, TResult> callback)
        {
            return System.Linq.Enumerable.ToArray<TResult>(System.Linq.Enumerable.Select<TSource, TResult>(array, callback));
        }

        public static bool Some<T>(this T[] array, Func<T, int, T[], bool> callback)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (callback(array[i], i, array))
                    return true;
            }
            return false;
        }

        public static bool Some<T>(this T[] array, Func<T, bool> callback)
        {
            return System.Linq.Enumerable.Any<T>(array, callback);
        }

        public static void Push<T>(this T[] source, params T[] values)
        {
            throw new NotSupportedException();
        }

        public static void Sort<T>(this T[] array)
        {
            Array.Sort<T>(array);
        }

        public static void Sort<T>(this T[] array, Func<T, T, int> compareCallback)
        {
            Array.Sort<T>(array, new Comparison<T>(compareCallback));
        }

        public static void ForEach<T>(this T[] array, Action<T, int, T[]> callback)
        {
            for (int i = 0; i < array.Length; i++)
            {
                callback(array[i], i, array);
            }
        }

        public static void ForEach<T>(this T[] array, Action<T> callback)
        {
            Array.ForEach<T>(array, callback);
        }
    }
}