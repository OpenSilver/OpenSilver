// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenSilver.Internal;
using System.ComponentModel;
using System.Threading;
using System.Windows.Automation.Peers;
using System.Windows.Markup;

namespace System.Windows.Controls;

/// <summary>
/// Represents a view mode that displays data items in columns for a <see cref="ListView"/> control.
/// </summary>
[StyleTypedProperty(Property = nameof(ColumnHeaderContainerStyle), StyleTargetType = typeof(GridViewColumnHeader))]
[ContentProperty(nameof(Columns))]
public class GridView : ViewBase
{
    //-------------------------------------------------------------------
    //
    //  Constructors
    //
    //-------------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="GridView"/> class.
    /// </summary>
    public GridView() { }

    //-------------------------------------------------------------------
    //
    //  Public Methods
    //
    //-------------------------------------------------------------------

    #region Public Methods

    /// <summary>
    /// Adds a <see cref="GridViewColumn"/> object to a <see cref="GridView"/>.
    /// </summary>
    /// <param name="column">
    /// The column to add.
    /// </param>
    protected virtual void AddChild(object column)
    {
        GridViewColumn c = column as GridViewColumn;

        if (c != null)
        {
            Columns.Add(c);
        }
        else
        {
            throw new InvalidOperationException(Strings.ListView_IllegalChildrenType);
        }
    }

    /// <summary>
    /// Not supported.
    /// </summary>
    /// <param name="text">
    /// Text string.
    /// </param>
    protected virtual void AddText(string text)
    {
        AddChild(text);
    }

    /// <summary>
    /// Returns the string representation of the <see cref="GridView"/> object.
    /// </summary>
    /// <returns>
    /// A string that indicates the number of columns in the <see cref="GridView"/>.
    /// </returns>
    public override string ToString()
    {
        return string.Format(Strings.ToStringFormatString_GridView, GetType(), Columns.Count);
    }

    /// <summary>
    /// Gets the <see cref="AutomationPeer"/> implementation for this <see cref="GridView"/> object.
    /// </summary>
    /// <param name="parent">
    /// The <see cref="ListView"/> control that implements this <see cref="GridView"/> view.
    /// </param>
    /// <returns>
    /// A <see cref="GridViewAutomationPeer"/> for this <see cref="GridView"/>.
    /// </returns>
    protected internal override IViewAutomationPeer GetAutomationPeer(ListView parent) => new GridViewAutomationPeer(this, parent);

    #endregion

    //-------------------------------------------------------------------
    //
    //  Public Properties
    //
    //-------------------------------------------------------------------

    #region Public Properties

    // For all the DPs on GridView, null is treated as unset,
    // because it's impossible to distinguish null and unset.
    // Change a property between null and unset, PropertyChangedCallback will not be called.

    // ----------------------------------------------------------------------------
    //  Defines the names of the resources to be consumed by the GridView style.
    //  Used to restyle several roles of GridView without having to restyle
    //  all of the control.
    // ----------------------------------------------------------------------------

    #region StyleKeys

    private static SystemThemeKey _gridViewScrollViewerStyleKey;
    private static SystemThemeKey _gridViewStyleKey;
    private static SystemThemeKey _gridViewItemContainerStyleKey;

    /// <summary>
    /// Gets the key that references the style that is defined for the <see cref="ScrollViewer"/> control 
    /// that encloses the content that is displayed by a <see cref="GridView"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ResourceKey"/> that references the <see cref="Style"/> that is applied to the 
    /// <see cref="ScrollViewer"/> control for a <see cref="GridView"/>. The default value is the style for 
    /// the <see cref="ScrollViewer"/> object of a <see cref="ListView"/> in the current theme.
    /// </returns>
    public static ResourceKey GridViewScrollViewerStyleKey
    {
        get
        {
            if (_gridViewScrollViewerStyleKey is null)
            {
                Interlocked.CompareExchange(ref _gridViewScrollViewerStyleKey, new SystemThemeKey(SystemResourceKeyID.GridViewScrollViewerStyle), null);
            }

            return _gridViewScrollViewerStyleKey;
        }
    }

