// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
public sealed partial class DataGridCell
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
        /// Test hook class that exposes internal and private members of the DataGridCell
        /// </summary>
        internal class InternalTestHook
        {
            //Reference to the outer 'parent' DataGridCell
            private DataGridCell _cell;

            internal InternalTestHook(DataGridCell cell)
            {
                _cell = cell;
            }

            #region Internal Properties
            internal Rectangle RightGridLine
            {
                get
                {
                    return _cell._rightGridLine;
                }
            }
            #endregion
        }
    }
}
