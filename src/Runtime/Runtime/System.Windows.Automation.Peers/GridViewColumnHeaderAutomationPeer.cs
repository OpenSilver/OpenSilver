// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenSilver.Internal;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes <see cref="GridViewColumnHeader"/> types to UI Automation.
/// </summary>
public class GridViewColumnHeaderAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider, ITransformProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridViewColumnHeaderAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="GridViewColumnHeader"/> that is associated with this <see cref="GridViewColumnHeaderAutomationPeer"/>.
    /// </param>
    public GridViewColumnHeaderAutomationPeer(GridViewColumnHeader owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Gets the control type for the <see cref="GridViewColumnHeader"/> that is associated with this 
    /// <see cref="GridViewColumnHeaderAutomationPeer"/>. This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="AutomationControlType.HeaderItem"/> enumeration value.
    /// </returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.HeaderItem;
    }

    // AutomationControlType.HeaderItem must return IsContentElement false.
    // See http://msdn.microsoft.com/en-us/library/ms742202.aspx
    /// <summary>
    /// Gets a value that indicates whether the element that is associated with this automation peer contains 
    /// data that is presented to the user. This method is called by <see cref="AutomationPeer.IsContentElement"/>.
    /// </summary>
    /// <returns>
    /// false in all cases.
    /// </returns>
    protected override bool IsContentElementCore()
    {
        return false;
    }

    /// <summary>
    /// Gets the name of the <see cref="GridViewColumnHeader"/> that is associated with this 
    /// <see cref="GridViewColumnHeaderAutomationPeer"/>. This method is called by <see cref="AutomationPeer.GetClassName"/>.
    /// </summary>
    /// <returns></returns>
    protected override string GetClassNameCore()
    {
        return "GridViewColumnHeader";
    }

    /// <summary>
    /// Gets the control pattern for the <see cref="GridViewColumnHeader"/> that is associated with this
    /// <see cref="GridViewColumnHeaderAutomationPeer"/>.
    /// </summary>
    /// <param name="patternInterface">
    /// One of the enumeration values.
    /// </param>
    /// <returns>
    /// If patternInterface is <see cref="PatternInterface.Transform"/> or <see cref="PatternInterface.Invoke"/>, 
    /// this method returns a this pointer; otherwise this method returns null.
    /// </returns>
    public override object GetPattern(PatternInterface patternInterface)
    {
        if (patternInterface == PatternInterface.Invoke || patternInterface == PatternInterface.Transform)
        {
            return this;
        }
        else
        {
            return base.GetPattern(patternInterface);
        }
    }

    void IInvokeProvider.Invoke()
    {
        if (!IsEnabled())
            throw new ElementNotEnabledException();

        GridViewColumnHeader owner = (GridViewColumnHeader)Owner;
        owner.AutomationClick();
    }

    #region ITransformProvider

    bool ITransformProvider.CanMove { get { return false; } }

    //Note: CanResize can be false if Max/MinWidth,Height has been added on GridViewColumn/ColumnHeader
    bool ITransformProvider.CanResize { get { return true; } }
    bool ITransformProvider.CanRotate { get { return false; } }

    //Note: Don't support Move so far, if users do need this feature to reorder columns, 
    //we can consider to add it later. (One concern is GVCH doesn't support reorder by moving itself)
    void ITransformProvider.Move(double x, double y)
    {
        throw new InvalidOperationException(Strings.UIA_OperationCannotBePerformed);
    }

    void ITransformProvider.Resize(double width, double height)
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);

        GridViewColumnHeader header = Owner as GridViewColumnHeader;
        if (header != null)
        {
            header.Column?.Width = width;

            header.Height = height;
        }
    }

    void ITransformProvider.Rotate(double degrees)
    {
        throw new InvalidOperationException(Strings.UIA_OperationCannotBePerformed);
    }

    #endregion
}