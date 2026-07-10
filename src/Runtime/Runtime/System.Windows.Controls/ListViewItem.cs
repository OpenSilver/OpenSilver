// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Controls;

/// <summary>
/// Represents an item in a <see cref="ListView"/> control.
/// </summary>
public class ListViewItem : ListBoxItem
{
    // NOTE: ListViewItem has no default theme style. It uses ThemeStyleKey 
    // to find default style for different view.

    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewItem"/> class.
    /// </summary>
    public ListViewItem() { }

    // helper to set DefaultStyleKey of ListViewItem
    internal void SetDefaultStyleKey(object key) => DefaultStyleKey = key;

    //  helper to clear DefaultStyleKey of ListViewItem
    internal void ClearDefaultStyleKey() => ClearValue(DefaultStyleKeyProperty);
}
