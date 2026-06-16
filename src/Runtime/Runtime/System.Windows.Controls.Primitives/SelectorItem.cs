
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using OpenSilver.Internal;

namespace System.Windows.Controls.Primitives;

/// <summary>
/// Provides a base class for <see cref="ListBoxItem"/>, <see cref="ComboBoxItem"/>.
/// </summary>
public class SelectorItem : ContentControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectorItem"/> class.
    /// </summary>
    protected SelectorItem() { }

    /// <summary>
    /// Identifies the <see cref="Selected"/> routed event.
    /// </summary>
    public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(SelectorItem));

    /// <summary>
    /// Occurs when a <see cref="SelectorItem"/> is selected.
    /// </summary>
    public event RoutedEventHandler Selected
    {
        add => AddHandler(SelectedEvent, value);
        remove => RemoveHandler(SelectedEvent, value);
    }

    /// <summary>
    /// Called when the <see cref="SelectorItem"/> is selected in a <see cref="Selector"/>.
    /// </summary>
    /// <param name="e">
    /// The event data.
    /// </param>
    protected virtual void OnSelected(RoutedEventArgs e) => RaiseEvent(e);

    /// <summary>
    /// Identifies the <see cref="Unselected"/> routed event.
    /// </summary>
    public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(SelectorItem));

    /// <summary>
    /// Occurs when a <see cref="SelectorItem"/> is unselected.
    /// </summary>
    public event RoutedEventHandler Unselected
    {
        add => AddHandler(UnselectedEvent, value);
        remove => RemoveHandler(UnselectedEvent, value);
    }

    /// <summary>
    /// Called when the <see cref="SelectorItem"/> is unselected in a <see cref="Selector"/>.
    /// </summary>
    /// <param name="e">
    /// The event data.
    /// </param>
    protected virtual void OnUnselected(RoutedEventArgs e) => RaiseEvent(e);

    /// <summary>
    /// Gets or sets a value that indicates whether the item is selected in a selector.
    /// </summary>
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValueInternal(IsSelectedProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
        Selector.IsSelectedProperty.AddOwner(
            typeof(SelectorItem),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsSelectedChanged));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var selectorItem = (SelectorItem)d;

        if ((bool)e.NewValue)
        {
            selectorItem.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, selectorItem));
        }
        else
        {
            selectorItem.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, selectorItem));
        }

        selectorItem.UpdateVisualStates();
    }

    internal Selector ParentSelector => ItemsControl.ItemsControlFromItemContainer(this) as Selector;
}
