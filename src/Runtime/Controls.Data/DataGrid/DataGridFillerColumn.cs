// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
internal class DataGridFillerColumn : DataGridColumn
    {
        public DataGridFillerColumn(DataGrid owningGrid)
        {
            this.IsReadOnly = true;
            this.OwningGrid = owningGrid;
            this.MinWidth = 0;
            this.MaxWidth = int.MaxValue;
        }

        #region Properties

        internal double FillerWidth
        {
            get;
            set;
        }

        // True if there is room for the filler column; otherwise, false
        internal bool IsActive
        {
            get
            {
                return this.FillerWidth > 0;
            }
        }

        // True if the FillerColumn's header cell is contained in the visual tree
        internal bool IsRepresented
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        internal override DataGridColumnHeader CreateHeader()
        {
            DataGridColumnHeader headerCell = base.CreateHeader();
            if (headerCell != null)
            {
                headerCell.IsEnabled = false;
            }
            return headerCell;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return null;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return null;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            Debug.Assert(false);
            return null;
        }

        #endregion Methods
    }
}
