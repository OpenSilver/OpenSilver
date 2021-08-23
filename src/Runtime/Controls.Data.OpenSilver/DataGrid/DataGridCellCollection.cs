// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
internal class DataGridCellCollection
    {
        #region Data

        private List<DataGridCell> _cells;
        private DataGridRow _owningRow;

        #endregion Data

        #region Events

        internal event EventHandler<DataGridCellEventArgs> CellAdded;
        internal event EventHandler<DataGridCellEventArgs> CellRemoved;

        #endregion Events

        public DataGridCellCollection(DataGridRow owningRow)
        {
            this._owningRow = owningRow;
            this._cells = new List<DataGridCell>();
        }

        public int Count
        {
            get
            {
                return this._cells.Count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this._cells.GetEnumerator();
        }

        public void Insert(int cellIndex, DataGridCell cell)
        {
            Debug.Assert(cellIndex >= 0 && cellIndex <= this._cells.Count);
            Debug.Assert(cell != null);

            cell.OwningRow = this._owningRow;
            this._cells.Insert(cellIndex, cell);

            if (CellAdded != null)
            {
                CellAdded(this, new DataGridCellEventArgs(cell));
            }
        }

        public void RemoveAt(int cellIndex)
        {
            DataGridCell dataGridCell = this._cells[cellIndex];
            this._cells.RemoveAt(cellIndex);
            dataGridCell.OwningRow = null;
            if (CellRemoved != null)
            {
                CellRemoved(this, new DataGridCellEventArgs(dataGridCell));
            }
        }

        public DataGridCell this[int index]
        {
            get
            {
                if (index < 0 || index >= this._cells.Count)
                {
                    throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, true, this._cells.Count, false);
                }
                return this._cells[index];
            }
        }
    }
}
