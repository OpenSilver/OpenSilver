// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenSilver.Internal;
using System.Windows.Automation.Peers;

namespace System.Windows.Controls.Primitives;

/// <summary>
/// Represents an item of a <see cref="StatusBar"/> control.
/// </summary>
public class StatusBarItem : ContentControl
{
    static StatusBarItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBarItem), new FrameworkPropertyMetadata(typeof(StatusBarItem)));
        IsTabStopProperty.OverrideMetadata(typeof(StatusBarItem), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusBarItem"/> class.
    /// </summary>
    public StatusBarItem() { }

    /// <summary>
    /// Specifies an <see cref="AutomationPeer"/> for the <see cref="StatusBarItem"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="StatusBarItemAutomationPeer"/> for this <see cref="StatusBarItem"/>.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer() => new StatusBarItemAutomationPeer(this);
}
