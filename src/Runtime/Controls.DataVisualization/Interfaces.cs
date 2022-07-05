using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.DataVisualization
{
    public interface IResourceDictionaryDispenser
    {
        /// <summary>
        /// Returns a rotating enumerator of ResourceDictionaries coordinated with
        /// the style dispenser object to ensure that no two enumerators are
        /// currently on the same one if possible.  If the dispenser is reset or
        /// its collection is changed then the enumerators will also be reset.
        /// </summary>
        /// <param name="predicate">A predicate that returns a value
        /// indicating whether to return a ResourceDictionary.</param>
        /// <returns>An enumerator of ResourceDictionaries.</returns>
        IEnumerator<ResourceDictionary> GetResourceDictionariesWhere(Func<ResourceDictionary, bool> predicate);

        /// <summary>
        /// Event that is invoked when the StyleDispenser's ResourceDictionaries have changed.
        /// </summary>
        event EventHandler ResourceDictionariesChanged;
    }

    public interface IRequireSeriesHost
    {
        /// <summary>Gets or sets the series host.</summary>
        ISeriesHost SeriesHost { get; set; }
    }

    public interface ISeriesHost : IRequireSeriesHost, IResourceDictionaryDispenser
    {
        /// <summary>
        /// Gets the collection of axes the series host has available.
        /// </summary>
        ObservableCollection<IAxis> Axes { get; }

        /// <summary>
        /// Gets the collection of series the series host has available.
        /// </summary>
        ObservableCollection<ISeries> Series { get; }

        /// <summary>Gets the foreground elements.</summary>
        ObservableCollection<UIElement> ForegroundElements { get; }

        /// <summary>Gets the background elements.</summary>
        ObservableCollection<UIElement> BackgroundElements { get; }
    }
}
