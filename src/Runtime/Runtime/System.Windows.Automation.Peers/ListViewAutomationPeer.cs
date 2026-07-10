// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes <see cref="ListView"/> types to UI Automation.
/// </summary>
public class ListViewAutomationPeer : ListBoxAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListViewAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="ListView"/> that is associated with this <see cref="ListViewAutomationPeer"/>.
    /// </param>
    public ListViewAutomationPeer(ListView owner)
        : base(owner)
    {
        Debug.Assert(owner != null);
    }

    /// <summary>
    /// Gets the control type for the <see cref="ListView"/> that is associated with this 
    /// <see cref="ListViewAutomationPeer"/>. This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="AutomationControlType.List"/> enumeration value.
    /// </returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        if (_viewAutomationPeer != null)
        {
            return _viewAutomationPeer.GetAutomationControlType();
        }
        else
        {
            return base.GetAutomationControlTypeCore();
        }
    }

    /// <summary>
    /// Gets the name of the <see cref="ListView"/> that is associated with this <see cref="ListViewAutomationPeer"/>.
    /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
    /// </summary>
    /// <returns>
    /// A string that contains "ListView".
    /// </returns>
    protected override string GetClassNameCore()
    {
        return "ListView";
    }

    /// <summary>
    /// Gets the control pattern for the <see cref="ListView"/> that is associated with this 
    /// <see cref="ListViewAutomationPeer"/>.
    /// </summary>
    /// <param name="patternInterface">
    /// A value in the enumeration.
    /// </param>
    /// <returns>
    /// An <see cref="AutomationPeer"/> for the view that this <see cref="ListView"/> is using.
    /// Default <see cref="ListView"/> implementation uses the <see cref="GridView"/>, and this 
    /// method returns <see cref="GridViewAutomationPeer"/>.
    /// </returns>
    public override object GetPattern(PatternInterface patternInterface)
    {
        object ret = null;
        if (_viewAutomationPeer != null)
        {
            ret = _viewAutomationPeer.GetPattern(patternInterface);
            if (ret != null)
            {
                return ret;
            }
        }

        return base.GetPattern(patternInterface);
    }

    /// <summary>
    /// Gets the collection of child elements of the <see cref="ListView"/> that is associated with 
    /// this <see cref="ListViewAutomationPeer"/>. This method is called by <see cref="AutomationPeer.GetChildren"/>.
    /// </summary>
    /// <returns>
    /// The collection of child elements.
    /// </returns>
    protected override List<AutomationPeer> GetChildrenCore()
    {
        if (_refreshItemPeers)
        {
            _refreshItemPeers = false;
        }

        List<AutomationPeer> ret = base.GetChildrenCore();

        if (_viewAutomationPeer != null)
        {
            //If a custom view doesn't want to implement GetChildren details
            //just return null, we'll use the base.GetChildren as the return value
            ret = _viewAutomationPeer.GetChildren(ret);
        }

        return ret;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ItemAutomationPeer"/> class.
    /// </summary>
    /// <param name="item">
    /// The <see cref="ListViewItem"/> that is associated with this <see cref="ListViewAutomationPeer"/>.
    /// </param>
    /// <returns>
    /// The <see cref="ItemAutomationPeer"/> instance that is associated with this 
    /// <see cref="ListViewAutomationPeer"/>.
    /// </returns>
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
    {
        return _viewAutomationPeer == null ? base.CreateItemAutomationPeer(item) : _viewAutomationPeer.CreateItemAutomationPeer(item);
    }

    /// <summary>
    /// Gets the <see cref="IViewAutomationPeer"/> for this <see cref="ListViewAutomationPeer"/>.
    /// </summary>
    /// <returns>
    /// The interface instance that is associated with this <see cref="ListViewAutomationPeer"/>.
    /// </returns>
    protected internal IViewAutomationPeer ViewAutomationPeer
    {
        // Note: see bug 1555137 for details.
        // Never inline, as we don't want to unnecessarily link the 
        // automation DLL via the ISelectionProvider interface type initialization.
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        get { return _viewAutomationPeer; }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        set
        {
            if (_viewAutomationPeer != value)
            {
                _refreshItemPeers = true;
            }
            _viewAutomationPeer = value;
        }
    }

    #region Private Fields

    private bool _refreshItemPeers = false;
    private IViewAutomationPeer _viewAutomationPeer;

    #endregion
}
