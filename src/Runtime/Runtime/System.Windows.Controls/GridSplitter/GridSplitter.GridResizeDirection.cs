// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows
    /// of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class GridSplitter : Control
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal enum GridResizeDirection
        {
            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            Auto,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            Columns,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            Rows
        }
    }
}