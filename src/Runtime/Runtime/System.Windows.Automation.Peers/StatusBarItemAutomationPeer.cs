// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Controls.Primitives;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes <see cref="StatusBarItem"/> types to UI Automation.
/// </summary>
public class StatusBarItemAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatusBarItemAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="StatusBarItem"/> that is associated with this <see cref="StatusBarItemAutomationPeer"/>.
    /// </param>
    public StatusBarItemAutomationPeer(StatusBarItem owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Gets the name of the <see cref="StatusBarItem"/> that is associated with this <see cref="StatusBarItemAutomationPeer"/>.
    /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
    /// </summary>
    /// <returns>
    /// A string that contains "StatusBarItem".
    /// </returns>
    protected override string GetClassNameCore() => "StatusBarItem";

    /// <summary>
    /// Gets the control type for the <see cref="StatusBarItem"/> that is associated with this <see cref="StatusBarItemAutomationPeer"/>.
    /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="AutomationControlType.Text"/> enumeration value.
    /// </returns>
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;
}