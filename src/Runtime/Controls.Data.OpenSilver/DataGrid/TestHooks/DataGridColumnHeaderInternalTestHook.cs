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
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    public partial class DataGridColumnHeader
    {
        private InternalTestHook _testHook;

        /// <summary>
        /// Internal property to expose the TestHook object
        /// </summary>
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
        /// Test hook class that exposes internal and private members of the DataGridColumnHeader
        /// </summary>
        internal class InternalTestHook
        {
            //Reference to the outer 'parent' DataGridColumnHeader
            private DataGridColumnHeader _columnHeader;

            /// <summary>
            /// Creates a TestHook for the given DataGridColumnHeader
            /// </summary>
            /// <param name="columnHeader">DataGridColumnHeader</param>
            internal InternalTestHook(DataGridColumnHeader columnHeader)
            {
                this._columnHeader = columnHeader;
            }

            #region Internal Methods

            /// <summary>
            /// Exposes the private OnLostMouseCapture method through the TestHook
            /// </summary>
            internal void OnLostMouseCapture()
            {
                this._columnHeader.OnLostMouseCapture();
            }

            /// <summary>
            /// Exposes the private OnMouseEnter method through the TestHook
            /// </summary>
            /// <param name="mousePosition">mouse position relative to the DataGridColumnHeader</param>
            internal void OnMouseEnter(Point mousePosition)
            {
                this._columnHeader.OnMouseEnter(mousePosition);
            }

            /// <summary>
            /// Exposes the private OnMouseLeave method through the TestHook
            /// </summary>
            internal void OnMouseLeave()
            {
                this._columnHeader.OnMouseLeave();
            }

            #endregion
        }
    }
}
