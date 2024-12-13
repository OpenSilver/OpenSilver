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
/// <QualityBand>Preview</QualityBand>
public class ContextMenu : MenuBase
{
    /// <summary>
    /// Stores a reference to the object that owns the ContextMenu.
    /// </summary>
    private FrameworkElement _owner;

    /// <summary>
    /// Stores a reference to the current Popup.
    /// </summary>
    private Popup _popup;

    /// <summary>
    /// Stores a value indicating whether the IsOpen property is being updated by ContextMenu.
    /// </summary>
    private bool _settingIsOpen;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextMenu"/> class.
    /// </summary>
    public ContextMenu()
    {
        DefaultStyleKey = typeof(ContextMenu);
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

    /// <summary>
    /// Handles changes to the HorizontalOffset DependencyProperty.
    /// </summary>
    /// <param name="o">DependencyObject that changed.</param>
    /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
    private static void OnHorizontalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var contextMenu = (ContextMenu)o;
        if (contextMenu._popup is not null)
        {
            contextMenu._popup.HorizontalOffset = (double)e.NewValue;
        }
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

    /// <summary>
    /// Handles changes to the VerticalOffset DependencyProperty.
    /// </summary>
    /// <param name="o">DependencyObject that changed.</param>
    /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
    private static void OnVerticalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var contextMenu = (ContextMenu)o;
        if (contextMenu._popup is not null)
        {
            contextMenu._popup.VerticalOffset = (double)e.NewValue;
        }
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
            if (_owner is not null && !_owner.HasDefaultValue(ToolTipService.PlacementProperty))
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
    /// Gets or sets a value indicating whether the ContextMenu is visible.
    /// </summary>
    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValueInternal(IsOpenProperty, value);
    }

    /// <summary>
    /// Handles changes to the IsOpen DependencyProperty.
    /// </summary>
    /// <param name="o">DependencyObject that changed.</param>
    /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
    private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var contextMenu = (ContextMenu)o;
        if (!contextMenu._settingIsOpen)
        {
            if ((bool)e.NewValue)
            {
                contextMenu.OpenPopup();
            }
            else
            {
                contextMenu.ClosePopup();
            }
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
    /// Occurs when a particular instance of a ContextMenu opens.
    /// </summary>
    public event RoutedEventHandler Opened;

    /// <summary>
    /// Called when the Opened event occurs.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnOpened(RoutedEventArgs e) => Opened?.Invoke(this, e);

    /// <summary>
    /// Occurs when a particular instance of a ContextMenu closes.
    /// </summary>
    public event RoutedEventHandler Closed;

    /// <summary>
    /// Called when the Closed event occurs.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnClosed(RoutedEventArgs e) => Closed?.Invoke(this, e);

    /// <summary>
    /// Called when the left mouse button is pressed.
    /// </summary>
    /// <param name="e">The event data for the MouseLeftButtonDown event.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        e.Handled = true;
        base.OnMouseLeftButtonDown(e);
    }

    /// <summary>
    /// Called when the right mouse button is pressed.
    /// </summary>
    /// <param name="e">The event data for the MouseRightButtonDown event.</param>
    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        e.Handled = true;
        base.OnMouseRightButtonDown(e);
    }

    /// <summary>
    /// Responds to the KeyDown event.
    /// </summary>
    /// <param name="e">The event data for the KeyDown event.</param>
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
                ClosePopup();
                e.Handled = true;
                break;
                // case Key.Apps: // Key.Apps not defined by Silverlight 4
        }
        base.OnKeyDown(e);
    }

    /// <summary>
    /// Handles the MouseRightButtonDown event for the owning element.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void HandleOwnerMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupService.OnMouseEvent(e);

        OpenPopup();
        e.Handled = true;
    }

    /// <summary>
    /// Sets focus to the next item in the ContextMenu.
    /// </summary>
    /// <param name="down">True to move the focus down; false to move it up.</param>
    private void FocusNextItem(bool down)
    {
        int count = Items.Count;
        int startingIndex = down ? -1 : count;
        if (FocusManager.GetFocusedElement() is MenuItem focusedMenuItem && this == focusedMenuItem.ParentMenuBase)
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

    /// <summary>
    /// Called when a child MenuItem is clicked.
    /// </summary>
    internal void ChildMenuItemClicked() => ClosePopup();

    /// <summary>
    /// Opens the Popup.
    /// </summary>
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

            _popup.OutsideClick += new EventHandler<CancelEventArgs>(OnOutsideClick);

            if (ReadLocalValue(DataContextProperty) == DependencyProperty.UnsetValue)
            {
                DependencyObject dataContextSource = Owner ?? PopupService.RootVisual;
                SetBinding(DataContextProperty, new Binding("DataContext") { Source = dataContextSource });
            }
        }

        _popup.HorizontalOffset = HorizontalOffset;
        _popup.VerticalOffset = VerticalOffset;
        _popup.Placement = EffectivePlacement;
        _popup.PlacementTarget = EffectivePlacementTarget;

        if (Owner is FrameworkElement fe)
        {
            fe.OnContextMenuOpening(PopupService.MousePosition.X, PopupService.MousePosition.Y);
        }

        _popup.IsOpen = true;
        Focus();

        // Update IsOpen
        _settingIsOpen = true;
        IsOpen = true;
        _settingIsOpen = false;

        OnOpened(new RoutedEventArgs());
    }

    private void OnOutsideClick(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        ClosePopup();
    }

    /// <summary>
    /// Closes the Popup.
    /// </summary>
    private void ClosePopup()
    {
        if (_popup is not null)
        {
            _popup.IsOpen = false;
        }

        // Update IsOpen
        _settingIsOpen = true;
        IsOpen = false;
        _settingIsOpen = false;

        OnClosed(new RoutedEventArgs());
    }
}