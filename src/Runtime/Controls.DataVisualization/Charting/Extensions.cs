using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>Extension methods for series hosts.</summary>
    internal static class ISeriesHostExtensions
    {
        /// <summary>
        /// Gets all series that track their global indexes recursively.
        /// </summary>
        /// <param name="rootSeriesHost">The root series host.</param>
        /// <returns>A sequence of series.</returns>
        public static IEnumerable<ISeries> GetDescendentSeries(this ISeriesHost rootSeriesHost)
        {
            Queue<ISeries> series = new Queue<ISeries>((IEnumerable<ISeries>)rootSeriesHost.Series);
            while (series.Count != 0)
            {
                ISeries currentSeries = series.Dequeue();
                yield return currentSeries;
                ISeriesHost seriesHost = currentSeries as ISeriesHost;
                if (seriesHost != null)
                {
                    foreach (ISeries series1 in (Collection<ISeries>)seriesHost.Series)
                        series.Enqueue(series1);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether an axis is in use by the series
        /// host.
        /// </summary>
        /// <param name="that">The series host.</param>
        /// <param name="axis">The axis that may or may not be used by a
        /// series.</param>
        /// <returns>A value indicating whether an axis is in use by the series
        /// host.</returns>
        public static bool IsUsedByASeries(this ISeriesHost that, IAxis axis)
        {
            return axis.RegisteredListeners.OfType<ISeries>().Intersect<ISeries>((IEnumerable<ISeries>)that.Series).Any<ISeries>();
        }
    }

    /// <summary>A set of extension methods for the DataPoint class.</summary>
    internal static class FrameworkElementExtensions
    {
        /// <summary>
        /// Returns the actual margin for a given framework element and axis.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="axis">The axis along which to return the margin.</param>
        /// <returns>The margin for a given framework element and axis.</returns>
        public static double GetActualMargin(this FrameworkElement element, IAxis axis)
        {
            double num = 0.0;
            if (axis.Orientation == AxisOrientation.X)
                num = element.ActualWidth;
            else if (axis.Orientation == AxisOrientation.Y)
                num = element.ActualHeight;
            return num / 2.0;
        }

        /// <summary>
        /// Returns the margin for a given framework element and axis.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="axis">The axis along which to return the margin.</param>
        /// <returns>The margin for a given framework element and axis.</returns>
        public static double GetMargin(this FrameworkElement element, IAxis axis)
        {
            double num = 0.0;
            if (axis.Orientation == AxisOrientation.X)
                num = !double.IsNaN(element.Width) ? element.Width : element.ActualWidth;
            else if (axis.Orientation == AxisOrientation.Y)
                num = !double.IsNaN(element.Height) ? element.Height : element.ActualHeight;
            return num / 2.0;
        }
    }

    /// <summary>Extension methods for the ResourceDictionary class.</summary>
    public static class ResourceDictionaryExtensions
    {
        /// <summary>
        /// Makes a shallow copy of the specified ResourceDictionary.
        /// </summary>
        /// <param name="dictionary">ResourceDictionary to copy.</param>
        /// <returns>Copied ResourceDictionary.</returns>
        public static ResourceDictionary ShallowCopy(this ResourceDictionary dictionary)
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            foreach (object key in (IEnumerable)dictionary.Keys)
                resourceDictionary.Add(key, dictionary[key]);
            return resourceDictionary;
        }
    }
}
