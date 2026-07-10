// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Automation.Peers;

namespace System.Windows.Controls;

/// <summary>
/// Represents the base class for views that define the appearance of data in a <see cref="ListView"/> control.
/// </summary>
public abstract class ViewBase : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewBase"/> class.
    /// </summary>
    protected ViewBase() { }

    #region Protected Methods

    /// <summary>
    /// Prepares an item in the view for display, by setting bindings and styles.
    /// </summary>
    /// <param name="item">
    /// The item to prepare for display.
    /// </param>
    protected internal virtual void PrepareItem(ListViewItem item) { }

    /// <summary>
    /// Removes all bindings and styling that are set for an item.
    /// </summary>
    /// <param name="item">
    /// The <see cref="ListViewItem"/> to remove settings from.
    /// </param>
    protected internal virtual void ClearItem(ListViewItem item) { }

    /// <summary>
    /// Gets the object that is associated with the style for the view mode.
    /// </summary>
    /// <returns>
    /// The style to use for the view mode. The default value is the style for the <see cref="ListBox"/>.
    /// </returns>
    protected internal virtual object DefaultStyleKey => typeof(ListBox);

    /// <summary>
    /// Gets the style to use for the items in the view mode.
    /// </summary>
    /// <returns>
    /// The style of a <see cref="ListViewItem"/>. The default value is the style for the <see cref="ListBoxItem"/> 
    /// control.
    /// </returns>
    protected internal virtual object ItemContainerDefaultStyleKey => typeof(ListBoxItem);

    // Propagate theme changes to contained headers
    internal virtual void OnThemeChanged() { }

    #endregion

    /// <summary>
    /// Is called when a <see cref="ListView"/> control creates a <see cref="ListViewAutomationPeer"/> for its 
    /// <see cref="ListView.View"/>.
    /// </summary>
    /// <param name="parent">
    /// The <see cref="ListView"/> control to use to create the <see cref="ListViewAutomationPeer"/>.
    /// </param>
    /// <returns>
    /// The <see cref="IViewAutomationPeer"/> interface that implements the <see cref="ListViewAutomationPeer"/> 
    /// for a custom <see cref="ListView.View"/>.
    /// </returns>
    protected internal virtual IViewAutomationPeer GetAutomationPeer(ListView parent) => null;

    // True, when view is assigned to a ListView.
    internal bool IsUsed { get; set; }
}
