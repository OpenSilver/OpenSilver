using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>An axes collection used by a series host.</summary>
    internal class SeriesHostAxesCollection : UniqueObservableCollection<IAxis>
    {
        /// <summary>Gets or sets the series host field.</summary>
        private ISeriesHost SeriesHost { get; set; }

        /// <summary>
        /// Gets or sets a collection of axes cannot be removed under any
        /// circumstances.
        /// </summary>
        private UniqueObservableCollection<IAxis> PersistentAxes { get; set; }

        /// <summary>
        /// Instantiates a new instance of the SeriesHostAxesCollection class.
        /// </summary>
        /// <param name="seriesHost">The series host.</param>
        internal SeriesHostAxesCollection(ISeriesHost seriesHost)
        {
            this.SeriesHost = seriesHost;
            this.PersistentAxes = new UniqueObservableCollection<IAxis>();
            this.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ThisCollectionChanged);
        }

        /// <summary>
        /// Instantiates a new instance of the SeriesHostAxesCollection class.
        /// </summary>
        /// <param name="seriesHost">The series host.</param>
        /// <param name="persistentAxes">A collection of axes that can never be
        /// removed from the chart.</param>
        internal SeriesHostAxesCollection(ISeriesHost seriesHost, UniqueObservableCollection<IAxis> persistentAxes)
          : this(seriesHost)
        {
            System.Diagnostics.Debug.Assert(persistentAxes != null, "Persistent axes collection cannot be null.");
            this.SeriesHost = seriesHost;
            this.PersistentAxes = persistentAxes;
            this.PersistentAxes.CollectionChanged += new NotifyCollectionChangedEventHandler(this.PersistentAxesCollectionChanged);
        }

        /// <summary>
        /// A method that attaches and removes listeners to axes added to this
        /// collection.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">Information about the event.</param>
        private void ThisCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IAxis newItem in (IEnumerable)e.NewItems)
                    newItem.RegisteredListeners.CollectionChanged += new NotifyCollectionChangedEventHandler(this.AxisRegisteredListenersCollectionChanged);
            }
            if (e.OldItems == null)
                return;
            foreach (IAxis oldItem in (IEnumerable)e.OldItems)
                oldItem.RegisteredListeners.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.AxisRegisteredListenersCollectionChanged);
        }

        /// <summary>
        /// Remove an axis from the collection if it is no longer used.
        /// </summary>
        /// <param name="sender">The axis that has had its registered
        /// listeners collection changed.</param>
        /// <param name="e">Information about the event.</param>
        private void AxisRegisteredListenersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IAxis axis = this.Where<IAxis>((Func<IAxis, bool>)(currentAxis => currentAxis.RegisteredListeners == sender)).First<IAxis>();
            if (e.OldItems == null || (this.PersistentAxes.Contains(axis) || this.SeriesHost.IsUsedByASeries(axis)))
                return;
            this.Remove(axis);
        }

        /// <summary>
        /// This method synchronizes the collection with the persistent axes
        /// collection when it is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        public void PersistentAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IAxis newItem in (IEnumerable)e.NewItems)
                {
                    if (!this.Contains(newItem))
                        this.Add(newItem);
                }
            }
            if (e.OldItems == null)
                return;
            foreach (IAxis oldItem in (IEnumerable)e.OldItems)
            {
                if (this.Contains(oldItem) && !this.SeriesHost.IsUsedByASeries(oldItem))
                    this.Remove(oldItem);
            }
        }

        /// <summary>
        /// Removes an item from the axes collection but throws an exception
        /// if a series in the series host is listening to it.
        /// </summary>
        /// <param name="index">The index of the item being removed.</param>
        protected override void RemoveItem(int index)
        {
            IAxis axis = this[index];
            if (this.SeriesHost.IsUsedByASeries(axis))
                throw new InvalidOperationException("SeriesHostAxesCollection: Axis Cannot Be Removed From A SeriesHost When One Or More Series Are Listening To It");
            if (this.PersistentAxes.Contains(axis))
                throw new InvalidOperationException("SeriesHostAxesCollection: Invalid Attempt To Remove Permanent Axis From Series Host");
            base.RemoveItem(index);
        }
    }
}