    /// <summary>
    /// Gets the key that references the style that is defined for the <see cref="GridView"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ResourceKey"/> that references the <see cref="Style"/> that is applied to the 
    /// <see cref="GridView"/>. The default value is the style for the <see cref="ListView"/> in the 
    /// current theme.
    /// </returns>
    public static ResourceKey GridViewStyleKey
    {
        get
        {
            if (_gridViewStyleKey is null)
            {
                Interlocked.CompareExchange(ref _gridViewStyleKey, new SystemThemeKey(SystemResourceKeyID.GridViewStyle), null);
            }

            return _gridViewStyleKey;
        }
    }

    /// <summary>
    /// Gets the key that references the style that is defined for each <see cref="ListViewItem"/> in 
    /// a <see cref="GridView"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ResourceKey"/> that references the style for each <see cref="ListViewItem"/>.
    /// The default value references the default style for a <see cref="ListViewItem"/> control in the 
    /// current theme.
    /// </returns>
    public static ResourceKey GridViewItemContainerStyleKey
    {
        get
        {
            if (_gridViewItemContainerStyleKey is null)
            {
                Interlocked.CompareExchange(ref _gridViewItemContainerStyleKey, new SystemThemeKey(SystemResourceKeyID.GridViewItemContainerStyle), null);
            }

            return _gridViewItemContainerStyleKey;
        }
    }

    #endregion StyleKeys

    #region GridViewColumnCollection Attached DP

    /// <summary>
    /// Gets the contents of the <b>GridView.ColumnCollection</b> attached property.
    /// </summary>
    /// <param name="element">
    /// The <see cref="DependencyObject"/> that is associated with the collection.
    /// </param>
    /// <returns>
    /// The <see cref="GridViewColumnCollection"/> of the specified <see cref="DependencyObject"/>.
    /// </returns>
    public static GridViewColumnCollection GetColumnCollection(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (GridViewColumnCollection)element.GetValue(ColumnCollectionProperty);
    }

