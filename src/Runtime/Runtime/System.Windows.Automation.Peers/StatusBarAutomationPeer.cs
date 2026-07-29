// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes <see cref="StatusBar"/> types to UI Automation.
/// </summary>
public class StatusBarAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StatusBarAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="StatusBar"/> that is associated with this <see cref="StatusBarAutomationPeer"/>.
    /// </param>
    public StatusBarAutomationPeer(StatusBar owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Gets the name of the <see cref="StatusBar"/> that is associated with this <see cref="StatusBarAutomationPeer"/>.
    /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
    /// </summary>
    /// <returns>
    /// A string that contains "StatusBar".
    /// </returns>
    protected override string GetClassNameCore() => "StatusBar";

    /// <summary>
    /// Gets the control type for the <see cref="StatusBar"/> that is associated with this <see cref="StatusBarAutomationPeer"/>.
    /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="AutomationControlType.StatusBar"/> enumeration value.
    /// </returns>
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.StatusBar;

    /// <summary>
    /// Gets the collection of child elements of the <see cref="StatusBar"/> that is associated with this 
    /// <see cref="StatusBarAutomationPeer"/>. This method is called by <see cref="AutomationPeer.GetChildren"/>.
    /// </summary>
    /// <returns>
    /// A list of child elements.
    /// </returns>    
    protected override List<AutomationPeer> GetChildrenCore()
    {
        List<AutomationPeer> list = [];
        if (Owner is ItemsControl itemscontrol)
        {
            foreach (object obj in itemscontrol.Items)
            {
                if (obj is Separator separator)
                {
                    list.Add(CreatePeerForElement(separator));
                }
                else
                {
                    if (itemscontrol.ItemContainerGenerator.ContainerFromItem(obj) is StatusBarItem item)
                    {
                        // If the item is a string or TextBlock or StatusBarItem
                        // StatusBarItemAutomationPeer will be created to show the text
                        // Or we'll use the control's automation peer
                        if (obj is string || obj is TextBlock || (obj is StatusBarItem statusBarItem && statusBarItem.Content is string))
                        {
                            list.Add(CreatePeerForElement(item));
                        }
                        else
                        {
                            List<AutomationPeer> childList = GetChildrenAutomationPeer(item);
                            if (childList != null)
                            {
                                foreach (AutomationPeer ap in childList)
                                {
                                    list.Add(ap);
                                }
                            }
                        }
                    }
                }
            }
        }

        return list;
    }


    /// <summary>
    /// Get the children of the parent which has automation peer
    /// </summary>
    private List<AutomationPeer> GetChildrenAutomationPeer(UIElement parent)
    {
        Debug.Assert(parent is not null);

        List<AutomationPeer> children = null;

        Iterate(parent,
                delegate (AutomationPeer peer)
                {
                    children ??= [];
                    children.Add(peer);
                    return false;
                });

        return children;
    }

    private delegate bool IteratorCallback(AutomationPeer peer);

    private static bool Iterate(UIElement parent, IteratorCallback callback)
    {
        bool done = false;

        AutomationPeer peer = null;

        int count = parent.InternalVisualChildrenCount;
        for (int i = 0; i < count && !done; i++)
        {
            UIElement child = parent.InternalGetVisualChild(i);
            if (child != null
                && child.ReadVisualFlag(VisualFlags.IsUIElement)
                && (peer = CreatePeerForElement(child)) != null)
            {
                done = callback(peer);
            }
            else
            {
                done = Iterate(child, callback);
            }
        }

        return done;
    }
}