// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenSilver.Internal;
using System.Collections.Specialized;
using System.Windows.Automation.Peers;

namespace System.Windows.Controls;

/// <summary>
/// Represents a control that displays a list of data items.
/// </summary>
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(ListViewItem))]
public class ListView : ListBox
{
    //-------------------------------------------------------------------
    //
    //  Constructors
    //
    //-------------------------------------------------------------------

    #region Constructors

    static ListView()
    {
        SelectionModeProperty.OverrideMetadata(typeof(ListView), new FrameworkPropertyMetadata(SelectionMode.Extended));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListView"/> class.
    /// </summary>
    public ListView() { }

    #endregion Constructors

    //-------------------------------------------------------------------
    //
    //  Public Methods
    //
    //-------------------------------------------------------------------

    //-------------------------------------------------------------------
    //
    //  Public Properties
    //
    //-------------------------------------------------------------------

    #region Public Properties

    /// <summary>
    /// Identifies the <see cref="View"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewProperty =
        DependencyProperty.Register(
            nameof(View),
            typeof(ViewBase),
            typeof(ListView),
            new PropertyMetadata(OnViewChanged));

    /// <summary>
    /// Gets or sets an object that defines how the data is styled and organized in a <see cref="ListView"/> control.
    /// </summary>
    /// <returns>
    /// A <see cref="ViewBase"/> object that specifies how to display information in the <see cref="ListView"/>.
    /// </returns>
    public ViewBase View
    {
        get => (ViewBase)GetValue(ViewProperty);
        set => SetValueInternal(ViewProperty, value);
    }

    private static void OnViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ListView listView = (ListView)d;

        ViewBase oldView = (ViewBase)e.OldValue;
        ViewBase newView = (ViewBase)e.NewValue;
        if (newView != null)
        {
            if (newView.IsUsed)
            {
                throw new InvalidOperationException(Strings.ListView_ViewCannotBeShared);
            }
            newView.IsUsed = true;
        }

        // In ApplyNewView ListView.ClearContainerForItemOverride will be called for each item.
        // Should use old view to do clear item.
        listView._previousView = oldView;
        listView.ApplyNewView();
        // After ApplyNewView, if item is removed, ListView.ClearContainerForItemOverride will be called.
        // Then should use new view to do clear item.
        listView._previousView = newView;

        //Switch ViewAutomationPeer in ListViewAutomationPeer
        ListViewAutomationPeer lvPeer = FrameworkElementAutomationPeer.FromElement(listView) as ListViewAutomationPeer;
        if (lvPeer != null)
        {
            lvPeer.ViewAutomationPeer?.ViewDetached();

            if (newView != null)
            {
                lvPeer.ViewAutomationPeer = newView.GetAutomationPeer(listView);
            }
            else
            {
                lvPeer.ViewAutomationPeer = null;
            }
            //Invalidate the ListView automation tree because the view has been changed
            lvPeer.InvalidatePeer();
        }

        oldView?.IsUsed = false;
    }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Protected Methods
    //
    //-------------------------------------------------------------------

    #region Protected Methods

    /// <summary>
    /// Sets the styles, templates, and bindings for a <see cref="ListViewItem"/>.
    /// </summary>
    /// <param name="element">
    /// An object that is a <see cref="ListViewItem"/> or that can be converted into one.
    /// </param>
    /// <param name="item">
    /// The object to use to create the <see cref="ListViewItem"/>.
    /// </param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        ListViewItem lvi = element as ListViewItem;
        if (lvi != null)
        {
            ViewBase view = View;
            if (view != null)
            {
                // update default style key
                lvi.SetDefaultStyleKey(view.ItemContainerDefaultStyleKey);
                view.PrepareItem(lvi);
            }
            else
            {
                lvi.ClearDefaultStyleKey();
            }
        }
    }

    /// <summary>
    /// Removes all templates, styles, and bindings for the object that is displayed as a <see cref="ListViewItem"/>.
    /// </summary>
    /// <param name="element">
    /// The <see cref="ListViewItem"/> container to clear.
    /// </param>
    /// <param name="item">
    /// The object that the <see cref="ListViewItem"/> contains.
    /// </param>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
        // This method no longer does the work it used to (bug 1445288).
        // It is called when a container is removed from the tree;  such a
        // container will be GC'd soon, so there's no point in changing
        // its properties.

        base.ClearContainerForItemOverride(element, item);
    }

    /// <summary>
    /// Determines whether an object is a <see cref="ListViewItem"/>.
    /// </summary>
    /// <param name="item">
    /// The object to evaluate.
    /// </param>
    /// <returns>
    /// true if the item is a <see cref="ListViewItem"/>; otherwise, false.
    /// </returns>
    protected override bool IsItemItsOwnContainerOverride(object item) => item is ListViewItem;

    /// <summary>
    /// Creates and returns a new <see cref="ListViewItem"/> container.
    /// </summary>
    /// <returns>
    /// A new <see cref="ListViewItem"/> control.
    /// </returns>
    protected override DependencyObject GetContainerForItemOverride() => new ListViewItem();

    /// <summary>
    /// Responds to an <see cref="ItemsControl.OnItemsChanged(NotifyCollectionChangedEventArgs)"/>.
    /// </summary>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);

        ListViewAutomationPeer lvPeer = FrameworkElementAutomationPeer.FromElement(this) as ListViewAutomationPeer;
        if (lvPeer != null && lvPeer.ViewAutomationPeer != null)
        {
            lvPeer.ViewAutomationPeer.ItemsChanged(e);
        }
    }

    #endregion // Protected Methods

    //-------------------------------------------------------------------
    //
    //  Accessibility
    //
    //-------------------------------------------------------------------

    #region Accessibility

    /// <summary>
    /// Defines an <see cref="AutomationPeer"/> for the <see cref="ListView"/> control.
    /// </summary>
    /// <returns>
    /// Returns a <see cref="ListViewAutomationPeer"/> object for the <see cref="ListView"/> control.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        ListViewAutomationPeer lvPeer = new ListViewAutomationPeer(this);
        if (lvPeer != null && View != null)
        {
            lvPeer.ViewAutomationPeer = View.GetAutomationPeer(this);
        }

        return lvPeer;
    }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Private Methods
    //
    //-------------------------------------------------------------------

    #region Private Methods

    // apply styles described in View.
    private void ApplyNewView()
    {
        ViewBase newView = View;

        if (newView != null)
        {
            // update default style key of ListView
            DefaultStyleKey = newView.DefaultStyleKey;
        }
        else
        {
            ClearValue(DefaultStyleKeyProperty);
        }

        // Encounter a new view after loaded means user is switching view.
        // Force to regenerate all containers.
        if (IsLoaded)
        {
            ItemContainerGenerator.Refresh();
        }
    }

    // Invalidate resources on the view header if the header isn't
    // reachable via the visual/logical tree
    internal override void OnThemeChanged()
    {
        // If the ListView does not have a template generated tree then its
        // View.Header is not reachable via a tree walk.
        if (!HasTemplateGeneratedSubTree && View != null)
        {
            View.OnThemeChanged();
        }
    }

    #endregion Private Methods

    //-------------------------------------------------------------------
    //
    //  Private Fields
    //
    //-------------------------------------------------------------------

    private ViewBase _previousView;
}
