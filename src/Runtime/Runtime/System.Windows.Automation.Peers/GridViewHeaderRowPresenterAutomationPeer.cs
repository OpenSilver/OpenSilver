// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes <see cref="GridViewHeaderRowPresenter"/> types to UI Automation.
/// </summary>
public class GridViewHeaderRowPresenterAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridViewHeaderRowPresenterAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="GridViewHeaderRowPresenter"/> that is associated with this <see cref="GridViewHeaderRowPresenterAutomationPeer"/>.
    /// </param>
    public GridViewHeaderRowPresenterAutomationPeer(GridViewHeaderRowPresenter owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Gets the name of the <see cref="GridViewHeaderRowPresenter"/> that is associated with this 
    /// <see cref="GridViewHeaderRowPresenterAutomationPeer"/>. This method is called by <see cref="AutomationPeer.GetClassName"/>.
    /// </summary>
    /// <returns>
    /// A string that contains "GridViewHeaderRowPresenter".
    /// </returns>
    protected override string GetClassNameCore()
    {
        return "GridViewHeaderRowPresenter";
    }

    /// <summary>
    /// Gets the control type for the <see cref="GridViewHeaderRowPresenter"/> that is associated with this 
    /// <see cref="GridViewHeaderRowPresenterAutomationPeer"/>. This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="AutomationControlType.Header"/> enumeration value.
    /// </returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Header;
    }

    // AutomationControlType.Header must return IsContentElement false.
    // See http://msdn.microsoft.com/en-us/library/ms753110.aspx
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
    /// Gets the collection of child elements of the <see cref="GridViewHeaderRowPresenter"/> that is associated 
    /// with this <see cref="GridViewHeaderRowPresenterAutomationPeer"/>. Called by <see cref="AutomationPeer.GetChildren"/>.
    /// </summary>
    /// <returns>
    /// The collection of child elements.
    /// </returns>
    protected override List<AutomationPeer> GetChildrenCore()
    {
        List<AutomationPeer> list = base.GetChildrenCore();
        List<AutomationPeer> newList = null;
        if (list != null)
        {
            newList = new List<AutomationPeer>(list.Count);
            //GVHRP contains 2 extra column headers, one is dummy header, the other is floating header
            //We need to remove them from the tree
            foreach (AutomationPeer peer in list)
            {
                if (peer is FrameworkElementAutomationPeer)
                {
                    GridViewColumnHeader header = ((FrameworkElementAutomationPeer)peer).Owner as GridViewColumnHeader;
                    if (header != null && header.Role == GridViewColumnHeaderRole.Normal)
                    {
                        //Because GVHRP uses inverse sequence to store column headers, we need to use insert here
                        newList.Insert(0, peer);
                    }
                }
            }
        }
        return newList;
    }
}