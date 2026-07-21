// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Controls;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes <see cref="ToolBar"/> types to UI Automation.
/// </summary>
public class ToolBarAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBarAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="ToolBar"/> that is associated with this <see cref="ToolBarAutomationPeer"/>.
    /// </param>
    public ToolBarAutomationPeer(ToolBar owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Gets the name of the <see cref="ToolBar"/> that is associated with this <see cref="ToolBarAutomationPeer"/>.
    /// Called by <see cref="AutomationPeer.GetClassName"/>.
    /// </summary>
    /// <returns>
    /// A string that contains the word "ToolBar".
    /// </returns>
    protected override string GetClassNameCore() => "ToolBar";

    /// <summary>
    /// Gets the control type for the <see cref="ToolBar"/> that is associated with this <see cref="ToolBarAutomationPeer"/>.
    /// Called by <see cref="AutomationPeer.GetAutomationControlType"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="ToolBar"/> enumeration value.
    /// </returns>
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ToolBar;
}