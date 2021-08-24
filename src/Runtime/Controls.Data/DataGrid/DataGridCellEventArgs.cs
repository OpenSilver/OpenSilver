// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
internal class DataGridCellEventArgs : EventArgs
    {
        internal DataGridCellEventArgs(DataGridCell dataGridCell)
        {
            Debug.Assert(dataGridCell != null);
            this.Cell = dataGridCell;
        }

        internal DataGridCell Cell
        {
            get;
            private set;
        }
    }
}
