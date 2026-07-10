// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes the cells in a <see cref="GridView"/> to UI Automation.
/// </summary>
public class GridViewCellAutomationPeer : FrameworkElementAutomationPeer, ITableItemProvider
{
    ///
    internal GridViewCellAutomationPeer(ContentPresenter owner, ListViewAutomationPeer parent)
        : base(owner)
    {
        Debug.Assert(parent != null);
        _listviewAP = parent;
    }

    ///
    internal GridViewCellAutomationPeer(TextBlock owner, ListViewAutomationPeer parent)
        : base(owner)
    {
        Debug.Assert(parent != null);
        _listviewAP = parent;
    }

    /// <summary>
    /// Gets the name of the element that is associated with this <see cref="GridViewCellAutomationPeer"/>.
    /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
    /// </summary>
    /// <returns>
    /// The name of the element.
    /// </returns>
    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    /// <summary>
    /// Gets the control type for the element that is associated with this <see cref="GridViewCellAutomationPeer"/>.
    /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
    /// </summary>
    /// <returns>
    /// If this <see cref="GridViewCellAutomationPeer"/> is associated with a <see cref="TextBlock"/> element, this 
    /// method returns <see cref="AutomationControlType.Text"/>; otherwise, this method returns 
    /// <see cref="AutomationControlType.Custom"/>.
    /// </returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        if (Owner is TextBlock)
        {
            return AutomationControlType.Text;
        }
        else
        {
            return AutomationControlType.Custom;
        }
    }

    /// <summary>
    /// Gets the control pattern for the element that is associated with this <see cref="GridViewCellAutomationPeer"/>.
    /// </summary>
    /// <param name="patternInterface">
    /// One of the enumeration values.
    /// </param>
    /// <returns>
    /// If patternInterface is <see cref="PatternInterface.GridItem"/> or <see cref="PatternInterface.TableItem"/>, this 
    /// method returns the current <see cref="GridViewCellAutomationPeer"/>.
    /// </returns>
    public override object GetPattern(PatternInterface patternInterface)
    {
        if (patternInterface == PatternInterface.GridItem || patternInterface == PatternInterface.TableItem)
        {
            return this;
        }

        return base.GetPattern(patternInterface);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the element that is associated with this <see cref="GridViewCellAutomationPeer"/>
    /// is understood by the end user as interactive or the user might understand the element as contributing to the logical 
    /// structure of the control in the GUI. This method is called by <see cref="AutomationPeer.IsControlElement"/>.
    /// </summary>
    /// <returns>
    /// If this <see cref="GridViewCellAutomationPeer"/> is associated with a <see cref="TextBlock"/> element, this method 
    /// returns true; otherwise, this method returns a list of child elements.
    /// </returns>
    protected override bool IsControlElementCore()
    {
        if (Owner is TextBlock)
        {
            // We only want this peer to show up in the Control view if it is visible
            // For compat we allow falling back to legacy behavior (returning true always)
            // based on AppContext flags, IncludeInvisibleElementsInControlView evaluates them.
            return Owner.IsVisible;
        }
        else
        {
            List<AutomationPeer> children = GetChildrenAutomationPeer(Owner, false);
            return children != null && children.Count >= 1;
        }
    }

    internal int Column { get; set; }

    internal int Row { get; set; }

    #region ITableItem

    IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems()
    {
        //If there are no row headers, return an empty array 
        return [];
    }

    IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
    {
        ListView listview = _listviewAP.Owner as ListView;
        if (listview != null && listview.View is GridView)
        {
            GridView gridview = listview.View as GridView;
            if (gridview.HeaderRowPresenter != null && gridview.HeaderRowPresenter.ActualColumnHeaders.Count > Column)
            {
                GridViewColumnHeader header = gridview.HeaderRowPresenter.ActualColumnHeaders[Column];
                AutomationPeer peer = FromElement(header);
                if (peer != null)
                {
                    return [ProviderFromPeer(peer)];
                }
            }
        }
        return [];
    }

    #endregion

    #region IGridItem

    int IGridItemProvider.Row { get { return Row; } }
    int IGridItemProvider.Column { get { return Column; } }
    int IGridItemProvider.RowSpan { get { return 1; } }
    int IGridItemProvider.ColumnSpan { get { return 1; } }
    IRawElementProviderSimple IGridItemProvider.ContainingGrid { get { return ProviderFromPeer(_listviewAP); } }

    #endregion


    #region Private Methods

    /// <summary>
    /// Get the children of the parent which has automation peer
    /// </summary>
    private List<AutomationPeer> GetChildrenAutomationPeer(UIElement parent, bool includeInvisibleItems)
    {
        Debug.Assert(parent != null);

        List<AutomationPeer> children = null;

        Iterate(parent, includeInvisibleItems,
                delegate (AutomationPeer peer)
                {
                    if (children == null)
                        children = new List<AutomationPeer>();

                    children.Add(peer);
                    return false;
                });

        return children;
    }

    private delegate bool IteratorCallback(AutomationPeer peer);

    //
    private static bool Iterate(UIElement parent, bool includeInvisibleItems, IteratorCallback callback)
    {
        bool done = false;

        AutomationPeer peer = null;

        int count = parent.InternalVisualChildrenCount;
        for (int i = 0; i < count && !done; i++)
        {
            UIElement child = parent.InternalGetVisualChild(i);
            if (child != null
                && child.ReadVisualFlag(VisualFlags.IsUIElement)
                && (includeInvisibleItems || child.IsVisible)
                && (peer = CreatePeerForElement(child)) != null)
            {
                done = callback(peer);
            }
            else
            {
                done = Iterate(child, includeInvisibleItems, callback);
            }
        }

        return done;
    }

    #endregion

    #region Private Fields

    private readonly ListViewAutomationPeer _listviewAP;

    #endregion
}