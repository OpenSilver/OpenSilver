// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.



#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class DataGridRow
    {
        private InternalTestHook _testHook;

        //Internal property to expose the TestHook object
        internal InternalTestHook TestHook
        {
            get
            {
                if (_testHook == null)
                {
                    _testHook = new InternalTestHook(this);
                }
                return _testHook;
            }
        }
        /// <summary>
        /// Test hook class that exposes internal and private members of the DataGridRow
        /// </summary>
        internal class InternalTestHook
        {
            //Reference to the outer 'parent' datagrid
            private DataGridRow _row;

            internal InternalTestHook(DataGridRow row)
            {
                _row = row;
            }

#region Internal Properties

            internal Rectangle BottomGridLine
            {
                get
                {
                    return _row._bottomGridLine;
                }
            }

            internal DataGridRowHeader HeaderCell
            {
                get
                {
                    return _row.HeaderCell;
                }
            }

            internal DataGridCellCollection Cells
            {
                get
                {
                    return _row.Cells;
                }
            }

            internal DataGridDetailsPresenter DetailsPresenter
            {
                get
                {
                    return _row._detailsElement;
                }
            }
#endregion
        }




    }
}
