// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenSilver.Internal;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace System.Windows.Controls;

/// <summary>
/// Specifies how <see cref="ToolBar"/> items are placed in the main toolbar panel and in the overflow panel.
/// </summary>
public enum OverflowMode
{
    /// <summary>
    /// Item moves between the main panel and overflow panel, depending on the available space.
    /// </summary>
    AsNeeded,

    /// <summary>
    /// Item is permanently placed in the overflow panel.
    /// </summary>
    Always,

    /// <summary>
    /// Item is never allowed to overflow.
    /// </summary>
    Never,

    // NOTE: if you add or remove any values in this enum, be sure to update ToolBar.IsValidOverflowMode()
}

/// <summary>
/// Provides a container for a group of commands or controls.
/// </summary>
[TemplatePart(Name = ToolBarPanelTemplateName, Type = typeof(ToolBarPanel))]
[TemplatePart(Name = ToolBarOverflowPanelTemplateName, Type = typeof(ToolBarOverflowPanel))]
public class ToolBar : HeaderedItemsControl
{
    #region Constructors

    static ToolBar()
    {
#if WPF
        // Disable tooltips on toolbar when the overflow is open
        ToolTipService.IsEnabledProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoerceToolTipIsEnabled)));
#endif
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(typeof(ToolBar)));

        IsTabStopProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        FocusableProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));

        EventManager.RegisterClassHandler(typeof(ToolBar), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);
        EventManager.RegisterClassHandler(typeof(ToolBar), ButtonBase.ClickEvent, new RoutedEventHandler(OnClick));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBar"/> class.
    /// </summary>
    public ToolBar() { }

#endregion

    #region Properties

    #region Orientation
    /// <summary>
    ///     Property key for OrientationProperty.
    /// </summary>
    private static readonly DependencyPropertyKey OrientationPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly(
            nameof(Orientation),
            typeof(Orientation),
            typeof(ToolBar),
            new FrameworkPropertyMetadata(Orientation.Horizontal, null, CoerceOrientation));

    /// <summary>
    /// Identifies the <see cref="Orientation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = OrientationPropertyKey.DependencyProperty;

    private static object CoerceOrientation(DependencyObject d, object value)
    {
        ToolBarTray toolBarTray = ((ToolBar)d).ToolBarTray;
        return (toolBarTray != null) ? toolBarTray.Orientation : value;
    }

    /// <summary>
    /// Gets the orientation of the <see cref="ToolBar"/>.
    /// </summary>
    /// <returns>
    /// The toolbar orientation. The default is <see cref="Orientation.Horizontal"/>.
    /// </returns>
    public Orientation Orientation => (Orientation)GetValue(OrientationProperty);

    #endregion Orientation

    #region Band
    /// <summary>
    /// Identifies the <see cref="Band"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BandProperty =
        DependencyProperty.Register(
            nameof(Band),
            typeof(int),
            typeof(ToolBar),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    /// <summary>
    /// Gets or sets a value that indicates where the toolbar should be located in the <see cref="ToolBarTray"/>.
    /// </summary>
    /// <returns>
    /// The band of the <see cref="ToolBarTray"/> in which the toolbar is positioned. The default is 0.
    /// </returns>
    public int Band
    {
        get => (int)GetValue(BandProperty);
        set => SetValueInternal(BandProperty, value);
    }
    #endregion Band

    #region BandIndex
    /// <summary>
    /// Identifies the <see cref="BandIndex"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BandIndexProperty =
        DependencyProperty.Register(
            nameof(BandIndex),
            typeof(int),
            typeof(ToolBar),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    /// <summary>
    /// Gets or sets the band index number that indicates the position of the toolbar on the band.
    /// </summary>
    /// <returns>
    /// The position of a toolbar on the band of a <see cref="ToolBarTray"/>.
    /// </returns>
    public int BandIndex
    {
        get => (int)GetValue(BandIndexProperty);
        set => SetValueInternal(BandIndexProperty, value);
    }
    #endregion BandIndex

    #region IsOverflowOpen
    /// <summary>
    /// Identifies the <see cref="IsOverflowOpen"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsOverflowOpenProperty =
        DependencyProperty.Register(
            nameof(IsOverflowOpen),
            typeof(bool),
            typeof(ToolBar),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnOverflowOpenChanged, CoerceIsOverflowOpen));

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="ToolBar"/> overflow area is currently visible.
    /// </summary>
    /// <returns>
    /// true if the overflow area is visible; otherwise, false.
    /// </returns>
    [Bindable(true), Browsable(false), Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsOverflowOpen
    {
        get => (bool)GetValue(IsOverflowOpenProperty);
        set => SetValueInternal(IsOverflowOpenProperty, value);
    }

    private static object CoerceIsOverflowOpen(DependencyObject d, object value)
    {
        if ((bool)value)
        {
            ToolBar tb = (ToolBar)d;
            if (!tb.IsLoaded)
            {
                tb.RegisterToOpenOnLoad();
                return BooleanBoxes.FalseBox;
            }
        }

        return value;
    }

#if WPF
    private static object CoerceToolTipIsEnabled(DependencyObject d, object value)
    {
        ToolBar tb = (ToolBar)d;
        return tb.IsOverflowOpen ? BooleanBoxes.FalseBox : value;
    }
#endif

    private void RegisterToOpenOnLoad()
    {
        Loaded += new RoutedEventHandler(OpenOnLoad);
    }

    private void OpenOnLoad(object sender, RoutedEventArgs e)
    {
        // Open overflow after toolbar has rendered (Loaded is fired before 1st render)
        Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object param)
        {
            CoerceValue(IsOverflowOpenProperty);

            return null;
        }), null);
    }

    private static void OnOverflowOpenChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
    {
        ToolBar toolBar = (ToolBar)element;

        if ((bool)e.NewValue)
        {
            // When the drop down opens, take capture
#if WPF
            Mouse.Capture(toolBar, CaptureMode.SubTree);
#else
            Mouse.Capture(toolBar);
#endif
            toolBar.SetFocusOnToolBarOverflowPanel();
        }
        else
        {
            // If focus is still within the ToolBarOverflowPanel, make sure we the focus is restored to the main focus scope
            ToolBarOverflowPanel overflow = toolBar.ToolBarOverflowPanel;
            if (overflow != null && overflow.IsKeyboardFocusWithin)
            {
                Keyboard.Focus(null);
            }

            if (Mouse.Captured == toolBar)
            {
                Mouse.Capture(null);
            }
        }

#if WPF
        toolBar.CoerceValue(ToolTipService.IsEnabledProperty);
#endif
    }

    private void SetFocusOnToolBarOverflowPanel()
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object param)
        {
            if (ToolBarOverflowPanel != null)
            {
                // If the overflow is opened by keyboard - focus the first item
                // otherwise - set focus on the panel itself
                if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
                {
                    ToolBarOverflowPanel.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                else
                {
                    ToolBarOverflowPanel.Focus();
                }
            }
            return null;
        }), null);
    }

