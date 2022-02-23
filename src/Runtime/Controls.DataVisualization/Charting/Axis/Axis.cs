// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// An axis class used to determine the plot area coordinate of values.
    /// </summary>
    [OpenSilver.NotImplemented]
    public abstract class Axis : Control, IAxis
    {
        /// <summary>
        /// Gets the list of child axes belonging to this axis.
        /// </summary>
        public ObservableCollection<IAxis> DependentAxes { get; private set; }

        #region public AxisOrientation Orientation
        /// <summary>
        /// Gets or sets the orientation of the axis.
        /// </summary>
        public AxisOrientation Orientation
        {
            get { return (AxisOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(AxisOrientation),
                typeof(Axis),
                new PropertyMetadata(AxisOrientation.None, OnOrientationPropertyChanged));

        /// <summary>
        /// OrientationProperty property changed handler.
        /// </summary>
        /// <param name="d">Axis that changed its Orientation.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis source = (Axis)d;
            AxisOrientation oldValue = (AxisOrientation)e.OldValue;
            AxisOrientation newValue = (AxisOrientation)e.NewValue;
            source.OnOrientationPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// OrientationProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnOrientationPropertyChanged(AxisOrientation oldValue, AxisOrientation newValue)
        {
            RoutedPropertyChangedEventHandler<AxisOrientation> handler = OrientationChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<AxisOrientation>(oldValue, newValue));
            }
        }

        /// <summary>
        /// This event is raised when the Orientation property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<AxisOrientation> OrientationChanged;

        #endregion public AxisOrientation Orientation

        /// <summary>
        /// Raises the invalidated event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnInvalidated(RoutedEventArgs args)
        {
        }

        /// <summary>
        /// Returns a value indicating whether the axis can plot a value.
        /// </summary>
        /// <param name="value">The value to plot.</param>
        /// <returns>A value indicating whether the axis can plot a value.
        /// </returns>
        public abstract bool CanPlot(object value);

        /// <summary>
        /// Instantiates a new instance of the Axis class.
        /// </summary>
        [OpenSilver.NotImplemented]
        protected Axis()
        {
            //RegisteredListeners = new UniqueObservableCollection<IAxisListener>();
            //this.RegisteredListeners.CollectionChanged += RegisteredListenersCollectionChanged;
            this.DependentAxes = new ObservableCollection<IAxis>();
            this.DependentAxes.CollectionChanged += OnChildAxesCollectionChanged;
        }

        /// <summary>
        /// Child axes collection changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnChildAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnDependentAxesCollectionChanged();
        }

        /// <summary>
        /// Child axes collection changed.
        /// </summary>
        protected virtual void OnDependentAxesCollectionChanged()
        {
        }

        /// <summary>
        /// This event is raised when the registered listeners collection is
        /// changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        [OpenSilver.NotImplemented]
        private void RegisteredListenersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }
    }
}