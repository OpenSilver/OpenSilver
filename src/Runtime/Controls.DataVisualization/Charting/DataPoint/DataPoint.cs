// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Linq;
using System.ComponentModel;

#if MIGRATION
using System.Windows.Input;
using System.Windows.Media;
#else
using System;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>Represents a control that displays a data point.</summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Hidden")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    [TemplateVisualState(GroupName = "RevealStates", Name = "Shown")]
    public abstract class DataPoint : Control
    {
        /// <summary>
        /// Identifies the IsSelectionEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectionEnabledProperty = DependencyProperty.Register(nameof(IsSelectionEnabled), typeof(bool), typeof(DataPoint), new PropertyMetadata((object)false, new PropertyChangedCallback(DataPoint.OnIsSelectionEnabledPropertyChanged)));
        /// <summary>Identifies the IsSelected dependency property.</summary>
        internal static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(DataPoint), new PropertyMetadata((object)false, new PropertyChangedCallback(DataPoint.OnIsSelectedPropertyChanged)));
        /// <summary>
        /// Identifies the ActualDependentValue dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualDependentValueProperty = DependencyProperty.Register(nameof(ActualDependentValue), typeof(IComparable), typeof(DataPoint), new PropertyMetadata((object)0.0, new PropertyChangedCallback(DataPoint.OnActualDependentValuePropertyChanged)));
        /// <summary>Identifies the DependentValue dependency property.</summary>
        public static readonly DependencyProperty DependentValueProperty = DependencyProperty.Register(nameof(DependentValue), typeof(IComparable), typeof(DataPoint), new PropertyMetadata((object)null, new PropertyChangedCallback(DataPoint.OnDependentValuePropertyChanged)));
        /// <summary>
        /// Identifies the DependentValueStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty DependentValueStringFormatProperty = DependencyProperty.Register(nameof(DependentValueStringFormat), typeof(string), typeof(DataPoint), new PropertyMetadata((object)null, new PropertyChangedCallback(DataPoint.OnDependentValueStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the FormattedDependentValue dependency property.
        /// </summary>
        public static readonly DependencyProperty FormattedDependentValueProperty = DependencyProperty.Register(nameof(FormattedDependentValue), typeof(string), typeof(DataPoint), (PropertyMetadata)null);
        /// <summary>
        /// Identifies the FormattedIndependentValue dependency property.
        /// </summary>
        public static readonly DependencyProperty FormattedIndependentValueProperty = DependencyProperty.Register(nameof(FormattedIndependentValue), typeof(string), typeof(DataPoint), (PropertyMetadata)null);
        /// <summary>Identifies the IndependentValue dependency property.</summary>
        public static readonly DependencyProperty IndependentValueProperty = DependencyProperty.Register(nameof(IndependentValue), typeof(object), typeof(DataPoint), new PropertyMetadata((object)null, new PropertyChangedCallback(DataPoint.OnIndependentValuePropertyChanged)));
        /// <summary>
        /// Identifies the IndependentValueStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty IndependentValueStringFormatProperty = DependencyProperty.Register(nameof(IndependentValueStringFormat), typeof(string), typeof(DataPoint), new PropertyMetadata((object)null, new PropertyChangedCallback(DataPoint.OnIndependentValueStringFormatPropertyChanged)));
        /// <summary>
        /// Identifies the ActualIndependentValue dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualIndependentValueProperty = DependencyProperty.Register(nameof(ActualIndependentValue), typeof(object), typeof(DataPoint), new PropertyMetadata(new PropertyChangedCallback(DataPoint.OnActualIndependentValuePropertyChanged)));
        /// <summary>Identifies the State dependency property.</summary>
        internal static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(DataPointState), typeof(DataPoint), new PropertyMetadata((object)DataPointState.Created, new PropertyChangedCallback(DataPoint.OnStatePropertyChanged)));
        /// <summary>Common state group.</summary>
        internal const string GroupCommonStates = "CommonStates";
        /// <summary>Normal state of the Common group.</summary>
        internal const string StateCommonNormal = "Normal";
        /// <summary>MouseOver state of the Common group.</summary>
        internal const string StateCommonMouseOver = "MouseOver";
        /// <summary>Selection state group.</summary>
        internal const string GroupSelectionStates = "SelectionStates";
        /// <summary>Unselected state of the Selection group.</summary>
        internal const string StateSelectionUnselected = "Unselected";
        /// <summary>Selected state of the Selection group.</summary>
        internal const string StateSelectionSelected = "Selected";
        /// <summary>Reveal state group.</summary>
        internal const string GroupRevealStates = "RevealStates";
        /// <summary>Shown state of the Reveal group.</summary>
        internal const string StateRevealShown = "Shown";
        /// <summary>Hidden state of the Reveal group.</summary>
        internal const string StateRevealHidden = "Hidden";
        /// <summary>
        /// A value indicating whether the mouse is hovering over the data
        /// point.
        /// </summary>
        private bool _isHovered;
        /// <summary>
        /// A value indicating whether the actual independent value is being
        /// coerced.
        /// </summary>
        private bool _isCoercingActualDependentValue;
        /// <summary>
        /// The preserved previous actual dependent value before coercion.
        /// </summary>
        private IComparable _oldActualDependentValueBeforeCoercion;
        /// <summary>
        /// A value indicating whether the actual independent value is being
        /// coerced.
        /// </summary>
        private bool _isCoercingActualIndependentValue;
        /// <summary>
        /// The preserved previous actual dependent value before coercion.
        /// </summary>
        private object _oldActualIndependentValueBeforeCoercion;
        /// <summary>
        /// Tracks whether the Reveal/Shown VisualState is available.
        /// </summary>
        private bool _haveStateRevealShown;
        /// <summary>
        /// Tracks whether the Reveal/Hidden VisualState is available.
        /// </summary>
        private bool _haveStateRevealHidden;
        /// <summary>Tracks whether the template has been applied yet.</summary>
        private bool _templateApplied;

        /// <summary>
        /// Gets or sets a value indicating whether selection is enabled.
        /// </summary>
        public bool IsSelectionEnabled
        {
            get
            {
                return (bool)this.GetValue(DataPoint.IsSelectionEnabledProperty);
            }
            set
            {
                this.SetValue(DataPoint.IsSelectionEnabledProperty, (object)value);
            }
        }

        /// <summary>IsSelectionEnabledProperty property changed handler.</summary>
        /// <param name="d">Control that changed its IsSelectionEnabled.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsSelectionEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnIsSelectionEnabledPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>IsSelectionEnabledProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnIsSelectionEnabledPropertyChanged(bool oldValue, bool newValue)
        {
            if (newValue)
                return;
            this.IsSelected = false;
            this.IsHovered = false;
        }

        /// <summary>
        /// Gets a value indicating whether the data point is active.
        /// </summary>
        internal bool IsActive { get; private set; }

        /// <summary>
        /// An event raised when the IsSelected property is changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<bool> IsSelectedChanged;

        /// <summary>
        /// Gets a value indicating whether the mouse is hovering over
        /// the data point.
        /// </summary>
        protected bool IsHovered
        {
            get
            {
                return this._isHovered;
            }
            private set
            {
                bool isHovered = this._isHovered;
                this._isHovered = value;
                if (isHovered == this._isHovered)
                    return;
                this.OnIsHoveredPropertyChanged(isHovered, value);
            }
        }

        /// <summary>IsHoveredProperty property changed handler.</summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnIsHoveredPropertyChanged(bool oldValue, bool newValue)
        {
            VisualStateManager.GoToState((Control)this, newValue ? "MouseOver" : "Normal", true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data point is selected.
        /// </summary>
        internal bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(DataPoint.IsSelectedProperty);
            }
            set
            {
                this.SetValue(DataPoint.IsSelectedProperty, (object)value);
            }
        }

        /// <summary>IsSelectedProperty property changed handler.</summary>
        /// <param name="d">Control that changed its IsSelected.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnIsSelectedPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>IsSelectedProperty property changed handler.</summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnIsSelectedPropertyChanged(bool oldValue, bool newValue)
        {
            VisualStateManager.GoToState((Control)this, newValue ? "Selected" : "Unselected", true);
            RoutedPropertyChangedEventHandler<bool> isSelectedChanged = this.IsSelectedChanged;
            if (isSelectedChanged == null)
                return;
            isSelectedChanged((object)this, new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue));
        }

        /// <summary>
        /// Event raised when the actual dependent value of the data point is changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<IComparable> ActualDependentValueChanged;

        /// <summary>
        /// Gets or sets the actual dependent value displayed in the chart.
        /// </summary>
        public IComparable ActualDependentValue
        {
            get
            {
                return (IComparable)this.GetValue(DataPoint.ActualDependentValueProperty);
            }
            set
            {
                this.SetValue(DataPoint.ActualDependentValueProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when the value of the ActualDependentValue property changes.
        /// </summary>
        /// <param name="d">Control that changed its ActualDependentValue.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnActualDependentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnActualDependentValuePropertyChanged((IComparable)e.OldValue, (IComparable)e.NewValue);
        }

        /// <summary>
        /// Called when the value of the ActualDependentValue property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnActualDependentValuePropertyChanged(IComparable oldValue, IComparable newValue)
        {
            double doubleValue = 0.0;
            if (!(newValue is double) && ValueHelper.TryConvert((object)newValue, out doubleValue))
            {
                this._isCoercingActualDependentValue = true;
                this._oldActualDependentValueBeforeCoercion = oldValue;
            }
            if (!this._isCoercingActualDependentValue)
            {
                if (this._oldActualDependentValueBeforeCoercion != null)
                {
                    oldValue = this._oldActualDependentValueBeforeCoercion;
                    this._oldActualDependentValueBeforeCoercion = (IComparable)null;
                }
                RoutedPropertyChangedEventHandler<IComparable> dependentValueChanged = this.ActualDependentValueChanged;
                if (dependentValueChanged != null)
                    dependentValueChanged((object)this, new RoutedPropertyChangedEventArgs<IComparable>(oldValue, newValue));
            }
            if (!this._isCoercingActualDependentValue)
                return;
            this._isCoercingActualDependentValue = false;
            this.ActualDependentValue = (IComparable)doubleValue;
        }

        /// <summary>
        /// This event is raised when the dependent value of the data point is
        /// changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<IComparable> DependentValueChanged;

        /// <summary>Gets or sets the dependent value of the Control.</summary>
        public IComparable DependentValue
        {
            get
            {
                return (IComparable)this.GetValue(DataPoint.DependentValueProperty);
            }
            set
            {
                this.SetValue(DataPoint.DependentValueProperty, (object)value);
            }
        }

        /// <summary>Called when the DependentValue property changes.</summary>
        /// <param name="d">Control that changed its DependentValue.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnDependentValuePropertyChanged((IComparable)e.OldValue, (IComparable)e.NewValue);
        }

        /// <summary>Called when the DependentValue property changes.</summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDependentValuePropertyChanged(IComparable oldValue, IComparable newValue)
        {
            this.SetFormattedProperty(DataPoint.FormattedDependentValueProperty, this.DependentValueStringFormat, (object)newValue);
            RoutedPropertyChangedEventHandler<IComparable> dependentValueChanged = this.DependentValueChanged;
            if (dependentValueChanged != null)
                dependentValueChanged((object)this, new RoutedPropertyChangedEventArgs<IComparable>(oldValue, newValue));
            if (this.State != DataPointState.Created)
                return;
            double doubleValue;
            this.ActualDependentValue = !ValueHelper.TryConvert((object)newValue, out doubleValue) ? newValue : (IComparable)doubleValue;
        }

        /// <summary>
        /// Gets or sets the format string for the FormattedDependentValue property.
        /// </summary>
        public string DependentValueStringFormat
        {
            get
            {
                return this.GetValue(DataPoint.DependentValueStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DataPoint.DependentValueStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when DependentValueStringFormat property changes.
        /// </summary>
        /// <param name="d">Control that changed its DependentValueStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentValueStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataPoint).OnDependentValueStringFormatPropertyChanged(e.OldValue as string, e.NewValue as string);
        }

        /// <summary>
        /// Called when DependentValueStringFormat property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnDependentValueStringFormatPropertyChanged(string oldValue, string newValue)
        {
            this.SetFormattedProperty(DataPoint.FormattedDependentValueProperty, newValue, (object)this.DependentValue);
        }

        /// <summary>
        /// Gets the DependentValue as formatted by the DependentValueStringFormat property.
        /// </summary>
        public string FormattedDependentValue
        {
            get
            {
                return this.GetValue(DataPoint.FormattedDependentValueProperty) as string;
            }
        }

        /// <summary>
        /// Gets the IndependentValue as formatted by the IndependentValueStringFormat property.
        /// </summary>
        public string FormattedIndependentValue
        {
            get
            {
                return this.GetValue(DataPoint.FormattedIndependentValueProperty) as string;
            }
        }

        /// <summary>
        /// Called when the independent value of the data point is changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<object> IndependentValueChanged;

        /// <summary>Gets or sets the independent value.</summary>
        public object IndependentValue
        {
            get
            {
                return this.GetValue(DataPoint.IndependentValueProperty);
            }
            set
            {
                this.SetValue(DataPoint.IndependentValueProperty, value);
            }
        }

        /// <summary>Called when the IndependentValue property changes.</summary>
        /// <param name="d">Control that changed its IndependentValue.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnIndependentValuePropertyChanged(e.OldValue, e.NewValue);
        }

        /// <summary>Called when the IndependentValue property changes.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnIndependentValuePropertyChanged(object oldValue, object newValue)
        {
            this.SetFormattedProperty(DataPoint.FormattedIndependentValueProperty, this.IndependentValueStringFormat, newValue);
            RoutedPropertyChangedEventHandler<object> independentValueChanged = this.IndependentValueChanged;
            if (independentValueChanged != null)
                independentValueChanged((object)this, new RoutedPropertyChangedEventArgs<object>(oldValue, newValue));
            if (this.State != DataPointState.Created)
                return;
            double doubleValue;
            this.ActualIndependentValue = !ValueHelper.TryConvert(newValue, out doubleValue) ? newValue : (object)doubleValue;
        }

        /// <summary>
        /// Gets or sets the format string for the FormattedIndependentValue property.
        /// </summary>
        public string IndependentValueStringFormat
        {
            get
            {
                return this.GetValue(DataPoint.IndependentValueStringFormatProperty) as string;
            }
            set
            {
                this.SetValue(DataPoint.IndependentValueStringFormatProperty, (object)value);
            }
        }

        /// <summary>
        /// Called when the value of the IndependentValueStringFormat property changes.
        /// </summary>
        /// <param name="d">Control that changed its IndependentValueStringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentValueStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataPoint).OnIndependentValueStringFormatPropertyChanged(e.OldValue as string, e.NewValue as string);
        }

        /// <summary>
        /// Called when the value of the IndependentValueStringFormat property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnIndependentValueStringFormatPropertyChanged(string oldValue, string newValue)
        {
            this.SetFormattedProperty(DataPoint.FormattedIndependentValueProperty, newValue, this.IndependentValue);
        }

        /// <summary>
        /// Occurs when the actual independent value of the data point is
        /// changed.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<object> ActualIndependentValueChanged;

        /// <summary>Gets or sets the actual independent value.</summary>
        public object ActualIndependentValue
        {
            get
            {
                return this.GetValue(DataPoint.ActualIndependentValueProperty);
            }
            set
            {
                this.SetValue(DataPoint.ActualIndependentValueProperty, value);
            }
        }

        /// <summary>
        /// Called when the ActualIndependentValue property changes.
        /// </summary>
        /// <param name="d">Control that changed its ActualIndependentValue.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnActualIndependentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnActualIndependentValuePropertyChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called when the ActualIndependentValue property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnActualIndependentValuePropertyChanged(object oldValue, object newValue)
        {
            double doubleValue = 0.0;
            if (!(newValue is double) && ValueHelper.TryConvert(newValue, out doubleValue))
            {
                this._isCoercingActualIndependentValue = true;
                this._oldActualIndependentValueBeforeCoercion = oldValue;
            }
            if (!this._isCoercingActualIndependentValue)
            {
                if (this._oldActualIndependentValueBeforeCoercion != null)
                {
                    oldValue = this._oldActualIndependentValueBeforeCoercion;
                    this._oldActualIndependentValueBeforeCoercion = (object)null;
                }
                RoutedPropertyChangedEventHandler<object> independentValueChanged = this.ActualIndependentValueChanged;
                if (independentValueChanged != null)
                    independentValueChanged((object)this, new RoutedPropertyChangedEventArgs<object>(oldValue, newValue));
            }
            if (!this._isCoercingActualIndependentValue)
                return;
            this._isCoercingActualIndependentValue = false;
            this.ActualIndependentValue = (object)doubleValue;
        }

        /// <summary>Occurs when the state of a data point is changed.</summary>
        internal event RoutedPropertyChangedEventHandler<DataPointState> StateChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the State property is being
        /// coerced to its previous value.
        /// </summary>
        private bool IsCoercingState { get; set; }

        /// <summary>Gets or sets the state of the data point.</summary>
        internal DataPointState State
        {
            get
            {
                return (DataPointState)this.GetValue(DataPoint.StateProperty);
            }
            set
            {
                this.SetValue(DataPoint.StateProperty, (object)value);
            }
        }

        /// <summary>Called when the value of the State property changes.</summary>
        /// <param name="d">Control that changed its State.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPoint)d).OnStatePropertyChanged((DataPointState)e.OldValue, (DataPointState)e.NewValue);
        }

        /// <summary>Called when the value of the State property changes.</summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnStatePropertyChanged(DataPointState oldValue, DataPointState newValue)
        {
            if (this.IsCoercingState)
                return;
            if (DataPointState.PendingRemoval <= newValue)
                this.IsActive = false;
            if (newValue < oldValue)
            {
                this.IsCoercingState = true;
                this.State = oldValue;
                this.IsCoercingState = false;
            }
            else
            {
                if (newValue > DataPointState.Normal)
                    this.IsSelectionEnabled = false;
                bool flag = false;
                switch (newValue)
                {
                    case DataPointState.Showing:
                    case DataPointState.Hiding:
                        flag = this.GoToCurrentRevealState();
                        break;
                }
                RoutedPropertyChangedEventHandler<DataPointState> stateChanged = this.StateChanged;
                if (stateChanged != null)
                    stateChanged((object)this, new RoutedPropertyChangedEventArgs<DataPointState>(oldValue, newValue));
                if (!flag && this._templateApplied)
                {
                    switch (newValue)
                    {
                        case DataPointState.Showing:
                            this.State = DataPointState.Normal;
                            break;
                        case DataPointState.Hiding:
                            this.State = DataPointState.Hidden;
                            break;
                    }
                }
            }
        }

        /// <summary>Gets the implementation root of the Control.</summary>
        /// <remarks>
        /// Implements Silverlight's corresponding internal property on Control.
        /// </remarks>
        private new FrameworkElement ImplementationRoot
        {
            get
            {
                return 1 == VisualTreeHelper.GetChildrenCount((DependencyObject)this) ? VisualTreeHelper.GetChild((DependencyObject)this, 0) as FrameworkElement : (FrameworkElement)null;
            }
        }

        /// <summary>Initializes a new instance of the DataPoint class.</summary>
        protected DataPoint()
        {
            this.Loaded += new RoutedEventHandler(this.OnLoaded);
            this.IsActive = true;
        }

        /// <summary>
        /// Updates the Control's visuals to reflect the current state(s).
        /// </summary>
        /// <returns>True if a state transition was started.</returns>
        private bool GoToCurrentRevealState()
        {
            bool flag = false;
            string stateName = (string)null;
            switch (this.State)
            {
                case DataPointState.Showing:
                    if (this._haveStateRevealShown)
                    {
                        stateName = "Shown";
                        break;
                    }
                    break;
                case DataPointState.Hiding:
                    if (this._haveStateRevealHidden)
                    {
                        stateName = "Hidden";
                        break;
                    }
                    break;
            }
            if (null != stateName)
            {
                if (!DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                    this.Dispatcher.BeginInvoke((Action)(() => VisualStateManager.GoToState((Control)this, stateName, true)));
                else
                    VisualStateManager.GoToState((Control)this, stateName, false);
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// Builds the visual tree for the DataPoint when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            VisualStateGroup visualStateGroup1 = VisualStateManager.GetVisualStateGroups(this.ImplementationRoot).CastWrapper<VisualStateGroup>().Where<VisualStateGroup>((Func<VisualStateGroup, bool>)(group => "RevealStates" == group.Name)).FirstOrDefault<VisualStateGroup>();
            if (null != visualStateGroup1)
                visualStateGroup1.CurrentStateChanged -= new EventHandler<VisualStateChangedEventArgs>(this.OnCurrentStateChanged);
            base.OnApplyTemplate();
            this._haveStateRevealShown = false;
            this._haveStateRevealHidden = false;
            VisualStateGroup visualStateGroup2 = VisualStateManager.GetVisualStateGroups(this.ImplementationRoot).CastWrapper<VisualStateGroup>().Where<VisualStateGroup>((Func<VisualStateGroup, bool>)(group => "RevealStates" == group.Name)).FirstOrDefault<VisualStateGroup>();
            if (null != visualStateGroup2)
            {
                visualStateGroup2.CurrentStateChanged += new EventHandler<VisualStateChangedEventArgs>(this.OnCurrentStateChanged);
                this._haveStateRevealShown = visualStateGroup2.States.CastWrapper<VisualState>().Where<VisualState>((Func<VisualState, bool>)(state => "Shown" == state.Name)).Any<VisualState>();
                this._haveStateRevealHidden = visualStateGroup2.States.CastWrapper<VisualState>().Where<VisualState>((Func<VisualState, bool>)(state => "Hidden" == state.Name)).Any<VisualState>();
            }
            this._templateApplied = true;
            this.GoToCurrentRevealState();
            if (!DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            this.State = DataPointState.Showing;
        }

        /// <summary>
        /// Changes the DataPoint object's state after one of the VSM state animations completes.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            switch (e.NewState.Name)
            {
                case "Shown":
                    if (this.State != DataPointState.Showing)
                        break;
                    this.State = DataPointState.Normal;
                    break;
                case "Hidden":
                    if (this.State != DataPointState.Hiding)
                        break;
                    this.State = DataPointState.Hidden;
                    break;
            }
        }

        /// <summary>Handles the Control's Loaded event.</summary>
        /// <param name="sender">The Control.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.GoToCurrentRevealState();
        }

        /// <summary>Provides handling for the MouseEnter event.</summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!this.IsSelectionEnabled)
                return;
            this.IsHovered = true;
        }

        /// <summary>Provides handling for the MouseLeave event.</summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelectionEnabled)
                return;
            this.IsHovered = false;
        }

        /// <summary>Provides handling for the MouseLeftButtonDown event.</summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.DefinitionSeriesIsSelectionEnabledHandling)
            {
                if (!this.IsSelectionEnabled)
                    e.Handled = true;
                base.OnMouseLeftButtonDown(e);
            }
            else
            {
                base.OnMouseLeftButtonDown(e);
                if (this.IsSelectionEnabled)
                {
                    this.IsSelected = ModifierKeys.None == (ModifierKeys.Control & Keyboard.Modifiers);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to handle IsSelectionEnabled in the DefinitionSeries manner.
        /// </summary>
        internal bool DefinitionSeriesIsSelectionEnabledHandling { get; set; }

        /// <summary>Sets a dependency property with the specified format.</summary>
        /// <param name="property">The DependencyProperty to set.</param>
        /// <param name="format">The Format string to apply to the value.</param>
        /// <param name="value">The value of the dependency property to be formatted.</param>
        internal void SetFormattedProperty(DependencyProperty property, string format, object value)
        {
            this.SetValue(property, (object)string.Format((IFormatProvider)CultureInfo.CurrentCulture, format ?? "{0}", new object[1]
            {
        value
            }));
        }
    }
}