#endregion IsOverflowOpen

    #region HasOverflowItems

    /// <summary>
    ///     The key needed set a read-only property.
    /// </summary>
    internal static readonly DependencyPropertyKey HasOverflowItemsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(HasOverflowItems),
            typeof(bool),
            typeof(ToolBar),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="HasOverflowItems"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HasOverflowItemsProperty = HasOverflowItemsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether the toolbar has items that are not visible.
    /// </summary>
    /// <returns>
    /// true if there are items on the toolbar that are not visible; otherwise, false. The default is false.
    /// </returns>
    public bool HasOverflowItems => (bool)GetValue(HasOverflowItemsProperty);

    #endregion HasOverflowItems

    #region IsOverflowItem
    /// <summary>
    ///     The key needed set a read-only property.
    /// Attached property to indicate if the item is placed in the overflow panel
    /// </summary>
    internal static readonly DependencyPropertyKey IsOverflowItemPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly(
            "IsOverflowItem",
            typeof(bool),
            typeof(ToolBar),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <b>ToolBar.IsOverflowItem</b> attached property.
    /// </summary>
    public static readonly DependencyProperty IsOverflowItemProperty = IsOverflowItemPropertyKey.DependencyProperty;

    /// <summary>
    /// Writes the attached property IsOverflowItem to the given element.
    /// </summary>
    /// <param name="element">The element to which to write the attached property.</param>
    /// <param name="value">The property value to set</param>
    internal static void SetIsOverflowItem(DependencyObject element, bool value)
    {
        element.SetValueInternal(IsOverflowItemPropertyKey, value);
    }

    /// <summary>
    /// Reads the value of the <b>ToolBar.IsOverflowItem</b> property from the specified element.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the property.
    /// </param>
    /// <returns>
    /// The value of the property.
    /// </returns>
    public static bool GetIsOverflowItem(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsOverflowItemProperty);
    }

    #endregion

    #region OverflowMode

    /// <summary>
    /// Identifies the <b>ToolBar.OverflowMode</b> attached property.
    /// </summary>
    public static readonly DependencyProperty OverflowModeProperty =
        DependencyProperty.RegisterAttached(
            "OverflowMode",
            typeof(OverflowMode),
            typeof(ToolBar),
            new FrameworkPropertyMetadata(OverflowMode.AsNeeded, OnOverflowModeChanged),
            IsValidOverflowMode);

    private static void OnOverflowModeChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
    {
        // When OverflowMode changes on a child container of a ToolBar,
        // invalidate layout so that the child can be placed in the correct
        // location (in the main bar or the overflow menu).
        ToolBar toolBar = ItemsControl.ItemsControlFromItemContainer(element) as ToolBar;
        toolBar?.InvalidateLayout();
    }

    private void InvalidateLayout()
    {
        // Reset the calculated min and max size
        _minLength = 0.0;
        _maxLength = 0.0;

        // Min and max sizes are calculated in ToolBar.MeasureOverride
        InvalidateMeasure();

        ToolBarPanel toolBarPanel = this.ToolBarPanel;
        // Whether elements are in the overflow or not is decided
        // in ToolBarPanel.MeasureOverride.
        toolBarPanel?.InvalidateMeasure();
    }

    private static bool IsValidOverflowMode(object o)
    {
        OverflowMode value = (OverflowMode)o;
        return value == OverflowMode.AsNeeded || value == OverflowMode.Always || value == OverflowMode.Never;
    }

    /// <summary>
    /// Writes the value of the <b>ToolBar.OverflowMode</b> property to the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to write the property to.
    /// </param>
    /// <param name="mode">
    /// The property value to set.
    /// </param>
    public static void SetOverflowMode(DependencyObject element, OverflowMode mode)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValueInternal(OverflowModeProperty, mode);
    }

    /// <summary>
    /// Reads the value of the <b>ToolBar.OverflowMode</b> property from the specified element.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the property.
    /// </param>
    /// <returns>
    /// The value of the property.
    /// </returns>
    [AttachedPropertyBrowsableForChildren(IncludeDescendants = true)]
    public static OverflowMode GetOverflowMode(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (OverflowMode)element.GetValue(OverflowModeProperty);
    }
    #endregion

    #endregion Properties

    #region Override methods

    /// <summary>
    /// Provides an appropriate <see cref="ToolBarAutomationPeer"/> implementation for this control,
    /// as part of the WPF infrastructure.
    /// </summary>
    /// <returns>
    /// The type-specific <see cref="AutomationPeer"/> implementation.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new ToolBarAutomationPeer(this);
    }

    /// <inheritdoc />
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        // For certain known types, automatically change their default style
        // to point to a ToolBar version.
        FrameworkElement fe = element as FrameworkElement;
        if (fe != null)
        {
            Type feType = fe.GetType();
            ResourceKey resourceKey = null;
            if (feType == typeof(Button))
                resourceKey = ButtonStyleKey;
            else if (feType == typeof(ToggleButton))
                resourceKey = ToggleButtonStyleKey;
            else if (feType == typeof(Separator))
                resourceKey = SeparatorStyleKey;
            else if (feType == typeof(CheckBox))
                resourceKey = CheckBoxStyleKey;
            else if (feType == typeof(RadioButton))
                resourceKey = RadioButtonStyleKey;
            else if (feType == typeof(ComboBox))
                resourceKey = ComboBoxStyleKey;
            else if (feType == typeof(TextBox))
                resourceKey = TextBoxStyleKey;
            else if (feType == typeof(Menu))
                resourceKey = MenuStyleKey;

            if (resourceKey != null)
            {
                ValueSource vs = DependencyPropertyHelper.GetValueSource(fe, StyleProperty);

                if (vs.BaseValueSource <= BaseValueSource.StyleTrigger)
                {
                    fe.SetResourceReference(StyleProperty, resourceKey);
                }

                fe.DefaultStyleKey = resourceKey;
            }
        }
    }

    internal override void OnTemplateChangedInternal(FrameworkTemplate oldTemplate, FrameworkTemplate newTemplate)
    {
        // Invalidate template references
        _toolBarPanel = null;
        _toolBarOverflowPanel = null;

        base.OnTemplateChangedInternal(oldTemplate, newTemplate);
    }

    /// <inheritdoc />
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        // When items change, invalidate layout so that the decision
        // regarding which items are in the overflow menu can be re-done.
        InvalidateLayout();

        base.OnItemsChanged(e);
    }

    /// <summary>
    /// Remeasures a <see cref="ToolBar"/>.
    /// </summary>
    /// <param name="constraint">
    /// The measurement constraints. A <see cref="ToolBar"/> cannot return a size larger than 
    /// the constraint.
    /// </param>
    /// <returns>
    /// The size of the <see cref="ToolBar"/>.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        // Perform a normal layout
        Size desiredSize = base.MeasureOverride(constraint);

        //
        // MinLength and MaxLength are used by ToolBarTray to determine
        // its layout. ToolBarPanel will calculate its version of these values.
        // ToolBar needs to add on the space used up by elements around the ToolBarPanel.
        //
        // Note: This calculation is not 100% accurate. If a scale transform is applied
        // within the template of the ToolBar (between the ToolBar and the ToolBarPanel),
        // then the coordinate spaces will not match and the values will be wrong.
        //
        // Note: If a ToolBarPanel is not contained within the ToolBar's template,
        // then these values will always be zero, and ToolBarTray will not layout correctly.
        //
        ToolBarPanel toolBarPanel = ToolBarPanel;
        if (toolBarPanel != null)
        {
            // Calculate the extra length from the extra space allocated between the ToolBar and the ToolBarPanel.
            double extraLength;
            Thickness margin = toolBarPanel.Margin;
            if (toolBarPanel.Orientation == Orientation.Horizontal)
            {
                extraLength = Math.Max(0.0, desiredSize.Width - toolBarPanel.DesiredSize.Width + margin.Left + margin.Right);
            }
            else
            {
                extraLength = Math.Max(0.0, desiredSize.Height - toolBarPanel.DesiredSize.Height + margin.Top + margin.Bottom);
            }

            // Add the calculated extra length to the lengths provided by ToolBarPanel
            _minLength = toolBarPanel.MinLength + extraLength;
            _maxLength = toolBarPanel.MaxLength + extraLength;
        }

        ToolBarTray?.ResetDragData();

        return desiredSize;
    }

