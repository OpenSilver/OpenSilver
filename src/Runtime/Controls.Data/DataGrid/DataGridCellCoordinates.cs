// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
internal class DataGridCellCoordinates
    {
        public DataGridCellCoordinates(int columnIndex, int slot)
        {
            this.ColumnIndex = columnIndex;
            this.Slot = slot;
        }

        public DataGridCellCoordinates(DataGridCellCoordinates dataGridCellCoordinates) : this(dataGridCellCoordinates.ColumnIndex, dataGridCellCoordinates.Slot)
        { 
        }

        public int ColumnIndex
        {
            get;
            set;
        }

        public int Slot
        {
            get;
            set;
        }                
        
        public override bool Equals(object o)
        {
            DataGridCellCoordinates dataGridCellCoordinates = o as DataGridCellCoordinates;
            if (dataGridCellCoordinates != null)
            {
                return dataGridCellCoordinates.ColumnIndex == this.ColumnIndex && dataGridCellCoordinates.Slot == this.Slot;
            }
            return false;
        }

        // There is build warning if this is missiing
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

#if DEBUG
        public override string ToString()
        {
            return "DataGridCellCoordinates {ColumnIndex = " + this.ColumnIndex.ToString(CultureInfo.CurrentCulture) +
                   ", Slot = " + this.Slot.ToString(CultureInfo.CurrentCulture) + "}";
        }
#endif
    }
}
