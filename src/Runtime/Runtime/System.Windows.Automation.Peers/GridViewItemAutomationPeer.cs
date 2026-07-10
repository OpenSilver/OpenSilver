// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes the data items in the collection of <see cref="ItemsControl.Items"/> in <see cref="GridView"/>
/// types to UI Automation.
/// </summary>
public class GridViewItemAutomationPeer : ListBoxItemAutomationPeer
{
    /// <summary>
    /// Creates a new instance of the <see cref="GridViewItemAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The data item that is associated with this <see cref="GridViewItemAutomationPeer"/>.
    /// </param>
    /// <param name="listviewAP">
    /// The <see cref="ListViewAutomationPeer"/> that is the parent of this <see cref="GridViewItemAutomationPeer"/>.
    /// </param>
    public GridViewItemAutomationPeer(object owner, ListViewAutomationPeer listviewAP)
        : base(owner, listviewAP)
    {
        Debug.Assert(listviewAP != null);

        _listviewAP = listviewAP;
    }

    /// <summary>
    /// Gets the name of the <see cref="ItemsControl.Items"/> collection that is associated with this 
    /// <see cref="GridViewItemAutomationPeer"/>. Called by <see cref="AutomationPeer.GetClassName"/>.
    /// </summary>
    /// <returns>
    /// A string that contains "ListViewItem".
    /// </returns>
    protected override string GetClassNameCore()
    {
        return "ListViewItem";
    }

    /// <summary>
    /// Gets the control type for the <see cref="ItemsControl.Items"/> collection that is associated with this
    /// <see cref="GridViewItemAutomationPeer"/>. Called by <see cref="AutomationPeer.GetAutomationControlType"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="AutomationControlType.DataItem"/> enumeration value.
    /// </returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.DataItem;
    }

    /// <summary>
    /// Gets the collection of child elements of the <see cref="ItemsControl.Items"/> collection that is associated 
    /// with this <see cref="GridViewItemAutomationPeer"/>. Called by <see cref="AutomationPeer.GetChildren"/>.
    /// </summary>
    /// <returns>
    /// The collection of child elements.
    /// </returns>
    protected override List<AutomationPeer> GetChildrenCore()
    {
        ListView listview = _listviewAP.Owner as ListView;
        Debug.Assert(listview != null);
        object item = Item;

        ListViewItem lvi = listview.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
        if (lvi != null)
        {
            GridViewRowPresenter rowPresenter = GridViewAutomationPeer.FindVisualByType(lvi, typeof(GridViewRowPresenter)) as GridViewRowPresenter;
            if (rowPresenter != null)
            {
                Hashtable oldChildren = _dataChildren; //cache the old ones for possible reuse
                _dataChildren = new Hashtable(rowPresenter.ActualCells.Count);

                List<AutomationPeer> list = new List<AutomationPeer>();
                int row = listview.ItemContainerGenerator.IndexFromContainer(lvi);
                int column = 0;

                foreach (UIElement ele in rowPresenter.ActualCells)
                {
                    GridViewCellAutomationPeer peer = (oldChildren == null ? null : (GridViewCellAutomationPeer)oldChildren[ele]);
                    if (peer == null)
                    {
                        if (ele is ContentPresenter)
                        {
                            peer = new GridViewCellAutomationPeer((ContentPresenter)ele, _listviewAP);
                        }
                        else if (ele is TextBlock)
                        {
                            peer = new GridViewCellAutomationPeer((TextBlock)ele, _listviewAP);
                        }
                        else
                        {
                            Debug.Assert(false, "Children of GridViewRowPresenter should be ContentPresenter or TextBlock");
                        }
                    }

                    //protection from indistinguishable UIElement - for example, 2 UIElement wiht same value
                    if (_dataChildren[ele] == null)
                    {
                        //Set Cell's row and column
                        peer.Column = column;
                        peer.Row = row;
                        list.Add(peer);
                        _dataChildren.Add(ele, peer);
                        column++;
                    }
                }
                return list;
            }
        }

        return null;
    }

    #region Private Fields

    private ListViewAutomationPeer _listviewAP;
    private Hashtable _dataChildren = null;

    #endregion
}
