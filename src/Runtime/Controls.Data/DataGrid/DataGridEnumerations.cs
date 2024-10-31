// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Defines modes that indicates how DataGrid content is copied to the Clipboard. 
    /// </summary>
    public enum DataGridClipboardCopyMode
    {
        /// <summary>
        /// Disable the DataGrid's ability to copy selected items as text.
        /// </summary>
        None,

        /// <summary>
        /// Enable the DataGrid's ability to copy selected items as text, but do not include
        /// the column header content as the first line in the text that gets copied to the Clipboard.
        /// </summary>
        ExcludeHeader,

        /// <summary>
        /// Enable the DataGrid's ability to copy selected items as text, and include
        /// the column header content as the first line in the text that gets copied to the Clipboard.
        /// </summary>
        IncludeHeader
    }

    /// <summary>
    /// Specifies constants that define what action was taken to end an edit.
    /// </summary>
    public enum DataGridEditAction
    {
        /// <summary>
        /// The edit was canceled.
        /// </summary>
        Cancel,

        /// <summary>
        /// The edit was committed.
        /// </summary>
        Commit
    }

    // Determines the location and visibility of the editing row.
    internal enum DataGridEditingRowLocation
    {
        Bottom = 0, // The editing row is collapsed below the displayed rows
        Inline = 1, // The editing row is visible and displayed
        Top = 2     // The editing row is collapsed above the displayed rows
    }

    /// <summary>
    /// Specifies constants that define which grid lines separating <see cref="DataGrid"/> inner cells are shown.
    /// </summary>
    [Flags]
    public enum DataGridGridLinesVisibility
    {
        /// <summary>
        /// No grid lines are shown.
        /// </summary>
        None = 0,
        /// <summary>
        /// Only horizontal grid lines, which separate rows, are shown.
        /// </summary>
        Horizontal = 1,
        /// <summary>
        /// Only vertical grid lines, which separate columns, are shown.
        /// </summary>
        Vertical = 2,
        /// <summary>
        /// Both horizontal and vertical grid lines are shown.
        /// </summary>
        All = 3,
    }

    /// <summary>
    /// Specifies constants that define whether editing is enabled on a cell level or on a row level.
    /// </summary>
    public enum DataGridEditingUnit
    {
        /// <summary>
        /// Cell editing is enabled.
        /// </summary>
        Cell = 0,
        /// <summary>
        /// Row editing is enabled.
        /// </summary>
        Row = 1,
    }

    /// <summary>
    /// Determines whether the row/column headers are shown or not.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [Flags]
    public enum DataGridHeadersVisibility
    {
        /// <summary>
        /// Show Row, Column, and Corner Headers
        /// </summary>
        All = Row | Column,

        /// <summary>
        /// Show only Column Headers with top-right corner Header
        /// </summary>
        Column = 0x01,

        /// <summary>
        /// Show only Row Headers with bottom-left corner
        /// </summary>
        Row = 0x02,

        /// <summary>
        /// Don’t show any Headers
        /// </summary>
        None = 0x00
    }

    /// <summary>
    /// Specifies constants that define when <see cref="DataGrid"/> row details are displayed.
    /// </summary>
    public enum DataGridRowDetailsVisibilityMode
    {
        /// <summary>
        /// The row details section is displayed only for selected rows.
        /// </summary>
        VisibleWhenSelected = 0,
        /// <summary>
        /// The row details section is displayed for all rows.
        /// </summary>
        Visible = 1,
        /// <summary>
        /// The row details section is not displayed for any rows.
        /// </summary>
        Collapsed = 2,
    }

    /// <summary>
    /// Determines the type of action to take when selecting items
    /// </summary>
    internal enum DataGridSelectionAction
    {
        AddCurrentToSelection,
        None,
        RemoveCurrentFromSelection,
        SelectCurrent,
        SelectFromAnchorToCurrent
    }

    /// <summary>
    /// Specifies constants that define the <see cref="DataGrid"/> selection modes.
    /// </summary>
    public enum DataGridSelectionMode
    {
        /// <summary>
        /// The user can select multiple items while holding down the SHIFT or CTRL keys.
        /// </summary>
        Extended = 0,
        /// <summary>
        /// The user can select only one item at a time.
        /// </summary>
        Single = 1
    }
}
