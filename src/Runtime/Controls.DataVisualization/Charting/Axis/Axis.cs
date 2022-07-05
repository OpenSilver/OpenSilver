// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>Axis position.</summary>
    public enum AxisLocation
    {
        Auto,
        Left,
        Top,
        Right,
        Bottom,
    }

    /// <summary>
    /// An axis class used to determine the plot area coordinate of values.
    /// </summary>
    public abstract class Axis : Control, IAxis
    {
        /// <summary>Identifies the Location dependency property.</summary>
        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register(nameof(Location), typeof(AxisLocation), typeof(Axis), new PropertyMetadata((object)AxisLocation.Auto, new PropertyChangedCallback(Axis.OnLocationPropertyChanged)));
        /// <summary>Identifies the Orientation dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(AxisOrientation), typeof(Axis), new PropertyMetadata((object)AxisOrientation.None, new PropertyChangedCallback(Axis.OnOrientationPropertyChanged)));

        /// <summary>Gets or sets the axis location.</summary>
        public AxisLocation Location
        {
            get
            {
                return (AxisLocation)this.GetValue(Axis.LocationProperty);
            }
            set
            {
                this.SetValue(Axis.LocationProperty, (object)value);
            }
        }

        /// <summary>LocationProperty property changed handler.</summary>
        /// <param name="d">Axis that changed its Location.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnLocationPropertyChanged((AxisLocation)e.OldValue, (AxisLocation)e.NewValue);
        }

        /// <summary>LocationProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnLocationPropertyChanged(AxisLocation oldValue, AxisLocation newValue)
        {
            RoutedPropertyChangedEventHandler<AxisLocation> locationChanged = this.LocationChanged;
            if (locationChanged == null)
                return;
            locationChanged((object)this, new RoutedPropertyChangedEventArgs<AxisLocation>(oldValue, newValue));
        }

        /// <summary>
        /// This event is raised when the location property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<AxisLocation> LocationChanged;

        /// <summary>Gets the list of child axes belonging to this axis.</summary>
        public ObservableCollection<IAxis> DependentAxes { get; private set; }

        /// <summary>Gets or sets the orientation of the axis.</summary>
        public AxisOrientation Orientation
        {
            get
            {
                return (AxisOrientation)this.GetValue(Axis.OrientationProperty);
            }
            set
            {
                this.SetValue(Axis.OrientationProperty, (object)value);
            }
        }

        /// <summary>OrientationProperty property changed handler.</summary>
        /// <param name="d">Axis that changed its Orientation.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)d).OnOrientationPropertyChanged((AxisOrientation)e.OldValue, (AxisOrientation)e.NewValue);
        }

        /// <summary>OrientationProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnOrientationPropertyChanged(AxisOrientation oldValue, AxisOrientation newValue)
        {
            RoutedPropertyChangedEventHandler<AxisOrientation> orientationChanged = this.OrientationChanged;
            if (orientationChanged == null)
                return;
            orientationChanged((object)this, new RoutedPropertyChangedEventArgs<AxisOrientation>(oldValue, newValue));
        }

        /// <summary>
        /// This event is raised when the Orientation property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<AxisOrientation> OrientationChanged;

        /// <summary>Raises the invalidated event.</summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnInvalidated(RoutedEventArgs args)
        {
            foreach (IAxisListener registeredListener in (Collection<IAxisListener>)this.RegisteredListeners)
                registeredListener.AxisInvalidated((IAxis)this);
        }

        /// <summary>
        /// Gets or the collection of series that are using the Axis.
        /// </summary>
        public ObservableCollection<IAxisListener> RegisteredListeners { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the axis can plot a value.
        /// </summary>
        /// <param name="value">The value to plot.</param>
        /// <returns>A value indicating whether the axis can plot a value.</returns>
        public abstract bool CanPlot(object value);

        /// <summary>The plot area coordinate of a value.</summary>
        /// <param name="value">The value for which to retrieve the plot area
        /// coordinate.</param>
        /// <returns>The plot area coordinate.</returns>
        public abstract UnitValue GetPlotAreaCoordinate(object value);

        /// <summary>Instantiates a new instance of the Axis class.</summary>
        protected Axis()
        {
            this.RegisteredListeners = (ObservableCollection<IAxisListener>)new UniqueObservableCollection<IAxisListener>();
            this.RegisteredListeners.CollectionChanged += new NotifyCollectionChangedEventHandler(this.RegisteredListenersCollectionChanged);
            this.DependentAxes = new ObservableCollection<IAxis>();
            this.DependentAxes.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnChildAxesCollectionChanged);
        }

        /// <summary>Child axes collection changed.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnChildAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnDependentAxesCollectionChanged();
        }

        /// <summary>Child axes collection changed.</summary>
        protected virtual void OnDependentAxesCollectionChanged()
        {
        }

        /// <summary>
        /// This event is raised when the registered listeners collection is
        /// changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void RegisteredListenersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (IAxisListener oldItem in (IEnumerable)e.OldItems)
                    this.OnObjectUnregistered(oldItem);
            }
            if (e.NewItems == null)
                return;
            foreach (IAxisListener newItem in (IEnumerable)e.NewItems)
                this.OnObjectRegistered(newItem);
        }

        /// <summary>This method is invoked when a series is registered.</summary>
        /// <param name="series">The series that has been registered.</param>
        protected virtual void OnObjectRegistered(IAxisListener series)
        {
        }

        /// <summary>This method is invoked when a series is unregistered.</summary>
        /// <param name="series">The series that has been unregistered.</param>
        protected virtual void OnObjectUnregistered(IAxisListener series)
        {
        }
    }
}