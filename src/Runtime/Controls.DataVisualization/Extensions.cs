using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>A set of extension methods for the Grid container.</summary>
    internal static class GridExtensions
    {
        /// <summary>Mirrors the grid either horizontally or vertically.</summary>
        /// <param name="grid">The grid to mirror.</param>
        /// <param name="orientation">The orientation to mirror the grid along.</param>
        public static void Mirror(this Grid grid, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                IList<RowDefinition> list = (IList<RowDefinition>)grid.RowDefinitions.Reverse<RowDefinition>().ToList<RowDefinition>();
                grid.RowDefinitions.Clear();
                foreach (FrameworkElement element in grid.Children.OfType<FrameworkElement>())
                    Grid.SetRow(element, list.Count - 1 - Grid.GetRow(element));
                foreach (RowDefinition rowDefinition in (IEnumerable<RowDefinition>)list)
                    grid.RowDefinitions.Add(rowDefinition);
            }
            else
            {
                if (orientation != Orientation.Vertical)
                    return;
                IList<ColumnDefinition> list = (IList<ColumnDefinition>)grid.ColumnDefinitions.Reverse<ColumnDefinition>().ToList<ColumnDefinition>();
                grid.ColumnDefinitions.Clear();
                foreach (FrameworkElement element in grid.Children.OfType<FrameworkElement>())
                    Grid.SetColumn(element, list.Count - 1 - Grid.GetColumn(element));
                foreach (ColumnDefinition columnDefinition in (IEnumerable<ColumnDefinition>)list)
                    grid.ColumnDefinitions.Add(columnDefinition);
            }
        }
    }

    /// <summary>
    /// Collection of functions that manipulate streams of ranges.
    /// </summary>
    internal static class RangeEnumerableExtensions
    {
        /// <summary>Returns the minimum and maximum values in a stream.</summary>
        /// <typeparam name="T">The type of the stream.</typeparam>
        /// <param name="that">The stream.</param>
        /// <returns>The range of values in the stream.</returns>
        public static Range<T> GetRange<T>(this IEnumerable<T> that) where T : IComparable
        {
            IEnumerator<T> enumerator = that.GetEnumerator();
            if (!enumerator.MoveNext())
                return new Range<T>();
            T minimum = enumerator.Current;
            T maximum = minimum;
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (ValueHelper.Compare((IComparable)minimum, (IComparable)current) == 1)
                    minimum = current;
                if (ValueHelper.Compare((IComparable)maximum, (IComparable)current) == -1)
                    maximum = current;
            }
            return new Range<T>(minimum, maximum);
        }

        /// <summary>Returns a range encompassing all ranges in a stream.</summary>
        /// <typeparam name="T">The type of the minimum and maximum values.</typeparam>
        /// <param name="that">The stream.</param>
        /// <returns>A range encompassing all ranges in a stream.</returns>
        public static Range<T> Sum<T>(this IEnumerable<Range<T>> that) where T : IComparable
        {
            Range<T> range = new Range<T>();
            IEnumerator<Range<T>> enumerator = that.GetEnumerator();
            while (enumerator.MoveNext())
                range = range.Add(enumerator.Current);
            return range;
        }
    }
}