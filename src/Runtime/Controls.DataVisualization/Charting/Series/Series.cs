// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>Represents a control that contains a data series.</summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract class Series : Control, ISeries, IRequireSeriesHost
    {
        /// <summary>Identifies the Title dependency property.</summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(object), typeof(Series), new PropertyMetadata(new PropertyChangedCallback(Series.OnTitleChanged)));
        /// <summary>The name of the Title property.</summary>
        protected const string TitleName = "Title";
        /// <summary>Stores the Parent instance the Series belongs to.</summary>
        private ISeriesHost _seriesHost;

        /// <summary>
        /// Gets or sets the parent instance the Series belongs to.
        /// </summary>
        public ISeriesHost SeriesHost
        {
            get
            {
                return this._seriesHost;
            }
            set
            {
                ISeriesHost seriesHost = this._seriesHost;
                this._seriesHost = value;
                if (seriesHost == this._seriesHost)
                    return;
                this.OnSeriesHostPropertyChanged(seriesHost, this._seriesHost);
            }
        }

        /// <summary>
        /// Called when the value of the SeriesHost property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new series host value.</param>
        protected virtual void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            if (newValue != null && oldValue != null)
                throw new InvalidOperationException("Series.SeriesHost: SeriesHost Property Not Null");
        }

        /// <summary>Gets the legend items to be added to the legend.</summary>
        public ObservableCollection<object> LegendItems { get; private set; }

        /// <summary>Gets or sets the title content of the Series.</summary>
        public object Title
        {
            get
            {
                return this.GetValue(Series.TitleProperty);
            }
            set
            {
                this.SetValue(Series.TitleProperty, value);
            }
        }

        /// <summary>TitleProperty property changed callback.</summary>
        /// <param name="o">Series for which the Title changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Series)o).OnTitleChanged(e.OldValue, e.NewValue);
        }

        /// <summary>Called when the Title property changes.</summary>
        /// <param name="oldValue">The old value of the Title property.</param>
        /// <param name="newValue">The new value of the Title property.</param>
        protected virtual void OnTitleChanged(object oldValue, object newValue)
        {
        }

        /// <summary>Initializes a new instance of the Series class.</summary>
        protected Series()
        {
            this.LegendItems = (ObservableCollection<object>)new NoResetObservableCollection<object>();
            this.ClipToBounds = true;
        }
    }
}