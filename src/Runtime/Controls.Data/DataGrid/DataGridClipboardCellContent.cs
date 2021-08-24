// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Runtime.InteropServices;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// This structure encapsulate the cell information necessary when clipboard content is prepared.
    /// </summary>
    public struct DataGridClipboardCellContent
    {
        #region Data

        private DataGridColumn _column;
        private object _content;
        private object _item;

        #endregion Data

        #region Constructors

        /// <summary>
        /// Creates a new DataGridClipboardCellValue structure containing information about a DataGrid cell.
        /// </summary>
        /// <param name="item">DataGrid row item containing the cell.</param>
        /// <param name="column">DataGridColumn containing the cell.</param>
        /// <param name="content">DataGrid cell value.</param>
        public DataGridClipboardCellContent(object item, DataGridColumn column, object content)
        {
            this._item = item;
            this._column = column;
            this._content = content;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// DataGridColumn containing the cell.
        /// </summary>
        public DataGridColumn Column
        {
            get
            {
                return _column;
            }
        }

        /// <summary>
        /// Cell content.
        /// </summary>
        public object Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// DataGrid row item containing the cell.
        /// </summary>
        public object Item
        {
            get
            {
                return _item;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Field-by-field comparison to avoid reflection-based ValueType.Equals.
        /// </summary>
        /// <param name="obj">DataGridClipboardCellContent to compare.</param>
        /// <returns>True iff this and data are equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is DataGridClipboardCellContent))
            {
                return false;
            }
            DataGridClipboardCellContent clipboardCellContent = (DataGridClipboardCellContent)obj;
            return (((this._column == clipboardCellContent._column) && (this._content == clipboardCellContent._content)) && (this._item == clipboardCellContent._item));
        }

        /// <summary>
        /// Returns a deterministic hash code.
        /// </summary>
        /// <returns>Hash value.</returns>
        public override int GetHashCode()
        {
            return ((this._column.GetHashCode() ^ this._content.GetHashCode()) ^ this._item.GetHashCode());
        }

        /// <summary>
        /// Field-by-field comparison to avoid reflection-based ValueType.Equals.
        /// </summary>
        /// <param name="clipboardCellContent1">The first DataGridClipboardCellContent.</param>
        /// <param name="clipboardCellContent2">The second DataGridClipboardCellContent.</param>
        /// <returns>True iff clipboardCellContent1 and clipboardCellContent2 are equal.</returns>
        public static bool operator ==(DataGridClipboardCellContent clipboardCellContent1, DataGridClipboardCellContent clipboardCellContent2)
        {
            return (((clipboardCellContent1._column == clipboardCellContent2._column) && (clipboardCellContent1._content == clipboardCellContent2._content)) && (clipboardCellContent1._item == clipboardCellContent2._item));
        }

        /// <summary>
        /// Field-by-field comparison to avoid reflection-based ValueType.Equals.
        /// </summary>
        /// <param name="clipboardCellContent1">The first DataGridClipboardCellContent.</param>
        /// <param name="clipboardCellContent2">The second DataGridClipboardCellContent.</param>
        /// <returns>True iff clipboardCellContent1 and clipboardCellContent2 are NOT equal.</returns>
        public static bool operator !=(DataGridClipboardCellContent clipboardCellContent1, DataGridClipboardCellContent clipboardCellContent2)
        {
            if ((clipboardCellContent1._column == clipboardCellContent2._column) && (clipboardCellContent1._content == clipboardCellContent2._content))
            {
                return (clipboardCellContent1._item != clipboardCellContent2._item);
            }
            return true;
        }

        #endregion Public Methods
    }
}
