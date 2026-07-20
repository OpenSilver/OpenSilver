// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using OpenSilver.Internal;

namespace System.Windows.Controls;

/// <summary>
/// Represents a pop-up menu that enables a control to expose functionality that is specific to the context of the control.
/// </summary>
public class ContextMenu : MenuBase
{
    private FrameworkElement _owner;
    private Popup _popup;

    static ContextMenu()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu), new PropertyMetadata(typeof(ContextMenu)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextMenu"/> class.
    /// </summary>
    public ContextMenu()
    {
        MenuModeChanged += new EventHandler(OnIsMenuModeChanged);
    }

    /// <summary>
    /// Identifies the <see cref="HorizontalOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalOffsetProperty =
        DependencyProperty.Register(
            nameof(HorizontalOffset),
            typeof(double),
            typeof(ContextMenu),
            new PropertyMetadata(0.0, OnHorizontalOffsetChanged));

    /// <summary>
    /// Gets or sets the horizontal distance between the target origin and the popup alignment point.
    /// </summary>
    [TypeConverter(typeof(LengthConverter))]
    public double HorizontalOffset
    {
        get => (double)GetValue(HorizontalOffsetProperty);
        set => SetValueInternal(HorizontalOffsetProperty, value);
    }

    private static void OnHorizontalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ((ContextMenu)o)._popup?.HorizontalOffset = (double)e.NewValue;
    }

    /// <summary>
    /// Identifies the <see cref="VerticalOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VerticalOffsetProperty =
        DependencyProperty.Register(
            nameof(VerticalOffset),
            typeof(double),
            typeof(ContextMenu),
            new PropertyMetadata(0.0, OnVerticalOffsetChanged));

    /// <summary>
    /// Gets or sets the vertical distance between the target origin and the popup alignment point.
    /// </summary>
    [TypeConverter(typeof(LengthConverter))]
    public double VerticalOffset
    {
        get => (double)GetValue(VerticalOffsetProperty);
        set => SetValueInternal(VerticalOffsetProperty, value);
    }

    private static void OnVerticalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        ((ContextMenu)o)._popup?.VerticalOffset = (double)e.NewValue;
    }

    /// <summary>
    /// Identifies the <see cref="Placement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PlacementProperty =
        ContextMenuService.PlacementProperty.AddOwner(typeof(ContextMenu));

    /// <summary>
    /// Gets or sets the <see cref="Placement"/> property of a <see cref="ContextMenu"/>.
    /// </summary>
    /// <returns>
    /// One of the <see cref="PlacementMode"/> enumeration. The default is <see cref="PlacementMode.MousePoint"/>.
    /// </returns>
    public PlacementMode Placement
    {
        get => (PlacementMode)GetValue(PlacementProperty);
        set => SetValueInternal(PlacementProperty, value);
    }

    private PlacementMode EffectivePlacement
    {
        get
        {
            if (_owner is not null && !_owner.HasDefaultValue(ContextMenuService.PlacementProperty))
            {
                return ContextMenuService.GetPlacement(_owner);
            }

            return Placement;
        }
    }

    /// <summary>
    /// Identifies the <see cref="PlacementTarget" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty PlacementTargetProperty =
        ContextMenuService.PlacementTargetProperty.AddOwner(typeof(ContextMenu));

    /// <summary>
    /// Gets or sets the <see cref="UIElement"/> relative to which the <see cref="ContextMenu"/> is positioned when it opens.
    /// </summary>
    /// <returns>
    /// The element relative to which the <see cref="ContextMenu"/> is positioned when it opens. The default is null.
    /// </returns>
    public UIElement PlacementTarget
    {
        get => (UIElement)GetValue(PlacementTargetProperty);
        set => SetValueInternal(PlacementTargetProperty, value);
    }

    private UIElement EffectivePlacementTarget
    {
        get
        {
            if (_owner is not null)
            {
                return ContextMenuService.GetPlacementTarget(_owner) ?? _owner;
            }

            return PlacementTarget;
        }
    }

    /// <summary>
    /// Identifies the <see cref="IsOpen"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(ContextMenu),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsOpenChanged));

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="ContextMenu"/> is visible.
    /// </summary>
    /// <returns>
    /// true if the <see cref="ContextMenu"/> is visible; otherwise, false. The default is false.
    /// </returns>
    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValueInternal(IsOpenProperty, value);
    }

    private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var contextMenu = (ContextMenu)o;
        if ((bool)e.NewValue)
        {
            contextMenu.OpenPopup();
        }
        else
        {
            contextMenu.ClosePopup();
        }
    }

    /// <summary>
    /// Gets or sets the owning object for the ContextMenu.
    /// </summary>
    internal FrameworkElement Owner
    {
        get { return _owner; }
        set
        {
            if (_owner is not null)
            {
                _owner.MouseRightButtonDown -= new MouseButtonEventHandler(HandleOwnerMouseRightButtonDown);
            }

            _owner = value;

            if (_owner is not null)
            {
                _owner.MouseRightButtonDown += new MouseButtonEventHandler(HandleOwnerMouseRightButtonDown);
            }
        }
    }

    /// <summary>
    /// Occurs when a particular instance of a <see cref="ContextMenu"/> opens.
    /// </summary>
    public event RoutedEventHandler Opened;

    /// <summary>
    /// Called when the <see cref="Opened"/> event occurs.
    /// </summary>
    /// <param name="e">
    /// The event data for the <see cref="Opened"/> event.
    /// </param>
    protected virtual void OnOpened(RoutedEventArgs e) => Opened?.Invoke(this, e);

    /// <summary>
    /// Occurs when a particular instance of a <see cref="ContextMenu"/> closes.
    /// </summary>
    public event RoutedEventHandler Closed;

    /// <summary>
    /// Called when the <see cref="Closed"/> event occurs.
    /// </summary>
    /// <param name="e">
    /// The event data for the <see cref="Closed"/> event.
    /// </param>
    protected virtual void OnClosed(RoutedEventArgs e) => Closed?.Invoke(this, e);

    /// <inheritdoc />
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        MenuItem.PrepareMenuItem(element, item);
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        e.Handled = true;
        base.OnMouseLeftButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        e.Handled = true;
        base.OnMouseRightButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                FocusNextItem(false);
                e.Handled = true;
                break;
            case Key.Down:
                FocusNextItem(true);
                e.Handled = true;
                break;
            case Key.Escape:
                SetCurrentValueInternal(IsOpenProperty, BooleanBoxes.FalseBox);
                e.Handled = true;
                break;
                // case Key.Apps: // Key.Apps not defined by Silverlight 4
        }
        base.OnKeyDown(e);
    }

    private void OnIsMenuModeChanged(object sender, EventArgs e)
    {
        if (IsMenuMode)
        {
            Focus();
        }
        else
        {
            SetCurrentValueInternal(IsOpenProperty, BooleanBoxes.FalseBox);
        }
    }

    private void HandleOwnerMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        SetCurrentValueInternal(IsOpenProperty, BooleanBoxes.TrueBox);
        e.Handled = true;
    }

    private void FocusNextItem(bool down)
    {
        int count = Items.Count;
        int startingIndex = down ? -1 : count;
        if (Keyboard.FocusedElement is MenuItem focusedMenuItem && this == focusedMenuItem.LogicalParent)
        {
            startingIndex = ItemContainerGenerator.IndexFromContainer(focusedMenuItem);
        }
        int index = startingIndex;
        do
        {
            index = (index + count + (down ? 1 : -1)) % count;
            if (ItemContainerGenerator.ContainerFromIndex(index) is MenuItem container)
            {
                if (container.IsEnabled && container.Focus())
                {
                    break;
                }
            }
        }
        while (index != startingIndex);
    }

    private void OpenPopup()
    {
        if (_popup is null)
        {
            _popup = new Popup
            {
                Child = this,
                StaysWithinScreenBounds = true,
                StayOpen = false,
            };

            _popup.Opened += new EventHandler(OnPopupOpened);
            _popup.Closed += new EventHandler(OnPopupClosed);
            _popup.OutsideClick += new EventHandler<CancelEventArgs>(OnOutsideClick);

            if (ReadLocalValue(DataContextProperty) == DependencyProperty.UnsetValue)
            {
                DependencyObject dataContextSource = Owner ?? Application.Current?.RootVisual;
                SetBinding(DataContextProperty, new Binding("DataContext") { Source = dataContextSource });
            }
        }

        _popup.HorizontalOffset = HorizontalOffset;
        _popup.VerticalOffset = VerticalOffset;
        _popup.Placement = EffectivePlacement;
        _popup.PlacementTarget = EffectivePlacementTarget;

        if (Owner is FrameworkElement fe)
        {
            Point pt = Mouse.GetPosition(null);
            fe.OnContextMenuOpening(pt.X, pt.Y);
        }

        _popup.IsOpen = true;
    }

    private void ClosePopup() => _popup?.IsOpen = false;

    private void OnPopupOpened(object sender, EventArgs e)
    {
        IsMenuMode = true;

        OnOpened(new RoutedEventArgs());
    }

    private void OnPopupClosed(object sender, EventArgs e)
    {
        IsMenuMode = false;

        OnClosed(new RoutedEventArgs());
    }

    private void OnOutsideClick(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        SetCurrentValueInternal(IsOpenProperty, BooleanBoxes.FalseBox);
    }
}