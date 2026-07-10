// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Allows a customized view of a <see cref="ListView"/> that derives from <see cref="ViewBase"/> to 
/// implement automation peer features that are specific to the custom view.
/// </summary>
public interface IViewAutomationPeer
{
    /// <summary>
    /// Gets the control type for the element that is associated with this <see cref="IViewAutomationPeer"/>.
    /// </summary>
    /// <returns>
    /// A value in the <see cref="AutomationControlType"/> enumeration.
    /// </returns>
    AutomationControlType GetAutomationControlType();

    /// <summary>
    /// Gets the control pattern that is associated with the specified patternInterface.
    /// </summary>
    /// <param name="patternInterface">
    /// A value in the enumeration.
    /// </param>
    /// <returns>
    /// Return the object that implements the control pattern. If this method returns null, the return value 
    /// from <see cref="GetPattern(PatternInterface)"/> is used.
    /// </returns>
    object GetPattern(PatternInterface patternInterface);

    /// <summary>
    /// Gets the collection of immediate child elements of the specified UI Automation peer.
    /// </summary>
    /// <param name="children">
    /// The automation peers for the list items.
    /// </param>
    /// <returns>
    /// The automation peers for all items in the control. If the view contains interactive or informational 
    /// elements in addition to the list items, automation peers for these elements must be added to the list.
    /// </returns>
    List<AutomationPeer> GetChildren(List<AutomationPeer> children);

    /// <summary>
    /// Creates a new instance of the <see cref="ItemAutomationPeer"/> class.
    /// </summary>
    /// <param name="item">
    /// The <see cref="ListViewItem"/> that is associated with the <see cref="ListView"/> that is used 
    /// by this <see cref="IViewAutomationPeer"/>.
    /// </param>
    /// <returns>
    /// The new <see cref="ItemAutomationPeer"/> instance.
    /// </returns>
    ItemAutomationPeer CreateItemAutomationPeer(object item);

    //Note: The following two reasons explain why we need the ItemsChanged method
    //      1 View must know when Items has been changed in order to fire event when IGridProvider.RowCount is changed
    //      2 ItemsControl doesn't fire a ItemsChanged event, the only way to do this is to override the OnItemsChanged event
    /// <summary>
    /// Called by <see cref="ListView"/> when the collection of items changes.
    /// </summary>
    /// <param name="e">
    /// A <see cref="NotifyCollectionChangedEventArgs"/> that contains the event data.
    /// </param>
    void ItemsChanged(NotifyCollectionChangedEventArgs e);

    /// <summary>
    /// Called when the custom view is no longer applied to the <see cref="ListView"/>.
    /// </summary>
    void ViewDetached();
}