#if WPF
    /// <summary>
    /// Provides class handling for the <see cref="UIElement.LostMouseCapture"/> routed event that 
    /// occurs when the <see cref="ToolBar"/> loses mouse capture.
    /// </summary>
    /// <param name="e">
    /// The arguments for the <see cref="UIElement.LostMouseCapture"/> event.
    /// </param>
    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        base.OnLostMouseCapture(e);

        // ToolBar has a capture when its overflow panel is open
        // close the overflow panel is case capture is set to null
        if (Mouse.Captured == null)
        {
            Close();
        }
    }
#endif

    #endregion Override methods

    #region Private implementation

    /// <summary>
    /// Gets reference to ToolBar's ToolBarPanel element.
    /// </summary>
    internal ToolBarPanel ToolBarPanel
    {
        get
        {
            if (_toolBarPanel == null)
                _toolBarPanel = FindToolBarPanel();

            return _toolBarPanel;
        }
    }

    private ToolBarPanel FindToolBarPanel()
    {
        DependencyObject child = GetTemplateChild(ToolBarPanelTemplateName);
        ToolBarPanel toolBarPanel = child as ToolBarPanel;
        if (child != null && toolBarPanel == null)
            throw new NotSupportedException(string.Format(Strings.ToolBar_InvalidStyle_ToolBarPanel, child.GetType()));
        return toolBarPanel;
    }

    /// <summary>
    /// Gets reference to ToolBar's ToolBarOverflowPanel element.
    /// </summary>
    internal ToolBarOverflowPanel ToolBarOverflowPanel
    {
        get
        {
            if (_toolBarOverflowPanel == null)
                _toolBarOverflowPanel = FindToolBarOverflowPanel();

            return _toolBarOverflowPanel;
        }
    }

    private ToolBarOverflowPanel FindToolBarOverflowPanel()
    {
        DependencyObject child = GetTemplateChild(ToolBarOverflowPanelTemplateName);
        ToolBarOverflowPanel toolBarOverflowPanel = child as ToolBarOverflowPanel;
        if (child != null && toolBarOverflowPanel == null)
            throw new NotSupportedException(string.Format(Strings.ToolBar_InvalidStyle_ToolBarOverflowPanel, child.GetType()));
        return toolBarOverflowPanel;
    }

    /// <summary>
    /// Provides class handling for the <see cref="UIElement.KeyDown"/> routed event that occurs when a 
    /// key is pressed on an item in the <see cref="ToolBar"/>.
    /// </summary>
    /// <param name="e">
    /// The arguments for the <see cref="UIElement.KeyDown"/> event.
    /// </param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        UIElement newFocusElement = null;
        UIElement currentFocusElement = e.Source as UIElement;
        if (currentFocusElement != null && ItemsControl.ItemsControlFromItemContainer(currentFocusElement) == this)
        {
            // itemsHost should be either ToolBarPanel or ToolBarOverflowPanel
            Panel itemsHost = VisualTreeHelper.GetParent(currentFocusElement) as Panel;
            if (itemsHost != null)
            {
                switch (e.Key)
                {
                    // itemsHost.Children.Count is greater than zero because itemsHost is visual parent of currentFocusElement
                    case Key.Home:
                        newFocusElement = VisualTreeHelper.GetChild(itemsHost, 0) as UIElement;
                        break;
                    case Key.End:
                        newFocusElement = VisualTreeHelper.GetChild(itemsHost, VisualTreeHelper.GetChildrenCount(itemsHost) - 1) as UIElement;
                        break;
                    case Key.Escape:
                        {
                            // If focus is within ToolBarOverflowPanel - move focus the the toggle button
                            ToolBarOverflowPanel overflow = ToolBarOverflowPanel;
                            if (overflow != null && overflow.IsKeyboardFocusWithin)
                            {
                                MoveFocus(new TraversalRequest(FocusNavigationDirection.Last));
                            }
                            else
                            {
                                Keyboard.Focus(null);
                            }

                            // Close the overflow the Esc is pressed
                            Close();
                        }
                        break;
                }

                if (newFocusElement != null)
                {
                    if (newFocusElement.Focus())
                        e.Handled = true;
                }
            }
        }

        if (!e.Handled)
            base.OnKeyDown(e);
    }

    private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
    {
        ToolBar toolBar = (ToolBar)sender;
        // Close the overflow for all unhandled mousedown in ToolBar
        if (!e.Handled)
        {
            toolBar.Close();
            e.Handled = true;
        }
    }

    // ButtonBase.Click class handler
    // When we get a click event from toolbar item - close the overflow panel
    private static void OnClick(object e, RoutedEventArgs args)
    {
        ToolBar toolBar = (ToolBar)e;
        ButtonBase bb = args.OriginalSource as ButtonBase;
        if (toolBar.IsOverflowOpen && bb != null && bb.Parent == toolBar)
            toolBar.Close();
    }

    internal override void OnAncestorChanged()
    {
        // Orientation depends on the logical parent -- so invalidate it when that changes
        CoerceValue(OrientationProperty);
    }

    private void Close()
    {
        SetCurrentValueInternal(IsOverflowOpenProperty, BooleanBoxes.FalseBox);
    }

    private ToolBarTray ToolBarTray
    {
        get
        {
            return Parent as ToolBarTray;
        }
    }

    internal double MinLength
    {
        get { return _minLength; }
    }

    internal double MaxLength
    {
        get { return _maxLength; }
    }

    #endregion Private implementation

    #region private data

    private ToolBarPanel _toolBarPanel;
    private ToolBarOverflowPanel _toolBarOverflowPanel;

    private const string ToolBarPanelTemplateName = "PART_ToolBarPanel";
    private const string ToolBarOverflowPanelTemplateName = "PART_ToolBarOverflowPanel";

    private double _minLength = 0d;
    private double _maxLength = 0d;

    #endregion private data

    #region ItemsStyleKey

    private static SystemThemeKey _buttonStyleKey;
    private static SystemThemeKey _toggleButtonStyleKey;
    private static SystemThemeKey _separatorStyleKey;
    private static SystemThemeKey _checkBoxStyleKey;
    private static SystemThemeKey _radioButtonStyleKey;
    private static SystemThemeKey _comboBoxStyleKey;
    private static SystemThemeKey _textBoxStyleKey;
    private static SystemThemeKey _menuStyleKey;

    /// <summary>
    /// Gets the <see cref="Style"/> applied to buttons on a <see cref="ToolBar"/>.
    /// </summary>
    /// <returns>
    /// A resource key that represents the default style for buttons on the toolbar.
    /// </returns>
    public static ResourceKey ButtonStyleKey
    {
        get
        {
            if (_buttonStyleKey is null)
            {
                Interlocked.CompareExchange(ref _buttonStyleKey, new SystemThemeKey(SystemResourceKeyID.ToolBarButtonStyle), null);
            }

            return _buttonStyleKey;
        }
    }

    /// <summary>
    /// Gets the <see cref="Style"/> applied to <see cref="ToggleButton"/> controls on a <see cref="ToolBar"/>.
    /// </summary>
    /// <returns>
    /// A resource key that represents the default style for toggle buttons on the toolbar.
    /// </returns>
    public static ResourceKey ToggleButtonStyleKey
    {
        get
        {
            if (_toggleButtonStyleKey is null)
            {
                Interlocked.CompareExchange(ref _toggleButtonStyleKey, new SystemThemeKey(SystemResourceKeyID.ToolBarToggleButtonStyle), null);
            }

            return _toggleButtonStyleKey;
        }
    }

    /// <summary>
    /// Gets the <see cref="Style"/> applied to separators on a <see cref="ToolBar"/>.
    /// </summary>
    /// <returns>
    /// A resource key that represents the default style for separators on the toolbar.
    /// </returns>
    public static ResourceKey SeparatorStyleKey
    {
        get
        {
            if (_separatorStyleKey is null)
            {
                Interlocked.CompareExchange(ref _separatorStyleKey, new SystemThemeKey(SystemResourceKeyID.ToolBarSeparatorStyle), null);
            }

            return _separatorStyleKey;
        }
    }

    /// <summary>
    /// Gets the <see cref="Style"/> applied to check boxes on a <see cref="ToolBar"/>.
    /// </summary>
    /// <returns>
    /// A resource key that represents the default style for check boxes on the <see cref="ToolBar"/>.
    /// </returns>
    public static ResourceKey CheckBoxStyleKey
    {
        get
        {
            if (_checkBoxStyleKey is null)
            {
                Interlocked.CompareExchange(ref _checkBoxStyleKey, new SystemThemeKey(SystemResourceKeyID.ToolBarCheckBoxStyle), null);
            }

            return _checkBoxStyleKey;
        }
    }

    /// <summary>
    /// Gets the <see cref="Style"/> applied to radio buttons on a toolbar.
    /// </summary>
    /// <returns>
    /// A resource key that represents the default style for radio buttons on the toolbar.
    /// </returns>
    public static ResourceKey RadioButtonStyleKey
    {
        get
        {
            if (_radioButtonStyleKey is null)
            {
                Interlocked.CompareExchange(ref _radioButtonStyleKey, new SystemThemeKey(SystemResourceKeyID.ToolBarRadioButtonStyle), null);
            }

            return _radioButtonStyleKey;
        }
    }

    /// <summary>
    /// Gets the <see cref="Style"/> applied to combo boxes on a <see cref="ToolBar"/>.
    /// </summary>
    /// <returns>
    /// A resource key that represents the default style for combo boxes on the toolbar.
    /// </returns>
    public static ResourceKey ComboBoxStyleKey
    {
        get
        {
            if (_comboBoxStyleKey is null)
            {
                Interlocked.CompareExchange(ref _comboBoxStyleKey, new SystemThemeKey(SystemResourceKeyID.ToolBarComboBoxStyle), null);
            }

            return _comboBoxStyleKey;
        }
    }

    /// <summary>
    /// Gets the <see cref="Style"/> applied to text boxes on a <see cref="ToolBar"/>.
    /// </summary>
    /// <returns>
    /// A resource key that represents the default style for text boxes on the toolbar.
    /// </returns>
    public static ResourceKey TextBoxStyleKey
    {
        get
        {
            if (_textBoxStyleKey is null)
            {
                Interlocked.CompareExchange(ref _textBoxStyleKey, new SystemThemeKey(SystemResourceKeyID.ToolBarTextBoxStyle), null);
            }

            return _textBoxStyleKey;
        }
    }

    /// <summary>
    /// Gets the <see cref="Style"/> applied to menus on a <see cref="ToolBar"/>.
    /// </summary>
    /// <returns>
    /// A resource key that represents the default style for menus on the toolbar.
    /// </returns>
    public static ResourceKey MenuStyleKey
    {
        get
        {
            if (_menuStyleKey is null)
            {
                Interlocked.CompareExchange(ref _menuStyleKey, new SystemThemeKey(SystemResourceKeyID.ToolBarMenuStyle), null);
            }

            return _menuStyleKey;
        }
    }

    #endregion ItemsStyleKey
}