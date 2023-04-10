namespace System.Windows;

public abstract class FrameworkElementBase : UIElement
{
    public abstract DependencyObject GetParent();

    internal abstract bool HasLogicalChildren { get; set; }

    internal abstract void SetTemplateChild(FrameworkElementBase templateChild);

    internal abstract DependencyObject TemplatedParent { get; set; }

    public static readonly RoutedEvent LoadedEvent =
        new RoutedEvent(
            nameof(Loaded),
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(FrameworkElementBase));

    /// <summary>
    /// Occurs when a FrameworkElement has been constructed and added to the object tree.
    /// </summary>
    public event RoutedEventHandler Loaded;

    internal void RaiseLoadedEvent() => Loaded?.Invoke(this, new RoutedEventArgs());

    // Indicates that the StyleProperty has been set locally on this element
    internal abstract bool HasLocalStyle { get; set; }

    internal abstract void UpdateHasLocalStyle();

    internal abstract Style ImplicitStyle { get; }

    internal abstract Style ThemeStyle { get; }

    internal abstract bool HasThemeStyleEverBeenFetched { get; set; }

    internal abstract void AddLogicalChild(object child);

    internal abstract void RemoveLogicalChild(object child);

    /// <summary>
    /// Enable or disable measure/arrange layout system in a sub part
    /// </summary>
    public bool CustomLayout
    {
        get => (bool)GetValue(CustomLayoutProperty);
        set => SetValue(CustomLayoutProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CustomLayout"/> dependency 
    /// property.
    /// </summary>
    public static readonly DependencyProperty CustomLayoutProperty =
        DependencyProperty.Register(
            nameof(CustomLayout),
            typeof(bool),
            typeof(FrameworkElementBase),
            new PropertyMetadata(false, OnCustomLayoutChanged, CoerceCustomLayout));

    private static void OnCustomLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var fe = (FrameworkElementBase)d;
        fe.OnCustomLayoutChanged(e);
    }

    private static object CoerceCustomLayout(DependencyObject d, object baseValue)
    {
        var fe = (FrameworkElementBase)d;
        return fe.CoerceCustomLayout(baseValue);
    }

    protected abstract void OnCustomLayoutChanged(DependencyPropertyChangedEventArgs e);
    protected abstract object CoerceCustomLayout(object baseValue);

    internal abstract void INTERNAL_SizeChangedWhenAttachedToVisualTree();

    internal abstract void LoadResources();

    internal abstract void UnloadResources();

    internal abstract bool ShouldLookupImplicitStyles { get; set; }

    public abstract bool IsLoaded { get; }

    internal bool IsLoadedInResourceDictionary { get; set; }

    internal abstract void INTERNAL_InitializeOuterDomElementSize(object outerDomElement);

    internal abstract void DetachResizeSensorFromDomElement();

    internal abstract void RaiseUnloadedEvent();

    /// <summary>
    /// Gets or sets a value indicating whether the user can interact with the control.
    /// </summary>
    public bool IsEnabled
    {
        get { return (bool)GetValue(IsEnabledProperty); }
        set { SetValue(IsEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsEnabled"/> dependency
    /// property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(
            nameof(IsEnabled),
            typeof(bool),
            typeof(FrameworkElementBase),
            new PropertyMetadata(true, IsEnabled_Changed, CoerceIsEnabled)
            {
                MethodToUpdateDom = IsEnabled_MethodToUpdateDom,
            });

    private static void IsEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var fe = (FrameworkElementBase)d;
        fe.IsEnabled_Changed(e);
    }

    protected abstract void IsEnabled_Changed(DependencyPropertyChangedEventArgs e);

    private static object CoerceIsEnabled(DependencyObject d, object baseValue)
    {
        var fe = (FrameworkElementBase)d;
        return fe.CoerceIsEnabled(baseValue);
    }

    protected abstract object CoerceIsEnabled(object baseValue);

    private static void IsEnabled_MethodToUpdateDom(DependencyObject d, object newValue)
    {
        var fe = (FrameworkElementBase)d;
        fe.IsEnabled_MethodToUpdateDom(newValue);
    }

    protected abstract void IsEnabled_MethodToUpdateDom(object newValue);

    internal abstract void INTERNAL_ApplyAlignmentAndSize();

    #region Triggers

    /// <summary>
    /// Gets the collection of triggers for animations that are defined for a <see cref="FrameworkElementBase"/>.
    /// </summary>
    /// <returns>
    /// The collection of triggers for animations that are defined for this object.
    /// </returns>
    public TriggerCollection Triggers
    {
        get
        {
            TriggerCollection triggers = (TriggerCollection)GetValue(TriggersProperty);
            if (triggers == null)
            {
                triggers = new TriggerCollection(this);
                SetValue(TriggersProperty, triggers);
            }

            return triggers;
        }
    }

    /// <summary>
    /// Identifies the <see cref="Triggers"/> dependency property.
    /// </summary>
    internal static readonly DependencyProperty TriggersProperty =
        DependencyProperty.Register(
            nameof(Triggers),
            typeof(TriggerCollection),
            typeof(FrameworkElementBase),
            null);

    #endregion Triggers
}