    /// <summary>
    /// Sets the contents of the <b>GridView.ColumnCollection</b> attached property.
    /// </summary>
    /// <param name="element">
    /// The <see cref="GridView"/> object.
    /// </param>
    /// <param name="collection">
    /// The <see cref="GridViewColumnCollection"/> object to assign.
    /// </param>
    public static void SetColumnCollection(DependencyObject element, GridViewColumnCollection collection)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValueInternal(ColumnCollectionProperty, collection);
    }

    /// <summary>
    /// Identifies the <b>GridView.ColumnCollectionProperty</b> attached property.
    /// </summary>
    public static readonly DependencyProperty ColumnCollectionProperty =
        DependencyProperty.RegisterAttached(
            "ColumnCollection",
            typeof(GridViewColumnCollection),
            typeof(GridView));

    /// <summary>
    /// Determines whether to serialize the <b>GridView.ColumnCollection</b> attached property.
    /// </summary>
    /// <param name="obj">
    /// The object on which the <b>GridView.ColumnCollection</b> is set.
    /// </param>
    /// <returns>
    /// true if the <b>GridView.ColumnCollection</b> must be serialized; otherwise, false.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool ShouldSerializeColumnCollection(DependencyObject obj)
    {
        ListViewItem listViewItem = obj as ListViewItem;
        if (listViewItem != null)
        {
            ListView listView = listViewItem.ParentSelector as ListView;
            if (listView != null)
            {
                GridView gridView = listView.View as GridView;
                if (gridView != null)
                {
                    // if GridViewColumnCollection attached on ListViewItem is Details.Columns, it should't be serialized.
                    GridViewColumnCollection localValue = listViewItem.ReadLocalValue(ColumnCollectionProperty) as GridViewColumnCollection;
                    return (localValue != gridView.Columns);
                }
            }
        }

        return true;
    }

    #endregion

    /// <summary>
    /// Gets the collection of <see cref="GridViewColumn"/> objects that is defined for this <see cref="GridView"/>.
    /// </summary>
    /// <returns>
    /// The collection of columns in the <see cref="GridView"/>. The default value is null.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public GridViewColumnCollection Columns
    {
        get
        {
            if (_columns == null)
            {
                _columns = new GridViewColumnCollection
                {
                    // Give the collection a back-link, this is used for the inheritance context
                    Owner = this,
                    InViewMode = true
                };
            }

            return _columns;
        }
    }

    #region ColumnHeaderContainerStyle

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderContainerStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderContainerStyleProperty =
        DependencyProperty.Register(
            nameof(ColumnHeaderContainerStyle),
            typeof(Style),
            typeof(GridView));

    /// <summary>
    /// Gets or sets the style to apply to column headers.
    /// </summary>
    /// <returns>
    /// The <see cref="Style"/> that is used to define the display properties for column headers. The default 
    /// value is null.
    /// </returns>
    public Style ColumnHeaderContainerStyle
    {
        get => (Style)GetValue(ColumnHeaderContainerStyleProperty);
        set => SetValueInternal(ColumnHeaderContainerStyleProperty, value);
    }

    #endregion // ColumnHeaderContainerStyle

    #region ColumnHeaderTemplate

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderTemplateProperty =
        DependencyProperty.Register(
            nameof(ColumnHeaderTemplate),
            typeof(DataTemplate),
            typeof(GridView),
            new FrameworkPropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets a template to use to display the column headers.
    /// </summary>
    /// <returns>
    /// The <see cref="DataTemplate"/> to use to display the column headers as part of the <see cref="GridView"/>.
    /// The default value is null.
    /// </returns>
    public DataTemplate ColumnHeaderTemplate
    {
        get => (DataTemplate)GetValue(ColumnHeaderTemplateProperty);
        set => SetValueInternal(ColumnHeaderTemplateProperty, value);
    }

    #endregion  ColumnHeaderTemplate

    #region ColumnHeaderTemplateSelector

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderTemplateSelector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty =
        DependencyProperty.Register(
            nameof(ColumnHeaderTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(GridView),
            new FrameworkPropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the selector object that provides logic for selecting a template to use for 
    /// each column header.
    /// </summary>
    /// <returns>
    /// The <see cref="DataTemplateSelector"/> object that determines the data template to use for 
    /// each column header. The default value is null.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataTemplateSelector ColumnHeaderTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(ColumnHeaderTemplateSelectorProperty);
        set => SetValueInternal(ColumnHeaderTemplateSelectorProperty, value);
    }

    #endregion ColumnHeaderTemplateSelector

    #region ColumnHeaderStringFormat

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderStringFormat"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderStringFormatProperty =
        DependencyProperty.Register(
            nameof(ColumnHeaderStringFormat),
            typeof(string),
            typeof(GridView));

    /// <summary>
    /// Gets or sets a composite string that specifies how to format the column headers of the 
    /// <see cref="GridView"/> if they are displayed as strings.
    /// </summary>
    /// <returns>
    /// A composite string that specifies how to format the column headers of the <see cref="GridView"/>
    /// if they are displayed as strings. The default is null.
    /// </returns>
    public string ColumnHeaderStringFormat
    {
        get => (string)GetValue(ColumnHeaderStringFormatProperty);
        set => SetValueInternal(ColumnHeaderStringFormatProperty, value);
    }

    #endregion  ColumnHeaderStringFormat

    #region AllowsColumnReorder

    /// <summary>
    /// Identifies the <see cref="AllowsColumnReorder"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowsColumnReorderProperty =
        DependencyProperty.Register(
            nameof(AllowsColumnReorder),
            typeof(bool),
            typeof(GridView),
            new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets whether columns in a <see cref="GridView"/> can be reordered by a drag-and-drop operation.
    /// </summary>
    /// <returns>
    /// true if columns can be reordered; otherwise, false. The default value is true.
    /// </returns>
    public bool AllowsColumnReorder
    {
        get => (bool)GetValue(AllowsColumnReorderProperty);
        set => SetValueInternal(AllowsColumnReorderProperty, value);
    }

    #endregion AllowsColumnReorder

    #region ColumnHeaderContextMenu

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderContextMenu"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderContextMenuProperty =
        DependencyProperty.Register(
            nameof(ColumnHeaderContextMenu),
            typeof(ContextMenu),
            typeof(GridView));

    /// <summary>
    /// Gets or sets a <see cref="ContextMenu"/> for the <see cref="GridView"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="ContextMenu"/> for the column headers in a <see cref="GridView"/>. The default value 
    /// is null.
    /// </returns>
    public ContextMenu ColumnHeaderContextMenu
    {
        get => (ContextMenu)GetValue(ColumnHeaderContextMenuProperty);
        set => SetValueInternal(ColumnHeaderContextMenuProperty, value);
    }

    #endregion ColumnHeaderContextMenu

    #region ColumnHeaderToolTip

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderToolTip"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderToolTipProperty =
        DependencyProperty.Register(
            nameof(ColumnHeaderToolTip),
            typeof(object),
            typeof(GridView));

    /// <summary>
    /// Gets or sets the content of a tooltip that appears when the mouse pointer pauses over one of 
    /// the column headers.
    /// </summary>
    /// <returns>
    /// An object that represents the content that appears as a tooltip when the mouse pointer is 
    /// paused over one of the column headers. The default value is not defined.
    /// </returns>
    public object ColumnHeaderToolTip
    {
        get => GetValue(ColumnHeaderToolTipProperty);
        set => SetValueInternal(ColumnHeaderToolTipProperty, value);
    }

    #endregion ColumnHeaderToolTip

    #endregion // Public Properties

    //-------------------------------------------------------------------
    //
    //  Protected Methods
    //
    //-------------------------------------------------------------------

    #region Protected Methods

    /// <summary>
    /// Prepares a <see cref="ListViewItem"/> for display according to the definition of this 
    /// <see cref="GridView"/> object.
    /// </summary>
    /// <param name="item">
    /// The <see cref="ListViewItem"/> to display.
    /// </param>
    protected internal override void PrepareItem(ListViewItem item)
    {
        base.PrepareItem(item);

        // attach GridViewColumnCollection to ListViewItem.
        SetColumnCollection(item, _columns);
    }

    /// <summary>
    /// Removes all settings, bindings, and styling from a <see cref="ListViewItem"/>.
    /// </summary>
    /// <param name="item">
    /// The <see cref="ListViewItem"/> to clear.
    /// </param>
    protected internal override void ClearItem(ListViewItem item)
    {
        item.ClearValue(ColumnCollectionProperty);

        base.ClearItem(item);
    }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Protected Properties
    //
    //-------------------------------------------------------------------

    #region Protected Properties

    /// <summary>
    /// Gets the reference for the default style for the <see cref="GridView"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="GridViewStyleKey"/>. The default value is the <see cref="GridViewStyleKey"/> in 
    /// the current theme.
    /// </returns>
    protected internal override object DefaultStyleKey => GridViewStyleKey;

    /// <summary>
    /// Gets the reference to the default style for the container of the data items in the <see cref="GridView"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="GridViewItemContainerStyleKey"/>. The default value is the <see cref="GridViewItemContainerStyleKey"/> 
    /// in the current theme.
    /// </returns>
    protected internal override object ItemContainerDefaultStyleKey => GridViewItemContainerStyleKey;

    #endregion

    //-------------------------------------------------------------------
    //
    //  Internal Methods
    //
    //-------------------------------------------------------------------

    #region Internal Methods

    // Propagate theme changes to contained headers
    internal override void OnThemeChanged()
    {
        if (_columns != null)
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].OnThemeChanged();
            }
        }
    }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Private Fields
    //
    //-------------------------------------------------------------------

    #region Private Fields

    private GridViewColumnCollection _columns;

    #endregion // Private Fields

    internal GridViewHeaderRowPresenter HeaderRowPresenter { get; set; }
}
