// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// This class encapsulates a selected row's information necessary for the CopyingRowClipboardContent event.
    /// </summary>
    public class DataGridRowClipboardEventArgs : EventArgs
    {
        #region Data

        private List<DataGridClipboardCellContent> _clipboardRowContent;
        private bool _isColumnHeadersRow;
        private object _item;

        #endregion Data

        #region Constructor

        /// <summary>
        /// Creates a DataGridRowClipboardEventArgs object and initializes the properties.
        /// </summary>
        /// <param name="item">The row's associated data item.</param>
        /// <param name="isColumnHeadersRow">Whether or not this EventArgs is for the column headers.</param>
        internal DataGridRowClipboardEventArgs(object item, bool isColumnHeadersRow)
        {
            this._isColumnHeadersRow = isColumnHeadersRow;
            this._item = item;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// This list should be used to modify, add ot remove a cell content before it gets stored into the clipboard.
        /// </summary>
        public List<DataGridClipboardCellContent> ClipboardRowContent
        {
            get
            {
                if (this._clipboardRowContent == null)
                {
                    this._clipboardRowContent = new List<DataGridClipboardCellContent>();
                }
                return this._clipboardRowContent;
            }
        }

        /// <summary>
        /// This property is true when the ClipboardRowContent represents column headers, in which case the Item is null.
        /// </summary>
        public bool IsColumnHeadersRow
        {
            get
            {
                return this._isColumnHeadersRow;
            }
        }

        /// <summary>
        /// DataGrid row item used for proparing the ClipboardRowContent.
        /// </summary>
        public object Item
        {
            get
            {
                return this._item;
            }
        }

        #endregion Public Properties
    }
}
