// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenSilver.Internal;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Markup;

namespace System.Windows.Controls;

/// <summary>
/// Represents a column that displays data.
/// </summary>
[ContentProperty(nameof(Header))]
[StyleTypedProperty(Property = nameof(HeaderContainerStyle), StyleTargetType = typeof(GridViewColumnHeader))]
public class GridViewColumn : DependencyObject, INotifyPropertyChanged
{
    //-------------------------------------------------------------------
    //
    //  Constructors
    //
    //-------------------------------------------------------------------

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GridViewColumn"/> class.
    /// </summary>
    public GridViewColumn()
    {
        ResetPrivateData();

        // Descendant of this class can override the metadata to give it
        // a value other than NaN and without trigger the propertychange
        // callback and thus, result in _state be out-of-sync with the
        // Width property.
        _state = double.IsNaN(Width) ? ColumnMeasureState.Init : ColumnMeasureState.SpecificWidth;
    }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Public Methods
    //
    //-------------------------------------------------------------------

    #region Public Methods

    /// <summary>
    /// Creates a string representation of the <see cref="GridViewColumn"/>.
    /// </summary>
    /// <returns>
    /// A string that identifies the object as a <see cref="GridViewColumn"/> object and displays the 
    /// value of the <see cref="Header"/> property.
    /// </returns>
    public override string ToString()
    {
        return string.Format(Strings.ToStringFormatString_GridViewColumn, GetType(), Header);
    }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Public Properties
    //
    //-------------------------------------------------------------------

    #region Public Properties

    // For all the DPs on GridViewColumn, null is treated as unset,
    // because it's impossible to distinguish null and unset.
    // Change a property between null and unset, PropertyChangedCallback will not be called.

    #region Header

