// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.Primitives;

namespace System.Windows.Controls;

/// <summary>
/// Provides the system implementation for displaying a ContextMenu.
/// </summary>
/// <QualityBand>Preview</QualityBand>
public static class ContextMenuService
{
    /// <summary>
    /// Gets the value of the ContextMenu property of the specified object.
    /// </summary>
    /// <param name="obj">Object to query concerning the ContextMenu property.</param>
    /// <returns>Value of the ContextMenu property.</returns>
    public static ContextMenu GetContextMenu(DependencyObject obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (ContextMenu)obj.GetValue(ContextMenuProperty);
    }

    /// <summary>
    /// Sets the value of the ContextMenu property of the specified object.
    /// </summary>
    /// <param name="obj">Object to set the property on.</param>
    /// <param name="value">Value to set.</param>
    public static void SetContextMenu(DependencyObject obj, ContextMenu value)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        obj.SetValueInternal(ContextMenuProperty, value);
    }

    /// <summary>
    /// Identifies the ContextMenu attached property.
    /// </summary>
    public static readonly DependencyProperty ContextMenuProperty =
        DependencyProperty.RegisterAttached(
            "ContextMenu",
            typeof(ContextMenu),
            typeof(ContextMenuService),
            new PropertyMetadata(null, OnContextMenuChanged));

    /// <summary>
    /// Handles changes to the ContextMenu DependencyProperty.
    /// </summary>
    /// <param name="o">DependencyObject that changed.</param>
    /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
    private static void OnContextMenuChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is FrameworkElement element)
        {
            if (e.OldValue is ContextMenu oldContextMenu)
            {
                oldContextMenu.Owner = null;
            }

            if (e.NewValue is ContextMenu newContextMenu)
            {
                newContextMenu.Owner = element;
            }
        }
    }

    /// <summary>
    /// Identifies the ContextMenuService.PlacementTarget attached property.
    /// </summary>
    public static readonly DependencyProperty PlacementTargetProperty =
        DependencyProperty.RegisterAttached(
            "PlacementTarget",
            typeof(UIElement),
            typeof(ContextMenuService),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets the value of the ContextMenuService.PlacementTarget property of the specified object.
    /// </summary>
    /// <param name="element">
    /// Object to query concerning the ContextMenuService.PlacementTarget property.
    /// </param>
    /// <returns>
    /// Value of the ContextMenuService.PlacementTarget property.
    /// </returns>
    public static UIElement GetPlacementTarget(DependencyObject element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return (UIElement)element.GetValue(PlacementTargetProperty);
    }

    /// <summary>
    /// Sets the value of the ContextMenuService.PlacementTarget property of the specified object.
    /// </summary>
    /// <param name="element">
    /// Object to set value on.
    /// </param>
    /// <param name="value">
    /// Value to set.
    /// </param>
    public static void SetPlacementTarget(DependencyObject element, UIElement value)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValueInternal(PlacementTargetProperty, value);
    }

    /// <summary>
    /// Identifies the ContextMenuService.Placement attached property.
    /// </summary>
    public static readonly DependencyProperty PlacementProperty =
        DependencyProperty.RegisterAttached(
            "Placement",
            typeof(PlacementMode),
            typeof(ContextMenuService),
            new PropertyMetadata(PlacementMode.MousePoint));

    /// <summary>
    /// Gets the value of the ContextMenuService.Placement property of the specified object.
    /// </summary>
    /// <param name="element">
    /// Object to query concerning the ContextMenuService.Placement property.
    /// </param>
    /// <returns>
    /// Value of the ContextMenuService.Placement property.
    /// </returns>
    public static PlacementMode GetPlacement(DependencyObject element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return (PlacementMode)element.GetValue(PlacementProperty);
    }

    /// <summary>
    /// Sets the value of the ContextMenuService.Placement property of the specified object.
    /// </summary>
    /// <param name="element">
    /// Object to set the value on.
    /// </param>
    /// <param name="value">
    /// Value to set.
    /// </param>
    public static void SetPlacement(DependencyObject element, PlacementMode value)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValueInternal(PlacementProperty, value);
    }
}