    /// <summary>
    /// Identifies the <see cref="Header"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(GridViewColumn),
            new FrameworkPropertyMetadata(OnHeaderChanged));

    /// <summary>
    /// Gets or sets the content of the header of a <see cref="GridViewColumn"/>.
    /// </summary>
    /// <returns>
    /// The object to use for the column header. The default is null.
    /// </returns>
    public object Header
    {
        get { return GetValue(HeaderProperty); }
        set { SetValueInternal(HeaderProperty, value); }
    }

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GridViewColumn c = (GridViewColumn)d;
        c.OnPropertyChanged(HeaderProperty.Name);
    }

    #endregion Header

    #region HeaderContainerStyle

    /// <summary>
    /// Identifies the <see cref="HeaderContainerStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderContainerStyleProperty =
        DependencyProperty.Register(
            nameof(HeaderContainerStyle),
            typeof(Style),
            typeof(GridViewColumn),
            new FrameworkPropertyMetadata(OnHeaderContainerStyleChanged));

    /// <summary>
    /// Gets or sets the style to use for the header of the <see cref="GridViewColumn"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="Style"/> that defines the display properties for the column header. The default is null.
    /// </returns>
    public Style HeaderContainerStyle
    {
        get { return (Style)GetValue(HeaderContainerStyleProperty); }
        set { SetValueInternal(HeaderContainerStyleProperty, value); }
    }

    private static void OnHeaderContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GridViewColumn c = (GridViewColumn)d;
        c.OnPropertyChanged(HeaderContainerStyleProperty.Name);
    }

    #endregion HeaderContainerStyle

    #region HeaderTemplate

    /// <summary>
    /// Identifies the <see cref="HeaderTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(
            nameof(HeaderTemplate),
            typeof(DataTemplate),
            typeof(GridViewColumn),
            new FrameworkPropertyMetadata(OnHeaderTemplateChanged));

    /// <summary>
    /// Gets or sets the template to use to display the content of the column header.
    /// </summary>
    /// <returns>
    /// A <see cref="DataTemplate"/> to use to display the column header. The default is null.
    /// </returns>
    public DataTemplate HeaderTemplate
    {
        get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        set { SetValueInternal(HeaderTemplateProperty, value); }
    }

    private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GridViewColumn c = (GridViewColumn)d;
        c.OnPropertyChanged(HeaderTemplateProperty.Name);
    }

    #endregion  HeaderTemplate

    #region HeaderTemplateSelector

    /// <summary>
    /// Identifies the <see cref="HeaderTemplateSelector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty =
        DependencyProperty.Register(
            nameof(HeaderTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(GridViewColumn),
            new FrameworkPropertyMetadata(OnHeaderTemplateSelectorChanged));

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> that provides logic to select the template 
    /// to use to display the column header.
    /// </summary>
    /// <returns>
    /// The <see cref="DataTemplateSelector"/> object that provides data template selection for each 
    /// <see cref="GridViewColumn"/>. The default is null.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataTemplateSelector HeaderTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
        set { SetValueInternal(HeaderTemplateSelectorProperty, value); }
    }

    private static void OnHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GridViewColumn c = (GridViewColumn)d;
        c.OnPropertyChanged(HeaderTemplateSelectorProperty.Name);
    }

    #endregion HeaderTemplateSelector

    #region HeaderStringFormat

    /// <summary>
    /// Identifies the <see cref="HeaderStringFormat"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderStringFormatProperty =
        DependencyProperty.Register(
            nameof(HeaderStringFormat),
            typeof(string),
            typeof(GridViewColumn),
            new FrameworkPropertyMetadata(null, OnHeaderStringFormatChanged));

    /// <summary>
    /// Gets or sets a composite string that specifies how to format the <see cref="Header"/> 
    /// property if it is displayed as a string.
    /// </summary>
    /// <returns>
    /// A composite string that specifies how to format the <see cref="Header"/> property if it 
    /// is displayed as a string. The default is null.
    /// </returns>
    public string HeaderStringFormat
    {
        get { return (string)GetValue(HeaderStringFormatProperty); }
        set { SetValueInternal(HeaderStringFormatProperty, value); }
    }

    /// <summary>
    ///     Called when HeaderStringFormatProperty is invalidated on "d."
    /// </summary>
    private static void OnHeaderStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GridViewColumn ctrl = (GridViewColumn)d;
        ctrl.OnHeaderStringFormatChanged((string)e.OldValue, (string)e.NewValue);
    }

    /// <summary>
    /// Occurs when the <see cref="HeaderStringFormat"/> property changes.
    /// </summary>
    /// <param name="oldHeaderStringFormat">
    /// The old value of the <see cref="HeaderStringFormat"/> property.
    /// </param>
    /// <param name="newHeaderStringFormat">
    /// The new value of the <see cref="HeaderStringFormat"/> property.
    /// </param>
    protected virtual void OnHeaderStringFormatChanged(string oldHeaderStringFormat, string newHeaderStringFormat)
    {
    }

    #endregion HeaderStringFormat

    #region DisplayMemberBinding

    /// <summary>
    /// Gets or sets the data item to bind to for this column.
    /// </summary>
    /// <returns>
    /// The specified data item type that displays in the column. The default is null.
    /// </returns>
    public BindingBase DisplayMemberBinding
    {
        get { return _displayMemberBinding; }
        set
        {
            if (_displayMemberBinding != value)
            {
                _displayMemberBinding = value;
                OnDisplayMemberBindingChanged();
            }
        }
    }

    private BindingBase _displayMemberBinding;

    /// <summary>
    /// If DisplayMemberBinding property changed, NotifyPropertyChanged event will be raised with this string.
    /// </summary>
    internal const string c_DisplayMemberBindingName = "DisplayMemberBinding";

    private void OnDisplayMemberBindingChanged()
    {
        OnPropertyChanged(c_DisplayMemberBindingName);
    }

    #endregion

    #region CellTemplate

    /// <summary>
    /// Identifies the <see cref="CellTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CellTemplateProperty =
        DependencyProperty.Register(
            nameof(CellTemplate),
            typeof(DataTemplate),
            typeof(GridViewColumn),
            new PropertyMetadata(OnCellTemplateChanged));

    /// <summary>
    /// Gets or sets the template to use to display the contents of a column cell.
    /// </summary>
    /// <returns>
    /// A <see cref="DataTemplate"/> that is used to format a column cell. The default is null.
    /// </returns>
    public DataTemplate CellTemplate
    {
        get { return (DataTemplate)GetValue(CellTemplateProperty); }
        set { SetValueInternal(CellTemplateProperty, value); }
    }

    private static void OnCellTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GridViewColumn c = (GridViewColumn)d;
        c.OnPropertyChanged(CellTemplateProperty.Name);
    }

    #endregion

    #region CellTemplateSelector

    /// <summary>
    /// Identifies the <see cref="CellTemplateSelector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CellTemplateSelectorProperty =
        DependencyProperty.Register(
            nameof(CellTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(GridViewColumn),
            new PropertyMetadata(OnCellTemplateSelectorChanged));

    /// <summary>
    /// Gets or sets a <see cref="DataTemplateSelector"/> that determines the template to use to display 
    /// cells in a column.
    /// </summary>
    /// <returns>
    /// A <see cref="DataTemplateSelector"/> that provides <see cref="DataTemplate"/> selection for column 
    /// cells. The default is null.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataTemplateSelector CellTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
        set { SetValueInternal(CellTemplateSelectorProperty, value); }
    }

    private static void OnCellTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GridViewColumn c = (GridViewColumn)d;
        c.OnPropertyChanged(CellTemplateSelectorProperty.Name);
    }

    #endregion

    #region Width

    /// <summary>
    /// Identifies the <see cref="Width"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty =
        FrameworkElement.WidthProperty.AddOwner(
            typeof(GridViewColumn),
            new PropertyMetadata(double.NaN, OnWidthChanged));

    /// <summary>
    /// Gets or sets the width of the column.
    /// </summary>
    /// <returns>
    /// The width of the column. The default is <see cref="double.NaN"/>, which automatically sizes to 
    /// the largest column item that is not the column header.
    /// </returns>
    [TypeConverter(typeof(LengthConverter))]
    public double Width
    {
        get { return (double)GetValue(WidthProperty); }
        set { SetValueInternal(WidthProperty, value); }
    }

    private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        GridViewColumn c = (GridViewColumn)d;

        double newWidth = (double)e.NewValue;

        // reset DesiredWidth if width is set to auto
        c.State = Double.IsNaN(newWidth) ? ColumnMeasureState.Init : ColumnMeasureState.SpecificWidth;

        c.OnPropertyChanged(WidthProperty.Name);
    }

    #endregion

    #region ActualWidth

    /// <summary>
    /// Gets the actual width of a <see cref="GridViewColumn"/>.
    /// </summary>
    /// <returns>
    /// The current width of the column. The default is zero (0.0).
    /// </returns>
    public double ActualWidth
    {
        get { return _actualWidth; }

        private set
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0.0)
            {
                Debug.Fail("Invalid value for ActualWidth.");
            }
            else if (_actualWidth != value)
            {
                _actualWidth = value;
                OnPropertyChanged(c_ActualWidthName);
            }
        }
    }

    #endregion

    #endregion Public Properties

    #region INotifyPropertyChanged

    /// <summary>
    /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
    /// </summary>
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add
        {
            _propertyChanged += value;
        }
        remove
        {
            _propertyChanged -= value;
        }
    }

    private event PropertyChangedEventHandler _propertyChanged;

    #endregion INotifyPropertyChanged

    //-------------------------------------------------------------------
    //
    //  Protected Methods
    //
    //-------------------------------------------------------------------

    #region Protected Methods

    /// <summary>
    /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </summary>
    /// <param name="e">
    /// The event data.
    /// </param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_propertyChanged != null)
        {
            _propertyChanged(this, e);
        }
    }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Internal Methodes
    //
    //-------------------------------------------------------------------

    #region Internal Methodes

    // Propagate theme changes to contained headers
    internal void OnThemeChanged()
    {
        if (Header is FrameworkElement fe)
        {
            TreeWalkHelper.InvalidateOnResourcesChange(fe, ResourcesChangeInfo.ThemeChangeInfo);
        }
    }

    /// <summary>
    /// ensure final column width is no less than a value
    /// </summary>
    internal double EnsureWidth(double width)
    {
        if (width > DesiredWidth)
        {
            DesiredWidth = width;
        }
        return DesiredWidth;
    }

    /// <summary>
    /// column collection should call this when remove a column from the collection.
    /// </summary>
    internal void ResetPrivateData()
    {
        _actualIndex = -1;
        _desiredWidth = 0.0;
        _state = double.IsNaN(Width) ? ColumnMeasureState.Init : ColumnMeasureState.SpecificWidth;
    }

    #endregion

    //-------------------------------------------------------------------
    //
    //  Internal Properties
    //
    //-------------------------------------------------------------------

    #region Internal Properties

    /// <summary>
    ///  Reachable State Transition Diagram:
    ///
    ///                        +- - - - - - - - - - +
    ///                        |       Init         |
    ///                        +- - - - - - - - - - +
    ///                           / /|   A   |\ \
    ///                          / /     |     \ \
    ///                         / /      |      \ \
    ///                        / /       |       \ \
    ///                       / /        |        \ \
    ///                      / /         |         \ \
    ///                     / /          |          \ \
    ///                   |/ /           |           \ \|
    ///    +--------------------+        |        +--------------------+
    ///    |      Headered      |--------+------->|        Data        |
    ///    +--------------------+        |        +--------------------+
    ///                      \           |           /
    ///                       \          |          /
    ///                        \         |         /
    ///                         \        |        /
    ///                          \       |       /
    ///                           \      |      /
    ///                            \|    |    |/
    ///                        +--------------------+
    ///                        |   SpecificWidth    |
    ///                        +--------------------+
    ///
    /// Note:
    ///
    /// 1) Init is a intermidiated state, that is a column should not stop on such a state;
    /// 2) Headered, Data and SpecificWidth are terminal state, that is a column can stop at
    ///     the state if no further data change / user interaction to trigger a change.
    ///
    /// Typical state transiton flows:
    ///
    ///   Case 1: column is auto, LV has header and data
    ///     Init --> [ Headered --> ] Data
    ///
    ///   Case 2: column is auto, LV has header but no data
    ///     Init --> Headered
    ///
    ///   Case 3: column has a specified width
    ///     SpecificWidth
    ///
    ///   Case 4: couble click a column of case 3
    ///     SpecificWidth --> Init --> Headered / Data (depends on the data)
    ///
    ///   Case 5: resize a column which has width as auto
    ///     Headered / Data --> SpecificWidth
    ///
    /// </summary>
    internal ColumnMeasureState State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;

                if (value != ColumnMeasureState.Init) // Headered, Data or SpecificWidth
                {
                    UpdateActualWidth();
                }
                else
                {
                    DesiredWidth = 0.0;
                }
            }
            else if (value == ColumnMeasureState.SpecificWidth)
            {
                UpdateActualWidth();
            }
        }
    }

    // NOTE: Perf optimization. To avoid re-search index again and again
    // by every GridViewRowPresenter, add an index here.
    internal int ActualIndex
    {
        get { return _actualIndex; }
        set { _actualIndex = value; }
    }

    /// <summary>
    /// Minimum width requirement for this column. Shared by all visible cells in this column
    /// </summary>
    /// <remarks>
    /// Below table shows an example of how column width is shared:
    ///
    ///     1. In the first round of layout, DesiredWidth continue to grow when each row comes into measure
    ///
    ///     2. after the 1st round, the desired width for this column is decided, each row on layout updated
    ///         with check this value with its copy of maxDesiredWidth, if not equal, triger another round of
    ///         measure.
    ///
    ///     3. after 2nd round of layout, all rows should be in same size.
    ///     +------------+-----------+--------------+------------+------------+-------------+
    ///     |            |   Width   |    Cell      |  Desired   | Presenter  |   Column    |
    ///     |            |           | DesiredWidth |   Width    | LocalCopy  |    State    |
    ///     |------------+-----------+--------------+------------+------------|-------------|
    ///     | 1st round  |   NaN     |              |    10.0    |            |    Init     |
    ///     |            |           |              |            |            |             |
    ///     |  (row 1)   |           |    12.0      |    12.0    |            |             |
    ///     |  (row 2)   |           |    70.0      |    70.0    |            |             |
    ///     |  (row 3)   |           |    80.0      |    80.0    |            |             |
    ///     |  (row 4)   |           |    60.0      |    80.0    |            |             |
    ///     |------------+-----------+--------------+------------+------------|-------------|
    ///     | layout     |   NaN     |              |            |            |             |
    ///     | updated    |           |              |            |            |             |
    ///     |            |           |              |            |            |             |
    ///     | [hdr_row]  |           |              |            |            | [Headered]* |
    ///     |            |           |              |            |            |             |
    ///     |  (row 1)   |           |              |    80.0    |    12.0    |    Data     |
    ///     |  (row 2)   |           |              |    80.0    |    70.0    |             |
    ///     |  (row 3)   |           |              |    80.0    |    80.0    |             |
    ///     |  (row 4)   |           |              |    80.0    |    80.0    |             |
    ///     |------------+-----------+--------------+------------+------------|-------------|
    ///     | 2nd round  |   NaN     |              |            |            |             |
    ///     |            |           |              |            |            |             |
    ///     |  (row 1)   |           |    12.0      |    80.0    |    80.0    |             |
    ///     |  (row 2)   |           |    70.0      |    80.0    |    80.0    |             |
    ///     +------------+-----------+--------------+------------+------------+-------------+
    ///
    ///   * Depends on the tree structure, it is possible that HeaderRowPresenter accomplish first
    ///     layout first. So the column state can be Headered for a while. But will be changed to
    ///     'Data' once a data row accomplish its first layout.
    ///
    /// </remarks>
    internal double DesiredWidth
    {
        get { return _desiredWidth; }
        private set { _desiredWidth = value; }
    }

    internal const string c_ActualWidthName = "ActualWidth";

    #endregion

    //-------------------------------------------------------------------
    //
    //  Private Methods / Fields
    //
    //-------------------------------------------------------------------

    #region Private Methods

    /// <summary>
    /// Helper to raise INotifyPropertyChanged.PropertyChanged event
    /// </summary>
    /// <param name="propertyName">Name of the changed property</param>
    private void OnPropertyChanged(string propertyName)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// force ActualWidth to be reevaluated
    /// </summary>
    private void UpdateActualWidth()
    {
        ActualWidth = (State == ColumnMeasureState.SpecificWidth) ? Width : DesiredWidth;
    }

    #endregion

    #region Private Fields

    private double _desiredWidth;
    private int _actualIndex;
    private double _actualWidth;
    private ColumnMeasureState _state;

    #endregion
}

/// <summary>
/// States of column when doing layout
/// See GridViewColumn.State for reachable state transition diagram
/// </summary>
internal enum ColumnMeasureState
{
    /// <summary>
    /// Column width is just initialized and will size to content width
    /// </summary>
    Init = 0,

    /// <summary>
    /// Column width reach max desired width of header(s) in this column
    /// </summary>
    Headered = 1,

    /// <summary>
    /// Column width reach max desired width of data row(s) in this column
    /// </summary>
    Data = 2,

    /// <summary>
    /// Column has a specific value as width
    /// </summary>
    SpecificWidth = 3
}
